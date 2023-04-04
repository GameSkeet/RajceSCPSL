using System;

namespace RajceUI.Binder
{
    public static class ConstGetter
    {
        public static ConstGetter<T> Create<T>(T obj) => new ConstGetter<T>(obj);

        public static IGetter Create(object obj, Type type)
        {
            var getterType = typeof(ConstGetter<>).MakeGenericType(type);
            return Activator.CreateInstance(getterType, obj) as IGetter;
        }
    }

    public class ConstGetter<T> : IGetter<T>
    {
        private readonly T _obj;

        public ConstGetter(T obj) => _obj = obj;

        public bool IsNull => _obj == null;
        public bool IsNullable => typeof(T).IsClass;
        public bool IsConst => true;
        public Type ValueType => typeof(T);

        public T Get() => _obj;
    }
}
