﻿using JetBrains.Annotations;
namespace DynamicProperty {
    /// <summary>
    /// Dependency consumer -  <see cref="DynamicValue{T}"/>
    /// </summary>
    interface IDependent {
        void DependsOn([NotNull] IDependency dependency);
        void DoesNotDependOn([NotNull] IDependency dependency);
        void Recalculate();
    }
}