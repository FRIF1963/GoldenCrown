using GoldenCrown.Infrastructure.Clients.CurrencyClient.Models;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoldenCrown.Infrastructure.Clients.CurrencyClient
{
    public interface IExchangeClient
    {
        Task<decimal> GetExchangeRate(string baseCurrencyCode, string targetCurrencyCode, CancellationToken cancellationToken);
        Task<ExchangeRateResponse[]> GetExchangeRates(string baseCurrencyCode, CancellationToken cancellationToken);
    }

   
}
