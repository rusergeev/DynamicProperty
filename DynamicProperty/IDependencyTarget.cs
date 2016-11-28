namespace Developer.Test
{
    interface IDependencyTarget
    {
        void SubscribeTo(IDependencySource source);
    }
}