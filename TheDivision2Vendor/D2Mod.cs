using System.ComponentModel;
using Newtonsoft.Json;

namespace TheDivision2Vendor
{
    public class D2Mod : D2Empty
    {
        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("rarity")]
        public string rarity { get; set; }

        [JsonProperty("vendor")]
        public string vendor { get; set; }

        [JsonProperty("level", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(0)]
        public int level { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("attributes")]
        public string attributes { get; set; }
    }
}
