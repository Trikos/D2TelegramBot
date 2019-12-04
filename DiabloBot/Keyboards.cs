using Telegram.Bot.Types.ReplyMarkups;

namespace DiabloBot
{
    class Keyboards
    {
        public InlineKeyboardMarkup MainMenuIK()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new [] // first row
                {                    
                    InlineKeyboardButton.WithCallbackData("START BOT", "/startbot"),
                    InlineKeyboardButton.WithCallbackData("STOP BOT", "/stopbot"),
                },
                new [] // second row
                {
                    InlineKeyboardButton.WithCallbackData("SET TIME", "/time"),
                    InlineKeyboardButton.WithCallbackData("MORE...", "/options"),
                },
                new [] // second row
                { InlineKeyboardButton.WithCallbackData("CLEAR CONSOLE", "/clearconsole") }
            });
        }

        public InlineKeyboardMarkup TimeIK()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new [] // first row
                {
                    InlineKeyboardButton.WithCallbackData("5", "/5min"),
                    InlineKeyboardButton.WithCallbackData("10", "/10min"),
                    InlineKeyboardButton.WithCallbackData("15", "/15min"),
                },
                new [] // second row
                {
                    InlineKeyboardButton.WithCallbackData("20", "/20min"),
                    InlineKeyboardButton.WithCallbackData("30", "/30min"),
                    InlineKeyboardButton.WithCallbackData("60", "/60min"),
                },
                new []
                { InlineKeyboardButton.WithCallbackData("BACK", "/mainmenu") }                
            });
        }

        public InlineKeyboardMarkup OptionsIK()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new [] // first row
                {
                    InlineKeyboardButton.WithCallbackData("Difficulty", "/difficulty"),
                    InlineKeyboardButton.WithCallbackData("Realm", "/realm"),
                    InlineKeyboardButton.WithCallbackData("Game Info", "/gameinfo"),
                },
                new [] // second row
                {
                    InlineKeyboardButton.WithCallbackData("Param", "/param"),
                    InlineKeyboardButton.WithCallbackData("Entry Script", "/entryscript"),
                    InlineKeyboardButton.WithCallbackData("Character", "/character"),
                },
                new []
                { InlineKeyboardButton.WithCallbackData("Bot Status", "/status") },
                new []
                { InlineKeyboardButton.WithCallbackData("BACK", "/mainmenu") }
            });
        }

        public InlineKeyboardMarkup BotStatusIK(BotStatus status)
        {
            string area = status.DebugInfo.Split('"')[7];
            return new InlineKeyboardMarkup(new[]
            {
                new [] // first row
                {InlineKeyboardButton.WithCallbackData("Name: " + status.Name)},
                new[]
                {InlineKeyboardButton.WithCallbackData("Exp: " + status.Experience)},
                new[]
                {InlineKeyboardButton.WithCallbackData("Gold: " + status.Gold)},
                new[]
                {InlineKeyboardButton.WithCallbackData("Level: " + status.Level)},
                new[]
                {InlineKeyboardButton.WithCallbackData("Deaths: " + status.Deaths)},
                new[]
                {InlineKeyboardButton.WithCallbackData("Runs: " + status.Runs)},
                new[]
                {InlineKeyboardButton.WithCallbackData("Script: " + area)},
                new[]
                {InlineKeyboardButton.WithCallbackData("BACK", "/options")}                                                                                                                                                            
            });
        }
    }
}
