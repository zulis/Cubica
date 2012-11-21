namespace ComponentFramework.Tools
{
    public class Dirtyable<T>
    {
        T value;

        public T Value
        {
            get { return value; }
        }

        public bool Dirty { get; private set; }

        public void Clean()
        {
            Dirty = false;
        }

        public void Set(T newValue)
        {
            value = newValue;
            Dirty = true;
        }

        public static implicit operator T(Dirtyable<T> dirtyable)
        {
            return dirtyable.Value;
        }
        public static implicit operator Dirtyable<T>(T dirtyable)
        {
            return new Dirtyable<T> { value = dirtyable };
        }
    }
}
