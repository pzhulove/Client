using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;

namespace GameClient
{
    class SelectItemFrame : ClientFrame
    {
        [UIControl("Name")]
        Text m_labName;

        [UIControl("Items")]
        ComUIListScript m_comItemList;

        [UIControl("Ok")]
        Button m_btnOk;

        [UIControl("Ok")]
        ComButtonEnbale m_comBtnOkEnable;

        [UIControl("Tip")]
        Text m_labCountTip;

        [UIControl("Ok")]
        UIGray uiGray;

        [UIControl("SelectInfo")]
        Text txtSelectInfo;

        ItemData m_giftItem = null;
        ProtoTable.GiftPackTable m_giftPackTable = null;
        List<ItemData> m_arrItems = new List<ItemData>();

        List<int> m_curSelectIndexs = new List<int>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Tip/SelectItem";
        }

        protected List<GiftBagItem> m_akGifgBagItemList = null;

        public GiftBagItem Create(GameObject parent)
        {
            if (parent == null)
            {
                Logger.LogError("SelectItemFrame Create function param parent is null!");
                return null;
            }

            GameObject item = CGameObjectPool.instance.GetGameObject("UIFlatten/Prefabs/Tip/GiftBagItem", enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);
            if (item != null)
            {
                GiftBagItem comItem = item.GetComponent<GiftBagItem>();
                if (comItem != null)
                {                    
                    comItem.gameObject.transform.SetParent(parent.transform, false);             
                    return comItem;
                }
            }
            return null;
        }

        public void Destroy(GiftBagItem a_comItem)
        {
            if (a_comItem != null && a_comItem.gameObject != null)
            {         
                if (true)
                {
                    CGameObjectPool.instance.RecycleGameObject(a_comItem.gameObject);                 
                }
            }
        }

        private GiftBagItem CreateGiftBagItem(GameObject goParent)
        {
            GiftBagItem giftBagItem = Create(goParent);
            if (giftBagItem != null)
            {
                if (m_akGifgBagItemList == null)
                {
                    m_akGifgBagItemList = new List<GiftBagItem>();
                }
                giftBagItem.Index = m_akGifgBagItemList.Count;
                m_akGifgBagItemList.Add(giftBagItem);
            }

            return giftBagItem;
        }

        void DestroyGifgBagItems()
        {
            if (m_akGifgBagItemList != null)
            {
                for (int i = 0; i < m_akGifgBagItemList.Count; ++i)
                {
                    if (m_akGifgBagItemList[i] != null)
                    {
                        m_akGifgBagItemList[i].IsSelect = false;
                        Destroy(m_akGifgBagItemList[i]);
                    }
                }
                m_akGifgBagItemList.Clear();
            }
        } 

        protected override void _OnOpenFrame()
        {
            if (uiGray == null)
            {
                uiGray = m_btnOk.GetComponent<UIGray>();
                if(uiGray != null)
                {
                    uiGray.bEnabled2Text = false;
                }
            }

            m_giftItem = userData as ItemData;
            if (m_giftItem == null)
            {
                Logger.LogError("open SelectItemFrame, user data is invalid!!");
                return;
            }

            GiftPackTable giftPackTable = TableManager.GetInstance().GetTableItem<GiftPackTable>(m_giftItem.PackID);

            if (giftPackTable != null && giftPackTable.UIType == 1/* || true*/)
            {
                ItemData itemdata = ItemDataManager.GetInstance().GetItem(m_giftItem.GUID);
                this.SetVisible(false);
                InvokeMethod.Invoke(0.5f, () =>
                {
                    ClientSystemManager.instance.CloseFrame(this, true);
                });
                
                ClientSystemManager.GetInstance().OpenFrame<SelectItemFrameEx>(FrameLayer.Middle, itemdata);            
                return;
            }


            List<GiftTable> arrGifts = m_giftItem.GetGifts();
            if (arrGifts == null || arrGifts.Count <= 0)
            {
                Logger.LogErrorFormat("礼包{0}不包含任何道具，请检查礼包表", m_giftItem.TableID);
                return;
            }         

            for (int i = 0; i < arrGifts.Count; ++i)
            {
                if (arrGifts[i].Levels.Count > 0)
                {
                    if (PlayerBaseData.GetInstance().Level < arrGifts[i].Levels[0] || PlayerBaseData.GetInstance().Level > arrGifts[i].Levels[1])
                    {
                        continue;
                    }
                }

                ItemData item = ItemDataManager.CreateItemDataFromTable(arrGifts[i].ItemID,100,arrGifts[i].Strengthen);
                if (item != null)
                {
                    item.Count = arrGifts[i].ItemCount;
                    m_arrItems.Add(item);
                }
            }


            if (m_labName != null)
                m_labName.text = m_giftItem.Name;
            

            m_giftPackTable = TableManager.GetInstance().GetTableItem<GiftPackTable>(m_giftItem.PackID);
            if(m_giftPackTable == null)
                return;

            //m_giftPackTable.FilterCount = 3;
            if (m_labCountTip != null)
            {
                m_labCountTip.text = TR.Value("gift_select_item_tip", m_giftPackTable.FilterCount);
            }

            if (txtSelectInfo != null)
            {
                if (_GetSelectItemsCount() == m_giftPackTable.FilterCount)
                {
                    txtSelectInfo.text = TR.Value("select_ok");
                }
                else
                {
                    txtSelectInfo.text = TR.Value("select_info", _GetSelectItemsCount(), m_giftPackTable.FilterCount);
                }
            }
            
            if (m_comItemList != null)
            {
                m_comItemList.Initialize();

                // 策划要求道具个数小于等于5个时将元素居中显示，这里做个偏移处理就OK了
                //if (m_arrItems.Count <= 5)
                //{
                //    m_comItemList.m_elementPadding.y = 70;
                //}

                m_comItemList.onBindItem = var =>
                {
                    GiftBagItem item = CreateGiftBagItem(Utility.FindGameObject(var, "Item"));
                    if (item != null)
                        item.IsSelect = false;
                    return item;
                };

                m_comItemList.onItemVisiable = var =>
                {
                    if (var != null && var.m_index >= 0 && var.m_index < m_arrItems.Count)
                    {
                        if (m_curSelectIndexs.Contains(var.m_index))
                        {
                            (var.gameObjectBindScript as GiftBagItem).IsSelect = true;
                        }
                        else
                        {
                            (var.gameObjectBindScript as GiftBagItem).IsSelect = false;
                        }

                        GiftBagItem comItem = var.gameObjectBindScript as GiftBagItem;
                        comItem.Setup(var.m_index, m_arrItems[var.m_index], (var1, var2) =>
                        {
                            if (m_giftPackTable.FilterCount <= 0)
                            {
                                return;
                            }

                            SelectItemFrame obj = this;

                            if (m_giftPackTable.FilterCount == 1)
                            {
                                for (int i = 0; i < m_arrItems.Count; ++i)
                                {
                                    m_arrItems[i].IsSelected = false;
                                }

                                for (int i = 0; i < m_akGifgBagItemList.Count; i++)
                                {
                                    m_akGifgBagItemList[i].IsSelect = false;
                                }

                                var2.IsSelected = true;

                                m_curSelectIndexs.Clear();
                                GiftBagItem item1 = var.gameObjectBindScript as GiftBagItem;
                                if (item1 != null)
                                {
                                    item1.IsSelect = !item1.IsSelect;
                                    if (item1.IsSelect)
                                    {
                                        m_curSelectIndexs.Add(var.m_index);
                                    }
                                }

                                m_comBtnOkEnable.SetEnable(_GetSelectItemsCount() == m_giftPackTable.FilterCount);

                                {
                                    //UIGray uiGray = m_btnOk.GetComponent<UIGray>();
                                    if (uiGray != null)
                                    {
                                        uiGray.SetEnable(!(_GetSelectItemsCount() == m_giftPackTable.FilterCount));
                                    }
                                }
                                m_comItemList.SetElementAmount(m_arrItems.Count);

                                //Text txtSelectInfo = m_btnOk.GetComponentInChildren<Text>();
                                if (txtSelectInfo != null)
                                {
                                    if (_GetSelectItemsCount() == m_giftPackTable.FilterCount)
                                    {
                                        txtSelectInfo.text = TR.Value("select_ok");
                                    }
                                    else
                                    {
                                        txtSelectInfo.text = TR.Value("select_info", _GetSelectItemsCount(), m_giftPackTable.FilterCount);
                                    }
                                }

                            }
                            else
                            {
                                var2.IsSelected = !var2.IsSelected;
                                GiftBagItem item1 = var.gameObjectBindScript as GiftBagItem;
                                if (item1 != null)
                                {
                                    item1.IsSelect = !item1.IsSelect;
                                    if (item1.IsSelect)
                                    {
                                        m_curSelectIndexs.Add(var.m_index);
                                    }
                                    else
                                    {
                                        m_curSelectIndexs.Remove(var.m_index);
                                    }
                                }

                                if (_GetSelectItemsCount() > m_giftPackTable.FilterCount)
                                {
                                    var2.IsSelected = !var2.IsSelected;
                                    if (item1 != null)
                                    {
                                        item1.IsSelect = !item1.IsSelect;
                                        if (item1.IsSelect)
                                        {
                                            m_curSelectIndexs.Add(var.m_index);
                                        }
                                        else
                                        {
                                            m_curSelectIndexs.Remove(var.m_index);
                                        }
                                    }

                                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("gift_select_item_max_count"));
                                    return;
                                }

                                m_comBtnOkEnable.SetEnable(_GetSelectItemsCount() == m_giftPackTable.FilterCount);

                                //UIGray uiGray = m_btnOk.GetComponent<UIGray>();
                                if (uiGray != null)
                                {
                                    uiGray.SetEnable(!(_GetSelectItemsCount() == m_giftPackTable.FilterCount));
                                }

                                //Text txtSelectInfo = m_btnOk.GetComponentInChildren<Text>();
                                if (txtSelectInfo != null)
                                {
                                    if (_GetSelectItemsCount() == m_giftPackTable.FilterCount)
                                    {
                                        txtSelectInfo.text = TR.Value("select_ok");
                                    }
                                    else
                                    {
                                        txtSelectInfo.text = TR.Value("select_info", _GetSelectItemsCount(), m_giftPackTable.FilterCount);
                                    }
                                }
                            }
                        });
                    }
                };

                //DestroyGifgBagItems();
                m_comItemList.SetElementAmount(m_arrItems.Count);

            }

            if (m_btnOk != null)
            {
                m_btnOk.onClick.RemoveAllListeners();
                m_btnOk.onClick.AddListener(() =>
                {
                    if (_GetSelectItemsCount() == m_giftPackTable.FilterCount)
                    {
                        int nHigh, nLow;
                        _GetSelectItemsMask(out nHigh, out nLow);
                        ItemDataManager.GetInstance().UseItem(m_giftItem, false, nLow, nHigh);

                        if (m_giftItem.Count <= 1 || m_giftItem.CD > 0)
                        {
                            ItemTipManager.GetInstance().CloseAll();
                        }

                        frameMgr.CloseFrame(this);
                    }
                });
            }

            if (m_comBtnOkEnable != null)
                m_comBtnOkEnable.SetEnable(false);
            
            //UIGray uiGray = m_btnOk.GetComponent<UIGray>();
            if (uiGray != null)
            {
                uiGray.SetEnable(true);
            }
        }

        protected override void _OnCloseFrame()
        {
            m_giftItem = null;
            m_giftPackTable = null;
            m_arrItems.Clear();
            m_curSelectIndexs.Clear();

            DestroyGifgBagItems();
        }

        void _GetSelectItemsMask(out int a_nHith, out int a_nLow)
        {
            a_nHith = 0;
            a_nLow = 0;
            for (int i = 0; i < m_arrItems.Count; ++i)
            {
                if (m_arrItems[i].IsSelected)
                {
                    if (i >= 0 && i < 32)
                    {
                        a_nLow |= (1 << i);
                    }
                    else
                    {
                        a_nHith |= (1 << (i - 32));
                    }
                }
            }
        }

        int _GetSelectItemsCount()
        {
            int nCount = 0;
            for (int i = 0; i < m_arrItems.Count; ++i)
            {
                if (m_arrItems[i].IsSelected)
                {
                    nCount++;
                }
            }
            return nCount;
        }

        #region ExtraUIBind
        private ButtonEx mClose = null;

        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<ButtonEx>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
