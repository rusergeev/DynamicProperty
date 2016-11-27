using System;

namespace Developer.Test
{
    class Wrapper<TContainer, T> : IDynamicProperty<T> where TContainer : IDynamicProperty<T>
    {
        protected Wrapper(TContainer value)
        {
            _value = value;
        }
        public virtual T Value
        {
            get { return _value.Value; }
            set { _value.Value = value; }
        }
        public IDisposable Subscribe(Action<T> callback)
        {
            return _value.Subscribe(callback);
        }

        protected readonly IDynamicProperty<T> _value;
    }
}