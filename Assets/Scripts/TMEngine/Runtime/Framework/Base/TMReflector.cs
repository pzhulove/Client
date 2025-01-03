

namespace Tenmove.Runtime
{
    using System;
    using System.Reflection;

    public class Reflector
    {
        public interface IField : IMember
        {
            T GetValue<T>();
            void SetValue<T>(T value);
        }

        public interface IProperty : IMember
        {
            T GetValue<T>();
            void SetValue<T>(T value);
        }

        public interface IMethod : IMember
        {
            R InvokeR<R>();
            R InvokeR<R, T0>(T0 param0);
            R InvokeR<R, T0, T1>(T0 param0, T1 param1);
            R InvokeR<R, T0, T1, T2>(T0 param0, T1 param1, T2 param2);
            R InvokeR<R, T0, T1, T2, T3>(T0 param0, T1 param1, T2 param2, T3 param3);
            R InvokeR<R, T0, T1, T2, T3, T4>(T0 param0, T1 param1, T2 param2, T3 param3, T4 param4);
            R InvokeR<R, T0, T1, T2, T3, T4, T5>(T0 param0, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5);
            R InvokeR<R>(params object[] args);


            void InvokeV();
            void InvokeV<T0>(T0 param0);
            void InvokeV<T0, T1>(T0 param0, T1 param1);
            void InvokeV<T0, T1, T2>(T0 param0, T1 param1, T2 param2);
            void InvokeV<T0, T1, T2, T3>(T0 param0, T1 param1, T2 param2, T3 param3);
            void InvokeV<T0, T1, T2, T3, T4>(T0 param0, T1 param1, T2 param2, T3 param3, T4 param4);
            void InvokeV<T0, T1, T2, T3, T4, T5>(T0 param0, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5);
            void InvokeV(params object[] args);
        }

        public interface IMemberSelector<out I> where I : IMember
        {
            I this[string key] { get; }
        }

        public interface IMember
        {
        }

        private interface IBase
        {
            bool Init(Reflector reflector, string name);
        }

        private struct Field : IField, IBase
        {
            private Reflector m_Reflector;
            private FieldInfo m_FieldInfo;

            public bool Init(Reflector reflector, string name)
            {
                if (null != reflector)
                {
                    m_Reflector = reflector;

                    BindingFlags flags = BindingFlags.Public |
                        BindingFlags.NonPublic |
                        BindingFlags.Instance |
                        BindingFlags.Static |
                        BindingFlags.SetField |
                        BindingFlags.GetField;
                    m_FieldInfo = m_Reflector.m_TargetType.GetField(name, flags);
                    return true;
                }

                return false;
            }

            public T GetValue<T>()
            {
                if (null != m_FieldInfo)
                    return (T)m_FieldInfo.GetValue(m_Reflector.m_TargetInstance);
                else
                {
                    Debugger.LogWarning("Invalid filed!");
                    return default(T);
                }
            }

            public void SetValue<T>(T value)
            {
                if (null != m_FieldInfo)
                    m_FieldInfo.SetValue(m_Reflector.m_TargetInstance, value);
                else
                    Debugger.LogWarning("Invalid filed!");
            }
        }

        private struct Property : IProperty, IBase
        {
            private Reflector m_Reflector;
            private PropertyInfo m_PropertyInfo;

            public bool Init(Reflector reflector, string name)
            {
                if (null != reflector)
                {
                    m_Reflector = reflector;

                    BindingFlags flags = BindingFlags.Public |
                        BindingFlags.NonPublic |
                        BindingFlags.Instance |
                        BindingFlags.Static |
                        BindingFlags.SetProperty |
                        BindingFlags.GetProperty;
                    m_PropertyInfo = m_Reflector.m_TargetType.GetProperty(name, flags);
                    return true;
                }

                return false;
            }

            public T GetValue<T>()
            {
                if (null != m_PropertyInfo)
                    return (T)m_PropertyInfo.GetValue(m_Reflector.m_TargetInstance,null);
                else
                {
                    Debugger.LogWarning("Invalid filed!");
                    return default(T);
                }
            }

            public void SetValue<T>(T value)
            {
                if (null != m_PropertyInfo)
                    m_PropertyInfo.SetValue(m_Reflector.m_TargetInstance, value,null);
                else
                    Debugger.LogWarning("Invalid filed!");
            }
        }

        private struct Method : IMethod, IBase
        {
            private static readonly System.Type MissingType = System.Type.Missing.GetType();

            private static readonly object[] NullParams = new object[0];
            private static readonly object[] Params1 = new object[1];
            private static readonly object[] Params2 = new object[2];
            private static readonly object[] Params3 = new object[3];
            private static readonly object[] Params4 = new object[4];
            private static readonly object[] Params5 = new object[5];
            private static readonly object[] Params6 = new object[6];

            private static readonly System.Type[] ParamsType0 = new System.Type[0];
            private static readonly System.Type[] ParamsType1 = new System.Type[1];
            private static readonly System.Type[] ParamsType2 = new System.Type[2];
            private static readonly System.Type[] ParamsType3 = new System.Type[3];
            private static readonly System.Type[] ParamsType4 = new System.Type[4];
            private static readonly System.Type[] ParamsType5 = new System.Type[5];
            private static readonly System.Type[] ParamsType6 = new System.Type[6];
            private static readonly System.Type[][] ParamTypeTable = new System.Type[][]
            {
                ParamsType0,
                ParamsType1,
                ParamsType2,
                ParamsType3,
                ParamsType4,
                ParamsType5,
                ParamsType6,
            };
            private Reflector m_Reflector;
            private string m_Name;

            public bool Init(Reflector reflector, string name)
            {
                if(null != reflector && !string.IsNullOrEmpty(name))
                {
                    m_Reflector = reflector;
                    m_Name = name;
                    
                    return true;
                }

                return false;
            }

            public R InvokeR<R>()
            {
                return (R)_GetMethod(NullParams).Invoke(m_Reflector.m_TargetInstance, NullParams);
            }

            public R InvokeR<R, T0>(T0 param0)
            {
                lock (Params1)
                {
                    Params1[0] = param0;
                    return (R)_GetMethod(Params1).Invoke(m_Reflector.m_TargetInstance, Params1);
                }
            }

            public R InvokeR<R, T0, T1>(T0 param0, T1 param1)
            {
                lock (Params2)
                {
                    Params2[0] = param0;
                    Params2[1] = param1;
                    return (R)_GetMethod(Params2).Invoke(m_Reflector.m_TargetInstance, Params2);
                }
            }

            public R InvokeR<R, T0, T1, T2>(T0 param0, T1 param1, T2 param2)
            {
                lock (Params3)
                {
                    Params3[0] = param0;
                    Params3[1] = param1;
                    Params3[2] = param2;
                    return (R)_GetMethod(Params3).Invoke(m_Reflector.m_TargetInstance, Params3);
                }
            }

            public R InvokeR<R, T0, T1, T2, T3>(T0 param0, T1 param1, T2 param2, T3 param3)
            {
                lock (Params4)
                {
                    Params4[0] = param0;
                    Params4[1] = param1;
                    Params4[2] = param2;
                    Params4[3] = param3;
                    return (R)_GetMethod(Params4).Invoke(m_Reflector.m_TargetInstance, Params4);
                }
            }

            public R InvokeR<R, T0, T1, T2, T3, T4>(T0 param0, T1 param1, T2 param2, T3 param3, T4 param4)
            {
                lock (Params5)
                {
                    Params5[0] = param0;
                    Params5[1] = param1;
                    Params5[2] = param2;
                    Params5[3] = param3;
                    Params5[4] = param4;
                    return (R)_GetMethod(Params5).Invoke(m_Reflector.m_TargetInstance, Params5);
                }
            }

            public R InvokeR<R, T0, T1, T2, T3, T4, T5>(T0 param0, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
            {
                lock (Params6)
                {
                    Params6[0] = param0;
                    Params6[1] = param1;
                    Params6[2] = param2;
                    Params6[3] = param3;
                    Params6[4] = param4;
                    Params6[5] = param5;
                    return (R)_GetMethod(Params6).Invoke(m_Reflector.m_TargetInstance, Params6);
                }
            }

            public R InvokeR<R>(params object[] args)
            {
                return (R)_GetMethod(args).Invoke(m_Reflector.m_TargetInstance, args);
            }

            public void InvokeV()
            {
                _GetMethod(NullParams).Invoke(m_Reflector.m_TargetInstance, NullParams);
            }

            public void InvokeV<T0>(T0 param0)
            {
                lock (Params1)
                {
                    Params1[0] = param0;
                    _GetMethod(Params1).Invoke(m_Reflector.m_TargetInstance, Params1);
                }
            }

            public void InvokeV<T0, T1>(T0 param0, T1 param1)
            {
                lock (Params2)
                {
                    Params2[0] = param0;
                    Params2[1] = param1;
                    _GetMethod(Params2).Invoke(m_Reflector.m_TargetInstance, Params2);
                }
            }

            public void InvokeV<T0, T1, T2>(T0 param0, T1 param1, T2 param2)
            {
                lock (Params3)
                {
                    Params3[0] = param0;
                    Params3[1] = param1;
                    Params3[2] = param2;
                    _GetMethod(Params3).Invoke(m_Reflector.m_TargetInstance, Params3);
                }
            }

            public void InvokeV<T0, T1, T2, T3>(T0 param0, T1 param1, T2 param2, T3 param3)
            {
                lock (Params4)
                {
                    Params4[0] = param0;
                    Params4[1] = param1;
                    Params4[2] = param2;
                    Params4[3] = param3;
                    _GetMethod(Params4).Invoke(m_Reflector.m_TargetInstance, Params4);
                }
            }

            public void InvokeV<T0, T1, T2, T3, T4>(T0 param0, T1 param1, T2 param2, T3 param3, T4 param4)
            {
                lock (Params5)
                {
                    Params5[0] = param0;
                    Params5[1] = param1;
                    Params5[2] = param2;
                    Params5[3] = param3;
                    Params5[4] = param4;
                    _GetMethod(Params5).Invoke(m_Reflector.m_TargetInstance, Params5);
                }
            }

            public void InvokeV<T0, T1, T2, T3, T4, T5>(T0 param0, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
            {
                lock (Params6)
                {
                    Params6[0] = param0;
                    Params6[1] = param1;
                    Params6[2] = param2;
                    Params6[3] = param3;
                    Params6[4] = param4;
                    Params6[5] = param5;
                    _GetMethod(Params6).Invoke(m_Reflector.m_TargetInstance, Params6);
                }
            }

            public void InvokeV(params object[] args)
            {
                _GetMethod(args).Invoke(m_Reflector.m_TargetInstance, args);
            }

            private MethodInfo _GetMethod(object[] args)
            {
                int argLength = args.Length;
                System.Type[] argType = null;
                if (argLength < 9)
                    argType = ParamTypeTable[argLength];
                else
                    argType = new System.Type[argLength];

                for (int i = 0, icnt = argType.Length; i < icnt; ++i)
                {
                    if (null != args[i])
                        argType[i] = args[i].GetType();
                    else
                        argType[i] = MissingType;
                }

                BindingFlags flags = BindingFlags.Public |
                                    BindingFlags.NonPublic |
                                    BindingFlags.Instance |
                                    BindingFlags.Static;

                MethodInfo matchMethod = m_Reflector.m_TargetType.GetMethod(m_Name,flags,null,argType,null);
                if(null == matchMethod)
                {
                    for (int i = argType.Length - 1; i>=0 ;--i)
                    {
                        if(MissingType != argType[i])
                            argType[i] = argType[i].MakeByRefType();
                        matchMethod = m_Reflector.m_TargetType.GetMethod(m_Name, flags, null, argType, null);
                        if (null != matchMethod)
                            return matchMethod;
                    }

                    matchMethod = m_Reflector.m_TargetType.GetMethod(m_Name, flags);
                }
                return matchMethod;
            }
        }


        private class MemberSelector<I, T> : IMemberSelector<I>
            where I : IMember
            where T : I, IBase
        {
            private readonly Reflector m_Reflector;

            public MemberSelector(Reflector reflector)
            {
                Debugger.Assert(null != reflector, "Parameter 'reflector' can not be null!");
                m_Reflector = reflector;
            }

            public I this[string key]
            {
                get
                {
                    T member = default(T);
                    member.Init(m_Reflector, key);
                    return member;
                }
            }
        }

        private readonly System.Type m_TargetType;
        protected readonly object m_TargetInstance;
        protected readonly IMemberSelector<IField> m_Fields;
        protected readonly IMemberSelector<IProperty> m_Properties;
        protected readonly IMemberSelector<IMethod> m_Methods;

        public Reflector(System.Type targetType, object targetInstance)
        {
            Debugger.Assert(null != targetType, "Parameter 'targetType' can not be null!");

            m_TargetType = targetType;
            m_TargetInstance = targetInstance;

            m_Fields = new MemberSelector<IField, Field>(this);
            m_Properties = new MemberSelector<IProperty, Property>(this);
            m_Methods = new MemberSelector<IMethod, Method>(this);
        }

        public IMemberSelector<IField> Fields
        {
            get
            {
                return m_Fields;
            }
        }

        public IMemberSelector<IProperty> Properties
        {
            get
            {
                return m_Properties;
            }
        }

        public IMemberSelector<IMethod> Methods
        {
            get
            {
                return m_Methods;
            }
        }

        static public Reflector Type(string targetTypeName, object targetInstance = null)
        {
            return new Reflector(Utility.Assembly.GetType(targetTypeName), targetInstance);
        }

        static public Reflector Type(System.Type targetType, object targetInstance = null)
        {
            return new Reflector(targetType, targetInstance);
        }

        static public Reflector Type(object targetInstance)
        {
            if (null != targetInstance)
                return new Reflector(targetInstance.GetType(), targetInstance);
            else
                return null;
        }

        static public Reflector Type<TYPE>()
        {
            return new Reflector(typeof(TYPE), null);
        }
    }
}