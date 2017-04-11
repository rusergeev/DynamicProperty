using System;

namespace DynamicProperty
{
    /// <summary>
    /// ependency source - <see cref="BasicProperty{T}"/>
    /// </summary>
    interface IDependencySource
    {
        /// <summary>
        ///  Subscribes a notify callback to this basic dynamic property
        /// </summary>
        /// <param name="notify"> property changed callback </param>
        /// <returns></returns>
        IDisposable Subscribe(Action notify);
        /// <summary>
        /// Notify all consumers
        /// </summary>
        void NotifyAllTargets();
    }
}