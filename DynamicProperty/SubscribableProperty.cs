using System;
using System.Collections.Generic;
using System.Linq;

namespace Developer.Test
{
    class SubscribableProperty<T>: IDynamicProperty<T>, IClient, IDisposable
    {
        public SubscribableProperty(Func<T> read, Action<T> write)
        {
            _read = read;
            _write = write;
            ThreadStack.Instance.Current.Push(this);
            _value = _read();
            ThreadStack.Instance.Current.Pop();
        }

        public void SubscribeTo<T1>(DynProperty<T1> to)
        {
            _dependencies[to] = to.Subscribe(Invalidate);
        }

        public void Invalidate()
        {
            _invalid = true /*_invalid & !to.Value.Equals(value)*/;
            //todo: make separate dependency
            NotifySubscribers(_value);
        }

        public T Value {
            get
            {
                if (_invalid)
                {
                    ReleaseLinks();
                    ThreadStack.Instance.Current.Push(this);
                    _value = _read();
                    ThreadStack.Instance.Current.Pop();
                    _invalid = false;
                }
                return _value;
            }
            set
            {
                //todo: do not update if valid and the same
                //if (!_invalid && _value.Equals(value)) return;
                _write(value);
                //_invalid = true;
                NotifySubscribers(value);
            }
        }

        public IDisposable Subscribe(Action<T> callback)
        {
            var subscription = new Subscription(Unsubscribe);
            _callbacks[subscription] = callback;
            return subscription;
        }

        private  T _value;
        private readonly Func<T> _read;
        private readonly Action<T> _write;
        private bool _invalid = false;
        private readonly Dictionary<object, IDisposable> _dependencies = new Dictionary<object, IDisposable>();
        private readonly Dictionary<IDisposable, Action<T>> _callbacks = new Dictionary<IDisposable, Action<T>>();

        private void NotifySubscribers(T value)
        {
            foreach (var callback in _callbacks.Values.AsParallel())
            {
                callback(value);
            }
        }

        private void Unsubscribe(IDisposable subscription)
        {
            _callbacks.Remove(subscription);
        }


        private void ReleaseLinks()
        {
            foreach (var dependency in _dependencies.Values.AsParallel())
            {
                dependency.Dispose();
            }
            _dependencies.Clear();
        }

        public void Dispose()
        {
            ReleaseLinks();
        }
    }
}
