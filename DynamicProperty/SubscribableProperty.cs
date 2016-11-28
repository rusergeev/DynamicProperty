using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Developer.Test
{
    class SubscribableProperty<T> : IValidDynamicProperty<T>
    {
        public SubscribableProperty(T initialValue)
        {
            _value = initialValue;
        }

        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                foreach (var callback in _callbacks.Values)
                {
                    callback(value);
                }
            }
        }

        public virtual bool Valid
        {
            get { return true; }
        }

        public IDisposable Subscribe(Action<T> callback)
        {
            var subscription = new Subscription(Unsubscribe);
            _callbacks[subscription] = callback;
            return subscription;
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
