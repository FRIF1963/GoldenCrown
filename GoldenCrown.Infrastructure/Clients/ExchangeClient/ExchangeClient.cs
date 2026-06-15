using GoldenCrown.Infrastructure.Clients.CurrencyClient.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace GoldenCrown.Infrastructure.Clients.CurrencyClient
{
    public class ExchangeClient : IExchangeClient
    {
        private readonly HttpClient _httpClient;

        private readonly ExchangeClientSettings _settings;
        public ExchangeClient(IOptions<ExchangeClientSettings> options, HttpClient httpClient) 
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<decimal> GetExchangeRate(string baseCurrencyCode, string targetCurrencyCode, CancellationToken cancellationToken)
        {
            return (await GetExchangeRates(baseCurrencyCode, cancellationToken)).First(x => x.Quote == targetCurrencyCode).Rate;
        }
        public async Task<ExchangeRateResponse[]> GetExchangeRates(string baseCurrencyCode, CancellationToken cancellationToken)
        {
            var url = string.Format(_settings.Url, baseCurrencyCode);
            var result = await _httpClient.GetAsync(url, cancellationToken);
            result.EnsureSuccessStatusCode();
            var rates = await result.Content.ReadFromJsonAsync<ExchangeRateResponse[]>(cancellationToken);

            return rates!;
        }
       

    }
}
