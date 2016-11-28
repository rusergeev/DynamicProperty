using System;

namespace Developer.Test
{
    sealed class Subscription : IDisposable
    {
        public Subscription(Action<Subscription> unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }

        public void Dispose()
        {
            _unsubscribe(this);
        }

        private readonly Action<Subscription> _unsubscribe;
    }
}