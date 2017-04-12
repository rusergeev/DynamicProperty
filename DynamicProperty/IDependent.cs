using JetBrains.Annotations;

namespace DynamicProperty
{
    interface IDependent
    {
        void DependsOn([NotNull] IDependency dependency);
        void DoesNotDependOn([NotNull] IDependency dependency);
        void Recalculate();
    }
}