using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    class GuildWelfareFrame : ClientFrame
    {
        [UIObject("Record/Content")]
        GameObject m_objRecordRoot;

        [UIObject("Record/Content/Template")]
        GameObject m_objRecordTemplate;

        [UIControl("DonateWithTicket/Contribution/Count")]
        Text m_labDonateTicketGet;

        [UIControl("DonateWithTicket/Cost/Item/Count")]
        Text m_labDonateTicketCostCount;

        [UIControl("DonateWithTicket/Cost/Item/Icon")]
        Image m_imgDonateTicketCostIcon;

        [UIControl("DonateWithTicket/Times/Image/Text")]
        Text m_labDonateTicketTimes;

        [UIControl("DonateWithTicket/Donate/Remain")]
        Text m_labDonateTicketRemainTimes;

        [UIControl("DonateWithGold/Contribution/Count")]
        Text m_labDonateGoldGet;

        [UIControl("DonateWithGold/Cost/Item/Count")]
        Text m_labDonateGoldCostCount;

        [UIControl("DonateWithGold/Cost/Item/Icon")]
        Image m_imgDonateGoldCostIcon;

        [UIControl("DonateWithGold/Times/Image/Text")]
        Text m_labDonateGoldTimes;

        [UIControl("DonateWithGold/Donate/Remain")]
        Text m_labDonateGoldRemainTimes;

        [UIControl("Exchange/Remain")]
        Text m_labExchangeRemainTimes;

        [UIObject("Exchange/Item")]
        GameObject m_objExchangeItemRoot;

        [UIControl("Exchange/ItemName")]
        Text m_labExchangeItemName;

        [UIControl("Exchange/Doit/CostDesc/Icon")]
        Image m_imgExchangeCostIcon;

        [UIControl("Exchange/Doit/CostDesc/Count")]
        Text m_imgExchangeCostCount;

        [UIControl("Exchange/Title/CD")]
        Text m_labExchangeCD;

        [UIControl("Exchange/Doit")]
        ComButtonEnbale m_comBtnExchange;

        [UIObject("Exchange/Doit/RedPoint")]
        GameObject m_objRedPoint;

        int m_nSelectGoldTimes = 0;
        int m_nSelectTicketTimes = 0;

        DelayCallUnitHandle m_delayCallUnit;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildWelfare";
        }

        protected override void _OnOpenFrame()
        {
            if (GuildDataManager.GetInstance().HasSelfGuild() == false)
            {
                return;
            }

            int nBuildingLevel = GuildDataManager.GetInstance().GetBuildingLevel(Protocol.GuildBuildingType.WELFARE);
            {
                m_objRecordTemplate.SetActive(false);
                m_labDonateGoldGet.text = GuildDataManager.GetInstance().donateGoldGet.ToString();
                m_labDonateGoldCostCount.text = GuildDataManager.GetInstance().donateGoldCost.ToString();
                ItemData goldData = ItemDataManager.GetInstance().GetMoneyTableDataByType(ProtoTable.ItemTable.eSubType.BindGOLD);
                // m_imgDonateGoldCostIcon.sprite = AssetLoader.instance.LoadRes(goldData.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref m_imgDonateGoldCostIcon, goldData.Icon);

                int nMaxTime = _GetGoldDonateMaxTimes();
                int nRemainTime = _GetGoldDonateRemainTimes();
                int nTimes = m_nSelectGoldTimes * 5;
                if (nTimes < 1)
                {
                    nTimes = 1;
                }
                if (nTimes > nRemainTime)
                {
                    nTimes = nRemainTime;
                }
                m_labDonateGoldTimes.text = nTimes.ToString();
                m_labDonateGoldRemainTimes.text = TR.Value("guild_remain_times", nRemainTime, nMaxTime);
            }

            {
                m_labDonateTicketGet.text = GuildDataManager.GetInstance().donatePointGet.ToString();
                m_labDonateTicketCostCount.text = GuildDataManager.GetInstance().donatePointCost.ToString();
                ItemData ticketData = ItemDataManager.GetInstance().GetMoneyTableDataByType(ProtoTable.ItemTable.eSubType.BindPOINT);
                // m_imgDonateTicketCostIcon.sprite = AssetLoader.instance.LoadRes(ticketData.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref m_imgDonateTicketCostIcon, ticketData.Icon);

                int nMaxTime = _GetTicketDonateMaxTimes();
                int nRemainTime = _GetTicketDonateRemainTimes();
                int nTimes = m_nSelectTicketTimes * 5;
                if (nTimes < 1)
                {
                    nTimes = 1;
                }
                if (nTimes > nRemainTime)
                {
                    nTimes = nRemainTime;
                }
                m_labDonateTicketTimes.text = nTimes.ToString();
                m_labDonateTicketRemainTimes.text = TR.Value("guild_remain_times", nRemainTime, nMaxTime);
            }

            {
                int nMaxTime = _GetExchangeMaxTimes();
                int nRemainTime = _GetExchangeRemainTimes();
                m_labExchangeRemainTimes.text = TR.Value("guild_remain_times", nRemainTime, nMaxTime);

                _UpdateExchangeItem();

                ItemData guildContribution = ItemDataManager.GetInstance().GetMoneyTableDataByType(ProtoTable.ItemTable.eSubType.GuildContri);
                // m_imgExchangeCostIcon.sprite = AssetLoader.instance.LoadRes(guildContribution.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref m_imgExchangeCostIcon, guildContribution.Icon);
                int nCost = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_EXCHANGE_COST).Value;
                m_imgExchangeCostCount.text = nCost.ToString();
            }

            _UpdateExchangeCD();
            _UpdateExchangeRedPoint();
            _TryStartExchangeCDCounter();

            GuildDataManager.GetInstance().RequsetDonateLog();

            _RegisterUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            _UnRegisterUIEvent();
            ClientSystemManager.GetInstance().delayCaller.StopItem(m_delayCallUnit);
            m_nSelectGoldTimes = 0;
            m_nSelectTicketTimes = 0;
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildRequestDonateLogSuccess, _OnRecordListUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDonateSuccess, _OnDonateSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildExchangeSuccess, _OnExchangeSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildUpgradeBuildingSuccess, _OnBuildingUpgradeSuccess);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildRequestDonateLogSuccess, _OnRecordListUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDonateSuccess, _OnDonateSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildExchangeSuccess, _OnExchangeSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildUpgradeBuildingSuccess, _OnBuildingUpgradeSuccess);
        }

        void _TryStartExchangeCDCounter()
        {
            uint nTimeCool = GuildDataManager.GetInstance().myGuild.nExchangeCoolTime;
            uint nCurrentTime = TimeManager.GetInstance().GetServerTime();
            if (nTimeCool > nCurrentTime)
            {
                ClientSystemManager.GetInstance().delayCaller.StopItem(m_delayCallUnit);
                m_delayCallUnit = ClientSystemManager.GetInstance().delayCaller.RepeatCall(1000, _UpdateExchangeCD);
            }
        }

        void _UpdateExchangeItem()
        {
            int nLevel = GuildDataManager.GetInstance().GetBuildingLevel(Protocol.GuildBuildingType.WELFARE);
            ProtoTable.GuildBuildingTable table = TableManager.GetInstance().GetTableItem<ProtoTable.GuildBuildingTable>(nLevel);
            if (table != null)
            {
                ComItem comItem = m_objExchangeItemRoot.GetComponentInChildren<ComItem>();
                if (comItem == null)
                {
                    comItem = CreateComItem(m_objExchangeItemRoot);
                }
                ItemData item = ItemDataManager.GetInstance().GetCommonItemTableDataByID(table.WelfareGiftId);
                comItem.Setup(
                    item,
                    (GameObject a_obj, ItemData a_item) => { ItemTipManager.GetInstance().ShowTip(a_item); }
                    );

                m_labExchangeItemName.text = item.GetColorName();
            }
        }

        void _UpdateExchangeCD()
        {
            uint nTimeCool = GuildDataManager.GetInstance().myGuild.nExchangeCoolTime;
            uint nCurrentTime = TimeManager.GetInstance().GetServerTime();
            if (nTimeCool > nCurrentTime)
            {
                uint nTimeLeft = nTimeCool - nCurrentTime;

                uint second = 0;
                uint minute = 0;
                uint hour = 0;
                second = nTimeLeft % 60;
                uint temp = nTimeLeft / 60;
                if (temp > 0)
                {
                    minute = temp % 60;
                    hour = temp / 60;
                }

                m_labExchangeCD.gameObject.SetActive(true);
                m_labExchangeCD.text = string.Format("{0:00}:{1:00}:{2:00}", hour, minute, second);
                m_comBtnExchange.SetEnable(false);
            }
            else
            {
                m_labExchangeCD.gameObject.SetActive(false);
                m_comBtnExchange.SetEnable(true);
                ClientSystemManager.GetInstance().delayCaller.StopItem(m_delayCallUnit);
                _UpdateExchangeRedPoint();
                RedPointDataManager.GetInstance().NotifyRedPointChanged();
            }
        }

        void _UpdateExchangeRedPoint()
        {
            m_objRedPoint.SetActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildExchange));
        }

        int _GetTicketDonateRemainTimes()
        {
            int nMaxTime = (int)Utility.GetCurVipLevelPrivilegeData(VipPrivilegeTable.eType.GUILD_TICKET_DONATE_DAILY);
            int nUsedTime = CountDataManager.GetInstance().GetCount("guild_donate_point");
            Logger.LogProcessFormat("guild -> count:guild_donate_point {0}", nUsedTime);
            int nRemainTime = nMaxTime - nUsedTime;
            if (nRemainTime < 0)
            {
                nRemainTime = 0;
            }
            return nRemainTime;
        }

        int _GetGoldDonateRemainTimes()
        {
            //int nMaxTime = TableManager.GetInstance().GetTableItem<ProtoTable.VipTable>(PlayerBaseData.GetInstance().VipLevel).PrivilegeData_19;
            int nMaxTime = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_GOLD_DONATE).Value;
            int nUsedTime = CountDataManager.GetInstance().GetCount("guild_donate_gold");
            Logger.LogProcessFormat("guild -> count:guild_donate_gold {0}", nUsedTime);
            int nRemainTime = nMaxTime - nUsedTime;
            if (nRemainTime < 0)
            {
                nRemainTime = 0;
            }
            return nRemainTime;
        }

        int _GetExchangeRemainTimes()
        {
            int nMaxTime = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_EXCHANGE_MAX_TIME).Value;
            int nUsedTime = CountDataManager.GetInstance().GetCount("guild_exchange");
            int nRemainTime = nMaxTime - nUsedTime;
            if (nRemainTime < 0)
            {
                nRemainTime = 0;
            }
            return nRemainTime;
        }

        int _GetTicketDonateMaxTimes()
        {
            return (int)Utility.GetCurVipLevelPrivilegeData(VipPrivilegeTable.eType.GUILD_TICKET_DONATE_DAILY);
        }

        int _GetGoldDonateMaxTimes()
        {
            //return TableManager.GetInstance().GetTableItem<ProtoTable.VipTable>(PlayerBaseData.GetInstance().VipLevel).PrivilegeData_19;
            return TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_GOLD_DONATE).Value;
        }

        int _GetExchangeMaxTimes()
        {
            return TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_EXCHANGE_MAX_TIME).Value;
        }

        void _OnRecordListUpdate(UIEvent a_event)
        {
            List<GuildDonateData> arrDonateData = a_event.Param1 as List<GuildDonateData>;
            for (int i = 0; i < arrDonateData.Count; ++i)
            {
                GameObject obj = GameObject.Instantiate(m_objRecordTemplate);
                obj.transform.SetParent(m_objRecordRoot.transform, false);
                obj.SetActive(true);

                GuildDonateData data = arrDonateData[i];

                Text lab = obj.GetComponent<Text>();
                lab.text = TR.Value("guild_donate_record", data.strName, data.nTimes, data.nFund);
            }
        }

        void _OnDonateSuccess(UIEvent a_event)
        {
            GuildDonateData data = a_event.Param1 as GuildDonateData;
            if (data != null)
            {
                GameObject obj = null;

                if (m_objRecordRoot.transform.childCount >= 12)
                {
                    obj = m_objRecordRoot.transform.GetChild(1).gameObject;
                    obj.transform.SetAsLastSibling();
                }
                else
                {
                    obj = GameObject.Instantiate(m_objRecordTemplate);
                    obj.transform.SetParent(m_objRecordRoot.transform, false);
                    obj.SetActive(true);
                }
                Text lab = obj.GetComponent<Text>();
                lab.text = TR.Value("guild_donate_record", data.strName, data.nTimes, data.nFund);

                if (data.eType == Protocol.GuildDonateType.GOLD)
                {
                    m_labDonateGoldRemainTimes.text = TR.Value("guild_remain_times", _GetGoldDonateRemainTimes(), _GetGoldDonateMaxTimes());
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_donate_gold_success", data.nTimes, data.nFund));
                }
                else
                {
                    m_labDonateTicketRemainTimes.text = TR.Value("guild_remain_times", _GetTicketDonateRemainTimes(), _GetTicketDonateMaxTimes());
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_donate_ticket_success", data.nTimes, data.nFund));
                }
            }
        }

        void _OnExchangeSuccess(UIEvent a_event)
        {
            m_labExchangeRemainTimes.text = TR.Value("guild_remain_times", _GetExchangeRemainTimes(), _GetExchangeMaxTimes());
            _UpdateExchangeCD();
            _UpdateExchangeRedPoint();
            _TryStartExchangeCDCounter();
            RedPointDataManager.GetInstance().NotifyRedPointChanged();
        }

        void _OnGuildDismissed(UIEvent a_event)
        {
            frameMgr.CloseFrame(this);
        }

        void _OnBuildingUpgradeSuccess(UIEvent a_event)
        {
            _UpdateExchangeItem();
        }

        [UIEventHandle("DonateWithTicket/Times/Decrease")]
        void _OnTicketTimesDecreaseClicked()
        {
            int nRemainTime = _GetTicketDonateRemainTimes();

            m_nSelectTicketTimes--;
            if (m_nSelectTicketTimes < 0)
            {
                m_nSelectTicketTimes = 0;
            }

            int nTimes = m_nSelectTicketTimes * 5;
            if (nTimes < 1)
            {
                nTimes = 1;
            }
            if (nTimes > nRemainTime)
            {
                nTimes = nRemainTime;
            }

            m_labDonateTicketTimes.text = nTimes.ToString();
            if (nTimes > 0)
            {
                m_labDonateTicketGet.text = (GuildDataManager.GetInstance().donatePointGet * nTimes).ToString();
                m_labDonateTicketCostCount.text = (GuildDataManager.GetInstance().donatePointCost * nTimes).ToString();
            }
        }

        [UIEventHandle("DonateWithTicket/Times/Increase")]
        void _OnTicketTimesIncreaseClicked()
        {
            int nRemainTime = _GetTicketDonateRemainTimes();

            int nMax = nRemainTime / 5;
            if (nRemainTime % 5 > 0)
            {
                nMax++;
            }
            m_nSelectTicketTimes++;
            if (m_nSelectTicketTimes > nMax)
            {
                m_nSelectTicketTimes = nMax;
            }

            int nTimes = m_nSelectTicketTimes * 5;
            if (nTimes < 1)
            {
                nTimes = 1;
            }
            if (nTimes > nRemainTime)
            {
                nTimes = nRemainTime;
            }

            m_labDonateTicketTimes.text = nTimes.ToString();
            if (nTimes > 0)
            {
                m_labDonateTicketGet.text = (GuildDataManager.GetInstance().donatePointGet * nTimes).ToString();
                m_labDonateTicketCostCount.text = (GuildDataManager.GetInstance().donatePointCost * nTimes).ToString();
            }
        }

        [UIEventHandle("DonateWithGold/Times/Decrease")]
        void _OnGoldTimesDecreaseClicked()
        {
            int nRemainTime = _GetGoldDonateRemainTimes();

            m_nSelectGoldTimes--;
            if (m_nSelectGoldTimes < 0)
            {
                m_nSelectGoldTimes = 0;
            }

            int nTimes = m_nSelectGoldTimes * 5;
            if (nTimes < 1)
            {
                nTimes = 1;
            }
            if (nTimes > nRemainTime)
            {
                nTimes = nRemainTime;
            }

            m_labDonateGoldTimes.text = nTimes.ToString();
            if (nTimes > 0)
            {
                m_labDonateGoldGet.text = (GuildDataManager.GetInstance().donateGoldGet * nTimes).ToString();
                m_labDonateGoldCostCount.text = (GuildDataManager.GetInstance().donateGoldCost * nTimes).ToString();
            }
        }

        [UIEventHandle("DonateWithGold/Times/Increase")]
        void _OnGoldTimesIncreaseClicked()
        {
            int nRemainTime = _GetGoldDonateRemainTimes();

            int nMax = nRemainTime / 5;
            if (nRemainTime % 5 > 0)
            {
                nMax++;
            }
            m_nSelectGoldTimes++;
            if (m_nSelectGoldTimes > nMax)
            {
                m_nSelectGoldTimes = nMax;
            }

            int nTimes = m_nSelectGoldTimes * 5;
            if (nTimes < 1)
            {
                nTimes = 1;
            }
            if (nTimes > nRemainTime)
            {
                nTimes = nRemainTime;
            }

            m_labDonateGoldTimes.text = nTimes.ToString();
            if (nTimes > 0)
            {
                m_labDonateGoldGet.text = (GuildDataManager.GetInstance().donateGoldGet * nTimes).ToString();
                m_labDonateGoldCostCount.text = (GuildDataManager.GetInstance().donateGoldCost * nTimes).ToString();
            }
        }

        [UIEventHandle("DonateWithGold/Donate")]
        void _OnDonateWithGold()
        {
            int nRemainTime = _GetGoldDonateRemainTimes();
            int nTimes = int.Parse(m_labDonateGoldTimes.text);
            if (nTimes > 0 && nTimes <= nRemainTime)
            {
                CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindGOLD);
                costInfo.nCount = GuildDataManager.GetInstance().donateGoldCost * nTimes;
                CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                {
                    GuildDataManager.GetInstance().Donate(Protocol.GuildDonateType.GOLD, nTimes);
                });
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_donate_times_invalid"));
            }
        }

        [UIEventHandle("DonateWithTicket/Donate")]
        void _OnDonateWithTicket()
        {
            int nRemainTime = _GetTicketDonateRemainTimes();
            int nTimes = int.Parse(m_labDonateTicketTimes.text);
            if (nTimes > 0 && nTimes <= nRemainTime)
            {
                CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindPOINT);
                costInfo.nCount = GuildDataManager.GetInstance().donatePointCost * nTimes;
                CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                {
                    GuildDataManager.GetInstance().Donate(Protocol.GuildDonateType.POINT, nTimes);
                });
            }
            else
            {
                SystemNotifyManager.SystemNotify(1000049, ()=> {
                    var frame = ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle) as VipFrame;
                    frame.OpenPayTab();
                });
            }
        }

        [UIEventHandle("Exchange/Doit")]
        void _OnExchangeClicked()
        {
            if (_GetExchangeRemainTimes() > 0)
            {
                if (PlayerBaseData.GetInstance().guildContribution < GuildDataManager.GetInstance().exchangeCost)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_less_contribution"));
                }
                else
                {
                    GuildDataManager.GetInstance().Exchange();
                }
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_less_exchange_times"));
            }
        }
    }
}
