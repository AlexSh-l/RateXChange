using Newtonsoft.Json;
using System;

namespace RateXChange.Model
{
    public class CurrencyChild
    {
        public int Id { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }
    }
}
