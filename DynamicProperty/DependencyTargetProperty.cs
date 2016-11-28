using System;
using System.Diagnostics;

namespace Developer.Test
{
    class DependencyTargetProperty<T> : ResponcibilityChain<T>, IDependencyTarget
    {
        public DependencyTargetProperty(Func<T> read, Action<T> write)
            : base(new DependencySourceProperty<T>(read, write))
        {
        }

        public override T Value
        {
            get
            {
                var targets = ThreadStack.Instance.Current;
                targets.Push(this);
                var value = _value.Value;
                var check = targets.Pop();
                Debug.Assert(check == this, "Thread stack is broken.");

                return value;
            }
            set { _value.Value = value; }
        }

        public void SubscribeTo<TSource>(DependencySourceProperty<TSource> source)
        {
            source.Subscribe(value => { });
        }
    }
}