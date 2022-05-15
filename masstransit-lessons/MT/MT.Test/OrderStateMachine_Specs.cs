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
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.CurrentState, Is.EqualTo(orderStateMachine.Submitted.Name));
            Assert.That(instance.CustomerNumber, Is.EqualTo(customerId));
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task Should_response_to_status_check()
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
            await saga.Exists(orderId, x => x.Submitted);

            var instance = saga.Created.Contains(orderId);
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.CurrentState, Is.EqualTo(orderStateMachine.Submitted.Name));
            Assert.That(instance.CustomerNumber, Is.EqualTo(customerId));

            await harness.Bus.Publish<ICustomerAccountClosed>(new
            {
                CustomerId = default(Guid),
                CustomerNumber = customerId,
                Stamp = DateTime.Now
            }); 
            
            
            var requestClient = await harness.ConnectRequestClient<ICheckOrder>();
            var response = await requestClient.GetResponse<IOrderStatus>(new {OrderId = orderId});
            
            
            Assert.That(response.Message.State,Is.EqualTo(orderStateMachine.Canceled.Name));
        }
        finally
        {
            await harness.Stop();
        }
    }
    
    
    
    [Test]
    public async Task Should_cancel_when_account_closed()
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
            await saga.Exists(orderId, x => x.Submitted);

            var instance = saga.Created.Contains(orderId);
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.CurrentState, Is.EqualTo(orderStateMachine.Submitted.Name));
            Assert.That(instance.CustomerNumber, Is.EqualTo(customerId));

            var requestClient = await harness.ConnectRequestClient<ICheckOrder>();
            var response = await requestClient.GetResponse<IOrderStatus>(new {OrderId = orderId});
            
            
            Assert.That(response.Message.State,Is.EqualTo(orderStateMachine.Submitted.Name));
        }
        finally
        {
            await harness.Stop();
        }
    }
}