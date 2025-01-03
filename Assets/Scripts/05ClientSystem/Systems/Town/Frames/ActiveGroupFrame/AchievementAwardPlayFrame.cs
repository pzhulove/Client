using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    class AchievementAwardPlayFrameData
    {
        public int iId = 2;
    }

    public class AchievementAwardPlayFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ActiveGroup/AchievementAwardPlayFrame";
        }

        [UIControl("", typeof(ComAchievementAwardPlayFrameConfig))]
        ComAchievementAwardPlayFrameConfig comAchievementAwardPlayFrameConfig;
        AchievementAwardPlayFrameData _Data = null;

        public static void CommandOpen(object argv)
        {
            //ClientSystemManager.GetInstance().OpenFrame<AchievementAwardPlayFrame>(FrameLayer.Middle,argv);
        }

        protected override void _OnOpenFrame()
        {
            _Data = userData as AchievementAwardPlayFrameData;
            if(null == _Data)
            {
                _Data = new AchievementAwardPlayFrameData();
            }
            if (null != comAchievementAwardPlayFrameConfig)
            {
                comAchievementAwardPlayFrameConfig.SetAwards(_Data.iId);
            }
            _AddButton("Close", () => { frameMgr.CloseFrame(this); });
        }

        protected override void _OnCloseFrame()
        {
            if(null != comAchievementAwardPlayFrameConfig)
            {
                comAchievementAwardPlayFrameConfig.DestroyComItems();
            }
            _Data = null;
        }
    }
}