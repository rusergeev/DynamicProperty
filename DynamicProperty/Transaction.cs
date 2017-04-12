using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DynamicProperty
{
    class Transaction : IDisposable {
        public static IDisposable Instance(DependencyNode dependency) {
            var id = Thread.CurrentThread.ManagedThreadId;
            if (!_map.ContainsKey(id))
                _map[id] = new Transaction();
            _map[id].CheckIn(dependency);
            return _map[id];
        }
        public void Dispose() {
            CheckOut();
        }
        private void CheckIn(DependencyNode dependency) {
            if (_stack.Any()){
                var last = _stack.Peek();
                last.DependsOn(dependency);
            }
            _stack.Push(dependency);
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
        private readonly Stack<DependencyNode> _stack = new Stack<DependencyNode>();
    }
}