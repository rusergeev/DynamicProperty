using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace Developer.Test
{
	internal interface IDependent : IDependency
	{
		void Invalidate();
		void AddDependendency(IDependency dependant);
		void RemoveDependency(IDependency dependant);
	}
	internal interface IDependency
	{
		void AddDependendent(IDependent dependant);
		void RemoveDependent(IDependent dependant);
	}
	class Transaction : IDisposable
	{
		public static IDisposable Instance(IDependency dependency)
		{
			var id = Thread.CurrentThread.ManagedThreadId;
			if (!_map.ContainsKey(id))
				_map[id] = new Transaction();
			_map[id].CheckIn(dependency);
			return _map[id];
		}
		public void Dispose()
		{
			CheckOut();
		}
		private void CheckIn(IDependency dependency)
		{
			//_access.WaitOne();

			if (_stack.Any())
			{
				var last = _stack.Peek();
				if (last != null)
					dependency.AddDependendent(last);
			}
			var dependent = dependency as IDependent;
			_stack.Push(dependent);
#if DEBUG
			property = dependent;
#endif
		}
		private void CheckOut()
		{
#if DEBUG
			var top =
#endif
			_stack.Pop();
#if DEBUG
			// if (!top.Equals(property))
			//	throw new InvalidOperationException("Transaction Stack broken!!!");
#endif
			if (!_stack.Any())
			{
				var id = Thread.CurrentThread.ManagedThreadId;
				_map.Remove(id);
			}
			//_access.ReleaseMutex();
		}
		private Transaction() { }
		private static readonly Dictionary<int, Transaction> _map = new Dictionary<int, Transaction>();
		private Stack<IDependent> _stack = new Stack<IDependent>();
		private readonly Mutex _access = new Mutex(initiallyOwned: true);
#if DEBUG
		IDependent property;
#endif
	}
	public class DynamicValue<T> : BasicValue<T>, IDependent {
		public DynamicValue(Func<T> read, Action<T> write) {
			_read = read;
			_write = write;
 			Evaluate();
		}
		void IDependent.Invalidate() {
			_valid = false;
			InvalidateDependants();
		}
		void IDependent.AddDependendency(IDependency dependency) {
			_dependencies.Add(dependency);
		}
		void IDependent.RemoveDependency(IDependency dependency) {
			_dependencies.Remove(dependency);
		}
		protected override T Get() {
			if (!_valid)
				Evaluate();
			return base.Get();
		}
		protected override void Set(T value){
			_write(value);
		}
		private void Evaluate() {
			RemoveDependencies();
			using (Transaction.Instance(this)) {
				base.Set(_read());
				_valid = true;
			}
		}
		private void RemoveDependencies(){
			var dependencies = new List<IDependency>(_dependencies);
			foreach( var dependency in dependencies){
				dependency.RemoveDependent(this);
			}
		}
		private readonly Func<T> _read;
		private readonly Action<T> _write;
		private bool _valid;
		private HashSet<IDependency> _dependencies = new HashSet<IDependency>();
	}
	public class BasicValue<T> : IDynamicProperty<T>, IDependency
	{
		public BasicValue(T initialValue){
			_value = initialValue;
		}
		T IDynamicProperty<T>.Value{
			get => Get(); 
			set {
				Set(value);
				InvalidateDependants();
			}
		}
		IDisposable IDynamicProperty<T>.Subscribe(Action<T> callback)
		{
			return _subscriptions.Create(callback);
		}
		void IDependency.AddDependendent(IDependent dependant) {
			_dependables.Add(dependant);
			dependant.AddDependendency(this);
		}
		void IDependency.RemoveDependent(IDependent dependant) {
			_dependables.Remove(dependant);
			dependant.RemoveDependency(this);
		}
		protected BasicValue() { }
		protected virtual T Get(){
			using (Transaction.Instance(this))
			{ return _value; }
		}
		protected virtual void Set(T value){
			_value = value;
			Notify(value);
		}
		protected void InvalidateDependants(){
			var dependables =  new List<IDependent>(_dependables);
			_dependables.Clear();
			foreach (var dependable in dependables){
				dependable.Invalidate();
			}
		}
		private void Notify(T value){
			foreach (var callback in _subscriptions.AsParallel())
				callback(value);
		}
		private T _value;
		private HashSet<IDependent> _dependables = new HashSet<IDependent>();
		private readonly Subscription<Action<T>> _subscriptions = new Subscription<Action<T>>();
	}
}
