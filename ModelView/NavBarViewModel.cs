using RateXChange.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RateXChange.ModelView
{
    public class NavBarViewModel: ObservableObject
    {
        public event Action SelectionOrDateChanged;
        public ObservableCollection<CurrencyStitched> Currencies { get; } = new ObservableCollection<CurrencyStitched>();

        public ICommand LoadCurrenciesCommand { get; }
        public ICommand SaveToFileCommand { get; }
        
        private CurrencyStitched _selectedCurrency;
        public CurrencyStitched SelectedCurrency
        {
            get => _selectedCurrency;
            set
            {
                if (SetProperty(ref _selectedCurrency, value) && value != null)
                {
                    Abbreviation = value.Abbreviation;
                    Name = value.Name;
                    SelectionOrDateChanged?.Invoke();
                }
            }
        }

        private string _abbreviation;
        public string Abbreviation
        {
            get => _abbreviation;
            set
            {
                if (SetProperty(ref _abbreviation, value) && !string.IsNullOrWhiteSpace(value))
                {
                    if (SelectedCurrency != null)
                    {
                        SelectedCurrency.Abbreviation = value.Trim();
                    }
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private DateTime _startDate = new DateTime(2026, 1, 1);
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    if (_startDate > EndDate)
                    {
                        EndDate = _startDate;
                    }
                    else
                    {
                        SelectionOrDateChanged?.Invoke();
                    }
                }
            }
        }

        private DateTime _endDate = new DateTime(2026, 11, 11);
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                {
                    if (_endDate < StartDate)
                    {
                        StartDate = _endDate;
                    }
                    else
                    {
                        SelectionOrDateChanged?.Invoke();
                    }
                }
            }
        }

        public NavBarViewModel(ICommand loadCurrenciesCommand, ICommand saveToFileCommand)
        {
            LoadCurrenciesCommand = loadCurrenciesCommand;
            SaveToFileCommand = saveToFileCommand;
        }
    }
}
