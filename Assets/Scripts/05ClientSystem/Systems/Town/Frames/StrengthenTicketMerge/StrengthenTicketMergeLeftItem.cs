using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class StrengthenTicketMergeLeftItem : CustomClientFrameItem
        // ICustomClientFrameItem<StrengthenTicketMergeFrame, object>
    {
        #region Model Params

        object mViewModel = null;
        StrengthenTicketMergeFrameView mView = null;

        //材料融合
        ComItemCustom mFirstMaterialItem = null;
        private bool bMaterialMergeItemListInited = false;           //材料合成 用
        private UnityEngine.Coroutine waitToMaterialMergeItemToggleOn;

        //券融合
        List<ComItemCustom> mReadyComItemCustoms = new List<ComItemCustom>();


        #endregion

        #region View Params

        private ComUIListScript mComList = null;
        private Text mTitle = null;
        private Text mNocache = null;

        #endregion

        #region PRIVATE METHODS

        protected override void _Init()
        {
            _BindUIEvent();
        }

        protected override void _Clear()
        {
            _UnBindUIEvent();

            mViewModel = null;
            mView = null;

            if (mFirstMaterialItem != null)
            {
                mFirstMaterialItem.Clear();
            }
            bMaterialMergeItemListInited = false;
            if (waitToMaterialMergeItemToggleOn != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToMaterialMergeItemToggleOn);
                waitToMaterialMergeItemToggleOn = null;
            }

            if (mReadyComItemCustoms != null)
            {
                for (int i = 0; i < mReadyComItemCustoms.Count; i++)
                {
                    mReadyComItemCustoms[i].Clear();
                }
                mReadyComItemCustoms.Clear();
            }

            if (mComList != null)
            {
                mComList.SetElementAmount(0);
            }
            mComList = null;
            mTitle = null;
            mNocache = null;
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenTicketMergeSelectType, _OnSrengtheTicketMergeSelectType);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenTicketMergeSelectType, _OnSrengtheTicketMergeSelectType);
        }

        private void _RefreshStrengthTicketContent(StrengthenTicketMergeType mergeType)
        {
            switch (mergeType)
            {
                case StrengthenTicketMergeType.Material:
                    _RefreshMaterialMergeStrengthenTickets();
                    _SetTitleName(TR.Value("strengthen_merge_material_default_tip"));
                    break;
                case StrengthenTicketMergeType.StrengthenTicket:
                    _RefreshFuseStrengthenTickets();
                    _SetTitleName(TR.Value("strengthen_merge_ticket_default_tip"));
                    break;
            }
        }

        private void _RefreshMaterialMergeStrengthenTickets()
        {
            mNocache.CustomActive(false);
            var mDisplayMaterialMergeModels = StrengthenTicketMergeDataManager.GetInstance().GetDisplayMaterialMergeTicketModels();        //需要展示的数据
            if (mDisplayMaterialMergeModels == null || mDisplayMaterialMergeModels.Count == 0)
            {
                mNocache.CustomActive(true);
                return;
            }
            //设置缓存
            mViewModel = mDisplayMaterialMergeModels;

            if (mComList == null)
            {
                return;
            }
            if (mComList.IsInitialised() == false)
            {
                mComList.Initialize();
                mComList.onBindItem = (GameObject go) =>
                {
                    ComItemCustom customItem = go.GetComponent<ComItemCustom>();
                    return customItem;
                };
            }
            mComList.onItemVisiable = (var) =>
            {
                if (var == null)
                {
                    return;
                }
                int iIndex = var.m_index;
                if (iIndex >= 0 && iIndex < mDisplayMaterialMergeModels.Count)
                {
                    ComItemCustom customItem = var.gameObjectBindScript as ComItemCustom;
                    if (customItem == null)
                    {
                        return;
                    }
                    var model = mDisplayMaterialMergeModels[iIndex];
                    if (model == null)
                    {
                        return;
                    }
                    if (!model.bDisplay)
                    {
                        return;
                    }
                    customItem.Init(false, model.displayItemTableId, false, true, true, ComItemCustomClickType.Toggle);
                    customItem.SetDescStr(model.name, true);
                    customItem.SetCustomItemSelectActive(false);
                    customItem.onTitleToggleClick = (isOn) =>
                    {
                        customItem.SetCustomItemSelectActive(isOn);
                        if (isOn)
                        {
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketMergeSelectTicket, model);
                        }
                    };

                    if (iIndex == 0)
                    {
                        mFirstMaterialItem = customItem;
                    }
                    else if (iIndex == mDisplayMaterialMergeModels.Count - 1)
                    {
                        bMaterialMergeItemListInited = true;
                    }
                }
            };
            mComList.OnItemRecycle = (var) =>
            {
                if (var == null)
                {
                    return;
                }
                ComItemCustom customItem = var.gameObjectBindScript as ComItemCustom;
                if (customItem != null)
                {
                    customItem.Clear();
                }
            };
            mComList.SetElementAmount(mDisplayMaterialMergeModels.Count);
        }

        private void _RefreshFuseStrengthenTickets()
        {
            mNocache.CustomActive(false);
            List<StrengthenTicketFuseItemData> ownStrengthenTickets = StrengthenTicketMergeDataManager.GetInstance().GetStrengthenTicketFuseItemDatas();
            if (ownStrengthenTickets == null || ownStrengthenTickets.Count == 0)
            {
                mNocache.CustomActive(true);
            }
            //设置缓存
            mViewModel = ownStrengthenTickets;

            //清理准备区
            if (mReadyComItemCustoms != null)
            {
                for (int i = 0; i < mReadyComItemCustoms.Count; i++)
                {
                    mReadyComItemCustoms[i].Clear();
                }
                mReadyComItemCustoms.Clear();
            }

            if (mComList == null)
            {
                return;
            }
            if (mComList.IsInitialised() == false)
            {
                mComList.Initialize();
                mComList.onBindItem = (GameObject go) =>
                {
                    ComItemCustom customItem = go.GetComponent<ComItemCustom>();
                    return customItem;
                };
            }
            mComList.onItemVisiable = (var) =>
            {
                if (var == null)
                {
                    return;
                }
                int iIndex = var.m_index;
                if (iIndex >= 0 && iIndex < ownStrengthenTickets.Count + 1)
                {
                    ComItemCustom customItem = var.gameObjectBindScript as ComItemCustom;
                    if (customItem == null)
                    {
                        return;
                    }
                    //需要添加额外icon 特殊处理
                    if (iIndex >= ownStrengthenTickets.Count)
                    {
                        customItem.Init(false, null, true, true, ComItemCustomClickType.Button);
                        customItem.SetExtendImgsActive(new List<int> { 0, 1 });
                        customItem.SetCustomItemSelectActive(false);
                        customItem.SetDescStr("");
                        customItem.onItemBtnClick = () =>
                        {
                            if (mView != null)
                            {
                                int itemId = mView.GetGotoGetBindItemId();
                                ItemComeLink.OnLink(itemId, 0, false);
                            }
                        };
                        customItem.transform.SetAsLastSibling();
                        customItem.CustomActive(true);
                        return;
                    }
                    
                    var model = ownStrengthenTickets[iIndex];
                    if (model == null)
                    {
                        return;
                    }
                    if (model.ticketItemData == null)
                    {
                        return;
                    }
                    customItem.Init(false, model.ticketItemData, false, true, ComItemCustomClickType.Button);
                    customItem.SetDescStr(model.ticketItemData.Name, true);
                    customItem.SetCustomItemSelectActive(false);
                    _SetCustomComItemCountWithFuseTicket(customItem, model);
                    //Logger.LogError("left custom data bReady : "+bReady.ToString() +" ||| data : "+model.ticketItemData.GUID + " || count : "+model.ticketItemData.Count);

                    customItem.onItemBtnClick = () =>
                    {
                        if (model.canFuse == false)
                        {
                            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_merge_fuse_limitlevel_max"));
                            return;
                        }
                        if (StrengthenTicketMergeDataManager.GetInstance().CheckFuseTicketCanAdd(model))
                        {
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketFuseAddTicket);
                            if (mReadyComItemCustoms != null)
                            {
                                mReadyComItemCustoms.Add(customItem);
                            }
                            _SetCustomComItemCountWithFuseTicket(customItem, model);
                        }
                        //Logger.LogError("222");
                    };
                    customItem.onExtendBtn1Click = () =>
                    {
                        if (StrengthenTicketMergeDataManager.GetInstance().CheckFuseTicketCanRemove(model))
                        {
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnStrengthenTicketFuseRemoveTicket);
                            if (mReadyComItemCustoms != null)
                            {
                                mReadyComItemCustoms.Remove(customItem);
                            }
                            _SetCustomComItemCountWithFuseTicket(customItem, model);
                        }
                        //Logger.LogError("333");                        
                    };
                }
            };
            mComList.OnItemRecycle = (var) =>
            {
                if (var == null)
                {
                    return;
                }
                ComItemCustom customItem = var.gameObjectBindScript as ComItemCustom;
                if (customItem != null)
                {
                    customItem.Clear();
                }
            };
            mComList.SetElementAmount(ownStrengthenTickets.Count + 1);
        }

        private void _SetCustomComItemCountWithFuseTicket(ComItemCustom customItem, StrengthenTicketFuseItemData model)
        {
            if (model == null || customItem == null)
            {
                return;
            }
            bool bReady = StrengthenTicketMergeDataManager.GetInstance().CheckFuseTicketOnReady(model);
            //customItem.SetExtendBtn1ShowAndEnable(bReady,bReady);//不显示额外按钮
            if (bReady)
            {
                int bReadyCount = StrengthenTicketMergeDataManager.GetInstance().CheckFuseTicketNumOnReadyByTableId(model);
                customItem.SetCustomItemCount(false, string.Format(TR.Value("strengthen_merge_fuse_item_count_format"),bReadyCount, model.ticketItemData.Count));
                customItem.SetCustomItemSelectActive(true);
            }
            else
            {
                customItem.SetCustomItemCount();
                customItem.SetCustomItemSelectActive(false);
            }
        }

        private void _SetTitleName(string title)
        {
            if (mTitle)
            {
                mTitle.text = title;
            }
        }

        private StrengthenTicketMergeType _GetCurrMergeType()
        {
            if (mView == null)
            {
                return StrengthenTicketMergeType.Count;
            }
            return mView.MergeType;
        }

        #region Callback
        private void _OnSrengtheTicketMergeSelectType(UIEvent _event)
        {
            if (_event == null)
            {
                return;
            }
            StrengthenTicketMergeType type = (StrengthenTicketMergeType)_event.Param1;
            _RefreshStrengthTicketContent(type);
        }

        #endregion

        #endregion

        #region  PUBLIC METHODS

        public void Create(StrengthenTicketMergeFrameView view, GameObject parent)
        {
            this._mParentGo = parent;
            this.mView = view;

            if (this._mSelfGo == null)
            {
                this._mSelfGo = AssetLoader.GetInstance().LoadResAsGameObject(StrengthenTicketMergeFrame.LEFT_VIEW_FRAME_RES_PATH);
            }
            if (this._mSelfGo != null)
            {
                _mBind = _mSelfGo.GetComponent<ComCommonBind>();
            }
            if (_mBind != null)
            {
                mComList = _mBind.GetCom<ComUIListScript>("ComList");
                mTitle = _mBind.GetCom<Text>("Title");
                mNocache = _mBind.GetCom<Text>("nocache");
            }
            Utility.AttachTo(_mSelfGo, _mParentGo);

            mNocache.CustomActive(false);
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
            if (_GetCurrMergeType() == StrengthenTicketMergeType.StrengthenTicket)
            {
                _RefreshFuseStrengthenTickets();
            }
        }

        public object GetViewModel()
        {
            return mViewModel;
        }

        public void Show()
        {
            _mSelfGo.CustomActive(true);
        }

        public void Hide()
        {
            _mSelfGo.CustomActive(false);
        }

        public void SetStrengthenTicketFuseRemoveTicket(StrengthenTicketFuseItemData ticketFuseItemData)
        {
            //不能这么操作
            //_RefreshFuseStrengthenTickets();
            if (mReadyComItemCustoms == null)
            {
                return;
            }
            if (ticketFuseItemData == null || ticketFuseItemData.ticketItemData == null)
            {
                return;
            }
            for (int i = 0; i < mReadyComItemCustoms.Count; i++)
            {
                var readyComItem = mReadyComItemCustoms[i];
                if (readyComItem == null || readyComItem.M_ItemData == null)
                {
                    continue;
                }
                if (readyComItem.M_ItemData.TableID.Equals(ticketFuseItemData.ticketItemData.TableID))
                {
                    _SetCustomComItemCountWithFuseTicket(readyComItem, ticketFuseItemData);
                    mReadyComItemCustoms.Remove(readyComItem);
                    break;
                }
            }
        }

        public void SetStrengthenTicketFuseAddTicket()
        {
            //不能这么操作
            //_RefreshFuseStrengthenTickets();
        }

        public void SetStrengthenTicketFuseSuccess()
        {
            _RefreshFuseStrengthenTickets();
        }

        public void SetResetItemSelect()
        {
            if (waitToMaterialMergeItemToggleOn != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToMaterialMergeItemToggleOn);
            }
            waitToMaterialMergeItemToggleOn = GameFrameWork.instance.StartCoroutine(_WaitToSetFirstMaterialItemToggleOn());

            //Logger.LogError("isOn 222 " + mFirstMaterialItem.GetCustomItemToggleIsOn());
        }

        IEnumerator _WaitToSetFirstMaterialItemToggleOn()
        {
            float waitTime = 0.2f;
            if (mView != null)
            {
                waitTime = mView.GetWaitToSelectMaterialFirstItemTime();
            }
            yield return Yielders.GetWaitForSeconds(waitTime);
            if (mFirstMaterialItem != null && bMaterialMergeItemListInited)
            {
                mFirstMaterialItem.SetCustomItemToggleOn(true);
                //Logger.LogError("isOn 333 " + mFirstMaterialItem.GetCustomItemToggleIsOn());
            }            
        }

        #endregion
    }
}