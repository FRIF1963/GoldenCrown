using AutoMapper;
using FluentValidation;
using GoldenCrown.Api.DTOs.Finance;
using GoldenCrown.Application.Feauters.Finance.Deposit;
using GoldenCrown.Application.Feauters.Finance.GetBalance;
using GoldenCrown.Application.Feauters.Finance.GetTransactionHistory;
using GoldenCrown.Application.Feauters.Finance.Transfer;
using GoldenCrown.Attributes;
using GoldenCrown.DTOs.Finance;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace GoldenCrown.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MyAuthorize]
    public class FinanceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public FinanceController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalanceAsync(BalanceRequest request, [FromServices] IValidator<BalanceRequest> validator)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var command = new GetBalanceQuery(GetUserId(), request.Currency);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new BalanceResponse
                {
                    Balance = result.Value
                });
            }

            return BadRequest();
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferAsync([FromBody] DTOs.Finance.TransferRequest request, [FromServices] IValidator<TransferRequest> validator)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var command = new TransferCommand(GetUserId(), request.ReceiverLogin, request.Amount, request.Currency);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("Deposit")]
        public async Task<IActionResult> Deposit([FromBody] DTOs.Finance.DepositRequest request, [FromServices] IValidator<DepositRequest> validator)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var command = new DepositCommand(GetUserId(), request.Amount, request.Currency);
            var result = await _mediator.Send(command);
            
            if(result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet("History")]
        public async Task<ActionResult<IEnumerable<TransactionHistoryResponse>>> GetHistoryAsync([FromQuery] TransactionHistoryRequest request, [FromServices] IValidator<TransactionHistoryRequest> validator)
        {
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var command = new GetTransactionHistoryQuery(GetUserId(), request.From, request.To, request.Ofset, request.Limit);
            var result = await _mediator.Send(command);

            if(result.IsSuccess)
            {
                var response = result.Value.Select(_mapper.Map<TransactionHistoryResponse>);
                return Ok(response);
            }

            return BadRequest();
        }

        internal int GetUserId()
        {
            var userId = HttpContext.Items[Constans.UserIdContextParametr] as int?;
            return userId!.Value;
        }
    }
}
