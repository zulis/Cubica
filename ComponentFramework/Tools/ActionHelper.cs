namespace ComponentFramework.Tools
{
    public static class ActionHelper
    {
        public static void NullAction() { }
        public static void NullAction<T>(T t) { }
        public static void NullAction<T, U>(T t, U u) { }
        public static void NullAction<T, U, V>(T t, U u, V v) { }
        public static void NullAction<T, U, V, W>(T t, U u, V v, W w) { }

        public static TResult NullFunc<TResult>() { return default(TResult); }
        public static TResult NullFunc<T, TResult>(T t) { return default(TResult); }
        public static TResult NullFunc<T, U, TResult>(T t, U u) { return default(TResult); }
        public static TResult NullFunc<T, U, V, TResult>(T t, U u, V v) { return default(TResult); }
        public static TResult NullFunc<T, U, V, W, TResult>(T t, U u, V v, W w) { return default(TResult); }
    }
}