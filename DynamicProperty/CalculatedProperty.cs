using System;

namespace Developer.Test
{
    class CalculatedProperty<T> : Wrapper<SubscribableProperty<T>, T>
    {
        public CalculatedProperty(Func<T> read, Action<T> write) : base(new SubscribableProperty<T>(read()))
        {
            _read = read;
            _write = write;
        }

        public override T Value
        {
            get { return Read(); }
            set { Write(value); }
        }

        private T Read()
        {
            if (_invalid)
            {
                _value.Value = _read();
                _invalid = false;
            }
            return _value.Value;
        }

        private void Write(T value)
        {
            _invalid = true;
            _write(value);
        }
        private readonly Func<T> _read;
        private readonly Action<T> _write;
        private bool _invalid = false;
    }
}