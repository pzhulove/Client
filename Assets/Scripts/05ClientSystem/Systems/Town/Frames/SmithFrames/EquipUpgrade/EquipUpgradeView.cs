using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using System;

namespace GameClient
{
    public class EquipUpgradeView : MonoBehaviour,ISmithShopNewView
    {
        [SerializeField]
        private GameObject mCurrentEquipItemParent;

        [SerializeField]
        private GameObject mNextLvEquipItemParent;

        [SerializeField]
        private ComUIListScript mConsumUIListScript;

        [SerializeField]
        private Button mUpgradeBtn;

        [SerializeField]
        private ComUIListScript mEquipUIListScript;

        [SerializeField] private Text mCurrentEquipmentName;
        [SerializeField] private Text mTargetEquipmentName;

        List<ulong> mAllEquipments = null;
        EquipUpgradeDataModel mEquipUpgradeDataModel = null;
        List<ItemSimpleData> mItemSimpleData = null;
        ComItemNew mCurrentComItem = null;
        ComItemNew mUpgradeComItem = null;

        private void RegisterEventHander()
        {
            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += OnAddNewItem;
        }

        private void UnRegisterEventHander()
        {
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnAddNewItem;
        }

        private void OnAddNewItem(List<Item> items)
        {
            if (items == null)
            {
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.Type == ProtoTable.ItemTable.eType.MATERIAL)
                {
                    SetConsumElementAmount();
                }
            }
        }

        void Awake()
        {
            RegisterEventHander();
            if (mUpgradeBtn)
            {
                mUpgradeBtn.onClick.RemoveAllListeners();
                mUpgradeBtn.onClick.AddListener(() =>
                {
                    if (mUpgradeBtn != null)
                    {
                        mUpgradeBtn.enabled = false;

                        InvokeMethod.Invoke(this, 0.50f, () => { mUpgradeBtn.enabled = true; });
                    }

                    OnEquipUpgradeClick();
                });
            }

            InitmConsumUIListScript();
            InitEquipUIListScript();
           
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnEquipUpgradeSuccess, OnEquipUpgradeSuccess);
            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRefreshEquipmentList, OnEquipUpgradeSuccess);
        }

        public void InitView(SmithShopNewLinkData linkData)
        {
            LoadAllEquipments();
        }

        public void OnEnableView()
        {
            RegisterEventHander();
            LoadAllEquipments();
        }

        public void OnDisableView()
        {
            UnRegisterEventHander();
        }

        #region 装备列表相关

        void OnEquipUpgradeSuccess(UIEvent uiEvent)
        {
            LoadAllEquipments();
        }

        private void InitEquipUIListScript()
        {
            mEquipUIListScript.Initialize();
            mEquipUIListScript.onBindItem += OnBindItemDelegate;
            mEquipUIListScript.onItemVisiable += OnItemVisiableDelegate;
            mEquipUIListScript.onItemSelected += OnItemSelectedDelegate;
            mEquipUIListScript.onItemChageDisplay += OnItemChangeDisplayDelegate;
        }

        public void LoadAllEquipments()
        {
            mAllEquipments = EquipUpgradeDataManager.GetInstance().GetAllEquipments();
            SetElementAmount(mAllEquipments.Count);
            _TrySetDefaultEquipment();
        }

        private void SetElementAmount(int count)
        {
            mEquipUIListScript.SetElementAmount(count);
        }

        private void _TrySetDefaultEquipment()
        {
            int index = -1;

            if (EquipUpgradeItem.ms_selected != null)
            {
                var find = mAllEquipments.Find(x => { return x == EquipUpgradeItem.ms_selected.GUID; });

                if (find != 0)
                {
                    EquipUpgradeItem.ms_selected = ItemDataManager.GetInstance().GetItem(find);
                }
                else
                {
                    EquipUpgradeItem.ms_selected = null;
                }
            }

            if (EquipUpgradeItem.ms_selected != null)
            {
                for (int i = 0; i < mAllEquipments.Count; i++)
                {
                    ulong guid = mAllEquipments[i];
                    if (guid != EquipUpgradeItem.ms_selected.GUID)
                    {
                        continue;
                    }

                    index = i;
                }
            }
            else
            {
                if (mAllEquipments.Count > 0)
                {
                    index = 0;
                }
            }

            SetSelectItem(index);
        }

        private void SetSelectItem(int index)
        {
            if (index >= 0 && index < mAllEquipments.Count)
            {
                EquipUpgradeItem.ms_selected = ItemDataManager.GetInstance().GetItem(mAllEquipments[index]);
                if (!mEquipUIListScript.IsElementInScrollArea(index))
                {
                    mEquipUIListScript.EnsureElementVisable(index);
                }
                mEquipUIListScript.SelectElement(index);
            }
            else
            {
                mEquipUIListScript.SelectElement(-1);
                EquipUpgradeItem.ms_selected = null;
            }

            UpdateLeftItemInfo();
        }

        private void UnInitEquipUIListScript()
        {
            if (mEquipUIListScript != null)
            {
                mEquipUIListScript.onBindItem -= OnBindItemDelegate;
                mEquipUIListScript.onItemVisiable -= OnItemVisiableDelegate;
                mEquipUIListScript.onItemSelected -= OnItemSelectedDelegate;
                mEquipUIListScript.onItemChageDisplay -= OnItemChangeDisplayDelegate;

                mEquipUIListScript = null;
            }
        }

        private EquipUpgradeItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<EquipUpgradeItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            EquipUpgradeItem equipUpgradeItem = item.gameObjectBindScript as EquipUpgradeItem;
            if (equipUpgradeItem != null && item.m_index >= 0 && item.m_index < mAllEquipments.Count)
            {
                ulong guid = mAllEquipments[item.m_index];

                var itemData = ItemDataManager.GetInstance().GetItem(guid);

                if (itemData == null)
                {
                    return;
                }

                equipUpgradeItem.OnItemVisiable(itemData);
            }
        }

        private void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            EquipUpgradeItem equipUpgradeItem = item.gameObjectBindScript as EquipUpgradeItem;
            if (equipUpgradeItem != null)
            {
                EquipUpgradeItem.ms_selected = equipUpgradeItem.ItemData;
                UpdateLeftItemInfo();
            }
        }

        private void OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            EquipUpgradeItem equipUpgradeItem = item.gameObjectBindScript as EquipUpgradeItem;
            if (equipUpgradeItem != null)
            {
                equipUpgradeItem.OnItemChangeDisplay(bSelected);
            }
        }

        /// <summary>
        /// 更新左边道具信息
        /// </summary>
        private void UpdateLeftItemInfo()
        {
            if (EquipUpgradeItem.ms_selected != null)
            {
                mEquipUpgradeDataModel = EquipUpgradeDataManager.GetInstance().GetEquipUpgradeData(EquipUpgradeItem.ms_selected.TableID);
                mItemSimpleData = EquipUpgradeDataManager.GetInstance().GetMaterialConsume(EquipUpgradeItem.ms_selected.TableID, EquipUpgradeItem.ms_selected.StrengthenLevel, EquipUpgradeItem.ms_selected.EquipType);
            }
            else
            {
                mEquipUpgradeDataModel = null;
                mItemSimpleData = new List<ItemSimpleData>();
            }
            
            UpdateSelectEquipItem(EquipUpgradeItem.ms_selected);
            UpdateUpgradeEquipItem(EquipUpgradeItem.ms_selected);
            SetConsumElementAmount();
        }
        #endregion

        #region 界面刷新相关
       
        /// <summary>
        /// 展示当前装备信息
        /// </summary>
        /// <param name="data"></param>
        private void UpdateSelectEquipItem(ItemData data)
        {
            if (mCurrentComItem == null)
            {
                mCurrentComItem = ComItemManager.CreateNew(mCurrentEquipItemParent);
            }

            if (data != null)
            {
                mCurrentComItem.Setup(data, Utility.OnItemClicked);

                if (mCurrentEquipmentName != null)
                {
                    mCurrentEquipmentName.text = data.GetColorName();
                }
            }
            else
            {
                mCurrentComItem.Setup(null, null);

                if (mCurrentEquipmentName != null)
                {
                    mCurrentEquipmentName.text = string.Empty;
                }
            }
        }

        /// <summary>
        /// 展示升级装备
        /// </summary>
        /// <param name="data"></param>
        private void UpdateUpgradeEquipItem(ItemData data)
        {
            if (mUpgradeComItem == null)
            {
                mUpgradeComItem = ComItemManager.CreateNew(mNextLvEquipItemParent);
            }

            if (mEquipUpgradeDataModel != null)
            {
                ItemData upgradeItemData = ItemDataManager.CreateItemDataFromTable(mEquipUpgradeDataModel.mUpgradeEquipID, (int)data.SubQuality);
                upgradeItemData = SwichItemData(data, upgradeItemData);
                
                mUpgradeComItem.Setup(upgradeItemData, Utility.OnItemClicked);

                if (mTargetEquipmentName != null)
                {
                    mTargetEquipmentName.text = upgradeItemData.GetColorName();
                }
            }
            else
            {
                mUpgradeComItem.Setup(null, null);

                if (mTargetEquipmentName != null)
                {
                    mTargetEquipmentName.text = string.Empty;
                }
            }
        }

        /// <summary>
        /// 把当前一些属性赋给升级道具
        /// </summary>
        /// <param name="currentItemData"></param>
        /// <param name="upgradeItemData"></param>
        /// <returns></returns>
        private ItemData SwichItemData(ItemData currentItemData,ItemData upgradeItemData)
        {
            upgradeItemData.StrengthenLevel = currentItemData.StrengthenLevel;
            upgradeItemData.bLocked = currentItemData.bLocked;
            upgradeItemData.SubQuality = currentItemData.SubQuality;
            upgradeItemData.mPrecEnchantmentCard = currentItemData.mPrecEnchantmentCard;
            upgradeItemData.PreciousBeadMountHole = currentItemData.PreciousBeadMountHole;
            upgradeItemData.BeadAdditiveAttributeBuffID = currentItemData.BeadAdditiveAttributeBuffID;
            upgradeItemData.EquipType = currentItemData.EquipType;
            upgradeItemData.GrowthAttrType = currentItemData.GrowthAttrType;
            upgradeItemData.GrowthAttrNum = EquipGrowthDataManager.GetInstance().GetGrowthAttributeValue(upgradeItemData, upgradeItemData.StrengthenLevel);
            upgradeItemData.InscriptionHoles = currentItemData.InscriptionHoles;

            var itemStrengthenAttrA = ItemStrengthAttribute.Create(upgradeItemData.TableID);
            itemStrengthenAttrA.SetStrength(upgradeItemData.StrengthenLevel);
            var itemData = itemStrengthenAttrA.GetItemData();
            
            upgradeItemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttack] = itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttack];
            upgradeItemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttackRate] = itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttackRate];
            upgradeItemData.BaseProp.props[(int)EEquipProp.IgnoreMagicAttack] = itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicAttack];
            upgradeItemData.BaseProp.props[(int)EEquipProp.IgnoreMagicAttackRate] = itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicAttackRate];
            upgradeItemData.BaseProp.props[(int)EEquipProp.IngoreIndependence] = itemData.BaseProp.props[(int)EEquipProp.IngoreIndependence];
            upgradeItemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefense] = itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefense];
            upgradeItemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefenseRate] = itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefenseRate];
            upgradeItemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefense] = itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefense];
            upgradeItemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefenseRate] = itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefenseRate];

            upgradeItemData.RefreshRateScore();

            return upgradeItemData;
        }

        private void SetConsumElementAmount()
        {
            if (mEquipUpgradeDataModel != null)
            {
                mConsumUIListScript.SetElementAmount(mItemSimpleData.Count);
            }
            else
            {
                mConsumUIListScript.SetElementAmount(0);
            }
        }

        private void InitmConsumUIListScript()
        {
            mConsumUIListScript.Initialize();
            mConsumUIListScript.onBindItem += OnConsumBindItemDelegate;
            mConsumUIListScript.onItemVisiable += OnConsumItemVisiableDelegate;
        }

        private void UnInitmConsumUIListScript()
        {
            if (mConsumUIListScript)
            {
                mConsumUIListScript.onBindItem -= OnConsumBindItemDelegate;
                mConsumUIListScript.onItemVisiable -= OnConsumItemVisiableDelegate;

                mConsumUIListScript = null;
            }
        }

        private EquipUpgradeCostItem OnConsumBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<EquipUpgradeCostItem>();
        }

        private void OnConsumItemVisiableDelegate(ComUIListElementScript item)
        {
            EquipUpgradeCostItem mCurrentCostItem = item.gameObjectBindScript as EquipUpgradeCostItem;
            if (mCurrentCostItem != null && item.m_index >= 0 && item.m_index < mItemSimpleData.Count)
            {
                mCurrentCostItem.OnItemVisiable(mItemSimpleData[item.m_index]);
            }
        }
        #endregion

        void OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            SetConsumElementAmount();
        }
        
        private void OnEquipUpgradeClick()
        {

            if (EquipUpgradeItem.ms_selected == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("EquipUpgrade_PutEquipment"));
                return;
            }

            if (EquipUpgradeItem.ms_selected.PackageType == EPackageType.WearEquip
                || EquipUpgradeItem.ms_selected.IsItemInUnUsedEquipPlan == true)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("EquipUpgrade_WearEquipDesc"));
                return;
            }

            //消耗材料检查
            bool consumCheck = false;
            int mCount = 0;
            for (int i = 0; i < mItemSimpleData.Count; i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mItemSimpleData[i].ItemID);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.Type == ItemTable.eType.MATERIAL)
                {
                    mCount = ItemDataManager.GetInstance().GetItemCount(itemData.TableID);
                }
                else
                {
                    mCount = ItemDataManager.GetInstance().GetOwnedItemCount(itemData.TableID);
                }

                if (mCount < mItemSimpleData[i].Count)
                {
                    consumCheck = true;
                    break;
                }
            }

            //检查如果消耗材料不足
            if (consumCheck)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("EquipUpgrade_LackOfMaterial"));
                return;
            }

            ItemData upgradeItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mEquipUpgradeDataModel.mUpgradeEquipID);
            if (upgradeItemData == null)
            {
                return;
            }

            //如果选中的是穿戴的装备
            if (EquipUpgradeItem.ms_selected.PackageType == EPackageType.WearEquip)
            {
                ////升级后的装备等级大于角色等级，漂提示
                //if (upgradeItemData.LevelLimit > PlayerBaseData.GetInstance().Level)
                //{
                //    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("EquipUpgrade_LevelDesc"));
                //    return;
                //}

                ////穿戴的是套装
                //if (EquipUpgradeItem.ms_selected.SuitID > 0)
                //{
                //    EquipSuitObj equipSuitObj = EquipSuitDataManager.GetInstance().GetSelfEquipSuitObj(EquipUpgradeItem.ms_selected.SuitID);
                //    if (equipSuitObj != null && equipSuitObj.equipSuitRes != null)
                //    {
                //        bool isWearSetOf = false; //表示是否穿戴一套
                //        int iSuitCount = 0;
                //        for (int i = 0; i < equipSuitObj.equipSuitRes.equips.Count; i++)
                //        {
                //            int equipID = equipSuitObj.equipSuitRes.equips[i];
                //            ItemData equip = ItemDataManager.GetInstance().GetCommonItemTableDataByID(equipID);
                //            if (!equipSuitObj.IsSuitEquipActive(equip))
                //            {
                //                isWearSetOf = false;
                //                break;
                //            }

                //            iSuitCount++;
                //            if (iSuitCount == equipSuitObj.equipSuitRes.equips.Count)
                //            {
                //                isWearSetOf = true;
                //            }
                //        }

                //        //穿戴了一整套
                //        if (isWearSetOf)
                //        {
                //            string mFullSetOfDesc = TR.Value("EquipUpgrade_FullSetOf", equipSuitObj.equipSuitRes.name);
                //            SystemNotifyManager.SysNotifyMsgBoxOkCancel(mFullSetOfDesc, () => 
                //            {
                //                OnSendSceneEquipUpdateReq();
                //            });

                //            return;
                //        }
                //        else
                //        {
                //            //未穿戴一整套
                //            if (iSuitCount >= 2)
                //            {
                //                string mNoFullSetOfDesc = TR.Value("EquipUpgrade_NoFullSetOf", equipSuitObj.equipSuitRes.name);
                //                SystemNotifyManager.SysNotifyMsgBoxOkCancel(mNoFullSetOfDesc, () =>
                //                {
                //                    OnSendSceneEquipUpdateReq();
                //                });

                //                return;
                //            }
                //        }
                //    }
                //}
            }

            //背包中的装备，//升级后的装备等级大于角色等级的提示
            if (upgradeItemData.LevelLimit > PlayerBaseData.GetInstance().Level)
            {
                string packageEquipUpgradeDesc = TR.Value("EquipUpgrade_PackageEquipUpgrade");
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(packageEquipUpgradeDesc, () => 
                {
                    OnSendSceneEquipUpdateReq();
                });

                return;
            }

            string mContent = TR.Value("EquipUpgrade_Tip", EquipUpgradeItem.ms_selected.GetColorName(), upgradeItemData.GetColorName());
            SystemNotifyManager.SysNotifyMsgBoxOkCancel(mContent, () => 
            {
                OnSendSceneEquipUpdateReq();
            });
        }

        /// <summary>
        /// 装备升级请求
        /// </summary>
        private void OnSendSceneEquipUpdateReq()
        {
            EquipUpgradeDataManager.GetInstance().OnSendSceneEquieUpdateReq(EquipUpgradeItem.ms_selected.GUID);
        }

        void OnDestroy()
        {
            UnRegisterEventHander();
            UnInitEquipUIListScript();
            UnInitmConsumUIListScript();
            mAllEquipments = null;
            mCurrentComItem = null;
            mUpgradeComItem = null;
            mEquipUpgradeDataModel = null;
            mItemSimpleData = null;
            EquipUpgradeItem.ms_selected = null;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnEquipUpgradeSuccess, OnEquipUpgradeSuccess);
            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshEquipmentList, OnEquipUpgradeSuccess);
        }
    }
}

