using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;
using Random = System.Random;

namespace GameClient
{
    //团队副本助手类

    public static class TeamDuplicationUtility
    {
        #region OpenFrame

        //团本中的聊天
        public static void OnOpenTeamDuplicationChatFrame()
        {
            OnCloseTeamDuplicationChatFrame();

            ClientSystemManager.GetInstance().OpenFrame<ChatFrame>(FrameLayer.Middle);
        }

        public static void OnCloseTeamDuplicationChatFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<ChatFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<ChatFrame>();
        }

        public static void OnCloseTeamDuplicationTipFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<ItemTipFrame>();
        }

        //团本中的技能界面
        public static void OnOpenTeamDuplicationSkillFrame()
        {
            OnCloseTeamDuplicationSkillFrame();
            ClientSystemManager.GetInstance().OpenFrame<SkillFrame>(FrameLayer.Middle);
        }

        private static void OnCloseTeamDuplicationSkillFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<SkillFrame>())
                ClientSystemManager.GetInstance().CloseFrame<SkillFrame>();
        }

        //团本中的背包界面
        public static void OnOpenTeamDuplicationPackageNewFrame()
        {
            OnCloseTeamDuplicationPackageNewFrame();
            ClientSystemManager.GetInstance().OpenFrame<PackageNewFrame>(FrameLayer.Middle);
        }

        private static void OnCloseTeamDuplicationPackageNewFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<PackageNewFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<PackageNewFrame>();
        }

        //组队场景的主界面
        public static void OnCloseTeamDuplicationMainBuildFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationMainBuildFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationMainBuildFrame>();
        }

        public static void OnOpenTeamDuplicationMainBuildFrame()
        {
            OnCloseTeamDuplicationMainBuildFrame();

            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationMainBuildFrame>(FrameLayer.Bottom);
        }

        //团队列表系统
        public static void OnCloseTeamDuplicationTeamListFrame()
        {
            
            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationTeamListFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationTeamListFrame>();
        }

        public static void OnOpenTeamDuplicationTeamListFrame()
        {
            OnCloseTeamDuplicationTeamListFrame();

            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationTeamListFrame>(FrameLayer.Middle);
        }

        //创建团队
        public static void OnCloseTeamDuplicationTeamBuildFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationTeamBuildFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationTeamBuildFrame>();
        }

        public static void OnOpenTeamDuplicationTeamBuildFrame()
        {
            OnCloseTeamDuplicationTeamBuildFrame();

            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationTeamBuildFrame>();
        }

        //设置团队
        public static void OnCloseTeamDuplicationTeamSettingFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationTeamSettingFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationTeamSettingFrame>();

        }

        public static void OnOpenTeamDuplicationTeamSettingFrame(TeamDuplicationTeamBuildDataModel teamBuildDataModel)
        {
            OnCloseTeamDuplicationTeamSettingFrame();

            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationTeamSettingFrame>(FrameLayer.Middle, teamBuildDataModel);
        }

        //查看队员权限
        public static void OnCloseTeamDuplicationTeamPermissionFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationTeamPermissionFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationTeamPermissionFrame>();
        }

        public static void OnOpenTeamDuplicationTeamPermissionFrame(TeamDuplicationPlayerDataModel ownerPlayerDataModel,
            TeamDuplicationPlayerDataModel selectedPlayerDataModel,
            Vector2 clickScreenPosition)
        {
            OnCloseTeamDuplicationTeamPermissionFrame();

            TeamDuplicationPermissionDataModel permissionDataModel = new TeamDuplicationPermissionDataModel();
            permissionDataModel.OwnerPlayerDataModel = ownerPlayerDataModel;
            permissionDataModel.SelectedPlayerDataModel = selectedPlayerDataModel;
            permissionDataModel.ClickScreenPosition = clickScreenPosition;

            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationTeamPermissionFrame>(FrameLayer.Middle, permissionDataModel);
        }

        //团本房间
        public static void OnCloseTeamDuplicationTeamRoomFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationTeamRoomFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationTeamRoomFrame>();
        }

        public static void OnOpenTeamDuplicationTeamRoomFrame(int teamId = 0)
        {
            OnCloseTeamDuplicationTeamRoomFrame();

            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationTeamRoomFrame>(FrameLayer.Middle, teamId);
        }

        //队伍邀请者列表
        public static void OnCloseTeamDuplicationTeamInviteListFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationTeamInviteListFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationTeamInviteListFrame>();
        }

        //打开界面的时候，设置标志位
        private static void SetTeamDuplicationOwnerTeamInviteFlag()
        {
            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerNewInvite == true)
            {
                TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerNewInvite = false;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationOwnerNewTeamInviteMessage);
        }

        public static void OnOpenTeamDuplicationTeamInviteListFrame()
        {
            SetTeamDuplicationOwnerTeamInviteFlag();

            OnCloseTeamDuplicationTeamInviteListFrame();

            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationTeamInviteListFrame>(FrameLayer.Middle);
        }

        //找队友
        public static void OnCloseTeamDuplicationFindTeamMateFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationFindTeamMateFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationFindTeamMateFrame>();
        }

        public static void OnOpenTeamDuplicationFindTeamMateFrame()
        {
            OnCloseTeamDuplicationFindTeamMateFrame();

            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationFindTeamMateFrame>(FrameLayer.Middle);
        }


        //团队申请者
        public static void OnCloseTeamDuplicationRequesterListFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationRequesterListFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationRequesterListFrame>();
        }

        //打开申请者列表的界面时候，重置申请标志
        private static void SetTeamDuplicationOwnerNewRequesterFlag()
        {
            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerNewRequester == true)
                TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerNewRequester = false;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationOwnerNewRequesterMessage);
        }

        public static void OnOpenTeamDuplicationRequesterListFrame()
        {
            SetTeamDuplicationOwnerNewRequesterFlag();
            OnCloseTeamDuplicationRequesterListFrame();

            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationRequesterListFrame>(FrameLayer.Middle);
        }

        //战斗主界面
        public static void OnCloseTeamDuplicationMainFightFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationMainFightFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationMainFightFrame>();
        }

        public static void OnOpenTeamDuplicationMainFightFrame()
        {
            OnCloseTeamDuplicationMainFightFrame();

            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationMainFightFrame>(FrameLayer.Bottom);
        }

        //战前设置
        public static void OnCloseTeamDuplicationFightPreSettingFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationFightPreSettingFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationFightPreSettingFrame>();
        }

        public static void OnOpenTeamDuplicationFightPreSettingFrame()
        {
            OnCloseTeamDuplicationFightPreSettingFrame();
            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationFightPreSettingFrame>(FrameLayer.Middle);
        }

        //战前配置小队难度选择
        public static void OnCloseTeamDuplicationFightSettingDifficultyFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationFightSettingDifficultyFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationFightSettingDifficultyFrame>();
        }

        public static void OnOpenTeamDuplicationFightSettingDifficultyFrame()
        {
            OnCloseTeamDuplicationFightSettingDifficultyFrame();
            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationFightSettingDifficultyFrame>(FrameLayer.Middle);
        }

        //战斗倒计时
        public static void OnCloseTeamDuplicationFightCountDownFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationFightCountDownFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationFightCountDownFrame>();
        }

        public static void OnOpenTeamDuplicationFightCountDownFrame()
        {
            OnCloseTeamDuplicationFightCountDownFrame();

            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationFightCountDownFrame>(FrameLayer.Middle);
        }

        //战斗开始投票
        public static void OnOpenTeamDuplicationFightStartVoteFrame()
        {
            OnCloseTeamDuplicationFightStartVoteFrame();
            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationFightStartVoteFrame>(FrameLayer.Middle);
        }

        public static void OnCloseTeamDuplicationFightStartVoteFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationFightStartVoteFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationFightStartVoteFrame>();
        }

        //战斗结束投票
        public static void OnOpenTeamDuplicationFightEndVoteFrame()
        {
            OnCloseTeamDuplicationFightEndVoteFrame();
            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationFightEndVoteFrame>(FrameLayer.Middle);
        }

        public static void OnCloseTeamDuplicationFightEndVoteFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationFightEndVoteFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationFightEndVoteFrame>();
        }

        //阶段开始
        public static void OnOpenTeamDuplicationFightStageBeginDescriptionFrame(int stageNumber)
        {

            OnCloseTeamDuplicationFightStageBeginDescriptionFrame();

            TeamDuplicationFightStageDescriptionDataModel stageDescriptionDataModel =
                new TeamDuplicationFightStageDescriptionDataModel();

            stageDescriptionDataModel.StageNumber = stageNumber;

            ClientSystemManager.GetInstance()
                .OpenFrame<TeamDuplicationFightStageBeginDescriptionFrame>(FrameLayer.Middle, stageDescriptionDataModel);

        }

        public static void OnCloseTeamDuplicationFightStageBeginDescriptionFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationFightStageBeginDescriptionFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationFightStageBeginDescriptionFrame>();
        }

        //阶段结束
        public static void OnOpenTeamDuplicationFightStageEndDescriptionFrame(int stageNumber)
        {

            OnCloseTeamDuplicationFightStageEndDescriptionFrame();

            TeamDuplicationFightStageDescriptionDataModel stageDescriptionDataModel =
                new TeamDuplicationFightStageDescriptionDataModel();

            stageDescriptionDataModel.StageNumber = stageNumber;

            ClientSystemManager.GetInstance()
                .OpenFrame<TeamDuplicationFightStageEndDescriptionFrame>(FrameLayer.Middle, stageDescriptionDataModel);

        }

        public static void OnCloseTeamDuplicationFightStageEndDescriptionFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationFightStageEndDescriptionFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationFightStageEndDescriptionFrame>();
        }


        //阶段奖励
        public static void OnOpenTeamDuplicationMiddleStageRewardFrame(int stageId)
        {
            OnCloseTeamDuplicationMiddleStageRewardFrame();
            ClientSystemManager.GetInstance()
                .OpenFrame<TeamDuplicationMiddleStageRewardFrame>(FrameLayer.Middle, stageId);
        }

        public static void OnCloseTeamDuplicationMiddleStageRewardFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationMiddleStageRewardFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationMiddleStageRewardFrame>();
        }

        //游戏结束界面
        public static void OnOpenTeamDuplicationGameResultFrame(bool isSucceed = false)
        {
            //默认是游戏失败
            OnCloseTeamDuplicationGameResultFrame();
            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationGameResultFrame>(FrameLayer.Middle, isSucceed);
        }

        public static void OnCloseTeamDuplicationGameResultFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationGameResultFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationGameResultFrame>();
        }

        //大阶段翻牌
        public static void OnOpenTeamDuplicationFinalStageCardFrame()
        {
            //默认是游戏失败
            OnCloseTeamDuplicationFinalStageCardFrame();
            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationFinalStageCardFrame>(FrameLayer.Middle);
        }

        public static void OnCloseTeamDuplicationFinalStageCardFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationFinalStageCardFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationFinalStageCardFrame>();
        }

        //离开队伍队伍之后，关闭相关的界面
        public static void OnCloseRelationFrameByLeaveTeam()
        {
            OnCloseTeamDuplicationChatFrame();
            OnCloseTeamDuplicationTeamRoomFrame();
        }

        //加入团队之后，关闭团本列表和团本邀请列表界面
        public static void OnCloseRelationFrameByOwnerTeam()
        {
            OnCloseTeamDuplicationTeamListFrame();
            OnCloseTeamDuplicationTeamInviteListFrame();
        }

        //自动切换到组队场景中，关闭相关界面
        public static void OnCloseRelationFrameBySwitchBuildSceneInTeamDuplication()
        {
            OnCloseTeamDuplicationTeamRoomFrame();
            OnCloseTeamDuplicationTeamPermissionFrame();
            ClientSystemManager.GetInstance().CloseFrame<ActorShowGroup>();

            OnCloseTeamDuplicationChatFrame();
        }

        //自动切换到攻坚场景中，关闭相关界面
        public static void OnCloseRelationFrameBySwitchFightSceneInTeamDuplication()
        {
            OnCloseTeamDuplicationTeamRoomFrame();
            OnCloseTeamDuplicationTeamPermissionFrame();
            ClientSystemManager.GetInstance().CloseFrame<ActorShowGroup>();

            OnCloseTeamDuplicationChatFrame();

            if (ClientSystemManager.GetInstance().IsFrameOpen<SkillFrame>())
                ClientSystemManager.GetInstance().CloseFrame<SkillFrame>();

            if(ClientSystemManager.GetInstance().IsFrameOpen<PackageNewFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<PackageNewFrame>();

            ShopNewUtility.OnCloseShopNewFrame();
            ShopNewUtility.OnCloseShowNewPurchaseItemFrame();

            ClientSystemManager.GetInstance().CloseFrame<RelationFrameNew>();

            ItemTipManager.GetInstance().CloseAll();
            ClientSystemManager.GetInstance().CloseFrame<CommonMsgBoxOkCancelNewFrame>();
        }

        //关闭离开战斗场景提示的界面
        public static void OnCloseTeamDuplicationFightSceneLeaveTipFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationFightSceneLeaveTipFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationFightSceneLeaveTipFrame>();
        }

        //离开的提示页面
        public static void OnOpenTeamDuplicationFightSceneLeaveTipFrame()
        {
            OnCloseTeamDuplicationFightSceneLeaveTipFrame();
            ClientSystemManager.GetInstance().OpenFrame<TeamDuplicationFightSceneLeaveTipFrame>(FrameLayer.Middle);
        }

        //战斗阶段的攻坚面板
        public static void OnCloseTeamDuplicationFightStagePanelFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamDuplicationFightStagePanelFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamDuplicationFightStagePanelFrame>();
        }

        public static void OnOpenTeamDuplicationFightStagePanelFrame(int fightStageId)
        {
            OnCloseTeamDuplicationFightStagePanelFrame();

            ClientSystemManager.GetInstance()
                .OpenFrame<TeamDuplicationFightStagePanelFrame>(FrameLayer.Middle, fightStageId);
        }

        #endregion

        #region TeamDuplicationScene

        //是否需要重连到团本场景
        public static bool IsNeedReconnectToTeamDuplicationScene()
        {
            if (TeamDuplicationDataManager.GetInstance().TeamDuplicationReconnectSceneId <= 0)
                return false;

            var sceneId = TeamDuplicationDataManager.GetInstance().TeamDuplicationReconnectSceneId;

            var citySceneTable = TableManager.GetInstance().GetTableItem<CitySceneTable>(sceneId);
            if (citySceneTable == null)
                return false;

            if (citySceneTable.SceneSubType != CitySceneTable.eSceneSubType.TeamDuplicationBuid
                && citySceneTable.SceneSubType != CitySceneTable.eSceneSubType.TeamDuplicationFight)
                return false;

            return true;
        }

        //同意前往开战场景，并开战
        public static void OpenReconnectToTeamDuplicationSceneTip(Action onReconnectToSceneAction,
            Action onCancelAction)
        {
            var tipContent = TR.Value("team_duplication_Reconnect_to_scene");
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = tipContent,
                IsShowNotify = false,
                LeftButtonText = TR.Value("common_data_cancel"),
                RightButtonText = TR.Value("common_data_sure"),
                OnLeftButtonClickCallBack = onCancelAction,
                OnRightButtonClickCallBack = onReconnectToSceneAction,
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }

        //重连到场景
        public static void ReconnectToTeamDuplicationScene()
        {

            //需要重连到攻坚场景的时候，已经过期
            if (TimeManager.GetInstance().GetServerTime() >
                TeamDuplicationDataManager.GetInstance().TeamDuplicationReconnectExpireTime)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_reconnect_failed_by_pass_time"));
                TeamDuplicationDataManager.GetInstance().ResetTeamDuplicationReconnectData();
                return;
            }

            var sceneId = TeamDuplicationDataManager.GetInstance().TeamDuplicationReconnectSceneId;
            TeamDuplicationDataManager.GetInstance().ResetTeamDuplicationReconnectData();

            if (sceneId == TeamDuplicationDataManager.GetInstance().TeamDuplicationBuildSceneId
                || sceneId == TeamDuplicationDataManager.GetInstance().TeamDuplicationFightSceneId)
            {
                //切换到对应场景
                SwitchToTeamDuplicationScene(sceneId);
                //请求自己队伍的数据
                TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamDataReq(0);
            }
        }

        //切换到赛利亚房间
        public static void SwitchToTeamDuplicationBirthCityScene()
        {
            var systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;

            if (systemTown == null)
                return;

            var townTableData = TableManager.GetInstance().GetTableItem<CitySceneTable>(
                systemTown.CurrentSceneID);

            if (townTableData == null)
                return;

            var sceneParams = new SceneParams()
            {
                currSceneID = systemTown.CurrentSceneID,
                currDoorID = 0,
                targetSceneID = townTableData.BirthCity,
                targetDoorID = 0,
            };

            GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(sceneParams));
        }

        //从攻坚场景回退到组队场景
        public static void BackToTeamDuplicationBuildScene()
        {
            SwitchToTeamDuplicationScene(TeamDuplicationDataManager.GetInstance().TeamDuplicationBuildSceneId);
        }

        //进入组队场景或者重连到攻坚场景(从城镇中）
        public static void EnterToTeamDuplicationSceneFromTown()
        {
            //正处在组队的状态
            if (TeamDataManager.GetInstance().HasTeam() == true)
            {
                SystemNotifyManager.SystemNotify(1104);
                return;
            }

            //不需要重连，直接进入组队
            if (TeamDuplicationDataManager.GetInstance().TeamDuplicationReconnectSceneId <= 0)
            {
                EnterInTeamDuplicationBuildScene();
                return;
            }

            //已经过期，直接进入组队
            if (TimeManager.GetInstance().GetServerTime() >
                TeamDuplicationDataManager.GetInstance().TeamDuplicationReconnectExpireTime)
            {
                EnterInTeamDuplicationBuildScene();
                return;
            }

            //需要重连到攻坚场景，并且没有过期，进入到攻坚场景
            if (TeamDuplicationDataManager.GetInstance().TeamDuplicationReconnectSceneId
                == TeamDuplicationDataManager.GetInstance().TeamDuplicationFightSceneId)
            {
                //切换到攻坚场景
                SwitchToTeamDuplicationScene(TeamDuplicationDataManager.GetInstance().TeamDuplicationFightSceneId);
                TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamDataReq(0);
                //数据重置
                TeamDuplicationDataManager.GetInstance().ResetTeamDuplicationReconnectData();
                return;
            }
            else
            {
                //切换到组队场景
                EnterInTeamDuplicationBuildScene();
                return;
            }
        }

        //进入到组队场景(从城镇点击前往)
        public static void EnterInTeamDuplicationBuildScene()
        {
            if (TeamDataManager.GetInstance().HasTeam() == true)
            {
                SystemNotifyManager.SystemNotify(1104);
                return;
            }

            //从城镇进入到团本，设置标志
            TeamDuplicationDataManager.GetInstance().IsEnterTeamDuplicationBuildSceneFromTown = true;
            SwitchToTeamDuplicationScene(TeamDuplicationDataManager.GetInstance().TeamDuplicationBuildSceneId);
            //请求自己队伍的数据
            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyTeamDataReq(0);
        }

        //团本切换到攻坚场景
        public static void SwitchToTeamDuplicationFightScene()
        {
            SwitchToTeamDuplicationScene(TeamDuplicationDataManager.GetInstance().TeamDuplicationFightSceneId);
        }

        //在攻坚场景中切换
        private static void SwitchToTeamDuplicationScene(int teamDuplicationSceneId)
        {
            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;

            if (systemTown == null)
                return;

            var townTableData = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (townTableData == null)
                return;

            if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>() == true)
            {
                var systemTownFrame =
                    ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;

                if (systemTownFrame != null)
                    systemTownFrame.SetForbidFadeIn(true);
            }

            var sceneParams = new SceneParams()
            {
                currSceneID = systemTown.CurrentSceneID,
                currDoorID = 0,
                targetSceneID = teamDuplicationSceneId,
                targetDoorID = 0,
            };

            GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(sceneParams));
        }

        //是否在攻坚场景
        public static bool IsTeamDuplicationInFightScene()
        {
            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;

            if (systemTown == null)
                return false;

            var townTableData = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (townTableData == null)
                return false;

            if (townTableData.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationFight)
                return true;

            return false;
        }

        //是否在组队场景
        public static bool IsTeamDuplicationInBuildScene()
        {
            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;

            if (systemTown == null)
                return false;

            var townTableData = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (townTableData == null)
                return false;

            if (townTableData.SceneSubType == CitySceneTable.eSceneSubType.TeamDuplicationBuid)
                return true;

            return false;
        }

        //退出队伍时，如果在攻坚场景，则回退到组队场景
        public static void SwitchTeamDuplicationToBuildSceneByQuitTeam()
        {
            //组队场景，不做操作
            if (IsTeamDuplicationInBuildScene() == true)
                return;

            //在攻坚场景中：因为退出了队伍，则直接切换到组队场景
            if (IsTeamDuplicationInFightScene() == true)
            {
                BackToTeamDuplicationBuildScene();
            }
        }

        //同意开战时：如果在组队场景，则前进到攻坚场景
        public static void SwitchTeamDuplicationToFightSceneByAgreeBattle()
        {
            //攻坚场景，不做操作
            if (IsTeamDuplicationInFightScene() == true)
                return;

            //在组队场景：因为同意了开战，则自动切换到攻坚场景
            if (IsTeamDuplicationInBuildScene() == true)
            {
                OnCloseRelationFrameBySwitchFightSceneInTeamDuplication();
                TeamDuplicationDataManager.GetInstance().FightVoteAgreeBySwitchFightScene = true;
                SwitchToTeamDuplicationFightScene();
            }
        }

        #endregion

        #region TeamDuplicationTeamDataModel

        //得到当前团本的装备评分
        public static int GetTeamDuplicationEquipmentScore()
        {
            var teamDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (teamDataModel == null)
                return 0;

            return teamDataModel.TeamEquipScore;
        }

        public static TeamDuplicationPlayerInformationDataModel CreateTeamDuplicationPlayerInformationDataModel()
        {
            var playerInformationDataModel = new TeamDuplicationPlayerInformationDataModel();

            ////从表中读出每日和每周的总的挑战次数
            //var dayNumberValueTable = TableManager.GetInstance().GetTableItem<TeamCopyValueTable>(
            //    TeamDuplicationDataManager.TeamDuplicationDayFightNumberId);
            //playerInformationDataModel.DayTotalFightNumber = dayNumberValueTable != null ? dayNumberValueTable.Value : 2;

            //var weekNumberValueTable = TableManager.GetInstance().GetTableItem<TeamCopyValueTable>(
            //    TeamDuplicationDataManager.TeamDuplicationWeekFightNumberId);
            //playerInformationDataModel.WeekTotalFightNumber =
            //    weekNumberValueTable != null ? weekNumberValueTable.Value : 2;

            return playerInformationDataModel;
        }

        //自己是否为团长
        public static bool IsSelfPlayerIsTeamLeaderInTeamDuplication()
        {
            var ownerPlayerDataModel = GetOwnerPlayerDataModel();
            if (ownerPlayerDataModel == null)
                return false;

            if (ownerPlayerDataModel.IsTeamLeader == true)
                return true;
            
            return false;
        }

        //团队是否为噩梦 难度
        public static bool IsTeamDuplicationTeamDifficultyHardLevel()
        {
            var teamDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (teamDataModel == null)
                return false;

            if ((TeamCopyTeamGrade) teamDataModel.TeamDifficultyLevel == TeamCopyTeamGrade.TEAM_COPY_TEAM_GRADE_DIFF)
                return true;

            return false;
        }

        //自己是否为队长
        public static bool IsSelfPlayerIsCaptainInTeamDuplication()
        {
            var ownerPlayerDataModel = GetOwnerPlayerDataModel();
            if (ownerPlayerDataModel == null)
                return false;

            if (ownerPlayerDataModel.IsCaptain == true)
                return true;
            
            return false;
        }

        //是否为小队长或者团长
        public static bool IsSelfPlayerIsCaptainOrTeamLeaderInTroop()
        {
            var ownerPlayerDataModel = GetOwnerPlayerDataModel();
            if (ownerPlayerDataModel == null)
                return false;

            if (ownerPlayerDataModel.IsTeamLeader == true
                || ownerPlayerDataModel.IsCaptain == true)
                return true;
            
            return false;
        }

        //得到团长的Guid
        public static ulong GetTeamLeaderPlayerGuid()
        {
            var teamTroopDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (teamTroopDataModel == null
                || teamTroopDataModel.CaptainDataModelList == null
                || teamTroopDataModel.CaptainDataModelList.Count <= 0)
                return 0;

            for (var i = 0; i < teamTroopDataModel.CaptainDataModelList.Count; i++)
            {
                var troopDataModel = teamTroopDataModel.CaptainDataModelList[i];
                if (troopDataModel != null && troopDataModel.PlayerList != null
                                           && troopDataModel.PlayerList.Count > 0)
                {
                    for (var j = 0; j < troopDataModel.PlayerList.Count; j++)
                    {
                        var curPlayerDataModel = troopDataModel.PlayerList[j];
                        //角色的类型为团长类型
                        if (curPlayerDataModel != null)
                        {
                            if (curPlayerDataModel.IsTeamLeader == true)
                                return curPlayerDataModel.Guid;
                        }
                    }
                }
            }

            return 0;
        }

        //创建团本的角色
        public static TeamDuplicationPlayerDataModel CreatePlayerDataModel(TeamCopyMember teamCopyMember)
        {
            TeamDuplicationPlayerDataModel playerDataModel =
                new TeamDuplicationPlayerDataModel();
            playerDataModel.Guid = teamCopyMember.playerId;
            playerDataModel.Name = teamCopyMember.playerName;
            playerDataModel.ProfessionId = (int)teamCopyMember.playerOccu;
            playerDataModel.PlayerAwakeState = (int) teamCopyMember.playerAwaken;
            playerDataModel.Level = (int)teamCopyMember.playerLvl;
            playerDataModel.EquipmentScore = (int)teamCopyMember.equipScore;
            playerDataModel.SeatId = (int)teamCopyMember.seatId;
            //非0:足够，0:不足
            playerDataModel.TicketIsEnough = teamCopyMember.ticketIsEnough != 0;
            playerDataModel.ZoneId = (int)teamCopyMember.zoneId;
            playerDataModel.ExpireTime = teamCopyMember.expireTime;

            //是否为团长
            playerDataModel.IsTeamLeader = ((teamCopyMember.post & (int) TeamCopyPost.TEAM_COPY_POST_CHIEF) != 0);
            //是否为队长
            playerDataModel.IsCaptain = ((teamCopyMember.post & (int) TeamCopyPost.TEAM_COPY_POST_CAPTAIN) != 0);
            //是否为金主
            playerDataModel.IsGoldOwner = ((teamCopyMember.post & (int)TeamCopyPost.TEAM_COPY_POST_GOLD) != 0);

            return playerDataModel;
        }

        //创建队伍属性列表
        public static List<ComControlData> GetTeamPropertyDataListByType(TeamCopyTeamModel teamModel,
            TeamDuplicationTeamPropertyType troopPropertyType)
        {
            List<ComControlData> troopPropertyValueList = new List<ComControlData>();
            
            int propertyId = 1;

            var teamDuplicationSetTables = TableManager.GetInstance().GetTable<TeamDuplicationSetTable>();
            if (teamDuplicationSetTables == null)
                return troopPropertyValueList;

            var setTableIter = teamDuplicationSetTables.GetEnumerator();
            while (setTableIter.MoveNext())
            {
                var curSetTable = setTableIter.Current.Value as TeamDuplicationSetTable;
                if (curSetTable == null)
                    continue;

                if (curSetTable.TeamType == (int) teamModel
                    && curSetTable.Type == (int) troopPropertyType)
                {
                    ComControlData curPropertyData = new ComControlData();
                    curPropertyData.Id = propertyId;
                    curPropertyData.Name = curSetTable.Number.ToString();
                    curPropertyData.Index = curSetTable.Number;
                    troopPropertyValueList.Add(curPropertyData);
                    propertyId += 1;
                }
            }

            if (troopPropertyValueList.Count > 1)
            {
                troopPropertyValueList.Sort((x, y) => x.Index.CompareTo(y.Index));
            }
            return troopPropertyValueList;
        }

        //得到角色的类型
        private static TeamDuplicationPlayerType GetPlayerType(uint post)
        {
            //团长
            if ((post & (int)TeamCopyPost.TEAM_COPY_POST_CHIEF) > 0)
            {
                return TeamDuplicationPlayerType.TeamLeader;
            }

            //队长
            if ((post & (int)TeamCopyPost.TEAM_COPY_POST_CAPTAIN) > 0)
            {
                return TeamDuplicationPlayerType.Captain;
            }

            //队员
            return TeamDuplicationPlayerType.Player;
        }

        //两个成员是否是在同一个小队
        public static bool IsInSameTroopOfTwoPlayer(ulong ownerGuid, ulong selectedGuid)
        {
            //同一个人
            if (ownerGuid == 0 || selectedGuid == 0
                || ownerGuid == selectedGuid)
                return false;

            var ownerTroopDataModel = GetTeamDuplicationCaptainDataModelByPlayerGuid(ownerGuid);

            //小队只有一个人
            if (ownerTroopDataModel == null || ownerTroopDataModel.PlayerList == null
                                            || ownerTroopDataModel.PlayerList.Count <= 1)
                return false;

            for (var i = 0; i < ownerTroopDataModel.PlayerList.Count; i++)
            {
                var curPlayerDataModel = ownerTroopDataModel.PlayerList[i];
                if (curPlayerDataModel != null && curPlayerDataModel.Guid == selectedGuid)
                    return true;
            }

            return false;
        }

        //根据PlayerGUId得到小队的ID，小队ID:1,2,3,4
        public static int GetTeamDuplicationCaptainIdByPlayerGuid(ulong guid)
        {
            var captainDataModel = GetTeamDuplicationCaptainDataModelByPlayerGuid(guid);
            if (captainDataModel == null)
                return 0;

            return captainDataModel.CaptainId;
        }

        //得到PlayerId所在小队的数据信息
        public static TeamDuplicationCaptainDataModel GetTeamDuplicationCaptainDataModelByPlayerGuid(ulong guid)
        {
            var teamTroopDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();

            if (teamTroopDataModel == null
                || teamTroopDataModel.CaptainDataModelList == null
                || teamTroopDataModel.CaptainDataModelList.Count <= 0)
                return null;

            for (var i = 0; i < teamTroopDataModel.CaptainDataModelList.Count; i++)
            {
                var troopDataModel = teamTroopDataModel.CaptainDataModelList[i];
                if (troopDataModel != null && troopDataModel.PlayerList != null
                                           && troopDataModel.PlayerList.Count > 0)
                {
                    for (var j = 0; j < troopDataModel.PlayerList.Count; j++)
                    {
                        var curPlayerDataModel = troopDataModel.PlayerList[j];
                        if (curPlayerDataModel != null
                            && curPlayerDataModel.Guid == guid)
                            return troopDataModel;
                    }
                }
            }

            return null;
        }

        //得到PlayerId所在小队的队员个数
        public static int GetTeamDuplicationCaptainPlayerNumberByPlayerGuid(ulong guid)
        {
            var captainDataModel = GetTeamDuplicationCaptainDataModelByPlayerGuid(guid);

            if (captainDataModel == null
                || captainDataModel.PlayerList == null
                || captainDataModel.PlayerList.Count <= 0)
                return 0;

            int playerNumber = 0;
            for (var i = 0; i < captainDataModel.PlayerList.Count; i++)
            {
                var playerDataModel = captainDataModel.PlayerList[i];
                if (playerDataModel != null)
                    playerNumber += 1;
            }

            return playerNumber;
        }

        //根据小队的ID，得到小队的数据
        public static TeamDuplicationCaptainDataModel GetTeamDuplicationCaptainDataModelByCaptainId(int captainId)
        {
            var teamTroopDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();

            if (teamTroopDataModel == null
                || teamTroopDataModel.CaptainDataModelList == null
                || teamTroopDataModel.CaptainDataModelList.Count <= 0)
                return null;

            for (var i = 0; i < teamTroopDataModel.CaptainDataModelList.Count; i++)
            {
                var captainDataModel = teamTroopDataModel.CaptainDataModelList[i];
                if(captainDataModel == null)
                    continue;

                if (captainDataModel.CaptainId == captainId)
                    return captainDataModel;
            }

            return null;
        }

        //根据小队的Id，得到小队中总的装备评分
        public static int GetTeamDuplicationCaptainTotalEquipmentScore(int captainId)
        {
            var captainDataModel = GetTeamDuplicationCaptainDataModelByCaptainId(captainId);
            var totalEquipmentScore = 0;

            if (captainDataModel == null || captainDataModel.PlayerList == null
                                         || captainDataModel.PlayerList.Count <= 0)
                return totalEquipmentScore;

            for (var i = 0; i < captainDataModel.PlayerList.Count; i++)
            {
                var playerDataModel = captainDataModel.PlayerList[i];
                if (playerDataModel != null)
                    totalEquipmentScore += playerDataModel.EquipmentScore;
            }

            return totalEquipmentScore;
        }

        //设置小队的默认难度（根据每个小队的装备评分设置，评分越高，难度越大);
        public static void SetTeamDuplicationDefaultCaptainDifficultySetting()
        {
            ////小队的装备评分
            //List<TeamDuplicationCaptainEquipmentScore> captainEquipmentScoreList =
            //    new List<TeamDuplicationCaptainEquipmentScore>();

            //for(var i = 0; i < TeamDuplicationDataManager.GetInstance().TeamDuplicationCaptainNumber; i++)
            //{
            //    var captainEquipmentScore = new TeamDuplicationCaptainEquipmentScore();
            //    captainEquipmentScore.CaptainId = i + 1;
            //    captainEquipmentScore.TotalEquipmentScore = GetTeamDuplicationCaptainTotalEquipmentScore(i + 1);
            //    captainEquipmentScoreList.Add(captainEquipmentScore);
            //}

            ////装备评分从高到低，评分一直的时候，Id从小到大
            //captainEquipmentScoreList.Sort((x, y) =>
            //{
            //    var a = -x.TotalEquipmentScore.CompareTo(y.TotalEquipmentScore);
            //    if (a == 0)
            //        a = x.CaptainId.CompareTo(y.CaptainId);

            //    return a;
            //});

            //难度设置
            List<TeamDuplicationTeamDifficultyConfigDataModel> defaultTeamConfigDataModelList =
                new List<TeamDuplicationTeamDifficultyConfigDataModel>();
            //难度从1-4，对应小队1-4
            for (var i = 0; i < TeamDuplicationDataManager.GetInstance().TeamDuplicationCaptainNumber; i++)
            {
                var teamConfigDataModel = new TeamDuplicationTeamDifficultyConfigDataModel();
                teamConfigDataModel.Difficulty = (TeamCopyGrade) (i + 1);
                teamConfigDataModel.TeamId = i + 1;
                //var captainEquipmentScore = captainEquipmentScoreList[i];
                //if (captainEquipmentScore != null)
                //{
                //    teamConfigDataModel.TeamId = captainEquipmentScore.CaptainId;
                //}

                defaultTeamConfigDataModelList.Add(teamConfigDataModel);
            }

            TeamDuplicationDataManager.GetInstance().InitTeamConfigDataModelList();
            TeamDuplicationDataManager.GetInstance().UpdateTeamConfigDataModelList(defaultTeamConfigDataModelList);
        }

        //得到自己的数据
        public static TeamDuplicationPlayerDataModel GetOwnerPlayerDataModel()
        {
            return GetPlayerDataModelByGuid(PlayerBaseData.GetInstance().RoleID);
        }

        //得到某个角色的数据
        public static TeamDuplicationPlayerDataModel GetPlayerDataModelByGuid(ulong playerGuid)
        {
            var teamTroopDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (teamTroopDataModel == null
                || teamTroopDataModel.CaptainDataModelList == null
                || teamTroopDataModel.CaptainDataModelList.Count <= 0)
                return null;

            for (var i = 0; i < teamTroopDataModel.CaptainDataModelList.Count; i++)
            {
                var troopDataModel = teamTroopDataModel.CaptainDataModelList[i];
                if (troopDataModel != null && troopDataModel.PlayerList != null
                                           && troopDataModel.PlayerList.Count > 0)
                {
                    for (var j = 0; j < troopDataModel.PlayerList.Count; j++)
                    {
                        var curPlayerDataModel = troopDataModel.PlayerList[j];
                        if (curPlayerDataModel != null
                            && curPlayerDataModel.Guid == playerGuid)
                            return curPlayerDataModel;
                    }
                }
            }
            return null;
        }

        //判断每个小队是否有队员
        public static bool IsEveryTroopOwnerPlayer()
        {
            //团本不存在
            var teamTroopDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (teamTroopDataModel == null
                || teamTroopDataModel.CaptainDataModelList == null
                || teamTroopDataModel.CaptainDataModelList.Count <= 0)
                return false;

            //小队数量少于4个
            if (teamTroopDataModel.CaptainDataModelList.Count < 4)
                return false;

            for (var i = 0; i < teamTroopDataModel.CaptainDataModelList.Count; i++)
            {
                var troopDataModel = teamTroopDataModel.CaptainDataModelList[i];
                //某个小队不存在
                if (troopDataModel == null)
                    return false;

                //小队的成员列表不存在
                if (troopDataModel.PlayerList == null)
                    return false;

                //小队中不存在队员
                if (troopDataModel.PlayerList.Count <= 0)
                    return false;

            }

            return true;
        }

        //判断团队成员是否满：存在4个小队，并且每个小队的人数都是3人
        public static bool IsTeamTroopIsFull()
        {
            //团本不存在
            var teamTroopDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (teamTroopDataModel == null
                || teamTroopDataModel.CaptainDataModelList == null
                || teamTroopDataModel.CaptainDataModelList.Count <= 0)
                return false;

            //小队数量少于4个
            if (teamTroopDataModel.CaptainDataModelList.Count < 4)
                return false;

            for (var i = 0; i < teamTroopDataModel.CaptainDataModelList.Count; i++)
            {
                var troopDataModel = teamTroopDataModel.CaptainDataModelList[i];
                //某个小队不存在
                if (troopDataModel == null)
                    return false;

                //小队的成员列表不存在
                if (troopDataModel.PlayerList == null)
                    return false;

                //存在队员为null
                for (var j = 0; j < troopDataModel.PlayerList.Count; j++)
                {
                    if (troopDataModel.PlayerList[j] == null)
                        return false;
                }
            }

            //团队已经满了
            return true;
        }

        //团队类型的列表
        public static List<ComControlData> GetTeamDuplicationTeamTypeDataList()
        {
            List<ComControlData> teamTypeDataList = new List<ComControlData>();

            ComControlData allTeamData = new ComControlData();
            allTeamData.Id = (int)TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_DEFAULT;
            allTeamData.Name = TR.Value("team_duplication_troop_all_team_name");
            teamTypeDataList.Add(allTeamData);

            ComControlData challengeModel = new ComControlData();
            challengeModel.Id = (int)TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_CHALLENGE;
            challengeModel.Name = TR.Value("team_duplication_troop_challenge_model_name");
            teamTypeDataList.Add(challengeModel);

            ComControlData goldModel = new ComControlData();
            goldModel.Id = (int)TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_GOLD;
            goldModel.Name = TR.Value("team_duplication_troop_gold_team_name");
            teamTypeDataList.Add(goldModel);

            return teamTypeDataList;
        }

        //队伍状态
        public static string GetCaptainStatusDescription(int captainStatus)
        {
            TeamCopySquadStatus teamCopySquadStatus = (TeamCopySquadStatus) captainStatus;
            switch (teamCopySquadStatus)
            {
                case TeamCopySquadStatus.TEAM_COPY_SQUAD_STATUS_BATTLE:
                    return TR.Value("team_duplication_captain_status_battle");
                case TeamCopySquadStatus.TEAM_COPY_SQUAD_STATUS_PREPARE:
                    return TR.Value("team_duplication_captain_status_prepare");
            }

            return TR.Value("team_duplication_captain_status_init");
        }

        //团本的数据是否为自己的团本
        public static bool IsOwnerTeamDuplicationDataModel(TeamCopyTeamDataRes teamDataRes)
        {
            if (teamDataRes == null)
                return false;

            if (teamDataRes.squadList == null || teamDataRes.squadList.Length <= 0)
                return false;

            for (var i = 0; i < teamDataRes.squadList.Length; i++)
            {
                var squadDataModel = teamDataRes.squadList[i];
                if(squadDataModel == null)
                    continue;

                if(squadDataModel.teamMemberList == null || squadDataModel.teamMemberList.Length <= 0)
                    continue;

                for (var j = 0; j < squadDataModel.teamMemberList.Length; j++)
                {
                    var teamMember = squadDataModel.teamMemberList[j];
                    if (teamMember != null && teamMember.playerId == PlayerBaseData.GetInstance().RoleID)
                        return true;
                }
            }

            return false;
        }

        //更新自己团本的数据
        public static void UpdateTeamDataModelByTeamCopyTeamDataRes(TeamDuplicationTeamDataModel teamDataModel,
            TeamCopyTeamDataRes teamDataRes)
        {
            if (teamDataModel == null || teamDataRes == null)
                return;

            //更新团本的数据
            teamDataModel.TeamId = (int)teamDataRes.teamId;
            teamDataModel.TeamName = teamDataRes.teamName;
            teamDataModel.TotalCommission = (int)teamDataRes.totalCommission;
            teamDataModel.BonusCommission = (int)teamDataRes.bonusCommission;
            teamDataModel.AutoAgreeGold = teamDataRes.autoAgreeGold != 0;
            teamDataModel.TeamModel = (int)teamDataRes.teamModel;
            teamDataModel.TeamStatus = (TeamCopyTeamStatus) teamDataRes.status;
            teamDataModel.TeamDifficultyLevel = teamDataRes.teamGrade;
            teamDataModel.TeamEquipScore = (int) teamDataRes.equipScore;

            //创建小队的数据
            for (var i = 0; i < TeamDuplicationDataManager.GetInstance().TeamDuplicationCaptainNumber; i++)
            {
                var troopDataModel = new TeamDuplicationCaptainDataModel();
                troopDataModel.CaptainId = i + 1;  //小队ID， 1-4；

                for (var j = 0; j < TeamDuplicationDataManager.GetInstance().TeamDuplicationPlayerNumberInCaptain; j++)
                {
                    troopDataModel.PlayerList.Add(null);
                }

                teamDataModel.CaptainDataModelList.Add(troopDataModel);
            }

            //更新小队的数量
            if (teamDataRes.squadList != null
                && teamDataRes.squadList.Length > 0)
            {
                //使用服务器的数据，更新小队的状态以及小队中成员的数量
                for (var i = 0; i < teamDataRes.squadList.Length; i++)
                {
                    var squadData = teamDataRes.squadList[i];

                    if(squadData == null)
                        continue;

                    TeamDuplicationCaptainDataModel captainDataModel = null;
                    for (var j = 0; j < teamDataModel.CaptainDataModelList.Count; j++)
                    {
                        var curCaptainDataModel = teamDataModel.CaptainDataModelList[j];
                        if (curCaptainDataModel != null && curCaptainDataModel.CaptainId == (int) squadData.squadId)
                        {
                            captainDataModel = curCaptainDataModel;
                            break;
                        }
                    }

                    if (captainDataModel == null)
                        continue;

                    captainDataModel.CaptainStatus = (int)squadData.squadStatus;

                    //更新小队中队员的信息
                    if (squadData.teamMemberList != null && squadData.teamMemberList.Length > 0)
                    {
                        for (var j = 0; j < squadData.teamMemberList.Length; j++)
                        {
                            var teamCopyMember = squadData.teamMemberList[j];
                            if (teamCopyMember != null)
                            {
                                var captainIndex = GetCaptainIndexBySeatId((int)teamCopyMember.seatId);
                                var playerIndex = GetPlayerIndexBySeatId((int)teamCopyMember.seatId);

                                if (captainIndex == captainDataModel.CaptainId
                                    && playerIndex >= 1
                                    && playerIndex <= TeamDuplicationDataManager.GetInstance().TeamDuplicationPlayerNumberInCaptain)
                                {

                                    var playerDataModel = CreatePlayerDataModel(teamCopyMember);

                                    captainDataModel.PlayerList[playerIndex - 1] = playerDataModel;
                                }
                            }
                        }
                    }
                    
                }
            }

        }

        //更新其他团本的详情数据
        public static void UpdateTeamDataModelByTeamCopyTeamDetailRes(TeamDuplicationTeamDataModel teamDataModel,
           TeamCopyTeamDetailRes teamCopyTeamDetailRes)
        {
            if (teamDataModel == null || teamCopyTeamDetailRes == null)
                return;

            //更新团本的数据
            teamDataModel.TeamId = (int)teamCopyTeamDetailRes.teamId;
            teamDataModel.TeamName = teamCopyTeamDetailRes.teamName;
            teamDataModel.TotalCommission = (int)teamCopyTeamDetailRes.totalCommission;
            teamDataModel.BonusCommission = (int)teamCopyTeamDetailRes.bonusCommission;
            teamDataModel.TeamModel = (int) teamCopyTeamDetailRes.teamModel;

            //创建小队的数据
            for (var i = 0; i < TeamDuplicationDataManager.GetInstance().TeamDuplicationCaptainNumber; i++)
            {
                var troopDataModel = new TeamDuplicationCaptainDataModel();
                troopDataModel.CaptainId = i + 1;  //小队ID， 1-4；

                for (var j = 0; j < TeamDuplicationDataManager.GetInstance().TeamDuplicationPlayerNumberInCaptain; j++)
                {
                    troopDataModel.PlayerList.Add(null);
                }

                teamDataModel.CaptainDataModelList.Add(troopDataModel);
            }

            //更新小队的数量
            if (teamCopyTeamDetailRes.squadList != null
                && teamCopyTeamDetailRes.squadList.Length > 0)
            {
                //使用服务器的数据，更新小队的状态以及小队中成员的数量
                for (var i = 0; i < teamCopyTeamDetailRes.squadList.Length; i++)
                {
                    var squadData = teamCopyTeamDetailRes.squadList[i];

                    if (squadData == null)
                        continue;

                    TeamDuplicationCaptainDataModel captainDataModel = null;
                    for (var j = 0; j < teamDataModel.CaptainDataModelList.Count; j++)
                    {
                        var curCaptainDataModel = teamDataModel.CaptainDataModelList[j];
                        if (curCaptainDataModel != null && curCaptainDataModel.CaptainId == (int)squadData.squadId)
                        {
                            captainDataModel = curCaptainDataModel;
                            break;
                        }
                    }

                    if (captainDataModel == null)
                        continue;

                    captainDataModel.CaptainStatus = (int)squadData.squadStatus;

                    //更新小队中队员的信息
                    if (squadData.teamMemberList != null && squadData.teamMemberList.Length > 0)
                    {
                        for (var j = 0; j < squadData.teamMemberList.Length; j++)
                        {
                            var teamCopyMember = squadData.teamMemberList[j];
                            if (teamCopyMember != null)
                            {
                                var captainIndex = GetCaptainIndexBySeatId((int) teamCopyMember.seatId);
                                var playerIndex = GetPlayerIndexBySeatId((int) teamCopyMember.seatId);

                                if (captainIndex == captainDataModel.CaptainId
                                    && playerIndex >= 1
                                    && playerIndex <= TeamDuplicationDataManager.GetInstance().TeamDuplicationPlayerNumberInCaptain)
                                {
                                    var playerDataModel = CreatePlayerDataModel(teamCopyMember);

                                    captainDataModel.PlayerList[playerIndex - 1] = playerDataModel;
                                }
                            }
                        }
                    }
                }
            }
        }

        //seatId 的数值： 1，2,3,4....12;
        //小队的Index， 1,2,3,4,
        //角色在小队中的Index， 1， 2， 3

        //根据座位得到小队的ID
        private static int GetCaptainIndexBySeatId(int seatId)
        {
            var captainIndex = (((int) seatId - 1) / 3 + 1);
            return captainIndex;
        }

        //根据座位得到所在小队的位置
        private static int GetPlayerIndexBySeatId(int seatId)
        {
            var playerIndex = ((int) seatId - 1) % 3 + 1;

            return playerIndex;
        }

        //得到团本的状态
        public static TeamCopyTeamStatus GetTeamDuplicationTeamStatus()
        {
            var teamDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (teamDataModel == null)
                return TeamCopyTeamStatus.TEAM_COPY_TEAM_STATUS_PARPARE;

            return teamDataModel.TeamStatus;
        }

        //团本中的角色是否处在战斗中
        public static bool IsTeamDuplicationPlayerInFightingStatus(TeamDuplicationPlayerDataModel playerDataModel)
        {
            if (playerDataModel == null)
                return false;

            var captainDataModel = GetTeamDuplicationCaptainDataModelByPlayerGuid(playerDataModel.Guid);
            if (captainDataModel == null)
                return false;

            if ((TeamCopySquadStatus) captainDataModel.CaptainStatus !=
                TeamCopySquadStatus.TEAM_COPY_SQUAD_STATUS_BATTLE)
                return false;

            return true;
        }

        //更新某个队员的离线时间
        public static void UpdateTeamDuplicationPlayerExpireTimeByGuid(ulong playerGuid, ulong expireTime)
        {
            var playerDataModel = GetPlayerDataModelByGuid(playerGuid);
            if (playerDataModel != null)
                playerDataModel.ExpireTime = expireTime;
        }

        //团本是否处在战斗阶段
        public static bool IsTeamDuplicationInFightingStatus()
        {
            var teamDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (teamDataModel == null)
                return false;

            if (teamDataModel.TeamStatus == TeamCopyTeamStatus.TEAM_COPY_TEAM_STATUS_BATTLE)
                return true;

            return false;
        }

        #endregion

        #region TeamDuplicationTeamBuild

        //团本的挑战次数是否达到上限
        public static bool IsFightNumberAlreadyReachLimit()
        {
            var playerInformationDataModel = TeamDuplicationDataManager.GetInstance().GetPlayerInformationDataModel();

            //数据不存在
            if (playerInformationDataModel == null)
                return true;
            
            //单日挑战达到上限
            if (playerInformationDataModel.DayAlreadyFightNumber >= playerInformationDataModel.DayTotalFightNumber)
                return true;

            //本周挑战达到上限
            if (playerInformationDataModel.WeekAlreadyFightNumber >= playerInformationDataModel.WeekTotalFightNumber)
                return true;

            //没有达到上限
            return false;
        }

        //判断是否佣金不足
        public static bool IsJoinInTeamDuplicationGoldIsNotEnough(int needGoldValue)
        {
            //非金主不用判断
            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationGoldOwner == false)
                return false;

            //金主，佣金足够
            if (PlayerBaseData.GetInstance().Gold >= (ulong)needGoldValue)
                return false;

            //佣金不足
            return true;
        }

        //团本邀请数据
        public static void UpdateTeamDuplicationTeamInviteDataModel(TCInviteInfo tcInviteInfo, TeamDuplicationTeamInviteDataModel teamInviteDataModel)
        {
            if (tcInviteInfo == null || teamInviteDataModel == null)
                return;

            teamInviteDataModel.TeamId =  tcInviteInfo.teamId;
            teamInviteDataModel.TeamType = (TeamCopyTeamModel) tcInviteInfo.teamModel;
            teamInviteDataModel.TeamDifficultyLevel = tcInviteInfo.teamGrade;
            teamInviteDataModel.TeamLeaderName = tcInviteInfo.name;
            teamInviteDataModel.TeamLeaderLevel = (int)tcInviteInfo.level;
            teamInviteDataModel.TeamLeaderProfessionId = (int) tcInviteInfo.occu;
            teamInviteDataModel.TeamLeaderAwakeState = (int) tcInviteInfo.awaken;
        }

        //请求者数据
        public static void UpdateTeamDuplicationRequesterDataModel(TeamCopyApplyProperty applyProperty,
            TeamDuplicationRequesterDataModel requesterDataModel)
        {
            if (applyProperty == null || requesterDataModel == null)
                return;

            requesterDataModel.PlayerGuid = applyProperty.playerId;
            requesterDataModel.Name = applyProperty.playerName;
            requesterDataModel.Level = (int)applyProperty.playerLevel;
            requesterDataModel.ProfessionId = (int)applyProperty.playerOccu;
            requesterDataModel.PlayerAwakeState = (int) applyProperty.playerAwaken;
            requesterDataModel.EquipmentScore = (int)applyProperty.equipScore;
            requesterDataModel.ZoneId = (int) applyProperty.zoneId;

            //0 非金主， 1 金主
            requesterDataModel.IsGold = applyProperty.isGold != 0;
            //是否为好友关系
            requesterDataModel.IsFriend = RelationUtility.IsRelationFriend(applyProperty.playerId);

            requesterDataModel.GuildId = applyProperty.guildId;
            //是否为同一工会
            requesterDataModel.IsGuild = RelationUtility.IsRelationSameGuild(applyProperty.guildId);
        }

        #endregion

        #region TeamDuplicationFightVote
        //同意前往开战场景，并开战
        public static void OnForwardFightSceneAndAgreeFight(string tipContent, Action onQuitAndReturn)
        {
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = tipContent,
                IsShowNotify = false,
                LeftButtonText = TR.Value("common_data_cancel"),
                RightButtonText = TR.Value("common_data_sure"),
                OnRightButtonClickCallBack = onQuitAndReturn,
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }

        //判断玩家是否已经战斗开始投票同意
        public static bool IsPlayerAlreadyAgreeFightStartVote(ulong playerGuid)
        {
            var agreePlayerList = TeamDuplicationDataManager.GetInstance().GetFightStartVoteAgreeList();
            if (agreePlayerList == null || agreePlayerList.Count <= 0)
                return false;

            for (var i = 0; i < agreePlayerList.Count; i++)
            {
                if (playerGuid == agreePlayerList[i])
                    return true;
            }

            return false;
        }


        //判断玩家是否已经战斗结束投票同意
        public static bool IsPlayerAlreadyAgreeFightEndVote(ulong playerGuid)
        {
            var fightEndVoteAgreeList = TeamDuplicationDataManager.GetInstance().FightEndVoteAgreeList;
            if (fightEndVoteAgreeList == null || fightEndVoteAgreeList.Count <= 0)
                return false;

            for (var i = 0; i < fightEndVoteAgreeList.Count; i++)
            {
                if (playerGuid == fightEndVoteAgreeList[i])
                    return true;
            }

            return false;
        }
        //判断玩家是否已经战斗结束投票同意
        public static bool IsPlayerAlreadyRefuseFightEndVote(ulong playerGuid)
        {
            var fightEndVoteRefuseList = TeamDuplicationDataManager.GetInstance().FightEndVoteRefuseList;
            if (fightEndVoteRefuseList == null || fightEndVoteRefuseList.Count <= 0)
                return false;

            for (var i = 0; i < fightEndVoteRefuseList.Count; i++)
            {
                if (playerGuid == fightEndVoteRefuseList[i])
                    return true;
            }

            return false;
        }
        
        #endregion

        #region TeamDuplicationFightPanel

        //自己团本战斗是否开启
        public static bool IsTeamDuplicationFightStart()
        {
            //准备阶段
            if (GetTeamDuplicationTeamStatus() == TeamCopyTeamStatus.TEAM_COPY_TEAM_STATUS_PARPARE)
                return false;

            //开战
            return true;
        }

        //据点是否在面板上
        public static bool IsFightPointShowInFightPanel(TeamDuplicationFightPointDataModel fightPointDataModel)
        {
            if (fightPointDataModel == null)
                return false;

            var fightPointStatusType = fightPointDataModel.FightPointStatusType;
            //无效状态和歼灭状态，不显示在面板上
            if (fightPointStatusType == TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_INVALID
                || fightPointStatusType == TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_DEFEAT)
                return false;

            return true;
        }

        //据点状态
        public static string GetFightPointStatusContent(TeamCopyFieldStatus fightPointStatusType)
        {
            switch (fightPointStatusType)
            {
                case TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_INVALID:
                    return TR.Value("team_duplication_setting_guide_model_invalid");
                case TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_INIT:
                    return TR.Value("team_duplication_setting_guide_model_can_challenge");
                case TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_REBORN:
                    return TR.Value("team_duplication_setting_guide_model_reborn");
                case TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_DEFEAT:
                    return TR.Value("team_duplication_setting_guide_model_destroy");
                case TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_URGENT:
                    return TR.Value("team_duplication_setting_guide_model_urgent");
                default:
                    return TR.Value("team_duplication_setting_guide_model_challenging");
            }
        }

        //更新据点数据
        public static void UpdateTeamDuplicationFightPointDataModel(
            TeamDuplicationFightPointDataModel fightPointDataModel,
            TeamCopyFeild teamCopyField)
        {
            fightPointDataModel.FightPointId = (int) teamCopyField.feildId;
            fightPointDataModel.FightPointLeftFightNumber = (int) teamCopyField.oddNum;
            fightPointDataModel.FightPointStatusType = (TeamCopyFieldStatus) teamCopyField.state;

            fightPointDataModel.FightPointRebornTime = (int) teamCopyField.rebornTime;
            fightPointDataModel.FightPointEnergyAccumulationStartTime = (int) teamCopyField.energyReviveTime;

            fightPointDataModel.FightPointTeamList = teamCopyField.attackSquadList.ToList();

            //位置
            var teamCopyFieldTable = TableManager.GetInstance().GetTableItem<TeamCopyFieldTable>(
                (int)teamCopyField.feildId);
            if (teamCopyFieldTable != null)
            {
                fightPointDataModel.FightPointPosition = teamCopyFieldTable.PositionIndex;
                fightPointDataModel.FightPointTotalFightNumber = teamCopyFieldTable.DefeatCond;
            }

            if (fightPointDataModel.FightPointTotalFightNumber < fightPointDataModel.FightPointLeftFightNumber)
                fightPointDataModel.FightPointTotalFightNumber = fightPointDataModel.FightPointLeftFightNumber;
        }

        //判断据点是否完成了解锁过程
        public static bool IsFightPointFinishUnlocking(TeamCopyFieldStatus fieldPreStatus,
            TeamCopyFieldStatus fieldCurrentStatus)
        {
            //状态相同，没有变化
            if (fieldPreStatus == fieldCurrentStatus)
                return false;

            //先前的状态不为解锁中
            if (fieldPreStatus != TeamCopyFieldStatus.TEAM_COPY_FIELD_STATUS_UNLOCKING)
                return false;
            
            //以前状态为Unlocking，当前的状态改变了
            return true;
        }

        //更新小队目标和团本目标
        public static void UpdateTeamDuplicationFightGoalDataModel(
            TeamDuplicationFightGoalDataModel teamDuplicationFightGoalDataModel,
            TeamCopyTarget teamCopyTarget)
        {
            if (teamDuplicationFightGoalDataModel == null || teamCopyTarget == null)
                return;

            teamDuplicationFightGoalDataModel.FightGoalId = (int)teamCopyTarget.targetId;

            teamDuplicationFightGoalDataModel.FightGoalDetailDataModelList.Clear();
            for (var i = 0; i < teamCopyTarget.targetDetailList.Length; i++)
            {
                var teamCopyTargetDetail = teamCopyTarget.targetDetailList[i];
                if (teamCopyTargetDetail != null)
                {
                    var fightGoalDetailDataModel = new TeamDuplicationFightGoalDetailDataModel();
                    fightGoalDetailDataModel.FightPointId = (int) teamCopyTargetDetail.fieldId;
                    fightGoalDetailDataModel.FightPointCurrentNumber = (int) teamCopyTargetDetail.curNum;
                    fightGoalDetailDataModel.FightPointTotalNumber = (int) teamCopyTargetDetail.totalNum;
                    teamDuplicationFightGoalDataModel.FightGoalDetailDataModelList.Add(fightGoalDetailDataModel);
                }
            }
        }


        //阶段的描述
        public static string GetTeamDuplicationStageDescription(int stageId)
        {
            var fightStageTable = TableManager.GetInstance().GetTableItem<TeamCopyStageTable>(stageId);

            if (fightStageTable != null)
                return fightStageTable.Description;

            return null;
        }

        public static string GetTeamDuplicationStageNamePath(int stageId)
        {
            var fightStageTable = TableManager.GetInstance().GetTableItem<TeamCopyStageTable>(stageId);

            if (fightStageTable != null)
                return fightStageTable.StageImagePath;

            return null;
        }

        //判断当前的据点是否在据点列表中
        public static bool IsSelectFightPointInFightPointList(int fightPointId)
        {
            var fightPointDataModelList =
                TeamDuplicationDataManager.GetInstance().TeamDuplicationFightPointDataModelList;

            if (fightPointDataModelList == null || fightPointDataModelList.Count <= 0)
                return false;

            for (var i = 0; i < fightPointDataModelList.Count; i++)
            {
                var curFightPointDataModel = fightPointDataModelList[i];
                if(curFightPointDataModel == null)
                    continue;

                if (curFightPointDataModel.FightPointId != fightPointId)
                    continue;

                if (IsFightPointShowInFightPanel(curFightPointDataModel) == true)
                    return true;
            }
            return false;
        }

        //初始化的时候，获得默认选中的小队目标据点
        public static TeamDuplicationFightPointDataModel GetDefaultSelectedFightPointDataModel()
        {
            var defaultSelectedFightPointDataModel = GetFirstShowFightPointDataModel();
            //不存在小队目标
            if (IsCaptainGoalExist() == false)
                return defaultSelectedFightPointDataModel;

            var fightGoalDetailDataModelList = TeamDuplicationDataManager.GetInstance()
                .TeamDuplicationCaptainFightGoalDataModel
                .FightGoalDetailDataModelList;
            
            //小队目标据点不存在
            if (fightGoalDetailDataModelList == null || fightGoalDetailDataModelList.Count <= 0)
                return defaultSelectedFightPointDataModel;

            var totalFightPointDataModelList =
                TeamDuplicationDataManager.GetInstance().TeamDuplicationFightPointDataModelList;

            if (totalFightPointDataModelList == null || totalFightPointDataModelList.Count <= 0)
                return defaultSelectedFightPointDataModel;

            for (var i = 0; i < fightGoalDetailDataModelList.Count; i++)
            {
                //小队目标的据点
                var fightGoalDetailDataModel = fightGoalDetailDataModelList[i];
                if (fightGoalDetailDataModel == null)
                    continue;

                //当前拥有的据点
                for (var j = 0; j < totalFightPointDataModelList.Count; j++)
                {
                    var curFightPointDataModel = totalFightPointDataModelList[j];
                    if(curFightPointDataModel == null)
                        continue;
                    //默认选中的据点
                    if (fightGoalDetailDataModel.FightPointId != curFightPointDataModel.FightPointId)
                        continue;

                    //找到目标据点
                    if (IsFightPointShowInFightPanel(curFightPointDataModel) == true)
                        return curFightPointDataModel;
                }
            }

            return defaultSelectedFightPointDataModel;
        }

        private static TeamDuplicationFightPointDataModel GetFirstShowFightPointDataModel()
        {
            var totalFightPointDataModelList =
                TeamDuplicationDataManager.GetInstance().TeamDuplicationFightPointDataModelList;
            if (totalFightPointDataModelList == null || totalFightPointDataModelList.Count <= 0)
                return null;

            for (var i = 0; i < totalFightPointDataModelList.Count; i++)
            {
                var curFightPointDataModel = totalFightPointDataModelList[i];
                if(curFightPointDataModel == null)
                    continue;

                if (IsFightPointShowInFightPanel(curFightPointDataModel) == true)
                    return curFightPointDataModel;
            }

            return null;
        }

        //判断据点是否为小队目标
        public static bool IsFightPointInCaptainGoal(int fightPointId)
        {
            if (IsCaptainGoalExist() == false)
                return false;

            var fightGoalDetailDataModelList = TeamDuplicationDataManager.GetInstance()
                .TeamDuplicationCaptainFightGoalDataModel
                .FightGoalDetailDataModelList;

            if (fightGoalDetailDataModelList == null || fightGoalDetailDataModelList.Count <= 0)
                return false;

            for (var i = 0; i < fightGoalDetailDataModelList.Count; i++)
            {
                var fightGoalDetailDataModel = fightGoalDetailDataModelList[i];
                if(fightGoalDetailDataModel == null)
                    continue;

                if (fightGoalDetailDataModel.FightPointId == fightPointId)
                    return true;
            }

            return false;
        }

        //判断小队目标是否存在
        public static bool IsCaptainGoalExist()
        {
            if (TeamDuplicationDataManager.GetInstance().TeamDuplicationCaptainFightGoalDataModel == null)
                return false;

            if (TeamDuplicationDataManager.GetInstance().TeamDuplicationCaptainFightGoalDataModel.FightGoalId <= 0)
                return false;

            return true;
        }

        //判断团本目标是否存在
        public static bool IsTeamDuplicationGoalExit()
        {
            if (TeamDuplicationDataManager.GetInstance().TeamDuplicationTeamFightGoalDataModel == null)
                return false;

            if (TeamDuplicationDataManager.GetInstance().TeamDuplicationTeamFightGoalDataModel.FightGoalId <= 0)
                return false;
            return true;
        }

        //攻坚面板目标的页签
        public static List<ComControlData> GetFightGoalDataModelList(TeamDuplicationFightPointDataModel fightPointDataModel = null)
        {
            List<ComControlData> goalDataList = new List<ComControlData>();

            //据点
            if (fightPointDataModel != null)
            {
                var fightPointGoal = new ComControlData
                {
                    Id = (int)TeamDuplicationFightGoalType.FightPointDescription,
                    Name = TR.Value("team_duplication_fight_point_goal_Label"),
                    Index = fightPointDataModel.FightPointId,
                };
                goalDataList.Add(fightPointGoal);
            }

            //团队
            if (IsTeamDuplicationGoalExit() == true)
            {
                var teamDuplicationGoal = new ComControlData
                {
                    Id = (int)TeamDuplicationFightGoalType.TeamDuplicationGoal,
                    Name = TR.Value("team_duplication_fight_team_duplication_goal_Label"),
                    Index = TeamDuplicationDataManager.GetInstance().TeamDuplicationTeamFightGoalDataModel.FightGoalId,
                };
                goalDataList.Add(teamDuplicationGoal);
            }

            //小队
            if (IsCaptainGoalExist() == true)
            {
                var captainGoalData = new ComControlData
                {
                    Id = (int)TeamDuplicationFightGoalType.CaptainGoal,
                    Name = TR.Value("team_duplication_fight_captain_goal_Label"),
                    Index = TeamDuplicationDataManager.GetInstance().TeamDuplicationCaptainFightGoalDataModel.FightGoalId,
                };
                goalDataList.Add(captainGoalData);
            }
            
            return goalDataList;
        }
       
        //展示单独一个据点解锁的飘字
        public static void ShowUnLockFightPointNameTips(int fightPointId)
        {
            var curFightPointTable = TableManager.GetInstance().GetTableItem<TeamCopyFieldTable>(fightPointId);
            if (curFightPointTable == null)
                return;

            //不显示
            if (curFightPointTable.UnLockTip == 0)
                return;

            var unLockFightPointName = curFightPointTable.Name;

            if (string.IsNullOrEmpty(unLockFightPointName) == true)
                return;


            var showContentStr = TR.Value("team_duplication_fight_point_unlock", unLockFightPointName);

            SystemNotifyManager.SysNotifyFloatingEffect(showContentStr);
        }


        //展示据点解锁的飘字
        public static void ShowUnLockFightPointListTips(List<string> unLockFightPointNameList)
        {
            if (unLockFightPointNameList == null || unLockFightPointNameList.Count <= 0)
                return;

            var unLockFightPointNameCount = unLockFightPointNameList.Count;

            var showNameStr = "";
            for (var i = 0; i < unLockFightPointNameCount; i++)
            {
                var curFightPointName = unLockFightPointNameList[i];
                showNameStr = showNameStr + curFightPointName;
                if (i < unLockFightPointNameCount - 1)
                    showNameStr = showNameStr + " ";
            }

            var showContentStr = TR.Value("team_duplication_fight_point_unlock", showNameStr);

            SystemNotifyManager.SysNotifyFloatingEffect(showContentStr);
        }


        //得到新解锁据点的名字
        public static void GetUnLockFightPointName(int fightPointId, List<string> unLockFightPointNameList)
        {
            if (unLockFightPointNameList == null)
                return;

            var curFightPointTable = TableManager.GetInstance().GetTableItem<TeamCopyFieldTable>(fightPointId);
            if (curFightPointTable == null)
                return;

            //不显示
            if (curFightPointTable.UnLockTip == 0)
                return;

            unLockFightPointNameList.Add(curFightPointTable.Name);
        }

        //小队目标和团本目标的描述
        public static string GetFightGoalDescription(TeamDuplicationFightGoalDataModel fightGoalDataModel)
        {
            if (fightGoalDataModel == null)
                return "";

            var captainGoalTable = TableManager.GetInstance().GetTableItem<TeamCopyTargetTable>(fightGoalDataModel.FightGoalId);
            if (captainGoalTable != null)
            {
                var captainDescription = captainGoalTable.Description;
                var subDescriptionStr = "";
                var captainSubDescriptionFormat = captainGoalTable.SubDescription;

                var fightGoalDetailDataModelList = fightGoalDataModel.FightGoalDetailDataModelList;

                if (fightGoalDetailDataModelList != null)
                {
                    for (var i = 0; i < fightGoalDetailDataModelList.Count; i++)
                    {
                        var fightGoalDetailDataModel = fightGoalDetailDataModelList[i];
                        if (fightGoalDetailDataModel == null)
                            continue;

                        var curFightPointTable = TableManager.GetInstance().GetTableItem<TeamCopyFieldTable>(fightGoalDetailDataModel.FightPointId);
                        if (curFightPointTable == null)
                            continue;

                        var fightGoalDetailDescriptionStr = string.Format(
                            captainSubDescriptionFormat,
                            curFightPointTable.Name,
                            fightGoalDetailDataModel.FightPointCurrentNumber,
                            fightGoalDetailDataModel.FightPointTotalNumber);

                        subDescriptionStr = subDescriptionStr + fightGoalDetailDescriptionStr;
                        if (i < fightGoalDetailDataModelList.Count - 1)
                            subDescriptionStr = subDescriptionStr + ",";
                    }
                }

                captainDescription = string.Format(captainDescription, subDescriptionStr);
                return captainDescription;
            }

            return "";
        }

        //能量蓄积据点相关的数据
        public static TeamDuplicationFightPointEnergyAccumulationDataModel GetEnergyAccumulationFightPointDataModel()
        {
            TeamDuplicationFightPointEnergyAccumulationDataModel fightPointEnergyAccumulationDataModel =
                new TeamDuplicationFightPointEnergyAccumulationDataModel();

            fightPointEnergyAccumulationDataModel.TimeUpdateInterval = 0.0f;

            #region TeamCopyValueTable
            //从表中读取配置的数值
            var energyAccumulation5ValueTable = TableManager.GetInstance().GetTableItem<TeamCopyValueTable>(39);
            if (energyAccumulation5ValueTable != null)
                fightPointEnergyAccumulationDataModel.FightPointEnergyAccumulation5 =
                    energyAccumulation5ValueTable.Value;

            var energyAccumulation30ValueTable = TableManager.GetInstance().GetTableItem<TeamCopyValueTable>(100);
            if (energyAccumulation30ValueTable != null)
                fightPointEnergyAccumulationDataModel.FightPointEnergyAccumulation30 =
                    energyAccumulation30ValueTable.Value;

            var energyAccumulation50ValueTable = TableManager.GetInstance().GetTableItem<TeamCopyValueTable>(101);
            if (energyAccumulation50ValueTable != null)
                fightPointEnergyAccumulationDataModel.FightPointEnergyAccumulation50 =
                    energyAccumulation50ValueTable.Value;

            var energyAccumulation80ValueTable = TableManager.GetInstance().GetTableItem<TeamCopyValueTable>(102);
            if (energyAccumulation80ValueTable != null)
                fightPointEnergyAccumulationDataModel.FightPointEnergyAccumulation80 =
                    energyAccumulation80ValueTable.Value;

            var energyAccumulation100ValueTable = TableManager.GetInstance().GetTableItem<TeamCopyValueTable>(103);
            if (energyAccumulation100ValueTable != null)
                fightPointEnergyAccumulationDataModel.FightPointEnergyAccumulation100 =
                    energyAccumulation100ValueTable.Value;
            #endregion

            return fightPointEnergyAccumulationDataModel;
        }


        #endregion

        #region TeamDuplicationFightStageReward

        //更新阶段奖励的数据模型
        public static void UpdateFightStageRewardDataModel(TeamCopyFlop teamCopyFlop,
            TeamDuplicationFightStageRewardDataModel fightStageRewardDataModel)
        {
            if (teamCopyFlop == null || fightStageRewardDataModel == null)
                return;

            fightStageRewardDataModel.PlayerGuid = teamCopyFlop.playerId;
            fightStageRewardDataModel.PlayerName = teamCopyFlop.playerName;
            fightStageRewardDataModel.RewardId = (int) teamCopyFlop.rewardId;
            fightStageRewardDataModel.RewardNum = (int) teamCopyFlop.rewardNum;
            fightStageRewardDataModel.RewardIndex = (int) teamCopyFlop.number;
            fightStageRewardDataModel.IsGoldReward = teamCopyFlop.goldFlop == 1;
            fightStageRewardDataModel.IsLimit = (TeamCopyFlopLimit)teamCopyFlop.isLimit;
        }

        //得到奖励限制的描述
        public static string GetStageLimitDescriptionByLimitType(TeamCopyFlopLimit flopLimit)
        {
            var limitDescription = "";

            
            if (flopLimit == TeamCopyFlopLimit.TEAM_COPY_FLOP_LIMIT_WEEK)
            {
                limitDescription = TR.Value("team_duplication_fight_stage_reward_week_zero");
            }
            else if (flopLimit == TeamCopyFlopLimit.TEAM_COPY_FLOP_LIMIT_DAY)
            {
                limitDescription = TR.Value("team_duplication_fight_stage_reward_day_zero");
            }
            else if (flopLimit == TeamCopyFlopLimit.TEAM_COPY_FLOP_LIMIT_PASS_GATE)
            {
                limitDescription = TR.Value("team_duplication_fight_stage_reward_without_pass");
            }

            return limitDescription;
        }

        #endregion

        #region TeamDuplicationChatContent

        public static void OnSendTeamRecru()
        {

            //TeamCopyRecruitReq
            var teamDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (teamDataModel == null)
            {
                Logger.LogErrorFormat("TeamDataModel is null");
                return;
            }

            var contentStr = TR.Value("team_duplication_quick_chat_challenge_type_content");
            if (teamDataModel.TeamModel == (int)TeamCopyTeamModel.TEAM_COPY_TEAM_MODEL_GOLD)
                contentStr = TR.Value("team_duplication_quick_chat_gold_type_content");

            ChatManager.GetInstance().SendChat(ChatType.CT_TEAM_COPY_PREPARE, contentStr);
        }

        //聊天气泡
        public static TeamDuplicationChatBubbleViewControl LoadTeamDuplicationChatBubbleViewControl(GameObject contentRoot)
        {
            if (contentRoot == null)
                return null;

            TeamDuplicationChatBubbleViewControl chatBubbleViewControl = null;

            var uiPrefabWrapper = contentRoot.GetComponent<UIPrefabWrapper>();
            if (uiPrefabWrapper != null)
            {
                var uiPrefab = uiPrefabWrapper.LoadUIPrefab();
                if (uiPrefab != null)
                {
                    uiPrefab.transform.SetParent(contentRoot.transform, false);
                    chatBubbleViewControl = uiPrefab.GetComponent<TeamDuplicationChatBubbleViewControl>();
                }
            }

            return chatBubbleViewControl;
        }

        //private static int index = 0;
        //private static ulong playerGuid = 904115217381097130;

        //public static void OnSendChatContent()
        //{
        //    index = index + 1;
        //    var chatContent = "聊天记录 --" + index.ToString() + "__" + index.ToString();
        //    //TeamDuplicationUtility.OnOpenTeamDuplicationSkillFrame();

        //    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveTeamDuplicationChatContentMessage,
        //        playerGuid, chatContent);
        //}

        #endregion

        #region TeamDuplicationQuitTeam

        //是否开战后每天免费退出团本
        public static bool IsPlayerCanFreeQuitTeam()
        {
            var playerInformationDataModel = TeamDuplicationDataManager.GetInstance().GetPlayerInformationDataModel();
            if (playerInformationDataModel != null && playerInformationDataModel.DayFreeQuitNumber > 0)
                return true;

            return false;
        }

        #endregion 

        #region Helper

		//职业名字，不包括专职后的名字
        public static string GetJobName(int jobId, int awakeState = 0)
        {
            var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(jobId);
            if (jobTable == null)
                return "";

            return jobTable.Name;
        }

        public static void OnQuitTeamAndReturnToTown(string tipContent, Action onQuitAndReturn)
        {
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = tipContent,
                IsShowNotify = false,
                LeftButtonText = TR.Value("common_data_cancel"),
                RightButtonText = TR.Value("common_data_sure"),
                OnRightButtonClickCallBack = onQuitAndReturn,
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }

        //排序请求者列表
        public static void SortTeamDuplicationRequesterDataModelList(
            List<TeamDuplicationRequesterDataModel> requesterDataModelList)
        {
            if (requesterDataModelList == null || requesterDataModelList.Count <= 0)
                return;

            //1：金主，2：好友，3：工会，4：其他
            requesterDataModelList.Sort((x, y) =>
            {

                var a = -x.IsGold.CompareTo(y.IsGold);
                if (a == 0)
                    a = -x.IsFriend.CompareTo(y.IsFriend);
                if (a == 0)
                    a = -x.IsGuild.CompareTo(y.IsGuild);

                return a;
            });
        }

        public static bool IsShowTeamDuplication()
        {
            //功能没有解锁
            if (Utility.IsUnLockFunc((int) FunctionUnLock.eFuncType.TeamCopy) == false)
                return false;

            //没有打开
            if (ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(ServiceType.SERVICE_TEAM_COPY) ==
                false)
                return false;
            
            //显示团本按钮
            return true;
        }

        //服务器的开关是否打开
        public static bool IsTeamDuplicationServerSwitchOpen()
        {
            //服务器的开关没有打开
            if (ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(ServiceType.SERVICE_TEAM_COPY) ==
                false)
                return false;

            return true;
        }

        public static void OnShowCommonMsgBoxFrame(string contentStr, Action onOkAction,
            TextAnchor contentTextAnchor = TextAnchor.MiddleCenter)
        {
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = contentStr,
                ContentTextAnchor = contentTextAnchor,
                IsShowNotify = false,
                LeftButtonText = TR.Value("common_data_cancel"),
                RightButtonText = TR.Value("common_data_sure_2"),
                OnRightButtonClickCallBack = onOkAction,
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }

        //角色在团本中是否拥有团队
        public static bool IsTeamDuplicationOwnerTeam()
        {
            //不在团本场景中，直接返回false
            if (IsInTeamDuplicationScene() == false)
                return false;

            return TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerTeam;
        }

        //角色是否在团本场景中
        public static bool IsInTeamDuplicationScene()
        {
            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;

            if (systemTown == null)
                return false;

            if (systemTown.CurrentSceneID != TeamDuplicationDataManager.GetInstance().TeamDuplicationBuildSceneId
                && systemTown.CurrentSceneID != TeamDuplicationDataManager.GetInstance().TeamDuplicationFightSceneId)
                return false;

            return true;
        }

        // 点击团本队伍链接回调函数(点击加入)
        public static void AcceptJoinTeamDuplicationLink(string param)
        {
            var tokens = param.Split('|');
            if (tokens.Length != 2)
                return;

            var teamRoomId = 0;

            if (int.TryParse(tokens[0], out teamRoomId) == false)
                return;

            if (teamRoomId <= 0)
                return;

			//不在团本场景
            if (IsInTeamDuplicationScene() == false)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_switch_to_team_duplication"));
                return;
            }

            if (TeamDuplicationDataManager.GetInstance().IsTeamDuplicationOwnerTeam == true)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("team_duplication_cannot_join_in_for_owner_team"));
                return;
            }

            TeamDuplicationDataManager.GetInstance().OnSendTeamCopyLinkJoinReq(teamRoomId);

        }


        public static void OnShowLeaveTeamTipFrame(string contentStr, Action onOkAction)
        {
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = contentStr,
                IsShowNotify = false,
                LeftButtonText = TR.Value("common_data_cancel"),
                RightButtonText = TR.Value("common_data_sure_2"),
                OnRightButtonClickCallBack = onOkAction,
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }
        #endregion

        #region PlayerExpireTime

        //得到角色的掉线时间
        public static ulong GetPlayerExpireTime(ulong playerGuid)
        {
            //return TimeManager.GetInstance().GetServerTime() + 50;

            if (TeamDuplicationDataManager.GetInstance().TeamDuplicationPlayerExpireTimeDic == null
                || TeamDuplicationDataManager.GetInstance().TeamDuplicationPlayerExpireTimeDic.Count <= 0)
                return 0;

            ulong expireTime = 0;
            if (TeamDuplicationDataManager.GetInstance().TeamDuplicationPlayerExpireTimeDic.ContainsKey(playerGuid) ==
                true)
            {
                expireTime = TeamDuplicationDataManager.GetInstance().TeamDuplicationPlayerExpireTimeDic[playerGuid];
            }

            return expireTime;
        }

        #endregion

        #region PlayerInformation

        //判断角色的门票是否足够
        public static bool IsPlayerTicketIsEnough()
        {
            var playerInformationDataModel = TeamDuplicationDataManager.GetInstance().GetPlayerInformationDataModel();
            if (playerInformationDataModel == null)
                return false;

            return playerInformationDataModel.TicketIsEnough;
        }

        //角色是否可以继续挑战团本，今天次数没有达到上限，以及本周次数没有达到上限
        public static bool IsPlayerCanDoTeamDuplication()
        {
            var playerInformationDataModel = TeamDuplicationDataManager.GetInstance().GetPlayerInformationDataModel();
            if (playerInformationDataModel == null)
                return false;

            if (playerInformationDataModel.DayAlreadyFightNumber >= playerInformationDataModel.DayTotalFightNumber)
                return false;

            if (playerInformationDataModel.WeekAlreadyFightNumber >= playerInformationDataModel.WeekTotalFightNumber)
                return false;

            return true;
        }

        #endregion


    }
}