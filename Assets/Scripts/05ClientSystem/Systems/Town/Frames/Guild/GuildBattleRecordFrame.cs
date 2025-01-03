using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    class GuildBattleRecordFrame : ClientFrame
    {
        class GuildRecordScrollItem : ScrollItem
        {
            GuildBattleRecordFrame m_frame;
            Text m_labRecord;
            Transform m_itemTrans;

            public GuildRecordScrollItem(GuildBattleRecordFrame a_frame)
            {
                m_frame = a_frame;
                m_itemTrans = GameObject.Instantiate(m_frame.m_objRecordTemplate).transform;
                m_itemTrans.SetParent(m_frame.m_objRecordRoot.transform, false);
                m_labRecord = m_itemTrans.GetComponent<Text>();
            }

            public override Vector3 GetPosInContent()
            {
                return m_itemTrans.GetComponent<RectTransform>().localPosition;
            }

            public override ScrollItem Clone()
            {
                return new GuildRecordScrollItem(m_frame);
            }

            public override void Destroy()
            {
                GameObject.Destroy(m_itemTrans.gameObject);
                m_itemTrans = null;
                m_frame = null;
                m_labRecord = null;
            }

            public override void SetAsFirstSibling()
            {
                m_itemTrans.SetAsFirstSibling();
            }

            public override void SetAsLastSibling()
            {
                m_itemTrans.SetAsLastSibling();
            }

            public override void SetActive(bool a_bActive)
            {
                if (m_itemTrans.gameObject.activeSelf != a_bActive)
                {
                    m_itemTrans.gameObject.SetActive(a_bActive);
                }
            }

            public override bool IsActive()
            {
                return m_itemTrans.gameObject.activeSelf;
            }

            protected override void _Refresh(int a_nDataID)
            {
                List<GuildBattleRecord> records;
                if (m_frame.m_togOnlySelf.isOn)
                {
                    records = GuildDataManager.GetInstance().GetSelfBattleRecords();
                }
                else
                {
                    records = GuildDataManager.GetInstance().GetBattleRecords();
                }
                
                if (a_nDataID >= 0 && a_nDataID < records.Count)
                {
                    m_labRecord.text = _ParseRecord(records[a_nDataID]);
                }
                else
                {
                    m_labRecord.text = string.Empty;
                }
                m_itemTrans.gameObject.name = a_nDataID.ToString();
            }

            string _ParseRecord(GuildBattleRecord a_record)
            {
                string strResult = string.Empty;
                ulong roleID = PlayerBaseData.GetInstance().RoleID;
                int nMyScore = 0;
                string strWinName;
                string strLoserName;
                if (a_record.winner.id == roleID)
                {
                    strWinName = TR.Value("color_green", TR.Value("guild_battle_me"));
                    nMyScore = a_record.winner.gotScore;
                }
                else
                {
                    strWinName = TR.Value("color_yellow", a_record.winner.name);
                }
                if (a_record.loser.id == roleID)
                {
                    strLoserName = TR.Value("color_green", TR.Value("guild_battle_me"));
                    nMyScore = a_record.loser.gotScore;
                }
                else
                {
                    strLoserName = TR.Value("color_yellow", a_record.loser.name);
                }

                if (a_record.winner.winStreak > 1)
                {
                    if (a_record.loser.winStreak > 1)
                    {
                        strResult = TR.Value("guild_battle_record_format1", strWinName, strLoserName, a_record.winner.winStreak, a_record.loser.winStreak);
                    }
                    else
                    {
                        strResult = TR.Value("guild_battle_record_format2", strWinName, strLoserName, a_record.winner.winStreak);
                    }
                }
                else
                {
                    if (a_record.loser.winStreak > 1)
                    {
                        strResult = TR.Value("guild_battle_record_format3", strWinName, strLoserName, a_record.loser.winStreak);
                    }
                    else
                    {
                        strResult = TR.Value("guild_battle_record_format4", strWinName, strLoserName);
                    }
                }

                if (nMyScore > 0)
                {
                    strResult += TR.Value("guild_battle_record_store", nMyScore);
                }

                return strResult;
            }
        }

        [UIObject("Content/Scroll/Viewport/Content")]
        GameObject m_objRecordRoot;

        [UIObject("Content/Scroll/Viewport/Text")]
        GameObject m_objRecordTemplate;

        [UIControl("Content/OnlySelf")]
        Toggle m_togOnlySelf;

        [UIControl("Content/Scroll")]
        ComScrollList m_comSclRecords;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildBattleRecord";
        }

        protected override void _OnOpenFrame()
        {
            _InitUI();
            _RegisterUIEvent();

            _InitRecords(m_togOnlySelf.isOn);
        }

        protected override void _OnCloseFrame()
        {
            _ClearUI();
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBattleRecordSync, _OnRecordSync);

        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBattleRecordSync, _OnRecordSync);
        }

        void _InitUI()
        {
            m_objRecordTemplate.SetActive(false);
            m_comSclRecords.startFouce = ComScrollList.EStartFouce.Bottom;
            m_comSclRecords.subScrollItemCount = 1;
            m_comSclRecords.mainScrollItemCount = 30;
            m_comSclRecords.dynamicMainScrollItemCount = 10;
            m_comSclRecords.SetTemplate(new GuildRecordScrollItem(this));
        }

        void _ClearUI()
        {
            m_comSclRecords.Clear();
        }

        void _InitRecords(bool a_bOnlySelf)
        {
            if (a_bOnlySelf)
            {
                m_comSclRecords.SetDataRange(0, GuildDataManager.GetInstance().GetSelfBattleRecords().Count - 1);
            }
            else
            {
                m_comSclRecords.SetDataRange(0, GuildDataManager.GetInstance().GetBattleRecords().Count - 1);
            }
            m_comSclRecords.InitScrollItems();
        }

        void _ClearRecords()
        {
            m_comSclRecords.ClearScrollItems();
        }

        void _UpdateRecords(bool a_bOnlySelf)
        {
            if (a_bOnlySelf)
            {
                m_comSclRecords.SetDataRange(0, GuildDataManager.GetInstance().GetSelfBattleRecords().Count - 1);
            }
            else
            {
                m_comSclRecords.SetDataRange(0, GuildDataManager.GetInstance().GetBattleRecords().Count - 1);
            }
        }

        void _OnRecordSync(UIEvent a_event)
        {
            _UpdateRecords(m_togOnlySelf.isOn);
        }

        [UIEventHandle("Content/OnlySelf", typeof(Toggle), typeof(UnityEngine.Events.UnityAction<bool>))]
        void _OnOnlySelfClicked(bool a_bChecked)
        {
            _ClearRecords();
            _InitRecords(a_bChecked);
        }

        [UIEventHandle("Content/Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
