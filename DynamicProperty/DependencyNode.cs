using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace DynamicProperty
{
    interface IDependent
    {
        void DependsOn([NotNull] IDependency dependency);
        void DoesNotDependOn([NotNull] IDependency dependency);
        void Recalculate();
    }

    interface IDependency
    {
        void Support([NotNull] IDependent dependent);
        void DoesNotSupport([NotNull] IDependent dependent);
    }
}
