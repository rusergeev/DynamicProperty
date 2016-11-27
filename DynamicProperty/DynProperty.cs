using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Developer.Test
{
    class DynProperty<T>
    {
        public DynProperty(T initialValue)
        {
            Value = initialValue;
        }
        public virtual T Value {
            get
            {
                Spam();
                return _value;
            }
            set
            {
                _value = value;
                Notify();
            }
        }
        public IDisposable Subscribe(Action callback)
        {
            var subscription = new Subscription(Unsubscribe);
            _callbacks[subscription] = callback;
            return subscription;
        }
        private void Spam()
        {
            ThreadStack.Instance.Current.Peek().SubscribeTo(this);
        }

        private void Notify()
        {
            foreach (var callback in _callbacks.Values)
            {
                callback();
            }
        }
        private void Unsubscribe(IDisposable subscription)
        {
            _callbacks.Remove(subscription);
        }
        private T _value;
        private readonly IDictionary<IDisposable, Action> _callbacks = new ConcurrentDictionary<IDisposable, Action>();
    }
}