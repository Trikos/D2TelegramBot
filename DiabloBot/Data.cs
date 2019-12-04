using Newtonsoft.Json;
using System;
using System.IO;
using Telegram.Bot;

namespace DiabloBot
{
    class Data
    {

        [JsonProperty("botToken")]
        public string BotToken { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("pathData")]
        public string PathData { get; set; }

        [JsonProperty("pathStatus")]
        public string PathStatus { get; set; }

        [JsonProperty("pathToItemLog")]
        public string PathToItemLog { get; set; }

        [JsonProperty("pathToSchedules")]
        public string PathToSchedules { get; set; }

        [JsonProperty("pathToJsonProfile")]
        public string PathToJsonProfile { get; set; }

        [JsonProperty("pathImages")]
        public string PathImages { get; set; }

        [JsonProperty("userChatId")]
        public long UserChatId { get; set; }
        

        public TelegramBotClient Initialize()
        {
            //Se LoadData torna false, inserisci i dati
            if (!LoadData())
            {
                try
                {
                    Console.WriteLine("Insert your d2bot folder path which contain D2Bot.exe:");
                    Console.WriteLine("Example \"C:\\Users\\YOUR_USERNAME\\Desktop\\d2bot-with-kolbot\"");
                    string halfPath = Console.ReadLine();
                    Path = halfPath + "\\"; ;
                    PathData = halfPath + "\\data\\";
                    PathToItemLog = halfPath + "\\d2bs\\kolbot\\logs\\ItemLog.txt";
                    PathImages = halfPath + "\\images\\";
                    PathToJsonProfile = halfPath + "\\data\\profile.json";
                    PathToSchedules = halfPath + "\\data\\schedules.json";

                    Console.WriteLine("Insert the exact name (case-sensitive) of the profile used in D2Bot.\n(The name under the tab \"Profile\" near \"Status\", \"Runs\", \"Exits\". etc");
                    string profileName = Console.ReadLine();
                    PathStatus = halfPath + "\\d2bs\\kolbot\\data\\" + profileName + ".json";

                    Console.WriteLine("Insert your Bot Telegram TOKEN:");
                    Console.WriteLine("Example of TOKEN: 1234567890:ABCDEfghilMnopQrs56qwe0qwO2B1Koosq");
                    BotToken = Console.ReadLine();

                    Console.WriteLine("Now say \"Hi\"/\"Hello\" to your bot on Telegram or press START button if it is the first time you open it up");
                    return new TelegramBotClient(BotToken);                                        
                }
                catch (Exception e)
                {
                    Console.WriteLine("INIT ERROR");
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                try
                {
                    return new TelegramBotClient(BotToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine("INIT ERROR");
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(e.Message);
                }
            }
            return null;
        }

        public bool LoadData()
        {
            try
            {
                //Check if file exists and create
                //CheckFile();
                //Load data from config file
                string userData = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "config.json");
                //If it is not empty
                if (!String.IsNullOrEmpty(userData))
                {
                    //Load data in this class
                    Data tmp = JsonConvert.DeserializeObject<Data>(userData);
                    BotToken = tmp.BotToken;
                    
                    Path = tmp.Path;
                    PathData = tmp.PathData;
                    PathStatus = tmp.PathStatus;
                    PathToItemLog = tmp.PathToItemLog;
                    PathToSchedules = tmp.PathToSchedules;
                    PathToJsonProfile = tmp.PathToJsonProfile;
                    PathImages = tmp.PathImages;
                    UserChatId = tmp.UserChatId;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (FileNotFoundException fnfe)
            {
                //This is a double check for file !exists
                //Console.WriteLine(fnfe.StackTrace);
                //Console.WriteLine(fnfe.Message);
                //Create file
                string directory = AppDomain.CurrentDomain.BaseDirectory;
                FileStream fs = File.Create(directory + "config.json");
                fs.Close();
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public void CheckConfigFile()
        {
            //Init the config.json file with your path and bot token
            /*string directory = AppDomain.CurrentDomain.BaseDirectory;
            //Check file exists
            if (!File.Exists(directory + "config.json"))
            {
                //Create it and return false 
                FileStream fs = File.Create(directory + "config.json");
                fs.Close();
                return;
            }*/
            ////Check file empty
            //using (var stream = File.Open(directory + "config.json", FileMode.Open))
            //{
            //    if (new FileInfo(directory + "config.json").Length == 0)
            //    {
            //        stream.Close();
            //        return;
            //    }
            //}

            ////If file exist and are full
            //using (var stream = File.Open(directory + "config.json", FileMode.Open))
            //{
            //    //If file exists return true
            //    string userData = File.ReadAllText(directory + "config.json");
            //    Data tmp = JsonConvert.DeserializeObject<Data>(userData);
            //    BotToken = tmp.BotToken;
            //    PathImages = tmp.PathImages;
            //    PathItems = tmp.PathItems;
            //    PathStatus = tmp.PathStatus;

            //    stream.Close();
            //    return;
            //}
        }
    }
}
