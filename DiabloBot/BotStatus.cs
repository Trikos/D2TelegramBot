using Newtonsoft.Json;

namespace DiabloBot
{
    class BotStatus
    {
        [JsonProperty("runs")]
        public long Runs { get; set; }

        [JsonProperty("experience")]
        public long Experience { get; set; }

        [JsonProperty("deaths")]
        public long Deaths { get; set; }

        [JsonProperty("lastArea")]
        public string LastArea { get; set; }

        [JsonProperty("gold")]
        public long Gold { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("gameName")]
        public string GameName { get; set; }

        [JsonProperty("ingameTick")]
        public long IngameTick { get; set; }

        [JsonProperty("handle")]
        public long Handle { get; set; }

        [JsonProperty("nextGame")]
        public string NextGame { get; set; }

        [JsonProperty("debugInfo")]
        public string DebugInfo { get; set; }                
    }
}
