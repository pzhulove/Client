using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class GuildBattleWinFrame : ClientFrame
    {
        [UIControl("Score/Total")]
        Text m_labTotalScore;

        [UIControl("Score/Delta")]
        Text m_labDeltaScore;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildBattleWin";
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
