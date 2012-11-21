using System;

namespace ComponentFramework.Tools
{
    public class Eventful<T>
    {
        event Action<T, T> changed = ActionHelper.NullAction;
        public event Action<T, T> Changed
        {
            add 
            { 
                changed += value;
                value(default(T), Value);
            }
            remove { changed -= value; }
        }

        public T Value { get; private set; }

        public void Set(T newValue)
        {
            var oldValue = Value;
            Value = newValue;
            changed(oldValue, newValue);
        }

        public void OnChanged(T oldValue)
        {
            changed(oldValue, Value);
        }

        public static implicit operator T(Eventful<T> eventAware)
        {
            return eventAware.Value;
        }
        public static implicit operator Eventful<T>(T value)
        {
            return new Eventful<T> { Value = value };
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}