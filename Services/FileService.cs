using Newtonsoft.Json;
using RateXChange.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateXChange.Services
{
    public interface IFileService
    {
        Task SaveCurrenciesCollectionAsync<T>(IEnumerable<T> items, string filePath);
        Task<Dictionary<int, T>> LoadCurrenciesCollectionAsync<T>(string filePath);
    }

    internal class FileService: IFileService
    {
        private readonly JsonSerializerSettings _settings;

        public FileService()
        {
            _settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public async Task SaveCurrenciesCollectionAsync<T>(IEnumerable<T> items, string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = await Task.Run(() => JsonConvert.SerializeObject(items, _settings));

            using (var writer = new StreamWriter(filePath, append: false, Encoding.UTF8))
            {
                await writer.WriteAsync(json);
            }
        }

        public async Task<Dictionary<int, T>> LoadCurrenciesCollectionAsync<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new Dictionary<int, T>();
            }

            string json;
            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                json = await reader.ReadToEndAsync();
            }

            return await Task.Run(() =>
                JsonConvert.DeserializeObject<Dictionary<int, T>>(json, _settings) ?? new Dictionary<int, T>());
        }
    }
}
