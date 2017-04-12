using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace DynamicProperty {

    public abstract class DependencyNode {
        public void DependsOn([NotNull] DependencyNode dependency)
        {
            if (dependency == this)
                throw new InvalidOperationException("Don't depend on itself!!!");
            _dependencies.Add(dependency);
            dependency.Support(this);
        }
        private void Support([NotNull] DependencyNode dependent)
        {
            if (dependent == this)
                throw new InvalidOperationException("Don't depend on itself!!!");
            _dependents.Add(dependent);
        }
        public void CutDependency() {
            foreach (var dependency in _dependencies)
                dependency._dependents.Remove(this);
            _dependencies.Clear();
        }
        public void Invalidate() {
            foreach (var dependent in _dependents.ToList())
                dependent.BurnUp();
            _dependents.Clear();
        }
        private void BurnUp(){
            Eval();
            foreach (var dependent in _dependents)
                dependent.BurnUp();
            _dependents.Clear();
            _dependencies.Clear();
        }
        protected abstract void Eval();
        private readonly ICollection<DependencyNode> _dependents = new HashSet<DependencyNode>();
        private readonly ICollection<DependencyNode> _dependencies = new HashSet<DependencyNode>();
    }
}
