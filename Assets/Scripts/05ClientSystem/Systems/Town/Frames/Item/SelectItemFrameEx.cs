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
    class SelectItemFrameEx : ClientFrame
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

        [UIControl("Ok/SelectInfo")]
        Text txtSelectInfo2;

        [UIControl("Ok/GetGiftText")]
        Image imgGetGiftText;

        List<int> m_curSelectIndexs = new List<int>();

        ItemData m_giftItem = null;
        ProtoTable.GiftPackTable m_giftPackTable = null;
        List<ItemData> m_arrItems = new List<ItemData>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Tip/SelectItemEx";
        }

        protected List<GiftBagItemEx> m_akGifgBagItemList = null;

        public GiftBagItemEx Create(GameObject parent)
        {
            if (parent == null)
            {
                Logger.LogError("SelectItemFrameEx Create function param parent is null!");
                return null;
            }

            GameObject item = CGameObjectPool.instance.GetGameObject("UIFlatten/Prefabs/Tip/GiftBagItemEx", enResourceType.UIPrefab, (uint)GameObjectPoolFlag.ReserveLast);
            if (item != null)
            {
                GiftBagItemEx comItem = item.GetComponent<GiftBagItemEx>();
                if (comItem != null)
                {                    
                    comItem.gameObject.transform.SetParent(parent.transform, false);             
                    return comItem;
                }
            }
            return null;
        }

        public void Destroy(GiftBagItemEx a_comItem)
        {
            if (a_comItem != null && a_comItem.gameObject != null)
            {         
                if (true)
                {
                    CGameObjectPool.instance.RecycleGameObject(a_comItem.gameObject);                 
                }
            }
        }

        private GiftBagItemEx CreateGiftBagItem(GameObject goParent)
        {
            GiftBagItemEx giftBagItem = Create(goParent);
            if (giftBagItem != null)
            {
                if (m_akGifgBagItemList == null)
                {
                    m_akGifgBagItemList = new List<GiftBagItemEx>();
                }
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
//             if (uiGray == null)
//             {
//                 uiGray = m_btnOk.GetComponent<UIGray>();
//                 if (uiGray != null)
//                 {
//                     uiGray.bEnabled2Text = false;
//                 }
//


            if(imgGetGiftText != null)
            {
                imgGetGiftText.CustomActive(false);
            }
            m_giftItem = userData as ItemData;
            if (m_giftItem == null)
            {
                Logger.LogError("open SelectItemFrame, user data is invalid!!");
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

                ItemData item = ItemDataManager.CreateItemDataFromTable(arrGifts[i].ItemID);
                Assert.IsNotNull(item);
                item.Count = arrGifts[i].ItemCount;
                m_arrItems.Add(item);
            }



            m_labName.text = m_giftItem.Name;

            m_giftPackTable = TableManager.GetInstance().GetTableItem<GiftPackTable>(m_giftItem.PackID);
            Assert.IsNotNull(m_giftPackTable);

            //m_giftPackTable.FilterCount = 3;

            //m_labCountTip.text = TR.Value("gift_select_item_tip", m_giftPackTable.FilterCount);

            {
                //Text txtSelectInfo = m_btnOk.GetComponentInChildren<Text>();
                if (txtSelectInfo != null)
                {                 
                    txtSelectInfo.text = string.Format("{0}选{1}", m_arrItems.Count, m_giftPackTable.FilterCount);
                }

                //                 if (txtSelectInfo2 != null)
                //                 { 
                //                     txtSelectInfo2.text = string.Format("（{0}/{1}）", _GetSelectItemsCount(), m_giftPackTable.FilterCount);
                //                 }

                if (txtSelectInfo2 != null)
                {
                    if (_GetSelectItemsCount() == m_giftPackTable.FilterCount)
                    {
                        if (imgGetGiftText != null)
                        {
                            imgGetGiftText.CustomActive(true);
                        }

                        txtSelectInfo2.text = "";
                    }
                    else
                    {
                        txtSelectInfo2.text = TR.Value("select_info", _GetSelectItemsCount(), m_giftPackTable.FilterCount);

                        if (imgGetGiftText != null)
                        {
                            imgGetGiftText.CustomActive(false);
                        }
                    }
                }
            }          

            m_comItemList.Initialize();
            m_comItemList.onBindItem = var =>
            {
                GiftBagItemEx item = CreateGiftBagItem(Utility.FindGameObject(var, "Item"));
                item.IsSelect = false;
                return item;
            };

            m_comItemList.onItemVisiable = var =>
            {
                if (var.m_index >= 0 && var.m_index < m_arrItems.Count)
                {
                    if (m_curSelectIndexs.Contains(var.m_index))
                    {
                        (var.gameObjectBindScript as GiftBagItemEx).IsSelect = true;
                    }
                    else
                    {
                        (var.gameObjectBindScript as GiftBagItemEx).IsSelect = false;
                    }

                    GiftBagItemEx comItem = var.gameObjectBindScript as GiftBagItemEx;
                    comItem.Setup(var.m_index, m_arrItems[var.m_index], (var1, var2) =>
                    {
                        if (m_giftPackTable.FilterCount <= 0)
                        {
                            return;
                        }

                        SelectItemFrameEx obj = this;

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
                            GiftBagItemEx item1 = var.gameObjectBindScript as GiftBagItemEx;
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
                                txtSelectInfo.text = string.Format("{0}选{1}", m_arrItems.Count, m_giftPackTable.FilterCount);
                            }

                            //                             if (txtSelectInfo2 != null)
                            //                             {
                            //                                 txtSelectInfo2.text = string.Format("（{0}/{1}）", _GetSelectItemsCount(), m_giftPackTable.FilterCount);
                            //                             }

                            if (txtSelectInfo2 != null)
                            {
                                if (_GetSelectItemsCount() == m_giftPackTable.FilterCount)
                                {
                                    if (imgGetGiftText != null)
                                    {
                                        imgGetGiftText.CustomActive(true);
                                    }

                                    txtSelectInfo2.text = "";
                                }
                                else
                                {
                                    txtSelectInfo2.text = TR.Value("select_info", _GetSelectItemsCount(), m_giftPackTable.FilterCount);

                                    if (imgGetGiftText != null)
                                    {
                                        imgGetGiftText.CustomActive(false);
                                    }
                                }
                            }

                        }
                        else
                        {
                            var2.IsSelected = !var2.IsSelected;
                            GiftBagItemEx item1 = var.gameObjectBindScript as GiftBagItemEx;
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
                                txtSelectInfo.text = string.Format("{0}选{1}", m_arrItems.Count, m_giftPackTable.FilterCount);
                            }

                            //                             if (txtSelectInfo2 != null)
                            //                             {
                            //                                 txtSelectInfo2.text = string.Format("（{0}/{1}）", _GetSelectItemsCount(), m_giftPackTable.FilterCount);
                            //                             }

                            if (txtSelectInfo2 != null)
                            {
                                if (_GetSelectItemsCount() == m_giftPackTable.FilterCount)
                                {
                                    if (imgGetGiftText != null)
                                    {
                                        imgGetGiftText.CustomActive(true);
                                    }

                                    txtSelectInfo2.text = "";
                                }
                                else
                                {
                                    txtSelectInfo2.text = TR.Value("select_info", _GetSelectItemsCount(), m_giftPackTable.FilterCount);

                                    if (imgGetGiftText != null)
                                    {
                                        imgGetGiftText.CustomActive(false);
                                    }
                                }
                            }
                        }
                    });
                }
            };

            //DestroyGifgBagItems();
            m_comItemList.SetElementAmount(m_arrItems.Count);

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
    }
}
