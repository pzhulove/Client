using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class GuildBattleResultFrame : ClientFrame
    {
        [UIObject("Content/ScrollView/Viewport/Content/Template")]
        GameObject m_objTemplate;

        [UIObject("Content/ScrollView/Viewport/Content")]
        GameObject m_objRoot;


        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildBattleResult";
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
            m_objTemplate.SetActive(false);

            GuildBattleEndInfo[] arrInfos = userData as GuildBattleEndInfo[];
            if (arrInfos != null)
            {
                for (int i = 0; i < arrInfos.Length; ++i)
                {
                    GameObject obj = GameObject.Instantiate(m_objTemplate);
                    obj.transform.SetParent(m_objRoot.transform, false);
                    obj.SetActive(true);

                    GuildBattleEndInfo info = arrInfos[i];
                    Utility.GetComponetInChild<Text>(obj, "Text0").text = info.terrName;
                    Utility.GetComponetInChild<Text>(obj, "Text1").text = string.IsNullOrEmpty(info.guildName) ? "-" : info.guildName;
                    Utility.GetComponetInChild<Text>(obj, "Text2").text = string.IsNullOrEmpty(info.guildLeaderName) ? "-" : info.guildLeaderName;
                }
            }
        }

        void _ClearUI()
        {

        }

        [UIEventHandle("Content/Close")]
        void _OnCloseClicked()
        {
            ClientSystemManager.instance.CloseFrame(this);
        }

        [UIEventHandle("Content/JumpToMail")]
        void _OnJumpToMailClicked()
        {
            ClientSystemManager.instance.OpenFrame<MailNewFrame>();
        }
    }
}
