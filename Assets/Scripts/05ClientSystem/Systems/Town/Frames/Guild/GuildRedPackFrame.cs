using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;


namespace GameClient
{
    class GuildRedPackFrame : ClientFrame
    {
        class RedPacketMyInfo
        {
            public RedPacketBaseEntry data;
            public GameObject objRoot;
        }

        class RedPacketOtherInfo
        {
            public RedPacketBaseEntry data;
            public GameObject objRoot;
            public Text labFunc;
        }

        [UIObject("MyRedPack/ScrollView/Viewport/Content/Template1")]
        GameObject m_objMyTemplate1;

        [UIObject("MyRedPack/ScrollView/Viewport/Content/Template2")]
        GameObject m_objMyTemplate2;

        [UIObject("MyRedPack/ScrollView/Viewport/Content")]
        GameObject m_objMyRoot;

        [UIObject("OtherRedPack/ScrollView/Viewport/Content")]
        GameObject m_objOtherRoot;

        [UIObject("OtherRedPack/ScrollView/Viewport/Content/Template")]
        GameObject m_objOtherTemplate;

        bool m_bTableDataInited = false;
        List<RedPacketTable> m_arrBuyTable = new List<RedPacketTable>();

        List<RedPacketMyInfo> m_arrMyInfos = new List<RedPacketMyInfo>();
        List<RedPacketOtherInfo> m_arrOtherInfos = new List<RedPacketOtherInfo>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildRedPack";
        }

        protected override void _OnOpenFrame()
        {
            if (GuildDataManager.GetInstance().HasSelfGuild() == false)
            {
                return;
            }

            GameObject objTempRoot = Utility.FindGameObject(frame, "MyRedPack/ScrollView/Template");
            objTempRoot.SetActive(false);
            m_objMyTemplate1.transform.SetParent(objTempRoot.transform, false);
            m_objMyTemplate2.transform.SetParent(objTempRoot.transform, false);
            m_objOtherTemplate.transform.SetParent(objTempRoot.transform, false);

            _InitBuyTableData();
            _InitMyRedPacket();
            _InitOtherRedPacket();

            _RegisterUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            m_arrMyInfos.Clear();
            m_arrOtherInfos.Clear();
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketSendSuccess, _OnSendSuccess);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketOpenSuccess, _OnOpenSuccess);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketCheckSuccess, _OnCheckSuccess);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketGet, _OnGetRedPacket);
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPacketDelete, _OnDeleteRedPacket);
        }

        void _UnRegisterUIEvent()
        {
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketSendSuccess, _OnSendSuccess);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketOpenSuccess, _OnOpenSuccess);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketCheckSuccess, _OnCheckSuccess);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketGet, _OnGetRedPacket);
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPacketDelete, _OnDeleteRedPacket);
        }

        void _InitBuyTableData()
        {
            if (m_bTableDataInited == false)
            {
                var iter = TableManager.GetInstance().GetTable<ProtoTable.RedPacketTable>().GetEnumerator();
                while (iter.MoveNext())
                {
                    RedPacketTable table = iter.Current.Value as RedPacketTable;
                    if (table.Type == RedPacketTable.eType.GUILD &&
                        table.SubType == RedPacketTable.eSubType.Buy)
                    {
                        m_arrBuyTable.Add(table);
                    }
                }

                if (m_arrBuyTable.Count > 0)
                {
                    m_arrBuyTable.Sort((a, b) => { return a.TotalMoney - b.TotalMoney; });
                }

                m_bTableDataInited = true;
            }
        }

        int _GetRemainBuyTime()
        {
            int nMaxTime = TableManager.instance.GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_GUILD_BUY_REDPACKET_TIME).Value;
            int nCostTime = CountDataManager.GetInstance().GetCount("guild_pay_rp");
            int nRemainTime = nMaxTime - nCostTime;
            if (nRemainTime < 0)
            {
                nRemainTime = 0;
            }
            return nRemainTime;
        }

        void _InitMyRedPacket()
        {
            m_arrMyInfos.Clear();
            List<RedPacketBaseEntry> arrRedPacket = RedPackDataManager.GetInstance().GetRedPacketsByType(Protocol.RedPacketType.GUILD);
            for (int i = 0; i < arrRedPacket.Count; ++i)
            {
                _CreateMyRedPacket(arrRedPacket[i]);
            }
            _CreateMyRedPacketBuy();

            _UpdateMyRedPacketLayout();
        }

        void _InitOtherRedPacket()
        {
            m_arrOtherInfos.Clear();

            List<RedPacketBaseEntry> arrRedPacket = RedPackDataManager.GetInstance().GetRedPacketsByType(Protocol.RedPacketType.GUILD);
            for (int i = 0; i < arrRedPacket.Count; ++i)
            {
                _CreateOtherRedPacket(arrRedPacket[i]);
            }

            _UpdateOtherRedPacketLayout();
        }

        void _CreateMyRedPacket(RedPacketBaseEntry a_data)
        {
            if (a_data.ownerId != PlayerBaseData.GetInstance().RoleID)
            {
                return;
            }

            RedPacketMyInfo info = new RedPacketMyInfo();
            info.data = a_data;
            if (a_data.status == (byte)RedPacketStatus.INIT)
            {
                info.objRoot = GameObject.Instantiate(m_objMyTemplate1);
                info.objRoot.SetActive(true);
                info.objRoot.transform.SetParent(m_objMyRoot.transform, false);

                Utility.GetComponetInChild<Text>(info.objRoot, "Title").text = a_data.name;
                Utility.GetComponetInChild<Text>(info.objRoot, "Func").text = TR.Value("guild_redpacket_send_single");
                //Utility.GetComponetInChild<Image>(info.objRoot, "Desc/Image").sprite =
                //    AssetLoader.instance.LoadRes(RedPackDataManager.GetInstance().GetCostMoneyIcon(a_data.reason), typeof(Sprite)).obj as Sprite;
                {
                    Image img = Utility.GetComponetInChild<Image>(info.objRoot, "Desc/Image");
                    ETCImageLoader.LoadSprite(ref img, RedPackDataManager.GetInstance().GetCostMoneyIcon(a_data.reason));
                }
                Utility.GetComponetInChild<Text>(info.objRoot, "Desc/Text").text = a_data.totalMoney.ToString();

                Button btn = info.objRoot.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    //frameMgr.OpenFrame<GuildSendRedPacketFrame>(FrameLayer.Middle, a_data);
                });
            }
            else if (a_data.status == (byte)RedPacketStatus.WAIT_RECEIVE && a_data.opened == 0)
            {
                info.objRoot = GameObject.Instantiate(m_objMyTemplate1);
                info.objRoot.SetActive(true);
                info.objRoot.transform.SetParent(m_objMyRoot.transform, false);

                Utility.GetComponetInChild<Text>(info.objRoot, "Title").text = a_data.name;
                Utility.GetComponetInChild<Text>(info.objRoot, "Func").text = TR.Value("guild_redpacket_open_single");
                //Utility.GetComponetInChild<Image>(info.objRoot, "Desc/Image").sprite =
                //    AssetLoader.instance.LoadRes(RedPackDataManager.GetInstance().GetCostMoneyIcon(a_data.reason), typeof(Sprite)).obj as Sprite;
                {
                    Image img = Utility.GetComponetInChild<Image>(info.objRoot, "Desc/Image");
                    ETCImageLoader.LoadSprite(ref img, RedPackDataManager.GetInstance().GetCostMoneyIcon(a_data.reason));
                }
                Utility.GetComponetInChild<Text>(info.objRoot, "Desc/Text").text = a_data.totalMoney.ToString();

                Button btn = info.objRoot.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    //RedPackDataManager.GetInstance().OpenRedPacket(a_data.id);
                });
            }
            else
            {
                info.objRoot = GameObject.Instantiate(m_objMyTemplate2);
                info.objRoot.SetActive(true);
                info.objRoot.transform.SetParent(m_objMyRoot.transform, false);

                Utility.GetComponetInChild<Text>(info.objRoot, "Name").text = a_data.name;
                //Utility.GetComponetInChild<Image>(info.objRoot, "Desc/Image").sprite =
                //    AssetLoader.instance.LoadRes(RedPackDataManager.GetInstance().GetCostMoneyIcon(a_data.reason), typeof(Sprite)).obj as Sprite;
                {
                    Image img = Utility.GetComponetInChild<Image>(info.objRoot, "Desc/Image");
                    ETCImageLoader.LoadSprite(ref img, RedPackDataManager.GetInstance().GetCostMoneyIcon(a_data.reason));
                }
                Utility.GetComponetInChild<Text>(info.objRoot, "Desc/Text").text = a_data.totalMoney.ToString();

                Button btn = info.objRoot.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    RedPackDataManager.GetInstance().CheckRedPacket(a_data.id);
                });
            }

            m_arrMyInfos.Add(info);
        }

        void _CreateMyRedPacketBuy()
        {
            if (m_arrBuyTable.Count > 0)
            {
                RedPacketMyInfo info = new RedPacketMyInfo();
                info.data = null;
                info.objRoot = GameObject.Instantiate(m_objMyTemplate1);
                info.objRoot.SetActive(true);
                info.objRoot.transform.SetParent(m_objMyRoot.transform, false);

                Utility.GetComponetInChild<Text>(info.objRoot, "Title").text = TR.Value("guild_redpacket_buy_desc");
                Utility.GetComponetInChild<Image>(info.objRoot, "Desc/Image").gameObject.SetActive(false);
                Text lab = Utility.GetComponetInChild<Text>(info.objRoot, "Desc/Text");
                if (Utility.GetCurVipLevelPrivilegeData(ProtoTable.VipPrivilegeTable.eType.GUILD_PRIVATECOST_REDPACKET) == 0)
                {
                    int nNeedVipLevel = Utility.GetFirstValidVipLevelPrivilegeData(ProtoTable.VipPrivilegeTable.eType.GUILD_PRIVATECOST_REDPACKET).Key;
                    lab.text = TR.Value("vip_open_limit_desc", nNeedVipLevel);
                }
                else
                {
                    lab.text = TR.Value("guild_redpacket_buy_remain_time", _GetRemainBuyTime());
                }

                Button btn = info.objRoot.GetComponent<Button>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    /*if (Utility.GetCurVipLevelPrivilegeData(ProtoTable.VipPrivilegeTable.eType.GUILD_PRIVATECOST_REDPACKET) == 0)
                    {
                        //int nNeedVipLevel = Utility.GetFirstValidVipLevelPrivilegeData(ProtoTable.VipPrivilegeTable.eType.GUILD_PRIVATECOST_REDPACKET).Key;
                        //SystemNotifyManager.SysNotifyTextAnimation(TR.Value("vip_open_limit_desc", nNeedVipLevel));

                        SystemNotifyManager.SystemNotify(1000051, () =>
                        {
                            var frame = ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle) as VipFrame;
                            frame.OpenPayTab();
                        });
                        return;
                    }*/
                    //frameMgr.OpenFrame<GuildSendRedPacketFrame>(FrameLayer.Middle, m_arrBuyTable);
                });
                m_arrMyInfos.Add(info);
            }
        }

        void _CreateOtherRedPacket(RedPacketBaseEntry a_data)
        {
            if (a_data.ownerId == PlayerBaseData.GetInstance().RoleID)
            {
                return;
            }

            RedPacketTable tableData = TableManager.instance.GetTableItem<RedPacketTable>(a_data.reason);
            if (tableData == null)
            {
                Logger.LogErrorFormat("红包表找不到ID为{0}的数据，可能服务器和客户端表格不一致", a_data.reason);
                return;
            }

            RedPacketOtherInfo info = new RedPacketOtherInfo();
            info.data = a_data;
            info.objRoot = GameObject.Instantiate(m_objOtherTemplate);
            info.objRoot.transform.SetParent(m_objOtherRoot.transform, false);
            info.objRoot.SetActive(true);
            info.labFunc = Utility.GetComponetInChild<Text>(info.objRoot, "Func/Text");
            Image imgState = Utility.GetComponetInChild<Image>(info.objRoot, "Tag");
            if (info.data.status == (byte)RedPacketStatus.WAIT_RECEIVE && info.data.opened == 0)
            {
                info.labFunc.text = TR.Value("guild_redpacket_open");
                //imgState.sprite = 
            }
            else
            {
                info.labFunc.text = TR.Value("guild_redpacket_check");
            }

            Utility.GetComponetInChild<Text>(info.objRoot, "OwnerName").text = info.data.ownerName;
            Utility.GetComponetInChild<Text>(info.objRoot, "Name").text = info.data.name;
            Utility.GetComponetInChild<Text>(info.objRoot, "Desc/Count").text = info.data.totalMoney.ToString();
            //Utility.GetComponetInChild<Image>(info.objRoot, "Desc/Icon").sprite = AssetLoader.instance.LoadRes(
            //    ItemDataManager.GetInstance().GetCommonItemTableDataByID(tableData.CostMoneyID).Icon, typeof(Sprite)).obj as Sprite;
            {
                Image img = Utility.GetComponetInChild<Image>(info.objRoot, "Desc/Icon");
                ETCImageLoader.LoadSprite(ref img, ItemDataManager.GetInstance().GetCommonItemTableDataByID(tableData.CostMoneyID).Icon);
            }

            Button btn = Utility.GetComponetInChild<Button>(info.objRoot, "Func");
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
//                 if (info.data.status == (byte)RedPacketStatus.WAIT_RECEIVE && info.data.opened == 0)
//                 {
//                     RedPackDataManager.GetInstance().OpenRedPacket(info.data.id);
//                 }
//                 else
//                 {
//                     RedPackDataManager.GetInstance().CheckRedPacket(info.data.id);
//                 }
            });

            m_arrOtherInfos.Add(info);
        }

        void _UpdateMyRedPacketLayout()
        {
            m_arrMyInfos.Sort((a, b) =>
            {
                if (a.data == null)
                {
                    return 1;
                }
                else if (b.data == null)
                {
                    return -1;
                }
                else
                {
                    if (a.data.status == (byte)RedPacketStatus.INIT)
                    {
                        if (b.data.status == (byte)RedPacketStatus.INIT)
                        {
                            return 0;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else if (a.data.status == (byte)RedPacketStatus.WAIT_RECEIVE)
                    {
                        if (b.data.status == (byte)RedPacketStatus.INIT)
                        {
                            return 1;
                        }
                        else if (b.data.status == (byte)RedPacketStatus.WAIT_RECEIVE)
                        {
                            return a.data.opened - b.data.opened;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        return 1;
                    }
                }
            });


            for (int i = 0; i < m_arrMyInfos.Count; ++i)
            {
                m_arrMyInfos[i].objRoot.transform.SetSiblingIndex(i);
            }

        }

        void _UpdateOtherRedPacketLayout()
        {
            m_arrOtherInfos.Sort((a, b) =>
            {
                if (a.data.status == (byte)RedPacketStatus.WAIT_RECEIVE)
                {
                    if (b.data.status == (byte)RedPacketStatus.WAIT_RECEIVE)
                    {
                        return a.data.opened - b.data.opened;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    if (b.data.status == (byte)RedPacketStatus.WAIT_RECEIVE)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            });

            for (int i = 0; i < m_arrOtherInfos.Count; ++i)
            {
                m_arrOtherInfos[i].objRoot.transform.SetSiblingIndex(i);
            }
        }

        void _UpdateMyRedPacketBuy()
        {
            for (int i = 0; i < m_arrMyInfos.Count; ++i)
            {
                if (m_arrMyInfos[i].data == null)
                {
                    Utility.GetComponetInChild<Text>(m_arrMyInfos[i].objRoot, "Desc/Text").text = 
                        TR.Value("guild_redpacket_buy_remain_time", _GetRemainBuyTime());
                }
            }
        }

        void _OnSendSuccess(UIEvent a_event)
        {
            RedPacketBaseEntry data = RedPackDataManager.GetInstance().GetRedPacketBaseInfo((ulong)a_event.Param1);
            if (data.ownerId == PlayerBaseData.GetInstance().RoleID)
            {
                for (int i = 0; i < m_arrMyInfos.Count; ++i)
                {
                    if (m_arrMyInfos[i].data != null && m_arrMyInfos[i].data.id == data.id)
                    {
                        GameObject.Destroy(m_arrMyInfos[i].objRoot);
                        m_arrMyInfos.RemoveAt(i);
                        break;
                    }
                }

                _UpdateMyRedPacketBuy();
                _CreateMyRedPacket(data);
                _UpdateMyRedPacketLayout();

                string str = TR.Value("guild_redpacket_send_chat", PlayerBaseData.GetInstance().Name, data.id);
                str = str.Replace('(', '{');
                str = str.Replace(')', '}');
                ChatManager.GetInstance().SendChat(ChatType.CT_GUILD, str);
            }
        }

        void _OnOpenSuccess(UIEvent a_event)
        {
//             if (frameMgr.IsFrameOpen<GuildOpenRedPacketFrame>() == false)
//             {
//                 frameMgr.OpenFrame<GuildOpenRedPacketFrame>(FrameLayer.Middle, a_event.Param1);
//             }

            RedPacketDetail detail = a_event.Param1 as RedPacketDetail;

            if (detail.baseEntry.ownerId == PlayerBaseData.GetInstance().RoleID)
            {
                for (int i = 0; i < m_arrMyInfos.Count; ++i)
                {
                    if (m_arrMyInfos[i].data != null && m_arrMyInfos[i].data.id == detail.baseEntry.id)
                    {
                        GameObject.Destroy(m_arrMyInfos[i].objRoot);
                        m_arrMyInfos.RemoveAt(i);
                        break;
                    }
                }

                _CreateMyRedPacket(detail.baseEntry);
                _UpdateMyRedPacketLayout();
            }
            else
            {
                for (int i = 0; i < m_arrOtherInfos.Count; ++i)
                {
                    if (m_arrOtherInfos[i].data.id == detail.baseEntry.id)
                    {
                        if (m_arrOtherInfos[i].data.status == (byte)RedPacketStatus.WAIT_RECEIVE && m_arrOtherInfos[i].data.opened == 0)
                        {
                            m_arrOtherInfos[i].labFunc.text = TR.Value("guild_redpacket_open");
                            //imgState.sprite = 
                        }
                        else
                        {
                            m_arrOtherInfos[i].labFunc.text = TR.Value("guild_redpacket_check");
                        }
                        _UpdateOtherRedPacketLayout();
                        break;
                    }
                }
            }
        }

        void _OnCheckSuccess(UIEvent a_event)
        {
            //frameMgr.OpenFrame<GuildOpenedRedPacketFrame>(FrameLayer.Middle, a_event.Param1);
        }

        void _OnGetRedPacket(UIEvent a_event)
        {
            List<ulong> ids = (List<ulong>)a_event.Param1;
            for (int i = 0; i < ids.Count; ++i)
            {
                RedPacketBaseEntry data = RedPackDataManager.GetInstance().GetRedPacketBaseInfo(ids[i]);
                if (data != null)
                {
                    if (data.ownerId == PlayerBaseData.GetInstance().RoleID)
                    {
                        _CreateMyRedPacket(data);
                        _UpdateMyRedPacketLayout();
                    }
                    else
                    {
                        _CreateOtherRedPacket(data);
                        _UpdateOtherRedPacketLayout();
                    }
                }
            }
        }

        void _OnDeleteRedPacket(UIEvent a_event)
        {
            ulong[] arrIDs = (ulong[])a_event.Param1;
            for (int i = 0; i < arrIDs.Length; ++i)
            {
                ulong uID = arrIDs[i];
                int nIndex = m_arrMyInfos.FindIndex(value =>
                {
                    if (value.data != null)
                    {
                        return value.data.id == uID;
                    }
                    else
                    {
                        return false;
                    }
                });

                if (nIndex >= 0 && nIndex < m_arrMyInfos.Count)
                {
                    GameObject.Destroy(m_arrMyInfos[nIndex].objRoot);
                    m_arrMyInfos.RemoveAt(nIndex);
                }
            }
        }

        [UIEventHandle("BG/Title/Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
