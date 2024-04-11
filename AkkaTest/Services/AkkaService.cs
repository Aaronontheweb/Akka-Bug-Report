using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AkkaTest.Services;

public class AkkaService : IAkkaService, IHostedService
{
    private ActorSystem _actorSystem = null;
    private readonly string _customHocon;
    private readonly IServiceProvider _sp;
    
    // generate HOCON to ensure all test actors run on the default dispatcher
    private static readonly Config Hocon = @"
                                           akka.test.test-actor.dispatcher = akka.actor.default-dispatcher
                                           ";

    public AkkaService(IServiceProvider sp, string customHocon)
    {
        _sp = sp;
        _customHocon = customHocon;
    }

    public ActorSystem ActorSystem => _actorSystem;

    public bool Started { get; private set; }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var bootstrap = _customHocon != null ?
            BootstrapSetup.Create().WithConfig(Hocon.SafeWithFallback(_customHocon)) :
            BootstrapSetup.Create();
        var di = DependencyResolverSetup.Create(_sp);
        var actorSystemSetup = bootstrap.And(di);
        _actorSystem = ActorSystem.Create("testActorSystem", actorSystemSetup);

        Started = true;

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await CoordinatedShutdown
            .Get(_actorSystem)
            .Run(CoordinatedShutdown.ClrExitReason.Instance);

        await _actorSystem.WhenTerminated;
    }
}