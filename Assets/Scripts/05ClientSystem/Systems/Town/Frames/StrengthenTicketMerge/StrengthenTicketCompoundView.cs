using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class StrengthenTicketCompoundView : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mDisplayItemList;
        [SerializeField] private Text mTextRate;
        [SerializeField] private Text mTextCount;
        [SerializeField] private Toggle mToggle1;
        [SerializeField] private ComItemNew mShowItem;
        [SerializeField] private List<StrengthenTicketCompoundNeedItem> mNeedItemList;
        [SerializeField] private SetComButtonCD mBtnCD;
        //所需显示的数据列表
        private List<StrengthenTicketMaterialMergeModel> mDisplayItemDataList;
        //当前选中的下标
        private int mCurSelectIndex;
        //当前增幅比例
        private int mCurRate;
        private Action mOnClickEvent;
        private UnityEngine.Coroutine waitToReqFuseTicket = null;
        private StrengthenTicketMaterialMergeModel mCurSelectModel {
            get {
                if (null != mDisplayItemDataList && mDisplayItemDataList.Count > mCurSelectIndex)
                    return mDisplayItemDataList[mCurSelectIndex];
                return null;
            }
            set {}
        }
        public void OnInit(Action onclickEvent)
        {
            mOnClickEvent = onclickEvent;
            _OnInitList();
            OnUpdateDisCount();
            _updateView();
        }

        //打折次数
        public void OnUpdateDisCount()
        {
            if (mTextCount != null)
            {
                Protocol.OpActivityData activityData = StrengthenTicketMergeDataManager.GetInstance().GetCurBuffPrayActivityData();
                if (activityData == null)//没有这个活动
                {
                    mTextCount.SafeSetText("");
                }
                else
                {
                    bool isFinish = StrengthenTicketMergeDataManager.GetInstance().PrayActivityIsFinish;
                    if (!isFinish)
                    {
                        mTextCount.text = string.Format(StrengthenTicketMergeDataManager.GetInstance().GetPrayTaskDes(), StrengthenTicketMergeDataManager.GetInstance().GetLeftPrayeTimer());
                    }
                    else
                        mTextCount.SafeSetText("");
                }


            }
        }

        private void _OnInitList()
        {
            //需要展示的数据
            mDisplayItemDataList = StrengthenTicketMergeDataManager.GetInstance().GetDisplayMaterialMergeTicketModels();
            mCurSelectIndex = 0;
            if (null != mDisplayItemList)
            {
                mDisplayItemList.Initialize();
                mDisplayItemList.onItemVisiable = _OnItemShow;
                mDisplayItemList.OnItemUpdate = _OnItemShow;
                mDisplayItemList.onItemSelected = _OnItemSelect;
                mDisplayItemList.SetElementAmount(mDisplayItemDataList.Count);
            }
        }

        private void _OnItemSelect(ComUIListElementScript item)
        {
            if (null == item || null == mDisplayItemDataList || mDisplayItemDataList.Count <= item.m_index)
                return;
            mCurSelectIndex = item.m_index;
            mDisplayItemList.UpdateElement();
            _updateView();
        }
        private void _OnItemShow(ComUIListElementScript item)
        {
            if (null == item || null == mDisplayItemDataList || mDisplayItemDataList.Count <= item.m_index)
                return;
            var script = item.GetComponent<StrengthenTicketCompoundItem>();
            if (null == script)
                return;
            var data = mDisplayItemDataList[item.m_index];
            script.OnInit(data, mCurSelectIndex == item.m_index);
        }

        //更新界面
        private void _updateView()
        {
            if (null == mCurSelectModel)
                return;
            ItemData itemData = ItemDataManager.CreateItemDataFromTable(mCurSelectModel.displayItemTableId);
            mShowItem.Setup(itemData, null, false);
            if(mToggle1.isOn)
            {
                OnToggleClick1(true);
            }
            else
            {
                mToggle1.isOn = true;
            }
        }

        //增幅后的数据
        private StrengthenTicketMaterialMergeModel mRateModel;
        //更新增幅相关
        private void _updateRate()
        {
            if (null == mCurSelectModel)
                return;
            //获取到增幅信息
            mRateModel = StrengthenTicketMergeDataManager.GetInstance().GetMaterialMergeStrengthenTicketTableId(mCurSelectModel, mCurRate);
            UpdateMaterialItem();
            //更新倍率
            mTextRate.SafeSetText(string.Format(TR.Value("strengthen_merge_material_preview_tip"), mRateModel.strengthenLevel, mRateModel.previewMinPercent,mRateModel.previewMaxPercent));
        }
        //更新材料
        public void UpdateMaterialItem()
        {
            if (null == mRateModel)
                return;
            var materialModel = mRateModel.needMaterialModel;
            if (materialModel == null)
                return;
            var materials = materialModel.needMaterialDatas;
            if (materials == null)
                return;
            for (int index = 0; mNeedItemList.Count > index; ++index)
            {
                var item = mNeedItemList[index];
                if (item != null && index < materials.Count)
                {
                    item.OnInit(materials[index].tempItemData);
                }
            }
        }

        //点击合成
        public void OnClickCompound()
        {
            if (mBtnCD == null || mBtnCD.IsBtnWork() == false)
            {
                return;
            }
            if (null == mRateModel)
            {
                Logger.LogError("所选的合成目标为空");
                return;
            }
            //判断材料是否足够
            bool isCanCompound = StrengthenTicketMergeDataManager.GetInstance().CheckMaterialMergeIsEnough(mRateModel);
            if (!isCanCompound)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_merge_material_not_enough"));
                return;
            }
            if (StrengthenTicketMergeDataManager.GetInstance().BSyntheticFrameTip == false)
            {
                //材料增幅数值等于100%弹提示
                if (mCurRate == 1)
                {
                    CommonMsgBoxOkCancelNewParamData comconMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData();
                    comconMsgBoxOkCancelParamData.ContentLabel = TR.Value("strengthen_tick_merge_desc");
                    comconMsgBoxOkCancelParamData.IsShowNotify = true;
                    comconMsgBoxOkCancelParamData.IsDefaultCheck = true;
                    comconMsgBoxOkCancelParamData.OnCommonMsgBoxToggleClick = OnUpdateSyntheticFrameTip;
                    comconMsgBoxOkCancelParamData.LeftButtonText = TR.Value("common_data_cancel");
                    comconMsgBoxOkCancelParamData.RightButtonText = TR.Value("common_data_sure_2");
                    comconMsgBoxOkCancelParamData.OnRightButtonClickCallBack = _OnReqMaterialMergeStrengthenTicket;
                    SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(comconMsgBoxOkCancelParamData);
                    return;
                }
            }
            _OnReqMaterialMergeStrengthenTicket();
            mBtnCD.StartBtCD();
        }
        private void OnUpdateSyntheticFrameTip(bool value)
        {
            StrengthenTicketMergeDataManager.GetInstance().BSyntheticFrameTip = value;
        }
        private void _OnReqMaterialMergeStrengthenTicket()
        {
            ItemSimpleData sData = StrengthenTicketMergeDataManager.GetInstance().TryGetFirstCoinItemDataInMaterials();
            if (sData != null)
            {
                CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo { nMoneyID = sData.ItemID, nCount = sData.Count }, () =>
                {
                    _StartReqMergeTicket();
                });
            }
        }
        void _StartReqMergeTicket()
        {
            if (waitToReqFuseTicket != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToReqFuseTicket);
            }
            waitToReqFuseTicket = GameFrameWork.instance.StartCoroutine(_WaitToReqMergeTicket());
        }
        IEnumerator _WaitToReqMergeTicket()
        {
            if (null != mOnClickEvent)
                mOnClickEvent();
            //播完一次动画才请求
            float animDuration = _PlayToProcessStageAnims();
            yield return Yielders.GetWaitForSeconds(animDuration);
            StrengthenTicketMergeDataManager.GetInstance().ReqMaterialMergeStrengthenTicket();
        }
        //动画播放时间
        float _PlayToProcessStageAnims()
        {
            float delayDuration = 1f;
            return delayDuration;
        }


        //点击1倍增幅
        public void OnToggleClick1(bool value)
        {
            if (!value)
                return;
            mCurRate = 1;
            _updateRate();
        }

        //点击2倍增幅
        public void OnToggleClick2(bool value)
        {
            if (!value)
                return;
            mCurRate = 2;
            _updateRate();
        }

        //点击3倍增幅
        public void OnToggleClick3(bool value)
        {
            if (!value)
                return;
            mCurRate = 3;
            _updateRate();
        }
    }

}