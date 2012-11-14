using System;
using System.Diagnostics;

namespace Griffin.Networking.Logging
{
    /// <summary>
    /// Logs to the debug window in Visual Studio
    /// </summary>
    public class SystemDebugLogger : BaseLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemDebugLogger"/> class.
        /// </summary>
        /// <param name="loggedType">Type being logged.</param>
        public SystemDebugLogger(Type loggedType)
            : base(loggedType)
        {
        }

        /// <summary>
        /// Gets number of frames to skip when using stack trace
        /// </summary>
        protected override int SkipFrameCount
        {
            get { return base.SkipFrameCount + 1; }
        }

        /// <summary>
        /// Writes the specified log level.
        /// </summary>
        /// <param name="logLevel">Used level.</param>
        /// <param name="msg">Message.</param>
        /// <param name="exception">The exception (or null).</param>
        public override void Write(LogLevel logLevel, string msg, Exception exception)
        {
            var frame = new StackTrace(SkipFrameCount).GetFrame(0);
            var caller = frame.GetMethod().ReflectedType.Name + "." +
                         frame.GetMethod().Name + "():" + frame.GetFileLineNumber();

            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " " + caller.PadRight(50) +
                                               logLevel.ToString().PadRight(10) + msg);
            if (exception != null)
                System.Diagnostics.Debug.WriteLine(BuildExceptionDetails(exception, 4));
        }
    }
}