using System.ComponentModel;
using Newtonsoft.Json;

namespace TheDivision2Vendor
{
    public class D2Weapon : D2Empty
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

        [JsonProperty("dmg")]
        public string dmg { get; set; }

        [JsonProperty("rpm", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(0)]
        public int rpm { get; set; }

        [JsonProperty("mag", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(0)]
        public int mag { get; set; }

        [JsonProperty("talent")]
        public string talent { get; set; }

        [JsonProperty("attribute1")]
        public string attribute1 { get; set; }

        [JsonProperty("attribute2")]
        public string attribute2 { get; set; }

        [JsonProperty("attribute3")]
        public string attribute3 { get; set; }
    }
}
