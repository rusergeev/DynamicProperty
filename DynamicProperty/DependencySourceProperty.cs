using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Developer.Test
{
    class DependencySourceProperty<T> : Wrapper<T>
    {
        public DependencySourceProperty(T initialValue) :  base(new BasicProperty<T>(initialValue) )
        {
        }
        public DependencySourceProperty(Func<T> read, Action<T> write) : base(new CalculatedProperty<T>(read, write))
        {
        }
        public override T Value
        {
            get
            {
                T val = _value.Value;
                var targets = ThreadStack.Instance.Current;
                if (targets.Any())
                {
                    targets.Peek().SubscribeTo(this);
                }
                return val;
            }
            set { _value.Value = value; }
        }
    }

    class DependencyTargetProperty<T> : Wrapper<T>, IDependencyTarget
    {
        public DependencyTargetProperty(Func<T> read, Action<T> write) : base(new DependencySourceProperty<T>(read, write))
        {
        }
        public override T Value
        {
            get
            {
                    var targets = ThreadStack.Instance.Current;
                    targets.Push(this);
                    var value = _value.Value;
                    var check = targets.Pop();
                    Debug.Assert(check == this, "Thread stack is broken.");

                return value;
            }
            set { _value.Value = value; }
        }

        public void SubscribeTo<TSource>(DependencySourceProperty<TSource> source)
        {
            source.Subscribe(value => { });
        }

    }
}
