using MassTransit;

namespace MT.SampleComponents.CourierAcitivities;

public class InformAllocateInventoryActivity : IActivity<IInformAllocateInventoryArguments, InformAllocateInventoryLog>
{
    public async Task<ExecutionResult> Execute(ExecuteContext<IInformAllocateInventoryArguments> context)
    {
        throw new Exception();
        
        return context.Completed(new
        {
            AlocationId = Guid.NewGuid()
        });
    }

    public async Task<CompensationResult> Compensate(CompensateContext<InformAllocateInventoryLog> context)
    {
        return context.Compensated();
    }
}