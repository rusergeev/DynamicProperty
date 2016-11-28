namespace Developer.Test
{
    interface IDependencyTarget
    {
        void SubscribeTo<T>(BasicProperty<T> source);
    }
}