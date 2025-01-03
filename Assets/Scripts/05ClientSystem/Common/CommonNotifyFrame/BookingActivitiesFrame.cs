using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using DG.Tweening;
using Protocol;
using Network;
using System.Net;
using System.IO;
using System.Text;
using XUPorterJSON;
using ProtoTable;

namespace GameClient
{
    public class BookingActivitiesFrame : ClientFrame
    {
        int mApponintenRoleID = 0;

        public override sealed string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/CommonSystemNotify/BookingActivitiesFrame";
        }

		#region ExtraUIBind
		private Image mJobImg = null;
		private Text mName = null;
		private Text mDes = null;
		private GameObject mItem1Root = null;
		private GameObject mItem2Root = null;
		private GameObject mItem3Root = null;
		private Button mClose = null;
		
		protected override void _bindExUI()
		{
			mJobImg = mBind.GetCom<Image>("JobImg");
			mName = mBind.GetCom<Text>("Name");
			mDes = mBind.GetCom<Text>("Des");
			mItem1Root = mBind.GetGameObject("item1Root");
			mItem2Root = mBind.GetGameObject("item2Root");
			mItem3Root = mBind.GetGameObject("item3Root");
			mClose = mBind.GetCom<Button>("close");
			if (null != mClose)
			{
				mClose.onClick.AddListener(_onCloseButtonClick);
			}
		}
		
		protected override void _unbindExUI()
		{
			mJobImg = null;
			mName = null;
			mDes = null;
			mItem1Root = null;
			mItem2Root = null;
			mItem3Root = null;
			if (null != mClose)
			{
				mClose.onClick.RemoveListener(_onCloseButtonClick);
			}
			mClose = null;
		}
		#endregion

#region Callback
        private void _onCloseButtonClick()
        {
            ClientSystemManager.instance.CloseFrame(this);
        }
        #endregion


        protected override sealed void _OnOpenFrame()
        {
            mApponintenRoleID = (int)userData;
            PlayerBaseData.GetInstance().JobTableID = mApponintenRoleID;
            InitRewardItem();
            SetJobTableId(mApponintenRoleID);
        }

        protected override sealed void _OnCloseFrame()
        {
            mApponintenRoleID = 0;
        }

        void InitRewardItem()
        {
            int rewardItemId1 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_BOOKING_ACT_ITEM_ICON_1).Value;
            int rewardItemId2 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_BOOKING_ACT_ITEM_ICON_2).Value;
            int rewardItemId3 = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_BOOKING_ACT_ITEM_ICON_3).Value;

            CreateItem(mItem1Root, rewardItemId1);
            CreateItem(mItem2Root, rewardItemId2);
            CreateItem(mItem3Root, rewardItemId3);        }

        void CreateItem(GameObject root, int id)
        {
            ComItem comitem = root.GetComponentInChildren<ComItem>();
            if (comitem == null)
            {
                comitem = CreateComItem(root);
            }
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(id);
            
            if (ItemDetailData != null)
            {
                ItemDetailData.Count = 1;
                comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { _OnShowTips(ItemDetailData); });
            }
        }
        void _OnShowTips(ItemData result)
        {
            if (result == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(result, null, TextAnchor.MiddleCenter, true, false, false);
        }

        void SetJobTableId(int id)
        {
            JobTable table = TableManager.GetInstance().GetTableItem<JobTable>(id);

            if (table == null)
            {
                return;
            }
            
            _setName(table.Name);
            _setJobImg(table.AppointmentJobImage);
            _setAppointmentRoleDes(table.prejob);
        } 

        void _setName(string name)
        {
            if (name == null)
            {
                return;
            }
            mName.text = name;
        }

        void _setJobImg(string path)
        {
            if (path == null)
            {
                return;
            }

            ETCImageLoader.LoadSprite(ref mJobImg, path);
            mJobImg.SetNativeSize();
        }
        void _setAppointmentRoleDes(int id)
        {
            JobTable table = TableManager.GetInstance().GetTableItem<JobTable>(id);
            if (table == null)
            {
                return;
            }
            mDes.text = TR.Value("appointmentrole_description", table.Name);
        }
    }
}
