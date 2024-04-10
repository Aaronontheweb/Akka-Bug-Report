using Akka.Actor;

namespace AkkaTest.Services;

public interface IAkkaService
{
    public ActorSystem ActorSystem { get; }
    bool Started { get; }
    Task StartAsync(CancellationToken cancellationToken);
}