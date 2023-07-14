using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

namespace OptimizeMe.App;

public class AntiVirusFriendlyConfig : ManualConfig
{
    public AntiVirusFriendlyConfig() => this.AddJob(Job.MediumRun.WithToolchain(InProcessNoEmitToolchain.Instance));
}