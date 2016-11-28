using System;
using System.Linq;

namespace Developer.Test
{
    class DependencySourceProperty<T> : IValidDynamicProperty<T>
    {
        public DependencySourceProperty(T initialValue)
        {
            _value = new BasicProperty<T>(initialValue);
        }

        protected DependencySourceProperty(Func<T> read, Action<T> write)
        {
            _value = new CalculatedProperty<T>(read, write);
        }

        public T Value
        {
            get
            {
                var targets = ThreadStack.Instance.Current;
                if (targets.Any())
                {
                    targets.Peek().SubscribeTo(this);
                }
                return _value.Value;
            }
            set { _value.Value = value; }
        }

        public bool Valid { get; protected set; }

        public IDisposable Subscribe(Action<T> callback)
        {
            return _value.Subscribe(callback);
        }

        private readonly IDynamicProperty<T> _value;
    }
}
