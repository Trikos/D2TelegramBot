using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace DiabloBot
{
    class Gator
    {
        enum ELevel : long
        {
            novanta = 1618470619,
            novantuno = 1764543065,
            novantadue = 1923762030,
            novantatre = 2097310703,
            novantaquattro = 2286478756,
            novantacinque = 2492671933,
            novantasei = 2717422497,
            novantasette = 2962400612,
            novantotto = 3229426756,
            novantanove = 3520485254
        }

        public TelegramBotClient bot;
        public Data data;
        public DiabloChecker diabloChecker;
        public Keyboards keyboards = new Keyboards();
        public Profile profile;
        public bool isFirstMessageArrived = false;
        public int seconds = 300000; // Default 5 mins. 1 sec => 1000
        public readonly int minute = 60000;

        public long UserId { get; set; }

        public async Task ExecuterAsync()
        {
            //Init user data
            data = new Data();            

            bot = data.Initialize();            

            bot.OnMessage += BotOnMessageReceived;
            bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            bot.StartReceiving(Array.Empty<UpdateType>());

            if (data.UserChatId <= 0)
            {
                // Wait until user text me
                while (!isFirstMessageArrived)
                {
                    Console.WriteLine("Waiting for message...");
                    await Task.Delay(5000);
                }
                //Save the new data in the config.json
                SaveOnFile();
            }

            //Init Profile            
            //Create List of Schedule and get all schedule existing
            List<Profile> profiles = new List<Profile>();

            int counter = 0;
            string line;

            // Read the file and store it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader(data.PathToJsonProfile);
            while ((line = file.ReadLine()) != null)
            {
                profiles.Add(JsonConvert.DeserializeObject<Profile>(line));
                counter++;
            }

            file.Close();
            //profile = JsonConvert.DeserializeObject<Profile>(File.ReadAllText(data.PathToJsonProfile));

            //diabloChecker = new DiabloChecker(data);
            //diabloChecker.ReadStatus();
            //Start updating every X minutes
            Update();                        
        }
        
        private void Update()
        {
            diabloChecker = new DiabloChecker(data);
            Dictionary<string, long> levels = GetAllLevels();            

            //Get number of picked up objects lines
            int currentLine = File.ReadLines(data.PathToItemLog).Count();

            //Get number of picked up objects images
            int fileInFolder = Directory.GetFiles(data.PathImages).Length;

            //Start Task and every minutes read and update the status            
            Task task = new Task(async () =>
            {                
                //Leggo stato prima volta per inizializzare
                diabloChecker.ReadStatus();
                //Loop for item, exp, death
                while (true)
                {                    
                    //Controllo se ci sono aggiornamenti
                    Tuple<string, bool> WasUpdated = diabloChecker.IsUpdated();
                    if (WasUpdated.Item2)
                    {
                        switch (WasUpdated.Item1)
                        {
                            case "EXP":
                                Experience(diabloChecker, levels);
                                break;
                            case "DEATH":
                                Death(diabloChecker);
                                break;
                            case "LVLUP":
                                LevelUp(diabloChecker);
                                break;
                        }
                    }
                    int readLine = File.ReadLines(data.PathToItemLog).Count();
                    //Se si son aggiunte righe
                    if (currentLine < readLine)
                    {
                        currentLine = readLine;
                        string lastLine = File.ReadLines(data.PathToItemLog).Last();
                        string message = "I picked an object:\n" + lastLine;
                        //Invio il messaggio assieme alla foto usando il bot
                        await bot.SendTextMessageAsync(data.UserChatId, message);

                        var directory = new DirectoryInfo(data.PathImages);
                        //var myFile = (from f in directory.GetFiles() orderby f.LastWriteTime descending select f).First();
                        var myFile = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
                        string imagePath = myFile.Directory.ToString() + "\\" + myFile.ToString();

                        //Send image of object found
                        using (Stream stream = System.IO.File.OpenRead(imagePath))
                        {
                            if (stream != null && stream.Length > 0)
                            {
                                await bot.SendPhotoAsync(
                                chatId: data.UserChatId,
                                photo: stream
                                );
                            }
                            else
                                await bot.SendTextMessageAsync(data.UserChatId, "Corrupted image");
                        }
                        Console.WriteLine("Item Drop/Pick");
                    }
                    else if (currentLine > readLine) currentLine = readLine;                    
                    Thread.Sleep(seconds);
                    Console.WriteLine("Current timer: " + seconds);
                }
            });
            task.Start();
            Console.Clear();
            Console.WriteLine("Running...");
            Console.ReadLine();
        }              

        private void SaveOnFile()
        {
            // Create a string from json
            string line = JsonConvert.SerializeObject(data);

            // Set a variable to the Documents path.
            string directory = AppDomain.CurrentDomain.BaseDirectory;

            // Write the string to a new file
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(directory, "config.json")))
            {
                outputFile.WriteLine(line);
                outputFile.Close();
            }
        }

        private static decimal Remap(decimal value, decimal fromSource, decimal toSource, decimal fromTarget, decimal toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }

        //TODO Add remaining levels in a more efficient way
        private Dictionary<string, long> GetAllLevels()
        {
            Dictionary<string, long> tmp = new Dictionary<string, long>();

            tmp.Add("90", (long)ELevel.novanta);
            tmp.Add("91", (long)ELevel.novantuno);
            tmp.Add("92", (long)ELevel.novantadue);
            tmp.Add("93", (long)ELevel.novantatre);
            tmp.Add("94", (long)ELevel.novantaquattro);
            tmp.Add("95", (long)ELevel.novantacinque);
            tmp.Add("96", (long)ELevel.novantasei);
            tmp.Add("97", (long)ELevel.novantasette);
            tmp.Add("98", (long)ELevel.novantotto);
            tmp.Add("99", (long)ELevel.novantanove);
            return tmp;
        }

        private async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;
            string currentTimer = "Current timer: ";
            // When Click on Inline Keyboard this method will get call
            switch (callbackQuery.Data)
            {
                //TODO Fix start and stop for different cd keys
                case "/stopbot":
                    //InitializeBotShutdownProcedure();
                    Console.WriteLine("Bot Shutdown");
                    break;                
                case "/startbot":
                    Console.WriteLine("Bot Starting...");
                    //InitializeBotResumeProcedure();
                    Console.WriteLine("Bot Started");                                    
                    break;
                case "/time":                
                    await bot.EditMessageTextAsync(
                        callbackQuery.Message.Chat.Id, 
                        callbackQuery.Message.MessageId,
                        "Choose how often to receive updates (minutes)",
                        replyMarkup: keyboards.TimeIK());
                    break;
                case "/5min":
                    seconds = minute * 5;
                    await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"5 minutes set");
                    Console.WriteLine(currentTimer + seconds);
                    break;
                case "/10min":
                    seconds = minute * 10;
                    await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"10 minutes set");
                    Console.WriteLine(currentTimer + seconds);
                    break;
                case "/15min":
                    seconds = minute * 15;
                    await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"15 minutes set");
                    Console.WriteLine(currentTimer + seconds);
                    break;
                case "/20min":
                    seconds = minute * 20;
                    await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"20 minutes set");
                    Console.WriteLine(currentTimer + seconds);
                    break;
                case "/30min":
                    seconds = minute * 30;
                    await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"30 minutes set");
                    Console.WriteLine(currentTimer + seconds);
                    break;
                case "/60min":
                    seconds = minute * 60;
                    await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"60 minutes set");
                    Console.WriteLine(currentTimer + seconds);
                    break;
                case "/options":
                    //await bot.EditMessageReplyMarkupAsync(
                    //    callbackQuery.Message.Chat.Id,
                    //    callbackQuery.Message.MessageId,                        
                    //    keyboards.OptionsIK());
                    await bot.EditMessageTextAsync(
                        callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId,                        
                        "Options menu",
                        replyMarkup: keyboards.OptionsIK());
                    break;
                case "/clearconsole":
                    Console.Clear();
                    Console.WriteLine("Running...");
                    await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"WIN PROMPT CLEARED");
                    break;
                case "/screenshot":
                    //TODO Bot will send a screen of your desktop
                    //Could be not safe so im not sure If I will implement it
                    break;
                case "/mainmenu":
                    await bot.EditMessageTextAsync(
                        callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId,
                        "Main Menu",
                        replyMarkup: keyboards.MainMenuIK());
                    break;
                case "/difficulty":
                    NotImplementedYet(callbackQuery);
                    break;
                case "/realm":
                    NotImplementedYet(callbackQuery);
                    break;
                case "/gameinfo":
                    NotImplementedYet(callbackQuery);
                    break;
                case "/param":
                    NotImplementedYet(callbackQuery);
                    break;
                case "/entryscript":
                    NotImplementedYet(callbackQuery);
                    break;
                case "/character":
                    NotImplementedYet(callbackQuery);
                    break;
                case "/status":
                    await bot.EditMessageTextAsync(
                        callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId,
                        "*STATUS*",
                        parseMode: ParseMode.Markdown,
                        replyMarkup: keyboards.BotStatusIK(diabloChecker.botStatus));
                    break;
                default:
                    data.UserChatId = callbackQuery.From.Id;                    
                    await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"NO COMMAND FOUND");
                    break;
            }
            
            //await bot.EditMessageReplyMarkupAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, replyKeyboard);
            // This is a pop sent to user
            //await bot.AnswerCallbackQueryAsync(
            //    callbackQuery.Id,
            //    $"POPUP Received {callbackQuery.Data}");
            // This is a message sent to user
            //await Bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Received {callbackQuery.Data}");
        }

        private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            string[] args = message.Text.Split(' ');
            switch (args[0].ToLower())
            {
                case "/start":
                    isFirstMessageArrived = true;
                    data.UserChatId = message.Chat.Id;                    
                    await bot.SendTextMessageAsync(message.Chat.Id, 
                        "Hi, Im your D2 butler\n" +
                        "If you lose the Main Menu just text me /menu and I pop up again");
                    await bot.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        "Main menu",
                        replyMarkup: keyboards.MainMenuIK()
                        );
                    break;
                case "hello":
                    isFirstMessageArrived = true;
                    data.UserChatId = message.Chat.Id;
                    await bot.SendTextMessageAsync(message.Chat.Id,
                        "Hi!\n" +
                        "I memorize your ID now I will send you the updates.\n" +
                        "type `/menu` to see more options", ParseMode.Markdown);
                    break;
                case "hi":
                    isFirstMessageArrived = true;
                    data.UserChatId = message.Chat.Id;
                    await bot.SendTextMessageAsync(message.Chat.Id,
                        "Hello!\n" +
                        "I memorize your ID now I will send you the updates.\n" +
                        "type `/menu` to see more options", ParseMode.Markdown);
                    break;
                case "/menu":                                        
                    await bot.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        "Main menu",
                        replyMarkup: keyboards.MainMenuIK()
                        );
                    break;                
                /*
                case "/help":                    
                    await bot.SendTextMessageAsync(message.Chat.Id,
                        "A list of helpful command\n" +
                        "- Use `/time [MIN]` to set updates (e.g. `/time 5`)\n" +                        
                        "- NOT IMPLEMENTED YET", ParseMode.Markdown);
                    break;
                case "/time":
                    if (args.Length <= 1)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id,
                        "Use `/time` followed by minutes");
                    }
                    else
                    {
                        int n = 0;
                        if (int.TryParse(args[1], out n))
                        {
                            if (n > 60)
                            {
                                await bot.SendTextMessageAsync(message.Chat.Id,
                                    "Max minutes allowed is 60 (for now)");
                            }
                            else if (n < 1)
                            {
                                await bot.SendTextMessageAsync(message.Chat.Id,
                                    "No negative minutes accepted");
                            }
                            else
                            {
                                seconds = System.Convert.ToInt32(n) * 1000;
                                //seconds = System.Convert.ToInt32(n) * minute;
                                await bot.SendTextMessageAsync(message.Chat.Id,
                                    "Timer updated. Now you will receive a message every " + n + " seconds");
                                Console.WriteLine("Time: " + seconds);
                            }
                        }
                        else
                            await bot.SendTextMessageAsync(message.Chat.Id,
                                "Only numbers allowed after /time command");
                    }
                    break;
                case "/stopbot":
                    InitializeBotShutdownProcedure();
                    break;
                case "/startbot":
                    InitializeBotResumeProcedure();
                    break;
                case "/clearconsole":
                    Console.Clear();
                    Console.WriteLine("Running...");
                    break;
                case "/screenshot":
                    //TODO Screenshot
                    break;
                    */
                default:
                    if(message.From.Username != null)
                        await bot.SendTextMessageAsync(message.Chat.Id, "Hey " + message.From.Username + ", Im super busy watching your diablo bot, can't chat right now!");                    
                    else
                        await bot.SendTextMessageAsync(message.Chat.Id, "Hey " + message.From.FirstName + ", Im super busy watching your diablo bot, can't chat right now!");

                    break;
            }            
        }

        private void InitializeBotResumeProcedure()
        {            
            string pScheduleName = "gator";
            //Check if scheduler is enable
            if (!profile.ScheduleEnable) profile.ScheduleEnable = true;

            //Check if gator scheduler is selected
            if (!profile.Schedule.Equals(pScheduleName) || String.IsNullOrEmpty(profile.Schedule)) profile.Schedule = pScheduleName;

            //Save back the updates
            //TODO it doesnt work with 1+ profile
            string json = JsonConvert.SerializeObject(profile);
            File.WriteAllText(data.PathToJsonProfile, json);
                        
            //Create List of Schedule and get all schedule existing
            List<Schedule> schedules = new List<Schedule>();
            
            int counter = 0;
            string line;

            // Read the file and store it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader(data.PathToSchedules);
            while ((line = file.ReadLine()) != null)
            {
                schedules.Add(JsonConvert.DeserializeObject<Schedule>(line));                
                counter++;
            }

            file.Close();
            //Check if private scheduler exists
            bool exists = false;
            foreach (Schedule schedule in schedules)
            {
                if (schedule.Name.Equals(pScheduleName)) exists = true;
            }

            Schedule tmp = new Schedule();
            //If schedule does not exists create a new one            
            if (!exists) tmp.Name = pScheduleName;
            //Otherwise get it
            else tmp = schedules.Find(e => e.Name.Equals(pScheduleName));
            //Calculate time and create a 24H schedule
            CalculateTime(tmp);

            //If schedule was new add into schedules list, otherwise it is already inside
            if(!exists) schedules.Add(tmp);

            //Due to schedules.json formatting is super wrong and doesnt respect json policy formatting I MUST do a foreach
            //Open file and delete everything inside it
            File.WriteAllText(data.PathToSchedules, string.Empty);
            foreach (Schedule item in schedules)
            {
                string appendText = JsonConvert.SerializeObject(item);
                File.AppendAllText(data.PathToSchedules, appendText + Environment.NewLine);
            }

            //Save it into schedules.json
            //string appendText = JsonConvert.SerializeObject(schedules);
            //File.WriteAllText(data.PathToSchedules, appendText);                                                                  

            //Start D2Bot program
            Process.Start(data.Path + "D2Bot.exe");
        }

        private void CalculateTime(Schedule schedule)
        {
            //Get current time and update the schedule
            DateTime localTime = DateTime.Now;
            //Start time
            schedule.Times[0].Hour = localTime.Hour;
            schedule.Times[0].Minute = localTime.Minute;
            
            //Minus 5 minuti
            DateTime updated = localTime.Subtract(new TimeSpan(0, 5, 0));
                        
            //End time
            schedule.Times[1].Hour = updated.Hour;
            schedule.Times[1].Minute = updated.Minute;            
        }

        private void InitializeBotShutdownProcedure()
        {
            //Get 2 processes, Game and D2Bot
            System.Diagnostics.Process[] game = System.Diagnostics.Process.GetProcessesByName("Game");
            //Prendo processo chiamato D2Bot e lo chiudo
            System.Diagnostics.Process[] d2bot = System.Diagnostics.Process.GetProcessesByName("D2Bot");

            //If both found kill them
            try
            {
                if (game != null) game[0].Kill();
                else Console.WriteLine("Unable to kill Game.exe because it was not found");
            }
            catch (IndexOutOfRangeException iore)
            {                
                Console.WriteLine(iore.Message);
                Console.WriteLine("[EXCEPTION]: No Game Process found");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.Message);
                Console.WriteLine("[EXCEPTION]: You should run me with Admin permisssion to perform these tasks. Command /stopbot ABORTED");
            }
            try
            { 
                if (d2bot != null) d2bot[0].Kill();
                else Console.WriteLine("Unable to kill D2Bot.exe because it was not found");
            }
            catch(IndexOutOfRangeException iore)
            {                
                Console.WriteLine(iore.Message);
                Console.WriteLine("[EXCEPTION]: No D2Bot Process found");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.Message);
                Console.WriteLine("[EXCEPTION]: You should run me with Admin permisssion to perform these tasks. Command /stopbot ABORTED");
            }

            //Disable scheduler in case the player want to run manually
            profile.ScheduleEnable = false;

            //Save back the updates
            string json = JsonConvert.SerializeObject(profile);
            File.WriteAllText(data.PathToJsonProfile, json);
        }

        //--------------------SEND_MESSAGES_METHODS--------------------------
        private async void Experience(DiabloChecker checker, Dictionary<string, long> levels)
        {
            //Prendo livello corrente
            string currentLevel = checker.botStatus.Level.ToString();
            //Prendo livello successivo
            string nextLevel = (System.Convert.ToInt32(currentLevel) + 1).ToString();
            
            //Prendo exp inizio livello corrente
            long expInitialCurrentLevel;
            levels.TryGetValue(currentLevel, out expInitialCurrentLevel);
            
            //Prendo exp fine livello corrente
            long expEndCurrentLevel;
            levels.TryGetValue(nextLevel, out expEndCurrentLevel);
            
            //Prendo exp corrente
            long currentExp = checker.botStatus.Experience;
            
            //Mappo il tutto e trasformo in stringa percentuale
            decimal expRemained = Remap(currentExp, expInitialCurrentLevel, expEndCurrentLevel, 0, 100);
            expRemained = System.Math.Round(expRemained, 2);
            if (expRemained >= 100)
            {
                //Check if level is actually changed
                CheckLevelUp(checker, currentLevel);                
                expRemained -= 100;
            }

            string percent = expRemained.ToString() + "%";
            string message;

            if (checker.IsLevelEnabled)
                message = "Current Level: " + currentLevel + "\nPercent complete " + percent;
            else
                message = percent + " completed";
            //Invio il messaggio usando il bot
            await bot.SendTextMessageAsync(checker.data.UserChatId, message);
            Console.WriteLine("Experience()");
        }

        private void CheckLevelUp(DiabloChecker check, string level)
        {
            //TODO CHECK IF ERROR OCCUR MODIFING THE FILE
            //If level not updated
            if (check.botStatus.Level.ToString() == level)
            {
                //Increment local level
                check.botStatus.Level++;
                //Increment level on file       
                try
                {
                    File.WriteAllText(data.PathStatus, JsonConvert.SerializeObject(check.botStatus));
                }
                catch (Exception e)
                {
                    bot.SendTextMessageAsync(check.data.UserChatId, e.StackTrace);
                    bot.SendTextMessageAsync(check.data.UserChatId, "Contact @Triko about this error");
                }
                
                //Send message LEVEL UP
                LevelUp(check);
            }
        }

        private void Death(DiabloChecker checker)
        {
            string message = "You are DEATH and lost experience";
            bot.SendTextMessageAsync(checker.data.UserChatId, message);
            Console.WriteLine("Death()");
        }

        private void LevelUp(DiabloChecker checker)
        {
            string message = "You LEVEL UP to " + checker.botStatus.Level;
            bot.SendTextMessageAsync(checker.data.UserChatId, message);
            Console.WriteLine("LevelUp()");
        }

        private async void NotImplementedYet(Telegram.Bot.Types.CallbackQuery callbackQuery)
        {            
            await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"NOT IMPLEMENTED YET");
        }
    }
}
