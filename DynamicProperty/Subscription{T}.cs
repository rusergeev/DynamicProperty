using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Developer.Test
{
    /// <summary>
    /// Subscription Factory
    /// </summary>
    /// <typeparam name="T"> callback type </typeparam>
    sealed class Subscription<T>: IEnumerable<T>
    {
        /// <summary>
        /// created a subscription
        /// </summary>
        /// <param name="callback"> callback to call when notify </param>
        /// <returns> Disposable object </returns>
        public IDisposable Create(T callback)
        {
            var subscription = new Subscription(Unsubscribe);
            _callbacks[subscription] = callback;
            return subscription;
        }
        /// <summary>
        /// To enumerate throw callback
        /// </summary>
        /// <returns> enumerator </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _callbacks.Values.GetEnumerator();
        }
        /// <summary>
        /// supports enumeration
        /// </summary>
        /// <returns> enumerator </returns>
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