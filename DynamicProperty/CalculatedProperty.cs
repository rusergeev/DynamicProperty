using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace DynamicProperty
{
    /// <summary>
    /// calculated dynamic property
    /// </summary>
    /// <typeparam name="T"> property value type </typeparam>
    sealed class CalculatedProperty<T> : BasicProperty<T>, IDependencyTarget, IDynamicProperty<T>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="read"> Called to calculate the value of the property. </param>
        /// <param name="write"> Called whenever the <see cref="IDynamicProperty{T}.Value"/> property setter of this is invoked. </param>
        public CalculatedProperty(Func<T> read, Action<T> write)
        {
            _read = read;
            _write = write;

            base.Init(EvaluatingRead());
        }
        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        public new T Value
        {
            get
            {
                T val = default(T);
                var done = false;
                while (!done)
                {
                    try
                    {
                        _rwl.AcquireReaderLock(TimeSpan.FromSeconds(1));
                        try
                        {
                            val = base.Value;
                            done = true;
                        }
                        finally
                        {
                            _rwl.ReleaseReaderLock();
                        }
                    }
                    catch (ApplicationException)
                    {
                        done = false;
                    }
                }
                return val;
            }
            set { _write(value); }
        }
        /// <summary>
        /// to subscribe to dependency source
        /// </summary>
        /// <param name="source"> a dependency source - <see cref="BasicProperty{T}"/></param>
        public void SubscribeTo(IDependencySource source)
        {
            if(!_dependency.ContainsKey(source))
                _dependency[source] = source.Subscribe(Invalidate);
        }

        private void ClearDependency()
        {
            foreach (var subscription in _dependency.Values)
            {
                subscription.Dispose();
            }
            _dependency.Clear();
        }

        private void RestoreValidReadCache()
        {
                ClearDependency();
                base.Value = EvaluatingRead();
        }

        private T EvaluatingRead()
        {
            T value = default(T);
            var done = false;
            while (!done)
            {
                try
                {
                    _rwl.AcquireWriterLock(TimeSpan.FromSeconds(1));
                    try
                    {
                        var targets = ThreadStack.Instance;
                        targets.Push(this);
                        value = _read();
                        var check = targets.Pop();
                        Debug.Assert(check == this, "Thread stack is broken.");
                        done = true;
                    }
                    finally
                    {
                        _rwl.ReleaseWriterLock();
                    }
                }
                catch (ApplicationException)
                {
                    done = false;
                }
            }

            return value;

        }

        private void Invalidate()
        {
            var source = this as IDependencySource;
            source.NotifyAllTargets();
            RestoreValidReadCache();
        }

        private readonly IDictionary<IDependencySource, IDisposable> _dependency = new ConcurrentDictionary<IDependencySource, IDisposable>();
        private readonly Func<T> _read;
        private readonly Action<T> _write;
        private readonly ReaderWriterLock _rwl = new ReaderWriterLock();
    }
}