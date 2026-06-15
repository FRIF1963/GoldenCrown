using GoldenCrown.Application;
using GoldenCrown.Application.Events;
using GoldenCrown.Application.Services.Currency;
using GoldenCrown.Database;
using GoldenCrown.Domain.Models;
using GoldenCrown.Infrastructure.RabbitMQ;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Application.Feauters.Finance.Transfer
{
    public class TransferCommand : IRequest<Result>
    {
        public int UserId { get; set; }

        public string ReceiverLogin { get; set; }

        public decimal Amount { get; set; }

        public string Currency {  get; set; }

        public string ReceiverCurrency { get; set; }

        public TransferCommand(int userId, string receiverLogin, decimal amount, string currency, string receiverCurrency)
        {
            UserId = userId;
            ReceiverLogin = receiverLogin;
            Amount = amount;
            Currency = currency;
            ReceiverCurrency = receiverCurrency;
        }

        public class TransferCommandHandler : IRequestHandler<TransferCommand, Result>
        {
            private readonly ApplicationDBContext _context;

            private readonly IMessageProducer _messageProducer;

            private readonly ICurrencyService _currencyService;

            public TransferCommandHandler(ApplicationDBContext context, IMessageProducer messageProducer, ICurrencyService currencyService) 
            { 
                _context = context;
                _messageProducer = messageProducer;
                _currencyService = currencyService;
            }

            public async Task<Result> Handle(TransferCommand request, CancellationToken cancellationToken)
            {
                var receiverUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.ReceiverLogin, cancellationToken);

                if (receiverUser == null)
                {
                    return Result.Failure($"{request.ReceiverLogin} Not Found");
                }

                var senderAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == request.UserId && a.Currency == request.Currency, cancellationToken);

                if (senderAccount == null)
                {
                    return Result.Failure($"Not Found senderAccount");
                }

                if (senderAccount.Balance < request.Amount)
                {
                    return Result.Failure($"Your balance is less than {request.Amount}");
                }
                else if (senderAccount.Balance <= 0)
                {
                    return Result.Failure("Your balance must be greater than 0");
                }

                var receiverAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == receiverUser.Id && a.Currency == request.ReceiverCurrency, cancellationToken);

                if (receiverAccount == null)
                {
                    return Result.Failure($"Not Found receiverAccount");
                }

                senderAccount.Balance -= request.Amount;

                var targetAmount = await _currencyService.Convert(request.Amount, senderAccount.Currency, receiverAccount.Currency, cancellationToken);

                receiverAccount.Balance += targetAmount;

                var transaction = new Transaction
                {
                    SenderAccountId = senderAccount.Id,
                    ReceiverAccountId = receiverAccount.Id,
                    CreateAt = DateTime.UtcNow,
                    Amount = request.Amount,
                    Currency = request.Currency
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync(cancellationToken);

                await _messageProducer.SendMessageAsync(new TransactionCreatedEvent
                {
                    SenderId = request.UserId,
                    ReceiverId= receiverUser.Id,
                    Amount = transaction.Amount,
                    Currency= transaction.Currency
                }, cancellationToken);

                return Result.Success();
            }
        }
    }
}
