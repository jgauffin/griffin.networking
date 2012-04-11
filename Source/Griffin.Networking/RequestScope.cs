using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Networking
{
    public class RequestScope
    {
        static  List<IScopeListener> _listeners = new List<IScopeListener>(); 
        public static void Subscribe(IScopeListener listener)
        {
            _listeners.Add(listener);
        }

        internal static void Begin()
        {
            foreach (var listener in _listeners)
            {
                listener.ScopeBegins();
            }
        }

        internal static void ScopeEnds()
        {
            foreach (var listener in _listeners)
            {
                listener.ScopeEnds();
            }
        }

    }

    public interface IScopeListener
    {
        void ScopeBegins();
        void ScopeEnds();
    }
}
