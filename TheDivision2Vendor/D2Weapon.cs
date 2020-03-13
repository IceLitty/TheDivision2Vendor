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

        [JsonProperty("score")]
        public int score { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("dmg")]
        public string dmg { get; set; }

        [JsonProperty("rpm")]
        public int rpm { get; set; }

        [JsonProperty("mag")]
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
