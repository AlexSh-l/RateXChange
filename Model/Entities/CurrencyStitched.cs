using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RateXChange.Model
{
    public class CurrencyStitched
    {
        public int Id { get; set; }

        public string Abbreviation { get; set; }

        public string Name { get; set; }

        public int Scale { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public List<CurrencyChild> Children { get; set; } = new List<CurrencyChild>();

        public List<HistoryLinkDTO> History { get; set; }
    }
}
