using System;
using System.Linq;

namespace DynamicProperty
{
    class BasicValue<T> : DependencyNode, IDynamicProperty<T> {
        public BasicValue(T initialValue) {
            _value = initialValue;
        }
        T IDynamicProperty<T>.Value {
            get {
                using (Transaction.Instance(this))
                { return Get(); }
            }
            set { Set(value); }
        }
        IDisposable IDynamicProperty<T>.Subscribe(Action<T> callback) {
            return _subscriptions.Create(callback);
        }
        protected BasicValue() { }
        protected virtual T Get() {
            return _value;
        }
        protected virtual void Set(T value) {
            _value = value;
            Invalidate();
            Notify(value);
        }
        private void Notify(T value) {
            foreach (var callback in _subscriptions.AsParallel())
                callback(value);
        }
        protected override void Eval() {
            throw new InvalidOperationException("Don't evaluate the basic value!");
        }
        private T _value;
        private readonly Subscription<Action<T>> _subscriptions = new Subscription<Action<T>>();
    }
}