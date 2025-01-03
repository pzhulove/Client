using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System;
using ProtoTable;
using DG.Tweening;
using Protocol;
using Network;

namespace GameClient
{
    public enum ChangeJobType
    {
        ChangeJobMission = 0,
        SwitchJob,
    }

    class ChangeJobSelectFrame : ClientFrame
    {
        public static ChangeJobType changeType = ChangeJobType.ChangeJobMission;

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ChangeJob/ChangeJobSelect";
        }

        protected sealed override void _OnOpenFrame()
        {
            if (userData != null)
            {
                changeType = (ChangeJobType)userData;
            }
            mView.OnInit(changeType);

            MissionManager.GetInstance().onUpdateMission += OnUpdateChangeTask;

            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeJobSelectFrameOpen);
        }

        protected sealed override void _OnCloseFrame()
        {
            mView.OnUninit();
            MissionManager.GetInstance().onUpdateMission -= OnUpdateChangeTask;
        }

        public void OnUpdateChangeTask(UInt32 iMissionID)
        {
            if (!Utility.IsChangeJobTask(iMissionID))
                return;
            frameMgr.CloseFrame(this);
        }

        private ChangeJobSelectFrameView mView;

        protected sealed override void _bindExUI()
        {
            mView = mBind.GetCom<ChangeJobSelectFrameView>("View");
        }

        protected sealed override void _unbindExUI()
        {
            mView = null;
        }
    }

}