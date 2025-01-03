using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;

namespace GameClient
{
    public class TriggerMallFrameView : MonoBehaviour
    {
        [SerializeField]private ComUIListScript mMallListScript;
        private List<MallItemInfo> mMallItemList = new List<MallItemInfo>();
        
        public void OnInit()
        {
            if (null != mMallListScript)
            {
                mMallListScript.Initialize();
                mMallListScript.onItemVisiable = OnMallItemShow;
                mMallListScript.OnItemUpdate = OnMallItemShow;
            }
            OnShowMallList();
        }

        public void OnUninit()
        {
            if (null != mMallListScript)
            {
                mMallListScript.UnInitialize();
            }
        }

        //显示商品列表
        private void OnShowMallList()
        {
            mMallItemList = MallNewDataManager.GetInstance().GetMallItemInfoList((int)MallTypeTable.eMallType.SN_GIFT, 3);
            if (null == mMallItemList || mMallItemList.Count <= 0)
            {
                Logger.LogError("找不到触发礼包或者触发礼包无内容");
                return;
            }
            mMallListScript.SetElementAmount(mMallItemList.Count);
        }

        private void OnMallItemShow(ComUIListElementScript item)
        {
            if (null == item || item.m_index >= mMallItemList.Count)
                return;
            var script = item.GetComponent<TriggerMallItem>();
            if (null == script)
                return;
            script.OnInit(mMallItemList[item.m_index]);
        }

        //商品数据更新
        public void OnUpdateMallList()
        {
            OnShowMallList();
        }
    }
}
