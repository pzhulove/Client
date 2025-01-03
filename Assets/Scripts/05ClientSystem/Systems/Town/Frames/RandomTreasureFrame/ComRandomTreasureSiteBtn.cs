 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComRandomTreasureSiteBtn : MonoBehaviour
    {
        #region Model Params

        RandomTreasureMapDigSiteModel mSiteModel = null;
        uint mRefreshTime = 0;
        uint mChangeStatusTime = 0;
        uint mServerTime = 0;
        bool bDirtyCD = false;

        ComItem mTipItem = null;
        ItemData mTipItemData = null;
        string mTips_Treasure_IsOpened = "";

        #endregion

        #region View Params

        [SerializeField]
        private Image mGoldBg = null;
        [SerializeField]
        private Image mGoldBg_Open = null;
        [SerializeField]
        private Image mSilverBg = null;
        [SerializeField]
        private Image mSilverBg_Open = null;
        [SerializeField]
        private Button mFuncBtn = null;
        [SerializeField]
        private Image mTipImg = null;
        [SerializeField]
        private Button mTipBtn = null;

        [Header("打开后展示道具的倒计时（秒）")]
        [SerializeField]
        private int mShowOpenedItemSecond = 60;

        private GameObject mParent = null;

        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        void Awake()
        {
            //初始化  尽量不放在 Unity life cycle中 初始化
            _SetOpenedTipItemShow(false);
            _SetGoldBgShowByStatus(false, true);
            _SetSilverBgShowByStatus(false, true);
        }
        
        //Unity life cycle
        void Start () 
        {
            if (mFuncBtn)
            {
                mFuncBtn.onClick.AddListener(_OnFuncBtnClick);
            }
            if (mTipBtn)
            {
                mTipBtn.onClick.AddListener(_OnTipBtnClick);
            }
            mTips_Treasure_IsOpened = TR.Value("random_treasure_map_site_isopened");
        }
        
        //Unity life cycle
        void Update () 
        {
            if (!bDirtyCD)
            {
                return;
            }
            mServerTime = TimeManager.GetInstance().GetServerTime();
            if (mServerTime - mChangeStatusTime < mShowOpenedItemSecond ||
                mServerTime - mRefreshTime < mShowOpenedItemSecond)
            {
                _SetOpenedTipItemShow(true);
                _SetTypeTreasureShow(true);
            }
            else
            {
                _SetOpenedTipItemShow(false);
                _SetTypeTreasureShow();
                bDirtyCD = false;
            }
        }
        
        //Unity life cycle
        void OnDestroy()
        {
            if (mFuncBtn)
            {
                mFuncBtn.onClick.RemoveListener(_OnFuncBtnClick);
                mFuncBtn = null;
            }
            mSiteModel = null;
            bDirtyCD = false;
            if (mTipItem != null)
            {
                ComItemManager.Destroy(mTipItem);
                mTipItem = null;
            }
            mTipItemData = null;
            if (mTipBtn)
            {
                mTipBtn.onClick.RemoveListener(_OnTipBtnClick);
                mTipBtn = null;
            }

            mParent = null;

            mTips_Treasure_IsOpened = "";
        }

        /// <summary>
        /// 根据类型设置宝箱 显示隐藏
        /// </summary>
        /// <param name="bNeedHide">是否需要强制隐藏</param>
        void _SetTypeTreasureShow(bool bForceHide = false)
        {
            if (mSiteModel == null)
            {
                return;
            }
            bool bOpen = false;
            switch (mSiteModel.status)
            {
                case Protocol.DigStatus.DIG_STATUS_OPEN:
                    bOpen = true;
                    break;
                case Protocol.DigStatus.DIG_STATUS_INIT:
                    bOpen = false;
                    break;
                case Protocol.DigStatus.DIG_STATUS_INVALID:
                    bOpen = false;
                    break;
            }
            if (mSiteModel.type == Protocol.DigType.DIG_GLOD)
            {
                _SetGoldBgShowByStatus(bOpen, bForceHide);
                //刷新其他类型状态
                _SetSilverBgShowByStatus(false, true);
            }
            else if (mSiteModel.type == Protocol.DigType.DIG_SILVER)
            {
                _SetSilverBgShowByStatus(bOpen, bForceHide);
                //刷新其他类型状态
                _SetGoldBgShowByStatus(false, true);
            }
        }

        void _SetParentGoShow(bool bShow)
        {
            if (mParent)
            {
                mParent.CustomActive(bShow);
            }
        }

        void _SetGoldBgShowByStatus(bool bOpen, bool bForceHide)
        {
            if (mGoldBg)
                mGoldBg.enabled = (!bOpen && !bForceHide);
            if (mGoldBg_Open)
                mGoldBg_Open.enabled = (bOpen && !bForceHide);
        }

        void _SetSilverBgShowByStatus(bool bOpen, bool bForceHide)
        {
            if (mSilverBg)
                mSilverBg.enabled = (!bOpen && !bForceHide);
            if (mSilverBg_Open)
                mSilverBg_Open.enabled = (bOpen && !bForceHide);
        }

        void _SetOpenedTipItem(ItemSimpleData sData)
        {
            if (sData == null)
            {
                return;
            }
            if (mTipImg && mTipBtn)
            {
                mTipItemData = ItemDataManager.CreateItemDataFromTable(sData.ItemID);
                if (mTipItemData == null)
                {
                    Logger.LogError("[ComRandomTreasureSiteBtn] - _SetOpenedTipItem  Please check item data null");
                    return;
                }
                //itemData.Count = sData.Count;
                //不使用 ComItem
                //if (mTipItem == null)
                //{
                //    mTipItem = ComItemManager.Create(mTipImg.gameObject);
                //}
                //if (mTipItem != null)
                //{
                //    mTipItem.Setup(itemData, Utility.OnItemClicked);
                //} 
                ETCImageLoader.LoadSprite(ref mTipImg, mTipItemData.Icon);
            }
        }

        void _SetOpenedTipItemShow(bool bShow)
        {
            if (mTipImg)
            {
                mTipImg.enabled = bShow;
            }

            if (mTipBtn)
            {
                mTipBtn.interactable = bShow;
            }
        }

        void _OnFuncBtnClick()
        {
            if (mSiteModel != null)
            {
                if (mSiteModel.status == Protocol.DigStatus.DIG_STATUS_OPEN || 
                    mSiteModel.status == Protocol.DigStatus.DIG_STATUS_INVALID)
                {
                    Logger.LogProcessFormat("[ComRandomTreasureSiteBtn] - OnFuncBtnClick : SiteModel status is not init {0}", mSiteModel.status);
                    SystemNotifyManager.SysNotifyTextAnimation(mTips_Treasure_IsOpened);
                    return;
                }
                RandomTreasureDataManager.GetInstance().ReqWatchTreasureSite(mSiteModel);
                //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTreasureRaffleStart);
            }
            else
            {
                Logger.LogError("[ComRandomTreasureSiteBtn] - OnFuncBtnClick : SiteModel is null");
            }
        }

        void _OnTipBtnClick()
        {
            if (mTipItemData == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(mTipItemData);
        }
        
        #endregion
        
        #region  PUBLIC METHODS

        public void Refresh(RandomTreasureMapDigSiteModel siteModel , GameObject parent = null)
        {
            bDirtyCD = false;
            this.mSiteModel = siteModel;
            this.mParent = parent;
            if (siteModel == null)
            {
                return;
            }
            if (siteModel.type == Protocol.DigType.DIG_INVALID || siteModel.status == Protocol.DigStatus.DIG_STATUS_INVALID)
            {
                _SetParentGoShow(false);
                return;
            }
            else
            {
                _SetParentGoShow(true);
            }

            _SetOpenedTipItemShow(false);
            _SetTypeTreasureShow();

            mRefreshTime = siteModel.refreshTime;
            mChangeStatusTime = siteModel.changeStatusTime;
            mServerTime = TimeManager.GetInstance().GetServerTime();

            //特殊处理 是否需要展示打开的道具
            if (siteModel.status == Protocol.DigStatus.DIG_STATUS_OPEN)
            {
                if (mServerTime - mChangeStatusTime < mShowOpenedItemSecond ||
                    mServerTime - mRefreshTime < mShowOpenedItemSecond)
                {
                    _SetOpenedTipItem(siteModel.openItem);
                    this.bDirtyCD = true;
                }
            }
        }

        #endregion
    }
}