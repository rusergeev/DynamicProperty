namespace Developer.Test
{
    interface IDependencyTarget
    {
        void SubscribeTo<T>(DependencySourceProperty<T> source);
    }
}