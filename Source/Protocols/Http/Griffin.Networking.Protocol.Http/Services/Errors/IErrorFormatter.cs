using System;
using Griffin.Networking.Http.Protocol;

namespace Griffin.Networking.Http.Services.Errors
{
    /// <summary>
    /// Takes a <see cref="Exception"/> and formats the <see cref="IResponse"/> accordingly.
    /// </summary>
    public interface IErrorFormatter
    {
        /// <summary>
        /// Format the response into something that the user understands.
        /// </summary>
        /// <param name="context">Context providing information for the error message generation</param>
        void Format(ErrorFormatterContext context);
    }
}