using System;

namespace Developer.Test
{
    class _DynProperty<T>:IDynamicProperty<T>
    {
        public _DynProperty(T initialValue)
        {
            _value = new BasicProperty<T>(initialValue);
        }

        public T Value
        {
            get { return _value.Value; }
            set { _value.Value = value; }
        }

        public IDisposable Subscribe(Action<T> callback)
        {
            return _value.Subscribe(callback);
        }

        private readonly IDynamicProperty<T> _value;
    }
}