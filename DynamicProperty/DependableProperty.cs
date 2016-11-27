using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Developer.Test
{
    class DependableProperty<TContainer, T> : Wrapper<TContainer, T> where TContainer : IDynamicProperty<T>
    {
        public DependableProperty(TContainer value) : base(value)
        {
        }
    }
}
