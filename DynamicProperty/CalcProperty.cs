using System;
using System.Collections.Generic;
using System.Linq;

namespace Developer.Test
{
    class CalcProperty<T> : IDynamicProperty<T>
    {
        public CalcProperty(Func<T> read, Action<T> write)
        {
            _read = read;
            _write = write;
            _cache = new SubsValue<T>(_read());
        }

        public T Value
        {
            get { return Read(); }
            set { Write(value); }
        }

        public IDisposable Subscribe(Action<T> callback)
        {
            return _cache.Subscribe(callback);
        }

        private T Read()
        {
            if (_invalid)
            {
                _cache.Value = _read();
                _invalid = false;
            }
            return _cache.Value;
        }

        private void Write(T value)
        {
            _invalid = true;
            _write(value);
        }
        private readonly Func<T> _read;
        private readonly Action<T> _write;
        private readonly SubsValue<T> _cache;
        private bool _invalid = false;
    }
}