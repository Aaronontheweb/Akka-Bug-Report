using Akka.DependencyInjection;
using Akka.TestKit.Xunit2;
using AkkaTest.Actors;
using AkkaTest.Messages;
using Xunit.Abstractions;
using Xunit;

namespace AkkaTest;

[Collection(nameof(TestActorFixture))]
public class UnitTestAkka: TestKit
{
    private readonly TestActorFixture _context;

    public UnitTestAkka(TestActorFixture context, ITestOutputHelper testOutputHelper)
        : base(context.ActorSystem, testOutputHelper )
    {
        _context = context;
    }
    
    [Fact]
    public async Task ExpectInitAndStartResponse()
    {
        var initActor = _context.ActorSystem
            .ActorOf(DependencyResolver.For(_context.ActorSystem)
                .Props<InitActor>());
        
        var probe = CreateTestProbe();
        initActor.Tell(probe.Ref, TestActor);
        
        initActor.Tell(new InitRequestMessage(), TestActor);
        
        await ExpectMsgAsync<InitResponseMessage>(TimeSpan.FromSeconds(2));
        await ExpectMsgAsync<StartRequestMessage>(TimeSpan.FromSeconds(2));
    }
}


