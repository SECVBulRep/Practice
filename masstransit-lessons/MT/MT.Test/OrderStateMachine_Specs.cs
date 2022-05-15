using MassTransit.Testing;
using MT.SampleComponents.Consumers;
using MT.SampleComponents.StateMachine;
using MT.SampleContracts;
using NUnit.Framework;

namespace MT.Test;

[TestFixture]
public class Submitting_an_order
{
    [Test]
    public async Task Should_create_an_instance()
    {
        var orderStateMachine = new OrderStateMachine();
        var harness = new InMemoryTestHarness();
        var saga = harness.StateMachineSaga<OrderState, OrderStateMachine>(orderStateMachine);

        await harness.Start();
        try
        {
            var orderId = Guid.NewGuid();
            var stamp = DateTime.UtcNow;
            var customerId = "Bob";
            
            await harness.Bus.Publish<IOrderSubmitted>(new
            {
                OrderId = orderId,
                TimeStamp = stamp,
                CustomerNumber = customerId
            });

            Assert.That(saga.Created.Select(x => x.CorrelationId == orderId).Any(), Is.True);
            
            // упадет тест
            //сначала вот это 
            //await Task.Delay(1000);
            // потом вот это 
           await saga.Exists(orderId, x => x.Submitted);
            
            var instance = saga.Created.Contains(orderId);
            Assert.That(instance,Is.Not.Null);
            Assert.That(instance.CurrentState,Is.EqualTo(orderStateMachine.Submitted.Name));
            Assert.That(instance.CustomerNumber,Is.EqualTo(customerId));
            
            
        }
        finally
        {
            await harness.Stop();
        }
    }
}