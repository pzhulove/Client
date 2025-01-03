using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class MouseYearRedPackageItem : ActivityItemBase
    {


        [SerializeField]
        private Button mGotoBtn;
        [SerializeField]
        private Button mReceiveBtn;
        [SerializeField]
        private GameObject mHaskTakenGo;
        [SerializeField]
        private Text mDesTxt;

        [SerializeField]
        private Transform mItemRoot;
        [SerializeField] Vector2 mComItemSize = new Vector2(100f, 100f);
        private List<ComItem> mComItems = new List<ComItem>();
        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            mGotoBtn.SafeAddOnClickListener(_OnGoToBtnClick);
            mReceiveBtn.SafeAddOnClickListener(_OnReciveBtnClick);

            mDesTxt.SafeSetText(data.Desc);

            if (data != null && data.AwardDataList != null)
            {
                for (int i = 0; i < data.AwardDataList.Count; ++i)
                {
                    var comItem = ComItemManager.Create(mItemRoot.gameObject);
                    if (comItem != null)
                    {
                        ItemData item = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[i].id);
                        item.Count = (int)data.AwardDataList[i].num;
                        comItem.Setup(item, Utility.OnItemClicked);
                        mComItems.Add(comItem);
                        (comItem.transform as RectTransform).sizeDelta = mComItemSize;
                    }
                }
            }
        }

     

        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            mGotoBtn.CustomActive(false);
            mReceiveBtn.CustomActive(false);
            mHaskTakenGo.CustomActive(false);
            switch (data.State)
            {
                case Protocol.OpActTaskState.OATS_INIT:
                case Protocol.OpActTaskState.OATS_UNFINISH:
                    mGotoBtn.CustomActive(true);
                    break;
                case Protocol.OpActTaskState.OATS_FINISHED:
                    mReceiveBtn.CustomActive(true);
                    break;
                case Protocol.OpActTaskState.OATS_OVER:
                    mHaskTakenGo.CustomActive(true);
                    break;
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            mGotoBtn.SafeRemoveOnClickListener(_OnGoToBtnClick);
            mReceiveBtn.SafeRemoveOnClickListener(_OnReciveBtnClick);

            if (mComItems != null)
            {
                for (int i = this.mComItems.Count - 1; i >= 0; --i)
                {
                    ComItemManager.Destroy(mComItems[i]);
                }
                mComItems.Clear();
            }
        }


        private void _OnReciveBtnClick()
        {
            base._OnItemClick();
        }

        private void _OnGoToBtnClick()
        {
            VipFrame vipframe = ClientSystemManager.GetInstance().OpenFrame<VipFrame>() as VipFrame;
            vipframe.SwitchPage(VipTabType.PAY);
        }

        private void _OnCommandLCKBtnClick()
        {
            ActiveManager.GetInstance().OpenActiveFrame(9380, 8600);
        }

    }
}
