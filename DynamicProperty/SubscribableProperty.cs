using System;

namespace Developer.Test
{
    class SubscribableProperty<T> : IDynamicProperty<T>
    {
        public SubscribableProperty(T initialValue)
        {
            _value = initialValue;
        }

        protected SubscribableProperty()
        {
        }

        protected void Init(T initialValue)
        {
            _value = initialValue;
        }

        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                Notify(value);
            }
        }

        public IDisposable Subscribe(Action<T> callback)
        {
            return subscriptions.Create(callback);
        }

        public void Notify(T value)
        {
            foreach (var callback in subscriptions)
            {
                callback(value);
            }
        }

        private T _value;
        private readonly Subscription<Action<T>> subscriptions = new Subscription<Action<T>>();
    }
}
