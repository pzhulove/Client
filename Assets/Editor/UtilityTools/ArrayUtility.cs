using System.Collections.Generic;

namespace HeroGo
{
    public class ArrayUtility
    {
        public delegate T ArrayNewHandle<T>(int idx) where T : new();
        public delegate void ArrayDeleteHandle<T>(T item) where T : new();

        public static void ArrayFiled<T>(int len, ref T[] array, ArrayNewHandle<T> handle = null, ArrayDeleteHandle<T> deleteHandle = null) where T : new()
        {
            List<T> list = null;

            if (len < array.Length)
            {
                list = new List<T>();
                for (int i = 0; i < len; i++)
                {
                    list.Add(array[i]);
                }

                if (deleteHandle != null)
                {
                    for (int i = len; i < array.Length; i++)
                    {
                        deleteHandle(array[i]);
                    }
                }

                array = list.ToArray();
            }
            else if (len > array.Length)
            {
                list = new List<T>(array);
                for (int i = array.Length; i < len; i++)
                {
                    if (handle != null)
                    {
                        list.Add(handle(i));
                    }
                    else
                    {
                        list.Add(new T());
                    }
                }
                array = list.ToArray();
            }
        }

        public static void ArrayRemove<T>(ref T[] array, int idx, ArrayDeleteHandle<T> deleteHandle = null) where T : new()
        {
            if (idx >= array.Length)
            {
                // out of range
                return;
            }

            if (deleteHandle != null)
            {
                deleteHandle(array[idx]);
            }

            List<T> list = new List<T>(array);
            list.Remove(array[idx]);
            array = list.ToArray();
        }

        public delegate bool ArrayRemoveDeleteJudge<T>(T item) where T : new();
        public static void ArrayRemoveBy<T>(ref T[] array, System.Predicate<T> match = null, ArrayDeleteHandle<T> deleteHandle = null) where T : new()
        {
            List<T> list = new List<T>(array);
            list.RemoveAll(match);
            array = list.ToArray();
        }
    }
}