using UnityEngine;
using System.Collections.Generic;
using Protocol;
using Network;
using System;

namespace GameClient
{
    class EntranceData
    {
        public ProtoTable.GuidanceEntranceTable entranceItem;
    }
    class DevelopGuidanceManager : DataManager<DevelopGuidanceManager>
    {
        bool m_bNeedOpen = false;
        #region delegate
        #endregion
        public void TryOpenGuidanceEntranceFrame()
        {
            if(!m_bNeedOpen)
            {
                return;
            }

            var current = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
            if(current == null)
            {
                return;
            }

            m_bNeedOpen = false;

            if(PlayerBaseData.GetInstance().Level < 15)
            {
                return;
            }
            ClientSystemManager.GetInstance().OpenFrame<DevelopGuidanceEntranceFrame>(FrameLayer.Middle);
        }

        public void SignalGuidanceEntrace()
        {
            m_bNeedOpen = true;
            TryOpenGuidanceEntranceFrame();
        }

        #region process
        public override void Initialize()
        {
        }

        public override void Clear()
        {
            m_bNeedOpen = false;
        }

        public override void OnApplicationStart()
        {

        }

        public override void OnApplicationQuit()
        {

        }
        #endregion
    }
}