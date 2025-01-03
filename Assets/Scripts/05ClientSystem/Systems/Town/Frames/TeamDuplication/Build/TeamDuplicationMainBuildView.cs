using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class TeamDuplicationMainBuildView : MonoBehaviour
    {
        [Space(15)] [HeaderAttribute("Control")] [Space(10)] [SerializeField]
        private Text titleLabel;

        [Space(15)] [HeaderAttribute("Control")] [Space(10)]
        [SerializeField] private TeamDuplicationBuildCommonControl buildCommonControl;

        [SerializeField] private TeamDuplicationHeadControl headControl;
        [SerializeField] private TeamDuplicationTeamCaptainPanelControl teamCaptainPanelControl;

        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationBuildTeamSuccessMessage,
                OnReceiveTeamDuplicationBuildTeamSuccessMessage);

            //场景加载完成
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SceneChangedLoadingFinish, OnReceiveSceneChangeLoadingFinish);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationBuildTeamSuccessMessage,
                OnReceiveTeamDuplicationBuildTeamSuccessMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SceneChangedLoadingFinish, OnReceiveSceneChangeLoadingFinish);
        }

        public void Init()
        {
            InitData();
            InitView();
        }

        private void InitData()
        {

        }

        private void InitView()
        {
            if (titleLabel != null)
                titleLabel.text = TR.Value("team_duplication_build_title");

            if(buildCommonControl != null)
                buildCommonControl.Init();

            if(headControl != null)
                headControl.Init();

            if(teamCaptainPanelControl != null)
                teamCaptainPanelControl.Init();
        }


        #region UIEvent

        //加入团本的时候,自动打开房间界面
        private void OnReceiveTeamDuplicationBuildTeamSuccessMessage(UIEvent uiEvent)
        {
            TeamDuplicationUtility.OnCloseRelationFrameByOwnerTeam();
            TeamDuplicationUtility.OnOpenTeamDuplicationTeamRoomFrame();
        }

        //场景加载完，进入到组队场景
        private void OnReceiveSceneChangeLoadingFinish(UIEvent uiEvent)
        {
            OnEnterTeamDuplicationBuildSceneFrameTown();
        }

        //从城镇中进入到团本中，执行相应的操作
        private void OnEnterTeamDuplicationBuildSceneFrameTown()
        {
            //不是从城镇中进入
            if (TeamDuplicationDataManager.GetInstance().IsEnterTeamDuplicationBuildSceneFromTown == false)
                return;

            //标志位重置
            TeamDuplicationDataManager.GetInstance().IsEnterTeamDuplicationBuildSceneFromTown = false;

            //已经展示门票不足的提示，直接返回
            if (TeamDuplicationDataManager.GetInstance().IsAlreadyShowTicketIsNotEnoughTip == true)
            {
                return;
            }

            //不可以挑战,不会触发
            if (TeamDuplicationUtility.IsPlayerCanDoTeamDuplication() == false)
            {
                return;
            }

            //门票足够，不会触发
            if (TeamDuplicationUtility.IsPlayerTicketIsEnough() == true)
            {
                return;
            }

            //设置标志位，打开界面
            TeamDuplicationDataManager.GetInstance().IsAlreadyShowTicketIsNotEnoughTip = true;
            //打开提示框
            CommonUtility.OnShowCommonMsgBox(TR.Value("team_duplication_less_cost_item_tip"),
                OnOpenTeamDuplicationShop, TR.Value("common_data_forward"));


        }

        private void OnOpenTeamDuplicationShop()
        {
            ShopNewDataManager.GetInstance().OpenShopNewFrame(TeamDuplicationDataManager.TeamDuplicationShopId);
        }

        #endregion 

    }
}
