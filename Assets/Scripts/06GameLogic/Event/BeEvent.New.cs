using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    /// <summary>
    /// 新版的战斗逻辑事件类
    /// </summary>
    public class BeEvent
    {
        public class BeEventParam
        {
            // 触发者的pid
            public int m_SenderId;
            //后续需要更多的参数传递 可以在后面添加 Reset函数中也要添加相应的参数
            public int m_Int;
            public int m_Int2;
            public int m_Int3;
            public int m_Int4;
            public int m_Int5;
            public bool m_Bool;
            public bool m_Bool2;
            public VPercent m_Percent;
            public VRate m_Rate;
            public VInt m_Vint;
            public VInt m_Vint2;
            public VInt3 m_Vint3;
            public Vector3 m_Vector;
            public object m_Obj;
            public object m_Obj2;
            public object m_Obj3;
            public string m_String;

            public void Reset()
            {
                m_SenderId = -1;
                m_Int = 0;
                m_Int2 = 0;
                m_Int3 = 0;
                m_Int4 = 0;
                m_Int5 = 0;
                m_Bool = false;
                m_Bool2 = false;
                m_Percent = VPercent.zero;
                m_Rate = VRate.zero;
                m_Vint = VInt.zero;
                m_Vint2 = VInt.zero;
                m_Vint3 = VInt3.zero;
                m_Vector = Vector3.zero;
                m_Obj = null;
                m_Obj2 = null;
                m_Obj3 = null;
                m_String = null;
            }
            
            public void FromStruct(EventParam param)
            {
                m_Int = param.m_Int;
                m_Int2 = param.m_Int2;
                m_Int3 = param.m_Int3;
                m_Int4 = param.m_Int4;
                m_Int5 = param.m_Int5;
                m_Bool = param.m_Bool;
                m_Bool2 = param.m_Bool2;
                m_Percent = param.m_Percent;
                m_Rate = param.m_Rate;
                m_Vint = param.m_Vint;
                m_Vint2 = param.m_Vint2;
                m_Vint3 = param.m_Vint3;
                m_Vector = param.m_Vector;
                m_Obj = param.m_Obj;
                m_Obj2 = param.m_Obj2;
                m_Obj3 = param.m_Obj3;
                m_String = param.m_String;
            }

            public void ToStruct(out EventParam param)
            {
                param.m_Int = m_Int;
                param.m_Int2 = m_Int2;
                param.m_Int3 = m_Int3;
                param.m_Int4 = m_Int4;
                param.m_Int5 = m_Int5;
                param.m_Bool = m_Bool;
                param.m_Bool2 = m_Bool2;
                param.m_Percent = m_Percent;
                param.m_Rate = m_Rate;
                param.m_Vint = m_Vint;
                param.m_Vint2 = m_Vint2;
                param.m_Vint3 = m_Vint3;
                param.m_Vector = m_Vector;
                param.m_Obj = m_Obj;
                param.m_Obj2 = m_Obj2;
                param.m_Obj3 = m_Obj3;
                param.m_String = m_String;
            }
        }

        

        public class BeEventHandleNew : IBeEventHandle
        {
            public delegate void Function(BeEventParam param);

            public int m_EventType;
            public Function m_Fn;
            public bool m_CanRemove;

            public BeEventHandleNew(int eventType, Function fn)
            {
                this.m_EventType = eventType;
                this.m_Fn = fn;
                this.m_CanRemove = false;
            }

            public void Remove()
            {
                m_CanRemove = true;
            }
        }
    }
        
    public struct EventParam
    {
        public int m_Int;
        public int m_Int2;
        public int m_Int3;
        public int m_Int4;
        public int m_Int5;
        public bool m_Bool;
        public bool m_Bool2;
        public VPercent m_Percent;
        public VRate m_Rate;
        public VInt m_Vint;
        public VInt m_Vint2;
        public VInt3 m_Vint3;
        public Vector3 m_Vector;
        public object m_Obj;
        public object m_Obj2;
        public object m_Obj3;
        public string m_String;
    }
}
