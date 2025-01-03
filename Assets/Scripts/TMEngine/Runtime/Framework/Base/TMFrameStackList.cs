using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public static class FrameStackList<T>
    {
        private static readonly FrameStackCache<List<T>> sm_ListFrameStack = new FrameStackCache<List<T>>(null, _ClearList);

        public static List<T> Acquire()
        {
            return sm_ListFrameStack.Acquire();
        }

        public static void Recycle(List<T> elemet)
        {
            sm_ListFrameStack.Recycle(elemet);
        }

        static private void _ClearList(List<T> list)
        {
            list.Clear();
        }
    }
}