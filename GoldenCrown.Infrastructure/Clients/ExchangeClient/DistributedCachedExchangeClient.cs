using GoldenCrown.Infrastructure.Clients.CurrencyClient;
using GoldenCrown.Infrastructure.Clients.CurrencyClient.Models;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using System.Text.Json;

namespace GoldenCrown.Infrastructure.Clients.ExchangeClient
{
    public class DistributedCachedExchangeClient : IExchangeClient
    {
        private readonly IExchangeClient _client;
        private readonly IDistributedCache _cache;
        private readonly ILogger<DistributedCachedExchangeClient> _logger;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1,1);

        private static readonly DistributedCacheEntryOptions _options = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };

        public DistributedCachedExchangeClient(IExchangeClient client, IDistributedCache cache, ILogger<DistributedCachedExchangeClient> logger)
        {
            _client = client;
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
            var cached = await _cache.GetStringAsync(key, cancellationToken);
            if (cached != null)
            {
                _logger.LogInformation($"Currency cache hit for {baseCurrencyCode}");
                return JsonSerializer.Deserialize<ExchangeRateResponse[]>(cached)!;
            }

            _logger.LogInformation($"Currency cache miss for {baseCurrencyCode}");

            ExchangeRateResponse[] rates;
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                cached = await _cache.GetStringAsync(key,cancellationToken);
                if(cached != null)
                {
                    _logger.LogInformation($"Currency cache hit after semaphore for {baseCurrencyCode}");
                    return JsonSerializer.Deserialize<ExchangeRateResponse[]>(cached!)!;
                }
                _logger.LogInformation($"Currency http request for {baseCurrencyCode}");
                rates = await _client.GetExchangeRates(baseCurrencyCode, cancellationToken);
                await _cache!.SetStringAsync(key,JsonSerializer.Serialize(rates), _options, cancellationToken);
            }
            finally
            {
                _semaphore.Release();
            }

            return rates;
        }
    }
}
