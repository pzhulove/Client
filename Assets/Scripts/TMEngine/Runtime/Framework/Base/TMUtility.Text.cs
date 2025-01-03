using System;

namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class Text
        {
            public static string GetNameWithType<T>(string name)
            {
                return GetNameWithType(typeof(T), name);
            }

            public static string GetNameWithType(Type type,string name)
            {
                string typeName = null;
                if (null != type)
                    typeName = type.FullName;
                else
                {
                    Debugger.LogWarning("Type is null!");
                    typeName = "<UnknownType>";
                }

                return string.IsNullOrEmpty(name) ? typeName : string.Format("{0}.{1}", typeName, name);
            }
        }
    }
}

