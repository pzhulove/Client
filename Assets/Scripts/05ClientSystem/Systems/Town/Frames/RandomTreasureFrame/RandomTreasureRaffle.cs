using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class RandomTreasureRaffle : ClientFrame
    {
        public const string RAFFLE_FRAME_PATH = "UIFlatten/Prefabs/RandomTreasureFrame/RandomTreasureRaffle";
        
        #region Model Params

        RandomTreasureMapDigSiteModel digSiteModel = null;
        ItemSimpleData titleShowItem = null;

        private string tr_raffle_must_require_desc = "开启后必定获得{0}个{1}";
        private string tr_raffle_gold_box_desc = "金宝箱";
        private string tr_raffle_silver_box_desc = "银宝箱";
        
        #endregion
        
        #region View Params

        #endregion
        
        #region PRIVATE METHODS

        protected override void _OnOpenFrame()
        {
            if (userData != null)
            {
                this.digSiteModel = userData as RandomTreasureMapDigSiteModel;
            }

            _InitView();
            _BindUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            _ClearData();
            _UnBindUIEvent();
        }

        public override string GetPrefabPath()
        {
            return RAFFLE_FRAME_PATH;
        }

        private void _ClearData()
        {
            digSiteModel = null;
            titleShowItem = null;

            tr_raffle_must_require_desc = "";
			tr_raffle_gold_box_desc = "";
			tr_raffle_silver_box_desc = "";

            if (mComUIGrids != null)
            {
                mComUIGrids.SetElementAmount(0);
            }
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnWatchRefreshDigSite, _OnWatchRefreshDigSite);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnOpenTreasureDigSite, _OnOpenTreasureDigSite);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureRaffleStart, _OnTreasureRaffleStart);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureRaffleEnd, _OnTreasureRaffleEnd);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnWatchRefreshDigSite, _OnWatchRefreshDigSite);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnOpenTreasureDigSite, _OnOpenTreasureDigSite);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureRaffleStart, _OnTreasureRaffleStart);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureRaffleEnd, _OnTreasureRaffleEnd);
        }

        private void _InitView()
        {
            if (mComUIGrids != null && mGridsBoard != null)
            {
                RectTransform rect = mComUIGrids.gameObject.GetComponent<RectTransform>();
                if (rect)
                {
                    rect.sizeDelta = new Vector2(mGridsBoard.GridContentSizeX, mGridsBoard.GridContentSizeY);
                }
                mComUIGrids.m_elementSpacing = new Vector2(mGridsBoard.mGridSpace, mGridsBoard.mGridSpace);
            }

            //if (mComUIFade != null)
            //{
            //    mComUIFade.InitView();
            //}

            if (mGridsBoard != null)
            {
                mGridsBoard.InitView();
            }

            _SetDigButtonEnable(true);
            _SetSkipAnimToggleActive();

            tr_raffle_must_require_desc = TR.Value("random_treasure_raffle_must_require_desc");
            tr_raffle_gold_box_desc = TR.Value("random_treasure_raffle_gold_box");
            tr_raffle_silver_box_desc = TR.Value("random_treasure_raffle_silver_box");
        }

        private void _ChangeDigSiteTypeIcon(Protocol.DigType digType)
        {
            bool bGoldDig = digType == Protocol.DigType.DIG_GLOD;
            bool bSilverDig = digType == Protocol.DigType.DIG_SILVER;
            if (mGoldTex)
            {
                mGoldTex.enabled = (bGoldDig && !bSilverDig);
            }
            if (mGoldBtnImg)
            {
                mGoldBtnImg.enabled = (bGoldDig && !bSilverDig);
            }
            if (mSilverTex)
            {
                mSilverTex.enabled = (!bGoldDig && bSilverDig);
            }
            if (mSilverBtnImg)
            {
                mSilverBtnImg.enabled = (!bGoldDig && bSilverDig);
            }
        }

        private void _ChangeDigSiteTypeName(Protocol.DigType digType)
        {
            if (mFrameTitle == null)
            {
                return;
            }
            if (digType == Protocol.DigType.DIG_GLOD)
            {
                mFrameTitle.text = tr_raffle_gold_box_desc;
            }
            else if (digType == Protocol.DigType.DIG_SILVER)
            {
                mFrameTitle.text = tr_raffle_silver_box_desc;
            }
            else {
                mFrameTitle.text = "宝箱";
            }
        }

        private void _SetDigButtonEnable(bool bEnable)
        {
            if (mDigBtn)
            {
                mDigBtn.enabled = bEnable;
            }
            if (mDigBtnGray)
            {
                mDigBtnGray.enabled = !bEnable;
            }
            if (mDigBtnEffui)
            {
                mDigBtnEffui.CustomActive(bEnable);
            }
        }

        private void _SetSkipAnimToggleActive()
        {
            bool bShow = false;
            if (digSiteModel != null && digSiteModel.type == Protocol.DigType.DIG_SILVER)
            {
                bShow = true;
            }
            if (mSkipAnimToggle)
            {
                mSkipAnimToggle.CustomActive(bShow);
                mSkipAnimToggle.isOn = RandomTreasureDataManager.GetInstance().BSilverRaffleSkipAnim;
            }
        }

        private void _RefreshData()
        {
            if (digSiteModel == null)
            {
                Logger.LogError("[RandomTreasureRaffle] - RefreshData digSiteModel is null");
                return;
            }
            if (digSiteModel.itemSDatas == null)
            {
                Logger.LogError("[RandomTreasureRaffle] - RefreshData digSiteModel.itemIds is null");
                return;
            }
            if (mComUIGrids == null)
            {
                Logger.LogError("[RandomTreasureRaffle] - RefreshData mComUIGrids is null");
                return;
            }
            var digSiteItems = digSiteModel.itemSDatas;
            if (mComUIGrids.IsInitialised() == false)
            {
                mComUIGrids.Initialize();
                mComUIGrids.onBindItem = (GameObject go) =>
                {
                    var gridInfo = go.GetComponent<RandomTreasureInfo>();
                    return gridInfo;
                };
            }
            mComUIGrids.onItemVisiable = (var) =>
            {
                if (var == null)
                {
                    return;
                }
                int iIndex = var.m_index;
                if (iIndex >= 0 && iIndex < digSiteItems.Count)
                {
                    ItemSimpleData sData = digSiteItems[iIndex];
                    if (sData == null)
                    {
                        return;
                    }
                    if (mGridsBoard == null)
                    {
                        return;
                    }
                    RandomTreasureInfo gridInfo = var.gameObjectBindScript as RandomTreasureInfo;

                    ItemData itemData = ItemDataManager.CreateItemDataFromTable(sData.ItemID);
                    gridInfo.SetInfoTitleImg(itemData.Icon);
                    gridInfo.onTitleBtnClick = () =>
                    {
                        ItemTipManager.GetInstance().ShowTip(itemData);
                    };
                }
            };
            mComUIGrids.SetElementAmount(digSiteItems.Count);


            //刷新标题描述
            if (mTitleInfo == null)
            {
                return;
            }
            if (digSiteModel.type == Protocol.DigType.DIG_GLOD)
            {
                titleShowItem = RandomTreasureDataManager.GetInstance().GoldRaffleMustGetItem;

            }
            else if (digSiteModel.type == Protocol.DigType.DIG_SILVER)
            {
                titleShowItem = RandomTreasureDataManager.GetInstance().SilverRaffleMustGetItem;
            }
            _ChangeDigSiteTypeIcon(digSiteModel.type);
            _ChangeDigSiteTypeName(digSiteModel.type);
            if (titleShowItem != null)
            {
                mTitleInfo.SetInfoContent(string.Format(tr_raffle_must_require_desc, titleShowItem.Count, titleShowItem.Name));
                mTitleInfo.CustomActive(true);
            }
            else
            {
                mTitleInfo.CustomActive(false);
            }
        }

        /// <summary>
        /// 播放抽奖动画
        /// </summary>
        /// <param name="itemIndex"> 抽中的奖励道具序号 </param>
        private void _StartPlayRaffleAnim(int itemIndex, int itemId, int itemCount, bool bSkipAnim = false)
        {
            //Logger.LogErrorFormat("[RandomTreasureRaffle] - StartPlayRaffleAnim itemIndex is {0}",itemIndex);
            if (mGridsBoard != null)
            {
                List<ItemSimpleData> otherShowItemDatas = new List<ItemSimpleData>();
                if (titleShowItem != null)
                {
                    otherShowItemDatas.Add(titleShowItem);
                }
                mGridsBoard.StartSelectAnim(itemIndex, itemId, itemCount, otherShowItemDatas, bSkipAnim);
            }
        }

        #region Callback

        private void _OnWatchRefreshDigSite(UIEvent uiEvent)
        {
            if (uiEvent.Param1 == null)
            {
                return;
            }
            var model = uiEvent.Param1 as RandomTreasureMapDigSiteModel;
            if (model != null)
            {
                this.digSiteModel = model;
                _RefreshData();
            }
        }

        private void _OnOpenTreasureDigSite(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }
            if (uiEvent.Param1 == null || uiEvent.Param2 == null || uiEvent.Param3 == null)
            {
                return;
            }
            int itemIndex = (int)uiEvent.Param1;
            int itemId = (int)uiEvent.Param2;
            int itemCount = (int)uiEvent.Param3;

            bool bSkipAnim = false;
            if (digSiteModel != null && digSiteModel.type == Protocol.DigType.DIG_SILVER)
            {
                bSkipAnim = RandomTreasureDataManager.GetInstance().BSilverRaffleSkipAnim;
            }
            _StartPlayRaffleAnim(itemIndex, itemId, itemCount, bSkipAnim);
        }

        private void _OnTreasureRaffleStart(UIEvent uiEvent)
        {
            _SetDigButtonEnable(false);
        }

        private void _OnTreasureRaffleEnd(UIEvent uiEvent)
        {
            _SetDigButtonEnable(true);
            this.Close();
        }

        #endregion

        #endregion

        #region ExtraUIBind
        private Button mMaskClose = null;
        private ComRandomTreasureUIFade mComUIFade = null;
        private ComUIListScript mComUIGrids = null;
        private ComRandomTreasureRaffleBoard mGridsBoard = null;
        private Button mDigBtn = null;
        private UIGray mDigBtnGray = null;
        private Image mGoldBtnImg = null;
        private Image mSilverBtnImg = null;
        private SetComButtonCD mDigBtnCD = null;
        private GameObject mDigBtnEffui = null;
        private Image mSilverTex = null;
        private Image mGoldTex = null;
        private RandomTreasureInfo mTitleInfo = null;
        private Text mFrameTitle = null;
        private Toggle mSkipAnimToggle = null;

        protected override void _bindExUI()
        {
            mMaskClose = mBind.GetCom<Button>("MaskClose");
            mMaskClose.onClick.AddListener(_onMaskCloseButtonClick);
            mComUIFade = mBind.GetCom<ComRandomTreasureUIFade>("ComUIFade");
            mComUIGrids = mBind.GetCom<ComUIListScript>("ComUIGrids");
            mGridsBoard = mBind.GetCom<ComRandomTreasureRaffleBoard>("GridsBoard");
            mDigBtn = mBind.GetCom<Button>("DigBtn");
            mDigBtn.onClick.AddListener(_onDigBtnButtonClick);
            mDigBtnGray = mBind.GetCom<UIGray>("DigBtnGray");
            mGoldBtnImg = mBind.GetCom<Image>("goldBtnImg");
            mSilverBtnImg = mBind.GetCom<Image>("silverBtnImg");
            mDigBtnCD = mBind.GetCom<SetComButtonCD>("DigBtnCD");
            mDigBtnEffui = mBind.GetGameObject("digBtnEffui");
            mSilverTex = mBind.GetCom<Image>("silverTex");
            mGoldTex = mBind.GetCom<Image>("goldTex");
            mTitleInfo = mBind.GetCom<RandomTreasureInfo>("TitleInfo");
            mFrameTitle = mBind.GetCom<Text>("FrameTitle");
            mSkipAnimToggle = mBind.GetCom<Toggle>("SkipAnimToggle");
            if (null != mSkipAnimToggle)
            {
                mSkipAnimToggle.onValueChanged.AddListener(_onSkipAnimToggleToggleValueChange);
            }
        }

        protected override void _unbindExUI()
        {
            mMaskClose.onClick.RemoveListener(_onMaskCloseButtonClick);
            mMaskClose = null;
            mComUIFade = null;
            mComUIGrids = null;
            mGridsBoard = null;
            mDigBtn.onClick.RemoveListener(_onDigBtnButtonClick);
            mDigBtn = null;
            mDigBtnGray = null;
            mGoldBtnImg = null;
            mSilverBtnImg = null;
            mDigBtnCD = null;
            mDigBtnEffui = null;
            mSilverTex = null;
            mGoldTex = null;
            mTitleInfo = null;
            mFrameTitle = null;
            if (null != mSkipAnimToggle)
            {
                mSkipAnimToggle.onValueChanged.RemoveListener(_onSkipAnimToggleToggleValueChange);
            }
            mSkipAnimToggle = null;
        }
        #endregion   

        #region Callback
        private void _onMaskCloseButtonClick()
        {
            /* put your code in here */
            if (mGridsBoard != null && mGridsBoard.GridSelectAnimPlaying)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("randrom_treasure_raffle_anim_playing"));
                return;
            }
            this.Close();
        }

        private void _onDigBtnButtonClick()
        {
            if (mDigBtnCD == null || mDigBtnCD.IsBtnWork() == false)
            {
                return;
            }
            /* put your code in here */
            if (digSiteModel != null)
            {
                int usedItemId = RandomTreasureDataManager.GetInstance().Gold_Treasure_Item_Id;
                if (digSiteModel.type == Protocol.DigType.DIG_GLOD)
                {
                    usedItemId = RandomTreasureDataManager.GetInstance().Gold_Treasure_Item_Id;
                }
                else if (digSiteModel.type == Protocol.DigType.DIG_SILVER)
                {
                    usedItemId = RandomTreasureDataManager.GetInstance().Silver_Treasure_Item_Id;
                }

                int mOwnedItemCount = ItemDataManager.GetInstance().GetOwnedItemCount(usedItemId);
                int mNeedCount = 1;
                if (mOwnedItemCount < mNeedCount)
                {
                    RandomTreasureDataManager.GetInstance().ReqFastMallBuy(usedItemId);
                }
                else
                {
                    RandomTreasureDataManager.GetInstance().ReqOpenTreasureDigSite(digSiteModel);
                }

                if (mDigBtnCD != null)
                {
                    mDigBtnCD.StartBtCD();
                }
            }
        }

        private void _onSkipAnimToggleToggleValueChange(bool changed)
        {
            RandomTreasureDataManager.GetInstance().BSilverRaffleSkipAnim = changed;
        }

        #endregion
    }
}