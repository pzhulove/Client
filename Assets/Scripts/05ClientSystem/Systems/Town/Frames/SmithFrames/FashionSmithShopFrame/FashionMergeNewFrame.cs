using Network;
using Protocol;
using ProtoTable;
using Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public enum FashionMergeType
    {
        [System.ComponentModel.DescriptionAttribute("item_slot_fashion_head")]
        FMT_HEAD = 0,
        [System.ComponentModel.DescriptionAttribute("item_slot_fashion_chest")]
        FMT_CHEST,
        [System.ComponentModel.DescriptionAttribute("item_slot_fashion_upper_body")]
        FMT_UPPER_BODY,
        [System.ComponentModel.DescriptionAttribute("item_slot_fashion_waist")]
        FMT_WAIST,
        [System.ComponentModel.DescriptionAttribute("item_slot_fashion_lower_body")]
        FMT_LOWER_BODY,
        MT_COUNT,
    }

    class FashionMergeFrameData
    {
        public ItemData fashionA;
        public ItemData fashionB;
        public FashionMergeType eMergeType;
    }

    class FashionMergeNewFrameData
    {
        public ulong bindId;
    }
    
    class FashionMergeNewFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/FashionSmithShop/FashionMergeNewFrame";
        }

        [UIControl("", typeof(ComFashionMergeDataBinder))]
        ComFashionMergeDataBinder mDataBinder;
        [UIControl("", typeof(StateController))]
        StateController comSkyMode;
        [UIControl("FrameType", typeof(StateController))]
        StateController m_FrameTypeStateCol;
        string mKeySky = "sky";
        string mKeyGoldSky = "gold_sky";
        string mFrameTypeNomal = "Nomal";
        string mFrameActivity = "ChangeSectionActivity";
        FashionMergeNewFrameData data = null;
        int autoEquipState = 0;
        public static void CommandOpen(object argv)
        {
            bool bOption = (bool)argv;
            if(bOption)
            {
                if(FashionMergeManager.GetInstance().HasSkyFashion(FashionMergeManager.GetInstance().FashionPart))
                {
                    FashionMergeManager.GetInstance().FashionPart = ProtoTable.ItemTable.eSubType.ST_NONE;
                }

                if (FashionMergeManager.GetInstance().FashionPart == ProtoTable.ItemTable.eSubType.ST_NONE)
                {
                    FashionMergeManager.GetInstance().FashionPart = FashionMergeManager.GetInstance().GetDefaultFashionPart();
                }
            }

            ClientSystemManager.GetInstance().OpenFrame<FashionMergeNewFrame>(FrameLayer.Middle, argv);
        }

        public static void OpenLinkFrame(string strParam)
        {
            if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_FASHION_MERGO))
            {
                return;
            }

            var tokens = strParam.Split('|');
            int mode = 0;
            int type = 0;
            int part = (int)ProtoTable.ItemTable.eSubType.FASHION_HEAD;
            ulong linkId = 0;
            int frameType = 0;//0代表天空套界面界面，1代表活动时装合成
            if(tokens.Length == 5)
            {
                if(int.TryParse(tokens[0],out mode) && int.TryParse(tokens[1],out frameType) && int.TryParse(tokens[2],out type) && int.TryParse(tokens[3],out part) && ulong.TryParse(tokens[4],out linkId))
                {
                    bool bOption = mode == 1;
                    if (bOption)
                    {
                        if (FashionMergeManager.GetInstance().HasSkyFashion(FashionMergeManager.GetInstance().FashionPart))
                        {
                            FashionMergeManager.GetInstance().FashionPart = ProtoTable.ItemTable.eSubType.ST_NONE;
                        }

                        if (FashionMergeManager.GetInstance().FashionPart == ProtoTable.ItemTable.eSubType.ST_NONE)
                        {
                            FashionMergeManager.GetInstance().FashionPart = FashionMergeManager.GetInstance().GetDefaultFashionPart();
                        }

                        if ((FashionType)type != FashionType.FT_NATIONALDAY)
                        {
                            FashionMergeManager.GetInstance().RecordNomalFashionType = (FashionType)type;
                        }

                        FashionMergeManager.GetInstance().FashionType = (FashionType)type;
                    }
                    else
                    {
                        FashionMergeManager.GetInstance().FashionPart = (ProtoTable.ItemTable.eSubType)part;
                        FashionMergeManager.GetInstance().FashionType = (FashionType)type;
                    }

                    if (frameType == 0)
                    {
                        FashionMergeManager.GetInstance().FashionType = FashionMergeManager.GetInstance().RecordNomalFashionType;
                    }

                    FashionMergeNewFrameData data = new FashionMergeNewFrameData();
                    data.bindId = linkId;

                    if (ClientSystemManager.GetInstance().IsFrameOpen<FashionMergeNewFrame>())
                    {
                        ClientSystemManager.GetInstance().CloseFrame<FashionMergeNewFrame>();
                    }

                    ClientSystemManager.GetInstance().OpenFrame<FashionMergeNewFrame>(FrameLayer.Middle, data);
                }
            }
        }

        protected sealed override void _OnOpenFrame()
        {
            if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_FASHION_MERGO))
            {
                ClientSystemManager.GetInstance().CloseFrame<FashionMergeNewFrame>();
                return;
            }
            data = userData as FashionMergeNewFrameData;
            if(null == data)
            {
                data = new FashionMergeNewFrameData();
                data.bindId = 0;
                int type = 0;
                int part = (int)ProtoTable.ItemTable.eSubType.FASHION_HEAD;
                FashionMergeManager.GetInstance().FashionPart = (ProtoTable.ItemTable.eSubType)part;
                FashionMergeManager.GetInstance().FashionType = (FashionType)type;

                Logger.LogErrorFormat("you should not open FashionMergeNewFrame by default,instead of calling OpenLinkFrame frame !!! to 郝晓亮 !!!");
            }
            _AddButton("BG/Title/Close", () => { frameMgr.CloseFrame(this); });
            _AddButton("ChangeMode", () => 
            {
                FashionMergeManager.GetInstance().FashionType =
                FashionMergeManager.GetInstance().FashionType == FashionType.FT_GOLD_SKY ?
                FashionType.FT_SKY : FashionType.FT_GOLD_SKY;
                _UpdateSkyItems();
                FashionMergeManager.GetInstance().ClearRedPoit();
            });
            _AddButton("GoFashionBag", () =>
            {
                ClientSystemManager.GetInstance().OpenFrame<PackageNewFrame>(FrameLayer.Middle,EPackageOpenMode.Fashion);
                FashionMergeManager.GetInstance().ClearRedPoit();
            });
            
            _InitSkySuit();
            _InitNormalSuit();
            _UpdateSkyItems();
            //this code must be after _UpdateSkyItems
            _InitBindItem();

            if (FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
            {
                m_FrameTypeStateCol.Key = mFrameActivity;
            }
            else
            {
                m_FrameTypeStateCol.Key = mFrameTypeNomal;
            }

            _InitAutoEquipState();
            UpdateProbabilityAscensionRoot();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnNormalFashionModeChanged, _OnNormalFashionModeChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnFashionMergeNotify, _OnFashionMergeNotify);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnFashionSpecialFly, _OnFashionSpecialFly);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnFashionAutoEquip, _OnFashionAutoEquip);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityLimitTimeDataUpdate, _OnLimitTimeDataUpdate);
        }

        void _UpdateSkyItems()
        {
            if (null != comSkyMode)
            {
                if (FashionMergeManager.GetInstance().FashionType == FashionType.FT_GOLD_SKY)
                    comSkyMode.Key = mKeyGoldSky;
                else
                    comSkyMode.Key = mKeySky;
            }

            if (null != mDataBinder)
            {
                mDataBinder.SetSkyMode(FashionMergeManager.GetInstance().FashionType);
                mDataBinder.SetSkyFashionPart(FashionMergeManager.GetInstance().FashionPart);
                mDataBinder.SetNormalFashionPart(FashionMergeManager.GetInstance().FashionPart);
                mDataBinder.SetSlotPart(FashionMergeManager.GetInstance().FashionPart);
                mDataBinder.SetSkySuitName();
            }

            _LoadSkyDatas(FashionMergeManager.GetInstance().FashionType);

            if (null != mDataBinder)
            {
                mDataBinder.UpdateSkyStatus();
                mDataBinder.ForceUpdateSkyStatus();
                mDataBinder.UpdateWindProcess();
            }
        }

        void _OnNormalFashionModeChanged(UIEvent uiEvent)
        {
            if(null != mDataBinder)
            {
                mDataBinder.SetNormalFashionPart(FashionMergeManager.GetInstance().FashionPart);
            }
        }

        void _OnFashionMergeNotify(UIEvent uiEvent)
        {
            if(null != mDataBinder)
            {
                mDataBinder.EnableMergeState();
                mDataBinder.UpdateSlotItems();
            }
        }

        void _InitSkySuit()
        {
            int startId = FashionMergeManager.GetInstance().SkySuitID;
            int key = FashionMergeManager.GetInstance().GetFashionByKey(FashionMergeManager.GetInstance().FashionType, PlayerBaseData.GetInstance().JobTableID, startId, ProtoTable.ItemTable.eSubType.FASHION_HEAD);
            if (0 == key)
            {
                startId = 1;
                FashionMergeManager.GetInstance().SkySuitID = startId;
            }
        }

        void _InitNormalSuit()
        {
            int startId = FashionMergeManager.GetInstance().NormalSuitID;
            int key = FashionMergeManager.GetInstance().GetFashionByKey(FashionMergeManager.GetInstance().FashionType, PlayerBaseData.GetInstance().JobTableID, startId, ProtoTable.ItemTable.eSubType.FASHION_HEAD);
            if (0 == key)
            {
                startId = 1;
                FashionMergeManager.GetInstance().SkySuitID = startId;
            }
        }

        void _InitBindItem()
        {
            if(null != mDataBinder)
            {
                mDataBinder.SetBindItem(data.bindId);
            }
        }

        void _InitAutoEquipState()
        {
            autoEquipState = CountDataManager.GetInstance().GetCount(CounterKeys.FASHION_MERGE_AUTO_EQUIP_STATE);
            if(autoEquipState == 0)
            {
                mAutoEquip.isOn = false;
            }
            else
            {
                mAutoEquip.isOn = true;
            }
        }

        void _LoadSkyDatas(FashionType eFashionType)
        {
            if(null == mDataBinder)
            {
                Logger.LogErrorFormat("mDataBinder is null !!");
                return;
            }

            var fashions = GamePool.ListPool<int>.Get();
            FashionMergeManager.GetInstance().GetFashionItemsByTypeAndOccu(eFashionType, PlayerBaseData.GetInstance().JobTableID, FashionMergeManager.GetInstance().SkySuitID, ref fashions);

            if(null != fashions)
            {
                for(int i = 0; i < fashions.Count; ++i)
                {
                    var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(fashions[i]);
                    if(null == item)
                    {
                        continue;
                    }

                    var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(fashions[i]);
                    if(null == itemData)
                    {
                        continue;
                    }

                    mDataBinder.SetSlotItems(itemData, _OnItemClicked, item.SubType);
                }
            }
            GamePool.ListPool<int>.Release(fashions);
        }

        void _OnItemClicked(GameObject obj, ItemData item)
        {
            if(null != item)
            {
                if(null != mDataBinder)
                {
                    var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(item.TableID);
                    if(null != itemTable)
                    {
                        if(itemTable.SubType != ProtoTable.ItemTable.eSubType.FASHION_HAIR)
                        {
                            mDataBinder.SetSkyFashionPart(itemTable.SubType);
                            mDataBinder.SetNormalFashionPart(itemTable.SubType);
                            mDataBinder.SetSlotPart(itemTable.SubType);
                            FashionMergeManager.GetInstance().FashionPart = itemTable.SubType;
                        }
                        else
                        {
                            if(!mDataBinder.IsWindUnLock())
                            {
                                if (FashionMergeManager.GetInstance().FashionType == FashionType.FT_SKY)
                                {
                                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("fashion_wind_unlock_hint_sky_lock"));
                                }
                                else if (FashionMergeManager.GetInstance().FashionType == FashionType.FT_GOLD_SKY)
                                {
                                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("fashion_wind_unlock_hint_gold_sky_lock"));
                                }
                            }
                            else
                            {
                                if (FashionMergeManager.GetInstance().FashionType == FashionType.FT_SKY)
                                {
                                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("fashion_wind_unlock_hint_sky_unlock"));
                                }
                                else if (FashionMergeManager.GetInstance().FashionType == FashionType.FT_GOLD_SKY)
                                {
                                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("fashion_wind_unlock_hint_gold_sky_unlock"));
                                }
                            }
                        }
                    }
                }
            }
        }

        void _SetGoldSkyMode(bool bOpen)
        {
            bool bCurOpen = FashionMergeManager.GetInstance().FashionType == FashionType.FT_GOLD_SKY;
            if(bOpen != bCurOpen)
            {
                FashionMergeManager.GetInstance().FashionType = bOpen ? FashionType.FT_GOLD_SKY : FashionType.FT_SKY;
                _UpdateSkyItems();
                FashionMergeManager.GetInstance().ClearRedPoit();
            }
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnNormalFashionModeChanged, _OnNormalFashionModeChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnFashionMergeNotify, _OnFashionMergeNotify);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnFashionSpecialFly, _OnFashionSpecialFly);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnFashionAutoEquip, _OnFashionAutoEquip);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityLimitTimeDataUpdate, _OnLimitTimeDataUpdate);
            InvokeMethod.RemoveInvokeCall(this);
            data = null;
            if (null != mDataBinder)
            {
                mDataBinder.ClearSlotValues();
            }
        }

        void _OnFashionAutoEquip(UIEvent uiEvent)
        {
            if(autoEquipState == 1)
            {
                mDataBinder.TryAutoEquipFashion();
            }
        }

        void _OnLimitTimeDataUpdate(UIEvent uiEvent)
        {
            UpdateProbabilityAscensionRoot();
        }
        
        void _OnShowTips(ItemData result)
        {
            if (result == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(result);
        }

        void _OnFashionSpecialFly(UIEvent uiEvent)
        {
            ItemData data = uiEvent.Param3 as ItemData;
            if(null == data)
            {
                return;
            }

            ProtoTable.FashionComposeSkyTable skyItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(data.TableID);
            if(null == skyItem)
            {
                return;
            }

            if(null == mDataBinder)
            {
                return;
            }

            FashionType eTargetFashionType = skyItem.Type == 1 ? FashionType.FT_SKY : FashionType.FT_GOLD_SKY;

            Vector3 startWorldPosition = Vector3.zero;
            startWorldPosition.x = (float)uiEvent.Param1;
            startWorldPosition.y = (float)uiEvent.Param2;
            startWorldPosition.z = 0.0f;

            bool isTypeSame = eTargetFashionType == FashionMergeManager.GetInstance().FashionType;

            if (FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType))
            {
                isTypeSame = true;
            }

            ItemData itemWind = uiEvent.Param4 as ItemData;

            if(isTypeSame)
            {
                mDataBinder.FlyEffect2Slot(9, startWorldPosition, (ProtoTable.ItemTable.eSubType)data.SubType, mDataBinder.mFlyLength,
                    () => 
                    {
                        if (null != mDataBinder.mOnFlyStart)
                        {
                            mDataBinder.mOnFlyStart.Invoke();
                        }
                    },
                    () =>
                    {
                        if (null != mDataBinder)
                        {
                            if (eTargetFashionType == FashionType.FT_GOLD_SKY)
                            {
                                mDataBinder.PlaySpecialSlotEffect((ProtoTable.ItemTable.eSubType)data.SubType);
                            }
                            else
                            {
                                mDataBinder.PlayNormalSlotEffect((ProtoTable.ItemTable.eSubType)data.SubType);
                            }

                            if (null == itemWind && !FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType)||
                            FashionMergeManager.GetInstance().IsChangeSectionActivity(FashionMergeManager.GetInstance().FashionType) && FashionMergeManager.GetInstance().ChangeFashionIsAllMerged == false)
                            {
                                _LoadSkyDatas(FashionMergeManager.GetInstance().FashionType);

                                if (null != mDataBinder)
                                {
                                    mDataBinder.UpdateSkyStatus();
                                    mDataBinder.ForceUpdateSkyStatus();
                                    mDataBinder.UpdateWindProcess();
                                    mDataBinder.DisableFlyState();
                                }
                            }
                            else
                            {
                                if (null != mDataBinder)
                                {
                                    mDataBinder.UpdateSlotSkyStatus((ProtoTable.ItemTable.eSubType)data.SubType);
                                    mDataBinder.ForceUpdateSkyStatus();
                                    mDataBinder.UpdateWindProcess();
                                }

                                if (null != mDataBinder.mEffectLoader)
                                {
                                    mDataBinder.mEffectLoader.DeActiveEffect(10);
                                    mDataBinder.mEffectLoader.DeActiveEffect(11);
                                }

                                InvokeMethod.RemoveInvokeCall(this);
                                InvokeMethod.Invoke(this, mDataBinder.mFadeLength,
                                    () =>
                                    {
                                        if (null != mDataBinder.mEffectLoader)
                                        {
                                            if(null != mDataBinder.mOnFlightStart)
                                            {
                                                mDataBinder.mOnFlightStart.Invoke();
                                            }
                                            mDataBinder.mEffectLoader.LoadEffect(11);
                                            mDataBinder.mEffectLoader.ActiveEffect(11);
                                        }
                                    });
                                InvokeMethod.Invoke(this, mDataBinder.mFadeLength + mDataBinder.mFlightLength,
                                    () =>
                                    {
                                        if (null != mDataBinder.mEffectLoader)
                                        {
                                            mDataBinder.mEffectLoader.DeActiveEffect(11);
                                            mDataBinder.mEffectLoader.LoadEffect(10);
                                            mDataBinder.mEffectLoader.ActiveEffect(10);
                                        }

                                        _LoadSkyDatas(FashionMergeManager.GetInstance().FashionType);

                                        if (null != mDataBinder)
                                        {
                                            mDataBinder.UpdateSkyStatus();
                                            mDataBinder.ForceUpdateSkyStatus();
                                            mDataBinder.UpdateWindProcess();
                                            mDataBinder.DisableFlyState();
                                        }

                                        ClientSystemManager.GetInstance().OpenFrame<FashionEquipFrame>(FrameLayer.Middle, new FashionEquipFrameData
                                        {
                                            eFashionType = FashionMergeManager.GetInstance().FashionType,
                                            Occu = PlayerBaseData.GetInstance().JobTableID,
                                            SuitID = FashionMergeManager.GetInstance().SkySuitID,
                                        });
                                    });
                            }
                        }
                    });
            }
            else
            {
                if (null != mDataBinder)
                {
                    mDataBinder.FlyEffect(9, startWorldPosition, mDataBinder.GetFlyFixedPosition(), mDataBinder.mFlyLength,
                        () =>
                        {
                            if (null != mDataBinder.mOnFlyStart)
                            {
                                mDataBinder.mOnFlyStart.Invoke();
                            }
                        },
                        () =>
                        {
                            if (null != mDataBinder)
                            {
                                mDataBinder.DisableFlyState();
                            }

                            if (null != itemWind)
                            {
                                FashionType eWindType = FashionType.FT_SKY;
                                int windSuitId = 1;
                                var windSkyItem = TableManager.GetInstance().GetTableItem<ProtoTable.FashionComposeSkyTable>(itemWind.TableID);
                                if (null != windSkyItem)
                                {
                                    eWindType = windSkyItem.Type == 1 ? FashionType.FT_SKY : FashionType.FT_GOLD_SKY;
                                    windSuitId = windSkyItem.SuitID;
                                }

                                ClientSystemManager.GetInstance().OpenFrame<FashionEquipFrame>(FrameLayer.Middle, new FashionEquipFrameData
                                {
                                    eFashionType = eWindType,
                                    Occu = PlayerBaseData.GetInstance().JobTableID,
                                    SuitID = windSuitId,
                                });
                            }
                        });
                }
            }
        }

        private void ShowFashionPropertyFrame()
        {
            ClientSystemManager.GetInstance().OpenFrame<FashionMergeNewPropertyFrame>(FrameLayer.Middle);
        }

        private void UpdateProbabilityAscensionRoot()
        {
            mProbabilityAscension.CustomActive(ActivityDataManager.GetInstance().CheckFashionSynthesisActivityIsOpen());
        }

        #region ExtraUIBind
        private Toggle mAutoEquip = null;
        private Button mWatchInfo = null;
        private Button mActivityWatchInfo = null;
        private GameObject mProbabilityAscension = null;

        protected sealed override void _bindExUI()
        {
            mAutoEquip = mBind.GetCom<Toggle>("autoEquip");
            if (null != mAutoEquip)
            {
                mAutoEquip.onValueChanged.AddListener(_onAutoEquipToggleValueChange);
            }
            mWatchInfo = mBind.GetCom<Button>("WatchInfo");
            if (mWatchInfo != null)
            {
                mWatchInfo.onClick.RemoveAllListeners();
                mWatchInfo.onClick.AddListener(ShowFashionPropertyFrame);
            }
            mActivityWatchInfo = mBind.GetCom<Button>("ActivityWatchInfo");
            if (mActivityWatchInfo != null)
            {
                mActivityWatchInfo.onClick.RemoveAllListeners();
                mActivityWatchInfo.onClick.AddListener(ShowActivityFashionPropertyFrame);
            }
            mProbabilityAscension = mBind.GetGameObject("ProbabilityAscension");
        }

        protected sealed override void _unbindExUI()
        {
            if (null != mAutoEquip)
            {
                mAutoEquip.onValueChanged.RemoveListener(_onAutoEquipToggleValueChange);
            }
            mAutoEquip = null;
            if (mWatchInfo != null)
            {
                mWatchInfo.onClick.RemoveAllListeners();
                mWatchInfo = null;
            }
            if (mActivityWatchInfo != null)
            {
                mActivityWatchInfo.onClick.RemoveAllListeners();
                mActivityWatchInfo = null;
            }
            mProbabilityAscension = null;
        }
        #endregion

        #region Callback
        private void _onAutoEquipToggleValueChange(bool changed)
        {
            /* put your code in here */
            //发协议过去
            if(changed)
            {
                if(autoEquipState != 1)
                {
                    autoEquipState = 1;
                    FashionMergeManager.GetInstance().SetAutoEquipState(1);
                }
                mDataBinder.TryAutoEquipFashion();
            }
            else
            {
                if (autoEquipState != 0)
                {
                    autoEquipState = 0;
                    FashionMergeManager.GetInstance().SetAutoEquipState(0);
                }
            }
        }

        void ShowActivityFashionPropertyFrame()
        {
            ClientSystemManager.GetInstance().OpenFrame<ActivityFashionMergeNewPropertyFrame>(FrameLayer.Middle);
        }
        #endregion
    }
}