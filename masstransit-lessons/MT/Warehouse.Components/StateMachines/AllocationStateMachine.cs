using MassTransit;
using Microsoft.Extensions.Logging;
using Warehouse.Contracts;

namespace Warehouse.Components.StateMachines;

public class AllocationStateMachine :
    MassTransitStateMachine<AllocationState>
{
    private ILogger<AllocationStateMachine> _logger;
    
    public AllocationStateMachine(
        ILogger<AllocationStateMachine> logger)
    {
        _logger = logger;
        Event(() => AllocationCreated, x => x.CorrelateById(m => m.Message.AllocationId));

        Schedule(() => HoldExpiration, x => x.HoldDurationToken, s =>
        {
            //запуск по умолчанию. использовать не будем.
            s.Delay = TimeSpan.FromHours(8);
            //точно так же куа и у евентов надо указывать как коррелировать
            s.Received = x => x.CorrelateById(x => x.Message.AllocationId);
        });

        InstanceState(x=>x.CurrentState);

        Initially(
            When(AllocationCreated)
                .Then(x => { _logger.LogInformation($" {DateTime.Now} AllocationCreated !!! "); })
                .Schedule(
                    // сам шедуллер
                    HoldExpiration,
                    // какой евент пуляем
                    context => context.Init<IAllocationHoldDurationExpired>(new
                    {
                        AllocationId = context.Message.AllocationId
                    }),
                    // сам триггер
                    context => context.Message.HoldDuration)
                .TransitionTo(Allocated)
        );

        During(Allocated,
            When(HoldExpiration.Received)
                .Then(x =>
                {
                    _logger.LogInformation($" {DateTime.Now} HoldExpiration.Received !!!");
                })
                .Finalize()
        );
        
        //говрим что бы удаляли из репы когда финлизировано
        SetCompletedWhenFinalized();
    }

    public Schedule<AllocationState, IAllocationHoldDurationExpired> HoldExpiration { get; set; }
    public State Allocated { get; set; }
    public Event<IAllocationCreated> AllocationCreated { get; set; }
}