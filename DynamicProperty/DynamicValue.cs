using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicProperty {
    class DynamicValue<T> : BasicValue<T>, IDependent
    {
		public DynamicValue(Func<T> read, Action<T> write) {
			_read = read;
			_write = write;
		    Evaluate();
		}
        protected override void Set(T value) {
			_write(value);
		}
        void IDependent.DependsOn(IDependency dependency) {
            if (dependency == this)
                throw new InvalidOperationException("Don't depend on itself!!!");
            _dependencies.Add(dependency);
            dependency.Support(this);
        }
        void IDependent.DoesNotDependOn(IDependency dependency) {
            if (dependency == this)
                throw new InvalidOperationException("Don't depend on itself!!!");
            _dependencies.Remove(dependency);
            dependency.DoesNotSupport(this);
        }
        void IDependent.Recalculate() {
            ClearDepedencies();
            Evaluate();
        }
        private void ClearDepedencies() {
            var dependencies = _dependencies.ToList();
            _dependencies.Clear();
            foreach (var dependency in dependencies)
                dependency.DoesNotSupport(this);
        }
        private void Evaluate() {
            var value = TransactionRead();
            base.Set(value);
        }
        private T TransactionRead() {
            using (Transaction.Instance(this))
            { return _read(); }
        }
        private readonly Func<T> _read;
        private readonly Action<T> _write;
        private readonly ICollection<IDependency> _dependencies = new HashSet<IDependency>();
    }
}
