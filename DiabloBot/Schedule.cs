using Newtonsoft.Json;

namespace DiabloBot
{
    public class Schedule
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Times")]
        public Time[] Times { get; set; } = new Time[2] { new Time() {}, new Time() {} };
    }

    public class Time
    {
        [JsonProperty("Hour")]
        public long Hour { get; set; }

        [JsonProperty("Minute")]
        public long Minute { get; set; }
    }

    
}
