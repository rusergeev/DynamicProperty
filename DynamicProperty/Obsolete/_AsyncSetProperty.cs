using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Developer.Test;

namespace Developer.Test
{
    sealed class _AsyncSetProperty<T>: IDynamicProperty<T>
    {
        public _AsyncSetProperty(_IValue<T> iValue)
        {
            _iValue = iValue;
        }

        public T Value
        {
            get
            {
                T val = default(T);
                var done = false;
                while (!done)
                {
                    try
                    {
                        _lock.AcquireReaderLock(TimeSpan.FromSeconds(1));
                        try
                        {
                            val = _iValue.Value;
                            done = true;
                        }
                        finally
                        {
                            _lock.ReleaseReaderLock();
                        }
                    }
                    catch (ApplicationException)
                    {
                        Debug.Write("Read timeout");
                    }
                }
                return val;
            }
            set
            {
                var done = false;

                try
                {
                    _lock.AcquireWriterLock(TimeSpan.FromSeconds(1));
                    try
                    {
                        _iValue.Value = value;
                        NotifySubscribers(value);
                        done = true;
                    }
                    finally
                    {
                        _lock.ReleaseWriterLock();
                    }
                }
                catch (ApplicationException)
                {
                    Debug.Write("Write timeout");
                }
                Debug.Assert(done, "Set's not done");
            }
        }

        public IDisposable Subscribe(Action<T> callback)
        {
            var subscription = new Subscription(Unsubscribe);
            _lock.AcquireWriterLock(TimeSpan.FromSeconds(1));
            _callbacks[subscription] = callback;
            _lock.ReleaseWriterLock();
            return subscription;
        }

        private readonly _IValue<T> _iValue;
        private readonly Dictionary<Subscription, Action<T>> _callbacks = new Dictionary<Subscription, Action<T>>();
        private readonly ReaderWriterLock _lock = new ReaderWriterLock();

        private void NotifySubscribers(T value)
        {
            _lock.AcquireReaderLock(TimeSpan.FromSeconds(1));
            foreach (var callback in _callbacks.Values)
            {
                callback(value);
            }
            _lock.ReleaseLock();
        }

        private void Unsubscribe(Subscription subscription)
        {
            _lock.AcquireWriterLock(TimeSpan.FromSeconds(1));
            _callbacks.Remove(subscription);
            _lock.ReleaseWriterLock();
        }

        private class Subscription : IDisposable
        {
            private readonly Action<Subscription> _unsubscribe;
            public Subscription(Action<Subscription> unsubscribe)
            {
                _unsubscribe = unsubscribe;
            }
            public void Dispose()
            {
                _unsubscribe(this);
            }
        }

    }
}
