using System;

namespace Developer.Test
{
    class ResponcibilityChain<T> : IValidDynamicProperty<T>
    {
        protected ResponcibilityChain(IValidDynamicProperty<T> value)
        {
            _value = value;
        }

        public virtual T Value
        {
            get { return _value.Value; }
            set { _value.Value = value; }
        }

        public virtual bool Valid
        {
            get { return _value.Valid; }
        }

        public IDisposable Subscribe(Action<T> callback)
        {
            return _value.Subscribe(callback);
        }

        protected readonly IValidDynamicProperty<T> _value;

    }
}