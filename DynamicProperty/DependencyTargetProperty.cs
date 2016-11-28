using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Developer.Test
{
    class DependencyTargetProperty<T> : DependencySourceProperty<T>, IDependencyTarget, IValidDynamicProperty<T>
    {
        public DependencyTargetProperty(Func<T> read, Action<T> write) : base(read, write)
        {
        }

        public new T Value
        {
            get
            {
                if (Valid)
                {
                    return base.Value;
                }

                ClearDependency();

                var targets = ThreadStack.Instance.Current;
                targets.Push(this);
                var value = base.Value;
                var check = targets.Pop();
                Debug.Assert(check == this, "Thread stack is broken.");

                Valid = true;

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

        public void SubscribeTo<TSource>(DependencySourceProperty<TSource> source)
        {
            _dependency[source] = source.Subscribe(value => Update());
        }

        private void Update()
        {
            Valid = false;
            var property = _value as BasicProperty<T>;
            Debug.Assert(property != null, "Cast must be right from IDependencyTarget to BasicProperty");
            property.Notify(property.Value);
        }

        private readonly IDictionary<object, IDisposable> _dependency = new ConcurrentDictionary<object, IDisposable>();
    }
}