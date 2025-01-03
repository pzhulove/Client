using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePool
{
    public struct ListPoolNode<T> : System.IDisposable
    {
        public static ListPoolNode<T> New()
        {
            return new ListPoolNode<T>(ListPool<T>.Get());
        }

        public List<T> Ref { get; private set; }

        public ListPoolNode(List<T> r)
        {
            Ref = r;
        }

        public void Dispose()
        {
            ListPool<T>.Release(Ref);
            Ref = null;
        }
    }
    internal static class ListPool<T>
    {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, l => l.Clear());

        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }
}
