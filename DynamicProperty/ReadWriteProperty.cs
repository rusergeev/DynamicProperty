using System;

namespace Developer.Test
{
    class ReadWriteProperty<T> : BasicProperty<T>,IDynamicProperty<T>
    {
        protected ReadWriteProperty(Func<T> read, Action<T> write) : base(read())
        {
            _read = read;
            _write = write;
        }

        public new virtual T Value
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

        protected bool Valid { get; set; } = true;
        private readonly Func<T> _read;
        private readonly Action<T> _write;
    }
}