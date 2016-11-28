using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Developer.Test
{
    class Subscription<T>
    {
        public IDisposable Create(T callback)
        {
            var subscription = new Subscription(Unsubscribe);
            _callbacks[subscription] = callback;
            return subscription;
        }

        public ICollection<T> All()
        {
            return _callbacks.Values;
        }

        private void Unsubscribe(IDisposable subscription)
        {
            _callbacks.Remove(subscription);
        }

        private readonly IDictionary<IDisposable, T> _callbacks = new ConcurrentDictionary<IDisposable, T>();
    }
}