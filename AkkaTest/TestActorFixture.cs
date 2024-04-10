using Akka.Actor;
using AkkaTest.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace AkkaTest;

public class TestActorFixture : IDisposable
{
    private readonly IServiceProvider _providers;
    private readonly IHost _host;

    public TestActorFixture()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .MinimumLevel.Debug()
                    .CreateLogger();

                services.AddSingleton(Log.Logger);
                services.AddSingleton<IAkkaService, AkkaService>(sp => new AkkaService(sp, null));
                services.AddHostedService<AkkaService>(sp => (AkkaService)sp.GetRequiredService<IAkkaService>());
            })
            .Build();

        _host.StartAsync();
        _providers = _host.Services;
    }

    public ActorSystem ActorSystem
    {
        get
        {
            var akkaService = _providers.GetRequiredService<IAkkaService>();

            while (!akkaService.Started)
                Task.Delay(50);

            return akkaService.ActorSystem;
        }
    }

    public void Dispose()
    {
        var actorSystem = ActorSystem;

        CoordinatedShutdown.Get(actorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);

        actorSystem.WhenTerminated.Wait();

        _host.StopAsync().Wait();

        GC.SuppressFinalize(this);
    }
}