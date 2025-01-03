using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class GuildChangeDutyData
    {
        public ulong uMemberGUID;
        public EGuildDuty eDuty;
    }

    class GuildChangeDutyFrame : ClientFrame
    {
        [UIObject("Content/List/Viewport/Content")]
        GameObject m_objMemberRoot;

        [UIObject("Content/List/Viewport/Content/Template")]
        GameObject m_objMemberTemplate;

        ulong m_uSelectMemberID = 0;
        GuildChangeDutyData m_changeData = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildChangeDuty";
        }

        protected override void _OnOpenFrame()
        {
            m_changeData = (GuildChangeDutyData)userData;
            if (m_changeData == null)
            {
                Logger.LogErrorFormat("open guild change duty frame, need userData");
                return;
            }

            m_uSelectMemberID = 0;
            m_objMemberTemplate.SetActive(false);
            List<GuildMemberData> arrMembers = GuildDataManager.GetInstance().GetMembersByDuty(m_changeData.eDuty);
            for (int i = 0; i < arrMembers.Count; ++i)
            {
                GameObject obj = GameObject.Instantiate(m_objMemberTemplate);
                obj.transform.SetParent(m_objMemberRoot.transform, false);
                obj.SetActive(true);

                Utility.GetComponetInChild<Text>(obj, "Name/Text").text = arrMembers[i].strName;
                Utility.GetComponetInChild<Text>(obj, "Duty/Text").text = TR.Value(arrMembers[i].eGuildDuty.GetDescription());
                Toggle toggle = Utility.GetComponetInChild<Toggle>(obj, "Oper/Check");

                ulong id = arrMembers[i].uGUID;
                if (i == 0)
                {
                    toggle.isOn = true;
                    m_uSelectMemberID = id;
                }
                else
                {
                    toggle.isOn = false;
                }
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener((bool a_bChecked) =>
                {
                    if (a_bChecked)
                    {
                        m_uSelectMemberID = id;
                    }
                });
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
        }

        protected override void _OnCloseFrame()
        {
            m_uSelectMemberID = 0;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
        }

        void _OnGuildDismissed(UIEvent a_event)
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Ok")]
        void _OnOkClicked()
        {
            if (m_uSelectMemberID > 0)
            {
                GuildDataManager.GetInstance().ChangeMemberDuty(m_changeData.uMemberGUID, m_changeData.eDuty, m_uSelectMemberID);
                frameMgr.CloseFrame(this);
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_replace_duty_need_select_one"));
            }
        }

        [UIEventHandle("Cancel")]
        void _OnCancelClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
