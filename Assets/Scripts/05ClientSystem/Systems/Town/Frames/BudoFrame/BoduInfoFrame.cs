using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Protocol;

namespace GameClient
{
    class BoduInfoFrame : ClientFrame
    {
        public static void Open(int jarID = 1)
        {
            var data = new BoduData();
            data.jarId = jarID;
            ClientSystemManager.GetInstance().OpenFrame<BoduInfoFrame>(FrameLayer.Middle, data);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Budo/BoduInfoFrame";
        }

        BoduData m_kData = null;
        Text m_kCostPre;
        GameObject goAwardParent;
        GameObject goAwardPrefab;
        Button btnAddParty;
        UIGray grayParty;

        protected override void _OnOpenFrame()
        {
            m_kData = userData as BoduData;
            m_kCostPre = Utility.FindComponent<Text>(frame, "PreCost");
            goAwardParent = Utility.FindChild(frame, "Jars");
            goAwardPrefab = Utility.FindChild(frame, "Jars/ItemParent");
            goAwardPrefab.CustomActive(false);
            btnAddParty = Utility.FindComponent<Button>(frame, "BtnAddParty");
            btnAddParty.onClick.RemoveAllListeners();
            btnAddParty.onClick.AddListener(_OnAddParty);
            grayParty = Utility.FindComponent<UIGray>(frame, "BtnAddParty");
            _SetData();

            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += OnRemoveItem;

            BudoManager.GetInstance().onBudoInfoChanged += _OnBudoInfoChanged;
        }

        void OnAddNewItem(List<Item> items)
        {
            _UpdateBoduInfo();
        }
        void OnRemoveItem(ItemData data)
        {
            _UpdateBoduInfo();
        }
        void OnUpdateItem(List<Item> items)
        {
            _UpdateBoduInfo();
        }

        void _OnBudoInfoChanged()
        {
            _UpdateBoduInfo();
        }

        void _OnAddParty()
        {
            if (TeamDataManager.GetInstance().HasTeam())
            {
                SystemNotifyManager.SystemNotify(1104);
                return;
            }

            if(ClientSystemManager.GetInstance().IsFrameOpen<PkSeekWaiting>())
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("budo_need_stop_match"));
                return;
            }

            if (BudoManager.GetInstance().CanParty)
            {
                if (!BudoManager.GetInstance().IsLevelFit)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(string.Format(TR.Value("budo_need_lv"),BudoManager.GetInstance().NeedLv));
                    return;
                }

                int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(GameClient.BudoManager.TicketID);
                if(iHasCount <= 0 && BudoManager.GetInstance().TotalTimes > 0)
                {
                    ItemComeLink.OnLink(GameClient.BudoManager.TicketID, 1, true, () =>
                    {
                        BudoManager.GetInstance().SendAddParty();
                        frameMgr.CloseFrame(this,true);
                    });
                    //SystemNotifyManager.SysNotifyTextAnimation(TR.Value("budo_ticket_not_enough"));
                    return;
                }
                BudoManager.GetInstance().SendAddParty();
            }
            else
            {
                BudoManager.GetInstance().GotoPvpBudo();
            }

            ActivityDungeonDataManager.GetInstance().mIsLimitActivityRedPoint = false;
           
            ClientSystemManager.GetInstance().CloseFrame<PkWaitingRoom>();
            frameMgr.CloseFrame(this);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshLimitTimeState);
        }

        protected override void _OnCloseFrame()
        {
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= OnRemoveItem;

            BudoManager.GetInstance().onBudoInfoChanged -= _OnBudoInfoChanged;

            if (btnAddParty != null)
            {
                btnAddParty.onClick.RemoveAllListeners();
                btnAddParty = null;
            }
        }

        void OnItemClicked(GameObject obj, ItemData item)
        {
            if(item != null)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }
        }

        void _SetData()
        {
            if (m_kData != null)
            {
                List<ProtoTable.BudoAwardTable> values = BudoManager.GetInstance().BudoJars;

                for (int i = 0; values != null && i < 5 && i < values.Count; ++i)
                {
                    var data = values[i];
                    var goLocal = GameObject.Instantiate(goAwardPrefab);
                    goLocal.CustomActive(true);
                    Utility.AttachTo(goLocal, goAwardParent);
                    ComItem comItem = CreateComItem(goLocal);
                    var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(data.ID);
                    comItem.Setup(itemData, OnItemClicked);

                    Text hint = Utility.FindComponent<Text>(goLocal, "Hint");
                    hint.text = data.desc;

                    Text name = Utility.FindComponent<Text>(goLocal, "Name");
                    if (itemData != null)
                    {
                        name.text = itemData.GetColorName();
                    }
                }

                _UpdateBoduInfo();
            }
        }

        void _UpdateBoduInfo()
        {
            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(GameClient.BudoManager.TicketID);
            if (iHasCount > 0)
            {
                m_kCostPre.text = string.Format(TR.Value("budo_ticket_hint_ok"), iHasCount);
            }
            else
            {
                m_kCostPre.text = string.Format(TR.Value("budo_ticket_hint_failed"), iHasCount);
            }

            if(BudoManager.GetInstance().TotalTimes <= 0)
            {
                m_kCostPre.text = string.Format(TR.Value("budo_free_hint"), iHasCount);
            }

            // (BudoManager.GetInstance().TotalTimes <= 0 || iHasCount > 0) && BudoManager.GetInstance().CanParty ||
            //BudoManager.GetInstance().CanAcqured;
            bool bCanParty = BudoManager.GetInstance().IsOpen;
            grayParty.enabled = !bCanParty;
            btnAddParty.enabled = bCanParty;
        }

        [UIEventHandle("Close")]
        void OnClickClose()
        {
            frameMgr.CloseFrame(this);
        }
    }
}