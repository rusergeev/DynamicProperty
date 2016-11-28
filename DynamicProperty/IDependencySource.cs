using System;

namespace Developer.Test
{
    interface IDependencySource
    {
        IDisposable Subscribe(Action notify);
        void NotifyAllTargets();
    }
}