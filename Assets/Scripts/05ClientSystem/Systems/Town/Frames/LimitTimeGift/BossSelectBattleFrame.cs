using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections;

namespace GameClient
{
    
    class BossSelectBattleFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/LimitTimeGift/BossSelectBattle";
        }
        protected override void _OnOpenFrame()
        {
            InitButtonState();
        }

        private void InitButtonState()
        {
            //var functionUnlockData = TableManager.GetInstance().GetTable<FunctionUnLock>();
            //var enumerator = functionUnlockData.GetEnumerator();
            //while(enumerator.MoveNext())
            //{
            //    FunctionUnLock FunctionUnLockItem = enumerator.Current.Value as FunctionUnLock;
            //    if(FunctionUnLockItem.ID == )
            //}
            var functionUnlockDataAbyss = TableManager.GetInstance().GetTableItem<FunctionUnLock>(23);
            var functionUnlockDataAncient = TableManager.GetInstance().GetTableItem<FunctionUnLock>(24);
            if(functionUnlockDataAbyss == null)
            {
                Logger.LogErrorFormat("please check FunctionUnlockTable ID :{0}", 23);
            }
            if(functionUnlockDataAncient == null)
            {
                Logger.LogErrorFormat("please check FunctionUnlockTable ID :{0}", 24);
            }
            if(functionUnlockDataAbyss != null)
            {
                if(PlayerBaseData.GetInstance().Level>=functionUnlockDataAbyss.FinishLevel)
                {
                    //mGoAbyss.CustomActive(true);
                    mGoAbyss.enabled = true;
                    mGoAbyssText.text = "前往深渊地下城";
                }
                else
                {
                    //mGoAbyss.CustomActive(false);
                    mGoAbyss.enabled = false;
                    mGoAbyssText.text = string.Format("前往深渊地下城（{0}级解锁）", functionUnlockDataAbyss.FinishLevel);
                }

                if(PlayerBaseData.GetInstance().Level >= functionUnlockDataAncient.FinishLevel)
                {
                    //mGoAncient.CustomActive(true);
                    mGoAncient.enabled = true;
                    mGoAncientText.text = "前往远古地下城";
                }
                else
                {
                    //mGoAncient.CustomActive(false);
                    mGoAncient.enabled = false;
                    mGoAncientText.text = string.Format("前往远古地下城（{0}级解锁）", functionUnlockDataAncient.FinishLevel);
                }
            }
        }

        private void TryCloseBossActivityFrame()
        {
            //if(ClientSystemManager.GetInstance().IsFrameOpen<ActivityCombineFrame>())
            //{
            //    ClientSystemManager.GetInstance().CloseFrame<ActivityCombineFrame>();
            //}
        }
        protected override void _OnCloseFrame()
        {
            frameMgr.CloseFrame(this);
        }

        #region ExtraUIBind
        private Button mGoAncient = null;
        private Button mGoAbyss = null;
        private Text mGoAbyssText = null;
        private Text mGoAncientText = null;
        private Button mGoDungeon = null;
        private Button mClose = null;

        protected override void _bindExUI()
        {
            mGoAncient = mBind.GetCom<Button>("GoAncient");
            mGoAncient.onClick.AddListener(_onGoAncientButtonClick);
            mGoAbyss = mBind.GetCom<Button>("GoAbyss");
            mGoAbyss.onClick.AddListener(_onGoAbyssButtonClick);
            mGoAbyssText = mBind.GetCom<Text>("GoAbyssText");
            mGoAncientText = mBind.GetCom<Text>("GoAncientText");
            mGoDungeon = mBind.GetCom<Button>("GoDungeon");
            mGoDungeon.onClick.AddListener(_onGoDungeonButtonClick);
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
        }

        protected override void _unbindExUI()
        {
            mGoAncient.onClick.RemoveListener(_onGoAncientButtonClick);
            mGoAncient = null;
            mGoAbyss.onClick.RemoveListener(_onGoAbyssButtonClick);
            mGoAbyss = null;
            mGoAbyssText = null;
            mGoAncientText = null;
            mGoDungeon.onClick.RemoveListener(_onGoDungeonButtonClick);
            mGoDungeon = null;
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
        }
        #endregion

        #region Callback
        private void _onGoAncientButtonClick()
        {
            //ActivityDungeonFrame.OpenLinkFrame("602000");
            var paramDataModel = new ChallengeMapParamDataModel()
            {
                ModelType = ProtoTable.DungeonModelTable.eType.AncientModel,
                // BaseDungeonId = 602000,
                BaseDungeonId = 611000,
            };

            ClientSystemManager.GetInstance().OpenFrame<ChallengeMapFrame>(FrameLayer.Middle, paramDataModel);

            frameMgr.CloseFrame(this);
            TryCloseBossActivityFrame();

        }
        private void _onGoAbyssButtonClick()
        {
            //ActivityDungeonFrame.OpenLinkFrame("702000");
            var paramDataModel = new ChallengeMapParamDataModel()
            {
                ModelType = ProtoTable.DungeonModelTable.eType.DeepModel,
                BaseDungeonId = 701000,
            };

            ClientSystemManager.GetInstance().OpenFrame<ChallengeMapFrame>(FrameLayer.Middle, paramDataModel);
            frameMgr.CloseFrame(this);
            TryCloseBossActivityFrame();
        }
        private void _onGoDungeonButtonClick()
        {
            /* put your code in here */
            int DungenID = ChapterUtility.GetLastedDungeonIDByDiff(0);
            GameClient.ClientSystemTown clientSystem = GameClient.ClientSystemManager.GetInstance().CurrentSystem as GameClient.ClientSystemTown;
            clientSystem.MainPlayer.CommandMoveToDungeon(DungenID);
            //ClientSystemManager.GetInstance().CloseFrame<BossActivityFrame>();
            frameMgr.CloseFrame(this);
            TryCloseBossActivityFrame();
        }

        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}