using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DynamicProperty
{
    public abstract class DependencyNode
    {
        public void AddLink([NotNull] DependencyNode to)
        {
            if (to == this)
                throw new InvalidOperationException("Don't set itself as link");
            _to.Add(to);
            to._from.Add(this);
        }

        //todo: better name
        public void CutDependency()
        {
            foreach (var from in _from)
            {
                from._to.Remove(this);
            }
        }
        //todo: better name
        public void Invalidate()
        {
            Valid = false;
            foreach (var to in _to)
            {
                to.Invalidate();
            }
            _to.Clear();
            _from.Clear();
        }

        public bool Valid { get; set; }

        private readonly ICollection<DependencyNode> _to = new HashSet<DependencyNode>();
        private readonly ICollection<DependencyNode> _from = new HashSet<DependencyNode>();

    }
}
