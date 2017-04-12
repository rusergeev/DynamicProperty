using System;

namespace DynamicProperty {
    public class DynamicValue<T> : BasicValue<T> {
		public DynamicValue(Func<T> read, Action<T> write) {
			_read = read;
			_write = write;
		    using (Transaction.Instance(this))
		    { base.Set(_read()); }
        }
		//protected override T Get() {
		//	if (!Valid)
		//		Evaluate();
		//	return base.Get();
		//}
		protected override void Set(T value) {
			_write(value);
		}
		private void Evaluate() {
            CutDependency();
            base.Set(_read());
		}

        protected override void Eval(){
                Evaluate();
        }
		private readonly Func<T> _read;
		private readonly Action<T> _write;
	}
}
