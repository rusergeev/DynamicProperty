namespace Developer.Test
{
    interface _IClient
    {
        void SubscribeTo<T>(_DynProperty<T> to);
    }
}