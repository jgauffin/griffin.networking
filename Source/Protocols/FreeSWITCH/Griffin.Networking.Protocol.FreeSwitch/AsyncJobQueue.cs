using System;
using System.Collections.Generic;
using System.Threading;

namespace Griffin.Networking.Protocol.FreeSwitch
{
    public class AsyncJobQueue
    {
        private readonly Dictionary<object, Wrapper> _items = new Dictionary<object, Wrapper>();

        public IAsyncResult Enqueue(object request, AsyncCallback callback, object state)
        {
            var wrapper = new Wrapper(callback, state);
            lock (_items)
            {
                _items.Add(request, wrapper);
            }

            return wrapper;
        }

        public void Trigger(object request, object result)
        {
            lock (_items)
            {
                Wrapper wrapper;
                if (_items.TryGetValue(request, out wrapper))
                {
                    _items.Remove(request);
                    wrapper.Result = result;
                    wrapper.Invoke();
                }
            }
        }

        //TODO: Resend them instead since we got the original commands in the queue.
        public void Clear()
        {
            lock (_items)
            {
                _items.Clear();
            }
        }

        #region Nested type: Wrapper

        private class Wrapper : IJobAsyncResult
        {
            private readonly AsyncCallback _callback;
            private readonly ManualResetEvent _event = new ManualResetEvent(false);

            public Wrapper(AsyncCallback callback, object state)
            {
                _callback = callback;
                AsyncState = state;
            }

            #region IJobAsyncResult Members

            /// <summary>
            /// Gets a value that indicates whether the asynchronous operation has completed.
            /// </summary>
            /// <returns>
            /// true if the operation is complete; otherwise, false.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public bool IsCompleted { get; set; }

            /// <summary>
            /// Gets a <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public WaitHandle AsyncWaitHandle
            {
                get { return _event; }
            }

            /// <summary>
            /// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
            /// </summary>
            /// <returns>
            /// A user-defined object that qualifies or contains information about an asynchronous operation.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public object AsyncState { get; set; }

            /// <summary>
            /// Gets a value that indicates whether the asynchronous operation completed synchronously.
            /// </summary>
            /// <returns>
            /// true if the asynchronous operation completed synchronously; otherwise, false.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public bool CompletedSynchronously { get; set; }

            public object Result { get; set; }

            #endregion

            public void Invoke()
            {
                _event.Set();
                _callback(this);
            }
        }

        #endregion
    }
}