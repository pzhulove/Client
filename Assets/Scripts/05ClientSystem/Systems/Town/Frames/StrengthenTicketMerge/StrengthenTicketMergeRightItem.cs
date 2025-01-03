using Protocol;
using Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class StrengthenTicketMergeRightItem : CustomClientFrameItem
        // ICustomClientFrameItem<StrengthenTicketMergeFrame, object>
    {
        private UnityEngine.Coroutine waitToReqMergeTicket = null;
        private UnityEngine.Coroutine waitToReqFuseTicket = null;
        private bool bSkipAnim = false;

        private UnityEngine.Coroutine waitToPlayNextAnim = null;

        #region Model Params

        object mViewModel = null;
        StrengthenTicketMergeFrameView mView = null;

        //材料合成

        //券融合        

        //材料放置格子
        List<ComItemCustom> mCustomItemGrids = new List<ComItemCustom>();
        //默认材料id
        List<int> mDefaultMergeMaterialItemIds = new List<int>();
        List<int> mDefaultFuseMaterialItemIds = new List<int>();
        List<int> mDefaultMergeOnlyShowNeedItemIds = new List<int>();
        List<int> mNeedFastBuyItemIds = new List<int>();
        private UnityEngine.Coroutine waitToCreateGrids = null;
        private bool bMaterialMergeGridInited = false;                  //材料合成 用

        private bool bSelectMergeLevelBtnEnable = false;

        private int iMaterialGrowthValue = 1;//材料增幅默认值(1：表示100%)

        private string tr_notice_select_left_tickets_merge = "请在左侧选择要合成的强化券";
        private string tr_notice_select_left_tickets_fuse = "请在左侧选择作为材料的强化券";
        private string tr_notice_material_item_count_format = "{0}/{1}";
        private string tr_notice_preview_material_merge = "+{0}({1}% - {2}%)";
        private string tr_notice_preview_ticket_fuse = "最低+{0}({1}%) - 最高+{2}({3}%)";
        private string tr_notice_please_select_ticket = "请选择强化券";
        private string tr_notice_desc_less_color_format = "<color=#ff0000ff>{0}</color>";
        private string tr_notice_desc_normal_color_format = "<color=#ffffffff>{0}</color>";
        private string tr_notice_merge_material_not_enough = "合成材料不足";
        private string tr_stengthen_tick_mrger_notice_tip = "合成所需的金币减少30%,绑金减少15%（剩余{0}次）";
        #endregion
        private bool mIsNotShowSkillConfigTip = false;
        #region View Params

        private Button mBtnMerge = null;
        private Toggle mSkipAnimToggle = null;
        private GameObject mMaterialItemRoot = null;
        private Text mMaterialTips = null;
        private ComDropDownCustom mIncreaseRateDrop = null;
        private ComStrengthenTicketAnim mResultPlaneComAnim = null;
        private Button mMergeBtnMask = null;
        private SetComButtonCD mBtnMergeCD = null;
        private Text mTipTxt;

        
        
        #endregion
        
        #region PRIVATE METHODS

        protected override void _Init()
        {
            _LoadTR();
            _BindUIEvent();
            _CreateCustomGrids();
            if (mSkipAnimToggle)
            {
                mSkipAnimToggle.isOn = false;
            }
            _PlayToNotReadyStageAnims();
            _OnUpdateUseTime();
        }

        protected override void _Clear()
        {
            if (waitToReqFuseTicket != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToReqFuseTicket);
                waitToReqFuseTicket = null;
            }
            if (waitToReqMergeTicket != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToReqMergeTicket);
                waitToReqMergeTicket = null;
            }

            bSkipAnim = false;

            if (waitToPlayNextAnim != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToPlayNextAnim);
                waitToPlayNextAnim = null;
            }

            _UnBindUIEvent();

            //mMaterialMergeModel = null;//去掉引用
            mViewModel = null;
            mView = null;

            if (mCustomItemGrids != null)
            {
                for (int i = 0; i < mCustomItemGrids.Count; i++)
                {
                    mCustomItemGrids[i].Clear();
                }
                mCustomItemGrids.Clear();
            }
            bMaterialMergeGridInited = false;

            if (mDefaultMergeMaterialItemIds != null)
            {
                mDefaultMergeMaterialItemIds.Clear();
            }
            if (mDefaultFuseMaterialItemIds != null)
            {
                mDefaultFuseMaterialItemIds.Clear();
            }
            if (mDefaultMergeOnlyShowNeedItemIds != null)
            {
                mDefaultMergeOnlyShowNeedItemIds.Clear();
            }
            if (mNeedFastBuyItemIds != null)
            {
                mNeedFastBuyItemIds.Clear();
            }

            if (waitToCreateGrids != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToCreateGrids);
                waitToCreateGrids = null;
            }

            if (null != mBtnMerge)
            {
                mBtnMerge.onClick.RemoveListener(_onBtnMergeButtonClick);
            }
            mBtnMerge = null;
            if (null != mSkipAnimToggle)
            {
                mSkipAnimToggle.onValueChanged.RemoveListener(_onSkipAnimToggleToggleValueChange);
            }
            mSkipAnimToggle = null;
            mMaterialItemRoot = null;
            mMaterialTips = null;
            if (mIncreaseRateDrop != null)
            {
                mIncreaseRateDrop.Clear();
                mIncreaseRateDrop = null;
            }
            mResultPlaneComAnim = null;
            if (null != mMergeBtnMask)
            {
                mMergeBtnMask.onClick.RemoveListener(_onMergeBtnMaskButtonClick);
            }
            mMergeBtnMask = null;
            mBtnMergeCD = null;
            mTipTxt = null; 

            tr_notice_select_left_tickets_merge = "";
            tr_notice_select_left_tickets_fuse = "";
            tr_notice_preview_material_merge = "";
            tr_notice_preview_ticket_fuse = "";
            tr_notice_material_item_count_format = "";
            tr_notice_please_select_ticket = "";
            tr_notice_desc_less_color_format = "";
            tr_notice_desc_normal_color_format = "";
            tr_notice_merge_material_not_enough = "";
            tr_stengthen_tick_mrger_notice_tip = "";
            iMaterialGrowthValue = 1;
        }

        private void _LoadTR()
        {
            tr_notice_select_left_tickets_merge = TR.Value("strengthen_merge_select_left_tickets_merge");
            tr_notice_select_left_tickets_fuse = TR.Value("strengthen_merge_select_left_tickets_fuse");
            tr_notice_material_item_count_format = TR.Value("strengthen_merge_material_count_format");
            tr_notice_preview_material_merge = TR.Value("strengthen_merge_material_preview_tip");
            tr_notice_preview_ticket_fuse = TR.Value("strengthen_merge_ticket_preview_tip");
            tr_notice_please_select_ticket = TR.Value("strengthen_merge_plase_select_ticket");
            tr_notice_desc_less_color_format = TR.Value("strengthen_merge_desc_less_color_format");
            tr_notice_desc_normal_color_format = TR.Value("strengthen_merge_desc_normal_color_format");
            tr_notice_merge_material_not_enough = TR.Value("strengthen_merge_material_not_enough");
            tr_stengthen_tick_mrger_notice_tip = StrengthenTicketMergeDataManager.GetInstance().GetPrayTaskDes();
               
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketMergeSelectType, _OnSrengtheTicketMergeSelectType);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeTaskUpdate, _OnUpdateUseTime);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketMergeStateUpdate,_OnUpdatePrayActivityState);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketFreshView,_OnStengthenTickFreshView);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketMergeSelectType, _OnSrengtheTicketMergeSelectType);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeTaskUpdate, _OnUpdateUseTime);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketMergeStateUpdate, _OnUpdatePrayActivityState);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketFreshView, _OnStengthenTickFreshView);
        }

        private StrengthenTicketMergeType _GetCurrMergeType()
        {
            if (mView == null)
            {
                return StrengthenTicketMergeType.Count;
            }
            return mView.MergeType;
        }

        private void _CreateCustomGrids()
        {
            if (mView != null)
            {
                List<int> tempMergeIds = mView.GetSpecialMergeBindItemIds();
                if (tempMergeIds != null)
                {
                    tempMergeIds.AddRange(mView.GetMergeBindItemIds());
                }
                mDefaultMergeMaterialItemIds = tempMergeIds;
                mDefaultFuseMaterialItemIds = mView.GetFuseBindItemIds();
                mDefaultMergeOnlyShowNeedItemIds = mView.GetMergeOnlyShowNeedCountItemIds();
                mNeedFastBuyItemIds = mView.GetNeedFastBuyItemIds();
            }       

            if (waitToCreateGrids != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToCreateGrids);
            }
            waitToCreateGrids = GameFrameWork.instance.StartCoroutine(_WaitToCreateGrids());
        }

        IEnumerator _WaitToCreateGrids()
        {
            if (mView == null)
            {
                yield break;
            }           
            if (mDefaultMergeMaterialItemIds == null || mDefaultMergeMaterialItemIds.Count == 0)
            {
                yield break;
            }
            int index = 0;
            //Logger.LogError("_WaitToCreateGrids mDefaultMergeMaterialItemIds Count : " + mDefaultMergeMaterialItemIds.Count);
            while (index < mDefaultMergeMaterialItemIds.Count)
            {
                GameObject grid = AssetLoader.GetInstance().LoadResAsGameObject(StrengthenTicketMergeFrame.CUSTOM_COM_ITEM_RES_PATH);
                if (grid == null)
                {
                    continue;
                }
                Utility.AttachTo(grid, mMaterialItemRoot);
                ComItemCustom customItem = grid.GetComponent<ComItemCustom>();
                if (customItem != null)
                {
                    customItem.Init(false, mDefaultMergeMaterialItemIds[index], false, true, true, ComItemCustomClickType.Button);
                    customItem.SetDescStr("");
                    customItem.CustomActive(true);
                }
                if (mCustomItemGrids != null && customItem != null)
                {
                    mCustomItemGrids.Add(customItem);
                }
                index++;
                yield return null;
            }
            //Logger.LogError("_WaitToCreateGrids mCustomItemGrids Count : " + mCustomItemGrids.Count);
            bMaterialMergeGridInited = true;
            //初始化完成 默认选中
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketMergeSelectReset);
        }

        private void _ResetStrengthTicketContent(StrengthenTicketMergeType mergeType)
        {
            switch (mergeType)
            {
                case StrengthenTicketMergeType.Material:
                    // if (mView != null)
                    // {
                    //     mView.SetComConsumeItemsActive(false);
                    // }
                    _SetMaterialDropdownActive(true);
                    if (bMaterialMergeGridInited)
                    {
                        _ResetMaterialPreview();
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketMergeSelectReset);
                    }
                    break;
                case StrengthenTicketMergeType.StrengthenTicket:
                    // if (mView != null)
                    // {
                    //     mView.SetComConsumeItemsActive(false);
                    // }
                    _SetMaterialDropdownActive(false);
                    _ResetOwnTicketsPreview();
                    break;
            }
        }

        /// <summary>
        /// 重置格子 材料合成
        /// </summary>
        private void _ResetMaterialPreview()
        {
            if (mCustomItemGrids == null || mDefaultMergeMaterialItemIds == null)
            {
                return;
            }
            for (int i = 0; i < mCustomItemGrids.Count; i++)
            {
                ComItemCustom customItem = mCustomItemGrids[i];
                if (customItem != null && i < mDefaultMergeMaterialItemIds.Count)
                {
                    // if (mView != null)
                    // {
                    //     mView.SetComConsumeItemActive(mDefaultMergeMaterialItemIds[i], true);
                    // }

                    customItem.Init(false, mDefaultMergeMaterialItemIds[i], false, true, true, ComItemCustomClickType.Button);
                    customItem.SetDescStr("");
                    customItem.CustomActive(true);
                }
            }

            _SetTipView(tr_notice_select_left_tickets_merge);
            _EnableMaterialDropdown(false);
            //_ResetMaterialDropdownIndex();

            _PlayToNotReadyStageAnims();
            //置空缓存区
            StrengthenTicketMergeDataManager.GetInstance().ClearCurrSelectMaterialMergeModel();    
        }

        /// <summary>
        /// 重置格子 券融合
        /// </summary>
        private void _ResetOwnTicketsPreview()
        {
            if (mCustomItemGrids == null)
            {
                return;
            }
            if(mView == null)
            {
                return;
            }
            int gridCount = mView.GetFuseMaterialGridCount();
            int customItemGridCount = mCustomItemGrids.Count;
            while (customItemGridCount > gridCount)
            {
                mCustomItemGrids[customItemGridCount - 1].CustomActive(false);
                customItemGridCount--;
            }
            for (int i = 0; i < gridCount; i++)
            {
                ComItemCustom customItem = mCustomItemGrids[i];
                if(customItem == null)
                {
                    continue;
                }
                if (i < mDefaultFuseMaterialItemIds.Count)
                {
                    // if (mView != null)
                    // {
                    //     mView.SetComConsumeItemActive(mDefaultFuseMaterialItemIds[i], true);
                    // }

                    customItem.Init(false, mDefaultFuseMaterialItemIds[i], false, true, true, ComItemCustomClickType.Button);
                    customItem.SetDescStr("");
                    customItem.CustomActive(true);
                }
                else 
                {
                    customItem.Init(false, null, false, true, true, ComItemCustomClickType.Button);
                    customItem.SetExtendImgsActive(new List<int>() { 0 });
                    customItem.SetDescStr(tr_notice_please_select_ticket);
                    customItem.CustomActive(true);
                }
            }

            _SetTipView(tr_notice_select_left_tickets_fuse);

            _PlayToNotReadyStageAnims();
            StrengthenTicketMergeDataManager.GetInstance().ClearTempMaterialFuseModel();
        }

        /// <summary>
        /// 刷新 材料合成 材料格子
        /// </summary>
        /// <param name="model"></param>
        private void _RefreshMaterialSelected(StrengthenTicketMaterialMergeModel model)
        {
            if (mCustomItemGrids == null)
            {
                return;
            }
            if (model == null)
            {
                return;
            }
            var materialModel = model.needMaterialModel;
            if (materialModel == null)
            {
                return;
            }
            var materials = materialModel.needMaterialDatas;
            if (materials == null)
            {
                return;
            }
            if (materials.Count > mCustomItemGrids.Count)
            {
                Logger.LogError("[StrengthenTicketMergeRightItem] - _RefreshMaterialSelected MaterialsCount > GridCount !!!");
                return;
            }
            // if (mView != null)
            // {
            //     mView.SetComConsumeItemsActive(false);
            // }
            for (int i = 0; i < mCustomItemGrids.Count; i++)
            {
                ComItemCustom customItem = mCustomItemGrids[i];
                if (customItem != null && i < materials.Count)
                {
                    // if (mView != null && materials[i] != null && materials[i].tempItemData != null)
                    // {
                    //     mView.SetComConsumeItemActive(materials[i].tempItemData.ItemID, true);
                    // }
                    customItem.Init(false, materials[i].tempItemData, false, true, true, ComItemCustomClickType.Button);
                    int ownCount = ItemDataManager.GetInstance().GetOwnedItemCount(materials[i].tempItemData.ItemID, false);
                    int needCount = materials[i].tempItemData.Count;
                    int needItemId = materials[i].tempItemData.ItemID;
                    if (ownCount < needCount)
                    {
                        customItem.onItemBtnClick = () => {
                            if (mNeedFastBuyItemIds != null && mNeedFastBuyItemIds.Contains(needItemId))
                            {
                                StrengthenTicketMergeDataManager.GetInstance().ReqFastMallBuy(needItemId);
                            }
                            else
                            {
                                ItemComeLink.OnLink(needItemId, 0, false, null, false, true);
                            }
                        };
                    }
                    customItem.SetDescStr(_FormatCustomComItemCount(ownCount, needCount, needItemId), false);
                    customItem.CustomActive(true);
                }
                else
                {
                    customItem.Clear();
                    customItem.CustomActive(false);
                }
            }
            _SetTipView(string.Format(tr_notice_preview_material_merge, model.strengthenLevel, model.previewMinPercent,model.previewMaxPercent));
            bool isMaterialEnough = StrengthenTicketMergeDataManager.GetInstance().CheckMaterialMergeIsEnough();
            if (isMaterialEnough)
            {
                _PlayToWaitingStageAnims();
            }
            else
            {
                _PlayToNotReadyStageAnims();
            }
        }

        private void _RefreshMaterialSelected(int index)
        {
            iMaterialGrowthValue = index;
            var mergeModel = mViewModel as StrengthenTicketMaterialMergeModel;
            var newMergeModel = StrengthenTicketMergeDataManager.GetInstance().GetMaterialMergeStrengthenTicketTableId(mergeModel, index);
            _RefreshMaterialSelected(newMergeModel);
        }

        private string _FormatCustomComItemCount(int ownCount, int needCount, int needItemId)
        {
            string ownCountStr = ownCount.ToString();
            string needCountStr = needCount.ToString();
            if (ownCount < needCount)
            {
                ownCountStr = string.Format(tr_notice_desc_less_color_format, ownCountStr);
            }
            else
            {
                ownCountStr = string.Format(tr_notice_desc_normal_color_format, ownCountStr);
            }
            //新增 need count format
            if (mDefaultMergeOnlyShowNeedItemIds != null)
            {
                for (int i = 0; i < mDefaultMergeOnlyShowNeedItemIds.Count; i++)
                {
                    if (mDefaultMergeOnlyShowNeedItemIds[i] != needItemId)
                    {
                        continue;
                    }
                    if (ownCount < needCount)
                    {
                        needCountStr = string.Format(tr_notice_desc_less_color_format, needCountStr);
                    }
                    else
                    {
                        needCountStr = string.Format(tr_notice_desc_normal_color_format, needCountStr);
                    }
                    return needCountStr;
                }
            }
            //一般 format
            needCountStr = string.Format(tr_notice_desc_normal_color_format, needCount.ToString());
            return string.Format(tr_notice_material_item_count_format, ownCountStr, needCountStr);
        }

        /// <summary>
        /// 刷新 券融合 券格子
        /// </summary>
        private void _RefreshTicketsSelected()
        {
            StrengthenTicketMergeDataManager.GetInstance().TryAddFuseMaterialCanUse();
            if (mCustomItemGrids == null)
            {
                return;
            }
            var fuseModel = StrengthenTicketMergeDataManager.GetInstance().TempMaterialFuseModel;
            if (fuseModel == null)
            {
                return;
            }
            //先刷新 材料道具
            //后刷新 准备阶段的券
            var fuseMaterials = fuseModel.materialModels;
            var fuseTickets = fuseModel.ticketModels;
            if (fuseMaterials == null || fuseTickets == null)
            {
                return;
            }
            int materialCount = fuseMaterials.Count;
            int ticketTotalCount = StrengthenTicketMergeDataManager.GetInstance().TicketFuseReadyCapacity;
            if (materialCount + ticketTotalCount > mCustomItemGrids.Count)
            {
                Logger.LogError("[StrengthenTicketMergeRightItem] - _RefreshMaterialSelected Total MaterialsCount > GridCount !!!");
                return;
            }
            // if (mView != null)
            // {
            //     mView.SetComConsumeItemsActive(false);
            // }
            for (int i = 0; i < mCustomItemGrids.Count; i++)
            {
                ComItemCustom customItem = mCustomItemGrids[i];
                if (customItem == null)
                {
                    continue;
                }
                if (i < materialCount)
                {
                    var fuseMat = fuseMaterials[i];
                    if (fuseMat.fuseNeedItemData == null)
                    {
                        continue;
                    }
                    // if (mView != null)
                    // {
                    //     mView.SetComConsumeItemActive(fuseMat.fuseNeedItemData.ItemID, true);
                    // }
                    customItem.Init(false, fuseMat.fuseNeedItemData, false, true, true, ComItemCustomClickType.Button);
                    int ownCount = ItemDataManager.GetInstance().GetOwnedItemCount(fuseMat.fuseNeedItemData.ItemID, false);
                    int needCount = fuseMat.fuseNeedItemData.Count;
                    int needItemId = fuseMat.fuseNeedItemData.ItemID;
                    if (ownCount < needCount)
                    {
                        customItem.onItemBtnClick = () =>
                        {
                            if (mNeedFastBuyItemIds != null && mNeedFastBuyItemIds.Contains(needItemId))
                            {
                                StrengthenTicketMergeDataManager.GetInstance().ReqFastMallBuy(needItemId);
                            }
                            else
                            {
                                ItemComeLink.OnLink(needItemId, 0, false, null, false, true);
                            }
                        };
                    }
                    customItem.SetDescStr(_FormatCustomComItemCount(ownCount, needCount, needItemId), false);                    
                    customItem.CustomActive(true);
                }
                else if (i >= materialCount && i < (materialCount + ticketTotalCount))
                {
                    int j = i - materialCount;
                    if (customItem != null && j < fuseTickets.Count)
                    {
                        var fuseTicket = fuseTickets[j];
                        if (fuseTicket == null || fuseTicket.ticketItemData == null)
                        {
                            return;
                        }
                        // if (mView != null)
                        // {
                        //     mView.SetComConsumeItemActive(fuseTicket.ticketItemData.TableID, true);
                        // }

                        customItem.Init(false, fuseTicket.ticketItemData, true, true, ComItemCustomClickType.Button);
                        customItem.onItemBtnClick = () =>
                        {
                            if (StrengthenTicketMergeDataManager.GetInstance().CheckFuseTicketCanRemove(fuseTicket))
                            {
                                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketFuseRemoveTicket, fuseTicket);
                            }
                            //Logger.LogError("444");
                        };
                        customItem.SetDescStr(fuseTicket.ticketItemData.Name, true);
                        customItem.SetExtendBtn1ShowAndEnable(true, false);
                        customItem.CustomActive(true);
                    }
                    else
                    {
                        customItem.Init(false, null, true, true, ComItemCustomClickType.Button);
                        customItem.SetExtendImgsActive(new List<int>() { 0 });
                        customItem.SetDescStr("");
                        customItem.CustomActive(true);
                    }
                }
                else
                {
                    customItem.Clear();
                    customItem.CustomActive(false);
                }
            }
            //尝试计算融合概率区间 
            bool canCal = StrengthenTicketMergeDataManager.GetInstance().TryCalculateFuseOutputProbabilityInterval();
            if (!canCal)
            {
                _SetTipView(tr_notice_select_left_tickets_fuse);
            }

            bool enabled = StrengthenTicketMergeDataManager.GetInstance().CheckMaterialFuseIsEnough();
            if (enabled)
            {
                _PlayToWaitingStageAnims();
            }
            else
            {
                _PlayToNotReadyStageAnims();
            }
        }
        /// <summary>
        /// 更新祈福的剩余次数
        /// </summary>
        private void _OnUpdateUseTime()
        {
            if (mTipTxt != null)
            {
                OpActivityData activityData = ActivityDataManager.GetInstance().GetActiveDataFromType(ActivityLimitTimeFactory.EActivityType.BUFF_PRAY_ACTIVITY);
                if (activityData==null)//没有这个活动
                {
                    mTipTxt.CustomActive(false);
                }
                else
                {
                    bool isFinish = StrengthenTicketMergeDataManager.GetInstance().PrayActivityIsFinish;
                    mTipTxt.CustomActive(!isFinish);
                    if (!isFinish)
                    {
                        mTipTxt.text = string.Format(tr_stengthen_tick_mrger_notice_tip, StrengthenTicketMergeDataManager.GetInstance().GetLeftPrayeTimer());
                    }
                }

              
            }
        }
      

        #region UI
        private void _SetTipView(string tip)
        {
            if (mMaterialTips)
            {
                mMaterialTips.text = tip;
            }
        }

        private void _SetMaterialDropdownActive(bool bShow)
        {
            if (mIncreaseRateDrop)
            {
                mIncreaseRateDrop.CustomActive(bShow);
            }
        }

        private void _RefreshMaterialDropdown(List<string> descList)
        {
            if (mIncreaseRateDrop != null)
            {
                mIncreaseRateDrop.Clear();
                mIncreaseRateDrop.Init(descList);
                mIncreaseRateDrop.onSelectIndex = (index) =>
                {
                    _RefreshMaterialSelected(index);
                };
                mIncreaseRateDrop.onDisabledHandle = () =>
                {
                    SystemNotifyManager.SysNotifyTextAnimation(tr_notice_select_left_tickets_merge);
                };
            }
        }

        private void _EnableMaterialDropdown(bool enabled)
        {
            if (mIncreaseRateDrop != null)
            {
                mIncreaseRateDrop.SetEnable(enabled);                
            }
        }

        private void _ResetMaterialDropdownIndex()
        {
            if (mIncreaseRateDrop != null)
            {
                 mIncreaseRateDrop.ResetFirstIndex();
            }
        }

        private int _GetDropdownFirstIndex()
        {
            if (mIncreaseRateDrop != null)
            {
                return mIncreaseRateDrop.GetFirstIndex();
            }
            return 1;
        }

        void _CheckCurrMergeStage()
        {
            if (mView == null)
            {
                return; 
            }
            switch (mView.CurrMergeStage)
            {
                case StrengthenTicketMergeStage.Ready:
                    _SetMergeBtnMaskBtnActive(true);
                    break;
                case StrengthenTicketMergeStage.Waiting:
                    _SetMergeBtnMaskBtnActive(false);
                    break;
                case StrengthenTicketMergeStage.NotReady:
                    _SetMergeBtnMaskBtnActive(true, true);
                    break;
                case StrengthenTicketMergeStage.Process:
                    _SetMergeBtnMaskBtnActive(true);
                    break;
                case StrengthenTicketMergeStage.ReverseReady:
                    _SetMergeBtnMaskBtnActive(true);
                    break;
            }
        }

        void _SetMergeBtnMaskBtnActive(bool bActive, bool bEnable = false)
        {
            if (mMergeBtnMask)
            {
                mMergeBtnMask.CustomActive(bActive);
                mMergeBtnMask.enabled = bEnable;
            }
        }

        #endregion

        #region Callback
        private void _onBtnMergeButtonClick()
        {
            if (mBtnMergeCD == null || mBtnMergeCD.IsBtnWork() == false)
            {
                return;
            }
            _Merge();
            //if (StrengthenTicketMergeDataManager.GetInstance().PrayActivityIsFinish)//祈福活动结束
            //{
               
            //}
            //else
            //{
            //    if (mIsNotShowSkillConfigTip)
            //    {
            //        _Merge();
            //    }
            //    else 
            //    {
            //        if (StrengthenTicketMergeDataManager.GetInstance().IsHaveLeftPrayTimer())
            //        {
            //            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            //            {
            //                //TODO
            //                ContentLabel = string.Format(tr_stengthen_tick_mrger_notice_tip, StrengthenTicketMergeDataManager.GetInstance().GetLeftPrayeTimer()),
            //                IsShowNotify = true,
            //                LeftButtonText = TR.Value("activity_week_hell_pre_task_not_received"),
            //                RightButtonText = TR.Value("activity_week_hell_pre_task_yes_received"),
            //                OnCommonMsgBoxToggleClick = OnUpdateSkillConfigTip,
            //                OnRightButtonClickCallBack = OnOKBtnClick
            //            };
            //            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
            //        }
            //        else
            //        {
            //            _Merge();
            //        }
     
            //    }
            // }
           

        }
        //private void OnOKBtnClick()
        //{
        //    _Merge();
        //}
        //private void OnUpdateSkillConfigTip(bool value)
        //{
        //    mIsNotShowSkillConfigTip = value;
        //}

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

        private void OnUpdateSyntheticFrameTip(bool value)
        {
            StrengthenTicketMergeDataManager.GetInstance().BSyntheticFrameTip = value;
        }

        private void _Merge()
        {
            if (_GetCurrMergeType() == StrengthenTicketMergeType.Material)
            {
                bool isMaterialEnough = StrengthenTicketMergeDataManager.GetInstance().CheckMaterialMergeIsEnough();
                if (isMaterialEnough == false)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(tr_notice_merge_material_not_enough);
                    return;
                }

                if (StrengthenTicketMergeDataManager.GetInstance().BSyntheticFrameTip == false)
                {
                    //材料增幅数值等于100%弹提示
                    if (iMaterialGrowthValue == 1)
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
            }
            else if (_GetCurrMergeType() == StrengthenTicketMergeType.StrengthenTicket)
            {
                bool enabled = StrengthenTicketMergeDataManager.GetInstance().CheckMaterialFuseIsEnough(
                    _StartReqFuseTickets,
                    () =>
                    {
                        mBtnMergeCD.StopBtCD();
                    });
                if (enabled == false)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(tr_notice_merge_material_not_enough);
                }
            }

            mBtnMergeCD.StartBtCD();
        }

        void _StartReqFuseTickets(ulong aGUID, ulong bGUID)
        {
            if (waitToReqFuseTicket != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToReqFuseTicket);
            }
            waitToReqFuseTicket = GameFrameWork.instance.StartCoroutine(_WaitToReqFuseTickets(aGUID, bGUID));
        }

        IEnumerator _WaitToReqFuseTickets(ulong aGUID, ulong bGUID)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketStartMerge);
            //播完一次动画才请求
            float animDuration = _PlayToProcessStageAnims();
            yield return Yielders.GetWaitForSeconds(animDuration);
            //请求合成
            StrengthenTicketMergeDataManager.GetInstance().ReqFuseStrengthenTicket(aGUID, bGUID);
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
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketStartMerge);
            //播完一次动画才请求
            float animDuration = _PlayToProcessStageAnims();
            yield return Yielders.GetWaitForSeconds(animDuration);
            StrengthenTicketMergeDataManager.GetInstance().ReqMaterialMergeStrengthenTicket();
        }

        private void _onSkipAnimToggleToggleValueChange(bool changed)
        {
            bSkipAnim = changed;
        }

        private void _onMergeBtnMaskButtonClick()
        {
            SystemNotifyManager.SysNotifyTextAnimation(tr_notice_merge_material_not_enough);
        }

        private void _OnSrengtheTicketMergeSelectType(UIEvent _event)
        {
            if (_event == null)
            {
                return;
            }
            StrengthenTicketMergeType type = (StrengthenTicketMergeType)_event.Param1;
            _ResetStrengthTicketContent(type);
            if(type==StrengthenTicketMergeType.StrengthenTicket)
            {
                mTipTxt.CustomActive(false);
            }else if(type==StrengthenTicketMergeType.Material)
            {
                mTipTxt.CustomActive(!StrengthenTicketMergeDataManager.GetInstance().PrayActivityIsFinish);
            }

        }

        private void _OnUpdateUseTime(UIEvent uiEvent)
        {
           if(uiEvent==null)
            {
                return;
            }
            _OnUpdateUseTime();
          
        }

        private void _OnUpdatePrayActivityState(UIEvent uiEvent)
        {

            if (uiEvent == null)
            {
                return;
            }
          

            mTipTxt.CustomActive(!StrengthenTicketMergeDataManager.GetInstance().PrayActivityIsFinish);
        }


        private void _OnStengthenTickFreshView(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }
           
           RefreshView(null);
        }
        #region Animation

        //动画播放时 要考虑前后动画的衔接

        /// <summary>
        /// 播放当前合成阶段对应的动画  //是否反向播放
        /// </summary>
        /// <param name="stage"> 合成阶段  </param>
        /// <returns> 返回动画信息 </returns>
        private AnimationClip _PlayResPlaneAnimByMergeStage(StrengthenTicketMergeStage stage, bool bReverse = false)
        {
            if (mResultPlaneComAnim == null || mResultPlaneComAnim.animator == null)
            {
                return null;
            }            
            int currIndex = (int)stage;

           // Logger.LogError("_PlayResPlaneAnimByMergeStage Try To Stage : " + stage.ToString() + " bReverse : " + bReverse);

            if (mView == null)
            {
                return null;
            }
            //当前正在播放的动画 不重复 !!!
            //if (mSelfFrame.CurrMergeStage == stage)
            //{
            //    return null;
            //}
            AnimationClip[] animClips = mResultPlaneComAnim.animator.runtimeAnimatorController.animationClips;
            if (animClips == null)
            {
                return null;
            }
            if (stage == StrengthenTicketMergeStage.ReverseReady)
            {
                currIndex = (int)StrengthenTicketMergeStage.Ready;
            }
            if (currIndex > animClips.Length)
            {
                Logger.LogError("[StrengthenTicketMergeRightItem] - PlayResPlaneAnimByMergeStage currIndex is more than animClipLength");
                return null;
            }
            AnimationClip currClip = animClips[currIndex];
            if (currClip == null)
            {
                return null;
            }
            if (bReverse)
            {
                mResultPlaneComAnim.animator.SetFloat(mResultPlaneComAnim.animSpeedMutifulParam_2, mResultPlaneComAnim.animSpeedMutiful_2);
            }
            else
            {
                mResultPlaneComAnim.animator.SetFloat(mResultPlaneComAnim.animSpeedMutifulParam_2, -mResultPlaneComAnim.animSpeedMutiful_2);
            }
            mResultPlaneComAnim.animator.Play(currClip.name, 0 , 0);
            mView.CurrMergeStage = stage;
            _CheckCurrMergeStage();
           // Logger.LogError("_PlayResPlaneAnimByMergeStage Curr Stage : " + stage.ToString() + " bReverse : " + bReverse);
            return currClip;
        }

        void _PlayToWaitingStageAnims()
        {
            if (mView == null)
            {
                return;
            }
            if (mView.CurrMergeStage == StrengthenTicketMergeStage.NotReady ||
                mView.CurrMergeStage == StrengthenTicketMergeStage.Waiting ||
                mView.CurrMergeStage == StrengthenTicketMergeStage.ReverseReady ||
                mView.CurrMergeStage == StrengthenTicketMergeStage.Ready)
            {
                var readyAnimClip = _PlayResPlaneAnimByMergeStage(StrengthenTicketMergeStage.Ready);
                if (readyAnimClip != null)
                {
                    _StartWaitToPlayStageAnim(readyAnimClip.length, StrengthenTicketMergeStage.Waiting);
                }
            }
            else if (mView.CurrMergeStage == StrengthenTicketMergeStage.Process)
            {
                _StartPlayWaitingAnim();
            }
        }

        void _PlayToNotReadyStageAnims()
        {
            if (mView == null)
            {
                return;
            }
            if (mView.CurrMergeStage == StrengthenTicketMergeStage.Waiting ||
                mView.CurrMergeStage == StrengthenTicketMergeStage.Ready)
            {
                var readyAnimClip = _PlayResPlaneAnimByMergeStage(StrengthenTicketMergeStage.ReverseReady, true);
                if (readyAnimClip != null)
                {
                    _StartWaitToPlayStageAnim(readyAnimClip.length, StrengthenTicketMergeStage.NotReady);
                }
            }
            else
            {
                _StartPlayNotReadyAnim();
            }
        }

        float _PlayToProcessStageAnims()
        {
            float delayDuration = 0f;
            if (mView == null)
            {
                return delayDuration;
            }
            if (mView.CurrMergeStage != StrengthenTicketMergeStage.NotReady)
            {
                if (!bSkipAnim)
                {
                    var animClip = _PlayResPlaneAnimByMergeStage(StrengthenTicketMergeStage.Process);
                    if (animClip == null)
                    {
                        return delayDuration;
                    }
                    delayDuration = animClip.length;
                }
            }
            return delayDuration;
        }

        void _StartWaitToPlayStageAnim(float waitTime, StrengthenTicketMergeStage stage, bool bReverse = false)
        {
            if (waitToPlayNextAnim != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToPlayNextAnim);
            }
            waitToPlayNextAnim = GameFrameWork.instance.StartCoroutine(_WaitToPlayStageAnim(waitTime, stage, bReverse));
        }

        IEnumerator _WaitToPlayStageAnim(float waitTime, StrengthenTicketMergeStage stage, bool bReverse = false)
        {
            yield return Yielders.GetWaitForSeconds(waitTime);
            _PlayResPlaneAnimByMergeStage(stage, bReverse);
        }

        void _StartPlayNotReadyAnim()
        {
            _PlayResPlaneAnimByMergeStage(StrengthenTicketMergeStage.NotReady);
        }
        void _StartPlayReadyAnim()
        {
            _PlayResPlaneAnimByMergeStage(StrengthenTicketMergeStage.Ready);
        }
        void _StartPlayWaitingAnim()
        {
            _PlayResPlaneAnimByMergeStage(StrengthenTicketMergeStage.Waiting);
        }
        void _StartPlayProcessAnim()
        {
            _PlayResPlaneAnimByMergeStage(StrengthenTicketMergeStage.Process);
        }
        #endregion

        #endregion

        #endregion

        #region  PUBLIC METHODS

        public void Create(StrengthenTicketMergeFrameView view, GameObject parent)
        {
            this._mParentGo = parent;
            this.mView = view;

            if (this._mSelfGo == null)
            {
                this._mSelfGo = AssetLoader.GetInstance().LoadResAsGameObject(StrengthenTicketMergeFrame.RIGHT_VIEW_FRAME_RES_PATH);
            }
            if (this._mSelfGo != null)
            {
                _mBind = _mSelfGo.GetComponent<ComCommonBind>();
            }
            if (_mBind != null)
            {
                mBtnMerge = _mBind.GetCom<Button>("BtnMerge");
                if (null != mBtnMerge)
                {
                    mBtnMerge.onClick.AddListener(_onBtnMergeButtonClick);
                }
                mSkipAnimToggle = _mBind.GetCom<Toggle>("SkipAnimToggle");
                if (null != mSkipAnimToggle)
                {
                    mSkipAnimToggle.onValueChanged.AddListener(_onSkipAnimToggleToggleValueChange);
                }
                mMaterialItemRoot = _mBind.GetGameObject("MaterialItemRoot");
                mMaterialTips = _mBind.GetCom<Text>("MaterialTips");
                mIncreaseRateDrop = _mBind.GetCom<ComDropDownCustom>("IncreaseRateDrop");
                mResultPlaneComAnim = _mBind.GetCom<ComStrengthenTicketAnim>("ResultPlaneComAnim");
                mMergeBtnMask = _mBind.GetCom<Button>("MergeBtnMask");
                if (null != mMergeBtnMask)
                {
                    mMergeBtnMask.onClick.AddListener(_onMergeBtnMaskButtonClick);
                }

                if (mBtnMerge != null)
                {
                    mBtnMergeCD = mBtnMerge.gameObject.GetComponent<SetComButtonCD>();
                }
                mTipTxt = _mBind.GetCom<Text>("TipTxt");
            }
            Utility.AttachTo(_mSelfGo, _mParentGo);

            if (_mSelfGo)
            {
                _mSelfGo.CustomActive(false);
            }
            _Init();
        }

        public void Destroy()
        {
            _Clear();
            _ClearBase();
        }

        public void RefreshView(object model)
        {
            if (_GetCurrMergeType() == StrengthenTicketMergeType.Material)
            {
                var mCurrentSelectMergeModel = StrengthenTicketMergeDataManager.GetInstance().CurrSelectMaterialMergeModel;
                if (mCurrentSelectMergeModel == null)
                {
                    return;
                }
                _RefreshMaterialSelected(mCurrentSelectMergeModel);
            }
            else if (_GetCurrMergeType() == StrengthenTicketMergeType.StrengthenTicket)
            {
                _RefreshTicketsSelected();
            }
        }

        public void Show()
        {
            _mSelfGo.CustomActive(true);
        }

        public void Hide()
        {
            _mSelfGo.CustomActive(false);
        }

        public object GetViewModel()
        {
            return mViewModel;
        }


        public void SetStrengthenTicketMergeSelectTicket(StrengthenTicketMaterialMergeModel mergeModel)
        {
            mViewModel = mergeModel;
            if (mViewModel == null)
            {
                return;
            }
            //刷新dropdown list
            // merge model 可以是 display model or total model 
            List<string> descList = StrengthenTicketMergeDataManager.GetInstance().GetMaterialMergeIncreaseLevelDescList(mergeModel);
            _RefreshMaterialDropdown(descList);
            _EnableMaterialDropdown(true);
            _ResetMaterialDropdownIndex();
            _RefreshMaterialSelected(_GetDropdownFirstIndex());
        }

        public void SetStrengthenTicketFuseRemoveTicket()
        {
            _RefreshTicketsSelected();
        }

        public void SetStrengthenTicketFuseAddTicket()
        {
            _RefreshTicketsSelected();
        }

        public void SetStrengthenTicketMergeSuccess()
        {
            var mCurrentSelectMergeModel = StrengthenTicketMergeDataManager.GetInstance().CurrSelectMaterialMergeModel;
            if (mCurrentSelectMergeModel == null)
            {
                return;
            }
            _RefreshMaterialSelected(mCurrentSelectMergeModel);
        }

        public void SetStrengthenTicketFuseSuccess()
        {
           //_RefreshTicketsSelected();
            _ResetOwnTicketsPreview();
        }

        public void SetStrengthenTicketMergeFailed()
        {
            var mCurrentSelectMergeModel = StrengthenTicketMergeDataManager.GetInstance().CurrSelectMaterialMergeModel;
            if (mCurrentSelectMergeModel == null)
            {
                return;
            }
            _RefreshMaterialSelected(mCurrentSelectMergeModel);
        }

        public void SetStrengthenTicketFuseFailed()
        {
            _RefreshTicketsSelected();
        }

        public void SetStrengthenTicketFuseCalPercent()
        {
            var tempFuseModel = StrengthenTicketMergeDataManager.GetInstance().TempMaterialFuseModel;
            if (tempFuseModel == null)
            {
                return;
            }
            _SetTipView(string.Format(
                tr_notice_preview_ticket_fuse, tempFuseModel.predictMinLevel, tempFuseModel.perdictMinPercent,
                tempFuseModel.predictMaxLevel, tempFuseModel.perdictMaxPercent));
        }

        #endregion
    }
}