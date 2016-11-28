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

        public virtual T Value
        {
            get
            {
                RegisterDependency();
                return _value.Value;
            }
            set { _value.Value = value; }
        }

        public virtual bool Valid { get; protected set; } = false;

        private void RegisterDependency()
        {
            var targets = ThreadStack.Instance.Current;
            if (targets.Any())
            {
                targets.Peek().SubscribeTo(this);
            }
        }

        public IDisposable Subscribe(Action<T> callback)
        {
            return _value.Subscribe(callback);
        }

        protected readonly IDynamicProperty<T> _value;
    }
}
