using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using System;
using ProtoTable;
using Protocol;
using EItemType = ProtoTable.ItemTable.eType;

namespace GameClient
{
   
    public enum ItemTabType
    {
        ITT_EQUIP,//装备
        ITT_TITLE,//称号
        ITT_COUNT,
    }
    
    public class BeadMosaicView : MonoBehaviour,ISmithShopNewView
    {
        [SerializeField] private CommonTabToggleGroup mCommonTabToggleGroup;
        [SerializeField] private ItemComeLink m_ItemComLink;

        [SerializeField] private GameObject mBeadItemRoot;
        [SerializeField] private GameObject m_kEquipItem;
        [SerializeField] private GameObject mScrollView;
        [SerializeField] private GameObject mEquipmentScrollView;
        
        [SerializeField] private Button BtnAddSarah;
        [SerializeField] private Button BtnPickBead;
        [SerializeField] private Button BtnRepalceBead;
        
        [SerializeField] private StateController mBeadStateCol;
        [SerializeField] private GameObject mBeadItemGo;
        [SerializeField] private ToggleGroup mToggleGroup;

        private List<BeadItemElement> mBeadItemList = new List<BeadItemElement>();//用来选中宝珠刷新界面
        private ComSarahCardInlayItemList m_kComSarahCardInlayItemList = new ComSarahCardInlayItemList();
        private BeadEquipmentList m_kBeadComEquipmentList = new BeadEquipmentList();
        private ItemTabType mCurrentSelectItemTab = ItemTabType.ITT_EQUIP;
        private int DefaultItemTabIndex = 0;
        private int mHoleIndex = 0;//孔索引
        private ComItemNew m_kComEquipItem;
        private string mSateNoHoleBead = "NoHoleBead";//没有镶嵌宝珠
        private string mStateHasHoleBead = "HasHoleBead";//有镶嵌宝珠品级为粉色且完美置换次数为0
        private string mStateHasLowHoleBead = "HasLowHoleBead";//有镶嵌宝珠但品级为粉色且完美置换次数不为0
        /// <summary>
        /// 当前选择的装备
        /// </summary>
        private ItemData currentSelectedItemData;
        /// <summary>
        /// 当前选择的镶嵌宝珠
        /// </summary>
        private ItemData currentSelectedBeadItemData;
        /// <summary>
        /// 默认选择的宝珠孔索引
        /// </summary>
        private int iDefaultSelectedHoleIndex = 1;
        private void Awake()
        {
            RegisterEventHandler();

            if (BtnAddSarah != null)
            {
                BtnAddSarah.onClick.RemoveAllListeners();
                BtnAddSarah.onClick.AddListener(OnAddSarahClick);
            }

            if (BtnPickBead != null)
            {
                BtnPickBead.onClick.RemoveAllListeners();
                BtnPickBead.onClick.AddListener(OnBtnPickBeadClick);
            }

            if (BtnRepalceBead != null)
            {
                BtnRepalceBead.onClick.RemoveAllListeners();
                BtnRepalceBead.onClick.AddListener(OnBtnRepalceBeadClick);
            }
        }

        private void OnDestroy()
        {
            UnRegisterEventHandler();

            m_kBeadComEquipmentList.UnInitialize();
            m_kComSarahCardInlayItemList.UnInitialize();
            mBeadItemList.Clear();
            mCurrentSelectItemTab = ItemTabType.ITT_EQUIP;
            DefaultItemTabIndex = 0;
            mHoleIndex = 0;
            m_kComEquipItem = null;
            currentSelectedItemData = null;
            currentSelectedBeadItemData = null;
            iDefaultSelectedHoleIndex = 1;
        }

        public void InitView(SmithShopNewLinkData linkData)
        {
            InitInterface();
            SetDefaultItemTabIndex(linkData);
            InitSarahCardsObjects();

            if (!m_kBeadComEquipmentList.Initilized)
            {
                m_kBeadComEquipmentList.Initialize(mEquipmentScrollView, OnItemSelected, mCurrentSelectItemTab, linkData);
            }

            InitItemTabComUIList();
        }

        public void OnEnableView()
        {
            RegisterDelegateHandler();

            m_kBeadComEquipmentList.RefreshAllEquipments(mCurrentSelectItemTab);
        }

        public void OnDisableView()
        {
            UnRegisterDelegateHandler();
        }

        #region Event(Delegate)

        private void RegisterDelegateHandler()
        {
            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem += OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
        }

        private void UnRegisterDelegateHandler()
        {
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem -= OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
        }

        private void RegisterEventHandler()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAddSarahSuccess, OnSlotItemsSarahChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BeadPickSuccess, OnSlotItemsSarahChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSarakCardSelected, OnSarahCardSelected);

            RegisterDelegateHandler();
        }

        private void UnRegisterEventHandler()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAddSarahSuccess, OnSlotItemsSarahChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BeadPickSuccess, OnSlotItemsSarahChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSarakCardSelected, OnSarahCardSelected);

            UnRegisterDelegateHandler();
        }

        private void OnSlotItemsSarahChanged(UIEvent uiEvent)
        {
            m_kBeadComEquipmentList.RefreshAllEquipments(mCurrentSelectItemTab);
            OnSarahItemChanged();
            BeadItem.ms_select = null;
        }

        private void OnSarahCardSelected(UIEvent uiEvent)
        {
            ItemData itemData = uiEvent.Param1 as ItemData;
            OnSarahCardChanged(itemData);
        }

        private void OnAddNewItem(List<Item> items)
        {
            if (items == null)
            {
                return;
            }

            bool bNeedRefreshSarahCards = false;
            for (int i = 0; i < items.Count; ++i)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData == null)
                {
                    continue;
                }

                if (!bNeedRefreshSarahCards && m_kComSarahCardInlayItemList.HasObject(itemData.GUID))
                {
                    var sarahData = ComSarahCardInlayItem._TryAddSarahCard(items[i].uid);
                    if (sarahData != null)
                    {
                        bNeedRefreshSarahCards = true;
                    }
                }
            }

            if (bNeedRefreshSarahCards)
            {
                m_kComSarahCardInlayItemList.RefreshAllEquipments(currentSelectedItemData);
            }
        }

        private void OnRemoveItem(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            ComBeadEquipment combeadEquipment = m_kBeadComEquipmentList.GetEquipment(itemData.GUID);
            if (combeadEquipment != null)
            {
                m_kBeadComEquipmentList.RefreshAllEquipments(mCurrentSelectItemTab);
            }

            if (m_kComSarahCardInlayItemList.HasObject(itemData.GUID))
            {
                m_kComSarahCardInlayItemList.RefreshAllEquipments(currentSelectedItemData);
            }
        }

        private void OnUpdateItem(List<Item> items)
        {
            if (items == null)
            {
                return;
            }

            bool bNeedRefreshSarahCard = false;
            for (int i = 0; i < items.Count; ++i)
            {
                if (items[i] == null)
                {
                    continue;
                }
                ItemData itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData != null)
                {
                    ComBeadEquipment combeadEquipment = m_kBeadComEquipmentList.GetEquipment(items[i].uid);
                    if (combeadEquipment != null)
                    {
                        m_kBeadComEquipmentList.RefreshAllEquipments(mCurrentSelectItemTab);
                    }

                    if (!bNeedRefreshSarahCard && m_kComSarahCardInlayItemList.HasObject(itemData.GUID))
                    {
                        bNeedRefreshSarahCard = true;
                    }
                }
            }

            if (bNeedRefreshSarahCard)
            {
                m_kComSarahCardInlayItemList.RefreshAllEquipments(currentSelectedItemData);
            }
        }

        #endregion

        private void OnItemSelected(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            if (itemData.Type == ItemTable.eType.FUCKTITTLE)
            {
                bool isFlag = (itemData.Packing || itemData.iMaxPackTime > 0);
                if (isFlag == true)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("BeadInlay_TitleDesc"));
                    return;
                }
            }
            
            if(currentSelectedItemData != null && currentSelectedItemData.GUID != itemData.GUID)
            {
                iDefaultSelectedHoleIndex = 1;
            }

            currentSelectedItemData = itemData;
            OnSarahItemChanged();
        }

        #region Interface

        private void InitInterface()
        {
            if (m_kComEquipItem == null)
            {
                m_kComEquipItem = ComItemManager.CreateNew(m_kEquipItem);
            }
        }

        /// <summary>
        /// 设置宝珠镶嵌选中装备页签还是称号页签
        /// </summary>
        private void SetDefaultItemTabIndex(SmithShopNewLinkData linkData)
        {
            DefaultItemTabIndex = 0;

            if (linkData != null)
            {
                if (linkData.itemData != null)
                {
                    var mBeadTable = TableManager.GetInstance().GetTableItem<BeadTable>(linkData.itemData.TableID);
                    if (mBeadTable != null)
                    {
                        if (mBeadTable.Parts.Contains((int)ItemTable.eSubType.TITLE))
                        {
                            DefaultItemTabIndex = 1;
                        }
                    }

                    if (linkData.itemData.Type == EItemType.FUCKTITTLE && linkData.itemData.SubType == (int)ItemTable.eSubType.TITLE)
                    {
                        DefaultItemTabIndex = 1;
                    }
                }
            }
        }

        private void OnSarahItemChanged()
        {
            RefreshEquipItem();
            RrfreshBeadInlaidHole();
            //UpdateFunctionButtonState();
        }

        private void RefreshEquipItem()
        {
            if (currentSelectedItemData != null)
            {
                m_kComEquipItem.Setup(currentSelectedItemData, Utility.OnItemClicked);
            }
            else
            {
                m_kComEquipItem.Setup(null, null);
            }
        }

        private void OnSetToggleGroupAllowSwitchOff(bool value)
        {
            if (mToggleGroup != null)
                mToggleGroup.allowSwitchOff = value;
        }

        /// <summary>
        /// 刷新装备宝珠孔
        /// </summary>
        private void RrfreshBeadInlaidHole()
        {
            if (currentSelectedItemData == null)
            {
                return;
            }
            
            currentSelectedBeadItemData = null;

            OnSetToggleGroupAllowSwitchOff(true);

            for (int i = 0; i < mBeadItemList.Count; i++)
            {
                var item = mBeadItemList[i];
                if (item == null)
                    continue;

                item.OnReSetToggleIsOn();
                item.CustomActive(false);
            }

            OnSetToggleGroupAllowSwitchOff(false);
            //默认选择孔索引
            iDefaultSelectedHoleIndex = RefreshDefaultSelectedHoleIndex();
                
            for (int i = 0; i < currentSelectedItemData.PreciousBeadMountHole.Length; i++)
            {
                PrecBead mBeadMountHole = currentSelectedItemData.PreciousBeadMountHole[i];
                if (mBeadMountHole == null)
                {
                    continue;
                }

                if (i < mBeadItemList.Count)
                {
                    BeadItemElement beadItemElement = mBeadItemList[i];
                    if(beadItemElement != null)
                    {
                        beadItemElement.CustomActive(true);
                        beadItemElement.RefreshBeadHoleInfo(mBeadMountHole, iDefaultSelectedHoleIndex == mBeadMountHole.index);
                    }
                }
                else
                {
                    GameObject gameObject = GameObject.Instantiate(mBeadItemGo);
                    Utility.AttachTo(gameObject, mBeadItemRoot);
                    gameObject.CustomActive(true);
                    BeadItemElement beadItemElement = gameObject.GetComponent<BeadItemElement>();
                    if(beadItemElement != null)
                    {
                        beadItemElement.InitBeadItemVisiable(mBeadMountHole, OnSelectedBeadHoleClick, iDefaultSelectedHoleIndex == mBeadMountHole.index);
                        mBeadItemList.Add(beadItemElement);
                    }
                }
            }
        }

        private void OnSelectedBeadHoleClick(PrecBead precBead)
        {
            if (precBead == null)
                return;

            if (currentSelectedItemData == null)
                return;

            mHoleIndex = precBead.index;

            //当操作某个宝珠孔，检查另一个宝珠孔是否镶嵌了宝珠，如果镶嵌了选择宝珠界面把同名宝珠过滤掉，不显示在列表中
            int beadId = 0;
            for (int i = 0; i < currentSelectedItemData.PreciousBeadMountHole.Length; i++)
            {
                var data = currentSelectedItemData.PreciousBeadMountHole[i];
                if (data == null)
                {
                    continue;
                }

                //是正在操作的孔过滤
                if (data.index == mHoleIndex)
                {
                    continue;
                }

                beadId = data.preciousBeadId;
            }

            m_kComSarahCardInlayItemList.ClearSelectedItem();
            m_kComSarahCardInlayItemList.RefreshAllEquipments(currentSelectedItemData, beadId);
        }

        /// <summary>
        /// 刷新默认选择的孔索引
        /// </summary>
        /// <returns></returns>
        private int RefreshDefaultSelectedHoleIndex()
        {
            if (currentSelectedItemData == null)
            {
                return iDefaultSelectedHoleIndex;
            }
            
            for (int i = 0; i < currentSelectedItemData.PreciousBeadMountHole.Length; i++)
            {
                var preBeadData = currentSelectedItemData.PreciousBeadMountHole[i];
                if (preBeadData == null)
                    continue;

                if(preBeadData.preciousBeadId <= 0)
                {
                    iDefaultSelectedHoleIndex = preBeadData.index;
                    break;
                }
            }

            return iDefaultSelectedHoleIndex;
        }

        /// <summary>
        /// 更新镶嵌、摘取和置换按钮的状态
        /// </summary>
        private void UpdateFunctionButtonState()
        {
            if (currentSelectedItemData == null)
            {
                return;
            }

            bool bHasInlayBeadQualyPinkReplaceNumberIsZero = false;//表示有镶嵌宝珠且品级为粉色且置换次数是否为0
            bool bHasInlayBeadQualyPink = false; //表示有镶嵌宝珠且品级为粉色
            for (int i = 0; i < currentSelectedItemData.PreciousBeadMountHole.Length; i++)
            {
                PrecBead mBeadMountHole = currentSelectedItemData.PreciousBeadMountHole[i];
                if (mBeadMountHole == null)
                {
                    continue;
                }

                BeadTable mBeadTable = TableManager.GetInstance().GetTableItem<BeadTable>(mBeadMountHole.preciousBeadId);
                if (mBeadTable == null)
                {
                    continue;
                }

                var mReplaceJewelsTable = BeadCardManager.GetInstance().GetBeadReplaceJewelsTableData(mBeadTable.Color, mBeadTable.Level, mBeadTable.BeadType);
                if (mReplaceJewelsTable != null)
                {
                    bHasInlayBeadQualyPink = true;
                    if (mReplaceJewelsTable.ReplaceNum != 0)
                    {
                        bHasInlayBeadQualyPinkReplaceNumberIsZero = true;
                        break;
                    }
                }
            }

            if (bHasInlayBeadQualyPink && bHasInlayBeadQualyPinkReplaceNumberIsZero == false)
            {
                mBeadStateCol.Key = mStateHasHoleBead;
            }
            else if (bHasInlayBeadQualyPink && bHasInlayBeadQualyPinkReplaceNumberIsZero)
            {
                mBeadStateCol.Key = mStateHasLowHoleBead;
            }
            else
            {
                mBeadStateCol.Key = mSateNoHoleBead;
            }
        }
        
        #endregion

        #region TabComUIListScript

        private void InitItemTabComUIList()
        {
            mCommonTabToggleGroup.InitComTab(OnTabValueChanged, DefaultItemTabIndex);
        }
        
        private void SetItemLinkID(int linkID)
        {
            if (m_ItemComLink != null)
            {
                m_ItemComLink.iItemLinkID = linkID;
            }
        }

        private void OnTabValueChanged(CommonTabData tabData)
        {
            if (tabData == null)
                return;

            //类型一样不处理
            if (mCurrentSelectItemTab == (ItemTabType)tabData.id)
                return;

            mCurrentSelectItemTab = (ItemTabType)tabData.id;
            if (mCurrentSelectItemTab == ItemTabType.ITT_EQUIP)
            {
                SetItemLinkID(119);
            }
            else
            {
                SetItemLinkID(227);
            }

            m_kBeadComEquipmentList.RefreshAllEquipments(mCurrentSelectItemTab);
        }

        #endregion

        #region  SarahCardInlayItemList

        private void InitSarahCardsObjects()
        {
            m_kComSarahCardInlayItemList.Initialize(mScrollView, OnSarahCardChanged, currentSelectedItemData);
        }

        void OnSarahCardChanged(ItemData itemData)
        {
            currentSelectedBeadItemData = itemData;
        }

        #endregion

        #region OnBtnClick

        private void OnAddSarahClick()
        {
            if (BtnAddSarah != null)
            {
                BtnAddSarah.enabled = false;

                InvokeMethod.Invoke(this, 0.50f, () =>
                {
                    if (BtnAddSarah != null)
                    {
                        BtnAddSarah.enabled = true;
                    }
                });
            }

            OnClickFunctionAddSarah();
        }

        private void OnClickFunctionAddSarah()
        {
            if (currentSelectedBeadItemData != null && currentSelectedItemData != null)
            {
                var beadItem = TableManager.GetInstance().GetTableItem<BeadTable>((int)currentSelectedItemData.PreciousBeadMountHole[mHoleIndex - 1].preciousBeadId);
                if (beadItem != null)
                {
                    ReplaceBead();
                }
                else
                {
                    SceneMountPreciousBeadReq();
                }
            }
            else
            {
                if (currentSelectedItemData == null)
                {
                    SystemNotifyManager.SystemNotify(1141);
                }
                else
                {
                    SystemNotifyManager.SystemNotify(1140);
                }
            }
        }

        /// <summary>
        /// 选中的宝珠未镶嵌宝珠的请求规则
        /// </summary>
        private void SceneMountPreciousBeadReq()
        {
            string content = TR.Value("HideGradeBeadInalyDesc");

            ItemData mBeadItemData = ItemDataManager.GetInstance().GetItem(currentSelectedBeadItemData.GUID);
            if (mBeadItemData != null)
            {
                if (mBeadItemData.Quality == ItemTable.eColor.PINK)
                {
                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(content, SendSceneMountPreciousBeadReq);
                }
                else
                {
                    SendSceneMountPreciousBeadReq();
                }
            }

        }

        private void SendSceneMountPreciousBeadReq()
        {
            if (currentSelectedBeadItemData != null && currentSelectedItemData != null)
            {
                ulong expendGUID = currentSelectedBeadItemData.GUID;
                ulong equipmentGUID = currentSelectedItemData.GUID;
                BeadCardManager.GetInstance().SedndSceneMountPreciousBeadReq(expendGUID, equipmentGUID, (byte)mHoleIndex);
            }
        }

        /// <summary>
        /// 替换宝珠
        /// </summary>
        private void ReplaceBead()
        {
            CommonReplaceData commonReplaceData = new CommonReplaceData();
            commonReplaceData.commonReplaceType = CommonReplaceType.CRT_BEAD;
            commonReplaceData.oldItemData = currentSelectedItemData;
            commonReplaceData.newItemData = currentSelectedBeadItemData;
            commonReplaceData.holeIndex = mHoleIndex;
            commonReplaceData.callBack = SendSceneMountPreciousBeadReq;

            ClientSystemManager.GetInstance().OpenFrame<CommonReplaceFrame>(FrameLayer.Middle, commonReplaceData);
        }

        private void OnBtnPickBeadClick()
        {
            BeadPickModel mModel = null;
            for (int i = 0; i < currentSelectedItemData.PreciousBeadMountHole.Length; i++)
            {
                var mPrecBead = currentSelectedItemData.PreciousBeadMountHole[i];
                if (mPrecBead == null)
                {
                    continue;
                }

                mModel = new BeadPickModel(mPrecBead, currentSelectedItemData);
                break;
            }

            ClientSystemManager.GetInstance().OpenFrame<BeadPickFrame>(FrameLayer.Middle, mModel);
        }

        private void OnBtnRepalceBeadClick()
        {
            BeadPerfectReplacementModel mBeadPerfectReplacementModel = new BeadPerfectReplacementModel(currentSelectedItemData);
            ClientSystemManager.GetInstance().OpenFrame<BeadPerfectReplacementFrame>(FrameLayer.Middle, mBeadPerfectReplacementModel);
        }

        #endregion
    }
}