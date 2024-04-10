using Akka.Actor;
using AkkaTest.Messages;

namespace AkkaTest.Actors;

public class InitActor : ReceiveActor
{
    public InitActor()
    {
        ReceiveAsync<InitRequestMessage>(Handler);
    }

    private async Task Handler(InitRequestMessage initMsg)
    {
        // Do something
        await Task.Delay(1);
        
        Sender.Tell(new InitResponseMessage(), Self);
        
        //await Task.Delay(1); // Uncomment this line to fix the test, with 1.5.15 works without uncomment
        
        Sender.Tell(new StartRequestMessage(), Self);
    }
}