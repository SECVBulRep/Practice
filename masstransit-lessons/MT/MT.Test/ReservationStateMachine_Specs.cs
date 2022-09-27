﻿using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
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


public class When_a_product_reservation_is_requested_for_an_avialable_product :
    StateMachineTestFixture<ReservationStateMacine, Reservation>
{
    [Test]
    public async Task Should_create_a_saga_instance()
    {
        var productId = Guid.NewGuid();
        var reservationId = Guid.NewGuid();
        var ClientId = Guid.NewGuid();
       

        await TestHarness.Bus.Publish<IProductAdded>(new
        {
            ProductId = productId,
            ManufacturerId = "0307969959",
            Name = "ps 5"
        });

        var existsId = await ProductSagaHarness.Exists(productId, x => x.Available);
        Assert.IsTrue(existsId.HasValue, "Saga did not exist");
        
        
        await TestHarness.Bus.Publish<IReservationRequested>(new
        {
            ProductId = productId,
            ReservationId = reservationId,
            TimeStamp = DateTime.Now,
            ClientId = ClientId
        });

      
        Assert.IsTrue(await SagaHarness.Consumed.Any<IReservationRequested>(), "Message not consumed by saga");
        Assert.IsTrue(await ProductSagaHarness.Consumed.Any<IReservationRequested>(), "Message not consumed by saga");
       
        var reservation =  SagaHarness.Sagas.ContainsInState(reservationId,Machine, x => x.Reserved);
        
        Assert.IsNotNull(reservation, "Saga did not exist");
    }

    [OneTimeSetUp]
    public void SetUp()
    {
        ProductSagaHarness = Provider.GetRequiredService<IStateMachineSagaTestHarness<Product, ProductStateMachine>>();
        ProductMachine = Provider.GetRequiredService<ProductStateMachine>();
    }

    public ProductStateMachine ProductMachine { get; set; }

    public IStateMachineSagaTestHarness<Product, ProductStateMachine> ProductSagaHarness { get; set; }


    protected override void ConfigureMasstransit(IBusRegistrationConfigurator cfg)
    {
        cfg.AddSagaStateMachine<ProductStateMachine, Product>()
            .InMemoryRepository();
        cfg.AddPublishMessageScheduler();
        cfg.AddSagaStateMachineTestHarness<ProductStateMachine, Product>();
    }
}