namespace Developer.Test
{
    class BasicProperty<T> : Wrapper<SubscribableProperty<T>, T>
    {
        public BasicProperty(T value) : base( new SubscribableProperty<T>(value))
        {
        }
    }
}