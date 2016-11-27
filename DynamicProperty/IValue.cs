using System;

namespace Developer.Test
{
    interface IValue<T>
    {
        T Value { get; set; }
        void Invalidate();
    }
}