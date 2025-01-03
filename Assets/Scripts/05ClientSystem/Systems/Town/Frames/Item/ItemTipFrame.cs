using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using EItemType = ProtoTable.ItemTable.eType;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Object = UnityEngine.Object;

namespace GameClient
{
    public delegate void OnTipFuncClicked(ItemData item, object param1);

    enum ETipItemType
    {
        ItemTitle,      // 预设体，物品标题
        HTwoLabels,     // 水平排版的两个label，第一个label左对齐，第二个label右对齐
        LeftLabel,      // 左对齐的label
        RightLabel,     // 右对齐的label
        Image,          // 图片
        MagicLabel,
        ShowItems,      // 道具集合显示
        Group,
    }

    class TipItem
    {
        public ETipItemType Type;
    }

    class TipItemItemTitle : TipItem
    {
        public ItemData itemData;
        public string levelLimitDesc;

        public TipItemItemTitle(ItemData a_data, string a_levelLimitDesc)
        {
            Type = ETipItemType.ItemTitle;
            itemData = a_data;
            levelLimitDesc = a_levelLimitDesc;
        }
    }

    class TipItemTwoLabels : TipItem
    {
        public string LeftContent;
        public string RightContent;

        public TipItemTwoLabels(string leftContent, string rightContent)
        {
            Type = ETipItemType.HTwoLabels;
            LeftContent = leftContent;
            RightContent = rightContent;
        }
    }

    class TipItemLeftLabel : TipItem
    {
        public string Content;

        public TipItemLeftLabel(string content)
        {
            Type = ETipItemType.LeftLabel;
            Content = content;
        }
    }

    class TipItemRightLabel : TipItem
    {
        public string Content;

        public TipItemRightLabel(string content)
        {
            Type = ETipItemType.RightLabel;
            Content = content;
        }
    }

    class TipItemImage : TipItem
    {
        public TipItemImage()
        {
            Type = ETipItemType.Image;
        }
    }

    class TipItemShowItems : TipItem
    {
        public List<ItemData> arrItemData = new List<ItemData>();

        public TipItemShowItems(List<GiftTable> a_arrData)
        {
            Type = ETipItemType.ShowItems;

            for (int i = 0; i < a_arrData.Count; ++i)
            {
                ItemData item = ItemDataManager.CreateItemDataFromTable(a_arrData[i].ItemID);
                if (item != null)
                {
                    item.Count = a_arrData[i].ItemCount;
                    arrItemData.Add(item);
                }
            }
        }
    }

    class TipItemGroup : TipItem
    {
        public List<TipItem> itemList = new List<TipItem>();

        public TipItemGroup()
        {
            Type = ETipItemType.Group;
        }
    }

    public enum TipFuncButtonType
    {
        None,
        Normal, //正常显示的按钮
        Other,  //其他按钮
        Trigger, //点击其他按钮才显示的按钮
    }

    public class TipFuncButon
    {
        public string icon;
        public string text;
        public OnTipFuncClicked callback;
        public string name;
        public TipFuncButtonType tipFuncButtonType = TipFuncButtonType.Normal;
    }

    // 按钮会单独显示在右下角
    class TipFuncButonSpecial : TipFuncButon { }

    /// <summary>
    /// 其他按钮是下拉框
    /// </summary>
    class TipFuncButtonOther : TipFuncButon { }

    class ItemTipData
    {
        public TextAnchor textAnchor;

        public ItemData item;
        public EquipSuitObj itemSuitObj;

        public ItemData compareItem;
        public EquipSuitObj compareItemSuitObj;

        public List<TipFuncButon> funcs;
        public int nTipIndex;

        //强制关闭modelAvatar的标志,默认为false，可以展示
        public bool IsForceCloseModelAvatar;

        /// <summary>
        /// 礼包道具是否请求服务器，默认为true
        /// </summary>
        public bool giftItemIsRequestServer = true;//礼包道具是否请求服务器，默认为true，现在预约界面有个礼包是不显示道具列表的，是因为没有正式进入游戏，请求服务器是不会返回的，所以这钟情况客户端处理。
    }

    class ItemTipPetData
    {
        public TextAnchor textAnchor;
        public PetItemTipsData item;
        public PetItemTipsData compareItem;
        public int nTipIndex;
    }

    class ItemTipFrame : ClientFrame
    {
        [UIObject("Template")]
        GameObject m_objTemplateGroup;

        [UIObject("Content")]
        GameObject m_objTipRoot;

        [UIObject("Content/Tip")]
        GameObject m_objTipTemplate;

        [UIObject("Content/Tip/InfoView/ViewPort/Content/Group")]
        GameObject m_groupTemplate;

        [UIObject("Content/Tip/InfoView/ViewPort/Content/GroupTitle")]
        GameObject m_groupTitleTemplate;

        [UIObject("Content/Tip/InfoView/ViewPort/Content/Space")]
        GameObject m_spaceTemplate;

        [UIObject("Content/Tip/InfoView/ViewPort/Content/Group/HTwoLabels")]
        GameObject m_hTwoLabelsTemplate;

        [UIObject("Content/Tip/InfoView/ViewPort/Content/Group/HLabelQuality")]
        GameObject m_labelQualityTemplate;

        [UIObject("Content/Tip/InfoView/ViewPort/Content/Group/LeftLabel")]
        GameObject m_leftLabelTemplate;

        [UIObject("Content/Tip/InfoView/ViewPort/Content/Group/RightLabel")]
        GameObject m_rightLabelTemplate;

        [UIObject("Template/Items")]
        GameObject m_showItemsTemplate;

        [UIObject("Content/Tip/InfoView/ViewPort/Content/Line")]
        GameObject m_imageTemplate;

        [UIObject("Content/Func")]
        GameObject m_objFuncBtnRoot;

        [UIObject("Content/Func/Special")]
        GameObject m_objFuncSpecial;

        [UIObject("Content/Func/BtnTemp")]
        GameObject m_objFuncBtnPrefab;
        
        [UIObject("Content/Func/Other")]
        GameObject m_objFuncOtherBtnPrefab;

        [UIObject("Content/Tip/InfoView/ViewPort/Content/CommonItemInfo")]
        GameObject m_objCommonItemInfoRoot;

        [UIObject("Content/Tip/InfoView/ViewPort/Content/MosaicName")]
        GameObject m_objMosaicNameTemplate;

        [UIObject("Content/Tip/InfoView/ViewPort/Content/CommonTitle")]
        GameObject m_objCommonTitleTemplate;
        
        [UIObject("Content/Tip/InfoView/ViewPort/Content/PriceInfo")]
        GameObject m_objPriceInfoTemplate;

        #region ItemTipAvatarModel
        [UIObject("Content/TipTwoToggleRoot")]
        GameObject m_tipTwoToggleRoot;

        [UIObject("Content/AvatarModelRoot")]
        GameObject m_avatarModelRoot;
        #endregion

        GameObject compareItemContentGo;
        ItemTipData data;
        /// <summary>
        /// 空白行标识
        /// </summary>
        private const string m_stretch = "stretch";

        float[] m_heights = { 570, 750, 935 };

        bool bIsSelect = false;
        ItemTipsFuncOtherButton mItemTipsFuncOtherButton;
        List<GameObject> otherBtnFuncInfosGo = new List<GameObject>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Tip/ItemTip";
        }

        void _InitAlign(TextAnchor eTextAnchor)
        {
            RectTransform contentRect = m_objTipRoot.GetComponent<RectTransform>();
            switch (eTextAnchor)
            {
                case TextAnchor.MiddleLeft:
                    {
                        contentRect.anchorMin = new Vector2(0.03f, 0.5f);
                        contentRect.anchorMax = new Vector2(0.0f, 0.5f);
                        contentRect.pivot = new Vector2(0.0f, 0.5f);
                        break;
                    }
                case TextAnchor.MiddleCenter:
                    {
                        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
                        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
                        contentRect.pivot = new Vector2(0.5f, 0.5f);
                        break;
                    }
                case TextAnchor.MiddleRight:
                    {
                        contentRect.anchorMin = new Vector2(0.97f, 0.5f);
                        contentRect.anchorMax = new Vector2(0.97f, 0.5f);
                        contentRect.pivot = new Vector2(1.0f, 0.5f);
                        break;
                    }
            }
        }

        void _InitOthers()
        {
            // content templete
            m_objTipTemplate.transform.SetParent(m_objTemplateGroup.transform, false);
            m_hTwoLabelsTemplate.transform.SetParent(m_objTemplateGroup.transform, false);
            m_labelQualityTemplate.transform.SetParent(m_objTemplateGroup.transform, false);
            m_leftLabelTemplate.transform.SetParent(m_objTemplateGroup.transform, false);
            m_rightLabelTemplate.transform.SetParent(m_objTemplateGroup.transform, false);
            //m_showItemsTemplate.transform.SetParent(m_objTemplateGroup.transform, true);
            m_groupTemplate.transform.SetParent(m_objTemplateGroup.transform, false);
            m_imageTemplate.transform.SetParent(m_objTemplateGroup.transform, false);
            m_objCommonItemInfoRoot.transform.SetParent(m_objTemplateGroup.transform, false);
            m_objMosaicNameTemplate.transform.SetParent(m_objTemplateGroup.transform, false);
            m_objCommonTitleTemplate.transform.SetParent(m_objTemplateGroup.transform, false);
            m_objPriceInfoTemplate.transform.SetParent(m_objTemplateGroup.transform, false);
            m_groupTitleTemplate.transform.SetParent(m_objTemplateGroup.transform, false);
            m_spaceTemplate.transform.SetParent(m_objTemplateGroup.transform, false);
            m_objTemplateGroup.SetActive(false);

            // funcbtn
            m_objFuncBtnPrefab.SetActive(false);
            m_objFuncSpecial.SetActive(false);
            m_objFuncBtnRoot.SetActive(false);
            m_objCommonItemInfoRoot.SetActive(false);
            m_objMosaicNameTemplate.CustomActive(false);
            m_objCommonTitleTemplate.CustomActive(false);
            m_objPriceInfoTemplate.CustomActive(false);
            m_groupTitleTemplate.CustomActive(false);
            m_spaceTemplate.CustomActive(false);
            m_labelQualityTemplate.CustomActive(false);
        }

        void _InitItemTipsData(ItemTipData tipData)
        {
            if(null == tipData)
            {
                return;
            }
            data = tipData;
            _InitAlign(tipData.textAnchor);
            _SetupTip(m_objTipTemplate, tipData.item, tipData.compareItem, tipData.itemSuitObj);
            if (tipData.compareItem != null)
            {
                compareItemContentGo = Object.Instantiate(m_objTipTemplate) as GameObject;

                if(tipData.item.SubType==(int)ItemTable.eSubType.WEAPON)
                    SetSwitchBtn(compareItemContentGo);

                _SetupTip(compareItemContentGo, tipData.compareItem, null, tipData.compareItemSuitObj);
            }
            _SetupFunc(tipData.item, tipData.funcs, tipData.nTipIndex);

            Button btnClose = frame.GetComponent<Button>();
            btnClose.onClick.RemoveAllListeners();
            btnClose.onClick.AddListener(() =>
            {
                ItemTipManager.GetInstance().CloseTip(tipData.nTipIndex);
            });

            RegisterItemTipEvents();

            InitItemTipModelAvatarContent(tipData);
        }

        #region ItemTipEvents

        private void RegisterItemTipEvents()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemNotifyRemoved, _OnItemRemoved);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemUseSuccess, _OnItemUseSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CounterChanged, OnCounterChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoreAndMoreBtnHandle, _OnMoreAndMoreClick);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemSellSuccess, _ItemSellSuccess);

            //接受打开和关闭的消息
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveItemTipFrameOpenMessage,
                OnReceiveItemTipFrameOpenMessage);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveItemTipFrameCloseMessage,
                OnReceiveItemTipFrameCloseMessage);
        }

        private void UnRegisterItemTipEvents()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemNotifyRemoved, _OnItemRemoved);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemUseSuccess, _OnItemUseSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CounterChanged, OnCounterChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoreAndMoreBtnHandle, _OnMoreAndMoreClick);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemSellSuccess, _ItemSellSuccess);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveItemTipFrameOpenMessage,
                OnReceiveItemTipFrameOpenMessage);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveItemTipFrameCloseMessage,
                OnReceiveItemTipFrameCloseMessage);
        }

        #endregion

        void _InitPetTipsData(ItemTipPetData petData)
        {
            if(null == petData)
            {
                return;
            }
            _InitAlign(petData.textAnchor);
            if (null != petData.compareItem)
            {
                _SetupPetTip(m_objTipRoot, petData.compareItem, 9528);
            }
            if (null != petData.item)
            {
                _SetupPetTip(m_objTipRoot, petData.item, 9527);
            }

            Button btnClose = frame.GetComponent<Button>();
            btnClose.onClick.RemoveAllListeners();
            btnClose.onClick.AddListener(() =>
            {
                ItemTipManager.GetInstance().CloseTip(petData.nTipIndex);
            });
        }

        void SetSwitchBtn(GameObject instanceObj)
        {
            if (instanceObj != null&&!data.item.isInSidePack)
            {

                GameObject mainWeapon = Utility.FindChild(instanceObj, "mainWeapnBtn");
                GameObject sideWeapon = Utility.FindChild(instanceObj, "sideWeaponBtn");

                ulong id = ItemDataManager.GetInstance().GetMainWeapon();
                if (id > 0)
                {
                    ItemData data = ItemDataManager.GetInstance().GetItem(id);
                    if (data != null)
                    {
                        EquipSuitObj suitObj = EquipSuitDataManager.GetInstance().GetSelfEquipSuitObj(data.SuitID);
                        mainWeapon.CustomActive(true);
                        mainWeapon.GetComponent<Button>().onClick.AddListener(()=> 
                        {
                            SetBtnState(mainWeapon, sideWeapon, true);
                            _SetupTip(m_objTipTemplate, this.data.item,data, suitObj);
                            _SetupTip(compareItemContentGo, data, null, suitObj);

                            GameStatisticManager.GetInstance().DoStartUIButton("TipsMainWeapon");
                        });
                    }
                }


                Dictionary<byte, ulong> sideWeaponDic = SwitchWeaponDataManager.GetInstance().GetSideWeaponDic();
                if (sideWeaponDic.Count > 0)
                {
                    ItemData data = ItemDataManager.GetInstance().GetItem(sideWeaponDic[0]);
                    if (data != null)
                    {
                        EquipSuitObj suitObj = EquipSuitDataManager.GetInstance().GetSelfEquipSuitObj(data.SuitID);
                        sideWeapon.CustomActive(true);
                        sideWeapon.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            SetBtnState(mainWeapon, sideWeapon, false);
                            _SetupTip(m_objTipTemplate, this.data.item,data, suitObj);
                            _SetupTip(compareItemContentGo, data, null, suitObj);

                            GameStatisticManager.GetInstance().DoStartUIButton("TipsSideWeapon");
                        });
                    }
                }           
            }
        }

        string selectPath = "UI/Image/Packed/p_UI_Package.png:UI_Package_Xuanzhong_Di";
        string unselectPath = "UI/Image/Packed/p_UI_Package.png:UI_Package_Weixuanzhong_Di";
        private void SetBtnState(GameObject mainBtn,GameObject sideBtn,bool isMainBtn=true)
        {
            Image mainImage = mainBtn.GetComponent<Image>();
            Image sideImage = sideBtn.GetComponent<Image>();
            ETCImageLoader.LoadSprite(ref mainImage,isMainBtn? selectPath:unselectPath);
            ETCImageLoader.LoadSprite(ref sideImage, isMainBtn? unselectPath:selectPath);

            Text mainText = mainBtn.GetComponentInChildren<Text>();
            Text sideText = sideBtn.GetComponentInChildren<Text>();
            mainText.color = isMainBtn ? new Color(1, 1, 1, 1) : new Color(68.0f/255,79.0f/255,104.0f/255,1);
            sideText.color = isMainBtn ? new Color(68.0f / 255, 79.0f / 255, 104.0f / 255, 1) : new Color(1,1,1,1);
        }

        void _SetupPetTip(GameObject root,PetItemTipsData tipsData,int iID)
        {
            var clientFrame = _GetChildFrame(iID);
            if(null == clientFrame)
            {
                clientFrame = ClientSystemManager.GetInstance().OpenFrame<PetItemTipsFrame>(root, tipsData,string.Format("PetItemTipsFrame_{0}",iID));
                _AddChildFrame(iID, clientFrame);
            }
        }

        void _UnInitPetTipsData()
        {

        }

        protected override void _OnOpenFrame()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GetGiftData, OnGetGiftData);

            InitItemTipModelAvatarData();

            _InitOthers();
            _InitItemTipsData(userData as ItemTipData);
            _InitPetTipsData(userData as ItemTipPetData);
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GetGiftData, OnGetGiftData);
            mVirtualPackContent.Clear();
            mExpendablePackContent.Clear();
            UnRegisterItemTipEvents();
            otherBtnFuncInfosGo = null;
            mItemTipsFuncOtherButton = null;
            bIsSelect = false;

            ResetItemTipModelAvatarData();
        }

        void _SetupTip(GameObject a_objTip, ItemData a_item, ItemData a_compareItem, EquipSuitObj a_suitObj)
        {
	        if (a_objTip == null)
	        {
		        return;
	        }
            a_objTip.transform.SetParent(m_objTipRoot.transform, false);
            a_objTip.transform.SetAsFirstSibling();

            float height = m_heights[1];
            switch (a_item.Type)
            {
                case EItemType.EQUIP:
                case EItemType.FASHION:
                case EItemType.FUCKTITTLE:
                    height = m_heights[2];
                    break;          
                case EItemType.EXPENDABLE:
                case EItemType.PET:
                case EItemType.HEAD_FRAME:
                case EItemType.ITEM_NEWTITLE:
                    {
                        if (a_item.PackID > 0)
                        {
                            height = m_heights[2];
                        }
                        else
                        {
                            height = m_heights[1];
                        }
                    }
                    break;
                case EItemType.VirtualPack:
                    {
                        height = m_heights[1];
                        break;
                    }
                case EItemType.TASK:
                case EItemType.MATERIAL:
                case EItemType.ENERGY:
                case EItemType.INCOME:
                    height = m_heights[1];
                    break;
            }
            a_objTip.GetComponent<LayoutElement>().minHeight = height;

            _SetupContent(a_objTip, a_item, a_compareItem, a_suitObj);
        }

        #region set element funcs
        void _SetupItemTitle(GameObject a_objRoot, ItemData a_itemData, bool isShowItemParent = true)
        {
            if (null == a_objRoot)
            {
                return ;
            }

            if (isShowItemParent)
            {
                GameObject itemParent = Utility.FindGameObject(a_objRoot, "BaseInfoView/ViewPort/Content/itemParent");
                
                if (null != itemParent)
                {
                    ComItemNew comItem = CreateComItemNew(itemParent);
                    comItem.CustomActive(true);
                    comItem.Setup(a_itemData, null);
                    if (a_itemData != null && a_itemData.IsShowTreasureFlagInTipFrame == true)
                    {
                        comItem.SetShowTreasure(true);
                    }
                }
            }

            Image bgImage = Utility.FindComponent<Image>(a_objRoot, "Tittle");
            if (null != bgImage /*&& !string.IsNullOrEmpty(a_itemData.GetQualityTipTitleBackGround())*/)
            {
                bgImage.color = a_itemData.GetQualityInfo().Col;
                //ETCImageLoader.LoadSprite(ref bgImage, a_itemData.GetQualityTipTitleBackGround());
            }

            Utility.GetComponetInChild<Text>(a_objRoot, "Tittle/name").text = a_itemData.GetColorName();
            Utility.GetComponetInChild<Text>(a_objRoot, "Tittle/LevelLimit").text = a_itemData.GetLevelLimitDesc();
            Utility.FindGameObject(a_objRoot, "Tittle/mark").SetActive(
                a_itemData.PackageType == EPackageType.WearEquip ||
                a_itemData.PackageType == EPackageType.WearFashion
            );

            Utility.GetComponetInChild<Text>(a_objRoot, "Tittle/rateScore").text = a_itemData.GetRateScoreDesc();
        }

        void _SetupHLabelQuality(GameObject a_objRoot, string a_strLeft,ItemData a_item)
        {
            GameObject obj = GameObject.Instantiate(m_labelQualityTemplate);
            if(obj != null)
            {
                ComCommonBind mBind = obj.GetComponent<ComCommonBind>();
                if(mBind != null)
                {
                    Text label = mBind.GetCom<Text>("LeftLabel");
                    Image icon = mBind.GetCom<Image>("Icon");
                    Text grade = mBind.GetCom<Text>("Grade");
                    
                    if(label != null)
                    {
                        label.text = a_strLeft;
                    }

                    if(icon != null)
                    {
                       icon.color = a_item.GetEquipGradeColor();
                    }

                    if (grade != null)
                    {
                        grade.text = a_item.GetEquipGradeDesc();
                    }
                }

                obj.transform.SetParent(a_objRoot.transform, false);
                obj.SetActive(true);
            }
        }

        void _SetupHTwoLabels(GameObject a_objRoot, string a_strLeft, string a_strRight,bool bIsReset = false)
        {
            GameObject obj = GameObject.Instantiate(m_hTwoLabelsTemplate);
            if (bIsReset)
            {
                HorizontalLayoutGroup horizontalLayoutGroup = obj.GetComponent<HorizontalLayoutGroup>();
                if(horizontalLayoutGroup != null)
                {
                    horizontalLayoutGroup.padding.left = 0;
                }
            }
            
            {
                Text leftLabel = Utility.FindGameObject(obj, "LeftLabel").GetComponent<Text>();
                leftLabel.text = a_strLeft;
            }
            {
                Text rightLabel = Utility.FindGameObject(obj, "RightLabel").GetComponent<Text>();
                rightLabel.text = a_strRight;
            }
            obj.transform.SetParent(a_objRoot.transform, false);
            obj.SetActive(true);
        }

        void _SetupLeftLabel(GameObject a_objRoot, string a_strLeft)
        {
            GameObject obj = GameObject.Instantiate(m_leftLabelTemplate);
            {
                Text label = obj.GetComponent<Text>();
                label.text = a_strLeft;
            }
            obj.transform.SetParent(a_objRoot.transform, false);
            obj.SetActive(true);
        }

        void _SetupRightLabel(GameObject a_objRoot, string a_strRight)
        {
            GameObject obj = GameObject.Instantiate(m_rightLabelTemplate);
            {
                Text label = obj.GetComponent<Text>();
                label.text = a_strRight;
            }
            obj.transform.SetParent(a_objRoot.transform, false);
            obj.SetActive(true);
        }

        void _realSetupLine(GameObject a_objRoot)
        {
            GameObject obj = GameObject.Instantiate(m_imageTemplate);
            obj.transform.SetParent(a_objRoot.transform, false);
            obj.SetActive(true);
        }

        void _SetupPriceInfo(GameObject a_objRoot, ItemData a_item)
        {
            GameObject obj = GameObject.Instantiate(m_objPriceInfoTemplate);
            if (obj != null)
            {
                ComCommonBind mBind = obj.GetComponent<ComCommonBind>();
                if (mBind != null)
                {
                    Image icon = mBind.GetCom<Image>("Icon");
                    Text label = mBind.GetCom<Text>("Price");
                   
                    var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(a_item.PriceItemID);
                    if(itemTable != null)
                    {
                        if(icon != null)
                        {
                            ETCImageLoader.LoadSprite(ref icon, itemTable.Icon);
                        }

                        if (label != null)
                        {
                            label.text = string.Format("{0}{1}", a_item.Price, itemTable.Name);
                        }
                    }
                }

                obj.transform.SetParent(a_objRoot.transform, false);
                obj.SetActive(true);
            }
        }
        
        void _SetupCommonTitle(GameObject a_objRoot, string a_strRight)
        {
            GameObject obj = GameObject.Instantiate(m_objCommonTitleTemplate);
            {
                ComCommonBind mBind = obj.GetComponent<ComCommonBind>();
                if(mBind != null)
                {
                    Text label = mBind.GetCom<Text>("Name");
                    if(label != null)
                    {
                       label.text = a_strRight;
                    }
                }
            }
            obj.transform.SetParent(a_objRoot.transform, false);
            obj.SetActive(true);
        }

        void _SetupMosaicName(GameObject a_objRoot, string a_strRight)
        {
            GameObject obj = GameObject.Instantiate(m_objMosaicNameTemplate);
            {
                ComCommonBind mBind = obj.GetComponent<ComCommonBind>();
                if (mBind != null)
                {
                    Text label = mBind.GetCom<Text>("Name");
                    if (label != null)
                    {
                        label.text = a_strRight;
                    }
                }
            }
            obj.transform.SetParent(a_objRoot.transform, false);
            obj.SetActive(true);
        }

        void _SetupLine(GameObject a_objRoot)
        {
            //GameObject obj = GameObject.Instantiate(m_imageTemplate);
            //obj.transform.SetParent(a_objRoot.transform, false);
            //obj.SetActive(true);
        }

        void _SetupShowItems(GameObject a_objRoot, List<GiftPackItemData> a_arrGiftData,int packID = 0)
        {
            _realSetupLine(a_objRoot);
            GameObject obj = GameObject.Instantiate(m_showItemsTemplate);
            GameObject parent = _SetupGroup(a_objRoot);
            parent.GetComponent<VerticalLayoutGroup>().padding.left = 10;
            obj.transform.SetParent(parent.transform, false);
            obj.SetActive(true);

            List<ItemData> arrItemData = new List<ItemData>();

            if (data.giftItemIsRequestServer)
            {
                for (int i = 0; i < a_arrGiftData.Count; ++i)
                {
                    if (a_arrGiftData[i].Levels.Count > 0)
                    {
                        if (PlayerBaseData.GetInstance().Level < a_arrGiftData[i].Levels[0] || PlayerBaseData.GetInstance().Level > a_arrGiftData[i].Levels[1])
                        {
                            continue;
                        }
                    }

                    ItemData item = ItemDataManager.CreateItemDataFromTable(a_arrGiftData[i].ItemID,100, a_arrGiftData[i].Strengthen);
                    if (item != null)
                    {
                        item.Count = a_arrGiftData[i].ItemCount;
                        item.EquipType = (EEquipType)a_arrGiftData[i].EquipType;
                        item.IsTimeLimit = a_arrGiftData[i].IsTimeLimit;
                        arrItemData.Add(item);
                    }
                }
            }
            else
            {
                arrItemData = ItemDataUtility.GetGiftItemDataList(packID, PlayerBaseData.GetInstance().JobTableID);
            }
            
            {
                ComUIListScript comList = obj.GetComponent<ComUIListScript>();
                comList.Initialize();

                comList.onBindItem = var =>
                {
                    return CreateComItem(Utility.FindGameObject(var, "Item"));
                };

                comList.onItemVisiable = var =>
                {
                    if (arrItemData != null)
                    {
                        if (var.m_index >= 0 && var.m_index < arrItemData.Count)
                        {
                            ComItem comItem = var.gameObjectBindScript as ComItem;
                            comItem.Setup(arrItemData[var.m_index], (var1, var2) =>
                            {
                                if (data.giftItemIsRequestServer)
                                    ItemTipManager.GetInstance().ShowTip(var2);
                                else
                                    ItemTipManager.GetInstance().ShowTip(var2, null, TextAnchor.MiddleCenter, true, false, false);
                            });
                            //是否显示珍品
                            if (arrItemData[var.m_index] != null &&
                                arrItemData[var.m_index].IsShowTreasureFlagInTipFrame == true)
                            {
                                comItem.SetShowTreasure(true);
                            }

                            Utility.GetComponetInChild<Text>(var.gameObject, "Name").text = arrItemData[var.m_index].GetColorName();
                        }
                    }
                };

                comList.SetElementAmount(arrItemData.Count);
            }
        }

        GameObject _SetupmSpace(GameObject a_objRoot)
        {
            GameObject obj = GameObject.Instantiate(m_spaceTemplate);
            obj.transform.SetParent(a_objRoot.transform, false);
            obj.SetActive(true);

            return obj;
        }

        GameObject _SetupGroupTitle(GameObject a_objRoot)
        {
            GameObject obj = GameObject.Instantiate(m_groupTitleTemplate);
            obj.transform.SetParent(a_objRoot.transform, false);
            obj.SetActive(true);

            return obj;
        }

        GameObject _SetupGroup(GameObject a_objRoot)
        {
            GameObject obj = GameObject.Instantiate(m_groupTemplate);
            obj.transform.SetParent(a_objRoot.transform, false);
            obj.SetActive(true);

            return obj;
        }
        #endregion

        void _SetupContent(GameObject a_objRoot, ItemData a_item, ItemData a_compareItem, EquipSuitObj a_suitObj)
        {
            if (a_objRoot == null || a_item == null)
            {
                return;
            }

            GameObject objContent = Utility.FindGameObject(a_objRoot, "InfoView/ViewPort/Content");
            GameObject objBaseInfoContent = Utility.FindGameObject(a_objRoot, "BaseInfoView/ViewPort/Content");
            _ClearContent(objContent);
            _ClearContent(objBaseInfoContent);

            switch (a_item.Type)
            {
                case EItemType.EQUIP:
                    {
                        _SetupEquipContent(a_objRoot, a_item, a_compareItem, a_suitObj);
                        break;
                    }
                case EItemType.PET:
                case EItemType.EXPENDABLE:
                case EItemType.HEAD_FRAME:
                case EItemType.ITEM_NEWTITLE:
				case EItemType.ITEM_INSCRIPTION:
                    {
                        _SetupExpendableConetnt(a_objRoot, a_item);
                        break;
                    }
                case EItemType.MATERIAL:
                    {
                        _SetupMaterialContent(a_objRoot, a_item);
                        break;
                    }
                case EItemType.TASK:
                    {
                        _SetupTaskContent(a_objRoot, a_item);
                        break;
                    }
                case EItemType.FASHION:
                    {
                        _SetupFashionContent(a_objRoot, a_item, a_compareItem, a_suitObj);
                        break;
                    }
                case EItemType.ENERGY:
                    {
                        _SetupEnergyContent(a_objRoot, a_item);
                        break;
                    }
                case EItemType.FUCKTITTLE:
                    {
                        _SetupTitleContent(a_objRoot, a_item, a_compareItem);
                        break;
                    }
                case EItemType.INCOME:
                    {
                        _SetupInComeContent(a_objRoot, a_item);
                        break;
                    }
                case EItemType.VirtualPack:
                    {
                        _SetupVirtualPackContent(a_objRoot, a_item);
                        break;
                    }
                default: break;
            }
        }

        void _ClearContent(GameObject a_obj)
        {
            if (a_obj == null)
            {
                return;
            }

            GameObject itemParent = Utility.FindGameObject(a_obj, "itemParent");

            if(itemParent != null)
            {
                for (int i = 0; i < itemParent.transform.childCount; i++)
                {
                    GameObject.Destroy(itemParent.transform.GetChild(i).gameObject);
                }
            }

            for (int i = 0; i < a_obj.transform.childCount; i++)
            {
                if (itemParent != a_obj.transform.GetChild(i).gameObject)
                {
                    GameObject.Destroy(a_obj.transform.GetChild(i).gameObject);
                }
            }
        }

        void _SetupEquipContent(GameObject a_objRoot, ItemData item, ItemData compareItem, EquipSuitObj suitObj)
        {
            #region title
            // 标题
            _SetupItemTitle(a_objRoot, item);
            #endregion
            
            GameObject objContent = Utility.FindGameObject(a_objRoot, "InfoView/ViewPort/Content");
            GameObject objBaseInfoContent = Utility.FindGameObject(a_objRoot, "BaseInfoView/ViewPort/Content");

            #region base attr
            {
                // *** 基础属性 ***
                // 装备品级     | 品质
                // 基础物魔攻防 | 子类别
                // ##空白行##   | 三类别
                // ##空白行##   | ##空白行##
                // ##空白行##   | 绑定状态
                // ##空白行##   | 剩余封装次数
                // 剩余时间     | 价格描述
                // 职业限制属性 |
                //
                // 
                // *** 基础属性 ***
                // 基础物魔攻防
                // 装备强化

                #region 新排版
                #region Title区域
                {
                    List<string> leftDescs = new List<string>();
                    _TryAddDesc(leftDescs, item.GetEquipTypeDesc());
                    _TryAddDesc(leftDescs, item.GetLevelLimitDesc());
                    _TryAddDesc(leftDescs, item.GetOccupationLimitDesc());
                    _TryAddDesc(leftDescs, m_stretch);

                    List<string> rightDescs = new List<string>();
                    //未进城镇数据没有初始化，导致现在创角界面的预约界面礼包里面的道具评分不对，跟策划商量下，把评分隐藏掉。通过是否giftItemIsRequestServer请求服务器字段来判断，请求显示，不请求不显示
                    if (data.giftItemIsRequestServer)
                    {
                        _TryAddDesc(rightDescs, item.GetRateScoreDesc());
                    }
                    _TryAddDesc(rightDescs, item.GetBindStateDesc());
                    _TryAddDesc(rightDescs, item.GetRepackTimeDesc());
                    _TryAddDesc(rightDescs, m_stretch);

                    _TryShowDescsOnBothSide(objBaseInfoContent, leftDescs, rightDescs);
                    _SetupmSpace(objBaseInfoContent);
                }
                #endregion
                
                #region 时间显示
                {
                    List<string> leftDescs = new List<string>();
                    _TryAddDesc(leftDescs, item.GetTimeLeftDesc());
                    //dead timestamp description
                    _TryAddDesc(leftDescs, item.GetDeadTimestampDesc());
                    //添加交易冷却时间
                    _TryAddDesc(leftDescs, item.GetItemAuctionCoolTimeDesc());
                    //可交易次数
                    _TryAddDesc(leftDescs, item.GetTransactionNumberDesc());
                    //异界装备提醒的红字
                    _TryAddDesc(leftDescs, item.GetBreathEquipDesc());

                    if(leftDescs.Count > 0)
                    {
                        _realSetupLine(objContent);
                    }

                    _TryShowDescsOnLeftSide(objContent, leftDescs);
                }
                #endregion

                #region 主属性
                {
                    List<string> leftDescs = new List<string>();
                    _TryAddDescs(leftDescs, _GetBaseMainPropDescs(item.BaseProp, compareItem == null ? null : compareItem.BaseProp, item));
                    _TryAddDescs(leftDescs, item.GetFourAttrAndResistMagicDescs());
                    _TryAddDescs(leftDescs, item.GetStrengthenDescs());

                    if(leftDescs.Count > 0)
                    {
                        _SetupCommonTitle(objContent, "主属性");
                    }

                    _TryShowDescsOnLeftAndQuality(objContent, leftDescs,item);
                }
                #endregion

                #region 镶嵌效果（宝珠、附魔卡、铭文）
                {
                    if (item.CheckEquipmentIsMosaicEffect() == true)
                    {
                        _SetupCommonTitle(objContent, "镶嵌效果");
                        //TODO 创建宝珠、附魔卡、铭文的字展示
                        if(item.CheckEquipmentIsMosiacBead())
                        {
                            _SetupMosaicName(objContent, "[宝珠]");
                            InitBeadAttr(objContent, item);
                        }
                        
                        if(item.CheckEquipmentIsMosiacEnchantmentCard())
                        {
                            if (item.CheckEquipmentIsMosiacBead())
                            {
                                _realSetupLine(objContent);
                            }
                            
                            _SetupMosaicName(objContent, "[附魔]");
                            InitEnchantmentAttr(objContent, item);
                        }
                        
                        if(item.CheckEquipmentIsMosiacInscription())
                        {
                            if(item.CheckEquipmentIsMosiacBead() || item.CheckEquipmentIsMosiacEnchantmentCard())
                            {
                                _realSetupLine(objContent);
                            }
                           
                            _SetupMosaicName(objContent, "[铭文]");
                            InitInscripotionAttr(objContent, item);
                        }
                    }
                }
                #endregion

                #region 附加属性
                {
                    List<string> leftDescs = new List<string>();
                    #region random attr
                    _TryAddDescs(leftDescs, item.GetRandomAttrDescs());
                    #endregion
                    #region additional attr
                    _TryAddDescs(leftDescs, item.GetAttachAttrDescs());
                    #endregion

                    #region complex attr
                    _TryAddDescs(leftDescs, item.GetComplexAttrDescs());
                    #endregion
                    
                    if(leftDescs.Count > 0)
                    {
                        _SetupCommonTitle(objContent, "附加属性");
                    }

                    _TryShowDescsOnLeftSide(objContent, leftDescs);
                }
                #endregion

                #region 护甲精通
                {
                    #region master attr

                    if(item.GetMasterAttrDes() != "")
                    {
                        _SetupCommonTitle(objContent, item.GetMasterAttrDes());
                        _TryShowDescsOnLeftSide(objContent, item.GetMasterAttrDescs(false));
                    }
                    #endregion
                }
                #endregion

                #region suit attr
                _SetupSuitContent(objContent, item, suitObj);
                #endregion

                #region description
                {
                    List<string> leftDescs = new List<string>();
                    _TryAddDesc(leftDescs, item.GetDescription());
                    _TryAddDesc(leftDescs, item.GetSourceDescription());

                    if(leftDescs.Count > 0)
                    {
                        _realSetupLine(objContent);
                    }

                    _TryShowDescsOnLeftSide(objContent, leftDescs);
                }
                #endregion
                
                #region GetPriceDesc

                if(item.CanSell)
                {
                    _realSetupLine(objContent);
                    _SetupPriceInfo(objContent, item);
                }
                #endregion

                #endregion
            }
            
            #endregion
        }

        private List<KeyValuePair<int, GameObject>> mExpendablePackContent = new List<KeyValuePair<int, GameObject>>();

        void _SetupExpendableConetnt(GameObject a_objRoot, ItemData a_item)
        {
            GameObject objContent = Utility.FindGameObject(a_objRoot, "InfoView/ViewPort/Content");
            GameObject objBaseInfoContent = Utility.FindGameObject(a_objRoot, "BaseInfoView/ViewPort/Content");

            if (a_item.SubType == (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard ||
                a_item.SubType == (int)ItemTable.eSubType.Bead||
                a_item.SubType == (int)ItemTable.eSubType.ST_INSCRIPTION)
            {
#region title
                _SetupItemTitle(a_objRoot, a_item);
                #endregion

                #region 新排版

                #region Title区域
                {
                    List<string> leftDescs = new List<string>();
                    _TryAddDesc(leftDescs, a_item.GetSubTypeDesc());
                    _TryAddDesc(leftDescs, a_item.GetMaxStackCountDesc());
                    _TryAddDesc(leftDescs, a_item.GetLevelLimitDesc());
                    _TryAddDesc(leftDescs, a_item.GetOccupationLimitDesc());
                    _TryAddDesc(leftDescs, m_stretch);
                    List<string> rightDescs = new List<string>();
                    _TryAddDesc(rightDescs, " ");
                    _TryAddDesc(rightDescs, a_item.GetBindStateDesc());
                    _TryAddDesc(rightDescs, m_stretch);
                    _TryAddDesc(rightDescs, " ");
                    _TryShowDescsOnBothSide(objBaseInfoContent, leftDescs, rightDescs);
                    _SetupmSpace(objBaseInfoContent);
                }
                #endregion

                #region 时间显示
                {
                    List<string> leftDescs = new List<string>();
                    _TryAddDesc(leftDescs, a_item.GetTimeLeftDesc());
                    //dead timestamp description
                    _TryAddDesc(leftDescs, a_item.GetDeadTimestampDesc());
                    //添加交易冷却时间
                    _TryAddDesc(leftDescs, a_item.GetItemAuctionCoolTimeDesc());
                    //可交易次数
                    _TryAddDesc(leftDescs, a_item.GetTransactionNumberDesc());

                    if(leftDescs.Count > 0)
                    {
                        _realSetupLine(objContent);
                    }

                    _TryShowDescsOnLeftSide(objContent, leftDescs);
                }
                #endregion

                #region magic attr
                {
                    if(a_item.SubType == (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard)
                    {
                        _realSetupLine(objContent);
                        
                        List<string> leftDescs = new List<string>();
                        _TryAddDesc(leftDescs, a_item.GetMagicPartDesc());
                        _TryAddDesc(leftDescs, m_stretch);
                        List<string> rightDescs = new List<string>();
                        _TryAddDesc(rightDescs, a_item.GetEnchantmentCardUpgradeDesce());
                        _TryAddDesc(rightDescs, m_stretch);
                        _TryShowDescsOnBothSide(objContent, leftDescs, rightDescs, true);

                        _TryShowDescOnLeftSide(objContent, EnchantmentsCardManager.GetInstance().GetEnchantmentCardAttributesDesc(a_item.TableID, a_item.mPrecEnchantmentCard.iEnchantmentCardLevel, true));
                    }
                }
                #endregion

                #region bead attr
                {
                    if (a_item.SubType == (int)ProtoTable.ItemTable.eSubType.Bead)
                    {
                        _realSetupLine(objContent);

                        List<string> leftDescs = new List<string>();
                        _TryAddDesc(leftDescs, a_item.GetBeadPartDesc());
                        _TryAddDesc(leftDescs, BeadCardManager.GetInstance().GetAttributesDesc(a_item.TableID));
                        _TryShowDescsOnLeftSide(objContent, leftDescs);
                        //_TryShowDescOnLeftSide(objContent, a_item.GetBeadNextLevelArrtDescs());
                        //_TryShowDescOnLeftSide(objContent, a_item.GetBeadAppendArrtDesce());
                    }
                }
                #endregion

                #region InscriptionAttr
                {
                    if (a_item.SubType == (int)ItemTable.eSubType.ST_INSCRIPTION)
                    {
                        _realSetupLine(objContent);
                        List<string> leftDescs = new List<string>();
                        _TryAddDesc(leftDescs, a_item.InscriptionMosaicSlot());
                        _TryAddDesc(leftDescs, a_item.GetInscriptionApplicableToProfessionalDescription());
                        string m_InscriptionAttr = InscriptionMosaicDataManager.GetInstance().GetInscriptionAttributesDesc(a_item.TableID);
                        m_InscriptionAttr += TR.Value("tip_inscription_attrdesc");
                        _TryAddDesc(leftDescs, m_InscriptionAttr);
                        _TryShowDescsOnLeftSide(objContent, leftDescs);
                    }
                }
                #endregion



                #region Interesting description
                _TryShowDescOnLeftSide(objContent, a_item.GetDescription());
                #endregion

                #region source description
                _TryShowDescOnLeftSide(objContent, a_item.GetSourceDescription());
                #endregion

                _TryShowDescOnLeftSide(objContent, BeadCardManager.GetInstance().GetBeadPickRemainNumber(a_item.TableID, a_item.BeadPickNumber));
                _TryShowDescOnLeftSide(objContent, BeadCardManager.GetInstance().GetBeadReplaceRemainNumber(a_item.TableID, a_item.BeadReplaceNumber));

                if (a_item.CanSell)
                {
                    _realSetupLine(objContent);
                    _SetupPriceInfo(objContent, a_item);
                }

                #endregion
            }
            else
            {
#region title
                _SetupItemTitle(a_objRoot, a_item);
                #endregion

                #region 新排版

                #region Title区域
                {
                    List<string> leftDescs = new List<string>();

                    if (a_item.SubType == (int)ItemTable.eSubType.PetEgg)
                    {
                        _TryAddDesc(leftDescs, a_item.GetItemTypeDesc());
                        _TryAddDesc(leftDescs, string.Format("{0}-{1}", a_item.GetSubTypeDesc(), a_item.GetThirdTypeDesc()));
                        _TryAddDesc(leftDescs, a_item.GetLevelLimitDesc());
                    }
                    else
                    {
                        _TryAddDesc(leftDescs, a_item.GetItemTypeDesc());
                        _TryAddDesc(leftDescs, a_item.GetMaxStackCountDesc());
                        if (a_item.SubType == (int)ItemTable.eSubType.ExperiencePill ||
                            a_item.SubType == (int)ItemTable.eSubType.ST_FLYUPITEM ||
                            a_item.ThirdType == ItemTable.eThirdType.LevelShow ||
                            a_item.SubType == (int)ItemTable.eSubType.ST_UP_LEVEL_BOOK)
                        {
                            _TryAddDesc(leftDescs, a_item.GetExperiencePillLevelLimitDesc());
                        }
                        else
                        {
                            _TryAddDesc(leftDescs, a_item.GetLevelLimitDesc());
                        }
                        _TryAddDesc(leftDescs, a_item.GetOccupationLimitDesc());
                    }
             
                    _TryAddDesc(leftDescs, m_stretch);
                    List<string> rightDescs = new List<string>();
                    _TryAddDesc(rightDescs, " ");
                    _TryAddDesc(rightDescs, a_item.GetBindStateDesc());
                    _TryAddDesc(rightDescs, m_stretch);
                    _TryAddDesc(rightDescs, " ");
                    _TryShowDescsOnBothSide(objBaseInfoContent, leftDescs, rightDescs);
                    _SetupmSpace(objBaseInfoContent);
                }
                #endregion

                #region 时间显示
                {
                    List<string> leftDescs = new List<string>();
                    _TryAddDesc(leftDescs, a_item.GetTimeLeftDesc());
                    //dead timestamp description
                    _TryAddDesc(leftDescs, a_item.GetDeadTimestampDesc());
                    //添加交易冷却时间
                    _TryAddDesc(leftDescs, a_item.GetItemAuctionCoolTimeDesc());
                    //可交易次数
                    _TryAddDesc(leftDescs, a_item.GetTransactionNumberDesc());

                    if (leftDescs.Count > 0)
                    {
                        _realSetupLine(objContent);
                    }

                    _TryShowDescsOnLeftSide(objContent, leftDescs);
                }
                #endregion

                {
                    List<string> leftDescs = new List<string>();
                    //每日使用次数
                    _TryAddDesc(leftDescs, a_item.GetUseTimeDesc());
                    //剩余使用次数
                    _TryAddDesc(leftDescs, a_item.GetRemainUseNumberDesc());

                    if (leftDescs.Count > 0)
                    {
                        _realSetupLine(objContent);
                    }

                    _TryShowDescsOnLeftSide(objContent, leftDescs);
                }
                
                if (a_item.SubType == (int)ItemTable.eSubType.PetEgg)
                {
                    var iter = TableManager.GetInstance().GetTable<PetTable>().GetEnumerator();
                    while (iter.MoveNext())
                    {
                        PetTable petTable = iter.Current.Value as PetTable;
                        if (petTable != null)
                        {
                            if (petTable.PetEggID == a_item.TableID)
                            {
                                _SetupCommonTitle(objContent, "对主人属性加成");
                                _TryShowDescOnLeftSide(objContent, PetDataManager.GetInstance().GetPetPropertyTips(petTable, petTable.MaxLv));
                                _SetupCommonTitle(objContent, "属性选择");
                                _TryShowDescOnLeftSide(objContent, PetDataManager.GetInstance().GetPetCurSkillTips(petTable, PlayerBaseData.GetInstance().JobTableID, 0, petTable.MaxLv));
                                _TryShowDescOnLeftSide(objContent, PetDataManager.GetInstance().GetCanSelectSkillTips(petTable, PlayerBaseData.GetInstance().JobTableID, 0, petTable.MaxLv));
                                break;
                            }
                        }
                    }
                }

                {
                    List<string> leftDescs = new List<string>();

                    #region complex attr
                    _TryAddDescs(leftDescs, a_item.GetComplexAttrDescs());
                    #endregion
                    #region Interesting description
                    _TryAddDesc(leftDescs, a_item.GetDescription());
                    #endregion
                    #region source description
                    _TryAddDesc(leftDescs, a_item.GetSourceDescription());
                    #endregion

                    if (leftDescs.Count > 0)
                    {
                        _realSetupLine(objContent);
                    }

                    _TryShowDescsOnLeftSide(objContent, leftDescs);
                }

                #region show items
                if (a_item.PackID > 0)
                {
                    if (data.giftItemIsRequestServer)
                    {
                        this.mExpendablePackContent.Add(new KeyValuePair<int, GameObject>(a_item.PackID, objContent));
                        GiftPackDataManager.GetInstance().GetGiftPackItem(a_item.PackID);
                    }
                    else
                    {
                        _SetupShowItems(objContent, new List<GiftPackItemData>(), a_item.PackID);
                    }
                }
                #endregion

                if (a_item.CanSell)
                {
                    _realSetupLine(objContent);
                    _SetupPriceInfo(objContent, a_item);
                }
                #endregion
            }
        }

        void _SetupMaterialContent(GameObject a_objRoot, ItemData a_item)
        {
            GameObject objContent = Utility.FindGameObject(a_objRoot, "InfoView/ViewPort/Content");
            GameObject objBaseInfoContent = Utility.FindGameObject(a_objRoot, "BaseInfoView/ViewPort/Content");

            #region title
            _SetupItemTitle(a_objRoot, a_item);
            #endregion

            #region 新排版

            #region Title区域
            {
                List<string> leftDescs = new List<string>();
                _TryAddDesc(leftDescs, a_item.GetItemTypeDesc());
                _TryAddDesc(leftDescs, a_item.GetMaxStackCountDesc());
                _TryAddDesc(leftDescs, a_item.GetLevelLimitDesc());
                _TryAddDesc(leftDescs, a_item.GetOccupationLimitDesc());
                _TryAddDesc(leftDescs, m_stretch);
                List<string> rightDescs = new List<string>();
                _TryAddDesc(rightDescs, " ");
                _TryAddDesc(rightDescs, a_item.GetBindStateDesc());
                _TryAddDesc(rightDescs, m_stretch);
                _TryAddDesc(rightDescs, " ");
                _TryShowDescsOnBothSide(objBaseInfoContent, leftDescs, rightDescs);
                _SetupmSpace(objBaseInfoContent);
            }
            #endregion
            #region 时间显示
            {
                List<string> leftDescs = new List<string>();
                _TryAddDesc(leftDescs, a_item.GetTimeLeftDesc());
                //dead timestamp description
                _TryAddDesc(leftDescs, a_item.GetDeadTimestampDesc());
                //添加交易冷却时间
                _TryAddDesc(leftDescs, a_item.GetItemAuctionCoolTimeDesc());
                //可交易次数
                _TryAddDesc(leftDescs, a_item.GetTransactionNumberDesc());

                if (leftDescs.Count > 0)
                {
                    _realSetupLine(objContent);
                }

                _TryShowDescsOnLeftSide(objContent, leftDescs);
            }
            #endregion

            {
                List<string> leftDescs = new List<string>();
                if(a_item.SubType == (int)ItemTable.eSubType.ST_SINAN)
                {
                    _TryAddDescs(leftDescs, a_item.GetSinanBuffDescs());
                }
                //Interesting description
                _TryAddDesc(leftDescs, a_item.GetDescription());
                // source description
                _TryAddDesc(leftDescs, a_item.GetSourceDescription());

                if (leftDescs.Count > 0)
                {
                    _realSetupLine(objContent);
                }

                _TryShowDescsOnLeftSide(objContent, leftDescs);
            }

            if (a_item.CanSell)
            {
                _realSetupLine(objContent);
                _SetupPriceInfo(objContent, a_item);
            }
            #endregion
        }

        void _SetupTaskContent(GameObject a_objRoot, ItemData a_item)
        {
            GameObject objContent = Utility.FindGameObject(a_objRoot, "InfoView/ViewPort/Content");
            GameObject objBaseInfoContent = Utility.FindGameObject(a_objRoot, "BaseInfoView/ViewPort/Content");

            #region title
            _SetupItemTitle(a_objRoot, a_item);
            #endregion
            #region 新排版

            #region Title区域
            {
                List<string> leftDescs = new List<string>();
                _TryAddDesc(leftDescs, a_item.GetItemTypeDesc());
                _TryAddDesc(leftDescs, a_item.GetMaxStackCountDesc());
                _TryAddDesc(leftDescs, a_item.GetLevelLimitDesc());
                _TryAddDesc(leftDescs, a_item.GetOccupationLimitDesc());
                _TryAddDesc(leftDescs, m_stretch);
                List<string> rightDescs = new List<string>();
                _TryAddDesc(rightDescs, " ");
                _TryAddDesc(rightDescs, a_item.GetBindStateDesc());
                _TryAddDesc(rightDescs, m_stretch);
                _TryAddDesc(rightDescs, " ");
                _TryShowDescsOnBothSide(objBaseInfoContent, leftDescs, rightDescs);
                _SetupmSpace(objBaseInfoContent);
            }
            #endregion
            #region 时间显示
            {
                List<string> leftDescs = new List<string>();
                _TryAddDesc(leftDescs, a_item.GetTimeLeftDesc());
                //dead timestamp description
                _TryAddDesc(leftDescs, a_item.GetDeadTimestampDesc());
                //添加交易冷却时间
                _TryAddDesc(leftDescs, a_item.GetItemAuctionCoolTimeDesc());
                //可交易次数
                _TryAddDesc(leftDescs, a_item.GetTransactionNumberDesc());

                if (leftDescs.Count > 0)
                {
                    _realSetupLine(objContent);
                }

                _TryShowDescsOnLeftSide(objContent, leftDescs);
            }
            #endregion

            {
                List<string> leftDescs = new List<string>();
                //Interesting description
                _TryAddDesc(leftDescs, a_item.GetDescription());
                // source description
                _TryAddDesc(leftDescs, a_item.GetSourceDescription());

                if (leftDescs.Count > 0)
                {
                    _realSetupLine(objContent);
                }

                _TryShowDescsOnLeftSide(objContent, leftDescs);
            }

            if (a_item.CanSell)
            {
                _realSetupLine(objContent);
                _SetupPriceInfo(objContent, a_item);
            }
            #endregion
        }

        void _SetupFashionContent(GameObject a_objRoot, ItemData a_item, ItemData a_compareItem, EquipSuitObj a_suitObj)
        {
            GameObject objContent = Utility.FindGameObject(a_objRoot, "InfoView/ViewPort/Content");
            GameObject objBaseInfoContent = Utility.FindGameObject(a_objRoot, "BaseInfoView/ViewPort/Content");

            bool bHasAttribute = a_item.HasFashionAttribute;

            #region title
            _SetupItemTitle(a_objRoot, a_item);
            #endregion

            #region 新排版
            #region Title区域
            {
                List<string> leftDescs = new List<string>();
                _TryAddDesc(leftDescs, string.Format("{0}-{1}", a_item.GetItemTypeDesc(), a_item.GetSubTypeDesc()));
                _TryAddDesc(leftDescs, " ");
                _TryAddDesc(leftDescs, a_item.GetOccupationLimitDesc());
                _TryAddDesc(leftDescs, m_stretch);

                List<string> rightDescs = new List<string>();
                //未进城镇数据没有初始化，导致现在创角界面的预约界面礼包里面的道具评分不对，跟策划商量下，把评分隐藏掉。通过是否giftItemIsRequestServer请求服务器字段来判断，请求显示，不请求不显示
                if (data.giftItemIsRequestServer)
                {
                    _TryAddDesc(rightDescs, a_item.GetRateScoreDesc());
                }
                _TryAddDesc(rightDescs, a_item.GetBindStateDesc());
                _TryAddDesc(rightDescs, " ");
                _TryAddDesc(rightDescs, m_stretch);

                _TryShowDescsOnBothSide(objBaseInfoContent, leftDescs, rightDescs);
                _SetupmSpace(objBaseInfoContent);
            }
            #endregion
            #region 时间显示
            {
                List<string> leftDescs = new List<string>();
                _TryAddDesc(leftDescs, a_item.GetTimeLeftDesc());
                //dead timestamp description
                _TryAddDesc(leftDescs, a_item.GetDeadTimestampDesc());

                if (leftDescs.Count > 0)
                {
                    _realSetupLine(objContent);
                }

                _TryShowDescsOnLeftSide(objContent, leftDescs);
            }
            #endregion
            
            if (!bHasAttribute)
            {
                #region 主属性
                {
                    List<string> leftDescs = new List<string>();
                    _TryAddDescs(leftDescs, _GetBaseMainPropDescs(a_item.BaseProp, a_compareItem == null ? null : a_compareItem.BaseProp));
                    _TryAddDescs(leftDescs, a_item.GetFourAttrDescs());
                    _TryAddDescs(leftDescs, a_item.GetStrengthenDescs());

                    if (leftDescs.Count > 0)
                    {
                        _SetupCommonTitle(objContent, "主属性");
                    }

                    _TryShowDescsOnLeftSide(objContent, leftDescs);
                }
                #endregion

                #region fashionAttrs
                _SetupFashionAttributeContent(objContent, a_item);
                #endregion

                #region 附加属性
                {
                    List<string> leftDescs = new List<string>();
                    #region additional attr
                    _TryAddDescs(leftDescs, a_item.GetAttachAttrDescs());
                    #endregion

                    #region complex attr
                    _TryAddDescs(leftDescs, a_item.GetComplexAttrDescs());
                    #endregion

                    if (leftDescs.Count > 0)
                    {
                        _SetupCommonTitle(objContent, "附加属性");
                    }

                    _TryShowDescsOnLeftSide(objContent, leftDescs);
                }
                #endregion
            }
            else
            {
                #region 主属性
                {
                    List<string> leftDescs = new List<string>();
                    // 时装光环比较特殊，既有基础属性也有可选属性
                    if (a_item.FashionWearSlotType == EFashionWearSlotType.Auras)
                    {
                        _TryAddDescs(leftDescs, _GetBaseMainPropDescs(a_item.BaseProp, a_compareItem == null ? null : a_compareItem.BaseProp));
                        _TryAddDescs(leftDescs, a_item.GetFourAttrDescs());
                    }
                      
                    _TryAddDescs(leftDescs, a_item.GetStrengthenDescs());

                    if (leftDescs.Count > 0)
                    {
                        _SetupCommonTitle(objContent, "主属性");
                    }

                    _TryShowDescsOnLeftSide(objContent, leftDescs);
                }
                #endregion

                #region fashionAttrs
                _SetupFashionAttributeContent(objContent, a_item);
                #endregion

                #region 附加属性
                { // 时装光环比较特殊，既有基础属性也有可选属性
                    if (a_item.FashionWearSlotType == EFashionWearSlotType.Auras)
                    {
                        List<string> leftDescs = new List<string>();
                        #region skill CD/MP attr
                        _TryAddDescs(leftDescs, a_item.GetSkillMPAndCDDescs());
                        #endregion
                        #region additional attr
                        _TryAddDescs(leftDescs, a_item.GetAttachAttrDescs());
                        #endregion
                        #region complex attr
                        _TryAddDescs(leftDescs, a_item.GetComplexAttrDescs());
                        #endregion
                    
                        if (leftDescs.Count > 0)
                        {
                            _SetupCommonTitle(objContent, "附加属性");
                        }

                        _TryShowDescsOnLeftSide(objContent, leftDescs);
                    }
                }
                #endregion
            }

            #region suit attr
            _SetupSuitContent(objContent, a_item, a_suitObj);
            #endregion

            {
                List<string> leftDescs = new List<string>();
                //Interesting description
                _TryAddDesc(leftDescs, a_item.GetDescription());
                // source description
                _TryAddDesc(leftDescs, a_item.GetSourceDescription());

                if (leftDescs.Count > 0)
                {
                    _realSetupLine(objContent);
                }

                _TryShowDescsOnLeftSide(objContent, leftDescs);
            }
            
            #endregion
        }

        void _SetupFashionAttributeContent(GameObject a_objRoot, ItemData a_item)
        {
            if (a_item.HasFashionAttribute || (a_item.fashionAttributes != null && a_item.fashionAttributes.Count > 0))
            {
                _SetupCommonTitle(a_objRoot, "属性选择");
            }

            #region curAttribute
            if (a_item.HasFashionAttribute)
            {
                var curFashionAttribute = TableManager.GetInstance().GetTableItem<ProtoTable.EquipAttrTable>(a_item.FashionAttributeID);
                if (curFashionAttribute != null)
                {
                    var curAttributeDesc = FashionAttributeSelectManager.GetInstance().GetAttributesDesc(a_item.FashionAttributeID, "tip_color_green_noparm");
                    if (!string.IsNullOrEmpty(curAttributeDesc))
                    {
                        GameObject objGroup = _SetupGroup(a_objRoot);
                        string temp = TR.Value("Common_Two_Format_One", TR.Value("tip_color_orange", TR.Value("fashion_cur_attribute_title")), a_item.GetFasionFreeTimesDesc()) ;
                        if (string.IsNullOrEmpty(temp) == false)
                        {
                            _SetupLeftLabel(objGroup, temp);
                            _SetupLeftLabel(objGroup, curAttributeDesc);
                        }
                    }
                }
            }
#endregion


#region attributeSet
            if (a_item.fashionAttributes != null && a_item.fashionAttributes.Count > 0)
            {
                GameObject objGroup = _SetupGroup(a_objRoot);
                string temp;

                temp = TR.Value("tip_color_gray2", TR.Value("fashion_selected_attribute_title"));
                if (string.IsNullOrEmpty(temp) == false)
                {
                    _SetupLeftLabel(objGroup, temp);
                }

                for (int i = 0; i < a_item.fashionAttributes.Count; ++i)
                {
                    var attributeDesc = FashionAttributeSelectManager.GetInstance().GetAttributesDesc(a_item.fashionAttributes[i].ID, "tip_color_gray_noparm");
                    if (string.IsNullOrEmpty(attributeDesc) == false)
                    {
                        _SetupLeftLabel(objGroup, attributeDesc);
                    }
                }
            }
#endregion
        }

        void _SetupEnergyContent(GameObject a_objRoot, ItemData a_item)
        {
            GameObject objContent = Utility.FindGameObject(a_objRoot, "InfoView/ViewPort/Content");
            GameObject objBaseInfoContent = Utility.FindGameObject(a_objRoot, "BaseInfoView/ViewPort/Content");

            #region title
            _SetupItemTitle(a_objRoot, a_item);
#endregion

#region base attr
            {
                List<string> leftDescs = new List<string>();
                _TryAddDesc(leftDescs, a_item.GetItemTypeDesc());
                _TryAddDesc(leftDescs, " ");
                _TryAddDesc(leftDescs, m_stretch);
                _TryAddDesc(leftDescs, a_item.GetBindStateDesc());
                _TryAddDesc(leftDescs, a_item.GetBindStateOwnerDesc());

                List<string> rightDescs = new List<string>();
                _TryAddDesc(rightDescs, " ");
                _TryAddDesc(rightDescs, " ");
                _TryAddDesc(rightDescs, " ");
                _TryAddDesc(rightDescs, m_stretch);

                _TryShowDescsOnBothSide(objBaseInfoContent, leftDescs, rightDescs);
                _SetupmSpace(objBaseInfoContent);
            }
            #endregion

            {
                List<string> leftDescs = new List<string>();
                //Interesting description
                _TryAddDesc(leftDescs, a_item.GetDescription());
                // source description
                _TryAddDesc(leftDescs, a_item.GetSourceDescription());

                if (leftDescs.Count > 0)
                {
                    _realSetupLine(objContent);
                }

                _TryShowDescsOnLeftSide(objContent, leftDescs);
            }
        }

        void _SetupTitleContent(GameObject a_objRoot, ItemData a_item, ItemData a_compareItem)
        {
            GameObject objContent = Utility.FindGameObject(a_objRoot, "InfoView/ViewPort/Content");
            GameObject objBaseInfoContent = Utility.FindGameObject(a_objRoot, "BaseInfoView/ViewPort/Content");

            #region Tittle
            _SetupItemTitle(a_objRoot, a_item);
            #endregion

            #region 新排版

            #region title区域
            {
                List<string> leftDescs = new List<string>();
                _TryAddDesc(leftDescs, a_item.GetItemTypeDesc());
                _TryAddDesc(leftDescs, a_item.GetOccupationLimitDesc());
                _TryAddDesc(leftDescs, m_stretch);
                _TryAddDesc(leftDescs, " ");
                List<string> rightDescs = new List<string>();
                _TryAddDesc(rightDescs, a_item.GetRateScoreDesc());
                _TryAddDesc(rightDescs, a_item.GetBindStateDesc());
                _TryAddDesc(rightDescs, m_stretch);
                _TryAddDesc(rightDescs, " ");

                _TryShowDescsOnBothSide(objBaseInfoContent, leftDescs, rightDescs);
                _SetupmSpace(objBaseInfoContent);
            }
            #endregion

            #region 时间显示
            {
                List<string> leftDescs = new List<string>();
                _TryAddDesc(leftDescs, a_item.GetTimeLeftDesc());
                //dead timestamp description
                _TryAddDesc(leftDescs, a_item.GetDeadTimestampDesc());
                //添加交易冷却时间
                _TryAddDesc(leftDescs, a_item.GetItemAuctionCoolTimeDesc());
                //可交易次数
                _TryAddDesc(leftDescs, a_item.GetTransactionNumberDesc());

                if (leftDescs.Count > 0)
                {
                    _realSetupLine(objContent);
                }

                _TryShowDescsOnLeftSide(objContent, leftDescs);
            }
            #endregion

            #region 主属性
            {
                List<string> leftDescs = new List<string>();
                _TryAddDescs(leftDescs, _GetBaseMainPropDescs(a_item.BaseProp, a_compareItem == null ? null : a_compareItem.BaseProp));
                _TryAddDescs(leftDescs, a_item.GetFourAttrAndResistMagicDescs());
                _TryAddDescs(leftDescs, a_item.GetStrengthenDescs());

                if (leftDescs.Count > 0)
                {
                    _SetupCommonTitle(objContent, "主属性");
                }

                _TryShowDescsOnLeftSide(objContent, leftDescs);
            }
            #endregion

            #region 镶嵌效果（宝珠、附魔卡、铭文）
            {
                if (a_item.CheckEquipmentIsMosaicEffect() == true)
                {
                    _SetupCommonTitle(objContent, "镶嵌效果");
                    
                    if (a_item.CheckEquipmentIsMosiacBead())
                    {
                        _SetupMosaicName(objContent, "[宝珠]");
                        InitBeadAttr(objContent, a_item);
                    }
                }
            }
            #endregion


            #region 附加属性
            {
                List<string> leftDescs = new List<string>();
              
                #region additional attr
                _TryAddDescs(leftDescs, a_item.GetAttachAttrDescs());
                #endregion

                #region complex attr
                _TryAddDescs(leftDescs, a_item.GetComplexAttrDescs());
                #endregion

                if (leftDescs.Count > 0)
                {
                    _SetupCommonTitle(objContent, "附加属性");
                }

                _TryShowDescsOnLeftSide(objContent, leftDescs);
            }
            #endregion
            
            #region Interesting description
            _TryShowDescOnLeftSide(objContent, a_item.GetDescription());
            #endregion

            #region source description
            _TryShowDescOnLeftSide(objContent, a_item.GetSourceDescription());
            #endregion

            #endregion


            #region skill CD/MP attr
            //_TryShowDescsOnLeftSide(objContent, a_item.GetSkillMPAndCDDescs());
            #endregion


        }

        void _SetupInComeContent(GameObject a_objRoot, ItemData a_item)
        {
            GameObject objContent = Utility.FindGameObject(a_objRoot, "InfoView/ViewPort/Content");
            GameObject objBaseInfoContent = Utility.FindGameObject(a_objRoot, "BaseInfoView/ViewPort/Content");

            #region title
            _SetupItemTitle(a_objRoot, a_item);
            #endregion

            #region 新排版

            #region Title区域
            {
                List<string> leftDescs = new List<string>();
                _TryAddDesc(leftDescs, a_item.GetItemTypeDesc());
                _TryAddDesc(leftDescs, m_stretch);
                List<string> rightDescs = new List<string>();
                _TryAddDesc(rightDescs, " ");
                _TryAddDesc(rightDescs, a_item.GetBindStateDesc());
                _TryAddDesc(rightDescs, m_stretch);
                _TryAddDesc(rightDescs, " ");
                _TryShowDescsOnBothSide(objBaseInfoContent, leftDescs, rightDescs);
                _SetupmSpace(objBaseInfoContent);
            }
            #endregion
            #region 时间显示
            {
                List<string> leftDescs = new List<string>();
                _TryAddDesc(leftDescs, a_item.GetTimeLeftDesc());
                //dead timestamp description
                _TryAddDesc(leftDescs, a_item.GetDeadTimestampDesc());
                if (leftDescs.Count > 0)
                {
                    _realSetupLine(objContent);
                }

                _TryShowDescsOnLeftSide(objContent, leftDescs);
            }
            #endregion
            
            {
                List<string> leftDescs = new List<string>();
                //Interesting description
                _TryAddDesc(leftDescs, a_item.GetDescription());
                // source description
                _TryAddDesc(leftDescs, a_item.GetSourceDescription());

                if (leftDescs.Count > 0)
                {
                    _realSetupLine(objContent);
                }

                _TryShowDescsOnLeftSide(objContent, leftDescs);
            }

            #endregion
        }

        private List<KeyValuePair<int, GameObject>> mVirtualPackContent = new List<KeyValuePair<int, GameObject>>();

        void _SetupVirtualPackContent(GameObject a_objRoot, ItemData a_item)
        {
            GameObject objContent = Utility.FindGameObject(a_objRoot, "InfoView/ViewPort/Content");
            #region title
            _SetupItemTitle(a_objRoot, a_item, false);
            #endregion

            #region Interesting description
            _TryShowDescOnLeftSide(objContent, a_item.GetDescription());
            #endregion

            if (a_item.PackID > 0)
            {
                this.mVirtualPackContent.Add(new KeyValuePair<int, GameObject>(a_item.PackID, objContent));
                GiftPackDataManager.GetInstance().GetGiftPackItem(a_item.PackID);
            }
        }

        void OnGetGiftData(UIEvent param)
        {
            if (param == null || param.Param1 == null)
            {
                Logger.LogError("礼包数据为空");
                return;
            }
            GiftPackSyncInfo data = param.Param1 as GiftPackSyncInfo;
            CheckVirtualPackList(data, mVirtualPackContent);
            CheckVirtualPackList(data, mExpendablePackContent);
        }

        void CheckVirtualPackList(GiftPackSyncInfo data, List<KeyValuePair<int, GameObject>> list)
        {
            if (list != null && list.Count > 0)
            {
                if (data != null)
                {
                    for (int i = list.Count - 1; i >= 0; --i)
                    {
                        if (list[i].Key == data.id)
                        {
                            List<GiftPackItemData> arrGifts = new List<GiftPackItemData>();
                            switch ((GiftPackTable.eFilterType)data.filterType)
                            {
                                case GiftPackTable.eFilterType.None:
                                case GiftPackTable.eFilterType.Custom:
                                case GiftPackTable.eFilterType.Random:
                                    {
                                        for (int j = 0; j < data.gifts.Length; ++j)
                                        {
                                            GiftPackItemData giftTable = GiftPackDataManager.GetGiftDataFromNet(data.gifts[j]);
                                            if (giftTable.ItemID > 0)
                                            {
                                                arrGifts.Add(giftTable);
                                            }
                                        }
                                        break;
                                    }
                                case GiftPackTable.eFilterType.CustomWithJob:
                                case GiftPackTable.eFilterType.Job:
                                    {
                                        for (int j = 0; j < data.gifts.Length; ++j)
                                        {
                                            GiftPackItemData giftTable = GiftPackDataManager.GetGiftDataFromNet(data.gifts[j]);
                                            if (giftTable.ItemID > 0 && giftTable.RecommendJobs.Contains(PlayerBaseData.GetInstance().JobTableID))
                                            {
                                                arrGifts.Add(giftTable);
                                            }
                                        }
                                        break;
                                    }
                            }
                            _SetupShowItems(list[i].Value, arrGifts);
                            list.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        void _SetupSuitContent(GameObject a_objRoot, ItemData a_item, EquipSuitObj a_suitObj)
        {
            if (a_item == null || a_suitObj == null)
            {
                return;
            }
            
            #region name
            {

                int wearCount = 0;
                for (int i = 0; i < a_suitObj.equipSuitRes.equips.Count; ++i)
                {
                    int equipID = a_suitObj.equipSuitRes.equips[i];

                    ItemData equip = ItemDataManager.GetInstance().GetCommonItemTableDataByID(equipID);
                    if (equip == null)
                        continue;

                    if (a_suitObj.IsSuitEquipActive(equip))
                    {
                        wearCount++;
                    }
                }

                string temp;
                if (wearCount < a_suitObj.equipSuitRes.equips.Count)
                {
                    //未穿整套
                    temp = TR.Value("Common_Two_Format_One", a_suitObj.equipSuitRes.name, TR.Value("grid_info", wearCount, a_suitObj.equipSuitRes.equips.Count));
                }
                else
                {
                    //穿戴整套
                    temp = TR.Value("Common_Two_Format_One", TR.Value("tip_color_green", string.Format("{0}", a_suitObj.equipSuitRes.name)), TR.Value("tip_color_green", TR.Value("grid_info", wearCount, a_suitObj.equipSuitRes.equips.Count)));
                }
                
                if (string.IsNullOrEmpty(temp) == false)
                {
                    _SetupCommonTitle(a_objRoot, temp);
                }
            }
            #endregion

            #region suit equips
            {
                GameObject objGroup = _SetupGroup(a_objRoot);
                string temp = "";

                for (int i = 0; i < a_suitObj.equipSuitRes.equips.Count; ++i)
                {
                    int equipID = a_suitObj.equipSuitRes.equips[i];

                    ItemData equip = ItemDataManager.GetInstance().GetCommonItemTableDataByID(equipID);
                    if(equip == null)
                        continue;

                    if (a_suitObj.IsSuitEquipActive(equip))
                    {
                        temp = TR.Value("tip_color_green", equip.Name);
                    }
                    else
                    {
                        temp = TR.Value("tip_color_gray3", equip.Name);
                    }
                    _SetupLeftLabel(objGroup, temp);
                }
            }
            #endregion

            #region suit effect
            {
                GameObject objGroup = _SetupGroup(a_objRoot);

                var iter = a_suitObj.equipSuitRes.props.GetEnumerator();
                while (iter.MoveNext())
                {
                    string strDesc = string.Empty;
                    bool bActive = a_suitObj.wearedEquipIDs.Count >= iter.Current.Key;
                    string title_color_format = bActive ? "tip_color_green" : "tip_color_gray3";
                    strDesc = TR.Value(title_color_format, TR.Value("tip_suit_effect", iter.Current.Key));

                    #region baseProp
                    {
                        List<string> strList = iter.Current.Value.GetPropsFormatStr(bActive ? true : false);
                        if (strList != null)
                        {
                            string content_color_format = bActive ? "tip_color_blue" : "tip_color_gray3";
                            for (int i = 0; i < strList.Count; ++i)
                            {
                                strDesc += "\n";
                                strDesc += TR.Value(content_color_format, strList[i]);
                            }
                        }
                    }
                    #endregion

                    #region buffSkill
                    {
                        List<EquipProp.BuffSkillInfo> buffSkillInfos = iter.Current.Value.GetBuffSkillInfos();
                        if (buffSkillInfos != null)
                        {
                            for (int i = 0; i < buffSkillInfos.Count; ++i)
                            {
                                EquipProp.BuffSkillInfo buffSkillInfo = buffSkillInfos[i];

                                string job_color_format;
                                if (_IsJobMatch(buffSkillInfo.jobID))
                                {
                                    if (bActive)
                                    {
                                        job_color_format = "color_orange";
                                    }
                                    else
                                    {
                                        job_color_format = "tip_color_gray3";
                                    }
                                }
                                else
                                {
                                    job_color_format = "tip_color_gray3";
                                }
                                strDesc += "\n";
                                strDesc += TR.Value(job_color_format, string.Format("[{0}]", buffSkillInfo.jobName));

                                string color_format;
                                if (_IsJobMatch(buffSkillInfo.jobID))
                                {
                                    if (bActive)
                                    {
                                        color_format = "tip_color_blue";
                                    }
                                    else
                                    {
                                        color_format = "tip_color_gray3";
                                    }
                                }
                                else
                                {
                                    color_format = "tip_color_gray3";
                                }
                                for (int j = 0; j < buffSkillInfo.skillDescs.Count; ++j)
                                {
                                    strDesc += "\n";
                                    strDesc += TR.Value(color_format, buffSkillInfo.skillDescs[j]);
                                }
                            }
                        }
                    }
                    #endregion

                    #region buffOther
                    {
                        string content_color_format = bActive ? "tip_color_blue" : "tip_color_gray3";
                        List<string> strList = iter.Current.Value.GetBuffCommonDescs();
                        if (strList != null)
                        {
                            for (int i = 0; i < strList.Count; ++i)
                            {
                                strDesc += "\n";
                                strDesc += TR.Value(content_color_format, strList[i]);
                            }
                        }
                        if (string.IsNullOrEmpty(iter.Current.Value.attachBuffDesc) == false)
                        {
                            strDesc += "\n";
                            strDesc += TR.Value(content_color_format, iter.Current.Value.attachBuffDesc);
                        }
                    }
                    #endregion

                    #region mechanism
                    {
                        string content_color_format = bActive ? "tip_color_blue" : "tip_color_gray3";
                        List<string> strList = iter.Current.Value.GetMechanismDescs();
                        if (strList != null)
                        {
                            for (int i = 0; i < strList.Count; ++i)
                            {
                                strDesc += "\n";
                                strDesc += TR.Value(content_color_format, strList[i]);
                            }
                        }
                        if (string.IsNullOrEmpty(iter.Current.Value.attachMechanismDesc) == false)
                        {
                            strDesc += "\n";
                            strDesc += TR.Value(content_color_format, iter.Current.Value.attachMechanismDesc);
                        }
                    }
                    #endregion

                    _SetupLeftLabel(objGroup, strDesc);
                }
            }
            #endregion
        }

        /// <summary>
        /// 基础属性
        ///
        /// 物理攻击
        /// 魔法攻击
        /// 物理防御
        /// 魔法防御
        /// 固定攻击
        /// </summary>
        List<string> _GetBaseMainPropDescs(EquipProp a_Prop, EquipProp a_compareProp, ItemData itemData = null)
        {
            List<string> descs = new List<string>();

            EEquipProp[] fourProps =
            {
                EEquipProp.PhysicsAttack,
                EEquipProp.MagicAttack,
                EEquipProp.PhysicsDefense,
                EEquipProp.MagicDefense,
                EEquipProp.Independence
            };

            EEquipProp[] fourPropsSt =
            {
                EEquipProp.IgnorePhysicsAttack,
                EEquipProp.IgnoreMagicAttack,
                EEquipProp.IgnorePhysicsDefense,
                EEquipProp.IgnoreMagicDefense
            };

            EEquipProp[] fourPropsStRate =
            {
                EEquipProp.IgnorePhysicsAttackRate,
                EEquipProp.IgnoreMagicAttackRate,
                EEquipProp.IgnorePhysicsDefenseRate,
                EEquipProp.IgnoreMagicDefenseRate
            };

            int value = 0;
            var systemValueTable = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_Independent);
            if (systemValueTable != null)
            {
                value = systemValueTable.Value;
            }

            string temp;
            for (int i = 0; i < fourProps.Length; ++i)
            {
                temp = a_Prop.GetPropFormatStr(fourProps[i]);
                if (string.IsNullOrEmpty(temp) == false)
                {
                    string desc = string.Empty;
                    if (fourProps[i] == EEquipProp.Independence)
                    {
                        desc = value > 0 ? string.Empty : TR.Value("tip_independence_pvpdesc");
                    }

                    if (a_compareProp != null)
                    {
                        int index = (int)fourProps[i];
                        int value1 = 0;
                        int value2 = 0;
                        if (fourProps[i] == EEquipProp.Independence)
                        {
                            value1 = (int)(a_Prop.props[index] / 1000.0f);
                            value2 = (int)(a_compareProp.props[index] / 1000.0f);
                        }
                        else
                        {
                            value1 = a_Prop.props[index];
                            value2 = a_compareProp.props[index];
                        }
                        
                        temp += " ";
                        temp += _GetDifferenceDesc(value1, value2);
                    }

                    //if (null != itemData && itemData.Type == EItemType.EQUIP && itemData.SubType == (int)ItemTable.eSubType.WEAPON)
                    //{
                    //    string streangthDesc = string.Empty;
                    //    int level            = itemData.StrengthenLevel;

                    //    string strentemp = a_Prop.GetPropFormatStr(fourPropsSt[i]);
                    //    if (!string.IsNullOrEmpty(strentemp))
                    //    {
                    //        streangthDesc += TR.Value("tip_strengthen_effect", level, strentemp);
                    //    }

                    //    string strentempRate = a_Prop.GetPropFormatStr(fourPropsStRate[i]);
                    //    if (!string.IsNullOrEmpty(strentempRate))
                    //    {
                    //        if (!string.IsNullOrEmpty(streangthDesc))
                    //        {
                    //            streangthDesc += " ";
                    //        }
                    //        streangthDesc += TR.Value("tip_strengthen_effect", level, strentempRate);
                    //    }

                    //    if (!string.IsNullOrEmpty(streangthDesc))
                    //    {
                    //        temp += string.Format(" ({0})", streangthDesc);
                    //    }
                    //}
                    temp += desc;
                    descs.Add(temp);
                }
            }


            //if (null != itemData && itemData.Type == EItemType.EQUIP)
            //{
            //    if (itemData.SubType == (int)ItemTable.eSubType.WEAPON)
            //    {
            //        _TryAddDesc(descs, " ");
            //    }
            //    else
            //    {
            //        _TryAddDesc(descs, " ");
            //        _TryAddDesc(descs, " ");
            //    }
            //}

            return descs;
        }

        string _GetDifferenceDesc(int a_value0, int a_value1)
        {
            if (a_value0 == a_value1)
            {
                return "";
            }
            else if (a_value0 > a_value1)
            {
                string desc = string.Format("(+{0})", a_value0 - a_value1);
                return TR.Value("tip_color_green", desc);
            }
            else
            {
                string desc = string.Format("({0})", a_value0 - a_value1);
                return TR.Value("color_black_white", desc);
            }
        }

        void _TryAddDesc(List<string> a_descs, string a_desc)
        {
            if (string.IsNullOrEmpty(a_desc) == false)
            {
                a_descs.Add(a_desc);
            }
        }

        void _TryAddDescs(List<string> a_targetDescs, List<string> a_sourceDescs)
        {
            if (a_sourceDescs != null && a_sourceDescs.Count > 0)
            {
                a_targetDescs.AddRange(a_sourceDescs);
            }
        }

        void _TryShowDescOnLeftSide(GameObject a_objRoot, string a_desc, bool a_bNeedLine = true)
        {
            if (string.IsNullOrEmpty(a_desc) == false)
            {
                if (a_objRoot.transform.childCount > 0 && a_bNeedLine)
                {
                    _SetupLine(a_objRoot);
                }

                GameObject objGroup = _SetupGroup(a_objRoot);
                _SetupLeftLabel(objGroup, a_desc);
            }
        }

        void _TryShowDescOnRightSide(GameObject a_objRoot, string a_desc, bool a_bNeedLine = true)
        {
            if (string.IsNullOrEmpty(a_desc) == false)
            {
                if (a_objRoot.transform.childCount > 0 && a_bNeedLine)
                {
                    _SetupLine(a_objRoot);
                }

                GameObject objGroup = _SetupGroup(a_objRoot);
                _SetupRightLabel(objGroup, a_desc);
            }
        }

        private void InitBeadAttr(GameObject a_objRoot, ItemData item)
        {
            if (item.PreciousBeadMountHole == null)
            {
                return;
            }

            for (int i = 0; i < item.PreciousBeadMountHole.Length; i++)
            {
                PrecBead mData = item.PreciousBeadMountHole[i];
                if (mData == null)
                {
                    continue;
                }

                var mBeadItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mData.preciousBeadId);
                if (mBeadItemData == null)
                {
                    continue;
                }

                UpdateCommonItemInfo(a_objRoot, mData, item, m_objCommonItemInfoRoot);
            }
        }

        private void InitEnchantmentAttr(GameObject a_objRoot, ItemData item)
        {
            if (item.mPrecEnchantmentCard == null)
            {
                return;
            }

            var enchantmentCardItemData = ItemDataManager.CreateItemDataFromTable(item.mPrecEnchantmentCard.iEnchantmentCardID);
            if (enchantmentCardItemData == null)
            {
                return;
            }

            UpdateCommonItemInfo(a_objRoot, item.mPrecEnchantmentCard, item, m_objCommonItemInfoRoot);
        }

        private void InitInscripotionAttr(GameObject a_objRoot, ItemData item)
        {
            if (item.InscriptionHoles == null)
            {
                return;
            }

            for (int i = 0; i < item.InscriptionHoles.Count; i++)
            {
                var inscriptionHoleData = item.InscriptionHoles[i];
                if (inscriptionHoleData == null)
                {
                    continue;
                }

                if (inscriptionHoleData.IsOpenHole == false)
                {
                    continue;
                }

                UpdateCommonItemInfo(a_objRoot, inscriptionHoleData, item, m_objCommonItemInfoRoot);
            }
        }

        private void UpdateCommonItemInfo(GameObject a_objRoot, object data,ItemData equipmentItem,GameObject a_objCommonItemInfoRoot)
        {
            GameObject inscriptionGo = _SetCommonItemInfo(a_objRoot, a_objCommonItemInfoRoot);
            CommonItemInfo mCommonItemInfo = inscriptionGo.GetComponent<CommonItemInfo>();
            if (mCommonItemInfo != null)
            {
                mCommonItemInfo.InitInterface(data, equipmentItem);
            }
        }
        
        GameObject _SetCommonItemInfo(GameObject a_objeRoot,GameObject holeRoot)
        {
            GameObject obj = GameObject.Instantiate(holeRoot);
            obj.transform.SetParent(a_objeRoot.transform, false);
            obj.SetActive(true);

            return obj;
        }

        void _TryShowDescsOnLeftAndQuality(GameObject a_objRoot, List<string> a_descs,ItemData a_item)
        {
            if (a_descs != null && a_descs.Count > 0)
            {
                GameObject objGroup = _SetupGroup(a_objRoot);

                _SetupHLabelQuality(objGroup, a_descs[0], a_item);

                for (int i = 1; i < a_descs.Count; i++)
                {
                    _SetupLeftLabel(objGroup, a_descs[i]);
                }
            }
        }

        void _TryShowDescsOnLeftSide(GameObject a_objRoot, List<string> a_descs)
        {
            if (a_descs != null && a_descs.Count > 0)
            {
                GameObject objGroup = _SetupGroup(a_objRoot);
                for (int i = 0; i < a_descs.Count; ++i)
                {
                    _SetupLeftLabel(objGroup, a_descs[i]);
                }
            }
        }

        /// <summary>
        /// 保证底部对齐，在m_stretch处进行伸缩（添加删除空白行）
        /// </summary>
        void _TryShowDescsOnBothSide(GameObject a_objRoot, List<string> a_leftDescs, List<string> a_rightDescs, bool bIsReset = false)
        {
            if (
                a_leftDescs != null && a_rightDescs != null &&
                (a_leftDescs.Contains(m_stretch) && a_rightDescs.Contains(m_stretch)) &&
                (a_leftDescs.Count > 1 || a_rightDescs.Count > 1)
                )
            {
                int spaceCount = Mathf.Abs(a_leftDescs.Count - a_rightDescs.Count);
                string[] spaceList = new string[spaceCount];
                for (int i = 0; i < spaceCount; ++i)
                {
                    spaceList[i] = " ";
                }
                if (a_leftDescs.Count > a_rightDescs.Count)
                {
                    a_rightDescs.InsertRange(a_rightDescs.FindIndex(data => { return data == m_stretch; }), spaceList);
                }
                else if (a_leftDescs.Count < a_rightDescs.Count)
                {
                    a_leftDescs.InsertRange(a_leftDescs.FindIndex(data => { return data == m_stretch; }), spaceList);
                }

                a_leftDescs.Remove(m_stretch);
                a_rightDescs.Remove(m_stretch);

                if (a_leftDescs.Count == a_rightDescs.Count)
                {
                    GameObject objGroup = _SetupGroupTitle(a_objRoot);
                    for (int i = 0; i < a_leftDescs.Count; ++i)
                    {
                        _SetupHTwoLabels(objGroup, a_leftDescs[i], a_rightDescs[i], bIsReset);
                    }
                }
            }
        }

        bool _IsJobMatch(int jobID)
        {
            return PlayerBaseData.GetInstance().ActiveJobTableIDs.Contains(jobID);
        }


        void _SetupFunc(ItemData item, List<TipFuncButon> funcInfos, int nTipIndex)
        {
            _ClearFunc();

            if (funcInfos == null || m_objFuncBtnPrefab == null || m_objFuncBtnRoot == null || m_objFuncOtherBtnPrefab == null)
            {
                return;
            }

            if (funcInfos.Count <= 0)
            {
                return;
            }

            int triggerNumber = 0;
            for (int i = 0; i < funcInfos.Count; i++)
            {
                if (funcInfos[i].tipFuncButtonType != TipFuncButtonType.Trigger)
                {
                    continue;
                }

                triggerNumber++;
            }

            otherBtnFuncInfosGo = new List<GameObject>();
            
            m_objFuncBtnRoot.SetActive(true);
            for (int i = 0; i < funcInfos.Count; ++i)
            {
                TipFuncButon temp = funcInfos[i];
                GameObject funcObj = null;
                if (temp is TipFuncButonSpecial)
                {
                    funcObj = m_objFuncSpecial;
                }
                else if (temp is TipFuncButtonOther)
                {
                    funcObj = GameObject.Instantiate(m_objFuncOtherBtnPrefab);
                    funcObj.transform.SetParent(m_objFuncBtnRoot.transform, false);
                    funcObj.name = temp.name;
                }
                else
                {
                    funcObj = GameObject.Instantiate(m_objFuncBtnPrefab);
                    funcObj.transform.SetParent(m_objFuncBtnRoot.transform, false);
                    funcObj.name = temp.name;
                }

                if (funcObj != null)
                {
                    if (item.Type == ItemTable.eType.EQUIP)
                    {
                        if (triggerNumber > 1)
                        {
                            if (temp.tipFuncButtonType == TipFuncButtonType.Trigger)
                            {
                                funcObj.CustomActive(false);
                                otherBtnFuncInfosGo.Add(funcObj);
                            }
                            else
                            {
                                funcObj.CustomActive(true);
                            }
                        }
                        else
                        {
                            if (temp.tipFuncButtonType == TipFuncButtonType.Other)
                            {
                                funcObj.CustomActive(false);
                            }
                            else
                            {
                                funcObj.CustomActive(true);
                            }
                        }
                    }
                    else
                    {
                        funcObj.CustomActive(true);
                    }
                    
                    
                    if (temp is TipFuncButtonOther)
                    {
                        mItemTipsFuncOtherButton = funcObj.GetComponent<ItemTipsFuncOtherButton>();
                    }

                    Button btn = funcObj.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() =>
                        {
                            temp.callback(item, nTipIndex);
                        });
                    }

                    Toggle tog = funcObj.GetComponent<Toggle>();
                    if (tog != null)
                    {
                        tog.onValueChanged.RemoveAllListeners();
                        tog.onValueChanged.AddListener((value) =>
                        {
                            bIsSelect = value;
                            temp.callback(item, nTipIndex);
                        });
                    }

                    Text name = Utility.GetComponetInChild<Text>(funcObj, "Text");
                    if (name != null)
                    {
                        name.text = temp.text;
#if APPLE_STORE
                        //add by mjx for ios app store
                        TryChangeShowShareTextForAppStore(name);
#endif
                    }
                }
            }
        }
        
        void _OnMoreAndMoreClick(UIEvent uiEvent)
        {
            if (mItemTipsFuncOtherButton != null)
            {
                mItemTipsFuncOtherButton.SetTabItemRoot();
            }

            for (int i = 0; i < otherBtnFuncInfosGo.Count; i++)
            {
                otherBtnFuncInfosGo[i].CustomActive(bIsSelect);
            }
        }

        void _ItemSellSuccess(UIEvent uiEvent)
        {
            _SetupItemTitle(m_objTipTemplate, data.item);
        }

        void _ClearFunc()
        {
            m_objFuncBtnRoot.SetActive(false);
            m_objFuncSpecial.SetActive(false);
            m_objFuncBtnPrefab.SetActive(false);
            m_objFuncOtherBtnPrefab.CustomActive(false);
        }

        void _OnItemRemoved(UIEvent a_event)
        {
            ItemTipData tipData = userData as ItemTipData;
            if (tipData.item.TableID == (uint)a_event.Param1)
            {
                if (ItemDataManager.GetInstance().GetItem(tipData.item.GUID) == null)
                {
                    ItemTipManager.GetInstance().CloseTip(tipData.nTipIndex);
                }
            }
        }

        void _OnItemUseSuccess(UIEvent a_event)
        {
            //使用一个道具对ItemTipFrame进行刷新
            ItemTipData tipData = userData as ItemTipData;
            if (tipData == null)
                return;

            //位置，顺序，父节点不变，直接刷新内容
            _SetupContent(m_objTipTemplate, tipData.item, tipData.compareItem, tipData.itemSuitObj);
            //_SetupTip(m_objTipTemplate, tipData.item, tipData.compareItem, tipData.itemSuitObj);
        }

        void OnCounterChanged(UIEvent iEvent)
        {
            ItemTipData tipData = userData as ItemTipData;
            if(tipData == null || tipData.item == null)
            {
                return;
            }

            if (tipData.item.SubType == (int)ItemTable.eSubType.ST_TEAM_COPY_CNT || 
                tipData.item.SubType == (int)ItemTable.eSubType.ST_TEAM_COPY_DIFF_CNT)
            {

            }
            else
            {
                if (tipData.item.SubType != (int)ProtoTable.ItemTable.eSubType.FatigueDrug)
                {
                    return;
                }
            }
            
            _SetupTip(m_objTipTemplate, tipData.item, tipData.compareItem, tipData.itemSuitObj);
	        _SetupTip(compareItemContentGo, tipData.compareItem, null, tipData.compareItemSuitObj);
		}

        void TryChangeShowShareTextForAppStore(Text name)
        {
            if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.SHARE_TEXT_CHANGE))
            {
                if (name)
                {
                    string showName = name.text;
                    if (showName.Equals(TR.Value("tip_share")))
                    {
                        name.text = "展示";
                    }
                }
            }
        }


        #region ItemTipModel

        //ItemTip上展示道具模型相关内容

        //ItemTipFrame的索引
        private int _itemTipFrameIndex;

        private int _otherProfessionId;

        //针对ModelAvatar加载的控件
        private ComTwoToggleController _twoToggleController;                    //右边Toggle控制器
        private ItemTipModelAvatarRootView _itemTipModelAvatarRootView;         //RootView

        //ItemTip展示两个对比的道具
        private int _selectItemTipCompareItemTypeIndex;
        private ItemTipShowAvatarType _itemTipShowAvatarType;

        //被点击的道具
        private ItemData _clickedItemData;

        //是否显示ModelAvatar
        private bool _isShowModelAvatarFlag = false;
        private int _itemTipModelAvatarLayerIndex;

        //礼包类型相关数据
        private GiftPackModelAvatarDataModel _giftPackModelAvatarDataModel;

        //初始化ItemTip中AvatarModel的数据相关数据
        private void InitItemTipModelAvatarData()
        {
            var itemTipData = userData as ItemTipData;
            if (itemTipData != null)
            {
                _itemTipFrameIndex = itemTipData.nTipIndex;
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveItemTipFrameOpenMessage,
                    _itemTipFrameIndex);
            }
        }

        //重置AvatarModel相关的数据
        private void ResetItemTipModelAvatarData()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnReceiveItemTipFrameCloseMessage, _itemTipFrameIndex);
            ClearData();
        }

        private void ClearData()
        {
            _twoToggleController = null;
            _itemTipModelAvatarRootView = null;

            _selectItemTipCompareItemTypeIndex = 0;
            _itemTipShowAvatarType = ItemTipShowAvatarType.None;
            _clickedItemData = null;

            _isShowModelAvatarFlag = false;
            _itemTipModelAvatarLayerIndex = 0;

            ResetGiftPackModelAvatarData();

            _otherProfessionId = 0;
        }

        //重置礼包道具的数据
        private void ResetGiftPackModelAvatarData()
        {
            _giftPackModelAvatarDataModel = null;
        }

        public void InitItemTipModelAvatarContent(ItemTipData itemTipData)
        {
            _isShowModelAvatarFlag = false;
            _itemTipShowAvatarType = ItemTipShowAvatarType.None;
            ResetGiftPackModelAvatarData();
            _otherProfessionId = 0;

            if (itemTipData == null)
                return;

            //强制关闭
            if (itemTipData.IsForceCloseModelAvatar == true)
                return;
            
            //道具为null
            _clickedItemData = itemTipData.item;
            if (_clickedItemData == null)
                return;
            if (_clickedItemData.TableData == null)
                return;

            //根据类型,子类型和获得情况判断是否可以展示ModelAvatar
            var isItemTipShowModelAvatar = ItemDataUtility.IsItemTipShowModelAvatar(_clickedItemData);
            if (isItemTipShowModelAvatar == false)
                return;

            //礼包类型：Type：虚拟礼包；subType为礼包
            if (_clickedItemData.TableData.Type == ItemTable.eType.VirtualPack ||
                _clickedItemData.TableData.SubType == ItemTable.eSubType.GiftPackage)
            {

                var giftPackId = _clickedItemData.TableData.PackageID;
                _giftPackModelAvatarDataModel = ItemDataUtility.GetGiftPackModelAvatarDataModel(giftPackId);

                //礼包展示的数据不存在
                if (_giftPackModelAvatarDataModel == null)
                    return;
            }
            else
            {
                var otherProfessionId = 0;
                //单一物体，是否职业适配
                var isOccupationFit =
                    ItemDataUtility.IsSuitPlayerProfession(_clickedItemData.TableData, out otherProfessionId);
                //职业不匹配
                if (isOccupationFit == false)
                {
                    var itemTableSubType = _clickedItemData.TableData.SubType;
                    //如果是武器或者时装武器，获得其他职业的Id
                    if (itemTableSubType == ItemTable.eSubType.WEAPON
                        || itemTableSubType == ItemTable.eSubType.FASHION_WEAPON)
                    {
                        //职业不匹配的时候，获得其他职业的Id,可以展示道具
                        _otherProfessionId = otherProfessionId;
                    }
                    else
                    {
                        //其他情况不能展示人物模型
                        return;
                    }
                }

                //职业匹配或者职业不匹配并且是武器和时装武器类型：可以展示模型
            }

            //可以展示
            _isShowModelAvatarFlag = true;
            //得到展示的Layer层级
            _itemTipModelAvatarLayerIndex =
                ItemTipManager.GetInstance().GetItemTipModelAvatarLayerIndex(_itemTipFrameIndex);

            var compareItemData = itemTipData.compareItem;
            //单独一个道具
            if (compareItemData == null)
            {
                _itemTipShowAvatarType = ItemTipShowAvatarType.SingleTipType;
                InitItemTipModelAvatarContentBySingleType();
            }
            else
            {
                _itemTipShowAvatarType = ItemTipShowAvatarType.CompareTipType;
                InitItemTipModelAvatarContentByCompareType();
            }
        }

        private void InitItemTipModelAvatarContentByCompareType()
        {
            if (m_tipTwoToggleRoot == null)
                return;
            CommonUtility.UpdateGameObjectVisible(m_tipTwoToggleRoot, true);

            _selectItemTipCompareItemTypeIndex = (int)ItemTipCompareItemType.CompareAttribute;

            var twoToggleControllerPrefab = CommonUtility.LoadGameObject(m_tipTwoToggleRoot);
            if (twoToggleControllerPrefab != null)
            {
                _twoToggleController = twoToggleControllerPrefab.GetComponent<ComTwoToggleController>();
                if (_twoToggleController != null)
                {
                    _twoToggleController.InitTwoToggleController(
                        (int) ItemTipCompareItemType.CompareAttribute,
                        TR.Value("Item_Tip_Frame_Attribute_Button_Label"),
                        (int) ItemTipCompareItemType.ShowAvatar,
                        TR.Value("Item_Tip_Frame_TryOn_Button_Label"),
                        OnItemTipToggleClicked,
                        _selectItemTipCompareItemTypeIndex);
                }
            }
        }

        private void OnItemTipToggleClicked(int toggleIndex)
        {
            if (toggleIndex == _selectItemTipCompareItemTypeIndex)
                return;

            _selectItemTipCompareItemTypeIndex = toggleIndex;

            UpdateItemTipCompareItemContent();
        }

        //更新ItemTip的内容（属性对比和试穿)
        private void UpdateItemTipCompareItemContent()
        {
            if (_selectItemTipCompareItemTypeIndex == (int)ItemTipCompareItemType.ShowAvatar)
            {
                CommonUtility.UpdateGameObjectVisible(compareItemContentGo, false);

                if (_itemTipModelAvatarRootView == null)
                {
                    var modelAvatarRoot = Object.Instantiate(m_avatarModelRoot) as GameObject;
                    if (modelAvatarRoot != null && m_objTipRoot != null)
                    {
                        modelAvatarRoot.transform.SetParent(m_objTipRoot.transform, false);
                        modelAvatarRoot.transform.SetAsFirstSibling();

                        _itemTipModelAvatarRootView = modelAvatarRoot.GetComponent<ItemTipModelAvatarRootView>();
                    }
                }

                //刷新ModelAvatar
                if (_itemTipModelAvatarRootView != null)
                {
                    CommonUtility.UpdateGameObjectVisible(_itemTipModelAvatarRootView.gameObject, true);
                    _itemTipModelAvatarRootView.UpdateModelAvatarRootViewByCompareItemType(_clickedItemData.TableData,
                        _itemTipShowAvatarType,
                        _itemTipModelAvatarLayerIndex,
                        _otherProfessionId);
                }
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(compareItemContentGo, true);

                if (_itemTipModelAvatarRootView != null)
                {
                    _itemTipModelAvatarRootView.OnDisappearShowModelAvatarView();
                    CommonUtility.UpdateGameObjectVisible(_itemTipModelAvatarRootView.gameObject, false);
                }
            }
        }

        //单独一个模型
        private void InitItemTipModelAvatarContentBySingleType()
        {
            //根节点
            if (m_avatarModelRoot == null || m_objTipRoot == null)
                return;

            //创建预制体
            var modelAvatarRoot = Object.Instantiate(m_avatarModelRoot) as GameObject;
            if (modelAvatarRoot == null)
                return;

            modelAvatarRoot.transform.SetParent(m_objTipRoot.transform, false);
            modelAvatarRoot.transform.SetAsFirstSibling();
            CommonUtility.UpdateGameObjectVisible(modelAvatarRoot, true);

            _itemTipModelAvatarRootView = modelAvatarRoot.GetComponent<ItemTipModelAvatarRootView>();
            if (_itemTipModelAvatarRootView != null)
                _itemTipModelAvatarRootView.UpdateModelAvatarRootViewBySingleItemType(_clickedItemData.TableData,
                    _itemTipShowAvatarType,
                    _itemTipModelAvatarLayerIndex,
                    _giftPackModelAvatarDataModel,
                    _otherProfessionId);
        }

        //收到其他ItemTipFrame打开的消息
        private void OnReceiveItemTipFrameOpenMessage(UIEvent uiEvent)
        {
            //不显示模型，不处理
            if (_isShowModelAvatarFlag == false)
                return;
            
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            int receiveItemTipFrameIndex = (int)uiEvent.Param1;

            //同一个TipFrame，直接返回
            if (_itemTipFrameIndex == receiveItemTipFrameIndex)
                return;

            //不是前一个，直接返回
            if (_itemTipFrameIndex != receiveItemTipFrameIndex - 1)
                return;

            if (_itemTipModelAvatarRootView == null)
                return;

            //单独展示模型，直接隐藏
            if (_itemTipShowAvatarType == ItemTipShowAvatarType.SingleTipType)
            {
                _itemTipModelAvatarRootView.OnDisappearShowModelAvatarView();
            }
            else if (_itemTipShowAvatarType == ItemTipShowAvatarType.CompareTipType)
            {
                //显示比较的时候
                //显示比较，并且当前选中的页签为展示模型
                if ((ItemTipCompareItemType)_selectItemTipCompareItemTypeIndex == ItemTipCompareItemType.ShowAvatar)
                {
                    _itemTipModelAvatarRootView.OnDisappearShowModelAvatarView();
                }
            }
        }

        //收到其他ItemTipFrame关闭的消息
        private void OnReceiveItemTipFrameCloseMessage(UIEvent uiEvent)
        {
            //不显示模型，不处理
            if (_isShowModelAvatarFlag == false)
                return;

            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            int receiveItemTipFrameIndex = (int)uiEvent.Param1;

            //同一个TipFrame，直接返回
            if(_itemTipFrameIndex == receiveItemTipFrameIndex)
                return;

            //不是前一个，直接返回
            if (_itemTipFrameIndex != receiveItemTipFrameIndex - 1)
                return;

            if (_itemTipModelAvatarRootView == null)
                return;

            //单独显示一个模型
            if (_itemTipShowAvatarType == ItemTipShowAvatarType.SingleTipType)
            {
                _itemTipModelAvatarRootView.OnShowModelAvatarView();
            }
            else if (_itemTipShowAvatarType == ItemTipShowAvatarType.CompareTipType)
            {
                //显示比较，并且当前选中的页签为展示模型
                if ((ItemTipCompareItemType) _selectItemTipCompareItemTypeIndex == ItemTipCompareItemType.ShowAvatar)
                {
                    _itemTipModelAvatarRootView.OnShowModelAvatarView();
                }
            }
        }

        public bool GetShowModelAvatarFlag()
        {
            return _isShowModelAvatarFlag;
        }

        #endregion

    }
}
