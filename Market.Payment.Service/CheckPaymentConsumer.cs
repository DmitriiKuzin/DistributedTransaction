using Market.Mq;
using MassTransit;

namespace Market.Payment.Service;

public class CheckPaymentConsumer: IConsumer<CheckPayment>
{
    public async Task Consume(ConsumeContext<CheckPayment> context)
    {
        await Task.Delay(500);
        if (context.Message.OrderId.ToString().StartsWith("1"))
        {
            await context.RespondAsync(new PaymentFailed(context.Message.OrderId));
            return;
        }
        await context.RespondAsync(new PaymentSucceed(context.Message.OrderId));
    }
}