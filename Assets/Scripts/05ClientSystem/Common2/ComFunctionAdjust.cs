using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ActivityLimitTime;
using Protocol;
using Network;
using UnityEngine.Events;

namespace GameClient
{
    class AdjustChangedAttr
    {
        public float iOrgAttr;
        public float iCurAttr;
        public EEquipProp eEEquipProp;
        public int iOrgQuality;
        public int iCurQuality;
        public float iFullAttr;
        public float iExtraAttr;
        public bool bEffect;

        public string ExtraAttr
        {
            get
            {
                return string.Format("{0:F1}", iExtraAttr);
            }
        }

        public float DeltaValue
        {
            get
            {
                return iCurAttr - iOrgAttr;
            }
        }

        public float fAmount
        {
            get
            {
                float fValue = 0.0f;
                if(iFullAttr != 0)
                {
                    fValue = iCurAttr * 1.0f / iFullAttr;
                    fValue = Mathf.Clamp01(fValue);
                }
                return fValue;
            }
        }

        public string Process
        {
            get
            {
                return string.Format("{0}/{1}", string.Format("{0:F1}", (iCurAttr > iFullAttr ? iFullAttr : iCurAttr)), string.Format("{0:F1}",iFullAttr));
            }
        }

        public bool IsFull
        {
            get
            {
                return iCurQuality >= 100;
            }
        }

        public bool IsChanged
        {
            get
            {
                return iCurAttr != iOrgAttr;
            }
        }

        public bool IsUp
        {
            get
            {
                return iCurAttr > iOrgAttr;
            }
        }
    }

    public class ComFunctionAdjust : MonoBehaviour
    {
        public GameObject goParent;
        public GameObject goPrefab;
        public Image kIcon0;
        public Text costCount;

        public Text qualityDown;
        public Text qualityUp;
        public Text qualityUpSpecial;
        public Text qualityUnChanged;
        public Text qualityFull;

        public Text startQuality;
        public Text endQuality;
        public Slider startQualitySlider;
        public Slider endQualitySlider;

        public GeUISwitchButton toggle;

        public GameObject goMask;
        public UIGray grayOnce;
        public UIGray grayCrazy;
        public Button buttonOnce;
        public Button buttonCrazy;
        public GameObject goStopCrazy;
        public GameObject goCrazyHint;

        public GameObject goCostItem;
        public GameObject goCostMoney;

        public StatusBinder statusBinder;

        CachedObjectListManager<CachedAdjustItem> m_akAdjustItems = new CachedObjectListManager<CachedAdjustItem>();

        ItemData itemDataEx = null;
        ItemData itemData
        {
            get
            {
                return itemDataEx;
            }
            set
            {
                itemDataEx = value;
                //Logger.LogErrorFormat("itemData value changed !");
            }
        }
        bool bCrazyMode = false;
        bool bStart = false;
        bool bFirstCrazy = true;
        bool bIsShowBtnOncebtnCreazy = false;
        int iAdjustItemId = 200110002;
        int m_perfectScrollsID;
        void Start()
        {
            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;
            RegisterDelegateHandler();
            m_perfectScrollsID = int.Parse(TR.Value("ItemKeyPerfectScrollsID"));
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUsePerfectWashRoll, OnUsePerfectWashRoll);
        }

        public void RegisterDelegateHandler()
        {
            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem += OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
        }

        public void UnRegisterDelegateHandler()
        {
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem -= OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
        }

        public void Initialize()
        {
           Locked = false;
        }

        void OnUsePerfectWashRoll(UIEvent iEvent)
        {
            OnPerfectScrollsClick();
        }

        public void OnClickPopHintCrazyMode()
        {
            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("carzy_adjust_intterupt_hint"));
        }

        void OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            _UpdateMoneyItems();
        }

        void OnAddNewItem(List<Item> items)
        {
            _UpdateUIStatus();
            _UpdateMoneyItems();
        }

        void OnUpdateItem(List<Item> items)
        {
            _UpdateUIStatus();
            _UpdateMoneyItems();
        }

        void OnRemoveItem(ItemData data)
        {
            _UpdateUIStatus();
            _UpdateMoneyItems();
        }

        public void StopEffect()
        {
            m_akAdjustItems.ActiveObjects.ForEach(x => { x.StopEffect(); });
        }

        void _UpdateMoneyItems()
        {
            var quickBuyItem = TableManager.GetInstance().GetTableItem<ProtoTable.QuickBuyTable>(iAdjustItemId);
            if(quickBuyItem == null)
            {
                return;
            }

            int iId = quickBuyItem.CostItemID;
            var itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(iId);
            if (itemData != null)
            {
                ETCImageLoader.LoadSprite(ref kIcon0, itemData.Icon);
            }
            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(iId,false);
            int iCostCount = quickBuyItem.CostNum;
            costCount.text = string.Format("x{0}", iCostCount);
            _UpdateMoneyItemConvert();
        }

        void _UpdateMoneyItemConvert()
        {
            int iHasItemCount = ItemDataManager.GetInstance().GetOwnedItemCount(iAdjustItemId);
            if (iHasItemCount > 0 || !toggle.states)
            {
                goCostItem.CustomActive(true);
                goCostMoney.CustomActive(false);
                statusBinder.ChangeStatus(2);
            }
            else
            {
                goCostItem.CustomActive(false);
                goCostMoney.CustomActive(true);
                statusBinder.ChangeStatus(1);
            }
        }

        void OnDestroy()
        {
            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;
            UnRegisterDelegateHandler();
            InvokeMethod.RemoveInvokeCall(this);
            bStart = false;
            bCrazyMode = false;
            if(null != mCurWait)
            {
                WaitNetMessageManager.GetInstance().CancelWait(mCurWait);
                mCurWait = null;
            }
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUsePerfectWashRoll, OnUsePerfectWashRoll);
        }

        public void SetUIData(ItemData itemData)
        {
            //Logger.LogErrorFormat("SetUIData = {0}", itemData.GUID);
            this.itemData = itemData;
            _UpdateMoneyItems();
            _UpdateUIStatus();
            _SaveFullMinusAttrs(itemData);
            if(!Locked)
            {
                _SaveOrgAttrs(itemData);
                m_eStopReason = StopReason.SR_INVALID;
            }
            _UpdateQualityDesc();
            _SaveFullAttrs(itemData);
            _SaveCurAttrs(itemData);
            _SavePerfectAttrs(itemData);
            _LoadChanges(Locked);
        }

        static EEquipProp[] ms_akProps = new EEquipProp[]
        {
            EEquipProp.PhysicsAttack,
            EEquipProp.MagicAttack,
            EEquipProp.PhysicsDefense,
            EEquipProp.MagicDefense,
            EEquipProp.Strenth,
            EEquipProp.Intellect,
            EEquipProp.Spirit,
            EEquipProp.Stamina,
            EEquipProp.LightAttack,
            EEquipProp.FireAttack,
            EEquipProp.IceAttack,
            EEquipProp.DarkAttack,
            EEquipProp.LightDefence,
            EEquipProp.FireDefence,
            EEquipProp.IceDefence,
            EEquipProp.DarkDefence,
            EEquipProp.Independence,
        };
        float[] orgAttrs = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0};
        int iOrgQuality = 0;
        int iEndQuality = 0;
        float[] curAttrs = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0};
        int iCurQuality = 0;
        float[] fullAttrs = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0};
        float[] perfectAttrs = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0};
        float[] fullMinusAttrs = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0};
        public enum StopReason
        {
            SR_INVALID = 0,
            SR_CRAZY_BEGIN,
            SR_CRAZY_CONTINUE,
            SR_NORMAL_BEGIN,
            SR_CRAZY_END,
            SR_NORMAL_END,
            SR_EFFECT_NOT_ENOUNGH,
            SR_BY_HANDLE,
            SR_REACH_TARGET,
        }
        StopReason m_eStopReason = StopReason.SR_INVALID;
        static bool ms_bLocked = false;
        public bool Locked
        {
            get
            {
                return ms_bLocked;
            }
            set
            {
                ms_bLocked = value;
                goMask.CustomActive(value);
                bStart = value;
                grayOnce.CustomActive(!value && bIsShowBtnOncebtnCreazy == false);
                grayCrazy.CustomActive(!value && bIsShowBtnOncebtnCreazy == false);
                //Logger.LogErrorFormat("Locked bStart = {0}", bStart);
            }
        }

        public enum AdjustCheckResult
        {
            ACR_OK = 0,
            ACR_QUALITY_BOX,
            ACR_ERROR,
        }

        public AdjustCheckResult CheckAdjust(UnityEngine.Events.UnityAction callback,UnityEngine.Events.UnityAction onFailed)
        {
            if(this.itemData == null)
            {
                return AdjustCheckResult.ACR_ERROR;
            }

            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(iAdjustItemId);
            if(iHasCount > 0)
            {
                if(callback != null)
                {
                    callback.Invoke();
                }
                return AdjustCheckResult.ACR_OK;
            }

            if(!toggle.states)
            {
                ItemComeLink.OnLink(iAdjustItemId, 1, true, () =>
                {
                    if (ItemDataManager.GetInstance().GetOwnedItemCount(iAdjustItemId) >= 1)
                    {
                        if (callback != null)
                        {
                            callback.Invoke();
                        }
                    }
                    else
                    {
                        if (onFailed != null)
                        {
                            onFailed.Invoke();
                        }
                    }
                    bFirstCrazy = false;
                }, false, false, toggle.states || bCrazyMode && !bFirstCrazy,
                () =>
                {
                    if (onFailed != null)
                    {
                        onFailed.Invoke();
                    }
                });
                return AdjustCheckResult.ACR_OK;
            }

            var item = TableManager.GetInstance().GetTableItem<ProtoTable.QuickBuyTable>(iAdjustItemId);
            if(item != null)
            {
                CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo { nMoneyID = item.CostItemID, nCount = item.CostNum }, () =>
                {
                    if(callback != null)
                    {
                        callback.Invoke();
                    }
                },
                "common_money_cost",
                () =>
                {
                    if (onFailed != null)
                    {
                        onFailed.Invoke();
                    }
                });
                return AdjustCheckResult.ACR_OK;
            }

            return AdjustCheckResult.ACR_ERROR;
        }

        public void OnPerfectScrollsClick()
        {
            if (this.itemData == null)
            {
                return;
            }

            if (Locked)
            {
                return;
            }
            bCrazyMode = false;
            m_eStopReason = StopReason.SR_NORMAL_BEGIN;
            _SaveOrgAttrs(this.itemData);
            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(m_perfectScrollsID);
            if (iHasCount > 0)
            {
                Locked = true;
                _NetSyncChangeQuality(this.itemData, false,true);
                return;
            }
        }

        public void AdjustQualityOnce()
        {
            if(this.itemData == null)
            {
                return;
            }

            if (Locked)
            {
                return;
            }

            if(SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }

            bCrazyMode = false;
            m_eStopReason = StopReason.SR_NORMAL_BEGIN;
            _SaveOrgAttrs(this.itemData);

            //if has quality changed cost item
            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(iAdjustItemId);
            if (iHasCount > 0)
            {
                Locked = true;
                _NetSyncChangeQuality(this.itemData, _CheckNeedUsePoint());
                return;
            }

            if (_CheckNeedUsePoint())
            {
                if(SecurityLockDataManager.GetInstance().CheckSecurityLock())
                {
                    return;
                }
                _TryAdjustCostMoney(600000002, 10, () =>
                {
                    Locked = true;
                    _NetSyncChangeQuality(this.itemData, _CheckNeedUsePoint());
                }, null);
                return;
            }

            ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.TryShowMallGift(MallGiftPackActivateCond.NO_QUILTY_ADJUST_BOX, () =>
            {
                //CommonNewMessageBox.Notify("UIFlatten/Prefabs/SmithShop/CrazyAdjustConfirmFrame", 
                //    7012,
                //    () =>
                //    {
                //        if(SecurityLockDataManager.GetInstance().CheckSecurityLock())
                //        {
                //            return;
                //        }
                //        _TryAdjustCostMoney(600000002, 10, () =>
                //        {
                //            toggle.isOn = true;
                //            Locked = true;
                //            _NetSyncChangeQuality(this.itemData, true);
                //        },
                //        () =>
                //        {
                //            toggle.isOn = true;
                //        });
                //    },
                //    null,
                //    null,
                //    null,
                //    new object[] { _GetTargetQualityValue(itemData) },
                //    null);

                ItemComeLink.OnLink(iAdjustItemId, 0, false);
            });
        }

        int _GetTargetQualityValue(ItemData itemData)
        {
            int iMin = 60;
            if(null == itemData)
            {
                return iMin;
            }

            int iRet = (int)itemData.SubQuality;
            if(iRet < iMin)
            {
                return iMin;
            }

            iRet = iRet + 1;
            iRet = IntMath.Min(iRet, 100);
            return iRet;
        }

        void _SaveOrgAttrs(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            var equipQLValueItem = TableManager.GetInstance().GetTableItem<ProtoTable.EquipQLValueTable>(_GetEquipQLValueId(itemData));
            if (null == equipQLValueItem)
            {
                return;
            }

            float fAtkDefRadio = equipQLValueItem.AtkDef * 0.001f;
            float fForDefRadio = equipQLValueItem.FourDimensional * 0.001f;
            float fIndependentResists = equipQLValueItem.IndependentResists *0.001f;

            for (int i = 0; i < ms_akProps.Length; ++i)
            {
                float fCurValue = 0.0f;
                if (i < 4 || ((i >= 8) && (i < 12)))
                {
                    fCurValue = fullMinusAttrs[i] * (1.0f - fAtkDefRadio) * itemData.SubQuality / 100.0f;
                }
                else if (i == 16)
                {
                    fCurValue = fullMinusAttrs[i] * (1.0f - fIndependentResists) * itemData.SubQuality / 100.0f;
                }
                else
                {
                    fCurValue = fullMinusAttrs[i] * (1.0f - fForDefRadio) * itemData.SubQuality / 100.0f;
                }
                orgAttrs[i] = fCurValue;
            }
            iOrgQuality = itemData.SubQuality;
            _SaveItemOrgQuality(itemData.SubQuality);
        }

        void _SaveItemOrgQuality(int data)
        {
            iEndQuality = data;
        }
        int _GetItemQuality()
        {
            return iEndQuality;
        }
        void _SaveCurAttrs(ItemData itemData)
        {
            if(itemData == null)
                return;
            
            var equipQLValueItem = TableManager.GetInstance().GetTableItem<ProtoTable.EquipQLValueTable>(_GetEquipQLValueId(itemData));
            if (null == equipQLValueItem)
            {
                return;
            }

            ItemData curItemData = ItemDataManager.GetInstance().GetItem(itemData.GUID);
            if(curItemData == null)
                return;

            float fAtkDefRadio = equipQLValueItem.AtkDef * 0.001f;
            float fForDefRadio = equipQLValueItem.FourDimensional * 0.001f;
            float fIndependentResists = equipQLValueItem.IndependentResists * 0.001f;

            if (itemData == null) return;
            for (int i = 0; i < ms_akProps.Length; ++i)
            {
                float fCurValue = 0.0f;
                if (i < 4||((i>= 8) && (i < 12)))
                {
                    fCurValue = fullMinusAttrs[i] * (1.0f - fAtkDefRadio) * curItemData.SubQuality / 100.0f;
                }
                else if (i == 16)
                {
                    fCurValue = fullMinusAttrs[i] * (1.0f - fIndependentResists) * curItemData.SubQuality / 100.0f;
                }
                else
                {
                    fCurValue = fullMinusAttrs[i] * (1.0f - fForDefRadio) * curItemData.SubQuality / 100.0f;
                }
                curAttrs[i] = fCurValue;
            }
            iCurQuality = curItemData.SubQuality;
        }

        void _SaveFullMinusAttrs(ItemData itemData)
        {
			if (itemData == null)
				return;
			
            System.Array.Clear(fullMinusAttrs, 0, fullMinusAttrs.Length);
            var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemData.TableID);
            if (null == itemTable)
            {
                return;
            }

            var equipAttrTable = TableManager.GetInstance().GetTableItem<ProtoTable.EquipAttrTable>(itemTable.EquipPropID);
            if (null == equipAttrTable)
            {
                return;
            }

            fullMinusAttrs[0] = itemData.ConvertAttrByValue(ms_akProps[0], equipAttrTable.Atk);
            fullMinusAttrs[1] = itemData.ConvertAttrByValue(ms_akProps[1], equipAttrTable.MagicAtk);
            fullMinusAttrs[2] = itemData.ConvertAttrByValue(ms_akProps[2], equipAttrTable.Def);
            fullMinusAttrs[3] = itemData.ConvertAttrByValue(ms_akProps[3], equipAttrTable.MagicDef);
            fullMinusAttrs[4] = itemData.ConvertAttrByValue(ms_akProps[4], equipAttrTable.Strenth);
            fullMinusAttrs[5] = itemData.ConvertAttrByValue(ms_akProps[5], equipAttrTable.Intellect);
            fullMinusAttrs[6] = itemData.ConvertAttrByValue(ms_akProps[6], equipAttrTable.Spirit);
            fullMinusAttrs[7] = itemData.ConvertAttrByValue(ms_akProps[7], equipAttrTable.Stamina);
            fullMinusAttrs[8] = itemData.ConvertAttrByValue(ms_akProps[8], equipAttrTable.LightAttack);
            fullMinusAttrs[9] = itemData.ConvertAttrByValue(ms_akProps[9], equipAttrTable.FireAttack);
            fullMinusAttrs[10] = itemData.ConvertAttrByValue(ms_akProps[10], equipAttrTable.IceAttack);
            fullMinusAttrs[11] = itemData.ConvertAttrByValue(ms_akProps[11], equipAttrTable.DarkAttack);
            fullMinusAttrs[12] = itemData.ConvertAttrByValue(ms_akProps[12], equipAttrTable.LightDefence);
            fullMinusAttrs[13] = itemData.ConvertAttrByValue(ms_akProps[13], equipAttrTable.FireDefence);
            fullMinusAttrs[14] = itemData.ConvertAttrByValue(ms_akProps[14], equipAttrTable.IceDefence);
            fullMinusAttrs[15] = itemData.ConvertAttrByValue(ms_akProps[15], equipAttrTable.DarkDefence);
            fullMinusAttrs[16] = itemData.ConvertAttrByValue(ms_akProps[16], equipAttrTable.Independence);
        }

        void _SaveFullAttrs(ItemData itemData)
        {
            if(itemData == null)return;
            System.Array.Clear(fullAttrs, 0, fullAttrs.Length);
            var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemData.TableID);
            if(null == itemTable)
            {
                return;
            }

            var equipAttrTable = TableManager.GetInstance().GetTableItem<ProtoTable.EquipAttrTable>(itemTable.EquipPropID);
            if(null == equipAttrTable)
            {
                return;
            }

            var equipQLValueItem = TableManager.GetInstance().GetTableItem<ProtoTable.EquipQLValueTable>(_GetEquipQLValueId(itemData));
            if(null == equipQLValueItem)
            {
                return;
            }

            float fAtkDefRadio = 1.0f - equipQLValueItem.AtkDef * 0.001f;
            float fForDefRadio = 1.0f - equipQLValueItem.FourDimensional * 0.001f;
            float fIndependentResists = 1.0f - equipQLValueItem.IndependentResists * 0.001f;

            fullAttrs[0] = itemData.ConvertAttrByValue(ms_akProps[0], equipAttrTable.Atk);
            fullAttrs[1] = itemData.ConvertAttrByValue(ms_akProps[1], equipAttrTable.MagicAtk);
            fullAttrs[2] = itemData.ConvertAttrByValue(ms_akProps[2], equipAttrTable.Def);
            fullAttrs[3] = itemData.ConvertAttrByValue(ms_akProps[3], equipAttrTable.MagicDef);
            fullAttrs[4] = itemData.ConvertAttrByValue(ms_akProps[4], equipAttrTable.Strenth);
            fullAttrs[5] = itemData.ConvertAttrByValue(ms_akProps[5], equipAttrTable.Intellect);
            fullAttrs[6] = itemData.ConvertAttrByValue(ms_akProps[6], equipAttrTable.Spirit);
            fullAttrs[7] = itemData.ConvertAttrByValue(ms_akProps[7], equipAttrTable.Stamina);
            fullAttrs[8] = itemData.ConvertAttrByValue(ms_akProps[8], equipAttrTable.LightAttack);
            fullAttrs[9] = itemData.ConvertAttrByValue(ms_akProps[9], equipAttrTable.FireAttack);
            fullAttrs[10] = itemData.ConvertAttrByValue(ms_akProps[10], equipAttrTable.IceAttack);
            fullAttrs[11] = itemData.ConvertAttrByValue(ms_akProps[11], equipAttrTable.DarkAttack);
            fullAttrs[12] = itemData.ConvertAttrByValue(ms_akProps[12], equipAttrTable.LightDefence);
            fullAttrs[13] = itemData.ConvertAttrByValue(ms_akProps[13], equipAttrTable.FireDefence);
            fullAttrs[14] = itemData.ConvertAttrByValue(ms_akProps[14], equipAttrTable.IceDefence);
            fullAttrs[15] = itemData.ConvertAttrByValue(ms_akProps[15], equipAttrTable.DarkDefence);
            fullAttrs[16] = itemData.ConvertAttrByValue(ms_akProps[16], equipAttrTable.Independence);

            for (int i = 0; i < 4; ++i)
            {
                fullAttrs[i] *= fAtkDefRadio;
            }

            for (int i = 4; i < 8; ++i)
            {
                fullAttrs[i] *= fForDefRadio;
            }

            for (int i = 8; i < 12; i++)
            {
                fullAttrs[i] *= fAtkDefRadio;
            }

            for (int i = 12; i < 16; i++)
            {
                fullAttrs[i] *= fForDefRadio;
            }

            fullAttrs[16] *= fIndependentResists;

        }

        void _SavePerfectAttrs(ItemData itemData)
        {
            System.Array.Clear(perfectAttrs, 0, perfectAttrs.Length);
            if(itemData != null)
            {
                var equipQLValueItem = TableManager.GetInstance().GetTableItem<ProtoTable.EquipQLValueTable>(_GetEquipQLValueId(itemData));
                if(equipQLValueItem != null)
                {
                    for(int i = 0; i < perfectAttrs.Length; ++i)
                    {
                        //base attrs - atk or def
                        if(i < 8)
                        {
                            perfectAttrs[i] = Mathf.FloorToInt(fullMinusAttrs[i] * equipQLValueItem.PerfectAtkDef * 0.001f);
                        }
                        else if (i == 16)
                        {
                            perfectAttrs[i] = Mathf.FloorToInt(fullMinusAttrs[i] * equipQLValueItem.PerfectIndependentResists * 0.001f);
                        }
                        else
                        {
                            //four attrs
                            perfectAttrs[i] = Mathf.FloorToInt(fullMinusAttrs[i] * equipQLValueItem.PerfectFourDimensional * 0.001f);
                        }
                        perfectAttrs[i] = Mathf.Max(perfectAttrs[i], 1.0f);
                    }
                }
            }
        }

        static int[] ms_thirdtype_map_QLValueId = new int[]
        {
            2,3,5,6,4
        };

        int _GetEquipQLValueId(ItemData itemData)
        {
            if(itemData != null)
            {
                if(itemData.SubType == (int)ProtoTable.ItemTable.eSubType.WEAPON)
                {
                    return 1;
                }

                if(itemData.SubType >= (int)ProtoTable.ItemTable.eSubType.HEAD &&
                    itemData.SubType <= (int)ProtoTable.ItemTable.eSubType.BOOT)
                {
                    int iIndex = (int)itemData.ThirdType - (int)ProtoTable.ItemTable.eThirdType.CLOTH;
                    if(iIndex >= 0 && iIndex < ms_thirdtype_map_QLValueId.Length)
                    {
                        return ms_thirdtype_map_QLValueId[iIndex];
                    }
                }

                if (itemData.SubType >= (int)ProtoTable.ItemTable.eSubType.RING &&
                    itemData.SubType <= (int)ProtoTable.ItemTable.eSubType.BRACELET)
                {
                    return 7;
                }
            }

            return 1;
        }

        void _LoadChanges(bool bNeedEffect)
        {
            //Logger.LogErrorFormat("_LoadChanges = {0}", bNeedEffect);

            //List<AdjustChangedAttr> changedAttrs = new List<AdjustChangedAttr>();
            List<AdjustChangedAttr> changedAttrs = GamePool.ListPool<AdjustChangedAttr>.Get();// new List<AdjustChangedAttr>();

            for (int i = 0; i < ms_akProps.Length; ++i)
            {
                bool bNeedFilter = orgAttrs[i] == 0 && curAttrs[i] == 0 && fullAttrs[i] == 0;
                if(bNeedFilter) { continue; }

                changedAttrs.Add(
                    new AdjustChangedAttr
                    {
                        eEEquipProp = ms_akProps[i],
                        iOrgAttr = orgAttrs[i],
                        iCurAttr = curAttrs[i],
                        iOrgQuality = iOrgQuality,
                        iCurQuality = iCurQuality,
                        iFullAttr = fullAttrs[i],
                        iExtraAttr = perfectAttrs[i],
                        bEffect = bNeedEffect,
                    });
            }

            m_akAdjustItems.RecycleAllObject();

            for (int i = 0; i < changedAttrs.Count; ++i)
            {
                m_akAdjustItems.Create(new object[] 
                {
                    goParent,
                    goPrefab,
                    changedAttrs[i],
                    false
                });
            }

            GamePool.ListPool<AdjustChangedAttr>.Release(changedAttrs);
        }

        public void AdjustQualityCrazy()
        {
            if (this.itemData == null)
            {
                return;
            }

            if (Locked)
            {
                return;
            }

            if(SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }
            int tempSubQuality = _GetItemQuality();
            if(tempSubQuality == -1)
            {
                return;
            }
            //if has quality changed cost item
            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(iAdjustItemId);
            if (iHasCount > 0)
            {
                _TryOpenNewMessageBox(_ConfirmToCrazyX, _ConfirmToCrazyX);
                return;
            }

            if(_CheckNeedUsePoint())
            {
                _TryOpenNewMessageBox(() =>
                    {
                        if(SecurityLockDataManager.GetInstance().CheckSecurityLock())
                        {
                            return;
                        }
                        toggle.states = true;
                        _TryAdjustCostMoney(600000002, 10,
                            () =>
                            {
                                _ConfirmToCrazyX();
                            },
                            null);
                }, () =>
                {
                    if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
                    {
                        return;
                    }
                    toggle.states = true;
                    _TryAdjustCostMoney(600000002, 10,
                        () =>
                        {
                            _ConfirmToCrazyX();
                    },
                        null);
                });
                return;
            }

            //TODO : MJX
            ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.TryShowMallGift(MallGiftPackActivateCond.NO_QUILTY_ADJUST_BOX, () =>
            {
                _TryOpenNewMessageBox(() =>
                    {
                        if(SecurityLockDataManager.GetInstance().CheckSecurityLock())
                        {
                            return;
                        }
                        toggle.states = true;
                        _TryAdjustCostMoney(600000002, 10,
                            () =>
                            {
                                _ConfirmToCrazyX();
                            },
                            null);
                }, () =>
                {
                    if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
                    {
                        return;
                    }
                    toggle.states = true;
                    _TryAdjustCostMoney(600000002, 10,
                        () =>
                        {
                            _ConfirmToCrazyX();
                    },
                        null);
                });
            });
        }
        void _TryOpenNewMessageBox(UnityAction tempAction1,UnityAction tempAction2)
        {
            _SaveItemOrgQuality(this.itemData.SubQuality);
            int tempSubQuality = _GetItemQuality();
            List<ToggleEvent> toggleEvents = new List<ToggleEvent>();
            if (tempSubQuality >= 99)
            {
                toggleEvents.Add(new ToggleEvent(TR.Value("quality_adjust_crazy_new_tip"), tempAction1));
            }
            else if (tempSubQuality >= 0 && tempSubQuality < 99)
            {
                toggleEvents.Add(new ToggleEvent(TR.Value("quality_adjust_crazy_old_tip", (tempSubQuality + 1 > 60 ? tempSubQuality + 1 : 60).ToString()), tempAction1));
                toggleEvents.Add(new ToggleEvent(TR.Value("quality_adjust_crazy_new_tip"), tempAction2));
            }
            CommonNewMessageBox.Notify("UIFlatten/Prefabs/SmithShop/CrazyAdjustConfirmFrame",
                    7013,
                    null,
                    null,
                    toggleEvents,
                    new object[] { _GetCrazyMsgBoxText() },
                    new object[] { _GetTargetQualityValue(itemData) });

            CrazyAdjustPercentSet.initTargetQuality = (tempSubQuality < 60 ? 60 :(tempSubQuality / 10 + 1) * 10);
        }

        string _GetCrazyMsgBoxText()
        {
            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(iAdjustItemId);
            if(iHasCount > 0)
            {
                return TR.Value("crazy_adjust_cost_item");
            }

            if(!toggle.states)
            {
                return TR.Value("crazy_adjust_try_cost_money");
            }

            return TR.Value("crazy_adjust_cost_money");
        }

        string _GetQualityDesc(int SubQuality, string ex = "")
        {
            string levelText = "";
            if (SubQuality <= 20)
            {
                levelText = TR.Value("tip_grade_lower_most" + ex, SubQuality);
            }
            else if (SubQuality <= 40)
            {
                levelText = TR.Value("tip_grade_lower" + ex, SubQuality);
            }
            else if (SubQuality <= 60)
            {
                levelText = TR.Value("tip_grade_middle" + ex, SubQuality);
            }
            else if (SubQuality <= 80)
            {
                levelText = TR.Value("tip_grade_high" + ex, SubQuality);
            }
            else if (SubQuality < 100)
            {
                levelText = TR.Value("tip_grade_high_most" + ex, SubQuality);
            }
            else
            {
                levelText = TR.Value("tip_grade_perfect" + ex, SubQuality);
            }

            return levelText;
        }
        
        void _ConfirmToCrazyX()
        {
            if(itemData != null && CrazyAdjustPercentSet.curTargetQuality <= itemData.SubQuality)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("target_quality_less_than_now"));

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.StopCloseCommonNewMessageBoxView);
                return;
            }

            startQuality.text = string.Format(TR.Value("quality_adjust_crazy_mode_hint", _GetQualityDesc(itemData.SubQuality, "_ex")));
            endQuality.text = string.Format(TR.Value("quality_adjust_crazy_mode_hint_end", _GetQualityDesc(CrazyAdjustPercentSet.curTargetQuality, "_ex")));
            startQualitySlider.value = itemData.SubQuality / 100.0f;
            endQualitySlider.value = CrazyAdjustPercentSet.curTargetQuality / 100.0f;

            m_eStopReason = StopReason.SR_CRAZY_BEGIN;
            bFirstCrazy = true;
            bCrazyMode = true;
            _SaveOrgAttrs(this.itemData);
            _SaveItemOrgQuality(CrazyAdjustPercentSet.curTargetQuality);
            Locked = true;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ContinueProcessStart);
            _NetSyncChangeQuality(this.itemData, _CheckNeedUsePoint());
        }
        
        void _TryAdjustCostMoney(int iId,int iCount,System.Action onOk, System.Action onFailed)
        {
            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(iAdjustItemId);
            if (iHasCount < 1)
            {
                CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo
                {
                    nMoneyID = iId,
                    nCount = iCount,
                },
                onOk,
                "common_money_cost",
                onFailed);
                return;
            }

            if(onOk != null)
            {
                onOk();
            }
        }

        bool _CheckNeedUsePoint()
        {
            return toggle.states && ItemDataManager.GetInstance().GetOwnedItemCount(iAdjustItemId, false) < 1;
        }

        public void StopCrazyAdjust()
        {
            bStart = false;
            //Logger.LogErrorFormat("StopCrazyAdjust bStart = {0}", bStart);
        }

        void _EndAdjust(ItemData a_item,bool bNeedEffect = true)
        {
            _SaveFullMinusAttrs(a_item);
            _SaveCurAttrs(a_item);
            bCrazyMode = false;
            Locked = false;
            _SaveFullAttrs(a_item);
            _SavePerfectAttrs(a_item);
            _LoadChanges(bNeedEffect);
            _UpdateQualityDesc();
            stopItem = null;
        }

        protected bool _SatisfiedWithQuality()
        {
            return iCurQuality >= iEndQuality && iCurQuality >= 60;
        }

        ItemData stopItem = null;
        void Update()
        {
            if(bCrazyMode && !bStart && Locked)
            {
                InvokeMethod.RemoveInvokeCall(this);
                m_eStopReason = StopReason.SR_CRAZY_CONTINUE;
                StopEffect();

                iCurQuality = stopItem.SubQuality;
                stopItem.SubQuality = iOrgQuality;
                string preQualityDesc = _GetQualityDesc(stopItem.SubQuality, "_1");
                stopItem.SubQuality = iCurQuality;
                string curQualityDesc = _GetQualityDesc(stopItem.SubQuality, "_1");

                _EndAdjust(stopItem,false);

                CommonMessageBox.Notify(7010,
                    "UIFlatten/Prefabs/SmithShop/CrazyAdjustFinish",
                    new object[] { preQualityDesc, curQualityDesc },  //content
                    null, //ok
                    null, //cancel
                    null);

                // 手动停止疯狂洗练时也要发送
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ContinueProcessFinish);

                //Logger.LogErrorFormat("stop by hand !");
            }
        }

        WaitNetMessageManager.IWaitData mCurWait = null;

        protected void _NetSyncChangeQuality(ItemData a_item,bool bUsePoint,bool bUsePrefectScrolls = false)
        {
            SceneRandEquipQlvReq msg = new SceneRandEquipQlvReq();
            msg.uid = a_item.GUID;
            msg.bUsePoint = bUsePoint ? (byte)1 : (byte)0;
            msg.usePerfect= bUsePrefectScrolls ? (byte)1 : (byte)0;
            bIsShowBtnOncebtnCreazy = bUsePrefectScrolls;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            if(null != mCurWait)
            {
                Logger.LogErrorFormat("IWait is not null !!!");
                return;
            }

            mCurWait = WaitNetMessageManager.GetInstance().Wait<SceneRandEquipQlvRet>(msgRet =>
            {
                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    mCurWait = null;
                    Locked = false;
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                }
                else
                {
                    mCurWait = null;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemQualityChanged);

                    // 非疯狂模式则刷新相关UI
                    if(!bCrazyMode)
                    {
                        m_eStopReason = StopReason.SR_NORMAL_END;
                        _EndAdjust(a_item,true);

                        if (bIsShowBtnOncebtnCreazy)
                        {
                            ClientSystemManager.GetInstance().OpenFrame<PerfectBaptizeResultFrame>(FrameLayer.Middle, a_item);
                        }

                        return;
                    }

                    // 疯狂模式，先检查是否达到目标值，如果达到目标值了则停止疯狂洗练通知刷新UI
                    if(_SatisfiedWithQuality())
                    {
                        m_eStopReason = StopReason.SR_CRAZY_END;

                        iCurQuality = a_item.SubQuality;
                        a_item.SubQuality = iOrgQuality;
                        string preQualityDesc = _GetQualityDesc(a_item.SubQuality, "_1");
                        a_item.SubQuality = iCurQuality;
                        string curQualityDesc = _GetQualityDesc(a_item.SubQuality, "_1");

                        _EndAdjust(a_item,true);

                        CommonMessageBox.Notify(7009,
                            "UIFlatten/Prefabs/SmithShop/CrazyAdjustFinish",
                            new object[] { preQualityDesc, curQualityDesc },  //content
                            null, //ok
                            null, //cancel
                            null);

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ContinueProcessFinish);

                        return;
                    }

                    stopItem = a_item;
                    InvokeMethod.RemoveInvokeCall(this);

                    // 没有达到目标值则准备再来一次洗练
                    InvokeMethod.Invoke(this, Global.Settings.qualityAdjust.fInterval, () =>
                    {
                        if(bStart)
                        {
                            // 检查材料是否足够，足够则发送洗练消息
                            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(iAdjustItemId);
                            if(iHasCount > 0)
                            {
                                Locked = true;
                                _NetSyncChangeQuality(this.itemData, _CheckNeedUsePoint());
                                return;
                            }             
          
                            UnityAction ShowCrazyAdjustResult = () => 
                            {
                                m_eStopReason = StopReason.SR_CRAZY_CONTINUE;

                                iCurQuality = a_item.SubQuality;
                                a_item.SubQuality = iOrgQuality;
                                string preQualityDesc = _GetQualityDesc(a_item.SubQuality, "_1");
                                a_item.SubQuality = iCurQuality;
                                string curQualityDesc = _GetQualityDesc(a_item.SubQuality, "_1");

                                _EndAdjust(a_item, false);

                                CommonMessageBox.Notify(7011,
                                    "UIFlatten/Prefabs/SmithShop/CrazyAdjustFinish",
                                    new object[] { preQualityDesc, curQualityDesc },  //content
                                    () =>
                                    {
                                        ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.TryShowMallGift(MallGiftPackActivateCond.NO_QUILTY_ADJUST_BOX, null);
                                    }, //ok
                                    () =>
                                    {
                                        ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.TryShowMallGift(MallGiftPackActivateCond.NO_QUILTY_ADJUST_BOX, null);
                                    }, //cancel
                                    null);

                                //Logger.LogErrorFormat("stop crazy failed! !toggle.isOn");
                            };

                            if (!toggle.states)
                            {
                                // 洗练盒不足，且没有勾选使用点券则直接弹出洗练结果
                                // add by qxy 2019-11-05 
                                // 策划 朱立帅
                                ShowCrazyAdjustResult();
                                return;
                            }

                            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
                            {
                                return;
                            }

                            _TryAdjustCostMoney(600000002, 10,
                                () =>
                                {
                                    toggle.states = true;
                                    Locked = true;
                                    _NetSyncChangeQuality(this.itemData, _CheckNeedUsePoint());
                                },
                                () =>
                                {
                                    m_eStopReason = StopReason.SR_CRAZY_CONTINUE;
                                    iCurQuality = a_item.SubQuality;
                                    a_item.SubQuality = iOrgQuality;
                                    string preQualityDesc = _GetQualityDesc(a_item.SubQuality, "_1");
                                    a_item.SubQuality = iCurQuality;
                                    string curQualityDesc = _GetQualityDesc(a_item.SubQuality, "_1");

                                    _EndAdjust(a_item, false);
                                    CommonMessageBox.Notify(7011,
                                        "UIFlatten/Prefabs/SmithShop/CrazyAdjustFinish",
                                        new object[] { preQualityDesc, curQualityDesc },  //content
                                        null, //ok
                                        null, //cancel
                                        null);

                                    //Logger.LogErrorFormat("stop crazy failed! !toggle.isOn");
                                });
                        }
                    });
                }
            },
            true,
            3,
            _OnAdjustTimerOver);
        }

        void _OnAdjustTimerOver()
        {
            Locked = false;
            if (null != mCurWait)
            {
                WaitNetMessageManager.GetInstance().CancelWait(mCurWait);
                mCurWait = null;
            }
        }

        /// <summary>
        /// 点击加点券接口
        /// </summary>
        public void OnClickAddCostItem()
        {
            VipFrame vipframe = ClientSystemManager.GetInstance().OpenFrame<VipFrame>() as VipFrame;
            vipframe.SwitchPage(VipTabType.PAY);
        }

        public void OnToggleChanged(bool bValue)
        {
            _UpdateMoneyItems();
        }

        void _UpdateUIStatus()
        {
            bool bFull = this.itemData != null && this.itemData.SubQuality >= 100;
            grayOnce.enabled = bFull;
            grayCrazy.enabled = bFull;
            buttonOnce.enabled = !bFull;
            buttonCrazy.enabled = !bFull;

            goStopCrazy.CustomActive(bStart && bCrazyMode);
            goCrazyHint.CustomActive(bStart && bCrazyMode);
            startQualitySlider.CustomActive(bStart && bCrazyMode);
            endQualitySlider.CustomActive(bStart && bCrazyMode);
        }

        void _UpdateQualityDesc()
        {
            if (itemData == null)
            {
                return;
            }
            if(m_eStopReason == StopReason.SR_CRAZY_CONTINUE || m_eStopReason == StopReason.SR_CRAZY_END ||
                m_eStopReason == StopReason.SR_NORMAL_END)
            {
                if (itemData.SubQuality >= 100)
                {
                    qualityUpSpecial.CustomActive(false);
                    qualityDown.CustomActive(false);
                    qualityUp.CustomActive(false);
                    qualityUnChanged.CustomActive(false);
                    qualityFull.CustomActive(true);
                }
                else if(iCurQuality > iOrgQuality)
                {
                    if (m_eStopReason == StopReason.SR_NORMAL_END)
                    {
                        qualityUpSpecial.CustomActive(false);
                        qualityUp.CustomActive(true);
                    }
                    else if(m_eStopReason == StopReason.SR_CRAZY_CONTINUE ||
                        m_eStopReason == StopReason.SR_CRAZY_END)
                    {
                        qualityUpSpecial.CustomActive(true);
                        qualityUp.CustomActive(false);
                    }
                    qualityDown.CustomActive(false);
                    qualityUnChanged.CustomActive(false);
                    qualityFull.CustomActive(false);
                }
                else if (iCurQuality == iOrgQuality)
                {
                    qualityUpSpecial.CustomActive(false);
                    qualityDown.CustomActive(false);
                    qualityUp.CustomActive(false);
                    qualityUnChanged.CustomActive(true);
                    qualityFull.CustomActive(false);
                }
                else
                {
                    qualityUpSpecial.CustomActive(false);
                    qualityDown.CustomActive(true);
                    qualityUp.CustomActive(false);
                    qualityUnChanged.CustomActive(false);
                    qualityFull.CustomActive(false);
                }
            }
            else
            {
                qualityUpSpecial.CustomActive(false);
                qualityDown.CustomActive(false);
                qualityUp.CustomActive(false);
                qualityUnChanged.CustomActive(false);
                qualityFull.CustomActive(false);
            }
        }
    }
}