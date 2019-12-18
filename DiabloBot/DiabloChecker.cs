using Newtonsoft.Json;
using System;
using System.IO;

namespace DiabloBot
{
    class DiabloChecker
    {
        //public string Path { get; set; }
        public Data data { get; set; }
        public BotStatus botStatus { get; set; }
        public bool IsLevelEnabled { get; set; } = false;
        
        public DiabloChecker(Data paths)
        {
            data = paths;            
        }

        public void ReadStatus()
        {
            //Read char status (exp, death, level)
            try
            {
                Console.WriteLine("Reading status from: " + data.PathStatus);
                botStatus = JsonConvert.DeserializeObject<BotStatus>(File.ReadAllText(data.PathStatus));
                Console.WriteLine("Data read");
            }
            catch
            {
                Console.WriteLine("Error reading status from: " + data.PathStatus);
                return;
            }
        }

        //If status read are differents update me
        public Tuple<string, bool> IsUpdated()
        {
            if (botStatus != null)
            {                 
                BotStatus tmp = JsonConvert.DeserializeObject<BotStatus>(File.ReadAllText(data.PathStatus));

                if (botStatus.Experience != tmp.Experience)
                {
                    botStatus.Experience = tmp.Experience;
                    return Tuple.Create("EXP", (bool)true);
                }
                if (botStatus.Deaths != tmp.Deaths)
                {
                    botStatus.Deaths = tmp.Deaths;
                    return Tuple.Create("DEATH", (bool)true);
                }
                if (botStatus.Level != tmp.Level)
                {
                    botStatus.Level = tmp.Level;
                    return Tuple.Create("LVLUP", (bool)true);
                }                
            }
            return Tuple.Create("", false);
        }
    }
}
