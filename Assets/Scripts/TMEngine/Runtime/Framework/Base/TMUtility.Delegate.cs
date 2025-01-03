

namespace Tenmove.Runtime
{
    public delegate void Function();
    public delegate void Function<P>(P param);
    public delegate void Function<P0, P1>(P0 param0,P1 param1);
    public delegate void Function<P0, P1, P2>(P0 param0, P1 param1, P2 param2);
    public delegate void Function<P0, P1, P2, P3>(P0 param0, P1 param1, P2 param2, P3 param3);
    public delegate R FuncReturn<R>();
    public delegate R FuncReturn<R, P>(P param);
    public delegate R FuncReturn<R, P0, P1>(P0 param0, P1 param1);
    public delegate R FuncReturn<R, P0, P1, P2>(P0 param0, P1 param1, P2 param2);
    public delegate R FuncReturn<R, P0, P1, P2, P3>(P0 param0,P1 param1, P2 param2, P3 param3);

    public static partial class Utility
    {
        public static class Delegate
        {
        }
    }
}