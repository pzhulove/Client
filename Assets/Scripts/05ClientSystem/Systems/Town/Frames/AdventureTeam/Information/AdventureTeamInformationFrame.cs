using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AdventureTeamMainFrameData
    {
        public AdventureTeamMainTabType type;
    }

    public class AdventureTeamInformationFrame : ClientFrame
    {
        private AdventureTeamMainFrameData frameData = null;

        /// <summary>
        /// 通过界面链接 打开界面
        /// </summary>
        /// <param name="strParam"> </param>
        public static void OpenLinkFrame(string strParam)
        {
            try
            {
                var tokens = strParam.Split('|');
                if(tokens.Length == 1)
                {
                    int mainTabType = int.Parse(tokens[0]);
                    OpenTabFrame((AdventureTeamMainTabType)mainTabType);
                }
                else
                {
                    OpenTabFrame(AdventureTeamMainTabType.BaseInformation);
                }
            }
            catch (System.Exception e)
            {
                Logger.LogErrorFormat("[AdventureTeamInformationFrame] - OpenLinkFrame failed : "+e.ToString());       
            }
        }
        
        public static void OpenTabFrame(AdventureTeamMainTabType tabType)
        {
            if (AdventureTeamDataManager.GetInstance().BFuncOpened == false)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("adventure_team_unlock"), ProtoTable.CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
                return;
            }

            var frameData = new AdventureTeamMainFrameData() { type = tabType };

            if (ClientSystemManager.GetInstance().IsFrameOpen<AdventureTeamInformationFrame>())
            {
                var currFrame = ClientSystemManager.GetInstance().GetFrame(typeof(AdventureTeamInformationFrame)) as AdventureTeamInformationFrame;
                if (currFrame != null)
                {
                    currFrame._TrySelectTab(tabType);
                }
                return;
               // ClientSystemManager.GetInstance().CloseFrame<AdventureTeamInformationFrame>();
            }
            ClientSystemManager.GetInstance().OpenFrame<AdventureTeamInformationFrame>(FrameLayer.Middle, frameData);
        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/AdventureTeam/AdventureTeamInformationFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (userData != null)
            {
                frameData = (AdventureTeamMainFrameData)userData;
            }

            if (frameData == null)
            {
                frameData = new AdventureTeamMainFrameData() { type = AdventureTeamMainTabType.BaseInformation };
            }

            if (mAdventureTeamInformationView != null)
            {
                mAdventureTeamInformationView.InitView(frameData.type);
            }
        }


        protected override void _OnCloseFrame()
        {
            frameData = null;

            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.AdventureTeam);
        }

        private void _TrySelectTab(AdventureTeamMainTabType tabType)
        {
            if (mAdventureTeamInformationView != null)
            {
                mAdventureTeamInformationView.SelectViewByTab(tabType);
            }
        }

        #region ExtraUIBind
        private AdventureTeamInformationView mAdventureTeamInformationView = null;

        protected override void _bindExUI()
        {
            mAdventureTeamInformationView = mBind.GetCom<AdventureTeamInformationView>("AdventureTeamInformationView");
        }

        protected override void _unbindExUI()
        {
            mAdventureTeamInformationView = null;
        }
        #endregion
        
    }

}
