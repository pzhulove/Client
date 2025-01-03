using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ProtoTable;

namespace GameClient
{
    public class SettlementInfo
    {
        public UInt32 rank;
        public UInt32 playerNum;
        public UInt32 kills;
        public UInt32 survivalTime;
        public UInt32 score;
        public UInt32 glory;

        public void ClearData()
        {
            rank = 0;
            playerNum = 0;
            kills = 0;
            survivalTime = 0;
            score = 0;
            glory = 0;
        }
    }

    public class ChijiSettlementFrame : ClientFrame
    {
        private bool bAutoQuitChiji = false;
        private float AutoQuitChijiTime = 0.0f;

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiSettlementFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            SettlementInfo settlementInfo = ChijiDataManager.GetInstance().Settlementinfo;

            if (settlementInfo != null)
            {
                InitInterface(settlementInfo);
            }

            bAutoQuitChiji = true;
            AutoQuitChijiTime = 0.0f;

            _bindUIEvent();
        }

        protected sealed override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            _ClearData();

            _unBindUIEvent();
        }

        private void _bindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CounterChanged,_OnCounterChanged);
        }

        private void _unBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CounterChanged, _OnCounterChanged);
        }

        private void _OnCounterChanged(UIEvent ui)
        {
            SettlementInfo settlementInfo = ChijiDataManager.GetInstance().Settlementinfo;

            if (settlementInfo != null)
            {
                InitInterface(settlementInfo);
            }
        }

        private void _ClearData()
        {
            bAutoQuitChiji = false;
            AutoQuitChijiTime = 0.0f;
        }
 
        private void InitInterface(SettlementInfo settlementInfo)
        {
          
            if (mName != null)
            {
                mName.text = PlayerBaseData.GetInstance().Name;
            }

            if (mRank != null)
            {
                mRank.text = settlementInfo.rank.ToString();
            }

            if (mTotalRank != null)
            {
                mTotalRank.text = settlementInfo.playerNum.ToString();
            }

            if (mKillHeroCount != null)
            {
                mKillHeroCount.text = settlementInfo.kills.ToString();
            }

            if (mSurvivalTime != null)
            {
                mSurvivalTime.text = Function.GetLastsTimeStr(settlementInfo.survivalTime);
            }

            if (mIntegralCount != null)
            {
                mIntegralCount.text = settlementInfo.score.ToString();
            }
            if(mGloryCount != null)
            {
                mGloryCount.text = settlementInfo.glory.ToString();
            }
            if(mTotalGloryCount != null)
            {
                mTotalGloryCount.text = string.Format("{0}/{1}",ChijiDataManager.GetInstance()._GetWeeklyTotalGlory(), ChijiDataManager.GetInstance()._GetWeeklyMaxPVPGlory());
            }
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            // 无论吃鸡还是死亡一定时间后退出自动吃鸡
            if (bAutoQuitChiji)
            {
                AutoQuitChijiTime += timeElapsed;

                if (AutoQuitChijiTime > 15.0f)
                {
                    bAutoQuitChiji = false;
                    _onBtCloseButtonClick();
                }
            }
        }

        #region ExtraUIBind
        private Text mRank = null;
        private Text mTotalRank = null;
        private Text mKillHeroCount = null;
        private Text mSurvivalTime = null;
        private Text mIntegralCount = null;
        private Image mWinOrFail = null;
        private Button mBtClose = null;
        private Text mName = null;

        private Text mGloryCount = null;
        private Text mTotalGloryCount = null;
        protected sealed override void _bindExUI()
        {
            mRank = mBind.GetCom<Text>("Rank");
            mTotalRank = mBind.GetCom<Text>("TotalRank");
            mKillHeroCount = mBind.GetCom<Text>("KillHeroCount");
            mSurvivalTime = mBind.GetCom<Text>("SurvivalTime");
            mIntegralCount = mBind.GetCom<Text>("IntegralCount");
            mWinOrFail = mBind.GetCom<Image>("WinOrFail");
            mBtClose = mBind.GetCom<Button>("btClose");
            if (null != mBtClose)
            {
                mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            }
            mName = mBind.GetCom<Text>("Name");
            mGloryCount = mBind.GetCom<Text>("GloryCount");
            mTotalGloryCount = mBind.GetCom<Text>("TotalGloryCount");
        }

        protected sealed override void _unbindExUI()
        {
            mRank = null;
            mTotalRank = null;
            mKillHeroCount = null;
            mSurvivalTime = null;
            mIntegralCount = null;
            mWinOrFail = null;
            if (null != mBtClose)
            {
                mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            }
            mBtClose = null;
            mName = null;
            mGloryCount = null;
            mTotalGloryCount = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            ClientSystemGameBattle sysChiji = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if(sysChiji != null)
            {
                CitySceneTable tableData = TableManager.GetInstance().GetTableItem<CitySceneTable>(sysChiji.CurrentSceneID);
                if(tableData != null)
                {
                    if(tableData.SceneType == CitySceneTable.eSceneType.BATTLE)
                    {
                        GameFrameWork.instance.StartCoroutine(sysChiji._NetSyncChangeScene(
                                    new SceneParams
                                    {
                                        currSceneID = sysChiji.CurrentSceneID,
                                        currDoorID = 0,
                                        targetSceneID = 10101,
                                        targetDoorID = 0,
                                    }));

                        ChijiDataManager.GetInstance().SwitchingChijiSceneToPrepare = true;
                    }

                    frameMgr.CloseFrame(this);
                }
                else
                {
                    frameMgr.CloseFrame(this);
                }
            }
            else
            {
                frameMgr.CloseFrame(this);
            }
        }
        #endregion
    }
}