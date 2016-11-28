using System;
using System.Linq;
using System.Threading.Tasks;

namespace Developer.Test
{
    /// <summary>
    /// root base class for all properties
    /// responsibility: notify subscribers on property update
    /// owns a property value
    /// </summary>
    /// <typeparam name="T"> property value type </typeparam>
    class SubscribableProperty<T> : IDynamicProperty<T>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="initialValue"> initial value </param>
        public SubscribableProperty(T initialValue)
        {
            _value = initialValue;
        }
        /// <summary>
        /// constructor to support late initialization from child class
        /// </summary>
        protected SubscribableProperty()
        {
        }

        /// <summary>
        /// late initialization function
        /// </summary>
        /// <param name="initialValue"> initial value </param>
        protected void Init(T initialValue)
        {
            _value = initialValue;
        }

        /// <summary>
        /// value accessor, notifies external subscribers on set
        /// </summary>
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                Notify(value);
            }
        }
        /// <summary>
        /// subscribes a callback to this dynamic property
        /// </summary>
        /// <param name="callback"> Method to be called whenever <see cref="Value"/> is modified </param>
        /// <returns> an object which can be disposed to cancel the subscription </returns>
        public IDisposable Subscribe(Action<T> callback)
        {
            return subscriptions.Create(callback);
        }

        private void Notify(T value)
        {
            foreach (var callback in subscriptions.AsParallel())
            {
                Task.Run(() => callback(value)).Wait();
            }
        }

        private T _value;
        private readonly Subscription<Action<T>> subscriptions = new Subscription<Action<T>>();
    }
}
