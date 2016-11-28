namespace Developer.Test
{
    interface IValidDynamicProperty<T> : IDynamicProperty<T>
    {
        bool Valid { get; }
    }
}