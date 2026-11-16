using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateXChange.Model
{
    public class HistoryLinkDTO
    {
        [JsonProperty("Cur_ID")]
        public int Id { get; set; }

        [JsonProperty("Date")]
        public DateTime Date { get; set; }

        [JsonProperty("Cur_OfficialRate")]
        public decimal Rate { get; set; }
    }
}
