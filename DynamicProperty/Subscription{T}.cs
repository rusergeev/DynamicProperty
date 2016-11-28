using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Developer.Test
{
    class Subscription<T>: IEnumerable<T>
    {
        public IDisposable Create(T callback)
        {
            var subscription = new Subscription(Unsubscribe);
            _callbacks[subscription] = callback;
            return subscription;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _callbacks.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Unsubscribe(IDisposable subscription)
        {
            _callbacks.Remove(subscription);
        }

        private readonly IDictionary<IDisposable, T> _callbacks = new ConcurrentDictionary<IDisposable, T>();

    }
}