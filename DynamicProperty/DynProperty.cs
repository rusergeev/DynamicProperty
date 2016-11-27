using System;

namespace Developer.Test
{
    class DynProperty<T>:IDynamicProperty<T>
    {
        public DynProperty(T initialValue)
        {
            _value = new SubsValue<T>(initialValue);
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

        private readonly SubsValue<T> _value;
    }
}