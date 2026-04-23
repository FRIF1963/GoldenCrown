using GoldenCrown.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GoldenCrown.DTOs.Finance;


namespace GoldenCrown.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceController : ControllerBase
    {
        private readonly IFinanceService _financeSevice;

        public FinanceController(IFinanceService financeSevice)
        {
            _financeSevice = financeSevice;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalanceAsync([FromHeader(Name = "Token")] string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var balanceresult = await _financeSevice.GetBalance(token);

            if (balanceresult.IsSuccess)
            {
                return Ok(new BalanceResponse
                {
                    Balance = balanceresult.Value
                });
            }

            return BadRequest(new {Message = balanceresult.ErrorMessage});
        }
    }
}
