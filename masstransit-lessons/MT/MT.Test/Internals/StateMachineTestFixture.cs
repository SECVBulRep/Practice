﻿using MassTransit.ExtensionsDependencyInjectionIntegration;

namespace MT.Test.Internals;
using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Quartz;


 public class StateMachineTestFixture<TStateMachine, TInstance>
        where TStateMachine : class, SagaStateMachine<TInstance>
        where TInstance : class, SagaStateMachineInstance
    {
        ISchedulerFactory _scheduler;
        TimeSpan _testOffset;
        protected TStateMachine Machine;
        protected ServiceProvider Provider;
        protected IStateMachineSagaTestHarness<TInstance, TStateMachine> SagaHarness;
        protected InMemoryTestHarness TestHarness;

        [OneTimeSetUp]
        public async Task Setup()
        {
            InterceptQuartzSystemTime();

            var collection = new ServiceCollection()
                .AddSingleton<ILoggerFactory>(provider => new TestOutputLoggerFactory(true))
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddSagaStateMachine<TStateMachine, TInstance>()
                        .InMemoryRepository();
                    cfg.AddPublishMessageScheduler();
                    cfg.AddSagaStateMachineTestHarness<TStateMachine, TInstance>();
                    
                    ConfigureMasstransit(cfg);
                });

            ConfigureServices(collection);
            Provider = collection.BuildServiceProvider(true);
            ConfigureLogging();

            TestHarness = Provider.GetRequiredService<InMemoryTestHarness>();
            TestHarness.OnConfigureInMemoryBus += configurator =>
            {
                configurator.UseInMemoryScheduler(out _scheduler);
            };

            await TestHarness.Start();

            SagaHarness = Provider.GetRequiredService<IStateMachineSagaTestHarness<TInstance, TStateMachine>>();
            Machine = Provider.GetRequiredService<TStateMachine>();
        }

        protected virtual void ConfigureMasstransit(IBusRegistrationConfigurator serviceCollectionBusConfigurator)
        {
        }
        
        
        protected virtual void ConfigureServices(IServiceCollection collection)
        {
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            try
            {
                await TestHarness.Stop();
            }
            finally
            {
                await Provider.DisposeAsync();
            }

            RestoreDefaultQuartzSystemTime();
        }

        protected async Task AdvanceSystemTime(TimeSpan duration)
        {
            if (duration <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(duration));

            var scheduler = await _scheduler.GetScheduler().ConfigureAwait(false);

            await scheduler.Standby().ConfigureAwait(false);

            _testOffset += duration;

            await scheduler.Start().ConfigureAwait(false);
        }

        void ConfigureLogging()
        {
            var loggerFactory = Provider.GetRequiredService<ILoggerFactory>();

            LogContext.ConfigureCurrentLogContext(loggerFactory);
            Quartz.Logging.LogContext.SetCurrentLogProvider(loggerFactory);
        }

        void InterceptQuartzSystemTime()
        {
            SystemTime.UtcNow = GetUtcNow;
            SystemTime.Now = GetNow;
        }

        static void RestoreDefaultQuartzSystemTime()
        {
            SystemTime.UtcNow = () => DateTimeOffset.UtcNow;
            SystemTime.Now = () => DateTimeOffset.Now;
        }

        DateTimeOffset GetUtcNow()
        {
            return DateTimeOffset.UtcNow + _testOffset;
        }

        DateTimeOffset GetNow()
        {
            return DateTimeOffset.Now + _testOffset;
        }
    }