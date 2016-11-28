using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Developer.Test
{
    class SubscribableProperty<T> : IDynamicProperty<T>
    {
        public SubscribableProperty(T initialValue)
        {
            _value = initialValue;
        }

        public virtual T Value
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
            var subscription = new Subscription(Unsubscribe);
            _callbacks[subscription] = callback;
            return subscription;
        }

        public void Notify(T value)
        {
            foreach (var callback in _callbacks.Values)
            {
                callback(value);
            }
        }

        private void Unsubscribe(IDisposable subscription)
        {
            _callbacks.Remove(subscription);
        }

        private T _value;

        private readonly IDictionary<IDisposable, Action<T>> _callbacks =
            new ConcurrentDictionary<IDisposable, Action<T>>();
    }
}
