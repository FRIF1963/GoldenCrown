using System;
using System.Collections.Generic;
using System.Text;

namespace GoldenCrown.Infrastructure.Clients.CurrencyClient.Models
{
    public class ExchangeRateResponse
    {
        public DateTime Date { get; set; }

        public string Base { get; set; }

        public string Quote { get; set; }

        public decimal Rate { get; set; }
    }
}
