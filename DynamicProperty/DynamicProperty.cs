namespace Developer.Test
{
    using System;

    /// <summary>
    /// Static factory methods to create <see cref="IDynamicProperty{T}"/> instances.
    /// </summary>
    public static class DynamicProperty
    {
        /// <summary>
        /// Creates an <see cref="IDynamicProperty{T}"/> instance with <paramref name="initialValue"/>
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="initialValue">The initial value of the property</param>
        /// <returns></returns>
        public static IDynamicProperty<T> Create<T>(T initialValue)
        {
            var simpleProperty = new DynProperty<T>(initialValue);
            return new CalcProperty<T>(() => simpleProperty.Value, (value) => simpleProperty.Value = value);
        }

        /// <summary>
        /// Creates a <see cref="IDynamicProperty{T}"/> instance whose Value property is determined by running a function.
        /// We call this a <c>CalculatedDynamicProperty</c>.
        /// Attempts to set the Value property only result in the write function being called--The actual Value of the property will not change
        /// unless a re-evaluation of the read function is triggered.
        /// </summary>
        /// <typeparam name="T">The data type</typeparam>
        /// <param name="read">
        /// Called to calculate the value of the property.
        /// This method will be called exactly 1 time during construction to determine the initial value
        /// of this calculated dynamic property.
        /// If, during execution, this method accesses any other <see cref="IDynamicProperty{T}"/> instances, then this calculated
        /// dynamic property will subscribe to those other instances.  When any of those instances are changed, this read function
        /// will be called again to determine the new value of this calculated dynamic property.
        /// Under no other circumstances will this method be called.  **Specifically this method should not automatically be called
        /// </param>
        /// <param name="write">
        /// Called whenever the <see cref="IDynamicProperty{T}.Value"/> property setter of this is invoked.  This write action
        /// can do anything it wants with the written value.
        /// </param>
        /// <returns></returns>
        public static IDynamicProperty<T> Create<T>(Func<T> read, Action<T> write)
        {
            return new CalcProperty<T>(read, write);
        }
    }
}
