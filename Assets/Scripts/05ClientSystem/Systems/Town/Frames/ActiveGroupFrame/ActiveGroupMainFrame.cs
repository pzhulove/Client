using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class ActiveGroupMainFrameData
    {
        public int iTabID = 0;
    }
    public class ActiveGroupMainFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ActiveGroup/ActiveGroupMainFrame";
        }

        ComActiveGroupMain comActiveMain;

        public static void CommandOpen(object argv)
        {
            if(null == argv)
            {
                argv = new ActiveGroupMainFrameData();
            }
            else
            {
                argv = argv as ActiveGroupMainFrameData;
            }
            ClientSystemManager.GetInstance().OpenFrame<ActiveGroupMainFrame>(FrameLayer.Middle,argv);
        }      

        ActiveGroupMainFrameData _data = null;
        protected override void _OnOpenFrame()
        {
            _data = userData as ActiveGroupMainFrameData;
            if(null == _data)
            {
                _data = new ActiveGroupMainFrameData();
            }

            comActiveMain = frame.GetComponent<ComActiveGroupMain>();
            if(null != comActiveMain)
            {
                comActiveMain.CreateMainTabs();
                comActiveMain.SelectTab(_data.iTabID);
                comActiveMain.InitTabDropDown();
                comActiveMain.UpdateAchievementPoint();
                comActiveMain.UpdateTabProcess();
                comActiveMain.UpdateAwardStatus();
            }
        }

        protected override void _OnCloseFrame()
        {
            if(null != comActiveMain)
            {
                comActiveMain.StopInvoke();
                comActiveMain = null;
            }
            _data = null;
        }
    }
}