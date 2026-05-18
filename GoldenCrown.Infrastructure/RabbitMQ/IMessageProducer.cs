using System;
using System.Collections.Generic;
using System.Text;

namespace GoldenCrown.Infrastructure.RabbitMQ
{
    public interface IMessageProducer
    {
        Task SendMessageAsync<T> (T message, CancellationToken token = default);
    }
}
