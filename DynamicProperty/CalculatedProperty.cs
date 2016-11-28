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
            get { return _read(); }
            set { _write(value); }
        }

        private readonly Func<T> _read;
        private readonly Action<T> _write;
    }
}