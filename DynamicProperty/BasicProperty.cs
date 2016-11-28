using System;
using System.Linq;

namespace Developer.Test
{
    class BasicProperty<T> : SubscribableProperty<T>, IDynamicProperty<T>
    {
        public BasicProperty(T initialValue):base(initialValue)
        {
        }

        public new virtual T Value
        {
            get
            {
                RegisterDependency();
                return base.Value;
            }
            set { base.Value = value; }
        }

        private void RegisterDependency()
        {
            var targets = ThreadStack.Instance.Current;
            if (targets.Any())
            {
                targets.Peek().SubscribeTo(this);
            }
        }
    }
}
