using Microsoft.Extensions.DependencyInjection;
using RateXChange.Services;
using RateXChange.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RateXChange.Config
{
    internal static class ServicesConfig
    {
        private const string _baseAddress = "https://api.nbrb.by/exrates/";

        internal static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton(sp =>
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(_baseAddress),
                    Timeout = TimeSpan.FromSeconds(30)
                };
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                return client;
            });

            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<IFileService, FileService>();

            services.AddTransient<MainViewModel>();
            services.AddSingleton<MainWindow>();

            return services;
        }
    }
}
