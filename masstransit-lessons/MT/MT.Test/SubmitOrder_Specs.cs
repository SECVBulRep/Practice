using MassTransit.Testing;
using MT.SampleComponents.Consumers;
using MT.SampleContracts;
using NUnit.Framework;

[TestFixture]
public class When_an_order_request_is_consumed
{
    [Test]
    public async Task Should_response_with_acceptance_if_ok()
    {
        var harness = new InMemoryTestHarness();
        var consumer = harness.Consumer<SubmitOrderConsumer>();

        await harness.Start();
        try
        {

            var orderId = Guid.NewGuid();
            var stamp = DateTime.UtcNow;
            var customerId = "Bob";

            await harness.InputQueueSendEndpoint.Send<ISubmitOrder>(new
            {
                OrderId = orderId,
                TimeStamp = stamp,
                CustomerNumber = customerId
            });

            Assert.That(consumer.Consumed.Select<ISubmitOrder>().Any(), Is.True);

        }
        finally
        {
            await harness.Stop();
        }
    }
    
    [Test]
    public async Task Should_response_with_acceptance_if_ok2()
    {
        var harness = new InMemoryTestHarness();
        var consumer = harness.Consumer<SubmitOrderConsumer>();

        await harness.Start();
        try
        {

            var orderId = Guid.NewGuid();
            var stamp = DateTime.UtcNow;
            var customerId = "Bob";

            var request_client = await harness.ConnectRequestClient<ISubmitOrder>();
            
            var response = await request_client.GetResponse<IOrderSubmissionAccepted>(new
            {
                OrderId = orderId,
                TimeStamp = stamp,
                CustomerNumber = customerId
            });

            Assert.That(consumer.Consumed.Select<ISubmitOrder>().Any(), Is.True);
            Assert.That(response.Message.OrderId, Is.EqualTo(orderId));
            Assert.That(harness.Sent.Select<IOrderSubmissionAccepted>().Any,Is.True);

        }
        finally
        {
            await harness.Stop();
        }
    }
    
     [Test]
    public async Task Should_response_with_rejected_if_ok2()
    {
        var harness = new InMemoryTestHarness{TestTimeout = TimeSpan.FromSeconds(5)};
        var consumer = harness.Consumer<SubmitOrderConsumer>();

        await harness.Start();
        try
        {

            var orderId = Guid.NewGuid();
            var stamp = DateTime.UtcNow;
            var customerId = "Test";

            var request_client = await harness.ConnectRequestClient<ISubmitOrder>();
            
            var response = await request_client.GetResponse<IOrderSubmissionRejected>(new
            {
                OrderId = orderId,
                TimeStamp = stamp,
                CustomerNumber = customerId
            });

            Assert.That(consumer.Consumed.Select<ISubmitOrder>().Any(), Is.True);
            Assert.That(response.Message.OrderId, Is.EqualTo(orderId));
            Assert.That(harness.Sent.Select<IOrderSubmissionRejected>().Any,Is.True);

        }
        finally
        {
            await harness.Stop();
        }
    }
   
    
    [Test]
    public async Task Should_publish_order_submitted_event()
    {
        var harness = new InMemoryTestHarness();
        var consumer = harness.Consumer<SubmitOrderConsumer>();

        await harness.Start();
        try
        {

            var orderId = Guid.NewGuid();
            var stamp = DateTime.UtcNow;
            var customerId = "Bob";

            await harness.InputQueueSendEndpoint.Send<ISubmitOrder>(new
            {
                OrderId = orderId,
                TimeStamp = stamp,
                CustomerNumber = customerId
            });

            Assert.That(harness.Published.Select<IOrderSubmitted>().Any(), Is.True);

        }
        finally
        {
            await harness.Stop();
        }
    }
}