using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    class TalkTabFrame : ClientFrame
    {
        public static void Open()
        {
            OpenTargetFrame<TalkTabFrame>();
        }
        public override string GetPrefabPath()
        {
            return "UI/Prefabs/Talktab";
        }

        protected override void _OnOpenFrame()
        {
            _InitFilters();
        }

        protected override void _OnCloseFrame()
        {

        }

        [UIEventHandle("Close")]
        void _OnClickClose()
        {
            frameMgr.CloseFrame(this);
        }

        void _InitFilters()
        {
            string[] contents = new string[(int)ChatType.CT_MAX_WORDS]
            {
                "TabC",
                "",
                "",
                "TabB",
                "TabA",
                "TabD",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
            };

            for (int i = 0; i < contents.Length; ++i)
            {
                if (!string.IsNullOrEmpty(contents[i]))
                {
                    Toggle toggle = Utility.FindComponent<Toggle>(frame, contents[i]);
                    toggle.isOn = SystemConfigManager.GetInstance().IsChatToggleOn((ChatType)i);
                    ChatType eChatType = (ChatType)i;
                    GameObject goCheckMark = Utility.FindChild(toggle.gameObject, "CheckMark");
                    goCheckMark.CustomActive(toggle.isOn);
                    toggle.onValueChanged.AddListener((bool bValue) =>
                    {
                        SystemConfigManager.GetInstance().SetChatToggle(eChatType, bValue);
                        goCheckMark.CustomActive(bValue);
                    });
                }
            }
        }
    }
}