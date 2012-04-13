using System;
using System.Diagnostics;

namespace Griffin.Networking
{
    public class LogManager
    {
        private static LogManager _current;
        private static readonly object _synLock = new object();

        public static ILogger GetLogger<T>() where T : class
        {
            return Current.GetLoggerInternal(typeof(T));
        }

        public static ILogger GetLogger(Type loggingType)
        {
            return Current.GetLoggerInternal(loggingType);
        }

        protected virtual ILogger GetLoggerInternal(Type loggingType)
        {
            return new NullLogger();
        }

        public static void Assign(LogManager logManager)
        {
            _current = logManager;
        }

        public static LogManager Current
        {
            get
            {
                if (_current == null)
                {
                    lock (_synLock)
                    {
                        if (_current == null)
                        {
                            //not a problem on x86 & x64
                            // ReSharper disable PossibleMultipleWriteAccessInDoubleCheckLocking
                            _current = new LogManager();
                            // ReSharper restore PossibleMultipleWriteAccessInDoubleCheckLocking
                        }
                    }
                }

                return _current;
            }
        }
    }

    public class SimpleFilteredLogManager : LogManager
    {
        protected override ILogger GetLoggerInternal(Type loggingType)
        {
            return new FilteredLogger(LogLevel.Debug, loggingType);
        }
    }

    public class SimpleSystemDebugLogManager : LogManager
    {

        protected override ILogger GetLoggerInternal(Type loggingType)
        {
            return new SystemDebugLogger(loggingType);
        }
    }
    public class ConsoleLogManager : LogManager
    {

        protected override ILogger GetLoggerInternal(Type loggingType)
        {
            return new ConsoleLogger(loggingType);
        }
    }

    public interface ILogger
    {
        void Trace(string message);
        void Trace(string message, Exception exception);
        void Debug(string message);
        void Debug(string message, Exception exception);
        void Warning(string message);
        void Warning(string message, Exception exception);
        void Error(string message);
        void Error(string message, Exception exception);
    }

    public class FilteredLogger : SystemDebugLogger
    {
        private readonly LogLevel _minLevel;

        public FilteredLogger(LogLevel minLevel, Type loggedType) : base(loggedType)
        {
            _minLevel = minLevel;
        }

        protected override int SkipFrameCount
        {
            get { return base.SkipFrameCount + 1; }
        }

        protected override void Write(LogLevel logLevel, string msg, Exception exception)
        {
            if(logLevel < _minLevel)
                return;

            base.Write(logLevel, msg, exception);
        }
    }
    public abstract class BaseLogger : ILogger
    {
        private readonly Type _loggedType;

        protected BaseLogger(Type loggedType)
        {
            _loggedType = loggedType;
        }

        /// <summary>
        /// Gets number of frames to skip when using stack trace
        /// </summary>
        protected virtual int SkipFrameCount
        {
            get { return 1; }
        }

        public Type LoggedType
        {
            get { return _loggedType; }
        }

        public void Trace(string message)
        {
            Write(LogLevel.Trace, message, null);
        }

        public void Trace(string message, Exception exception)
        {
            Write(LogLevel.Trace, message, exception);
        }

        public void Debug(string message)
        {
            Write(LogLevel.Debug, message, null);
        }

        public void Debug(string message, Exception exception)
        {
            Write(LogLevel.Debug, message, exception);
        }

        public void Warning(string message)
        {
            Write(LogLevel.Warning, message, null);
        }

        public void Warning(string message, Exception exception)
        {
            Write(LogLevel.Warning, message, exception);
        }

        public void Error(string message)
        {
            Write(LogLevel.Error, message, null);
        }

        public void Error(string message, Exception exception)
        {
            Write(LogLevel.Error, message, exception);
        }

        protected abstract void Write(LogLevel logLevel, string msg, Exception exception);

        protected virtual string BuildExceptionDetails(Exception exception, int spaces)
        {
            var buffer = "".PadLeft(spaces) + exception + "\r\n";
            if (exception.InnerException != null)
                buffer += BuildExceptionDetails(exception.InnerException, spaces + 4);

            return buffer;
        }
    }

    public class NullLogger : ILogger
    {
        public void Trace(string message)
        {
            
        }

        public void Trace(string message, Exception exception)
        {
        }

        public void Debug(string message)
        {

        }

        public void Debug(string message, Exception exception)
        {

        }

        public void Warning(string message)
        {

        }

        public void Warning(string message, Exception exception)
        {

        }

        public void Error(string message)
        {

        }

        public void Error(string message, Exception exception)
        {

        }
    }

    public class SystemDebugLogger : BaseLogger
    {
        public SystemDebugLogger(Type loggedType)
            : base(loggedType)
        {
        }

        protected override int SkipFrameCount
        {
            get { return base.SkipFrameCount + 1; }
        }

        protected override void Write(LogLevel logLevel, string msg, Exception exception)
        {
            var frame = new StackTrace(SkipFrameCount).GetFrame(0);
            var caller = frame.GetMethod().ReflectedType.Name + "." +
                         frame.GetMethod().Name + "():" + frame.GetFileLineNumber();

            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " " + caller.PadRight(50) + logLevel.ToString().PadRight(10) + msg);
            if (exception != null)
                System.Diagnostics.Debug.WriteLine(BuildExceptionDetails(exception, 4));
        }

    }
    public class ConsoleLogger : BaseLogger
    {
        public ConsoleLogger(Type loggedType)
            : base(loggedType)
        {
        }

        protected override int SkipFrameCount
        {
            get { return base.SkipFrameCount + 1; }
        }

        protected override void Write(LogLevel logLevel, string msg, Exception exception)
        {
            var frame = new StackTrace(SkipFrameCount).GetFrame(0);
            var caller = frame.GetMethod().ReflectedType.Name + "." +
                         frame.GetMethod().Name + "():" + frame.GetFileLineNumber();

            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " " + caller.PadRight(50) + logLevel.ToString().PadRight(10) + msg);
            if (exception != null)
                Console.WriteLine(BuildExceptionDetails(exception, 4));
        }

    }

    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warning,
        Error, 
        Fatal
    }
}
