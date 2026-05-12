using Azure.Core;
using FluentValidation;
using GoldenCrown.Attributes;
using GoldenCrown.Database.Models;
using GoldenCrown.DTOs.Finance;
using GoldenCrown.Feauters.Finance.Deposit;
using GoldenCrown.Feauters.Finance.GetBalance;
using GoldenCrown.Feauters.Finance.GetTransactionHistory;
using GoldenCrown.Feauters.Finance.Transfer;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace GoldenCrown.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MyAuthorize]
    public class FinanceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FinanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalanceAsync()
        {
            var command = new GetBalanceCommand(GetUserId());
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new BalanceResponse
                {
                    Balance = result.Value
                });
            }

            return BadRequest(new {Message = result.ErrorMessage});
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferAsync([FromBody] DTOs.Finance.TransferRequest request, [FromServices] IValidator<TransferRequest> validator)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var command = new TransferCommand(GetUserId(), request.ReceiverLogin, request.Amount);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpPost("Deposit")]
        public async Task<IActionResult> Deposit([FromBody] DTOs.Finance.DepositRequest request, [FromServices] IValidator<DepositRequest> validator)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var command = new DepositCommand(GetUserId(), request.amount);
            var result = await _mediator.Send(command);
            
            if(result)
            {
                return Ok();
            }

            return BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpGet("History")]
        public async Task<IActionResult> GetHistoryAsync([FromQuery] TransactionHistoryRequest request, [FromServices] IValidator<TransactionHistoryRequest> validator)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var command = new GetTransactionHistoryCommand(GetUserId(), request.From, request.To, request.Ofset, request.Limit);
            var result = await _mediator.Send(command);

            if(result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return BadRequest(new {Message = result.ErrorMessage});
        }

        internal int GetUserId()
        {
            var userId = HttpContext.Items[Constans.UserIdContextParametr] as int?;
            return userId!.Value;
        }
    }
}
