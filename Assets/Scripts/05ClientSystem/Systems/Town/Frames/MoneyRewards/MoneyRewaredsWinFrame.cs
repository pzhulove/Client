using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class MoneyRewardsWinFrame : ClientFrame
    {
        [UIControl("", typeof(StateController))]
        StateController comState;

        [UIControl("", typeof(ComMoneyRewardsVSWinSetting))]
        ComMoneyRewardsVSWinSetting comVsSetting;

        [UIControl("Score/Total")]
        Text m_labTotalScore;

        [UIControl("Score/Delta")]
        Text m_labDeltaScore;

        [UIControl("Score/Glory")]
        Text m_labGlory;
        [UIControl("Score/getStatus/d")]
        Text m_getScore;

        [UIControl("Score/fullStatus/d")]
        Text m_fullScore;

        [UIControl("Score", typeof(StateController))]
        StateController scoreStatus;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MoneyRewards/MoneyRewardsWin";
        }

        protected override void _OnOpenFrame()
        {
            _InitUI();
            _RegisterUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            _ClearUI();
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {

        }

        void _UnRegisterUIEvent()
        {

        }

        void _InitUI()
        {
            WorldPremiumLeagueRaceEnd data = userData as WorldPremiumLeagueRaceEnd;
            m_labTotalScore.text = data.newScore.ToString();
            m_labDeltaScore.text = (data.newScore - data.oldScore).ToString();
            m_labGlory.text = data.getHonor.ToString();

            if(null != comState)
            {
                switch(MoneyRewardsDataManager.GetInstance().eMoneyRewardsStatus)
                {
                    case MoneyRewardsStatus.MRS_INVALID:
                    case MoneyRewardsStatus.MRS_READY:
                    case MoneyRewardsStatus.MRS_8_RACE:
                    case MoneyRewardsStatus.MRS_PRE_4_RACE:
                    case MoneyRewardsStatus.MRS_END:
                        {
                            comState.Key = "need_score";
                            if(data.preliminayRewardNum > 0)
                            {
                                if(null != scoreStatus)
                                {
                                    scoreStatus.Key = "get";
                                }

                                if(null != m_getScore)
                                {
                                    if (null != comVsSetting)
                                    {
                                        m_getScore.text = string.Format(comVsSetting.FmtString0, data.preliminayRewardNum);
                                    }
                                }
                            }
                            else
                            {
                                if (null != scoreStatus)
                                {
                                    scoreStatus.Key = "full";
                                }

                                if(null != m_fullScore)
                                {
                                    if(null != comVsSetting)
                                    {
                                        m_fullScore.text = string.Format(comVsSetting.FmtString1, MoneyRewardsDataManager.GetInstance().MaxAwardEachVS);
                                    }
                                }
                            }
                        }
                        break;
                    case MoneyRewardsStatus.MRS_4_RACE:
                        {
                            comState.Key = "level4";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_2_RACE:
                        {
                            comState.Key = "level2";
                        }
                        break;
                    case MoneyRewardsStatus.MRS_RACE:
                        {
                            comState.Key = "level1";
                        }
                        break;
                }
            }
        }

        void _ClearUI()
        {
            
        }

        [UIEventHandle("Quit")]
        void _OnQuitClicked()
        {
            ClientSystemManager.instance.CloseFrame(this);
            ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
        }
    }
}
