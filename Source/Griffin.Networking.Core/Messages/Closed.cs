using Griffin.Networking.Messages;

namespace Griffin.Networking.Channel
{
    /// <summary>
    /// Channel/pipeline will be closed after this message have been handled.
    /// </summary>
    public class Closed : IPipelineMessage
    {
    }
}