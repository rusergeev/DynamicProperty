using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Developer.Test
{
    sealed class ThreadStack
    {
        private ThreadStack()
        {
        }

        public static ThreadStack Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_protection)
                    {
                        _instance = new ThreadStack();
                    }
                }
                return _instance;
            }
        }

        public Stack<IDependencyTarget> Current
        {
            get
            {
                var id = Thread.CurrentThread.ManagedThreadId;
                if (!_instance._map.ContainsKey(id))
                    _instance._map[id] = new Stack<IDependencyTarget>();
                return _instance._map[id];
            }
        }

        private static volatile ThreadStack _instance;
        private static readonly object _protection = new object();
        //todo: remove key if empty
        private readonly IDictionary<int, Stack<IDependencyTarget>> _map =
            new ConcurrentDictionary<int, Stack<IDependencyTarget>>();
    }
}