using JetBrains.Annotations;

namespace DynamicProperty
{
    interface IDependency
    {
        void Support([NotNull] IDependent dependent);
        void DoesNotSupport([NotNull] IDependent dependent);
    }
}
