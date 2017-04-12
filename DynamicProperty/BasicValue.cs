using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicProperty
{
    class BasicValue<T> : IDynamicProperty<T>, IDependency
    {
        protected BasicValue() { }
        public BasicValue(T initialValue) {
            _value = initialValue;
        }
        T IDynamicProperty<T>.Value {
            get { return TransactionGet(); }
            set { Set(value); }
        }
        private T TransactionGet() {
            using (Transaction.Instance(this))
            { return _value; }
        }
        IDisposable IDynamicProperty<T>.Subscribe(Action<T> callback) {
            return _subscriptions.Create(callback);
        }
        protected virtual void Set(T value) {
            _value = value;
            Notify(value);
            RecalculateDependents();
        }
        private void Notify(T value) {
            foreach (var callback in _subscriptions.AsParallel())
                callback(value);
        }
        void IDependency.Support(IDependent dependent) {
            if (dependent == this)
                throw new InvalidOperationException("Don't depend on itself!!!");
            _dependents.Add(dependent);
        }
        void IDependency.DoesNotSupport(IDependent dependent) {
            if (dependent == this)
                throw new InvalidOperationException("Don't depend on itself!!!");
            _dependents.Remove(dependent);
        }
        private void RecalculateDependents() {
            var dependents = _dependents.ToList();
            foreach (var dependent in dependents) {
                dependent.DoesNotDependOn(this);
                dependent.Recalculate();
            }
        }
        private T _value;
        private readonly Subscription<Action<T>> _subscriptions = new Subscription<Action<T>>();
        private readonly ICollection<IDependent> _dependents = new HashSet<IDependent>();
    }
}