using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using System;
using Protocol;

namespace GameClient
{
    public class MaterialSynthesisView : MonoBehaviour
    {
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private GameObject mCostPrefabs;
        [SerializeField] private GameObject mCostParent;
        [SerializeField] private Text mName;
        [SerializeField] private Text mInputNumber;
        [SerializeField] private Text mItemDesc;
        [SerializeField] private Button mInputBtn;
        [SerializeField] private Button mSyntheisBtn;
        [SerializeField] private Button mAddBtn;
        [SerializeField] private Button mReductionBtn;
        [SerializeField] private ComUIListScript mMaterialSynthesisUIListScript;
        [SerializeField] private int iMaxNumber = 99;

        private List<GameObject> mCostPrefabsList = new List<GameObject>();
        private List<MaterialsSynthesisData> mMaterialsSynthesisItemList = new List<MaterialsSynthesisData>();
        private int iSynthesisNumber = 1;
        private MaterialsSynthesisData mCurrentSelectMaterialsSynthesisData;
        private CommonNewItem  commonNewItem;
        List<ItemSimpleData> costMaterials = new List<ItemSimpleData>();

        public void InitView()
        {
            if (mCostPrefabsList == null)
            {
                mCostPrefabsList = new List<GameObject>();
            }
            else
            {
                mCostPrefabsList.Clear();
            }

            if (mMaterialsSynthesisItemList == null)
            {
                mMaterialsSynthesisItemList = new List<MaterialsSynthesisData>();
            }
            else
            {
                mMaterialsSynthesisItemList.Clear();
            }

            if (commonNewItem == null)
            {
                commonNewItem = CommonUtility.CreateCommonNewItem(mItemParent);
            }

            var lists = EquipGrowthDataManager.GetInstance().GetMaterialsSynthesisData();
            mMaterialsSynthesisItemList.AddRange(lists);

            if (mMaterialSynthesisUIListScript != null)
            {
                mMaterialSynthesisUIListScript.SetElementAmount(mMaterialsSynthesisItemList.Count);
            }

            TrySetDefaultItem();
        }
        
        private void Awake()
        {
            InitMaterialSynthesisUIListScript();
            RegisterDelegateHandler();
            RegisterUIEventHandle();
            if (mInputBtn != null)
            {
                mInputBtn.onClick.RemoveAllListeners();
                mInputBtn.onClick.AddListener(OnInputBtnClick);
            }

            if (mSyntheisBtn != null)
            {
                mSyntheisBtn.onClick.RemoveAllListeners();
                mSyntheisBtn.onClick.AddListener(OnSyntheisBtnClick);
            }

            if(mAddBtn != null)
            {
                mAddBtn.onClick.RemoveAllListeners();
                mAddBtn.onClick.AddListener(OnAddBtnClick);
            }

            if(mReductionBtn != null)
            {
                mReductionBtn.onClick.RemoveAllListeners();
                mReductionBtn.onClick.AddListener(OnReductionBtnClick);
            }
        }

        private void OnDestroy()
        {
            iSynthesisNumber = 1;
            if (mCostPrefabsList != null)
            {
                mCostPrefabsList.Clear();
            }

            if (mMaterialsSynthesisItemList != null)
            {
                mMaterialsSynthesisItemList.Clear();
            }

            if (costMaterials != null)
            {
                costMaterials.Clear();
            }

            if (mInputBtn != null)
            {
                mInputBtn.onClick.RemoveListener(OnInputBtnClick);
            }

            if (mSyntheisBtn != null)
            {
                mSyntheisBtn.onClick.RemoveListener(OnSyntheisBtnClick);
            }

            if (mAddBtn != null)
            {
                mAddBtn.onClick.RemoveListener(OnAddBtnClick);
            }

            if (mReductionBtn != null)
            {
                mReductionBtn.onClick.RemoveListener(OnReductionBtnClick);
            }

            commonNewItem = null;
            UnInitMaterialSynthesisUIListScript();
            UnRegisterDelegateHandler();
            UnRegisterUIEventHandle();
        }

        private void RegisterDelegateHandler()
        {
            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += OnAddNewItem;
        }

        private void UnRegisterDelegateHandler()
        {
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnAddNewItem;
        }

        private void RegisterUIEventHandle()
        {
            //绑定键盘事件
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCommonKeyBoardInput, OnCommonKeyBoardInput);
            RegisterDelegateHandler();
        }

        private void UnRegisterUIEventHandle()
        {
            //绑定键盘事件
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCommonKeyBoardInput, OnCommonKeyBoardInput);
            UnRegisterDelegateHandler();
        }

        private void OnCommonKeyBoardInput(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null || uiEvent.Param2 == null)
            {
                return;
            }
            CommonKeyBoardInputType inputType = (CommonKeyBoardInputType)uiEvent.Param1;
            ulong inputValue = (ulong)uiEvent.Param2;
            if (inputType == CommonKeyBoardInputType.ChangeNumber)
            {

                iSynthesisNumber = (int)inputValue;
                UpdateInputNumber(iSynthesisNumber);
            }
            else if (inputType == CommonKeyBoardInputType.Finished)
            {
                iSynthesisNumber = Mathf.Clamp(iSynthesisNumber, 1, iMaxNumber);
                UpdateInputNumber(iSynthesisNumber);
                UpdateCostMaterialsItem(mCurrentSelectMaterialsSynthesisData);
            }
        }

        private void OnAddNewItem(List<Item> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.Type == ProtoTable.ItemTable.eType.MATERIAL)
                {
                    UpdateCostMaterialsItem(mCurrentSelectMaterialsSynthesisData);
                    break;
                }
            }
        }

        private void UpdateInputNumber(int number)
        {
            if (mInputNumber != null)
            {
                mInputNumber.text = number.ToString();
            }
        }
        
        private void InitMaterialSynthesisUIListScript()
        {
            if (mMaterialSynthesisUIListScript != null)
            {
                mMaterialSynthesisUIListScript.Initialize();
                mMaterialSynthesisUIListScript.onBindItem += OnBindItemDelegate;
                mMaterialSynthesisUIListScript.onItemVisiable += OnItemVisiableDelegate;
                mMaterialSynthesisUIListScript.onItemSelected += OnItemSelectedDelegate;
                mMaterialSynthesisUIListScript.onItemChageDisplay += OnItemChangedDisplayDelegate;
            }
        }

        private void UnInitMaterialSynthesisUIListScript()
        {
            if (mMaterialSynthesisUIListScript != null)
            {
                mMaterialSynthesisUIListScript.onBindItem -= OnBindItemDelegate;
                mMaterialSynthesisUIListScript.onItemVisiable -= OnItemVisiableDelegate;
                mMaterialSynthesisUIListScript.onItemSelected -= OnItemSelectedDelegate;
                mMaterialSynthesisUIListScript.onItemChageDisplay -= OnItemChangedDisplayDelegate;
            }
        }

        private MaterialSynthesisItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<MaterialSynthesisItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var materialSynthesisItem = item.gameObjectBindScript as MaterialSynthesisItem;
            if (materialSynthesisItem != null && item.m_index >= 0 && item.m_index < mMaterialsSynthesisItemList.Count)
            {
                materialSynthesisItem.OnItemVisiable(mMaterialsSynthesisItemList[item.m_index]);
            }
        }

        private void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var materialSynthesisItem = item.gameObjectBindScript as MaterialSynthesisItem;
            if (materialSynthesisItem != null)
            {
                mCurrentSelectMaterialsSynthesisData = materialSynthesisItem.mMaterialsSynthesisData;
                OnCostItemClick(mCurrentSelectMaterialsSynthesisData);
            }
        }

        private void OnItemChangedDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var materialSynthesisItem = item.gameObjectBindScript as MaterialSynthesisItem;
            if (materialSynthesisItem != null)
            {
                materialSynthesisItem.OnOnItemChangeDisplayDelegate(bSelected);
            }
        }

        private void OnCostItemClick(MaterialsSynthesisData data)
        {
            if (data == null)
            {
                return;
            }

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(data.tableID);
            if (itemData == null)
            {
                return;
            }

            if (commonNewItem != null)
            {
                commonNewItem.InitItem(itemData.TableID);
            }

            mName.text = itemData.GetColorName();
            if (mItemDesc != null)
                mItemDesc.text = itemData.Description;

            iSynthesisNumber = 1;
            UpdateInputNumber(iSynthesisNumber);
            UpdateCostMaterialsItem(data);
        }

        private void UpdateCostMaterialsItem(MaterialsSynthesisData data)
        {
            if (data == null)
            {
                return;
            }
            
            for (int i = 0; i < mCostPrefabsList.Count; i++)
            {
                mCostPrefabsList[i].CustomActive(false);
            }
            
            var costs = EquipGrowthDataManager.GetInstance().GetCostMaterialsList(data.tableID);

            if (costMaterials != null)
            {
                costMaterials.Clear();
            }

            costMaterials.AddRange(costs);
            for (int i = 0; i < costMaterials.Count; i++)
            {
                var itemSimpleData = costMaterials[i];
                if (i < mCostPrefabsList.Count)
                {
                    GameObject obj = mCostPrefabsList[i];
                    if (obj != null)
                    {
                        EquipUpgradeCostItem costItem = obj.GetComponent<EquipUpgradeCostItem>();
                        if (costItem != null)
                        {
                            costItem.OnItemVisiable(itemSimpleData, iSynthesisNumber);
                        }
                        obj.CustomActive(true);
                    }
                }
                else
                {
                    GameObject go = GameObject.Instantiate(mCostPrefabs);
                    
                    if (go != null)
                    {
                       
                        Utility.AttachTo(go, mCostParent);
                        EquipUpgradeCostItem costItem = go.GetComponent<EquipUpgradeCostItem>();
                        if (costItem != null)
                        {
                            costItem.OnItemVisiable(itemSimpleData, iSynthesisNumber);
                        }

                        mCostPrefabsList.Add(go);
                        go.CustomActive(true);
                    }
                }
            }

            if (costMaterials.Count >= 3)
            {
                int a = ItemDataManager.GetInstance().GetOwnedItemCount(costMaterials[0].ItemID) / costMaterials[0].Count;
                int b = ItemDataManager.GetInstance().GetOwnedItemCount(costMaterials[1].ItemID) / costMaterials[1].Count;
                int c = ItemDataManager.GetInstance().GetOwnedItemCount(costMaterials[2].ItemID) / costMaterials[2].Count;

                int d = a < b ? a : b;
                iMaxNumber = d < c ? d : c;
            }
            else if (costMaterials.Count == 2)
            {
                int a = ItemDataManager.GetInstance().GetOwnedItemCount(costMaterials[0].ItemID) / costMaterials[0].Count;
                int b = ItemDataManager.GetInstance().GetOwnedItemCount(costMaterials[1].ItemID) / costMaterials[1].Count;
                iMaxNumber = a < b ? a : b;
            }

            iMaxNumber = iMaxNumber == 0 ? 1 : iMaxNumber;
        }

        private void TrySetDefaultItem()
        {
            int iSelectIndex = -1;
            if (mCurrentSelectMaterialsSynthesisData != null)
            {
                for (int i = 0; i < mMaterialsSynthesisItemList.Count; i++)
                {
                    var item = mMaterialsSynthesisItemList[i];
                    if (item.tableID != mCurrentSelectMaterialsSynthesisData.tableID)
                    {
                        continue;
                    }

                    iSelectIndex = i;
                    break;
                }
            }
            else
            {
                if (mMaterialsSynthesisItemList.Count > 0)
                {
                    iSelectIndex = 0;
                }
            }

            SetSelectItem(iSelectIndex);
        }

        private void SetSelectItem(int index)
        {
            if (index >= 0 && index < mMaterialsSynthesisItemList.Count)
            {
                if (mMaterialSynthesisUIListScript != null)
                {
                    mCurrentSelectMaterialsSynthesisData = mMaterialsSynthesisItemList[index];
                    if (!mMaterialSynthesisUIListScript.IsElementInScrollArea(index))
                    {
                        mMaterialSynthesisUIListScript.EnsureElementVisable(index);
                    }

                    mMaterialSynthesisUIListScript.SelectElement(index);
                }
            }
            else
            {
                if (mMaterialSynthesisUIListScript != null)
                {
                    mMaterialSynthesisUIListScript.SelectElement(-1);
                    mCurrentSelectMaterialsSynthesisData = null;
                }
            }

            OnCostItemClick(mCurrentSelectMaterialsSynthesisData);
        }

        private void OnInputBtnClick()
        {
            CommonUtility.OnOpenCommonKeyBoardFrame(new Vector3(500, 0, 0), 0, 99);
        }

        private void OnAddBtnClick()
        {
            iSynthesisNumber++;
            iSynthesisNumber = Mathf.Clamp(iSynthesisNumber, 1, iMaxNumber);
            UpdateInputNumber(iSynthesisNumber);
        }

        private void OnReductionBtnClick()
        {
            iSynthesisNumber--;
            iSynthesisNumber = Mathf.Clamp(iSynthesisNumber, 1, iMaxNumber);
            UpdateInputNumber(iSynthesisNumber);
        }

        /// <summary>
        /// 合成按钮
        /// </summary>
        private void OnSyntheisBtnClick()
        {
            if (mCurrentSelectMaterialsSynthesisData == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation("请选择要合成的材料");
                return;
            }

            if (costMaterials != null)
            {
                for (int i = 0; i < costMaterials.Count; i++)
                {
                    var item = costMaterials[i];
                    int needCount = item.Count * iSynthesisNumber;
                    int totalCout = ItemDataManager.GetInstance().GetOwnedItemCount(item.ItemID);
                    if (needCount > totalCout)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation("材料不足");
                        return;
                    }
                }
            }
            
            EquipGrowthDataManager.GetInstance().OnSceneEnhanceMaterialCombo((UInt32)mCurrentSelectMaterialsSynthesisData.tableID, (UInt32)iSynthesisNumber);
        }
    }
}