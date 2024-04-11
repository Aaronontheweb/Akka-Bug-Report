using Akka.Actor;
using Akka.Event;
using AkkaTest.Messages;

namespace AkkaTest.Actors;

public class InitActor : ReceiveActor
{
    private readonly ILoggingAdapter _logger = Context.GetLogger();
    
    public InitActor()
    {
        ReceiveAsync<InitRequestMessage>(Handler);
    }

    private async Task Handler(InitRequestMessage initMsg)
    {
        _logger.Info("First step - Sender: {0}", Sender);
        
        // Do something'
        await Task.Delay(1);
        
        Sender.Tell(new InitResponseMessage(), Self);
        
        _logger.Info("Second step - Sender: {0}", Sender);
        
        //await Task.Delay(1); // Uncomment this line to fix the test, with 1.5.15 works without uncomment
        
        Sender.Tell(new StartRequestMessage(), Self);
        
        _logger.Info("Third step - Sender: {0}", Sender);
    }
}