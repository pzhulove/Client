using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;

namespace GameClient
{

    public class ArtifactJarDailyRewardFrame : ClientFrame
    {
        private List<ArtifactJarBuy> allJarData = new List<ArtifactJarBuy>();
        #region ExtraUIBind
        private ArtifactJarDailyRewardView mArtifactJarDailyRewardView = null;
		
		protected override void _bindExUI()
		{
			mArtifactJarDailyRewardView = mBind.GetCom<ArtifactJarDailyRewardView>("ArtifactJarDailyRewardView");
		}
		
		protected override void _unbindExUI()
		{
			mArtifactJarDailyRewardView = null;
		}
		#endregion
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ArtifactJar/ArtifactJarDailyReward";
        }

        protected override void _OnOpenFrame()
        {
            _BindUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            _UnBindUIEvent();
        }


        #region ui event
        void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ArtifactDailyRewardUpdate, _UpdateToggle);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ArtifactDailyRecordUpdate, _UpdateRecord);
        }

        void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ArtifactDailyRewardUpdate, _UpdateToggle);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ArtifactDailyRecordUpdate, _UpdateRecord);
        }

        void _UpdateToggle(UIEvent uiEvent)
        {
            //allJarData = ArtifactDataManager.GetInstance().getArtiFactJarBuyData();
            mArtifactJarDailyRewardView.InitView();
        }

        void _UpdateRecord(UIEvent uiEvent)
        {
            int jarId = (int)uiEvent.Param1;
            mArtifactJarDailyRewardView.UpdateRecord(jarId);
        }
        #endregion
    }
}
