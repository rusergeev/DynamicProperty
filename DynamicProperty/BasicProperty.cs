namespace Developer.Test
{
    class BasicProperty<T> : Wrapper<T>
    {
        public BasicProperty(T value) : base( new SubscribableProperty<T>(value))
        {
        }
    }
}