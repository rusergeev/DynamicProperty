using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Developer.Test
{
    class CalculatedProperty<T> : BasicProperty<T>, IDependencyTarget, IDynamicProperty<T>
    {
        public CalculatedProperty(Func<T> read, Action<T> write) : base(read())
        {
            _read = read;
            _write = write;
        }

        public new T Value
        {
            get
            {
                if (_valid)
                {
                    return base.Value;
                }

                ClearDependency();

                var targets = ThreadStack.Instance.Current;
                targets.Push(this);
                var value = _read();
                var check = targets.Pop();
                Debug.Assert(check == this, "Thread stack is broken.");

                _valid = true;

                return value;
            }
            set { base.Value = value; }
        }

        private void ClearDependency()
        {
            foreach (var subscription in _dependency.Values)
            {
                subscription.Dispose();
            }
            _dependency.Clear();
        }

        public void SubscribeTo(IDependencySource source)
        {
            _dependency[source] = source.Subscribe(Invalidate);
        }

        private void Invalidate()
        {
            _valid = false;
            var source = this as IDependencySource;
            Debug.Assert(source != null, "Cast must be right from this to IDependencySource");
            source.NotifyAllTargets();
        }

        private readonly IDictionary<object, IDisposable> _dependency = new ConcurrentDictionary<object, IDisposable>();
        private readonly Func<T> _read;
        private readonly Action<T> _write;
        private bool _valid = true;
    }
}