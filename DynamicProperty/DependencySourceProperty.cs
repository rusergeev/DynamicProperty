using System;
using System.Collections.Generic;
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
            get { return _value.Value; }
            set { _value.Value = value; }
        }
    }

    class DependencyTargetProperty<T> : Wrapper<T>
    {
        public DependencyTargetProperty(Func<T> read, Action<T> write) : base(new DependencySourceProperty<T>(read, write))
        {
        }
    }
}
