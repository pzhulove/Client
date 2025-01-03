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
using System.Collections;
using DG.Tweening;

namespace GameClient
{
    // 铭文合成界面
    public class InscriptionComposeFrameView : MonoBehaviour
    {
      
        #region ui bind

        [SerializeField]Button btnCompopse = null;

        [SerializeField]ComDropDownControl qulityDrop = null;

        [SerializeField]ComDropDownControl typeDrop = null;

        [SerializeField]InscriptionComposeItem[] inscriptionComposeItems = new InscriptionComposeItem[0];

        [SerializeField]ComUIListScript inscriptionScrollView = null;

        [SerializeField] private ComUIListScript canBeGetIncriptionUIList;

        [SerializeField] private Text noItemTips;

        [SerializeField] private GameObject canBeGetRoot;

        [SerializeField] private ScrollRect inscriptionScrollRect;

        [SerializeField] private Text selectNumberDesc;

        #endregion

        private List<ItemData> inscriptionItemDataList = new List<ItemData>();
        private List<ComControlData> inscriptionQulityTabDataList = new List<ComControlData>();
        private List<ComControlData> inscriptionThirdTypeTabDataList = new List<ComControlData>();
        private List<ItemData> putInIncriptionItemDataList = new List<ItemData>();//放入的铭文
        private List<CanBeObtainedInscriptionItemData> canBeObTainedInscriptionItemList = new List<CanBeObtainedInscriptionItemData>();//合成铭文可获得的铭文数据
        private int defalutQultityId = 0;
        private int defalutThirdTypeId = 0;
        private int selectIncriptionQultity = 0;//选择的铭文品质
        int maxSynthesisNum = 0;//最大合成数量
        private ItemData currentSelectIncriptionItemData;//当前选择的铭文
        void Awake()
        {
            if (btnCompopse != null)
            {
                btnCompopse.onClick.RemoveAllListeners();
                btnCompopse.onClick.AddListener(OnBtnCompopseClick);
            }

            BindUIEvent();
            InitInscriptionScrollView();
            InitCanBeGetIncriptionUIList();

            RegisterDelegateHandler();
        }

        private void RegisterDelegateHandler()
        {
            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem += _OnRemoveItem;
        }

        private void UnRegisterDelegateHandler()
        {
            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem -= _OnRemoveItem;
        }

        void OnDestroy()
        {
            UnBindUIEvent();
            UnInitInscriptionScrollView();
            UnInitCanBeGetIncriptionUIList();

            if (inscriptionItemDataList != null)
            {
                inscriptionItemDataList.Clear();
            }

            if (inscriptionQulityTabDataList != null)
            {
                inscriptionQulityTabDataList.Clear();
            }

            if (inscriptionThirdTypeTabDataList != null)
            {
                inscriptionThirdTypeTabDataList.Clear();
            }

            if (canBeObTainedInscriptionItemList != null)
            {
                canBeObTainedInscriptionItemList.Clear();
            }

            if (putInIncriptionItemDataList != null)
            {
                putInIncriptionItemDataList.Clear();
            }

            defalutQultityId = 0;
            defalutThirdTypeId = 0;
            selectIncriptionQultity = 0;
            maxSynthesisNum = 0;
            currentSelectIncriptionItemData = null;

            UnRegisterDelegateHandler();
        }

        public void InitView()
        {
            inscriptionQulityTabDataList = InscriptionMosaicDataManager.GetInstance().GetInscriptionQualityTabDataList();
            inscriptionThirdTypeTabDataList = InscriptionMosaicDataManager.GetInstance().GetInscriptionThirdTypeTabDataList();

            InitQulityDrop();
            InitThridTypeDrop();

            RefreshSelectNumberDesc(0, inscriptionComposeItems.Length);
            RefrshInscriptionItemList(defalutQultityId, defalutThirdTypeId);
        }
        
        #region  ComUIListScript

        private void InitCanBeGetIncriptionUIList()
        {
            if (canBeGetIncriptionUIList != null)
            {
                canBeGetIncriptionUIList.Initialize();
                canBeGetIncriptionUIList.onBindItem += OnBindCanBeGetIncriptionItemDelegate;
                canBeGetIncriptionUIList.onItemVisiable += OnCanBeGetIncriptionItemVisiableDelegate;
            }
        }

        private void UnInitCanBeGetIncriptionUIList()
        {
            if (canBeGetIncriptionUIList != null)
            {
                canBeGetIncriptionUIList.onBindItem -= OnBindCanBeGetIncriptionItemDelegate;
                canBeGetIncriptionUIList.onItemVisiable -= OnCanBeGetIncriptionItemVisiableDelegate;
            }
        }

        private InscriptionSynthesisAvailableItem OnBindCanBeGetIncriptionItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<InscriptionSynthesisAvailableItem>();
        }

        private void OnCanBeGetIncriptionItemVisiableDelegate(ComUIListElementScript item)
        {
            var inscriptionSynthesisAvailableItem = item.gameObjectBindScript as InscriptionSynthesisAvailableItem;
            if (inscriptionSynthesisAvailableItem != null &&
                item.m_index >= 0 && item.m_index < canBeObTainedInscriptionItemList.Count)
            {
                inscriptionSynthesisAvailableItem.OnItemVisiable(canBeObTainedInscriptionItemList[item.m_index]);
            }
        }

        private void InitInscriptionScrollView()
        {
            if (inscriptionScrollView != null)
            {
                inscriptionScrollView.Initialize();
                inscriptionScrollView.onBindItem += OnBindItemDelegate;
                inscriptionScrollView.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        private void UnInitInscriptionScrollView()
        {
            if (inscriptionScrollView != null)
            {
                inscriptionScrollView.onBindItem -= OnBindItemDelegate;
                inscriptionScrollView.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private InscriptionSelectItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<InscriptionSelectItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var inscriptionSelectItem = item.gameObjectBindScript as InscriptionSelectItem;
            if (inscriptionSelectItem != null && item.m_index >= 0 && item.m_index < inscriptionItemDataList.Count)
            {
                inscriptionSelectItem.OnItemVisiable(inscriptionItemDataList[item.m_index], selectIncriptionQultity, IncriptionPutInList, putInIncriptionItemDataList);
            }
        }
        
        /// <summary>
        /// 铭文放入
        /// </summary>
        /// <param name="itemData"></param>
        private void IncriptionPutInList(ItemData itemData, InscriptionSelectItem inscriptionSelectItem)
        {
            if (itemData == null)
            {
                return;
            }

            currentSelectIncriptionItemData = itemData;

            //当前选择的铭文品质不为0
            if (selectIncriptionQultity != 0)
            {
                //放入的铭文品质与选中的铭文品质不一致
                if (selectIncriptionQultity != (int)itemData.Quality)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Inscription_Compose_QualityDifferent_Desc"));
                    return;
                }
            }

            selectIncriptionQultity = (int)itemData.Quality;

            maxSynthesisNum = InscriptionMosaicDataManager.GetInstance().GetMaxInscriptionSynthesisNum(selectIncriptionQultity);

            //放入的铭文数量大于等于最大合成数
            if (putInIncriptionItemDataList.Count >= maxSynthesisNum)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Inscription_Compose_Beyond_Desc", maxSynthesisNum));
                return;
            }
            
            int allCount = itemData.Count;
            int count = 0;
            for (int i = 0; i < putInIncriptionItemDataList.Count; i++)
            {
                var item = putInIncriptionItemDataList[i];
                if (item == null)
                {
                    continue;
                }

                if (item.TableID == itemData.TableID)
                {
                    count++;
                }
            }

            //如果同样的铭文已全部放进去
            if (count >= allCount)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Inscription_Compose_SameQualityBeyond_Desc"));
                return;
            }

            var data = ItemDataManager.CreateItemDataFromTable(itemData.TableID);
            putInIncriptionItemDataList.Add(data);

            RefreshSelectNumberDesc(putInIncriptionItemDataList.Count, maxSynthesisNum);

            if (inscriptionSelectItem != null)
            {
                inscriptionSelectItem.SetCheckMaskRoot(true);
            }
            RefrshInscriptionItemList(defalutQultityId, defalutThirdTypeId,false);
            UpdatePutInIncriptionItemInfo(maxSynthesisNum);
            UpdateCanBeObtainedInscriptionItemDataInfo(itemData);
        }

        private void RefreshSelectNumberDesc(int currentCount,int maxCount)
        {
            if(selectNumberDesc != null)
            {
                selectNumberDesc.text = TR.Value("Inscription_Compose_Select_Number_Desc", currentCount, maxCount);
            }
        }

        /// <summary>
        /// 更新放入的铭文道具信息
        /// </summary>
        private void UpdatePutInIncriptionItemInfo(int maxSynthesisNum)
        {
            //更新槽位
            for (int i = 0; i < inscriptionComposeItems.Length; i++)
            {
                InscriptionComposeItem inscriptionComposeItem = inscriptionComposeItems[i];
                if (inscriptionComposeItem == null)
                {
                    continue;
                }

                if (i < maxSynthesisNum)
                {
                    inscriptionComposeItem.SetUp(null);
                }
                else
                {
                    inscriptionComposeItem.SetupSlot();
                }
            }

            for (int i = 0; i < inscriptionComposeItems.Length; i++)
            {
                InscriptionComposeItem inscriptionComposeItem = inscriptionComposeItems[i];
                if (inscriptionComposeItem == null)
                {
                    continue;
                }

                if (i < putInIncriptionItemDataList.Count)
                {
                    ItemData itemData = putInIncriptionItemDataList[i];
                    if (itemData == null)
                    {
                        continue;
                    }

                    inscriptionComposeItem.SetUp(itemData);
                }
            }
        }

        /// <summary>
        /// 更新可获得铭文信息
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateCanBeObtainedInscriptionItemDataInfo(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            canBeGetRoot.CustomActive(false);

            if (putInIncriptionItemDataList.Count != 0)
            {
                var datas = InscriptionMosaicDataManager.GetInstance().GetCanBeObtainedInscriptionItemData((int)itemData.Quality, putInIncriptionItemDataList.Count);

                if (canBeObTainedInscriptionItemList != null)
                {
                    canBeObTainedInscriptionItemList.Clear();
                }

                canBeObTainedInscriptionItemList.AddRange(datas);
                if (canBeObTainedInscriptionItemList.Count > 0)
                {
                    canBeGetRoot.CustomActive(true);
                }

                canBeGetIncriptionUIList.SetElementAmount(canBeObTainedInscriptionItemList.Count);
            }

        }

        /// <summary>
        /// 刷新铭文道具列表
        /// </summary>
        /// <param name="defalutQultityId"></param>
        /// <param name="defalutThirdTypeId"></param>
        private void RefrshInscriptionItemList(int defalutQultityId,int defalutThirdTypeId,bool setScrollRect = true)
        {
            if (inscriptionItemDataList != null)
            {
                inscriptionItemDataList.Clear();
            }

            var itemIds = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Inscription);
            if (itemIds != null)
            {
                for (int i = 0; i < itemIds.Count; i++)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(itemIds[i]);
                    if (itemData == null)
                    {
                        continue;
                    }

                    if (defalutQultityId != 0)
                    {
                        if (defalutQultityId != (int)itemData.Quality)
                        {
                            continue;
                        }
                    }

                    if (defalutThirdTypeId != 0)
                    {
                        if (defalutThirdTypeId != (int)itemData.ThirdType)
                        {
                            continue;
                        }
                    }

                    inscriptionItemDataList.Add(itemData);
                }
            }

            inscriptionItemDataList.Sort(SortInscriptionItemList);

            if (inscriptionScrollView != null)
            {
                inscriptionScrollView.SetElementAmount(inscriptionItemDataList.Count);
            }

            if (inscriptionItemDataList.Count <= 0)
            {
                if (defalutQultityId > 0 || defalutThirdTypeId > 0)
                {
                    noItemTips.text = "背包中未有符合条件的铭文";
                }
                else
                {
                    noItemTips.text = "背包中未有铭文";
                }
            }

            if (setScrollRect)
            {
                if (inscriptionScrollRect != null)
                {
                    inscriptionScrollRect.verticalNormalizedPosition = 1;
                }
            }
        }

        private int SortInscriptionItemList(ItemData x,ItemData y)
        {
            if (x.Quality != y.Quality)
            {
                return y.Quality - x.Quality;
            }

            if (x.ThirdType != y.ThirdType)
            {
                return x.ThirdType - y.ThirdType;
            }

            return x.TableID - y.TableID;
        }
        #endregion

        #region qulityDrop

        private void InitQulityDrop()
        {
            if (inscriptionQulityTabDataList != null && inscriptionQulityTabDataList.Count > 0)
            {
                var inscriptionQulityTabData = inscriptionQulityTabDataList[0];
                for (int i = 0; i < inscriptionQulityTabDataList.Count; i++)
                {
                    if (defalutQultityId == inscriptionQulityTabDataList[i].Id)
                    {
                        inscriptionQulityTabData = inscriptionQulityTabDataList[i];
                        break;
                    }
                }

                if (qulityDrop != null)
                {
                    qulityDrop.InitComDropDownControl(inscriptionQulityTabData, inscriptionQulityTabDataList, OnInscriptionQulityDropDownItemClicked);
                }
            }
        }

        private void OnInscriptionQulityDropDownItemClicked(ComControlData comControlData)
        {
            if (comControlData == null)
                return;

            //品质相同，直接返回
            if (defalutQultityId == comControlData.Id)
                return;

            //赋值选中的品质
            defalutQultityId = comControlData.Id;

            //根据选中的品质进行更新 
            RefrshInscriptionItemList(defalutQultityId, defalutThirdTypeId);
        }

        #endregion

        #region typeDrop

        private void InitThridTypeDrop()
        {
            if (inscriptionThirdTypeTabDataList != null && inscriptionThirdTypeTabDataList.Count > 0)
            {
                var inscriptionThirdTypeTabData = inscriptionThirdTypeTabDataList[0];
                for (int i = 0; i < inscriptionThirdTypeTabDataList.Count; i++)
                {
                    if (defalutThirdTypeId == inscriptionThirdTypeTabDataList[i].Id)
                    {
                        inscriptionThirdTypeTabData = inscriptionThirdTypeTabDataList[i];
                        break;
                    }
                }

                if (typeDrop != null)
                {
                    typeDrop.InitComDropDownControl(inscriptionThirdTypeTabData, inscriptionThirdTypeTabDataList, OnInscriptionThirdTypeDropDownItemClicked);
                }
            }
        }

        private void OnInscriptionThirdTypeDropDownItemClicked(ComControlData comControlData)
        {
            if (comControlData == null)
                return;

            //三类型相同，直接返回
            if (defalutThirdTypeId == comControlData.Id)
                return;

            //赋值选中的三类型
            defalutThirdTypeId = comControlData.Id;

            //根据选中的三类型进行更新
            RefrshInscriptionItemList(defalutQultityId, defalutThirdTypeId);
        }

        #endregion

        #region method

        private void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCloseSynthesisIncriptionChanged, OnCloseSynthesisIncriptionChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnIncriptionSynthesisSuccess, OnIncriptionSynthesisSuccess);
        }

        private void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCloseSynthesisIncriptionChanged, OnCloseSynthesisIncriptionChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnIncriptionSynthesisSuccess, OnIncriptionSynthesisSuccess);
        }

        #endregion

        #region ui event

        private void OnCloseSynthesisIncriptionChanged(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }

            int incriptionId = (int)uiEvent.Param1;

            for (int i = 0; i < putInIncriptionItemDataList.Count; i++)
            {
                var itemData = putInIncriptionItemDataList[i];
                if (itemData == null)
                {
                    continue;
                }

                //找到要从列表删除的铭文id
                if (itemData.TableID == incriptionId)
                {
                    putInIncriptionItemDataList.Remove(itemData);
                    break;
                }
            }

            if (putInIncriptionItemDataList.Count <= 0)
            {
                selectIncriptionQultity = 0;
                maxSynthesisNum = inscriptionComposeItems.Length;
                RefrshInscriptionItemList(defalutQultityId, defalutThirdTypeId);
            }
            else
            {
                RefrshInscriptionItemList(defalutQultityId, defalutThirdTypeId,false);
            }

            RefreshSelectNumberDesc(putInIncriptionItemDataList.Count, maxSynthesisNum);
            UpdatePutInIncriptionItemInfo(maxSynthesisNum);
            UpdateCanBeObtainedInscriptionItemDataInfo(currentSelectIncriptionItemData);
        }

        private void OnIncriptionSynthesisSuccess(UIEvent uiEvent)
        {
            maxSynthesisNum = inscriptionComposeItems.Length;
            selectIncriptionQultity = 0;
            putInIncriptionItemDataList.Clear();
            RefrshInscriptionItemList(defalutQultityId, defalutThirdTypeId);
            UpdatePutInIncriptionItemInfo(maxSynthesisNum);
            UpdateCanBeObtainedInscriptionItemDataInfo(currentSelectIncriptionItemData);
        }

        private void _OnAddNewItem(List<Item> items)
        {
            RefrshInscriptionItemList(defalutQultityId, defalutThirdTypeId);
        }

        private void _OnRemoveItem(ItemData itemData)
        {
            RefrshInscriptionItemList(defalutQultityId, defalutThirdTypeId);
        }

        #endregion

        /// <summary>
        /// 合成
        /// </summary>
        private void OnBtnCompopseClick()
        {
            //当未放入铭文或放入的铭文数量不足以进行合成
            if (putInIncriptionItemDataList.Count == 0 || canBeObTainedInscriptionItemList.Count <= 0)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Inscription_Compose_IncriptionInsufficient_Desc"));
                return;
            }

            //是否有品质高于粉色的
            bool isHightQuality = false;
            int quality = 0;
            for (int i = 0; i < putInIncriptionItemDataList.Count; i++)
            {
                var itemData = putInIncriptionItemDataList[i];
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.Quality < ItemTable.eColor.PINK)
                {
                     continue;
                }

                isHightQuality = true;
                quality = (int)itemData.Quality;
                break;
            }

            if (isHightQuality)
            {
                if (InscriptionMosaicDataManager.IncriptionSynthesisHightQualityBounced == false)
                {
                    string mContent = TR.Value("Inscription_Compose_HasHigheQualityIncription_Desc", TR.Value(string.Format("Inscription_Compose_Quailty_Desc_{0}", quality)));
                    MallNewUtility.CommonIntergralMallPopupWindow(mContent, OnToggleClick, OnSceneEquipInscriptionSynthesisReq);
                    return;
                }
            }

            OnSceneEquipInscriptionSynthesisReq();
        }

        /// <summary>
        /// 请求铭文合成
        /// </summary>
        private void OnSceneEquipInscriptionSynthesisReq()
        {
            UInt32[] incriptionIds = new UInt32[putInIncriptionItemDataList.Count];
            for (int i = 0; i < putInIncriptionItemDataList.Count; i++)
            {
                var itemData = putInIncriptionItemDataList[i];
                if (itemData == null)
                {
                    continue;
                }

                incriptionIds[i] = (UInt32)itemData.TableID;
            }

            InscriptionMosaicDataManager.GetInstance().OnSceneEquipInscriptionSynthesisReq(incriptionIds);
        }

        private void OnToggleClick(bool value)
        {
            InscriptionMosaicDataManager.IncriptionSynthesisHightQualityBounced = value;
        }
    }
}
