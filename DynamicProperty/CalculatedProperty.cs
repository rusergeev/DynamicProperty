using System;

namespace Developer.Test
{
    class CalculatedProperty<T> : BasicProperty<T>, IValidDynamicProperty<T>
    {
        public CalculatedProperty(Func<T> read, Action<T> write) : base(read())
        {
            _read = read;
            _write = write;
        }

        public new T Value
        {
            get
            {
                if (!Valid)
                {
                    base.Value = _read();
                    Valid = true;
                }
                return base.Value;
            }
            set { _write(value); }
        }

        public bool Valid { get; private set; } = true;

        private readonly Func<T> _read;
        private readonly Action<T> _write;
    }
}