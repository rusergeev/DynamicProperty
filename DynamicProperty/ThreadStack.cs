using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DynamicProperty
{
    /// <summary>
    /// builds stacks for a current thread, can work with several threads
    /// </summary>
    [Obsolete]
    sealed class ThreadStack
    {
        private ThreadStack()
        {
        }
        /// <summary>
        /// Singleton standard pattern
        /// </summary>
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
        /// <summary>
        /// pushes an <see cref="IDependencyTarget"/> to current thread stack
        /// </summary>
        /// <param name="target"> a dependency target object </param>
        public void Push(IDependencyTarget target)
        {
            Current.Push(target);
        }
        /// <summary>
        /// pops an <see cref="IDependencyTarget"/> from current thread stack
        /// </summary>
        /// <returns>a dependency target object </returns>
        public IDependencyTarget Pop()
        {
            var target = Current.Pop();
            if (!Current.Any())
            {
                _map.Remove(Thread.CurrentThread.ManagedThreadId);
            }
            return target;
        }
        /// <summary>
        /// peaks an <see cref="IDependencyTarget"/> from current thread stack
        /// </summary>
        /// <returns>a dependency target object</returns>
        public IDependencyTarget Peek()
        {
            return Current.Peek();
        }
        /// <summary>
        /// Indicates if there stack is not empty
        /// </summary>
        /// <returns> true if there is any target dependency object on the current thread stack </returns>
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