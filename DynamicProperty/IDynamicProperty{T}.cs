using System;
using JetBrains.Annotations;

namespace DynamicProperties {
    /// <summary>
    /// Represents a property that can be observed or updated.
    /// </summary>
    public interface IDynamicProperty<T> {
        /// <summary>
        /// Gets or sets the value of the property.
        /// Whenever the value is updated, all observers will be notified of the new value.
        /// </summary>
        T Value { get; set; }
        /// <summary>
        /// Subscribes a callback to this dynamic property.
        /// Anytime this dynamic property value is modified, <paramref name="callback"/> should be called with the new value.
        /// Returns an object that can be disposed to end the subscription and stop notifications sent to <paramref name="callback"/>.
        /// </summary>
        /// <param name="callback">Method to be called whenever <see cref="Value"/> is modified</param>
        /// <returns>An object which can be disposed to cancel the subscription</returns>
        /// <remarks>
        /// Any number of subscriptions can be made to dynamic property.  All subscriptions should be notified when <see cref="Value"/> changes.
        /// </remarks>
        [NotNull]
        IDisposable Subscribe([NotNull] Action<T> callback);
    }
}
