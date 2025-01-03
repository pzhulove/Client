using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using System;
using ProtoTable;

namespace GameClient
{
    public class InscriptionMosaicView : MonoBehaviour,ISmithShopNewView
    {
        [SerializeField] private ComUIListScript mEquipmentUIListScript;
        [SerializeField] private ComUIListScript mInscriptionUIListScript;
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private Text mName;
        [SerializeField] private GameObject mInscriptionHoleRoot;
        [SerializeField] private Button mInscriptionMergeBtn;
        [SerializeField] private Button mBtnMosaic;
        [SerializeField] private UIGray mGrayMosaic;
        [SerializeField] private ToggleGroup mToggleGroup;

        [SerializeField] private string mSInscriptionHoleItemPath = "UIFlatten/Prefabs/SmithShop/InscriptionFrame/InscriptionHoleItem";

        private List<GameObject> mInscriptionHoleInfoList = new List<GameObject>();
        private List<ItemData> mAllInscripionEquipment = new List<ItemData>();
        private ComItemNew mComItem;
        private ItemData mCurrentSelectedEquipItemData;//当前选择的装备
        private ItemData mCurrentSelectedInscriptionItem;//当前选择的铭文道具
        private int mInscriptionHoleIndex = 0;//镶嵌铭文孔索引     
        /// <summary>
        /// 默认选择的铭文孔索引
        /// </summary>
        private int iDefaultSelectedHoleIndex = 1;
        /// <summary>
        /// 铭文列表
        /// </summary>
        private List<ItemData> mInscriptionItemDataList = new List<ItemData>();
        private void Awake()
        {
            if (mBtnMosaic != null)
            {
                mBtnMosaic.onClick.RemoveAllListeners();
                mBtnMosaic.onClick.AddListener(OnInscriptionMosaicClick);
            }
            
            if(mInscriptionMergeBtn != null)
            {
                mInscriptionMergeBtn.onClick.RemoveAllListeners();
                mInscriptionMergeBtn.onClick.AddListener(OnInscriptionMergeBtnClick);
            }

            BindUIEventSystem();
            InitEquipmentUIListScript();
            InitInscriptionUIList();
        }

        private void OnDestroy()
        {
            if (mBtnMosaic != null)
            {
                mBtnMosaic.onClick.RemoveAllListeners();
            }
            
            if (mInscriptionMergeBtn != null)
            {
                mInscriptionMergeBtn.onClick.RemoveAllListeners();
            }
            
            if (mInscriptionHoleInfoList != null)
            {
                mInscriptionHoleInfoList.Clear();
            }

            if (mAllInscripionEquipment != null)
            {
                mAllInscripionEquipment.Clear();
            }

            if(mInscriptionItemDataList != null)
            {
                mInscriptionItemDataList.Clear();
            }

            mComItem = null;
            mCurrentSelectedEquipItemData = null;
            mCurrentSelectedInscriptionItem = null;
            mInscriptionHoleIndex = 0;
            iDefaultSelectedHoleIndex = 1;

            UnBindUIEventSystem();
            UnInitEquipmentUIListScript();
            UnInitInscriptionUIList();
        }

        public void InitView(SmithShopNewLinkData linkData)
        {
            UpdateInscriptionEquipment();
        }

        public void OnEnableView()
        {
            UpdateInscriptionEquipment();
        }

        public void OnDisableView() { }

        #region EquipmentUIListScript

        private void InitEquipmentUIListScript()
        {
            if (mEquipmentUIListScript != null)
            {
                mEquipmentUIListScript.Initialize();
                mEquipmentUIListScript.onBindItem += OnBindItemDelegate;
                mEquipmentUIListScript.onItemVisiable += OnItemVisiableDelegate;
                mEquipmentUIListScript.onItemSelected += OnItemSelectedDelegate;
                mEquipmentUIListScript.onItemChageDisplay += OnItemChangeDisplayDelegate;
            }
        }

        private void UnInitEquipmentUIListScript()
        {
            if (mEquipmentUIListScript != null)
            {
                mEquipmentUIListScript.onBindItem -= OnBindItemDelegate;
                mEquipmentUIListScript.onItemVisiable -= OnItemVisiableDelegate;
                mEquipmentUIListScript.onItemSelected -= OnItemSelectedDelegate;
                mEquipmentUIListScript.onItemChageDisplay -= OnItemChangeDisplayDelegate;
            }
        }

        private InscriptionEquipmentItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<InscriptionEquipmentItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var equipmentItem = item.gameObjectBindScript as InscriptionEquipmentItem;
            if (equipmentItem != null && item.m_index >= 0 && item.m_index < mAllInscripionEquipment.Count)
            {
                equipmentItem.OnitemVisiable(mAllInscripionEquipment[item.m_index]);
            }
        }

        private void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var equipmentItem = item.gameObjectBindScript as InscriptionEquipmentItem;
            if (equipmentItem != null)
            {
                InscriptionEquipmentItem.mSelectItemData = equipmentItem.CurrentItemData;
                mCurrentSelectedInscriptionItem = null;
                UpdateLeftEquipmentInfo(equipmentItem.CurrentItemData);
            }
        }

        private void OnItemChangeDisplayDelegate(ComUIListElementScript item,bool isSelected)
        {
            var equipmentItem = item.gameObjectBindScript as InscriptionEquipmentItem;
            if (equipmentItem != null)
                equipmentItem.OnItemChangeDisplay(isSelected);
        }

        private void SetElementAmount()
        {
            List<Vector2> sizeList = new List<Vector2>();
            for (int i = 0; i < mAllInscripionEquipment.Count; i++)
            {
                List<InscriptionHoleData> inscriptionHoleList = mAllInscripionEquipment[i].InscriptionHoles;
                if (inscriptionHoleList == null)
                    continue;

                bool bIsDoubleHole = false;
                for (int j = 0; j < inscriptionHoleList.Count; j++)
                {
                    InscriptionHoleData holeData = inscriptionHoleList[j];
                    if (holeData == null)
                    {
                        bIsDoubleHole = false;
                        break;
                    }

                    bIsDoubleHole = true;
                }

                if (bIsDoubleHole)
                {
                    sizeList.Add(new Vector2(422, 211));
                }
                else
                {
                    sizeList.Add(new Vector2(422, 151));
                }
            }

            mEquipmentUIListScript.SetElementAmount(mAllInscripionEquipment.Count, sizeList);
        }

        private void TrySelectDefultItem()
        {
            int index = -1;

            if (InscriptionEquipmentItem.mSelectItemData != null)
            {
                var isFind = mAllInscripionEquipment.Find(x=> { return x.GUID == InscriptionEquipmentItem.mSelectItemData.GUID; } );
                if (isFind != null)
                {
                    InscriptionEquipmentItem.mSelectItemData = isFind;
                }
                else
                {
                    InscriptionEquipmentItem.mSelectItemData = null;
                }
            }

            if (InscriptionEquipmentItem.mSelectItemData != null)
            {
                for (int i = 0; i < mAllInscripionEquipment.Count; i++)
                {
                    ulong guid = mAllInscripionEquipment[i].GUID;
                    if (guid != InscriptionEquipmentItem.mSelectItemData.GUID)
                    {
                        continue;
                    }

                    index = i;
                }
            }
            else
            {
                if (mAllInscripionEquipment.Count > 0)
                {
                    index = 0;
                }
            }

            if (index >= 0 && index < mAllInscripionEquipment.Count)
            {
                InscriptionEquipmentItem.mSelectItemData = mAllInscripionEquipment[index];
                if (!mEquipmentUIListScript.IsElementInScrollArea(index))
                {
                    mEquipmentUIListScript.EnsureElementVisable(index);
                }
                mEquipmentUIListScript.SelectElement(index);
            }
            else
            {
                mEquipmentUIListScript.SelectElement(-1);
                InscriptionEquipmentItem.mSelectItemData = null;
            }

            if (InscriptionEquipmentItem.mSelectItemData != null)
            {
                UpdateLeftEquipmentInfo(InscriptionEquipmentItem.mSelectItemData);
            }
        }
        #endregion

        #region InscriptionUIListScript

        private void InitInscriptionUIList()
        {
            if (mInscriptionUIListScript != null)
            {
                mInscriptionUIListScript.Initialize();
                mInscriptionUIListScript.onBindItem += OnBindInscriptionItemDelegate;
                mInscriptionUIListScript.onItemVisiable += OnInscriptionItemVisiableDelegate;
                mInscriptionUIListScript.onItemSelected += OnInscriptionItemSelectedDelegate;
                mInscriptionUIListScript.onItemChageDisplay += OnInscriptionItemChangeDisplayDelegate;
            }
        }

        private void UnInitInscriptionUIList()
        {
            if (mInscriptionUIListScript != null)
            {
                mInscriptionUIListScript.onBindItem -= OnBindInscriptionItemDelegate;
                mInscriptionUIListScript.onItemVisiable -= OnInscriptionItemVisiableDelegate;
                mInscriptionUIListScript.onItemSelected -= OnInscriptionItemSelectedDelegate;
                mInscriptionUIListScript.onItemChageDisplay -= OnInscriptionItemChangeDisplayDelegate;
            }
        }

        private InscriptionItemElement OnBindInscriptionItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<InscriptionItemElement>();
        }

        private void OnInscriptionItemVisiableDelegate(ComUIListElementScript item)
        {
            var itemElement = item.gameObjectBindScript as InscriptionItemElement;
            if (itemElement != null && item.m_index >= 0 && item.m_index < mInscriptionItemDataList.Count)
            {
                itemElement.OnItemVisiable(mInscriptionItemDataList[item.m_index]);
            }
        }

        private void OnInscriptionItemSelectedDelegate(ComUIListElementScript item)
        {
            var itemElement = item.gameObjectBindScript as InscriptionItemElement;
            if (itemElement != null)
            {
                mCurrentSelectedInscriptionItem = itemElement.CurrentItemData;
            }
        }

        private void OnInscriptionItemChangeDisplayDelegate(ComUIListElementScript item, bool isSelected)
        {
            var itemElement = item.gameObjectBindScript as InscriptionItemElement;
            if (itemElement != null)
                itemElement.OnItemChangeDisplay(isSelected);
        }

        private void OnSetInscriptionItemElementAmount()
        {
            mCurrentSelectedInscriptionItem = null;

            if (mInscriptionUIListScript != null)
            {
                mInscriptionUIListScript.ResetSelectedElementIndex();
                mInscriptionUIListScript.SetElementAmount(mInscriptionItemDataList.Count);
            }
        }
        #endregion

        #region BindUIEvent
        private void BindUIEventSystem()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnInscriptionHoleOpenHoleSuccess, OnInscriptionHoleOpenHoleSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RefreshInscriptionEquipmentList, OnRefreshInscriptionEquipmentList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnInscriptionMosaicSuccess, OnRefreshInscriptionEquipmentList);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRefreshEquipmentList, OnInscriptionHoleOpenHoleSuccess);
        } 

        private void UnBindUIEventSystem()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnInscriptionHoleOpenHoleSuccess, OnInscriptionHoleOpenHoleSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RefreshInscriptionEquipmentList, OnRefreshInscriptionEquipmentList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnInscriptionMosaicSuccess, OnRefreshInscriptionEquipmentList);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshEquipmentList, OnInscriptionHoleOpenHoleSuccess);
        }

        private void OnInscriptionHoleOpenHoleSuccess(UIEvent uiEvent)
        {
            mEquipmentUIListScript.ResetSelectedElementIndex();
            UpdateInscriptionEquipment();
        }

        private void OnRefreshInscriptionEquipmentList(UIEvent uiEvent)
        {
            UpdateInscriptionEquipment();
        }
        #endregion

        /// <summary>
        /// 刷新默认选择的孔索引
        /// </summary>
        /// <returns></returns>
        private int RefreshDefaultSelectedHoleIndex()
        {
            if (mCurrentSelectedEquipItemData == null)
            {
                return iDefaultSelectedHoleIndex;
            }

            for (int i = 0; i < mCurrentSelectedEquipItemData.InscriptionHoles.Count; i++)
            {
                var data = mCurrentSelectedEquipItemData.InscriptionHoles[i];
                if (data == null)
                    continue;

                if (data.InscriptionId <= 0)
                {
                    iDefaultSelectedHoleIndex = data.Index;
                    break;
                }
            }

            return iDefaultSelectedHoleIndex;
        }

        private void UpdateLeftEquipmentInfo(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            mCurrentSelectedEquipItemData = itemData;

            if (mComItem == null)
            {
                mComItem = ComItemManager.CreateNew(mItemParent);
            }

            mComItem.Setup(itemData, Utility.OnItemClicked);

            if (mName != null)
            {
                mName.text = itemData.GetColorName();
            }

            //默认选择孔索引
            iDefaultSelectedHoleIndex = RefreshDefaultSelectedHoleIndex();

            if (mInscriptionHoleInfoList != null && mInscriptionHoleInfoList.Count > 0)
            {
                for (int i = 0; i < mInscriptionHoleInfoList.Count; i++)
                {
                    mInscriptionHoleInfoList[i].CustomActive(false);
                }
            }

            for (int i = 0; i < mCurrentSelectedEquipItemData.InscriptionHoles.Count; i++)
            {
                if (i < mInscriptionHoleInfoList.Count)
                {
                    mInscriptionHoleInfoList[i].CustomActive(true);
                    InscriptionHoleItem inscriptionHoleItem = mInscriptionHoleInfoList[i].GetComponent<InscriptionHoleItem>();
                    if (inscriptionHoleItem != null)
                    {
                        InscriptionHoleData inscriptionHoleData = mCurrentSelectedEquipItemData.InscriptionHoles[i];
                        inscriptionHoleItem.OnItemVisiable(inscriptionHoleData, mCurrentSelectedEquipItemData, mToggleGroup, iDefaultSelectedHoleIndex == inscriptionHoleData.Index, OnClickInscriptionItem);
                    }
                }
                else
                {
                    GameObject go = AssetLoader.GetInstance().LoadResAsGameObject(mSInscriptionHoleItemPath);
                    if (go != null)
                    {
                        Utility.AttachTo(go, mInscriptionHoleRoot);
                        InscriptionHoleItem inscriptionHoleItem = go.GetComponent<InscriptionHoleItem>();
                        if (inscriptionHoleItem != null)
                        {
                            InscriptionHoleData inscriptionHoleData = mCurrentSelectedEquipItemData.InscriptionHoles[i];
                            inscriptionHoleItem.OnItemVisiable(inscriptionHoleData, mCurrentSelectedEquipItemData, mToggleGroup, iDefaultSelectedHoleIndex == inscriptionHoleData.Index, OnClickInscriptionItem);
                        }

                        mInscriptionHoleInfoList.Add(go);
                    }
                }
            }

            UpdateIncriptionMosaicPickBtnState();
        }

        /// <summary>
        /// 更新镶嵌 摘取按钮
        /// </summary>
        private void UpdateIncriptionMosaicPickBtnState()
        {
            bool isCanMosaic = false;//是否可镶嵌
            for (int i = 0; i < mCurrentSelectedEquipItemData.InscriptionHoles.Count; i++)
            {
                if (mCurrentSelectedEquipItemData.InscriptionHoles[i].IsOpenHole == false)
                {
                    continue;
                }

                isCanMosaic = true;
            }

            mGrayMosaic.enabled = !isCanMosaic;
            mBtnMosaic.image.raycastTarget = isCanMosaic;
        }

        private void OnClickInscriptionItem(InscriptionHoleData inscriptionHoleData)
        {
            if (inscriptionHoleData == null)
                return;
            
            mInscriptionHoleIndex = inscriptionHoleData.Index;

            if (mInscriptionItemDataList != null)
                mInscriptionItemDataList.Clear();

            mInscriptionItemDataList = InscriptionMosaicDataManager.GetInstance().GetCanMosaicInscription(inscriptionHoleData.Type);
            OnSetInscriptionItemElementAmount();
        }

        private void UpdateInscriptionEquipment()
        {
            mCurrentSelectedInscriptionItem = null;
            mAllInscripionEquipment = InscriptionMosaicDataManager.GetInstance().GetInscriptionAllEquipment();
            SetElementAmount();
            TrySelectDefultItem();
        }
        
        /// <summary>
        /// 镶嵌
        /// </summary>
        private void OnInscriptionMosaicClick()
        {
            //如果是封装装备不可镶嵌铭文
            if (mCurrentSelectedEquipItemData != null && mCurrentSelectedEquipItemData.Packing == true)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Inscription_Mosaic_SealEquip_Desc"));
                return;
            }

            //未放入铭文
            if (mCurrentSelectedInscriptionItem == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Inscription_Mosaic_NoPutInInscription_Desc"));
                return;
            }

            int inscriptionId = mCurrentSelectedEquipItemData.InscriptionHoles[mInscriptionHoleIndex - 1].InscriptionId;
            //如果选中孔已经有铭文的情况
            if (inscriptionId > 0)
            {
                //被替换的铭文
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(inscriptionId);
                CommonReplaceData commonReplaceData = new CommonReplaceData();
                commonReplaceData.commonReplaceType = CommonReplaceType.CRT_INSCRIPTIONMOSAIC;
                commonReplaceData.oldItemData = itemData;
                commonReplaceData.newItemData = mCurrentSelectedInscriptionItem;
                commonReplaceData.holeIndex = mInscriptionHoleIndex;
                commonReplaceData.callBack = OnOnSceneEquipInscriptionMountReq;

                ClientSystemManager.GetInstance().OpenFrame<CommonReplaceFrame>(FrameLayer.Middle, commonReplaceData);


                return;
            }

            //如果镶嵌铭文的品质大于紫色
            if (mCurrentSelectedInscriptionItem.Quality > ItemTable.eColor.PURPLE)
            {

                string content = TR.Value("Inscription_Mosaic_HightQualityIncription_Desc");

                SystemNotifyManager.SysNotifyMsgBoxOkCancel(content, OnOnSceneEquipInscriptionMountReq);

                return;
            }
            else
            {
                if (InscriptionMosaicDataManager.InscriptionMosiacBounced == false)
                {
                    string content = TR.Value("Inscription_Mosaic_LowInscription_Desc");
                    MallNewUtility.CommonIntergralMallPopupWindow(content, OnToggleClick, OnOnSceneEquipInscriptionMountReq);

                    return;
                }

                OnOnSceneEquipInscriptionMountReq();


                return;
            }
        }

        private void OnToggleClick(bool value)
        {
            InscriptionMosaicDataManager.InscriptionMosiacBounced = value;
        }

        private void OnOnSceneEquipInscriptionMountReq()
        {
            if (mCurrentSelectedEquipItemData != null && mCurrentSelectedInscriptionItem != null)
            {
                InscriptionMosaicDataManager.GetInstance().OnSceneEquipInscriptionMountReq(mCurrentSelectedEquipItemData.GUID, (uint)mInscriptionHoleIndex, mCurrentSelectedInscriptionItem.GUID, (uint)mCurrentSelectedInscriptionItem.TableID);
            }
        }

        private void OnInscriptionMergeBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<InscriptionComposeFrame>();
        }
    }
}