namespace Developer.Test
{
    class BasicProperty<T> : ResponcibilityChain<T>
    {
        public BasicProperty(T value) : base(new SubscribableProperty<T>(value))
        {
        }

        public override bool Valid
        {
            get { return true; }
        }
    }
}