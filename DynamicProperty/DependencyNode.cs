using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace DynamicProperty {
    public abstract class DependencyNode {
        public void AddLink([NotNull] DependencyNode to) {
            if (to == this)
                throw new InvalidOperationException("Don't set itself as link");
            _to.Add(to);
            to._from.Add(this);
        }
        public void CutDependency() {
            foreach (var from in _from)
                from._to.Remove(this);
        }
        public void Invalidate() {
            foreach (var to in _to.ToList())
                to.BurnUp();
            _to.Clear();
        }
        private void BurnUp(){
            Eval();
            foreach (var to in _to)
                to.BurnUp();
            _to.Clear();
            _from.Clear();
        }
        protected abstract void Eval();
        private readonly ICollection<DependencyNode> _to = new HashSet<DependencyNode>();
        private readonly ICollection<DependencyNode> _from = new HashSet<DependencyNode>();
    }
}
