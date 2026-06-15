using System;
using System.Collections.Generic;
using System.Text;

namespace GoldenCrown.Infrastructure.Clients.CurrencyClient
{
    public interface IExchangeClient
    {
        Task<decimal> GetExchangeRate(string baseCurrencyCode, string targetCurrencyCode, CancellationToken cancellationToken);
    }
}
