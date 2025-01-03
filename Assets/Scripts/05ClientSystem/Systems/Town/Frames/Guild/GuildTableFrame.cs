using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using System.Collections;
using DG.Tweening;

namespace GameClient
{
    class GuildTableFrame :ClientFrame
    {
        [UIObject("First/Get")]
        GameObject m_objFirstGetItemRoot;

        [UIObject("Help/Get")]
        GameObject m_objHelpGetItemRoot;

        [UIControl("First/RemainTimes")]
        Text m_labFirstRemainTimes;

        [UIControl("Help/RemainTimes")]
        Text m_labHelpRemainTimes;

        [UIControl("Desc/Content")]
        Text m_labDecs;

        [UIControl("Table/BG/UIEffectParticle")]
        GeUIEffectParticle m_parCompleteEffect1;

        [UIControl("Table/BG/UIEffectParticle (1)")]
        GeUIEffectParticle m_parCompleteEffect2;

        [UIObject("ItemTemplate")]
        GameObject m_objGetTemplate;

        class PosInfo
        {
            public GuildTableMember data;
            public Button btnJoin;
            public ComButtonEnbale comJoinEnable;
            public Text labJoin;
            public Image imgIcon;
            public Text labName;
            public Text labLevel;
            public GameObject objHelp;
            public Vector3 vecIconPos;
            public Color colIconColor;
            public DOTweenAnimation[] dotweens;
        }
        List<PosInfo> m_arrPosInfos = new List<PosInfo>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildTable";
        }

        protected override void _OnOpenFrame()
        {
            m_objGetTemplate.SetActive(false);
            m_parCompleteEffect1.StopEmit();
            m_parCompleteEffect2.StopEmit();
            for (int i = 0; i < 7; ++i)
            {
                GameObject objRoot = Utility.FindGameObject(string.Format("Table/Pos{0}", i));
                if(objRoot != null)
                {
                    PosInfo info = new PosInfo();
                    info.btnJoin = Utility.GetComponetInChild<Button>(objRoot, "Join");
                    info.comJoinEnable = Utility.GetComponetInChild<ComButtonEnbale>(objRoot, "Join");
                    info.labJoin = Utility.GetComponetInChild<Text>(objRoot, "Join/Text");
                    info.imgIcon = Utility.GetComponetInChild<Image>(objRoot, "Icon");
                    info.vecIconPos = info.imgIcon.transform.localPosition;
                    info.colIconColor = info.imgIcon.color;
                    info.dotweens = info.imgIcon.GetComponents<DOTweenAnimation>();
                    info.labName = Utility.GetComponetInChild<Text>(objRoot, "Name");
                    info.labLevel = Utility.GetComponetInChild<Text>(objRoot, "Level");
                    info.objHelp = Utility.FindGameObject(objRoot, "HelpMark");
                    m_arrPosInfos.Add(info);
                }
                else
                {
                    m_arrPosInfos.Add(null);
                }
            }

            for (int i = 0; i < GuildDataManager.GetInstance().myGuild.arrTableMembers.Length; ++i)
            {
                GuildTableMember member = GuildDataManager.GetInstance().myGuild.arrTableMembers[i];
                _UpdatePos(i, member);
            }

            _UpdateRemainTimes();
            _UpdateFirstGetItems();
            _UpdateHelpGetItems();
            _UpdateDesc();
            _RegisterUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            m_arrPosInfos.Clear();
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildJoinTableSuccess, _OnGuildJoinTableSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildAddTableMember, _OnGuildAddTableMember);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildRemoveTableMember, _OnGuildRemoveTableMember);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildTableFinished, _OnTableFinished);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildJoinTableSuccess, _OnGuildJoinTableSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildAddTableMember, _OnGuildAddTableMember);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildRemoveTableMember, _OnGuildRemoveTableMember);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildTableFinished, _OnTableFinished);
        }

        void _UpdatePos(int a_nPos, GuildTableMember a_data)
        {
            if (a_nPos >= 0 && a_nPos < m_arrPosInfos.Count)
            {
                PosInfo info = m_arrPosInfos[a_nPos];
                info.data = a_data;
                info.comJoinEnable.SetEnable(true);
                if (a_data != null)
                {
                    Logger.LogProcessFormat("公会圆桌会议，更新第{0}个位置，有成员：{1}", a_nPos + 1, a_data.name);
                    info.btnJoin.onClick.RemoveAllListeners();
                    info.labJoin.gameObject.SetActive(false);
                    string strIcon = string.Empty;
                    ProtoTable.JobTable table = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(a_data.occu);
                    if (table != null)
                    {
                        ProtoTable.ResTable resTable = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(table.Mode);
                        if (resTable != null)
                        {
                            strIcon = resTable.IconPath;
                        }
                    }
                    info.imgIcon.gameObject.SetActive(true);
                    info.labLevel.gameObject.SetActive(true);
                    info.labName.gameObject.SetActive(true);
                    // info.imgIcon.sprite = AssetLoader.GetInstance().LoadRes(strIcon, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref info.imgIcon, strIcon);
                    info.labName.text = a_data.name;
                    info.labLevel.text = TR.Value("guild_table_level", a_data.level);
                    info.objHelp.SetActive(a_data.type != (byte)ProtoTable.GuildRoundtableTable.eType.First);
                }
                else
                {
                    Logger.LogProcessFormat("公会圆桌会议，更新第{0}个位置，没有成员", a_nPos + 1);
                    info.btnJoin.onClick.RemoveAllListeners();
                    info.btnJoin.onClick.AddListener(() => {
                        GuildDataManager.GetInstance().JoinTable(a_nPos);
                    });
                    info.labJoin.gameObject.SetActive(true);
                    info.imgIcon.gameObject.SetActive(false);
                    info.labLevel.gameObject.SetActive(false);
                    info.labName.gameObject.SetActive(false);
                    info.objHelp.SetActive(false);
                }
            }
        }

        void _UpdateFirstGetItems()
        {
            ProtoTable.GuildRoundtableTable table =
                TableManager.GetInstance().GetTableItem<ProtoTable.GuildRoundtableTable>((int)ProtoTable.GuildRoundtableTable.eType.First);
            for (int i = 0; i < table.GetItems.Count; ++i)
            {
                string[] values = table.GetItems[i].Split(',');
                int nID = int.Parse(values[0]);
                int nCount = int.Parse(values[1]);
                _SetupItemUI(nID, nCount, m_objFirstGetItemRoot);
            }
        }

        void _UpdateHelpGetItems()
        {
            ProtoTable.GuildRoundtableTable table =
                TableManager.GetInstance().GetTableItem<ProtoTable.GuildRoundtableTable>((int)ProtoTable.GuildRoundtableTable.eType.Help);
            for (int i = 0; i < table.GetItems.Count; ++i)
            {
                string[] values = table.GetItems[i].Split(',');
                int nID = int.Parse(values[0]);
                int nCount = int.Parse(values[1]);
                _SetupItemUI(nID, nCount, m_objHelpGetItemRoot);
            }
        }

        void _SetupItemUI(int a_nID, int a_nCount, GameObject a_objRoot)
        {
            GameObject obj = GameObject.Instantiate(m_objGetTemplate);
            obj.transform.SetParent(a_objRoot.transform, false);
            obj.SetActive(true);

            ItemData item = ItemDataManager.GetInstance().GetCommonItemTableDataByID(a_nID);
            //Utility.GetComponetInChild<Image>(obj, "Icon").sprite =
            //    AssetLoader.GetInstance().LoadRes(item.Icon, typeof(Sprite)).obj as Sprite;
            {
                Image img = Utility.GetComponetInChild<Image>(obj, "Icon");
                ETCImageLoader.LoadSprite(ref img, item.Icon);
            }
            Utility.GetComponetInChild<Text>(obj, "Count").text = a_nCount.ToString();
        }

        void _UpdateDesc()
        {
            m_labDecs.text = TR.Value("guild_table_desc");
        }

        void _UpdateRemainTimes()
        {
            m_labFirstRemainTimes.text = TR.Value("guild_table_remain_times", _GetFirstRemainTimes());
            m_labHelpRemainTimes.text = TR.Value("guild_table_remain_times", _GetHelpRemainTimes());
        }

        int _GetFirstRemainTimes()
        {
            int nMax = TableManager.GetInstance().GetTableItem<ProtoTable.GuildRoundtableTable>((int)ProtoTable.GuildRoundtableTable.eType.First).TimesLimit;
            int nTimes = nMax - CountDataManager.GetInstance().GetCount("guild_table");
            if (nTimes < 0)
            {
                nTimes = 0;
            }
            return nTimes;
        }

        int _GetHelpRemainTimes()
        {
            int nMax = TableManager.GetInstance().GetTableItem<ProtoTable.GuildRoundtableTable>((int)ProtoTable.GuildRoundtableTable.eType.Help).TimesLimit;
            int nTimes = nMax - CountDataManager.GetInstance().GetCount("guild_table_help");
            if (nTimes < 0)
            {
                nTimes = 0;
            }
            return nTimes;
        }

        IEnumerator _ShowTableFinished()
        {
            Logger.LogProcessFormat("公会圆桌会议完成，开始表现效果....");

            {
                float startTime = Time.time;
                float elapsed = 0.0f;
                while (elapsed < 0.5f)
                {
                    elapsed = Time.time - startTime;
                    yield return Yielders.EndOfFrame;
                }
            }

            for (int i = 0; i < m_arrPosInfos.Count; ++i)
            {
                PosInfo info = m_arrPosInfos[i];
                DOTweenAnimation[] dotweens = info.dotweens;
                for (int j = 0; j < dotweens.Length; ++j)
                {
                    dotweens[j].DORestart();
                }

                info.comJoinEnable.SetEnable(false);
                info.labLevel.gameObject.SetActive(false);
                info.labName.gameObject.SetActive(false);
                info.objHelp.SetActive(false);
            }
            m_parCompleteEffect1.StartEmit();
            m_parCompleteEffect2.StartEmit();

            {
                float startTime = Time.time;
                float elapsed = 0.0f;
                while (elapsed < 2.0f)
                {
                    elapsed = Time.time - startTime;
                    yield return Yielders.EndOfFrame;
                }
            }

            m_parCompleteEffect1.StopEmit();
            m_parCompleteEffect2.StopEmit();

            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_table_finished"));

            {
                float startTime = Time.time;
                float elapsed = 0.0f;
                while (elapsed < 2.0f)
                {
                    elapsed = Time.time - startTime;
                    yield return Yielders.EndOfFrame;
                }
            }

            for (int i = 0; i < m_arrPosInfos.Count; ++i)
            {
                PosInfo info = m_arrPosInfos[i];
                info.comJoinEnable.SetEnable(true);
                info.imgIcon.transform.localPosition = info.vecIconPos;
                info.imgIcon.color = info.colIconColor;
            }

            for (int i = 0; i < GuildDataManager.GetInstance().myGuild.arrTableMembers.Length; ++i)
            {
                GuildTableMember member = GuildDataManager.GetInstance().myGuild.arrTableMembers[i];
                _UpdatePos(i, member);
            }

            Logger.LogProcessFormat("公会圆桌会议完成效果结束");
        }

        void _OnGuildDismissed(UIEvent a_event)
        {
            frameMgr.CloseFrame(this);
        }

        void _OnGuildJoinTableSuccess(UIEvent a_event)
        {
//             int nIndex = (int)a_event.Param1;
//             if (nIndex >= 0 && nIndex < GuildDataManager.GetInstance().myGuild.arrTableMembers.Length)
//             {
//                 Logger.LogProcessFormat("公会圆桌会议，加入成功，位置{0}", nIndex + 1);
//                 _UpdatePos(nIndex, GuildDataManager.GetInstance().myGuild.arrTableMembers[nIndex]);
//             }
            _UpdateRemainTimes();
        }

        void _OnGuildAddTableMember(UIEvent a_event)
        {
            int nIndex = (int)a_event.Param1;
            if (nIndex >= 0 && nIndex < GuildDataManager.GetInstance().myGuild.arrTableMembers.Length)
            {
                Logger.LogProcessFormat("公会圆桌会议，第{0}个位置添加成员", nIndex + 1);
                _UpdatePos(nIndex, GuildDataManager.GetInstance().myGuild.arrTableMembers[nIndex]);
            }
        }

        void _OnGuildRemoveTableMember(UIEvent a_event)
        {
            int nIndex = (int)a_event.Param1;
            if (nIndex >= 0 && nIndex < GuildDataManager.GetInstance().myGuild.arrTableMembers.Length)
            {
                Logger.LogProcessFormat("公会圆桌会议，第{0}个位置移除成员", nIndex + 1);
                _UpdatePos(nIndex, GuildDataManager.GetInstance().myGuild.arrTableMembers[nIndex]);
            }
        }

        void _OnTableFinished(UIEvent a_event)
        {
            StartCoroutine(_ShowTableFinished());
        }

        [UIEventHandle("SendMessage")]
        void _OnSendMessageClicked()
        {
            ChatManager.GetInstance().SendChat(ChatType.CT_GUILD, TR.Value("guild_table_send_chat"));
            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_table_send_chat_success"));
        }
    }
}
