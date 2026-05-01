using Azure.Core;
using GoldenCrown.Attributes;
using GoldenCrown.Database.Models;
using GoldenCrown.DTOs.Finance;
using GoldenCrown.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace GoldenCrown.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MyAuthorize]
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
        public async Task<IActionResult> GetHistoryAsync([FromQuery] TransactionHistoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var historyTransactionResult = await _financeSevice.GetTransactionHistoryAsync(request.Token, request.From, request.To, request.Ofset, request.Limit);

            if(historyTransactionResult.IsSuccess)
            {
                return Ok(historyTransactionResult.Value);
            }

            return BadRequest(new {Message =  historyTransactionResult.ErrorMessage});
        }

        internal int GetUserId()
        {
            var userId = HttpContext.Items[Constans.UserIdContextParametr] as int?;
            return userId!.Value;
        }
    }
}
