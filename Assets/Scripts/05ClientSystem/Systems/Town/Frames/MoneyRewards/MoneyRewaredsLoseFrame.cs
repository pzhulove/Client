using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class MoneyRewardsLoseFrame : ClientFrame
    {
        [UIControl("", typeof(StateController))]
        StateController comState;

        [UIControl("Score/Total")]
        Text m_labTotalScore;

        [UIControl("Score/Delta")]
        Text m_labDeltaScore;

        [UIControl("Score/Glory")]
        Text m_labGlory;
        [UIControl("Title/BG/Text")]
        Text m_labTitle;

        [UIObject("Tie")]
        GameObject m_objTie;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MoneyRewards/MoneyRewardsLose";
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

            if (null != comState)
            {
                switch (MoneyRewardsDataManager.GetInstance().eMoneyRewardsStatus)
                {
                    case MoneyRewardsStatus.MRS_INVALID:
                    case MoneyRewardsStatus.MRS_READY:
                    case MoneyRewardsStatus.MRS_8_RACE:
                    case MoneyRewardsStatus.MRS_PRE_4_RACE:
                    case MoneyRewardsStatus.MRS_END:
                        {
                            comState.Key = "need_score";
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

            if (data.result == 2)
            {
                m_labTitle.text = TR.Value("guild_battle_lose");
                m_objTie.SetActive(false);
            }
            else if (data.result == 4)
            {
                m_labTitle.text = TR.Value("guild_battle_lose");
                m_objTie.SetActive(true);
            }
            else
            {
                m_labTitle.text = TR.Value("guild_battle_result_error");
                m_objTie.SetActive(false);
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
