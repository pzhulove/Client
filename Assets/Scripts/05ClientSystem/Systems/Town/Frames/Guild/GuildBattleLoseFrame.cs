using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class GuildBattleLoseFrame : ClientFrame
    {

        [UIControl("Score/Total")]
        Text m_labTotalScore;

        [UIControl("Score/Delta")]
        Text m_labDeltaScore;

        [UIControl("Title/BG/Text")]
        Text m_labTitle;

        [UIObject("Tie")]
        GameObject m_objTie;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildBattleLose";
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
            WorldGuildBattleRaceEnd data = userData as WorldGuildBattleRaceEnd;
            m_labTotalScore.text = data.newScore.ToString();
            m_labDeltaScore.text = (data.newScore - data.oldScore).ToString();

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
