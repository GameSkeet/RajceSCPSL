using System;
using System.Collections.Generic;

namespace UnityEngine.Pool
{
    public interface IObjectPool<T> where T : class
    {
        int CountInactive { get; }
        T Get();
        PooledObject<T> Get(out T v);
        void Release(T elem);
        void Clear();
    }

    public struct PooledObject<T> : IDisposable where T : class
    {
        private readonly T m_ToReturn;
        private readonly IObjectPool<T> m_Pool;

        internal PooledObject(T value, IObjectPool<T> pool)
        {
            m_ToReturn = value;
            m_Pool = pool;
        }

        void IDisposable.Dispose()
        {
            m_Pool.Release(m_ToReturn);
        }
    }

    public class ObjectPool<T> : IDisposable, IObjectPool<T> where T : class
    {
        private readonly int m_MaxSize;
        private readonly Func<T> m_CreateFunc;
        private readonly Action<T> m_ActionOnGet;
        private readonly Action<T> m_ActionOnRelease;
        private readonly Action<T> m_ActionOnDestroy;

        internal bool m_CollectionCheck;
        internal readonly Stack<T> m_Stack;

        public int CountAll { get; private set; }
        public int CountInactive
        {
            get => m_Stack.Count;
        }
        public int CountActive
        {
            get => CountAll - CountInactive;
        }

        public ObjectPool(Func<T> createFunc, Action<T> actionOnGet = null, Action<T> actionOnRelease = null, Action<T> actionOnDestroy = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
        {
            if (createFunc == null)
                throw new ArgumentNullException("createFunc");
            if (maxSize <= 0)
                throw new ArgumentException("Max Size must be greater than 0", "maxSize");

            m_Stack = new Stack<T>(defaultCapacity);
            m_CreateFunc = createFunc;
            m_MaxSize = maxSize;
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
            m_ActionOnDestroy = actionOnDestroy;
            m_CollectionCheck = collectionCheck;
        }

        public T Get()
        {
            T t;
            if (m_Stack.Count == 0)
            {
                t = m_CreateFunc.Invoke();
                CountAll++;
            } else t = m_Stack.Pop();

            if (m_ActionOnGet != null)
                m_ActionOnGet(t);

            return t;
        }
        public PooledObject<T> Get(out T v) => new PooledObject<T>(v = Get(), this);

        public void Release(T elem)
        {
            if (m_CollectionCheck && m_Stack.Count > 0)
                if (m_Stack.Contains(elem))
                    throw new InvalidOperationException("Trying to release an object that has already been release to the pool");

            if (m_ActionOnRelease != null)
                m_ActionOnRelease.Invoke(elem);
            if (CountInactive < m_MaxSize)
                m_Stack.Push(elem);
            else if (m_ActionOnDestroy != null)
                m_ActionOnDestroy.Invoke(elem);
        }
        public void Clear()
        {
            if (m_ActionOnDestroy != null)
                foreach (T t in m_Stack)
                    m_ActionOnDestroy.Invoke(t);

            m_Stack.Clear();
            CountAll = 0;
        }

        public void Dispose() => Clear();
    }

    public class CollectionPool<TColl, TItem> where TColl : class, ICollection<TItem>, new()
    {
        internal static readonly ObjectPool<TColl> s_Pool = new ObjectPool<TColl>(() => Activator.CreateInstance<TColl>(), null, delegate (TColl l)
        {
            l.Clear();
        }, null, true, 10, 10000);

        public static TColl Get() => CollectionPool<TColl, TItem>.s_Pool.Get();
        public static PooledObject<TColl> Get(out TColl value) => CollectionPool<TColl, TItem>.s_Pool.Get(out value);
        public static void Release(TColl toRelease) => CollectionPool<TColl, TItem>.s_Pool.Release(toRelease);
    }

    public class ListPool<T> : CollectionPool<List<T>, T>
    {
    }
}

namespace RajceUI
{
    internal static class Extensions
    {
        public static bool TryDequeue<T>(this Queue<T> queue, out T v) where T : class
        {
            v = null;

            if (queue.Count == 0)
                return false;

            v = queue.Dequeue();
            return v != null;
        }

        public static UnityEngine.Object[] FindObjectsOfType(Type type, bool includeInactive)
        {
            UnityEngine.Object[] objs = UnityEngine.Object.FindObjectsOfType(type);
            
            if (includeInactive)
                return objs;

            List<UnityEngine.Object> res = new List<UnityEngine.Object>();

            foreach (UnityEngine.GameObject obj in objs as UnityEngine.GameObject[])
                if (obj.activeSelf)
                    res.Add(obj);

            return res.ToArray();
        }
    }
}