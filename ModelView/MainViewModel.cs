using RateXChange.Model;
using RateXChange.ModelView;
using RateXChange.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RateXChange.View
{
    public class MainViewModel : ObservableObject
    {
        private readonly IApiService _apiService;
        private readonly IFileService _fileService;

        private int _selectedCurrencyId = 145;
        private readonly string _cacheFolderPath = "results";
        private readonly string _cacheFileName = "CurrenciesData.json";
        private Dictionary<int, CurrencyStitched> _currenciesDictionary;

        public ObservableCollection<HistoryItemViewModel> ChartData { get; set; } = new ObservableCollection<HistoryItemViewModel>();
        public NavBarViewModel NavBar { get; }

        public ICommand LoadCurrenciesCommand { get; }
        public ICommand LoadHistoryCommand { get; }
        public ICommand InitializeCommand { get; }
        public ICommand SaveToFileCommand { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }   

        public MainViewModel(IApiService apiService, IFileService fileService)
        {
            _apiService = apiService;
            _fileService = fileService;

            InitializeCommand = new AsyncRelayCommand(InitializeAsync);
            LoadCurrenciesCommand = new AsyncRelayCommand(LoadCurrenciesAsync);
            LoadHistoryCommand = new AsyncRelayCommand(LoadHistoryAsync);
            SaveToFileCommand = new AsyncRelayCommand(SaveToFileAsync);

            NavBar = new NavBarViewModel(LoadCurrenciesCommand, SaveToFileCommand);

            NavBar.SelectionOrDateChanged += () =>
            {
                if (NavBar.SelectedCurrency != null)
                {
                    _selectedCurrencyId = NavBar.SelectedCurrency.Id;
                    if (LoadHistoryCommand.CanExecute(null))
                    {
                        LoadHistoryCommand.Execute(null);
                    }
                }
            };
        }

        private async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                string appRoot = AppContext.BaseDirectory;
                string resultsFilePath = Path.Combine(appRoot, $"{_cacheFolderPath}\\{_cacheFileName}");

                if (File.Exists(resultsFilePath))
                {
                    _currenciesDictionary = await _fileService.LoadCurrenciesCollectionAsync<CurrencyStitched>(resultsFilePath);
                }
                else
                {
                    _currenciesDictionary = await _apiService.GetCurrenciesAsync();
                }

                PopulateNavBar();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка инициализации: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadCurrenciesAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                _currenciesDictionary = await _apiService.GetCurrenciesAsync();

                PopulateNavBar();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки данных по валютам: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadHistoryAsync()
        {
            var selectedCurrency = NavBar.SelectedCurrency;
            if (selectedCurrency == null) return;

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                ChartData.Clear();

                List<HistoryLinkDTO> history = _currenciesDictionary[selectedCurrency.Id].History;

                var validId = GetCurrentId(selectedCurrency, NavBar.EndDate);
                if (history == null && validId > 0)
                {
                    history = await _apiService.GetHistoryAsync(validId, NavBar.StartDate, NavBar.EndDate);
                    selectedCurrency.History = history;
                }

                if (history != null)
                {
                    foreach (var link in history)
                    {
                        ChartData.Add(new HistoryItemViewModel(link));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки истории изменения курсов: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }       

        private async Task SaveToFileAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                string folderPath = Path.Combine(AppContext.BaseDirectory, _cacheFolderPath);
                Directory.CreateDirectory(folderPath);
                string resultsFilePath = Path.Combine(folderPath, _cacheFileName);

                await _fileService.SaveCurrenciesCollectionAsync(_currenciesDictionary, resultsFilePath);
                
                PopulateNavBar();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка сохранения данных: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private int GetCurrentId(CurrencyStitched value, DateTime date)
        {
            if (DateTime.Compare(value.DateEnd, date) > 0)
            {
                return value.Id;
            }
            foreach (var child in value.Children)
            {
                if (DateTime.Compare(child.DateEnd, date) > 0)
                {
                    return child.Id;
                }
            }

            return -1;
        }

        private void PopulateNavBar()
        {
            if (_currenciesDictionary == null) return;

            NavBar.Currencies.Clear();
            foreach (var item in _currenciesDictionary.Values)
            {
                NavBar.Currencies.Add(item);
            }

            if (_currenciesDictionary.TryGetValue(_selectedCurrencyId, out var defaultCurrency))
            {
                NavBar.SelectedCurrency = defaultCurrency;
            }
        }
    }
}
