using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace DynamicProperties {
    class BasicValue<T> : IDynamicProperty<T>, IDependency {
        protected BasicValue() { }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="initialValue"> initial value </param>
        public BasicValue(T initialValue) {
            _value = initialValue;
        }
        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        T IDynamicProperty<T>.Value {
            get { return TransactionGet(); }
            set { Set(value); }
        }
        /// <summary>
        /// subscribes a callback to this dynamic property
        /// </summary>
        /// <param name="callback"> Method to be called whenever <see cref="Value"/> is modified </param>
        /// <returns> an object which can be disposed to cancel the subscription </returns>
        IDisposable IDynamicProperty<T>.Subscribe(Action<T> callback) {
            return _subscriptions.Create(callback);
        }
        /// <summary>
        /// Sets the value to basic property and notifies consumers
        /// </summary>
        /// <param name="value"> the value </param>
        protected virtual void Set(T value) {
            _value = value;
            Notify(value);
            RecalculateDependents();
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
        private T TransactionGet() {
            using (Transaction.Instance(this))
            { return _value; }
        }
        private void Notify(T value) {
            foreach (var callback in _subscriptions.AsParallel())
                callback(value);
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
        [ItemNotNull]
        private readonly ICollection<IDependent> _dependents = new HashSet<IDependent>();
    }
}