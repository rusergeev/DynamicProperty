using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        public void Push(IDependencyTarget target)
        {
            Current.Push(target);
        }

        public IDependencyTarget Pop()
        {
            var target = Current.Pop();
            if (!Current.Any())
            {
                _map.Remove(Thread.CurrentThread.ManagedThreadId);
            }
            return target;
        }

        public IDependencyTarget Peek()
        {
            return Current.Peek();
        }

        public bool Any()
        {
            return Current.Any();
        }

        private Stack<IDependencyTarget> Current
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

        private readonly IDictionary<int, Stack<IDependencyTarget>> _map =
            new ConcurrentDictionary<int, Stack<IDependencyTarget>>();
    }
}