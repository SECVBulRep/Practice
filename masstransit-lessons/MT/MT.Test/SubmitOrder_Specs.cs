using MassTransit.Testing;
using MT.SampleComponents.Consumers;
using MT.SampleContracts;
using NUnit.Framework;

[TestFixture]
public class When_an_order_request_is_consumed
{
    [Test]
    public async Task Shuold_response_with_acceptance_if_ok()
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
}