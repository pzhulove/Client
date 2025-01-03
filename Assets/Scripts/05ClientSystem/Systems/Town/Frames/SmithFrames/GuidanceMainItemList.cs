using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using ProtoTable;
using Scripts.UI;

namespace GameClient
{
    class GuidanceMainItemList
    {
        ClientFrame clientFrame = null;
        GameObject gameObject = null;
        ComUIListScript comUIListScript = null;
        List<ProtoTable.GuidanceMainItemTable> realValues = new List<GuidanceMainItemTable>();
        int iFilterId = 1;
        bool bInitialized = false;

        ComGuidanceMainItem _OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ComGuidanceMainItem>();
        }

        public void Initialize(ClientFrame clientFrame,GameObject gameObject)
        {
            this.clientFrame = clientFrame;
            this.gameObject = gameObject;
            comUIListScript = this.gameObject.GetComponent<ComUIListScript>();
            comUIListScript.Initialize();

            comUIListScript.onBindItem += _OnBindItemDelegate;
            comUIListScript.onItemVisiable += _OnItemVisiableDelegate;
            bInitialized = true;
        }

        List<ProtoTable.GuidanceMainItemTable> _OnFilter()
        {
            List<ProtoTable.GuidanceMainItemTable> ret = new List<GuidanceMainItemTable>();
            var data = TableManager.GetInstance().GetTableItem<ProtoTable.GuidanceMainTable>(iFilterId);
            if(data != null)
            {
                for(int i = 0; i < data.LinkItems.Count; ++i)
                {
                    var guidanceMainItem = TableManager.GetInstance().GetTableItem<ProtoTable.GuidanceMainItemTable>(data.LinkItems[i]);
                    if (null == guidanceMainItem)
                    {
                        continue;
                    }

                    if(guidanceMainItem.recommandLv <= 0)
                    {
                        continue;
                    }

                    var functionUnLockData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>(guidanceMainItem.FunctionId);
                    if(functionUnLockData != null && functionUnLockData.FinishLevel > PlayerBaseData.GetInstance().Level)
                    {
                        continue;
                    }

                    ret.Add(guidanceMainItem);
                }
            }
            return ret;
        }

        public void RefreshDatas(int iFilterId)
        {
            this.iFilterId = iFilterId;
            if (bInitialized)
            {
                realValues = _OnFilter();
                comUIListScript.SetElementAmount(realValues.Count);
            }
        }

        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            if(item != null && item.m_index >= 0 && item.m_index < realValues.Count)
            {
                var target = item.gameObjectBindScript as ComGuidanceMainItem;
                if(target != null)
                {
                    target.OnVisible(realValues[item.m_index],clientFrame as DevelopGuidanceMainFrame);
                }
            }
        }

        public void UnInitialize()
        {
            bInitialized = false;
            comUIListScript.onBindItem -= _OnBindItemDelegate;
            comUIListScript.onItemVisiable -= _OnItemVisiableDelegate;
            comUIListScript = null;

            this.clientFrame = null;
            this.gameObject = null;
        }
    }
}