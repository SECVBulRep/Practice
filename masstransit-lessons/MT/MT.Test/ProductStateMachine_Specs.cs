using MassTransit.Testing;
using MT.Test.Internals;
using NUnit.Framework;
using Warehouse.Components.StateMachines;
using Warehouse.Contracts;

namespace MT.Test;

public class When_a_product_is_added :
    StateMachineTestFixture<ProductStateMachine, Product>
{
    [Test]
    public async Task Should_create_a_saga_instance()
    {
        var productId = Guid.NewGuid();

        await TestHarness.Bus.Publish<IProductAdded>(new
        {
            ProductId = productId,
            ManufacturerId = "0307969959",
            Name = "ps 5"
        });

        Assert.IsTrue(await TestHarness.Consumed.Any<IProductAdded>(), "Message not consumed");

        Assert.IsTrue(await SagaHarness.Consumed.Any<IProductAdded>(), "Message not consumed by saga");

        Assert.That(await SagaHarness.Created.Any(x => x.CorrelationId == productId));

        var instance = SagaHarness.Created.ContainsInState(productId, Machine, Machine.Available);
        Assert.IsNotNull(instance, "Saga instance not found");

        var existsId = await SagaHarness.Exists(productId, x => x.Available);
        Assert.IsTrue(existsId.HasValue, "Saga did not exist");
    }
}