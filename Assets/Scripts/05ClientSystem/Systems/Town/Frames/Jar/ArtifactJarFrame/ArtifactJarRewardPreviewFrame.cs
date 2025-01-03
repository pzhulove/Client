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
using System.Collections;

namespace GameClient
{
    public class ArtifactJarRewardData
    {
        public List<ItemReward> itemList = new List<ItemReward>();
        public string desc = null;
    }
    public class ArtifactJarRewardPreviewFrame : ClientFrame
    {
        //List<ArtifactJarLotteryTable> itemList = new List<ArtifactJarLotteryTable>();
        ArtifactJarRewardData artifactJarRewardData = new ArtifactJarRewardData();
        #region ExtraUIBind
        private ComUIListScript mItemComUIList = null;
		private GameObject mItemRoot = null;
		private Text mDes = null;
		
		protected override void _bindExUI()
		{
			mItemComUIList = mBind.GetCom<ComUIListScript>("ItemComUIList");
			mItemRoot = mBind.GetGameObject("ItemRoot");
			mDes = mBind.GetCom<Text>("des");
		}
		
		protected override void _unbindExUI()
		{
			mItemComUIList = null;
			mItemRoot = null;
			mDes = null;
		}
		#endregion
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ArtifactJar/ArtifactJarRewardPreviewFrame";
        }

        protected override void _OnOpenFrame()
        {
            if(userData != null)
            {
                artifactJarRewardData = (ArtifactJarRewardData)userData;
                _BindUIEvent();
                _InitComUIList();
                if(artifactJarRewardData.itemList != null)
                {
                mItemComUIList.SetElementAmount(artifactJarRewardData.itemList.Count);
                }
                if(artifactJarRewardData.desc == null)
                {
                    mDes.CustomActive(false);
                }
                else
                {
                    mDes.CustomActive(true);
                    mDes.text = artifactJarRewardData.desc;
                }
            }
        }

        void _InitComUIList()
        {
            mItemComUIList.Initialize();
            mItemComUIList.onItemVisiable = (item) =>
            {
                if (artifactJarRewardData != null && artifactJarRewardData.itemList != null && item.m_index >= 0 && item.m_index < artifactJarRewardData.itemList.Count)
                {
                    ComItem comitem = item.gameObject.GetComponentInChildren<ComItem>();
                    if (comitem == null)
                    {
                        comitem = CreateComItem(item.gameObject);
                    }
                    ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable((int)artifactJarRewardData.itemList[item.m_index].id);
                    if (null == ItemDetailData)
                    {
                        return;
                    }
                    ItemDetailData.Count = (int)artifactJarRewardData.itemList[item.m_index].num;
                    comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { _OnShowTips(ItemDetailData); });
                }
            };
            mItemComUIList.OnItemRecycle = (item) =>
            {
            };
        }
        void _OnShowTips(ItemData result)
        {
            if (result == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(result);
        }

        protected override void _OnCloseFrame()
        {
            _UnBindUIEvent();
        }


        #region ui event
        void _BindUIEvent()
        {
            
        }

        void _UnBindUIEvent()
        {
            
        }
        
        #endregion
    }
}
