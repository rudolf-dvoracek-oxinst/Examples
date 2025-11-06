// ReSharper disable once CheckNamespace
namespace HelloWorldService;

public partial class HelloReply
{
    /// <summary>
    /// Constructor for the HelloReply message
    /// </summary>
    /// <param name="message"> The message to be sent in the reply </param>
    public HelloReply(string message)
    {
        this.Message = message;
    }
}