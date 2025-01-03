using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using Protocol;
using ProtoTable;

namespace GameClient
{
    class BeadPerfectReplacementView : MonoBehaviour,IDisposable
    {
        [SerializeField]
        private ComUIListScript mInalyBeadEquipmentComUIListScript;
        [SerializeField]
        private ComUIListScript mBeadComUIListScript;
        [SerializeField]
        private Button mCloseBtn;
        [SerializeField]
        private Button mPrefectReplaceBtn;
        [SerializeField]
        private Text mGoldInfo;
        [SerializeField]
        private Image mGoldIcon;
        [SerializeField]
        private GameObject mNoBeadRoot;

        BeadPerfectReplacementModel mModelData;
        public delegate void OnMountedItemSelect(BeadItemModel model);//选择已镶嵌的宝珠
        public OnMountedItemSelect onMountedItemSelect;
        public delegate void OnUnMountedItemSelect(ItemData mItemData);//选择未镶嵌的宝珠
        public OnUnMountedItemSelect onUnMountedItemSelect;
        public delegate void OnOkButtonClick();//完美置换
        public OnOkButtonClick onOkButtonClick;
        List<BeadItemModel> equipments = new List<BeadItemModel>();
        List<ItemData> beadList = new List<ItemData>();
        int mCurrentSelectHoleType;
        ItemData mCurrentSelectInalyBead = null;
        public void InitView(BeadPerfectReplacementModel mModel, OnMountedItemSelect onMountedItemSelect, OnUnMountedItemSelect onUnMountedItemSelect, OnOkButtonClick onOkButtonClick, UnityAction closeCallBack)
        {
            mModelData = mModel;
            this.onMountedItemSelect = onMountedItemSelect;
            this.onUnMountedItemSelect = onUnMountedItemSelect;
            this.onOkButtonClick = onOkButtonClick;
            mCloseBtn.onClick.RemoveAllListeners();
            mCloseBtn.onClick.AddListener(closeCallBack);
            OnOKButoonAddListener();
            InitBeadComUIListScript();
            InitInalyBeadEquipmentComUIListScript();
            //UpdateNoBeadRoot();
        }

#region 初始化已镶嵌的宝珠列表

        void InitInalyBeadEquipmentComUIListScript()
        {
            mInalyBeadEquipmentComUIListScript.Initialize();
            mInalyBeadEquipmentComUIListScript.onBindItem += _OnBindItemDelegate;
            mInalyBeadEquipmentComUIListScript.onItemVisiable += _OnItemVisiableDelegate;
            mInalyBeadEquipmentComUIListScript.onItemSelected += _OnItemSelectDelegate;
            mInalyBeadEquipmentComUIListScript.onItemChageDisplay += _OnItemChangeDisplayDelegate;
            _LoadAllBeadEquipItems(ref equipments);
            TrySelectedItem(0);
        }

        BeadItem _OnBindItemDelegate(GameObject itemObject)
        {
            BeadItem beadItem = itemObject.GetComponent<BeadItem>();
            return beadItem;
        }

        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as BeadItem;
            if (current != null && item.m_index >= 0 && item.m_index < equipments.Count)
            {
                current.OnItemVisible(equipments[item.m_index]);
            }
        }

        void _OnItemSelectDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as BeadItem;
            BeadItem.ms_select = current == null ? null : current.Model;
            mCurrentSelectHoleType = BeadItem.ms_select.holeType;
            mCurrentSelectInalyBead = BeadItem.ms_select.beadItemData;
            if (onMountedItemSelect != null)
            {
                onMountedItemSelect.Invoke(BeadItem.ms_select);
            }
        }

        void _OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var current = item.gameObjectBindScript as BeadItem;
            if (current != null)
            {
                current.OnItemChangeDisplay(bSelected);
            }
        }

        void _LoadAllBeadEquipItems(ref List<BeadItemModel> kBeadItems)
        {
            kBeadItems.Clear();
            for (int i = 0; i < mModelData.mEquipItemData.PreciousBeadMountHole.Length; i++)
            {
                var mPreBead = mModelData.mEquipItemData.PreciousBeadMountHole[i];
                if (mPreBead == null)
                {
                    continue;
                }

                var beadItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID((int)mPreBead.preciousBeadId);
                if (beadItemData == null)
                {
                    continue;
                }
                BeadItemModel model = new BeadItemModel((int)UpgradePrecType.Mounted, beadItemData, (int)mPreBead.index, mModelData.mEquipItemData, (int)mPreBead.randomBuffId, mPreBead.pickNumber,mPreBead.type,mPreBead.beadReplaceNumber);
                kBeadItems.Add(model);
            }

            kBeadItems.Sort(_SortBeadItems);

            mInalyBeadEquipmentComUIListScript.SetElementAmount(equipments.Count);
        }

        int _SortBeadItems(BeadItemModel left, BeadItemModel right)
        {
            if (left.beadItemData.Quality != right.beadItemData.Quality)
            {
                return (int)right.beadItemData.Quality - (int)left.beadItemData.Quality;
            }
            return GetBeadLevel(right.beadItemData.TableID) - GetBeadLevel(left.beadItemData.TableID);
        }

        int GetBeadLevel(int beadId)
        {
            int level = 0;
            var mBeadTable = TableManager.GetInstance().GetTableItem<BeadTable>(beadId);
            if (mBeadTable != null)
            {
                level = mBeadTable.Level;
            }

            return level;
        }

        void TrySelectedItem(int iBindIndex)
        {
            if (iBindIndex < 0 || equipments.Count < 0)
            {
                return;
            }

            if (iBindIndex >= 0 && iBindIndex < equipments.Count)
            {
                BeadItem.ms_select = equipments[iBindIndex];
                if (!mInalyBeadEquipmentComUIListScript.IsElementInScrollArea(iBindIndex))
                {
                    mInalyBeadEquipmentComUIListScript.EnsureElementVisable(iBindIndex);
                }
                mInalyBeadEquipmentComUIListScript.SelectElement(iBindIndex);
            }
            else
            {
                mInalyBeadEquipmentComUIListScript.SelectElement(-1);
                BeadItem.ms_select = null;
            }

            mCurrentSelectHoleType = BeadItem.ms_select.holeType;
            mCurrentSelectInalyBead = BeadItem.ms_select.beadItemData;
            if (onMountedItemSelect != null)
            {
                onMountedItemSelect.Invoke(BeadItem.ms_select);
            }
        }

        void UnInitInalyBeadEquipmentComUIListScript()
        {
            if (mInalyBeadEquipmentComUIListScript != null)
            {
                mInalyBeadEquipmentComUIListScript.onBindItem -= _OnBindItemDelegate;
                mInalyBeadEquipmentComUIListScript.onItemVisiable -= _OnItemVisiableDelegate;
                mInalyBeadEquipmentComUIListScript.onItemSelected -= _OnItemSelectDelegate;
                mInalyBeadEquipmentComUIListScript.onItemChageDisplay -= _OnItemChangeDisplayDelegate;
                mInalyBeadEquipmentComUIListScript = null;
            }
        }
        #endregion

        #region 初始化可替换宝珠列表
        void InitBeadComUIListScript()
        {
            mBeadComUIListScript.Initialize();
            mBeadComUIListScript.onBindItem += _OnBeadBindItemDelegate;
            mBeadComUIListScript.onItemVisiable += _OnBeadItemVisiableDelegate;
            mBeadComUIListScript.onItemSelected += _OnBeadItemSelectDelegate;
            mBeadComUIListScript.onItemChageDisplay += _OnBeadItemChangeDisplayDelegate;
            //_LoadAllBeadItems(ref beadList);
        }

        public void RefreshBeadItemList()
        {
            _LoadAllBeadItems(ref beadList);
            UpdateNoBeadRoot();
        }

        ComSarahCardInlayItem _OnBeadBindItemDelegate(GameObject itemObject)
        {
            ComSarahCardInlayItem beadItem = itemObject.GetComponent<ComSarahCardInlayItem>();
            return beadItem;
        }

        void _OnBeadItemVisiableDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComSarahCardInlayItem;
            if (current != null && item.m_index >= 0 && item.m_index < beadList.Count)
            {
                current.OnItemVisible(beadList[item.m_index]);
            }
        }

        void _OnBeadItemSelectDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ComSarahCardInlayItem;
            ComSarahCardInlayItem.ms_selected = current == null ? null : current.ItemData;
            if (onUnMountedItemSelect != null)
            {
                onUnMountedItemSelect.Invoke(ComSarahCardInlayItem.ms_selected);
            }
        }

        void _OnBeadItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var current = item.gameObjectBindScript as ComSarahCardInlayItem;
            if (current != null)
            {
                current.OnItemChangeDisplay(bSelected);
            }
        }

        void _LoadAllBeadItems(ref List<ItemData> kBeadItems)
        {
            kBeadItems.Clear();

            var itemids = ItemDataManager.GetInstance().GetItemsByType(ItemTable.eType.EXPENDABLE);
            for (int i = 0; i < itemids.Count; ++i)
            {
                var itemData = ComSarahCardInlayItem._TryAddSarahCard(itemids[i]);
                if (itemData != null)
                {
                    kBeadItems.Add(itemData);
                }
            }
            var mCurrentSelectInalyBeadTabel = TableManager.GetInstance().GetTableItem<BeadTable>((int)mCurrentSelectInalyBead.TableID);
            if (mCurrentSelectInalyBeadTabel == null)
            {
                Logger.LogErrorFormat("[BeadPerfectReplacementView]  _LoadAllBeadItems 函数 当前选择已镶嵌的宝珠ID为 NULL");
            }
            kBeadItems.RemoveAll(x =>
            {
                var beadItem = TableManager.GetInstance().GetTableItem<BeadTable>((int)x.TableID);
                if (beadItem == null)
                {
                    return true;
                }

                if (mCurrentSelectHoleType == (int)PreciousBeadMountHoleType.PBMHT_COMMON)
                {
                    if (beadItem.BeadType != 1)
                    {
                        return true;
                    }
                }
                
                if (mModelData == null || mModelData.mEquipItemData == null)
                {
                    return true;
                }

                if (!mCurrentSelectInalyBeadTabel.ReplacePearl.Contains(x.TableID))
                {
                    return true;
                }

                if (beadItem.Parts.Contains((int)mModelData.mEquipItemData.EquipWearSlotType))
                {
                    return false;
                }

                return true;
            });

            kBeadItems.Sort(ComSarahCardInlayItem.Sort);
            mBeadComUIListScript.SetElementAmount(beadList.Count);
        }

        void UnBeadComUIListScript()
        {
            if (mBeadComUIListScript != null)
            {
                mBeadComUIListScript.onBindItem -= _OnBeadBindItemDelegate;
                mBeadComUIListScript.onItemVisiable -= _OnBeadItemVisiableDelegate;
                mBeadComUIListScript.onItemSelected -= _OnBeadItemSelectDelegate;
                mBeadComUIListScript.onItemChageDisplay -= _OnBeadItemChangeDisplayDelegate;
                mBeadComUIListScript = null;
            }
        }

        void UpdateNoBeadRoot()
        {
            if (beadList.Count > 0)
            {
                mNoBeadRoot.CustomActive(false);
            }
            else
            {
                mNoBeadRoot.CustomActive(true);
            }
        }
        #endregion

        public void UpdateExpendGoldInfo(ItemSimpleData mSimpleData)
        {
            if (mSimpleData == null)
            {
                return;
            }

            ItemData mData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mSimpleData.ItemID);
            if (mData == null)
            {
                return;
            }

            if (mGoldIcon != null)
            {
                ETCImageLoader.LoadSprite(ref mGoldIcon, mData.Icon);
            }

            if (mGoldInfo != null)
            {
                int mCount = ItemDataManager.GetInstance().GetOwnedItemCount(mSimpleData.ItemID);
                if (mCount < mSimpleData.Count)
                {
                    mGoldInfo.text = string.Format("<color={0}>{1}</color>", TR.Value("Bead_red_color"), mSimpleData.Count);
                }
                else
                {
                    mGoldInfo.text = string.Format("<color={0}>{1}</color>", TR.Value("Bead_Green_color"), mSimpleData.Count);
                }
            }
        }

        void OnOKButoonAddListener()
        {
            if (mPrefectReplaceBtn != null)
            {
                mPrefectReplaceBtn.onClick.RemoveAllListeners();
                mPrefectReplaceBtn.onClick.AddListener(() =>
                {
                    if (onOkButtonClick != null)
                    {
                        onOkButtonClick.Invoke();
                    }
                });
            }
        }
        public void Dispose()
        {
            mCloseBtn.onClick.RemoveAllListeners();
            mPrefectReplaceBtn.onClick.RemoveAllListeners();
            UnInitInalyBeadEquipmentComUIListScript();
            UnBeadComUIListScript();
            equipments.Clear();
            beadList.Clear();
            mCurrentSelectHoleType = 0;
            onMountedItemSelect = null;
            onUnMountedItemSelect = null;
            onOkButtonClick = null;
            mCurrentSelectInalyBead = null;
        }
    }
}

