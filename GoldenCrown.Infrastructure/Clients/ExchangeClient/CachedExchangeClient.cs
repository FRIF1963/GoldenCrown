using GoldenCrown.Infrastructure.Clients.CurrencyClient;
using GoldenCrown.Infrastructure.Clients.CurrencyClient.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoldenCrown.Infrastructure.Clients.ExchangeClient
{
    public class CachedExchangeClient : IExchangeClient
    {
        private readonly IExchangeClient _exchangeClient;

        private readonly IMemoryCache _cache;

        private readonly ILogger<CachedExchangeClient> _logger;

        private static readonly TimeSpan Ttl = TimeSpan.FromHours(1);

        public CachedExchangeClient(IExchangeClient exchangeClient, IMemoryCache cache, ILogger<CachedExchangeClient> logger)
        {
            _exchangeClient = exchangeClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<decimal> GetExchangeRate(string baseCurrencyCode, string targetCurrencyCode, CancellationToken cancellationToken)
        {
            var rates = await GetExchangeRates(baseCurrencyCode, cancellationToken);
            return rates.First(x => x.Quote == targetCurrencyCode).Rate;
        }
        public async Task<ExchangeRateResponse[]> GetExchangeRates(string baseCurrencyCode, CancellationToken cancellationToken)
        {
            string key = $"currency:{baseCurrencyCode.ToUpper()}";
            if (_cache.TryGetValue<ExchangeRateResponse[]>(key, out var cached))
            {
                _logger.LogInformation($"Currency cache hit for {baseCurrencyCode}");
                return cached;
            }

            _logger.LogInformation($"Currency cache miss for {baseCurrencyCode}");
            var rates = await _exchangeClient.GetExchangeRates(baseCurrencyCode, cancellationToken);

            _cache.Set(key, rates, Ttl);
            return rates;
        }
    }
}
