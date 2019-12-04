using Newtonsoft.Json;

namespace DiabloBot
{
    public class Profile
    {
        [JsonProperty("Account")]
        public string Account { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("Character")]
        public string Character { get; set; }

        [JsonProperty("GameName")]
        public string GameName { get; set; }

        [JsonProperty("GamePass")]
        public string GamePass { get; set; }

        [JsonProperty("D2Path")]
        public string D2Path { get; set; }

        [JsonProperty("Realm")]
        public string Realm { get; set; }

        [JsonProperty("Mode")]
        public string Mode { get; set; }

        [JsonProperty("Difficulty")]
        public string Difficulty { get; set; }

        [JsonProperty("Parameters")]
        public string Parameters { get; set; }

        [JsonProperty("Entry")]
        public string Entry { get; set; }

        [JsonProperty("Location")]
        public string Location { get; set; }

        [JsonProperty("KeyList")]
        public string KeyList { get; set; }

        [JsonProperty("Schedule")]
        public string Schedule { get; set; }

        [JsonProperty("GameCount")]
        public long GameCount { get; set; }

        [JsonProperty("Runs")]
        public long Runs { get; set; }

        [JsonProperty("Chickens")]
        public long Chickens { get; set; }

        [JsonProperty("Deaths")]
        public long Deaths { get; set; }

        [JsonProperty("Crashes")]
        public long Crashes { get; set; }

        [JsonProperty("Restarts")]
        public long Restarts { get; set; }

        [JsonProperty("RunsPerKey")]
        public long RunsPerKey { get; set; }

        [JsonProperty("KeyRuns")]
        public long KeyRuns { get; set; }

        [JsonProperty("InfoTag")]
        public string InfoTag { get; set; }

        [JsonProperty("Visible")]
        public bool Visible { get; set; }

        [JsonProperty("BlockRD")]
        public bool BlockRd { get; set; }

        [JsonProperty("ScheduleEnable")]
        public bool ScheduleEnable { get; set; }

        [JsonProperty("Type")]
        public long Type { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Group")]
        public string Group { get; set; }
    }
}
