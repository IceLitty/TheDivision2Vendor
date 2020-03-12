using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheDivision2Vendor
{
    public class D2Gear
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

        [JsonProperty("attributes")]
        public string attributes { get; set; }

        [JsonProperty("talents")]
        public string talents { get; set; }

        [JsonProperty("mods")]
        public string mods { get; set; }
    }
}
