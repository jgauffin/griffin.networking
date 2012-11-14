using Griffin.Networking.JsonRpc;
using Griffin.Networking.JsonRpc.Messages;
using Griffin.Networking.Pipelines;

namespace ServerDemo
{
    internal class MyApplication : IUpstreamHandler
    {
        #region IUpstreamHandler Members

        /// <summary>
        /// Handle an message
        /// </summary>
        /// <param name="context">Context unique for this handler instance</param>
        /// <param name="message">Message to process</param>
        /// <remarks>
        /// All messages that can't be handled MUST be send up the chain using <see cref="IPipelineHandlerContext.SendUpstream"/>.
        /// </remarks>
        public void HandleUpstream(IPipelineHandlerContext context, IPipelineMessage message)
        {
            var msg = message as ReceivedRequest;
            if (msg == null)
                return;


            var parray = msg.Request.Parameters as object[];
            if (parray == null)
                return; // muhahaha, violating the API specification

            object result;
            switch (msg.Request.Method)
            {
                case "add":
                    result = int.Parse(parray[0].ToString()) + int.Parse(parray[0].ToString());
                    break;
                case "substract":
                    result = int.Parse(parray[0].ToString()) + int.Parse(parray[0].ToString());
                    break;
                default:
                    result = "Nothing useful.";
                    break;
            }

            var response = new Response(msg.Request.Id, result);
            context.SendDownstream(new SendResponse(response));
        }

        #endregion
    }
}