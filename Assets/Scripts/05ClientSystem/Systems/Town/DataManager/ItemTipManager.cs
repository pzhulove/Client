using System.Collections;
using System.Collections.Generic;
using EItemType = ProtoTable.ItemTable.eType;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    // 提供各种tip界面的打开、关闭
    class ItemTipManager : DataManager<ItemTipManager>
    {
        int m_nMaxTipFrameIdx = -1;

        public override void Initialize()
        {

        }

        public override void Clear()
        {
            CloseAll();
        }

        public void ShowPetTips(PetItemTipsData petData, PetItemTipsData comparePetData)
        {
            if(null == petData)
            {
                return;
            }

            ItemTipPetData data = new ItemTipPetData();
            data.textAnchor = TextAnchor.MiddleCenter;
            data.item = petData;
            data.compareItem = comparePetData;
            data.nTipIndex = m_nMaxTipFrameIdx;

            m_nMaxTipFrameIdx++;
            ClientSystemManager.GetInstance().OpenFrame<ItemTipFrame>(FrameLayer.Middle, data, _GetFrameName(m_nMaxTipFrameIdx));
        }

        public void ShowTipWithCompareItem(ItemData a_item, ItemData a_compareItem, List<TipFuncButon> funcs = null, bool a_enableMask = true)
        {
            if (a_item == null || a_compareItem == null)
            {
                return;
            }

            ItemTipData tipData = new ItemTipData();
            tipData.item = a_item;
            tipData.itemSuitObj = EquipSuitDataManager.GetInstance().GetSelfEquipSuitObj(a_item.SuitID);
            tipData.compareItem = a_compareItem;
            tipData.compareItemSuitObj = EquipSuitDataManager.GetInstance().GetSelfEquipSuitObj(a_compareItem.SuitID);
            tipData.textAnchor = TextAnchor.MiddleCenter;
            tipData.funcs = funcs;
            tipData.giftItemIsRequestServer = true;
            m_nMaxTipFrameIdx++;
            tipData.nTipIndex = m_nMaxTipFrameIdx;

            ClientSystemManager.GetInstance().OpenFrame<ItemTipFrame>(FrameLayer.Middle, tipData, _GetFrameName(m_nMaxTipFrameIdx));
        }

        public ItemData GetCompareItem(ItemData a_item)
        {
            ItemData a_compareItem = null;
            if (a_item.Type == EItemType.EQUIP ||
    a_item.Type == EItemType.FASHION ||
    a_item.Type == EItemType.FUCKTITTLE)
            {
                var tipsItem = ItemDataManager.GetInstance().GetItem(a_item.GUID);
                //如果TIPS装备不是自己的物品 或者 没有装备
                if (null == tipsItem ||
                    tipsItem.PackageType != EPackageType.WearEquip &&
                    tipsItem.PackageType != EPackageType.WearFashion)
                {
                    if (a_item.Type == EItemType.EQUIP ||
                        a_item.Type == EItemType.FUCKTITTLE)
                    {
                        var items = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
                        if (null != items)
                        {
                            for (int i = 0; i < items.Count; ++i)
                            {
                                var item = ItemDataManager.GetInstance().GetItem(items[i]);
                                if (null != item && item.EquipWearSlotType == a_item.EquipWearSlotType)
                                {
                                    a_compareItem = item;
                                    break;
                                }
                            }
                        }
                    }
                    else if (a_item.Type == EItemType.FASHION)
                    {
                        var items = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);
                        if (null != items)
                        {
                            for (int i = 0; i < items.Count; ++i)
                            {
                                var item = ItemDataManager.GetInstance().GetItem(items[i]);
                                if (null != item && item.FashionWearSlotType == a_item.FashionWearSlotType)
                                {
                                    a_compareItem = item;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return a_compareItem;
        }

        public void ShowTipWithoutModelAvatar(ItemData itemData)
        {
            ShowTip(itemData,
                null,
                TextAnchor.MiddleCenter,
                true,
                true);
        }

        //isForceCloseModelAvatar 默认为false, giftItemIsRequestServer 默认为true，
        public void ShowTip(ItemData a_item, 
            List<TipFuncButon> funcs = null, 
            TextAnchor a_textAnchor = TextAnchor.MiddleCenter, 
            bool a_enableMask = true,
            bool isForceCloseModelAvatar = false,
            bool giftItemIsRequestServer = true)
        {
#if UNITY_EDITOR
            Debug.LogFormat("item table id = {0}", a_item.TableID);
#endif

            if (a_item == null)
            {
                return;
            }

            ItemData a_compareItem = GetCompareItem(a_item);
            EquipSuitObj suitObj = null;
            if(null != a_compareItem)
            {
                suitObj = EquipSuitDataManager.GetInstance().GetSelfEquipSuitObj(a_compareItem.SuitID);
            }

            ItemTipData tipData = new ItemTipData();
            tipData.item = a_item;
            tipData.itemSuitObj = EquipSuitDataManager.GetInstance().GetSelfEquipSuitObj(a_item.SuitID);
            tipData.compareItem = a_compareItem;
            tipData.compareItemSuitObj = suitObj;
            tipData.textAnchor = a_textAnchor;
            tipData.funcs = funcs;
            tipData.IsForceCloseModelAvatar = isForceCloseModelAvatar;
            tipData.giftItemIsRequestServer = giftItemIsRequestServer;
            m_nMaxTipFrameIdx++;
            tipData.nTipIndex = m_nMaxTipFrameIdx;
            
            ClientSystemManager.GetInstance().OpenFrame<ItemTipFrame>(FrameLayer.Middle, tipData, _GetFrameName(m_nMaxTipFrameIdx));
        }

        public void ShowOtherPlayerTipWithCompareItem(ItemData a_item, EquipSuitObj a_itemSuitObj, 
            ItemData a_compareItem, EquipSuitObj a_compareItemSuitObj, List<TipFuncButon> funcs = null, bool a_enableMask = true)
        {
            if (a_item == null)
            {
                return;
            }

            ItemTipData tipData = new ItemTipData();
            tipData.item = a_item;
            tipData.itemSuitObj = a_itemSuitObj;
            tipData.compareItem = a_compareItem;
            tipData.compareItemSuitObj = a_compareItemSuitObj;
            tipData.textAnchor = TextAnchor.MiddleCenter;
            tipData.funcs = funcs;
            m_nMaxTipFrameIdx++;
            tipData.nTipIndex = m_nMaxTipFrameIdx;

            ClientSystemManager.GetInstance().OpenFrame<ItemTipFrame>(FrameLayer.Middle, tipData, _GetFrameName(m_nMaxTipFrameIdx));
        }

        public void ShowOtherPlayerTip(ItemData a_item, EquipSuitObj a_itemSuitObj, List<TipFuncButon> funcs = null, TextAnchor a_textAnchor = TextAnchor.MiddleCenter, bool a_enableMask = true)
        {
            if (a_item == null)
            {
                return;
            }

            ItemData a_compareItem = GetCompareItem(a_item);
            EquipSuitObj suitObj = null;
            if (null != a_compareItem)
            {
                suitObj = EquipSuitDataManager.GetInstance().GetSelfEquipSuitObj(a_compareItem.SuitID);
            }

            ItemTipData tipData = new ItemTipData();
            tipData.item = a_item;
            tipData.itemSuitObj = a_itemSuitObj;
            tipData.compareItem = a_compareItem;
            tipData.compareItemSuitObj = suitObj;
            tipData.textAnchor = a_textAnchor;
            tipData.funcs = funcs;
            tipData.giftItemIsRequestServer = true;
            m_nMaxTipFrameIdx++;
            tipData.nTipIndex = m_nMaxTipFrameIdx;

            ClientSystemManager.GetInstance().OpenFrame<ItemTipFrame>(FrameLayer.Middle, tipData, _GetFrameName(m_nMaxTipFrameIdx));
        }

        public void CloseAll()
        {
            while (m_nMaxTipFrameIdx >= 0)
            {
                ClientSystemManager.GetInstance().CloseFrame(_GetFrameName(m_nMaxTipFrameIdx));
                m_nMaxTipFrameIdx--;
            }
            m_nMaxTipFrameIdx = -1;
        }

        public void CloseTip(int a_nTipIndex)
        {
            while (m_nMaxTipFrameIdx >= a_nTipIndex)
            {
                ClientSystemManager.GetInstance().CloseFrame(_GetFrameName(m_nMaxTipFrameIdx));
                m_nMaxTipFrameIdx--;
            }

            if (m_nMaxTipFrameIdx < 0)
            {
                m_nMaxTipFrameIdx = -1;
            }
        }

        string _GetFrameName(int nIdx)
        {
            return string.Format("ItemTipFrame{0}", nIdx);
        }


        #region ItemTipAvatarModelLayer
        public readonly int ItemTipModelAvatarBaseLayerIndex = 29;
        public readonly int ItemTipModelAvatarMaxLayerIndex = 31;
        //得到当前ItemTip上展示AvatarModel的Layer层级， 介于29-31
        public int GetItemTipModelAvatarLayerIndex(int itemTipIndex)
        {

            var currentLayerIndex = GetCurrentShowModelAvatarLayerNumber(itemTipIndex);

            var showModelAvatarLayerIndex = currentLayerIndex + ItemTipModelAvatarBaseLayerIndex;

            if (showModelAvatarLayerIndex < ItemTipModelAvatarBaseLayerIndex)
            {
                //小于基础值，赋值基础值
                showModelAvatarLayerIndex = ItemTipModelAvatarBaseLayerIndex;
            }
            else if (showModelAvatarLayerIndex > ItemTipModelAvatarMaxLayerIndex)
            {
                //大于最大值，赋值最大值
                showModelAvatarLayerIndex = ItemTipModelAvatarMaxLayerIndex;
            }

            return showModelAvatarLayerIndex;
        }

        //得到当前ItemTipFrame之前的ItemTipFrame中已经有几个展示了ModelAvatar
        //0,1,2
        private int GetCurrentShowModelAvatarLayerNumber(int currentItemTipIndex)
        {
            int currentShowModelAvatarLayerNumber = 0;

            if (currentItemTipIndex <= 0)
                return currentShowModelAvatarLayerNumber;

            for (var i = 0; i < currentItemTipIndex; i++)
            {
                var frameName = _GetFrameName(i);
                var itemTipFrame = ClientSystemManager.GetInstance().GetFrame(frameName) as ItemTipFrame;

                if (itemTipFrame != null)
                {
                    if (itemTipFrame.GetShowModelAvatarFlag() == true)
                    {
                        currentShowModelAvatarLayerNumber += 1;
                    }
                }
            }
            return currentShowModelAvatarLayerNumber;
        }

        

        #endregion

    }
}
