using MassTransit;

namespace Market.Mq;

public class OrderState : SagaStateMachineInstance
{
    
    /// <inheritdoc />
    public Guid CorrelationId { get; set; }

    public string Value { get; set; }

    /// <summary>
    /// the current saga state
    /// </summary>
    public string CurrentState { get; set; }
}

public record SubmitOrder(Guid OrderId, string Value, Guid CorrelationId): CorrelatedBy<Guid>;

public class OrderStateMachine :
    MassTransitStateMachine<OrderState>
{

    public Event<SubmitOrder> SubmitOrder { get; private set; } = null!;
    // public State Initial { get; private set; } 
    public State Submitted { get; private set; } = null!;
    public State Accepted { get; private set; } = null!;
    // public State Final { get; private set; } 
    
    public OrderStateMachine() 
    {
        Event(
            () => SubmitOrder,
            e =>
            {
                e.SetSagaFactory(cxt =>
                {
                    // complex constructor logic
                    return new OrderState 
                    {
                        CorrelationId = cxt.Message.CorrelationId
                    };
                });
            }
        );
        // Tell the saga where to store the current state
        InstanceState(x => x.CurrentState);
        Initially(  When(SubmitOrder)
            .Then(behaviorContext => 
            {
                var saga = behaviorContext.Saga;
                var message = behaviorContext.Message;
                saga.Value = message.Value;
                saga.CorrelationId = message.CorrelationId;
             })
            .TransitionTo(Submitted));
    }
}

public class OrderSaga :
    ISaga,
    InitiatedBy<SubmitOrder>
{
    public Guid CorrelationId { get; set; }
    public DateTime? SubmitDate { get; set; }
    public DateTime? AcceptDate { get; set; }

    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        await Task.Delay(100);
    }
}

public class OrderStateDefinition :
    SagaDefinition<OrderState>
{
    public OrderStateDefinition()
    {
        // specify the message limit at the endpoint level, which influences
        // the endpoint prefetch count, if supported
        Endpoint(e => e.ConcurrentMessageLimit = 16);
    }
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
    {
        var partition = endpointConfigurator.CreatePartitioner(16);
        sagaConfigurator.Message<SubmitOrder>(x => x.UsePartitioner(partition, m => m.Message.CorrelationId));
    }
}
