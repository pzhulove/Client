using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public enum SendRedPackType
    {
        Invalid,
        NewYear,    // 新年红包
        GuildMember, // 公会成员
        GuildWar,       // 公会战
        CrossGuildWar,  // 跨服公会战
        GuildDungeon,   // 公会地下城
    }

    class SendRedPacketFrame : ClientFrame
    {
        [UIObject("TotalMoney/Decrease")]
        GameObject m_objDecreaseMoney;

        [UIObject("TotalMoney/Increase")]
        GameObject m_objIncreaseMoney;

        [UIObject("TotalNum/Decrease")]
        GameObject m_objDecreaseNum;

        [UIObject("TotalNum/Increase")]
        GameObject m_objIncreaseNum;

        [UIControl("TotalMoney/Group/Icon")]
        Image m_imgMoneyIcon;

        [UIControl("TotalMoney/Group/Text")]
        Text m_labMoneyCount;

        [UIControl("TotalNum/Group/Text")]
        Text m_labNumCount;

        [UIControl("Name/InputField", typeof(InputField))]
        InputField m_inputName;

        [UIControl("Button/Icon")]
        Image m_imgCostIcon;

        [UIControl("Button/Count")]
        Text m_labCostCount;

        [UIControl("Owned/Group/Icon")]
        Image m_imgOwnedIcon;

        [UIControl("Owned/Group/Text")]
        Text m_labOwnedCount;

        [UIControl("Button")]
        Button m_btnSend;

        List<RedPacketTable> m_buyData = null;
        int m_nCurrentMoney = 0;
        int m_nCurrentNum = 0;
        RedPacketBaseEntry m_sendData = null;       

        public static SendRedPackType sendRedPackType = SendRedPackType.NewYear;     

        private Text type = null;
        private Button select = null;
        private GameObject guildRedPackTypeListRoot = null;
        private Toggle togSelect = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/RedPack/SendRedPack";
        }        

        protected override void _OnOpenFrame()
        {
            RedPackDataManager.GetInstance().SendWorldGetGuildRedPacketInfoReq();

            m_buyData = userData as List<RedPacketTable>;
            m_sendData = userData as RedPacketBaseEntry;

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSelectGuildRedPackType, _OnSelectGuildRedPackType);

            if (m_buyData == null)
            {
                CalGuildRedPackBuyData();

                if(m_buyData == null)
                {
                    return;
                }

                if (m_buyData.Count == 0)
                {
                    Debug.LogErrorFormat("m_buyData count = 0,sendRedPackType = {0}", sendRedPackType);
                    return;
                }               
            }

            SetRedPackTypeName();

            if(sendRedPackType == SendRedPackType.NewYear)
            {
                select.SafeSetGray(true);
                UIGray uIGray = togSelect.gameObject.SafeAddComponent<UIGray>();
                if(uIGray != null)
                {
                    uIGray.enabled = true;
                }
                togSelect.interactable = false;
                togSelect.image.raycastTarget = false;
            }

            // 自费红包
            if (m_buyData.Count <= 0)
            {
                Logger.LogError("open guild send redpacket frame, buyData is invalid!");
                return;
            }

            m_objDecreaseMoney.SetActive(true);
            m_objDecreaseNum.SetActive(true);
            m_objIncreaseMoney.SetActive(true);
            m_objIncreaseNum.SetActive(true);

            RedPacketTable data = m_buyData[m_nCurrentMoney];
            string strIcon = ItemDataManager.GetInstance().GetCommonItemTableDataByID(data.CostMoneyID).Icon;

            // m_imgMoneyIcon.sprite = AssetLoader.instance.LoadRes(strIcon, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref m_imgMoneyIcon, strIcon);
            // m_imgCostIcon.sprite = AssetLoader.instance.LoadRes(strIcon, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref m_imgCostIcon, strIcon);
            // m_imgOwnedIcon.sprite = AssetLoader.instance.LoadRes(strIcon, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref m_imgOwnedIcon, strIcon);
            m_labMoneyCount.text = data.TotalMoney.ToString();
            m_labNumCount.text = _ModifyCurrentNum(data) ? data.Num[m_nCurrentNum].ToString() : "0";
            m_labCostCount.text = data.TotalMoney.ToString();
            m_labOwnedCount.text = ItemDataManager.GetInstance().GetOwnedItemCount(data.CostMoneyID).ToString();
            m_inputName.interactable = true;
            m_inputName.text = data.Desc;

            m_btnSend.onClick.RemoveAllListeners();
            m_btnSend.onClick.AddListener(() => { OnClickSendRedPack(); });

            //             else
            //             {
            //                 // 系统红包
            //                 RedPacketTable data = TableManager.GetInstance().GetTableItem<RedPacketTable>(m_sendData.reason);
            //                 if (data == null)
            //                 {
            //                     Logger.LogErrorFormat("红包表找不到ID为{0}的数据，可能服务器和客户端表格不一致", m_sendData.reason);
            //                     return;
            //                 }
            // 
            //                 if (data.Num.Count > 1)
            //                 {
            //                     m_objDecreaseMoney.SetActive(true);
            //                     m_objDecreaseNum.SetActive(true);
            //                     m_objIncreaseMoney.SetActive(true);
            //                     m_objIncreaseNum.SetActive(true);
            //                 }
            //                 else
            //                 {
            //                     m_objDecreaseMoney.SetActive(false);
            //                     m_objDecreaseNum.SetActive(false);
            //                     m_objIncreaseMoney.SetActive(false);
            //                     m_objIncreaseNum.SetActive(false);
            //                 }
            // 
            //                 string strIcon = ItemDataManager.GetInstance().GetCommonItemTableDataByID(data.CostMoneyID).Icon;
            // 
            //                 // m_imgMoneyIcon.sprite = AssetLoader.instance.LoadRes(strIcon, typeof(Sprite)).obj as Sprite;
            //                 ETCImageLoader.LoadSprite(ref m_imgMoneyIcon, strIcon);
            //                 // m_imgCostIcon.sprite = AssetLoader.instance.LoadRes(strIcon, typeof(Sprite)).obj as Sprite;
            //                 ETCImageLoader.LoadSprite(ref m_imgCostIcon, strIcon);
            //                 // m_imgOwnedIcon.sprite = AssetLoader.instance.LoadRes(strIcon, typeof(Sprite)).obj as Sprite;
            //                 ETCImageLoader.LoadSprite(ref m_imgOwnedIcon, strIcon);
            //                 m_labMoneyCount.text = m_sendData.totalMoney.ToString();
            //                 m_labNumCount.text = _ModifyCurrentNum(data) ? data.Num[m_nCurrentNum].ToString() : "0";
            //                 m_labCostCount.text = "0";
            //                 m_labOwnedCount.text = ItemDataManager.GetInstance().GetOwnedItemCount(data.CostMoneyID).ToString();
            //                 m_inputName.interactable = false;
            //                 m_inputName.text = m_sendData.name;
            // 
            //                 m_btnSend.onClick.RemoveAllListeners();
            //                 m_btnSend.onClick.AddListener(() =>
            //                 {
            //                     RedPackDataManager.GetInstance().SendRedPacket(m_sendData.id, data.Num[m_nCurrentNum]);
            //                     frameMgr.CloseFrame(this);
            //                 });
            //             }
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSelectGuildRedPackType, _OnSelectGuildRedPackType);

            sendRedPackType = SendRedPackType.NewYear;

            m_buyData = null;
            m_sendData = null;
            m_nCurrentMoney = 0;
            m_nCurrentNum = 0;
        }

        private void _OnSelectGuildRedPackType(UIEvent uiEvent)
        {
            if(uiEvent.Param1 is SendRedPackType)
            {
                togSelect.SafeSetToggleOnState(false);

                SendRedPackType type = (SendRedPackType)uiEvent.Param1;
                if(type != SendRedPackType.NewYear)
                {
                    sendRedPackType = type;
                    CalGuildRedPackBuyData();
                    SetRedPackTypeName();

                    m_nCurrentMoney = 0;
                    m_nCurrentNum = 0;
                    _OnMoneyDecreaesClicked();
                    _OnNumIncreaesClicked();
                }
            }            
        }

        protected override void _bindExUI()
        {
            type = mBind.GetCom<Text>("type");
            select = mBind.GetCom<Button>("select");
            select.SafeSetOnClickListener(() => 
            {
                guildRedPackTypeListRoot.CustomActive(true);
            });

            guildRedPackTypeListRoot = mBind.GetGameObject("guildRedPackTypeListRoot");

            togSelect = mBind.GetCom<Toggle>("togSelect");
            togSelect.SafeSetOnValueChangedListener((value) => 
            {
                guildRedPackTypeListRoot.CustomActive(value);
            });
        }

        protected override void _unbindExUI()
        {
            type = null;
            select = null;
            guildRedPackTypeListRoot = null;
            togSelect = null;
        }

        void CalGuildRedPackBuyData()
        {
            m_buyData = new List<RedPacketTable>();
            if (m_buyData == null)
            {
                return;
            }

            RedPacketTable.eThirdType eThirdType = RedPacketTable.eThirdType.INVALID;
            if (sendRedPackType == SendRedPackType.GuildMember)
            {
                eThirdType = RedPacketTable.eThirdType.GUILD_ALL;
            }
            else if (sendRedPackType == SendRedPackType.GuildWar)
            {
                eThirdType = RedPacketTable.eThirdType.GUILD_BATTLE;
            }
            else if (sendRedPackType == SendRedPackType.CrossGuildWar)
            {
                eThirdType = RedPacketTable.eThirdType.GUILD_CROSS_BATTLE;
            }
            else if (sendRedPackType == SendRedPackType.GuildDungeon)
            {
                eThirdType = RedPacketTable.eThirdType.GUILD_DUNGEON;
            }

            Dictionary<int, object> dicts = TableManager.instance.GetTable<RedPacketTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    RedPacketTable adt = iter.Current.Value as RedPacketTable;
                    if (adt == null)
                    {
                        continue;
                    }

                    if (adt.ThirdType == eThirdType && adt.SubType == RedPacketTable.eSubType.Buy)
                    {
                        m_buyData.Add(adt);
                    }
                }
            }

            return;
        }

        public static string GetRedPackTypeName(SendRedPackType sendRedPackType)
        {
            switch (sendRedPackType)
            {
                case SendRedPackType.NewYear:
                    return (TR.Value("guild_red_packet_type_new_year"));               
                case SendRedPackType.GuildMember:
                    return (TR.Value("guild_red_packet_type_all_members"));             
                case SendRedPackType.GuildWar:
                    return (TR.Value("guild_red_packet_type_guild_war"));            
                case SendRedPackType.CrossGuildWar:
                    return (TR.Value("guild_red_packet_type_cross_guild_war"));     
                case SendRedPackType.GuildDungeon:
                    return (TR.Value("guild_red_packet_type_guild_dungeon"));                 
                default:
                    return ("");               
            }
        }

        void SetRedPackTypeName()
        {
            type.SafeSetText(GetRedPackTypeName(sendRedPackType));
        }

        bool _ModifyCurrentMoney()
        {
            if (m_nCurrentMoney < 0)
            {
                m_nCurrentMoney = 0;
            }
            if (m_nCurrentMoney >= m_buyData.Count)
            {
                m_nCurrentMoney = m_buyData.Count - 1;
            }

            if (m_nCurrentMoney >= 0 && m_nCurrentMoney < m_buyData.Count)
            {
                return true;
            }
            else
            {
                Logger.LogErrorFormat("当前选中的红包金额非法！(index:{0})", m_nCurrentMoney);
                return false;
            }
        }

        bool _ModifyCurrentNum(RedPacketTable a_data)
        {
            if (m_nCurrentNum < 0)
            {
                m_nCurrentNum = 0;
            }
            if (m_nCurrentNum >= a_data.Num.Count)
            {
                m_nCurrentNum = a_data.Num.Count - 1;
            }

            if (m_nCurrentNum >= 0 && m_nCurrentNum < a_data.Num.Count)
            {
                return true;
            }
            else
            {
                Logger.LogErrorFormat("当前选中的红包数量非法！(index:{0})", m_nCurrentNum);
                return false;
            }
        }

        // 如果 红包数大于人数    提示：            要花费XX点券发放  ，是否确定
        // 如果人数大于红包       提示：              您发的红包个数超过了可领取人数，多余的红包无法被领取也不会被退还，是否要花费xx点券发放？
        void OnClickSendRedPack()
        {
            if (m_nCurrentMoney >= 0 && m_nCurrentMoney < m_buyData.Count)
            {
                RedPacketTable temp = m_buyData[m_nCurrentMoney];
                if (m_nCurrentNum >= 0 && m_nCurrentNum < temp.Num.Count)
                {
                    string strName = m_inputName.text;                  
                    if (string.IsNullOrEmpty(strName) == false)
                    {
                        string content = TR.Value("guild_red_packet_send_confirm", temp.TotalMoney);                        
                        if (temp.ThirdType != RedPacketTable.eThirdType.INVALID)
                        {
                            GuildRedPacketSpecInfo guildRedPacketSpecInfo = RedPackDataManager.GetInstance().GetGuildRedPacketSpecInfo(sendRedPackType);
                            if(guildRedPacketSpecInfo != null && guildRedPacketSpecInfo.joinNum < temp.Num[m_nCurrentNum])
                            {
                                content = TR.Value("guild_red_packet_send_confirm2", temp.TotalMoney);
                            }
                        }

                        SystemNotifyManager.SysNotifyMsgBoxCancelOk(content,null,() => 
                        {
                            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();

                            costInfo.nMoneyID = temp.CostMoneyID;
                            costInfo.nCount = temp.TotalMoney;

                            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                            {
                                RedPackDataManager.GetInstance().SendRedPacket((ushort)temp.ID, temp.Num[m_nCurrentNum], strName);
                                frameMgr.CloseFrame(this);
                            });
                        });                       
                    }
                    else
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_redpacket_send_need_name"));
                    }
                }
                else
                {
                    Logger.LogErrorFormat("当前选中的红包数量非法！(index:{0})", m_nCurrentNum);
                }
            }
            else
            {
                Logger.LogErrorFormat("当前选中的红包金额非法！(index:{0})", m_nCurrentMoney);
            }
        }

        [UIEventHandle("TotalMoney/Decrease")]
        void _OnMoneyDecreaesClicked()
        {
            m_nCurrentMoney--;
            if (_ModifyCurrentMoney())
            {
                RedPacketTable data = m_buyData[m_nCurrentMoney];
                string strIcon = ItemDataManager.GetInstance().GetCommonItemTableDataByID(data.CostMoneyID).Icon;
                // m_imgMoneyIcon.sprite = AssetLoader.instance.LoadRes(strIcon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref m_imgMoneyIcon, strIcon);
                // m_imgCostIcon.sprite = AssetLoader.instance.LoadRes(strIcon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref m_imgCostIcon, strIcon);
                // m_imgOwnedIcon.sprite = AssetLoader.instance.LoadRes(strIcon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref m_imgOwnedIcon, strIcon);
                m_labMoneyCount.text = data.TotalMoney.ToString();
                m_labNumCount.text = _ModifyCurrentNum(data) ? data.Num[m_nCurrentNum].ToString() : "0";
                m_labCostCount.text = data.TotalMoney.ToString();
                m_labOwnedCount.text = ItemDataManager.GetInstance().GetOwnedItemCount(data.CostMoneyID).ToString();
            }
        }

        [UIEventHandle("TotalMoney/Increase")]
        void _OnMoneyIncreaesClicked()
        {
            m_nCurrentMoney++;
            if (_ModifyCurrentMoney())
            {
                RedPacketTable data = m_buyData[m_nCurrentMoney];
                string strIcon = ItemDataManager.GetInstance().GetCommonItemTableDataByID(data.CostMoneyID).Icon;
                // m_imgMoneyIcon.sprite = AssetLoader.instance.LoadRes(strIcon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref m_imgMoneyIcon, strIcon);
                // m_imgCostIcon.sprite = AssetLoader.instance.LoadRes(strIcon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref m_imgCostIcon, strIcon);
                // m_imgOwnedIcon.sprite = AssetLoader.instance.LoadRes(strIcon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref m_imgOwnedIcon, strIcon);
                m_labMoneyCount.text = data.TotalMoney.ToString();
                m_labNumCount.text = _ModifyCurrentNum(data) ? data.Num[m_nCurrentNum].ToString() : "0";
                m_labCostCount.text = data.TotalMoney.ToString();
                m_labOwnedCount.text = ItemDataManager.GetInstance().GetOwnedItemCount(data.CostMoneyID).ToString();
            }
        }

        [UIEventHandle("TotalNum/Decrease")]
        void _OnNumDecreaesClicked()
        {
            m_nCurrentNum--;

            RedPacketTable data = m_buyData[m_nCurrentMoney];
            m_labNumCount.text = _ModifyCurrentNum(data) ? data.Num[m_nCurrentNum].ToString() : "0";
        }

        [UIEventHandle("TotalNum/Increase")]
        void _OnNumIncreaesClicked()
        {
            m_nCurrentNum++;

            RedPacketTable data = m_buyData[m_nCurrentMoney];
            m_labNumCount.text = _ModifyCurrentNum(data) ? data.Num[m_nCurrentNum].ToString() : "0";
        }

    }
}
