using System;

namespace Developer.Test
{
    interface _IValue<T>
    {
        T Value { get; set; }
        void Invalidate();
    }
}