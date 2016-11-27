namespace Developer.Test
{
    interface IClient
    {
        void SubscribeTo<T>(DynProperty<T> to);
    }
}