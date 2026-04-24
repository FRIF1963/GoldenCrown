using GoldenCrown.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GoldenCrown.DTOs.Finance;
using GoldenCrown.Database.Models;


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
            var balanceresult = await _financeSevice.GetBalanceAsync(token);

            if (balanceresult.IsSuccess)
            {
                return Ok(new BalanceResponse
                {
                    Balance = balanceresult.Value
                });
            }

            return BadRequest(new {Message = balanceresult.ErrorMessage});
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferAsync([FromBody] DTOs.Finance.TransferRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transferresult = await _financeSevice.TransferAsync(request.Token, request.ReceiverLogin, request.Amount);

            if (transferresult.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(new { Message = transferresult.ErrorMessage });
        }

        [HttpPost("Deposit")]
        public async Task<IActionResult> Deposit([FromBody] DTOs.Finance.DepositRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var depositResult = await _financeSevice.DepositAsync(request.token, request.amount);
            
            if(depositResult.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(new { Message = depositResult.ErrorMessage });
        }

        [HttpGet("History")]
        public async Task<IActionResult> GetHistoryAsync(
            [FromHeader] string token,
            [FromHeader] DateTime from,
            [FromHeader] DateTime to,
            [FromHeader] int ofset,
            [FromHeader] int limit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var historyTransactionResult = await _financeSevice.GetTransactionHistoryAsync(token, from, to, ofset, limit);

            if(historyTransactionResult.IsSuccess)
            {
                return Ok(historyTransactionResult);
            }

            return BadRequest(new {Message =  historyTransactionResult.ErrorMessage});
        }
    }
}
