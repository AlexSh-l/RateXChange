using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RateXChange.Model;

namespace RateXChange.Services
{
    public interface IApiService
    {
        Task<Dictionary<int, CurrencyStitched>> GetCurrenciesAsync();
        Task<List<HistoryLinkDTO>> GetHistoryAsync(int currencyId, DateTime startDate, DateTime endDate);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Dictionary<int, CurrencyStitched>> GetAllDataCurrenciesAsync()
        {
            var response = await _httpClient.GetAsync("currencies");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<CurrencyDTO>>(json);
            var currencyDictionary = new Dictionary<int, CurrencyStitched>();

            foreach (var currency in result)
            {
                var parsedStartDate = currency.DateStart;
                var parsedEndDate = currency.DateEnd;

                if (currency.Id != currency.ParentId && currencyDictionary.ContainsKey(currency.ParentId))
                {
                    var parent = currencyDictionary[currency.ParentId];

                    parent.Children.Add(new CurrencyChild
                    {
                        Id = currency.Id,
                        DateStart = parsedStartDate,
                        DateEnd = parsedEndDate
                    });
                }
                else
                {
                    currencyDictionary.Add(currency.Id, new CurrencyStitched
                    {
                        Id = currency.Id,
                        Abbreviation = currency.Abbreviation,
                        Name = currency.Name,
                        Scale = currency.Scale,
                        DateStart = parsedStartDate,
                        DateEnd = parsedEndDate
                    });
                }
            }

            return currencyDictionary;
        }

        public async Task<Dictionary<int, CurrencyStitched>> GetCurrenciesAsync()
        {
            var response = await _httpClient.GetAsync("currencies");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<CurrencyDTO>>(json);
            var currencyDictionary = new Dictionary<int, CurrencyStitched>();

            foreach (var currency in result)
            {
                if (currency.Id != currency.ParentId && currencyDictionary.ContainsKey(currency.ParentId))
                {
                    var parent = currencyDictionary[currency.ParentId];

                    parent.Children.Add(new CurrencyChild
                    {
                        Id = currency.Id,
                        DateStart = currency.DateStart,
                        DateEnd = currency.DateEnd
                    });
                }
                else
                {
                    currencyDictionary.Add(currency.Id, new CurrencyStitched
                    {
                        Id = currency.Id,
                        Abbreviation = currency.Abbreviation,
                        Name = currency.Name,
                        Scale = currency.Scale,
                        DateStart = currency.DateStart,
                        DateEnd = currency.DateEnd
                    });
                }
            }

            return currencyDictionary;
        }

        public async Task<List<HistoryLinkDTO>> GetHistoryAsync(int currencyId, DateTime startDate, DateTime endDate)
        {
            string startDateString = startDate.ToString("yyyy-MM-dd");
            string endDateString = endDate.ToString("yyyy-MM-dd");
            string requestLink = $"rates/dynamics/{currencyId}?startdate={startDateString}&enddate={endDateString}";

            var response = await _httpClient.GetAsync(requestLink);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<HistoryLinkDTO>>(json);

            return result;
        }
    }
}
