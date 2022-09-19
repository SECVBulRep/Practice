using MassTransit.Testing;
using MT.Test.Internals;
using NUnit.Framework;
using Warehouse.Components.StateMachines;
using Warehouse.Contracts;

namespace MT.Test;

public class When_a_resevation_is_added :
    StateMachineTestFixture<ReservationStateMacine, Reservation>
{
    [Test]
    public async Task Should_create_a_saga_instance()
    {
        var productId = Guid.NewGuid();
        var reservationId = Guid.NewGuid();
        var ClientId = Guid.NewGuid();

        await TestHarness.Bus.Publish<IReservationRequested>(new
        {
            ProductId = productId,
            ReservationId = reservationId,
            TimeStamp = DateTime.Now,
            ClientId = ClientId
        });

        Assert.IsTrue(await TestHarness.Consumed.Any<IReservationRequested>(), "Message not consumed");

        Assert.IsTrue(await SagaHarness.Consumed.Any<IReservationRequested>(), "Message not consumed by saga");

        Assert.That(await SagaHarness.Created.Any(x => x.CorrelationId == reservationId));

        var instance = SagaHarness.Created.ContainsInState(reservationId, Machine, Machine.Requested);
        Assert.IsNotNull(instance, "Saga instance not found");

        var existsId = await SagaHarness.Exists(reservationId, x => x.Requested);
        Assert.IsTrue(existsId.HasValue, "Saga did not exist");
    }
}