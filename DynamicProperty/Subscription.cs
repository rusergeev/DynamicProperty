using System;
using JetBrains.Annotations;

namespace DynamicProperties {
    /// <summary>
    /// disposable Subscription object
    /// </summary>
    sealed class Subscription : IDisposable {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="unsubscribe"> action to call on dispose </param>
        public Subscription([NotNull] Action<Subscription> unsubscribe) {
            _unsubscribe = unsubscribe;
        }
        /// <summary>
        /// to support IDisposable
        /// </summary>
        public void Dispose() {
            _unsubscribe(this);
        }
        private readonly Action<Subscription> _unsubscribe;
    }
}