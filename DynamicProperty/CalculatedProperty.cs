using System;

namespace Developer.Test
{
    class CalculatedProperty<T> : Wrapper<T>
    {
        public CalculatedProperty(Func<T> read, Action<T> write) : base(new SubscribableProperty<T>(read()))
        {
            _read = read;
            _write = write;
        }

        public override T Value
        {
            get
            {
                if (!_valid)
                {
                    _value.Value = _read();
                    _valid = true;
                }
                return _value.Value;
            }
            set { _write(value); }
        }

        public override bool Valid
        {
            get { return _valid; }
        }

        private readonly Func<T> _read;
        private readonly Action<T> _write;
        private bool _valid = true;
    }
}