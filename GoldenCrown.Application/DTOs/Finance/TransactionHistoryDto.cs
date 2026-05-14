using System;
using System.Collections.Generic;
using System.Text;

namespace GoldenCrown.Application.DTOs.Finance
{
    public class TransactionHistoryDto
    {
        public string ReceiverName { get; set; }

        public string SenderName { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreateAt { get; set; }
        public string Currency { get; set; }
    }
}
