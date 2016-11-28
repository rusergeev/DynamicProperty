using System;
using System.Linq;

namespace Developer.Test
{
    class BasicProperty<T> : SubscribableProperty<T>, IDynamicProperty<T>, IDependencySource
    {
        public BasicProperty(T initialValue) : base(initialValue)
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

        public IDisposable Subscribe(Action notify)
        {
            return dependencies.Create(notify);
        }

        public void NotifyAllTargets()
        {
            foreach (var notify in dependencies.All())
            {
                notify();
            }
        }

        private void RegisterDependency()
        {
            var targets = ThreadStack.Instance.Current;
            if (targets.Any())
            {
                targets.Peek().SubscribeTo(this);
            }
        }

        private readonly Subscription<Action> dependencies = new Subscription<Action>();
    }


}
