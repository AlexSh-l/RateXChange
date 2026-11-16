using RateXChange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateXChange.ModelView
{
    public class HistoryItemViewModel: ObservableObject
    {
        private readonly HistoryLinkDTO _historyLinkDTO;

        public int Id => _historyLinkDTO.Id;

        public DateTime Date => _historyLinkDTO.Date;

        public decimal Rate
        {
            get => _historyLinkDTO.Rate;
            set
            {
                if (_historyLinkDTO.Rate != value)
                {
                    _historyLinkDTO.Rate = value;
                    OnPropertyChanged();
                }
            }
        }

        public HistoryItemViewModel(HistoryLinkDTO historyLinkDTO)
        {
            _historyLinkDTO = historyLinkDTO;
        }
    }
}
