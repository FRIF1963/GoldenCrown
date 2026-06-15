using GoldenCrown.Infrastructure.Clients.CurrencyClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoldenCrown.Application.Services.Currency
{
    public interface ICurrencyService
    {
        public ValueTask<decimal> Convert(decimal amount, string currencyCode, string targetCurrencyCode, CancellationToken cancellationToken);
    }

    public class CurrencyService : ICurrencyService
    {
        private readonly IExchangeClient _exchangeClient;
        public CurrencyService(IExchangeClient exchangeClient) 
        {
            _exchangeClient = exchangeClient;
        }
        public async ValueTask<decimal> Convert(decimal amount, string currencyCode, string targetCurrencyCode, CancellationToken cancellationToken)
        {
            if(currencyCode == targetCurrencyCode)
            {
                return amount;
            }

            var rate = await _exchangeClient.GetExchangeRate(currencyCode, targetCurrencyCode, cancellationToken);
            return amount * rate;
        }
    }
}
