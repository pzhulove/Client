using System;
using System.Collections.Generic;
///////É¾³ýlinq
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Runtime.InteropServices;
using Protocol;
using Network;

namespace GameClient
{
    public class EnterGameMessageHandleAttribute : Attribute
    {
        public uint id;
        public int order;
        public EnterGameMessageHandleAttribute(uint a_id, int a_nOrder = 0)
        {
            this.id = a_id;
            this.order = a_nOrder;
        }
    }

    public class EnterGameBinding
    {
        public uint id;
        public int order;
        public Action<MsgDATA> method;
    }

    public interface IDataManager
    {
        EEnterGameOrder GetOrder();
        //void InitializeAll(WaitNetMessageManager.NetMessages a_msgEvent);

        void InitiallizeSystem();

        void ProcessInitNetMessage(WaitNetMessageManager.NetMessages a_msgEvent);
        
        void ClearAll();

        void Update(float a_fTime);

        void BindEnterGameMsg(List<uint> a_msgEvent);

        void OnEnterSystem();
        void OnExitSystem();

        void OnApplicationStart();
        void OnApplicationQuit();

    }
}