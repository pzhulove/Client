using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GameClient
{
    public interface IComItemCustom
    {
        void Init(bool enableTips, ItemData itemData, bool bHideCount, bool hasBottomDesc, ComItemCustomClickType clickType);
        void Init(bool enableTips, ItemSimpleData itemSData, bool bOwned, bool bHideCount, bool hasBottomDesc, ComItemCustomClickType clickType);
        void Init(bool enableTips, int itemDataId, bool bOwned, bool bHideCount, bool hasBottomDesc, ComItemCustomClickType clickType);
    }

    public enum ComItemCustomClickType
    {
        None,
        Button,
        Toggle
    }

    [ExecuteAlways]
    public class ComItemCustom : MonoBehaviour, IComItemCustom
    {
        #region Model Params

        public delegate void ItemBtnClickHandle();
        public ItemBtnClickHandle onItemBtnClick;
        public ItemBtnClickHandle onExtendBtn1Click;

        public delegate void ItemToggleClickHandle(bool isOn);
        public ItemToggleClickHandle onTitleToggleClick;

        public delegate string CountFormatter(ComItemCustom a_comItem);
        public CountFormatter m_countFormatter;
        
        private ItemData m_ItemData = null;
        public ItemData M_ItemData
        {
            get { return m_ItemData; }
        }

        //private ComItem m_ComItem = null;
        //private ClientFrame m_DependFrame = null;
        //private bool bInitWithComItem = false;

        private bool bInitHideCount = false;
        private bool hasBottomExtendDesc = false;
        private bool bItemShowTipEnable = true;                      //点击事件是否可用

        #endregion

        #region View Params

        //[SerializeField]
        private ComItemCustomClickType m_ClickType = ComItemCustomClickType.None;

        [SerializeField]
        private GameObject m_CustomItemDisplayObj = null;
        [SerializeField]
        private GameObject m_CustomItemEventObj = null;
        [SerializeField]
        private Image m_CustomItemIconBgImg = null;
        [SerializeField]
        private Image m_CustomItemIconImg = null;
        [SerializeField]
        private Image m_CustomItemIconSelectImg = null;
        [SerializeField]
        private Text m_CustomItemCount = null;
        [SerializeField]
        private Text m_CustomItemDesc = null;
        [SerializeField]
        private Image m_CustomItemBindLockImg = null;

        [SerializeField]
        private Button m_CustomItemIconBtn = null;
        [SerializeField]
        private Toggle m_CustomItemToggle = null;
        [SerializeField]
        private ToggleGroup m_CustomItemToggleGroup = null;

        //[SerializeField]
        //private GameObject m_ComItemRoot = null;

        [SerializeField]
        [Header("扩展图片")]
        private List<Image> m_ExtendImgs = null;
        [SerializeField]
        [Header("扩展按钮1")]
        private Button m_ExtendBtn_1 = null;
        [SerializeField]
        [Header("扩展按钮1图片")]
        private Image m_ExtendBtnImg_1 = null;

        #endregion

        #region PRIVATE METHODS

        //Unity life cycle
        void Start()
        {
            if (m_CustomItemIconBtn)
            {
                m_CustomItemIconBtn.onClick.AddListener(_OnItemBtnClick);
            }
            if (m_CustomItemToggle)
            {
                m_CustomItemToggle.onValueChanged.AddListener(_OnItemToggleClick);
                if (m_CustomItemToggleGroup)
                    m_CustomItemToggle.group = m_CustomItemToggleGroup;
            }
            if (m_ExtendImgs != null)
            {
                for (int i = 0; i < m_ExtendImgs.Count; i++)
                {
                    m_ExtendImgs[i].CustomActive(true);
                    m_ExtendImgs[i].enabled = false;
                }
            }
            if (m_ExtendBtn_1)
            {
                m_ExtendBtn_1.onClick.AddListener(_OnExtendBtn1Click);
            }
        }

        //Unity life cycle
        void OnDestroy()
        {
            if (m_CustomItemIconBtn)
            {
                m_CustomItemIconBtn.onClick.RemoveListener(_OnItemBtnClick);
            }
            if(m_CustomItemToggle)
            {
                m_CustomItemToggle.onValueChanged.RemoveListener(_OnItemToggleClick);
            }
            if (m_ExtendBtn_1)
            {
                m_ExtendBtn_1.onClick.RemoveListener(_OnExtendBtn1Click);
            }

            Clear();
        }

        void _Init()
        {
            switch (m_ClickType)
            {
                case ComItemCustomClickType.Button:
                    if (m_CustomItemIconBtn)
                    {
                        m_CustomItemIconBtn.enabled = true;
                        m_CustomItemIconBtn.targetGraphic.enabled = true;
                    }
                    if (m_CustomItemToggle)
                    {
                        m_CustomItemToggle.isOn = false;
                        m_CustomItemToggle.enabled = false;
                        m_CustomItemToggle.targetGraphic.enabled = false;
                    }

                    break;
                case ComItemCustomClickType.Toggle:
                    if (m_CustomItemToggle)
                    {
                        m_CustomItemToggle.isOn = false;
                        m_CustomItemToggle.enabled = true;
                        m_CustomItemToggle.targetGraphic.enabled = true;
                    }
                    if (m_CustomItemIconBtn)
                    {
                        m_CustomItemIconBtn.enabled = false;
                        m_CustomItemIconBtn.targetGraphic.enabled = false;
                    }

                    break;
                default:
                    if (m_CustomItemIconBtn)
                    {
                        m_CustomItemIconBtn.enabled = false;
                        m_CustomItemIconBtn.targetGraphic.enabled = false;
                    }
                    if (m_CustomItemToggle)
                    {
                        m_CustomItemToggle.isOn = false;
                        m_CustomItemToggle.enabled = false;
                        m_CustomItemToggle.targetGraphic.enabled = false;
                    }
                    break;
            }

            SetCustomItemSelectActive(false);
            SetExtendImgsActive();
            SetExtendBtn1ShowAndEnable(false, false);

            _SetCustomItemImg();
            _SetCustomItemCount();
            _SetCustomItemDesc();
            _SetCustomItemBindLockImgShow();
        }

        void _OnItemBtnClick()
        {
            if (this.bItemShowTipEnable)
            {
                ItemTipManager.GetInstance().ShowTip(this.m_ItemData);
            }
            else
            {
                if (onItemBtnClick != null)
                {
                    onItemBtnClick();
                }
            }
        }

        void _OnItemToggleClick(bool isOn)
        {
            if (onTitleToggleClick != null)
            {
                onTitleToggleClick(isOn);
            }
        }

        void _OnExtendBtn1Click()
        {
            if (onExtendBtn1Click != null)
            {
                onExtendBtn1Click();
            }
        }

        void SetNameColorByQuality()
        {
            if (m_ItemData == null)
            {
                return;
            }
            var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(m_ItemData.TableID);
            if (m_CustomItemDesc == null)
            {
                return;
            }
            string colorStr = "white";
            if (itemTable != null)
            {
                colorStr = Parser.ItemParser.GetItemColor(itemTable);
            }
            string textStr = m_CustomItemDesc.text;
            m_CustomItemDesc.text = string.Format("<color={0}>", colorStr) + textStr + "</color>";
        }

        void _SetCustomItemCount()
        {
            string count = "";
            if (this.bInitHideCount)
            {
                count = "";
            }
            else
            {
                if (this.m_ItemData != null && this.m_ItemData.Count > 1)
                {
                    count = this.m_ItemData.Count.ToString();
                }
            }
            if (m_CustomItemCount)
            {
                m_CustomItemCount.text = count;
            }
        }

        void _SetCustomItemImg()
        {
            string bgPath = "";
            string resPath = "";
            if (this.m_ItemData != null)
            {
                bgPath = this.m_ItemData.GetQualityInfo().Background;
                resPath = this.m_ItemData.Icon;
            }
            if (m_CustomItemIconBgImg && !string.IsNullOrEmpty(bgPath))
            {
                m_CustomItemIconBgImg.enabled = true;
                ETCImageLoader.LoadSprite(ref m_CustomItemIconBgImg, bgPath);
            }
            if (m_CustomItemIconImg && !string.IsNullOrEmpty(resPath))
            {
                m_CustomItemIconImg.enabled = true;
                ETCImageLoader.LoadSprite(ref m_CustomItemIconImg, resPath);
            }
        }

        void _SetCustomItemDesc()
        {
            if (m_CustomItemDesc)
            {
                m_CustomItemDesc.CustomActive(this.hasBottomExtendDesc);
            }
        }

        void _SetCustomItemBindLockImgShow()
        {
            if (m_CustomItemBindLockImg)
            {
                m_CustomItemBindLockImg.enabled = false;
                if (this.m_ItemData != null)
                {
                    if (this.m_ItemData.SubType == (int)ProtoTable.ItemTable.eSubType.Coupon)
                    {
                        m_CustomItemBindLockImg.enabled = this.m_ItemData.BindAttr != ProtoTable.ItemTable.eOwner.NOTBIND;
                    } 
                }
            }            
        }

        //void SetInfoWithItem()
        //{
        //    if (m_ComItem == null && m_ComItemRoot != null)
        //    {
        //        m_ComItem = ComItemManager.Create(m_ComItemRoot);
        //    }
        //    if (m_ComItem == null)
        //    {
        //        return;
        //    }
        //    if (this.bItemShowTipEnable)
        //    {
        //        m_ComItem.Setup(m_ItemData, Utility.OnItemClicked);
        //    }
        //    else
        //    {
        //        m_ComItem.Setup(m_ItemData, null);
        //        //前置自定义UI事件
        //        SetCustomItemEventLastSibling();
        //    }

        //    if (this.bInitHideCount)
        //    {
        //        m_ComItem.SetCountFormatter((var) =>
        //        {
        //            return "";
        //        });
        //    }
        //}


        /// <summary>
        /// 当不初始化ComItem时  前置自定义UI事件节点 方便自定义事件点击
        /// </summary>
        //void SetCustomItemEventLastSibling()
        //{
        //    if (m_CustomItemEventObj)
        //    {
        //        m_CustomItemEventObj.transform.SetAsLastSibling();
        //    }
        //}

        #endregion

        #region  PUBLIC METHODS

        public void Init(bool enableTips, ItemData itemData, bool bHideCount, bool hasBottomDesc, ComItemCustomClickType clickType)
        {
            Clear();
            //this.m_DependFrame = dependFrame;
            //this.bInitWithComItem = bUsedComItem;
            this.bItemShowTipEnable = enableTips;
            this.bInitHideCount = bHideCount;
            this.hasBottomExtendDesc = hasBottomDesc;
            this.m_ClickType = clickType;
            
            this.m_ItemData = itemData;
            //if (this.bInitWithComItem)
            //{
            //    SetInfoWithItem();
            //}
            //else
            //{
            //    SetCustomItemEventLastSibling();
            //}
            _Init();
        }


        public void Init(bool enableTips, ItemSimpleData itemSData, bool bOwned, bool bHideCount, bool hasBottomDesc, ComItemCustomClickType clickType)
        {
            Clear();
            //this.m_DependFrame = dependFrame;
            //this.bInitWithComItem = bUsedComItem;
            this.bItemShowTipEnable = enableTips;
            this.bInitHideCount = bHideCount;
            this.hasBottomExtendDesc = hasBottomDesc;
            this.m_ClickType = clickType;
            
            if (itemSData != null)
            {
                if (bOwned)
                {
                    this.m_ItemData = ItemDataManager.GetInstance().GetItemByTableID(itemSData.ItemID);
                }
                else
                {
                    //this.m_ItemData = new ItemData(itemSData.ItemID);
                    this.m_ItemData = ItemDataManager.CreateItemDataFromTable(itemSData.ItemID);
                }
                //if (this.bInitWithComItem)
                //{
                //    SetInfoWithItem();
                //}
                //else
                //{
                //    SetCustomItemEventLastSibling();
                //}
            }
            _Init();
        }

        public void Init(bool enableTips, int itemDataId, bool bOwned, bool bHideCount, bool hasBottomDesc, ComItemCustomClickType clickType)
        {
            Clear();
            //this.m_DependFrame = dependFrame;
            //this.bInitWithComItem = bUsedComItem;
            this.bItemShowTipEnable = enableTips;
            this.bInitHideCount = bHideCount;
            this.hasBottomExtendDesc = hasBottomDesc;
            this.m_ClickType = clickType;
            
            if (bOwned)
            {
                this.m_ItemData = ItemDataManager.GetInstance().GetItemByTableID(itemDataId);
            }
            else
            {
                //this.m_ItemData = new ItemData(itemDataId);
                this.m_ItemData = ItemDataManager.CreateItemDataFromTable(itemDataId);
            }
            //if (this.bInitWithComItem)
            //{
            //    SetInfoWithItem();
            //}
            //else
            //{
            //    SetCustomItemEventLastSibling();
            //}
            _Init();
        }

        public void Clear()
        {
            onItemBtnClick = null;
            onTitleToggleClick = null;
            onExtendBtn1Click = null;

            //if (m_ComItem != null)
            //{
            //    ComItemManager.Destroy(m_ComItem);
            //    m_ComItem = null;
            //}
            m_ItemData = null;
            //m_DependFrame = null;

            //bInitWithComItem = false;
            bInitHideCount = false;
            hasBottomExtendDesc = false;
            bItemShowTipEnable = true;

            if (m_CustomItemIconBtn)
            {
                m_CustomItemIconBtn.enabled = false;
                m_CustomItemIconBtn.targetGraphic.enabled = false;
            }
            if (m_CustomItemToggle)
            {
                m_CustomItemToggle.isOn = false;
                m_CustomItemToggle.enabled = false;
                m_CustomItemToggle.targetGraphic.enabled = false;
            }

            if (m_CustomItemIconImg)
            {
                m_CustomItemIconImg.sprite = null;
                m_CustomItemIconImg.enabled = false;
            }
            if (m_CustomItemIconBgImg)
            {
                m_CustomItemIconBgImg.sprite = null;
                m_CustomItemIconBgImg.enabled = false;
            }
            if (m_CustomItemIconSelectImg)
            {
                m_CustomItemIconSelectImg.enabled = false;
            }
            if (m_CustomItemBindLockImg)
            {
                m_CustomItemBindLockImg.enabled = false;
            }
            if (m_CustomItemCount)
            {
                m_CustomItemCount.text = "";
            }
            if (m_CustomItemDesc)
            {
                m_CustomItemDesc.text = "";
            }
        }

        public void SetDescStr(string desc, bool isParser = true)
        {
            if (m_CustomItemDesc)
            {
                m_CustomItemDesc.text = desc;
                if (isParser)
                {
                    SetNameColorByQuality();
                }
            }
        }

        public void SetCustomItemSelectActive(bool bActive)
        {
            if (m_CustomItemIconSelectImg)
            {
                m_CustomItemIconSelectImg.enabled = bActive;
            }
        }

        public void SetCustomItemCount(bool checkItemOne = true, string formatStr = "")
        {
            string count = "";
            if (this.bInitHideCount)
            {
                count = "";
            }
            else
            {
                if (checkItemOne)
                {
                    if (m_ItemData.Count == 1)
                    {
                        count = "";
                    }
                    else
                    {
                        count = m_ItemData.Count.ToString();
                    }
                }
                else
                {
                    count = formatStr;
                }
            }
            if (m_CustomItemCount)
            {
                m_CustomItemCount.text = count;
            }
        }

        public void SetCustomItemToggleOn(bool isOn)
        {
            if (m_ClickType == ComItemCustomClickType.Toggle)
            {
                if (m_CustomItemToggle)
                {
                    m_CustomItemToggle.isOn = isOn;
                }
            }
        }

        public bool GetCustomItemToggleIsOn()
        {
            if (m_CustomItemToggle)
            {
                return m_CustomItemToggle.isOn;
            }
            return false;
        }

        /// <summary>
        /// 显示额外添加的默认图
        /// </summary>
        /// <param name="activeIndexes">序号 从0开始</param>
        public void SetExtendImgsActive(List<int> activeIndexes = null)
        {
            if (m_ExtendImgs == null)
            {
                return;
            }
            for (int i = 0; i < m_ExtendImgs.Count; i++)
            {
                m_ExtendImgs[i].enabled = false;
            }
            if(activeIndexes != null)
            {
                for (int i = 0; i < activeIndexes.Count; i++)
                {
                    int index = activeIndexes[i];
                    if (index < m_ExtendImgs.Count)
                    {
                        m_ExtendImgs[index].enabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// 设置扩展按钮1 是否可用
        /// </summary>
        public void SetExtendBtn1ShowAndEnable(bool bShow, bool bEnable)
        {
            if (m_ExtendBtn_1)
            {
                m_ExtendBtn_1.enabled = bEnable;
                m_ExtendBtn_1.targetGraphic.enabled = bShow;
            }
            if (m_ExtendBtnImg_1)
            {
                m_ExtendBtnImg_1.enabled = bEnable;
            }
        }

        #endregion
    }
}