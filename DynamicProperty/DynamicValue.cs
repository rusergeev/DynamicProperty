using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DynamicProperty {
	class Transaction : IDisposable {
		public static IDisposable Instance(DependencyNode dependency){
			var id = Thread.CurrentThread.ManagedThreadId;
			if (!_map.ContainsKey(id))
				_map[id] = new Transaction();
			_map[id].CheckIn(dependency);
			return _map[id];
		}
		public void Dispose(){
			CheckOut();
		}
		private void CheckIn(DependencyNode dependency)
		{
			//_access.WaitOne();
			if (_stack.Any()){
				var last = _stack.Peek();
				dependency.AddLink(last);
			}
			_stack.Push(dependency);
		}
		private void CheckOut(){
			_stack.Pop();
			if (!_stack.Any()){
				var id = Thread.CurrentThread.ManagedThreadId;
				_map.Remove(id);
			}
			//_access.ReleaseMutex();
		}
		private Transaction() { }
		private static readonly Dictionary<int, Transaction> _map = new Dictionary<int, Transaction>();
		private readonly Stack<DependencyNode> _stack = new Stack<DependencyNode>();
		//private readonly Mutex _access = new Mutex(initiallyOwned: true);
	}
	public class DynamicValue<T> : BasicValue<T> {
		public DynamicValue(Func<T> read, Action<T> write) {
			_read = read;
			_write = write;
		    using (Transaction.Instance(this))
		    { base.Set(_read()); }
        }
		protected override T Get() {
			if (!Valid)
				Evaluate();
			return base.Get();
		}
		protected override void Set(T value){
			_write(value);
		}
		private void Evaluate() {
            CutDependency();
            base.Set(_read());
		}
		private readonly Func<T> _read;
		private readonly Action<T> _write;
	}
	public class BasicValue<T> : DependencyNode, IDynamicProperty<T>
	{
		public BasicValue(T initialValue){
			_value = initialValue;
		}
		T IDynamicProperty<T>.Value{
		    get {
		        using (Transaction.Instance(this))
		        { return Get(); }
            }
		    set { Set(value); }
		}
		IDisposable IDynamicProperty<T>.Subscribe(Action<T> callback){
			return _subscriptions.Create(callback);
		}
		protected BasicValue() { }
		protected virtual T Get(){
		    return _value;
		}
		protected virtual void Set(T value){
            Invalidate();
			_value = value;
		    Valid = true;
			Notify(value);
		}
		private void Notify(T value){
			foreach (var callback in _subscriptions.AsParallel())
				callback(value);
		}
		private T _value;
		private readonly Subscription<Action<T>> _subscriptions = new Subscription<Action<T>>();
	}
}
