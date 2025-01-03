using ProtoTable;
using Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameClient
{
    public class FairDuelHelpFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/FairDuel/FairDuelHelpFrame";
        }

        private ComUIListScript mEquipComUIList;
        private ComUIListScript mFashionComUIList;
        
        protected override void _bindExUI()
        {
            mEquipComUIList = mBind.GetCom<ComUIListScript>("EquipItemList");
            mFashionComUIList = mBind.GetCom<ComUIListScript>("FashionItemList");
          
        }

        protected override void _unbindExUI()
        {
            if (mEquipComUIList != null)
            {
                mEquipComUIList = null;
            }
            if (mFashionComUIList != null)
            {
                mFashionComUIList = null;
            }
         
        }
        protected override void _OnOpenFrame()
        {
            FairDuelDataManager.GetInstance().IintEqualPvPEuqipTableList();
            if (mEquipComUIList != null && !mEquipComUIList.IsInitialised())
            {
                mEquipComUIList.Initialize();
                mEquipComUIList.onBindItem = (obj) =>
                {
                    return CreateComItem(obj);
                };
                mEquipComUIList.onItemVisiable = OEquipListVisiable;
            }
        
            mEquipComUIList.SetElementAmount(FairDuelDataManager.GetInstance().EquipIdList.Count);


            if (mFashionComUIList != null && !mFashionComUIList.IsInitialised())
            {
                mFashionComUIList.Initialize();
                mFashionComUIList.onBindItem = (obj) =>
                {
                    return CreateComItem(obj);
                };
                mFashionComUIList.onItemVisiable = OFashionListVisiable;
            }
            mFashionComUIList.SetElementAmount(FairDuelDataManager.GetInstance().FashioIdList. Count);


        }


        protected override void _OnCloseFrame()
        {
           
        }


        private void OFashionListVisiable(ComUIListElementScript item)
        {
            ComGridBindItem bind = item.GetComponent<ComGridBindItem>();
            if (item.m_index >= 0 && item.m_index < FairDuelDataManager.GetInstance().FashioIdList.Count)
            {
                ComItem comItem = item.gameObjectBindScript as ComItem;
                int id = FairDuelDataManager.GetInstance().FashioIdList[item.m_index];
                ItemData itemDetailData = ItemDataManager.CreateItemDataFromTable(id);
                if (itemDetailData != null)
                {
                    if (FairDuelDataManager.GetInstance().FashionDic.ContainsKey(id))
                    {
                        itemDetailData.StrengthenLevel = FairDuelDataManager.GetInstance().FashionDic[id].StrengthenLv;
                        //itemDetailData.SetStrengthenAttr(itemDetailData);
                        comItem.Setup(itemDetailData, OnShowTips);
                    }

                }

            }
        }

        private void OEquipListVisiable(ComUIListElementScript item)
        {
            ComGridBindItem bind = item.GetComponent<ComGridBindItem>();
            if (item.m_index >= 0 && item.m_index < FairDuelDataManager.GetInstance().EquipIdList.Count)
            {
                ComItem comItem = item.gameObjectBindScript as ComItem;
                int id = FairDuelDataManager.GetInstance().EquipIdList[item.m_index];
                ItemData itemDetailData = ItemDataManager.CreateItemDataFromTable(id);
                if (itemDetailData != null)
                {
                    if (FairDuelDataManager.GetInstance().EquipDic.ContainsKey(id))
                    {
                        
                        itemDetailData.StrengthenLevel = FairDuelDataManager.GetInstance().EquipDic[id].StrengthenLv;
                        itemDetailData.SetStrengthenAttr(itemDetailData);
                        comItem.Setup(itemDetailData, OnShowTips);
                    }

                }

            }
        }

        private void OnShowTips(GameObject obj, ItemData itemData)
        {
            if (itemData == null)
                return;

            ItemTipManager.GetInstance().ShowTip(itemData);
        }

        
      
    }
}
