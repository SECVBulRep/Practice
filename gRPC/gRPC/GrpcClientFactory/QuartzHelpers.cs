using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GrpcClientFactory
{
    public static class QuartzServices
    {
        public static void InitQuartz(this IServiceCollection services, IConfiguration configuration,
            IHostEnvironment environment)
        {
            services.Configure<QuartzOptions>(configuration.GetSection("Quartz"));

            var options = new QuartzOptions();
            configuration.Bind("Quartz", options);
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionScopedJobFactory();
 
                if (environment.IsDevelopment())
                {
                    q.SchedulerName = "Ait.App.Background.Test." + Environment.MachineName;
                }
                q.AddJobAndTrigger<TestJob>(configuration);
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }
    }
    
     public static class ServiceCollectionQuartzConfiguratorExtensions
    {
        public static void AddJobAndTrigger<T>(
            this IServiceCollectionQuartzConfigurator quartz,
            IConfiguration config)
            where T : IJob
        {
            string jobName;
            string schedName;

            if (typeof(T).BaseType?.Name == nameof(QuartzJob))
            {
                jobName = $"QuartzJob<{typeof(T).GetGenericArguments().First().FullName}>";
                schedName = typeof(T).GetGenericArguments().First().Name;
            }
            else
            {
                jobName = typeof(T).Name;
                schedName = jobName;
            }

            // Try and load the schedule from configuration
            var configKey = $"Jobs:{schedName}";
            var cronSchedule = config[configKey];

            // Some minor validation
            if (string.IsNullOrEmpty(cronSchedule))
            {
                return;
            }

            // register the job as before
            var jobKey = new JobKey(jobName);
            quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

            quartz.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity(jobName + ".trigger")
                .WithCronSchedule(cronSchedule)); // use the schedule from configuration
        }
    }
    
    public abstract class QuartzJob : IJob, IDisposable
    {
        protected readonly IServiceScope Scope;
        protected readonly ILogger<QuartzJob> Logger;

        protected QuartzJob(IServiceProvider serviceProvider, ILogger<QuartzJob> logger)
        {
            Logger = logger;
            Scope = serviceProvider.CreateScope();
        }

        public abstract Task Execute(IJobExecutionContext context);

        public void Dispose()
        {
            Scope?.Dispose();
        }
    }

    [DisallowConcurrentExecution]
    public class QuartzJob<T> : QuartzJob
        where T : IExecutable
    {
        public QuartzJob(IServiceProvider serviceProvider, ILogger<QuartzJob> logger)
            : base(serviceProvider, logger)
        {
        }

        public override async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Scope.ServiceProvider.GetService<T>().Execute(context.MergedJobDataMap.WrappedMap);
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
            }
        }
    }
    
    public interface IExecutable
    {
        Task Execute(IDictionary<string, object> parameters);
    }
}