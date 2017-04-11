using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DynamicProperty
{
    public abstract class DependencyNode
    {
        public void AddLink([NotNull] DependencyNode to)
        {
            Valid = true;
            if (to == this)
                return;
           if(!to.Valid)
                throw new InvalidOperationException("the dependent should be already valid!");
            _to.Add(to);
            to._from.Add(this);
        }

        public void Invalidate()
        {
            foreach (var from in _from)
            {
                from._to.Remove(this);
            }
            BurnForward();
        }

        private void BurnForward()
        {
            Valid = false;
            foreach (var to in _to)
            {
                to.BurnForward();
            }
            _to.Clear();
            _from.Clear();
        }

        public bool Valid { get; private set; }

        private readonly ICollection<DependencyNode> _to = new HashSet<DependencyNode>();
        private readonly ICollection<DependencyNode> _from = new HashSet<DependencyNode>();

    }
}
