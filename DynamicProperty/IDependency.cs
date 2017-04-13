using JetBrains.Annotations;

namespace DynamicProperties {
    /// <summary>
    /// dependency source - <see cref="BasicValue{T}"/>
    /// </summary>
    interface IDependency {
        void Support([NotNull] IDependent dependent);
        void DoesNotSupport([NotNull] IDependent dependent);
    }
}
