using System;

namespace DynamicProperty
{
    /// <summary>
    /// Dependency consumer -  <see cref="CalculatedProperty{T}"/>
    /// </summary>
    [Obsolete]
    interface IDependencyTarget
    {
        /// <summary>
        /// to subscribe to dependency source
        /// </summary>
        /// <param name="source"> a dependency source - <see cref="BasicProperty{T}"/></param>
        void SubscribeTo(IDependencySource source);
    }
}