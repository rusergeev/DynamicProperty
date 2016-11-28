using System;

namespace Developer.Test
{
    class BasicProperty<T> : SubscribableProperty<T>, IDynamicProperty<T>, IDependencySource
    {
        public BasicProperty(T initialValue) : base(initialValue)
        {
        }

        protected BasicProperty()
        {
        }

        protected new void Init(T initialValue)
        {
            base.Init(initialValue);
        }

        public new virtual T Value
        {
            get
            {
                RegisterTargetForEvaluation();
                return base.Value;
            }
            set
            {
                base.Value = value;
                NotifyAllTargets();
            }
        }

        public IDisposable Subscribe(Action notify)
        {
            return _dependencies.Create(notify);
        }

        public void NotifyAllTargets()
        {
            foreach (var notify in _dependencies)
            {
                notify();
            }
        }

        private void RegisterTargetForEvaluation()
        {
            var targets = ThreadStack.Instance;
            if (targets.Any())
            {
                targets.Peek().SubscribeTo(this);
            }
        }

        private readonly Subscription<Action> _dependencies = new Subscription<Action>();
    }


}
