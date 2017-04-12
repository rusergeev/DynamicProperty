using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DynamicProperty
{
    /// <summary>
    /// builds stacks for a current thread, can work with several threads
    /// </summary>
    class Transaction : IDisposable {
        /// <summary>
        ///
        /// </summary>
        /// <param name="dependency"> A Dynamic Property, which evaluates its dependencies </param>
        /// <returns> a disposable transaction </returns>
        public static IDisposable Instance(IDependency dependency) {
            var id = Thread.CurrentThread.ManagedThreadId;
            if (!_map.ContainsKey(id))
                _map[id] = new Transaction();
            _map[id].CheckIn(dependency);
            return _map[id];
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing resources:
        /// In the Transaction class, checks out a dependency from stack
        /// </summary>
        public void Dispose() {
            CheckOut();
        }
        private void CheckIn(IDependency dependency) {
            if (_stack.Any()){
                var last = _stack.Peek();
                last.DependsOn(dependency);
            }
            _stack.Push(dependency as IDependent);
        }
        private void CheckOut() {
            _stack.Pop();
            if (!_stack.Any()){
                var id = Thread.CurrentThread.ManagedThreadId;
                _map.Remove(id);
            }
        }
        private Transaction() { }
        private static readonly Dictionary<int, Transaction> _map = new Dictionary<int, Transaction>();
        private readonly Stack<IDependent> _stack = new Stack<IDependent>();
    }
}