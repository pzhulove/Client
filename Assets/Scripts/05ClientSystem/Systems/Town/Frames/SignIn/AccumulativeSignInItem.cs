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
using DG.Tweening;


namespace GameClient
{
    using UIItemData = AwardItemData;

    public class AccumulativeSignInItem: MonoBehaviour
    {
        [SerializeField]
        Text accumulativeDay = null;

        [SerializeField]
        Image gotMark = null; // 已经领取的勾勾

        [SerializeField]
        GameObject canGetEff = null; // 可领取特效

        [SerializeField] Text mTextCount;
        [SerializeField] Image mImageIcon;
        [SerializeField] Image mImageBg;
        [SerializeField] float mAlpha = 0.5f;//已领取时的alpha
        [SerializeField] float mScale = 0.8f;//不领取时背景图缩放比例
        [SerializeField] string mBgGotPath;//已领取时的背景图片
        [SerializeField] string mBgNoGetPath;//未领取时的背景图片
        [SerializeField] GameObject mEffParent;//可领取特效父物体
        [SerializeField] DOTweenAnimation mCanGetAwardAni;//可领取动画
        [SerializeField] float mCanGetAwardAniStartScale = 0.3f;//可领取动画初始缩放


        void ShowItemTip(GameObject go, ItemData itemData)
        {
            if (null != itemData)
            {
                ItemTipManager.GetInstance().CloseAll();
                ItemTipManager.GetInstance().ShowTip(itemData);
            }
        }

        void SetComItemData(ComItem comItem, UIItemData uIItemData)
        {
            if(comItem == null || uIItemData == null)
            {
                return;
            }

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(uIItemData.ID);
            if (itemData != null)
            {
                itemData.Count = uIItemData.Num;
                comItem.Setup(itemData, ShowItemTip);
            }

            return;
        }

        string GetColorName(UIItemData uIItemData)
        {
            if(uIItemData == null)
            {
                return "";
            }

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(uIItemData.ID);
            if (itemData != null)
            {
                return itemData.GetColorName();
            }

            return "";
        }
        
        string GetVipAddUpText(int day)
        {
            return "";
        }

        private ActivityDataManager.AccumulativeSignInItemData mData;
        public void SetUp(object data)
        {
            if(data == null)
            {
                return;
            }

            mData = data as ActivityDataManager.AccumulativeSignInItemData;
            if(mData == null)
            {
                return;
            }

            DateTime dateTime = Function.ConvertIntDateTime(TimeManager.GetInstance().GetServerDoubleTime());    

            //canGetEff.CustomActive(!mData.hasGotAward && mData.accumulativeDay <= ActivityDataManager.GetInstance().GetHasSignInCount());

            accumulativeDay.SafeSetText(TR.Value("accumulative_day", mData.accumulativeDay));
            if(mData.accumulativeDay >= ActivityDataManager.GetMonthDayNum(dateTime.Year, dateTime.Month))
            {
                accumulativeDay.SafeSetText(TR.Value("at_duty_every_day"));
            }
            gotMark.CustomActive(mData.hasGotAward);
            if (mData.hasGotAward)
            {
                var color = mTextCount.color;
                color.a = mAlpha;
                mTextCount.color = color;

                color = mImageIcon.color;
                color.a = mAlpha;
                mImageIcon.color = color;

                color = mImageBg.color;
                color.a = mAlpha;
                mImageBg.color = color;
                mImageBg.transform.localScale = Vector3.one * mScale;
                mEffParent.CustomActive(false);

                if (mCanGetAwardAni != null)
                {
                    mCanGetAwardAni.DOPause();
                    mCanGetAwardAni.transform.localEulerAngles = Vector3.zero;
                    mCanGetAwardAni.transform.localScale = Vector3.one;
                }

                ETCImageLoader.LoadSprite(ref mImageBg, mBgNoGetPath);
            }
            else if (mData.accumulativeDay <= ActivityDataManager.GetInstance().GetHasSignInCount())
            {
                ETCImageLoader.LoadSprite(ref mImageBg, mBgGotPath);
                mImageBg.transform.localScale = Vector3.one;
                mEffParent.CustomActive(true);

                if (mCanGetAwardAni != null)
                {
                    mCanGetAwardAni.transform.localScale =Vector3.one * mCanGetAwardAniStartScale;
                    mCanGetAwardAni.DORestart();
                }
            }

            if (mData.awardItemData.Num > 1)
            {
                mTextCount.SafeSetText(mData.awardItemData.Num.ToString());
            }
            else
            {
                mTextCount.SafeSetText(string.Empty);
            }
            var tableData = TableManager.GetInstance().GetTableItem<ItemTable>(mData.awardItemData.ID);
            if (tableData != null)
            {
                ETCImageLoader.LoadSprite(ref mImageIcon, tableData.Icon);
            }

            return;
        }

        public void OnButtonClick()
        {
            if (mData == null)
            {
                return;
            }

            if (!mData.hasGotAward && mData.accumulativeDay <= ActivityDataManager.GetInstance().GetHasSignInCount())
            {
                ActivityDataManager.GetInstance().SendGetAccumulativeSignInAward(mData.accumulativeDay);
            }
            else
            {
                if (mData.awardItemData != null)
                {
                    ItemData itemData = ItemDataManager.CreateItemDataFromTable(mData.awardItemData.ID);
                    if (itemData != null)
                    {
                        itemData.Count = mData.awardItemData.Num;
                        ItemTipManager.GetInstance().CloseAll();
                        ItemTipManager.GetInstance().ShowTip(itemData);
                    }
                }
            }

        }
    }
}


