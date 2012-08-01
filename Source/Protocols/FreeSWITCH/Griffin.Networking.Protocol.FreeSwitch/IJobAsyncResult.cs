using System;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    /// <summary>
    /// Our own result when doing asynchronous things.
    /// </summary>
    internal interface IJobAsyncResult : IAsyncResult
    {
        object Result { get; }
    }
}