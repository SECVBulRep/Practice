using MassTransit;
using Warehouse.Contracts;

namespace Warehouse.Components.StateMachines;

public class AllocationStateMachine :
    MassTransitStateMachine<AllocationState>
{
    public AllocationStateMachine()
    {
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
                .Then(Action));
        //говрим что бы удаляли из репы когда финлизировано
        SetCompletedWhenFinalized();
    }

    private void Action(BehaviorContext<AllocationState, IAllocationHoldDurationExpired> obj)
    {
       
    }

    public Schedule<AllocationState, IAllocationHoldDurationExpired> HoldExpiration { get; set; }
    public State Allocated { get; set; }
    public Event<IAllocationCreated> AllocationCreated { get; set; }
}