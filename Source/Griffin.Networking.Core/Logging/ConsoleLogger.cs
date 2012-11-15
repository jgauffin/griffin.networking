using System;
using System.Diagnostics;

namespace Griffin.Networking.Logging
{
    /// <summary>
    /// Log everything to the console
    /// </summary>
    /// <remarks>Prints one stack frame using colored output.</remarks>
    public class ConsoleLogger : BaseLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogger"/> class.
        /// </summary>
        /// <param name="loggedType">Type being logged.</param>
        public ConsoleLogger(Type loggedType)
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

            var color = Console.ForegroundColor;
            Console.ForegroundColor = GetColor(logLevel);
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " " + caller.PadRight(50) +
                              logLevel.ToString().PadRight(10) + msg);
            if (exception != null)
                Console.WriteLine(BuildExceptionDetails(exception, 4));
            Console.ForegroundColor = color;
        }

        /// <summary>
        /// Get a color for a specific log level
        /// </summary>
        /// <param name="logLevel">Level to get color for</param>
        /// <returns>Level color</returns>
        protected virtual ConsoleColor GetColor(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return ConsoleColor.DarkGray;
                case LogLevel.Debug:
                    return ConsoleColor.Gray;
                case LogLevel.Info:
                    return ConsoleColor.White;
                case LogLevel.Warning:
                    return ConsoleColor.Yellow;
                case LogLevel.Error:
                    return ConsoleColor.Red;
                default:
                    return ConsoleColor.Blue;
            }
        }
    }
}