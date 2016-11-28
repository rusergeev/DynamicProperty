using System;
using System.Linq;
using System.Threading.Tasks;

namespace Developer.Test
{
    /// <summary>
    ///  Creates an <see cref="IDynamicProperty{T}"/> instance
    /// </summary>
    /// <typeparam name="T"> property value type </typeparam>
    class BasicProperty<T> : SubscribableProperty<T>, IDynamicProperty<T>, IDependencySource
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="initialValue"> initial value </param>
        public BasicProperty(T initialValue) : base(initialValue)
        {
        }
        /// <summary>
        /// constructor to support late initialization from child class
        /// </summary>
        protected BasicProperty()
        {
        }
        /// <summary>
        /// late initialization function
        /// </summary>
        /// <param name="initialValue"> initial value </param>
        protected new void Init(T initialValue)
        {
            base.Init(initialValue);
        }
        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        public new T Value
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
        /// <summary>
        ///  Subscribes a notify callback to this basic dynamic property
        /// </summary>
        /// <param name="notify"> property changed callback </param>
        /// <returns></returns>
        public IDisposable Subscribe(Action notify)
        {
            return _dependencies.Create(notify);
        }
        /// <summary>
        /// Notify all consumers
        /// </summary>
        public void NotifyAllTargets()
        {
            foreach (var notify in _dependencies.AsParallel())
            {
                Task.Run(notify).Wait();
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
