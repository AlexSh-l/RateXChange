using Newtonsoft.Json;
using System;

namespace RateXChange.Model
{
    public class CurrencyDTO
    {
        [JsonProperty("Cur_ID")]
        public int Id { get; set; }

        [JsonProperty("Cur_ParentID")]
        public int ParentId { get; set; }

        [JsonProperty("Cur_Abbreviation")]
        public string Abbreviation { get; set; }

        [JsonProperty("Cur_Name")]
        public string Name { get; set; }

        [JsonProperty("Cur_Scale")]
        public int Scale { get; set; }

        [JsonProperty("Cur_DateStart")]
        public DateTime DateStart { get; set; }

        [JsonProperty("Cur_DateEnd")]
        public DateTime DateEnd { get; set; }
    }
}
