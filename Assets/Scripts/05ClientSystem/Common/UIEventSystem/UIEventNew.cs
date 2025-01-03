using UnityEngine;

namespace GameClient
{
    public struct UIEventParam
    {
        public int m_Int;
        public int m_Int2;
        public bool m_Bool;
        public Vector3 m_Vector;
        public object m_Obj;
        public object m_Obj2;
        public object m_Obj3;
        public string m_String;
    }
    public class UIEventNew
    {
        public class UIEventParamNew
        {
            //后续需要更多的参数传递 可以在后面添加 Reset函数中也要添加相应的参数
            public int m_Int;
            public int m_Int2;
            public bool m_Bool;
            public Vector3 m_Vector;
            public object m_Obj;
            public object m_Obj2;
            public object m_Obj3;
            public string m_String;

            public void Reset()
            {
                m_Int = 0;
                m_Int2 = 0;
                m_Bool = false;
                m_Obj = null;
                m_Obj2 = null;
                m_Obj3 = null;
                m_String = null;
            }

            public void FromStruct(UIEventParam param)
            {
                m_Int = param.m_Int;
                m_Int2 = param.m_Int2;
                m_Bool = param.m_Bool;
                m_Vector = param.m_Vector;
                m_Obj = param.m_Obj;
                m_Obj2 = param.m_Obj2;
                m_Obj3 = param.m_Obj3;
                m_String = param.m_String;
            }

            public void ToStruct(out UIEventParam param)
            {
                param.m_Int = m_Int;
                param.m_Int2 = m_Int2;
                param.m_Bool = m_Bool;
                param.m_Vector = m_Vector;
                param.m_Obj = m_Obj;
                param.m_Obj2 = m_Obj2;
                param.m_Obj3 = m_Obj3;
                param.m_String = m_String;
            }
        }

        public class UIEventHandleNew
        {
            public delegate void Function(UIEventParamNew param);

            public int m_EventType;
            public Function m_Fn;

            public UIEventHandleNew(int eventType, Function fn)
            {
                this.m_EventType = eventType;
                this.m_Fn = fn;
            }
            public void Remove()
            {
                m_Fn = null;
            }
        }
    }
}
