using Newtonsoft.Json;

namespace TheDivision2Vendor
{
    public class D2Gear : D2Empty
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

        [JsonProperty("brand")]
        public string brand { get; set; }

        [JsonProperty("armor")]
        public string armor { get; set; }

        [JsonProperty("core")]
        public string core { get; set; }

        [JsonProperty("attributes")]
        public string attributes { get; set; }

        [JsonProperty("talents")]
        public string talents { get; set; }

        [JsonProperty("mods")]
        public string mods { get; set; }

        public bool hasTalents()
        {
            var has = false;
            if (talents != null && !string.IsNullOrWhiteSpace(talents) && !talents.Equals("-")) has = true;
            return has;
        }
    }
}
