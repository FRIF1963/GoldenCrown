using Microsoft.AspNetCore.Mvc;

namespace GoldenCrown.Api.DTOs.Finance
{
    public class BalanceRequest
    {
        [FromQuery] public string Currency { get; set; }
    }
}
