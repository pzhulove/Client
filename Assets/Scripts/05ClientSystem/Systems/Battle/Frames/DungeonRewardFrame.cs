using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Protocol;
using Network;
using System.Collections;
using System.Collections.Generic;
using ActivityLimitTime;
using ProtoTable;
using DG.Tweening;

namespace GameClient
{
    [LoggerModel("Chapter")]
    public class DungeonRewardFrame : ClientFrame
    {
        private enum eType 
        {
            None,
            Normal,
            Another,
            Vip,
            AnotherWorld,
        }

        /// <summary>
        /// 另一个牌翻开类型
        /// </summary>
        private enum eAnotherCardFlipType
        {
            /// <summary>
            /// 非法
            /// </summary>
            None,

            /// <summary>
            /// Vip免费翻牌
            /// </summary>
            VipFree,

            /// <summary>
            /// 点券翻牌
            /// </summary>
            TicketCost,

            /// <summary>
            /// 金币翻牌
            /// </summary>
            GoldCost,

            /// <summary>
            /// 没有消耗的金币
            /// </summary>
            NotCostGoldItem,

            /// <summary>
            /// 没有消耗的点券
            /// </summary>
            NotCostTicketItem,

            /// <summary>
            /// 还没有开启
            /// </summary>
            NotOpen,
        }

        private enum eFlipProcess
        {
            None,
            WaitFlip,
            WaitClose,
            Finish
        }

        private DungeonTable.eCardType _mCurCarType = DungeonTable.eCardType.Golden_Card;
        public DungeonTable.eCardType mCurCarType
        {
            get { return _mCurCarType; }
            set { _mCurCarType = value; }
        }

        private eFlipProcess _mCurprocess = eFlipProcess.None;

        private eFlipProcess mCurProcessStatus 
        {
            get
            {
                return _mCurprocess;
            }

            set
            {
                _mCurprocess = value;
                UnityEngine.Debug.LogFormat("[翻牌] 状态 {0}", _mCurprocess);
            }
        }

        private Button[] mNormalRewardList = new Button[kRewardButtonCount];
        private Button[] mAnotherRewardList = new Button[kRewardButtonCount];
        private Text[] mAnotherRewardCostList = new Text[kRewardButtonCount];
        private UIPrefabWrapper[] mAnotherRewardWrapList = new UIPrefabWrapper[kRewardButtonCount];
        private Image[] mAnotherRewardCostIconList = new Image[kRewardButtonCount];
        int number = 0; // number 表示翻牌福利还剩几次
        private Button[] mAnotherWorldRewardList = new Button[kRewardButtonCount];
        private GameObject[] mYijieKamianhouEffect = new GameObject[kRewardButtonCount];
        private GameObject[] mYijieKamianqianEffect = new GameObject[kRewardButtonCount];

#region ExtraUIBind
        private ComCountScript mCount10 = null;
        private HorizontalLayoutGroup mNormalGroup = null;
        private UIGray mAnotherGrayGroup = null;
        private Text mTip = null;
        //private GameObject mNoVipRoot = null;
        private Button mClose = null;

        private GameObject mNormalRoot = null;
        private GameObject mVipRoot = null;
        private Text mVipLevel = null;
        private Text mVipLeft = null;
        private Text mVipSum = null;
        private Text mCostTypeName = null;
        private Text mHellCostTypeName = null;
        private GameObject mHellRoot = null;
        private GameObject mCentTextRoot = null;

        private GameObject mNotOpenRoot = null;
        private Text mNotOpenLevel = null;
        private GameObject mCountRoot = null;

        private Button[] mNormalVipRewards = new Button[kRewardButtonCount];
        private CanvasGroup mInfomationCanvasGroup = null;
        private GameObject mFatigueCombustionRoot = null;
        private UIPrefabWrapper mCrossServiceFlipCardNumberWrapper = null;
        private GameObject mCrossServiceFlipCardNumberRoot = null;
        private Text mCount = null;
        private GameObject mYijieLabelRoot = null;
        private GameObject mMidRoot = null;
        private GameObject mYijieRoot = null;
        private GameObject mmAnotherWorldCardRoot = null;
        private GameObject mGoldCardRoot = null;
        private GameObject mTitleRoot = null;
        private GameObject mYiJieTitleRoot = null;

        protected override void _bindExUI()
        {
            mCount10 = mBind.GetCom<ComCountScript>("count10");
            mTip = mBind.GetCom<Text>("Tip");
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);

            mCountRoot = mBind.GetGameObject("CountRoot");

            mFatigueCombustionRoot = mBind.GetGameObject("FatigueCombustionRoot");
            mCrossServiceFlipCardNumberWrapper = mBind.GetCom<UIPrefabWrapper>("CrossServiceFlipCardNumberRoot");
           
            mMidRoot = mBind.GetGameObject("MidRoot");
            mYijieRoot = mBind.GetGameObject("YijieRoot");
            mTitleRoot = mBind.GetGameObject("Title");
            mYiJieTitleRoot = mBind.GetGameObject("YiJieTitle");
        }

        protected override void _unbindExUI()
        {
            mCount10 = null;
            mNormalGroup = null;
            mAnotherGrayGroup = null;
            if (mNormalRewardList[0] != null)
            {
                mNormalRewardList[0].onClick.RemoveListener(_onNormalReward0ButtonClick);
                mNormalRewardList[0] = null;
            }
            if (mNormalRewardList[1] != null)
            {
                mNormalRewardList[1].onClick.RemoveListener(_onNormalReward1ButtonClick);
                mNormalRewardList[1] = null;
            }
            if (mNormalRewardList[2] != null)
            {
                mNormalRewardList[2].onClick.RemoveListener(_onNormalReward2ButtonClick);
                mNormalRewardList[2] = null;
            }
            if (mAnotherRewardList[0] != null)
            {
                mAnotherRewardList[0].onClick.RemoveListener(_onAnotherReward0ButtonClick);
                mAnotherRewardList[0] = null;
            }
            if (mAnotherRewardList[1] != null)
            {
                mAnotherRewardList[1].onClick.RemoveListener(_onAnotherReward1ButtonClick);
                mAnotherRewardList[1] = null;
            }
            if (mAnotherRewardList[2] != null)
            {
                mAnotherRewardList[2].onClick.RemoveListener(_onAnotherReward2ButtonClick);
                mAnotherRewardList[2] = null;
            }
            mAnotherRewardWrapList[0] = null;
            mAnotherRewardWrapList[1] = null;
            mAnotherRewardWrapList[2] = null;
            mAnotherRewardCostList[0] = null;
            mAnotherRewardCostList[1] = null;
            mAnotherRewardCostList[2] = null;
            mTip = null;
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;

            mNormalRoot = null; 
            mVipRoot    = null;
            mVipLevel   = null;            
            mVipLeft    = null;                
            mVipSum     = null;

            mCostTypeName = null;

            mAnotherRewardCostIconList[0] = null;
            mAnotherRewardCostIconList[1] = null;
            mAnotherRewardCostIconList[2] = null;

            mHellCostTypeName = null;
            mHellRoot = null;
            mCentTextRoot = null;

            mNotOpenRoot = null;
            mNotOpenLevel = null;
            mCountRoot = null;

            if (mNormalVipRewards[0] != null)
            {
                mNormalVipRewards[0].onClick.RemoveListener(_onNormalVipReward0ButtonClick);
                mNormalVipRewards[0] = null;
            }

            if (mNormalVipRewards[1] != null)
            {
                mNormalVipRewards[1].onClick.RemoveListener(_onNormalVipReward1ButtonClick);
                mNormalVipRewards[1] = null;
            }

            if (mNormalVipRewards[2] != null)
            {
                mNormalVipRewards[2].onClick.RemoveListener(_onNormalVipReward2ButtonClick);
                mNormalVipRewards[2] = null;
            }

            mInfomationCanvasGroup = null;
            mFatigueCombustionRoot = null;
            mCrossServiceFlipCardNumberWrapper = null;
            mCrossServiceFlipCardNumberRoot = null;
            mCount = null;

            if (mAnotherWorldRewardList[0] != null)
            {
                mAnotherWorldRewardList[0].onClick.RemoveListener(_onAnotherWorldReward0ButtonClick);
                mAnotherWorldRewardList[0] = null;
            }
            if (mAnotherWorldRewardList[1] != null)
            {
                mAnotherWorldRewardList[1].onClick.RemoveListener(_onAnotherWorldReward1ButtonClick);
                mAnotherWorldRewardList[1] = null;
            }
            if (mAnotherWorldRewardList[2] != null)
            {
                mAnotherWorldRewardList[2].onClick.RemoveListener(_onAnotherWorldReward2ButtonClick);
                mAnotherWorldRewardList[2] = null;
            }
            mMidRoot = null;
            mmAnotherWorldCardRoot = null;
            mGoldCardRoot = null;
            mYijieKamianhouEffect[0] = null;
            mYijieKamianhouEffect[1] = null;
            mYijieKamianhouEffect[2] = null;
            mYijieKamianqianEffect[0] = null;
            mYijieKamianqianEffect[1] = null;
            mYijieKamianqianEffect[2] = null;
            mTitleRoot = null;
            mYiJieTitleRoot = null;
            mYijieRoot = null;
        }
#endregion

#region Callback
        private void _onNormalReward0ButtonClick()
        {
            /* put your code in here */
            _onClickReward(0);

        }
        private void _onNormalReward1ButtonClick()
        {
            /* put your code in here */
            _onClickReward(1);
        }
        private void _onNormalReward2ButtonClick()
        {
            /* put your code in here */
            _onClickReward(2);
        }
        private void _onNormalReward3ButtonClick()
        {
            /* put your code in here */
            _onClickReward(3);
        }
        private void _onAnotherReward0ButtonClick()
        {
            /* put your code in here */
            _onClickAnotherReward(0);
        }
        private void _onAnotherReward1ButtonClick()
        {
            /* put your code in here */
            _onClickAnotherReward(1);
        }
        private void _onAnotherReward2ButtonClick()
        {
            /* put your code in here */
            _onClickAnotherReward(2);
        }

        private void _RestartAni(Button button, UnityAction callBack, bool isOnlyPlayAni = false) 
        {
            if (button == null)
            {
                return;
            }

            DOTweenAnimation ani = button.GetComponent<DOTweenAnimation>();
            if (ani == null)
            {
                if (callBack != null)
                {
                    callBack();
                }
            }
            else
            {
                if (!isOnlyPlayAni)
                {
                    ani.tween.onComplete = () => { callBack(); };
                    ani.CreateTween();
                }

                ani.DORestart();
            }
        }

        private void _onAnotherReward3ButtonClick()
        {
            /* put your code in here */
            _onClickAnotherReward(3);
        }
        private void _onCloseButtonClick()
        {
            /* put your code in here */
            //_onHandle();
            CloseTimeCount();
            _timeOutClick();
        }

        private void CloseTimeCount()
        {
            mCount10.PauseCount();
            mCount10.CustomActive(false);
        }

        private void _onNormalVipReward0ButtonClick()
        {
            /* put your code in here */
            //_onClickReward(0);
            _showNoneVipTips();
        }
        private void _onNormalVipReward1ButtonClick()
        {
            /* put your code in here */
            //_onClickReward(1);
            _showNoneVipTips();
        }
        private void _onNormalVipReward2ButtonClick()
        {
            /* put your code in here */
            //_onClickReward(2);
            _showNoneVipTips();
        }
        private void _onNormalVipReward3ButtonClick()
        {
            /* put your code in here */
            //_onClickReward(3);
            _showNoneVipTips();
        }
        private void _onAnotherWorldReward0ButtonClick()
        {
            _onAnotherWorldClickReward(0);
        }
        private void _onAnotherWorldReward1ButtonClick()
        {
            _onAnotherWorldClickReward(1);
        }
        private void _onAnotherWorldReward2ButtonClick()
        {
            _onAnotherWorldClickReward(2);
        }
        private void _onAnotherWorldReward3ButtonClick()
        {
            _onAnotherWorldClickReward(3);
        }
        #endregion

        private void _showNoneVipTips()
        {
            if (!_isLocalPlayerMonthCard())
            {
                SystemNotifyManager.SystemNotify(1248);
            }
        }

        private bool _isLocalPlayerMonthCard()
        {
            if (null != BattleMain.instance && null != BattleMain.instance.GetLocalPlayer())
            {
                return BattleMain.instance.GetLocalPlayer().IsPlayerMonthCard();
            }

            return false;
        }


        private const string kOpenFrameSoundPath = "Sound/SE/reward_divide";

        private const int kDefaultSelectedIdx = -1;
        private const int kRewardButtonCount = 3;
		private const int kTotalRewardButtonCount = kRewardButtonCount * 3;
        private const int kAnotherRewardButtonOffset = 3;

        /// <summary>
        /// 月卡翻牌偏移
        /// </summary>
        private const int kVipNormalRewardButtonOffset = 6;

        private float kCloseWaitTime = 1.5f;

        private const string kEffectPath = "Effects/Scene_effects/EffectUI/chouka/EffUI_chouka_kapian";
        private const string kGoldCardEffectPath = "Effects/Scene_effects/EffectUI/chouka/EffUI_chouka_kapian02";
        private const string kAnotherWorldEffectPath = "Effects/Scene_effects/EffectUI/chouka/EffUI_chouka_kapian_yijie";
        private const string kIconEffectPath = "Effects/Scene_effects/EffectUI/chouka/EffUI_chouka_iconchuxian";
        private const string kEffectHuisePath = "Effects/Scene_effects/EffectUI/chouka/EffUI_chouka_kapian_huise";

        private const bool kLocalTest =
#if UNITY_EDITOR
        false;
#else
        false;
#endif
        private bool mIsGetNormalReward = false;
        private bool mIsGetAnotherReward = false;
        private bool mIsGetAnotherWorldReward = false;

        private bool mIsRealGetNormalReward = false;
        private bool mIsRealGetAnotherReward = false;
        private bool mIsAnotherWorldRealGetReward = false;

		private int mNormalClickIndex = -1;
		private int mAnotherClickIndex = -1;
        private int mAnotherWorldClickIndex = -1;

        private bool mIsClosed = false;
        private bool mIsGuide = false;

		private bool[] isCardOpened = new bool[kTotalRewardButtonCount];

		int openChestCount = 0;
		int normalChestCount = 0;
		int goldChestCount = 0;
		bool countTimeOut = false;
		bool[] opened = new bool[kTotalRewardButtonCount];

        #region Override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Battle/Reward/DungeonReward";
        }

        protected override bool _isLoadFromPool()
        {
            return true;
        }

        protected override void _OnOpenFrame()
        {
			if (ClientSystemManager.instance.IsFrameOpen<DungeonRebornFrame>())
			{
				ClientSystemManager.instance.CloseFrame<DungeonRebornFrame>();
			}

            _bindEvent();

            if (!_bindCountCallback())
            {
                return;
            }

            _updateAnotherFlipType();

            mIsGetAnotherWorldReward = false;
            mIsAnotherWorldRealGetReward = false;
            mIsRealGetAnotherReward = false;
            mIsRealGetNormalReward = false;
            mIsGetAnotherReward = false;
            mIsGetNormalReward = false;
            mIsClosed = false;

            mCurProcessStatus = eFlipProcess.WaitFlip;

            GameStatisticManager.instance.DoStatInBattleEx(StatInBattleType.CLICK_CARD,
                    _GetDungeonIdWithOutDiff(false),
                    BattleMain.instance.GetDungeonManager().GetDungeonDataManager().CurrentAreaID(),
                    "");

            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRewardFrameOpen);

            mFatigueCombustionRoot.CustomActive(ActivityLimitTimeCombineManager.GetInstance().LimitTimeManager.fatigueBurnType == (int)FatigueBurnType.FBT_ADVANCED);   //精力燃烧

            if (mCrossServiceFlipCardNumberWrapper != null)
            {
                mCrossServiceFlipCardNumberWrapper.SetCallback(_SetCrossServiceFlipCardNumCount);
            }

            if (_ismCrossServiceFlipCardNumberRoot())
            {
                if (mCrossServiceFlipCardNumberRoot != null)
                {
                    mCount.SafeSetText(number.ToString());
                }
                mCrossServiceFlipCardNumberWrapper.CustomActive(true);
            }
            
        }

        private void _SetCrossServiceFlipCardNumCount(string path, object asset, object userData)
        {
            if (asset == null)
            {
                return;
            }

            mCrossServiceFlipCardNumberRoot = asset as GameObject;
            ComCommonBind bind = mCrossServiceFlipCardNumberRoot.GetComponent<ComCommonBind>();
            if (bind != null)
            {
                mCount = bind.GetCom<Text>("count");
                mCount.SafeSetText(number.ToString());
            }
        }

        protected override void _OnCloseFrame()
        {
            _unbindEvent();

            if (null != mNormalVIPSelect)
            {
                GameObject.Destroy(mNormalVIPSelect);
                mNormalVIPSelect = null;
            }


            if (null != mNormalSelect)
            {
                GameObject.Destroy(mNormalSelect);
                mNormalSelect = null;
            }

            if (null != mAnotherSelect)
            {
                GameObject.Destroy(mAnotherSelect);
                mAnotherSelect = null;
            }

            if (null != mTimeOut) { GameFrameWork.instance.StopCoroutine(mTimeOut); }

            _stopDelayClose();

            number = 0;
        }

        /// <summary>
        /// 设置金币卡牌图标
        /// </summary>
        //private void _setAnotherFlipSprite(Sprite sp)
        //{
        //    for (int i = 0; i < kRewardButtonCount; ++i)
        //    {
        //        mAnotherRewardImageList[i].sprite = sp;
        //    }
        //}
        private void _setAnotherFlipSprite(string spriteName)
        {
            for (int i = 0; i < kRewardButtonCount; ++i)
            {
                // mAnotherRewardImageList[i].sprite = sp;
                mBind.GetSprite(spriteName, ref mAnotherRewardImageList[i]);
            }
        }

        /// <summary>
        /// 设置金币卡牌消耗图标
        /// </summary>
        //private void _setAnotherFlipCostSprite(Sprite sp)
        //{
        //    for (int i = 0; i < kRewardButtonCount; ++i)
        //    {
        //        mAnotherRewardCostIconList[i].sprite = sp;
        //    }
        //}
        private void _setAnotherFlipCostSprite(string spriteName)
        {
            for (int i = 0; i < kRewardButtonCount; ++i)
            {
                // mAnotherRewardCostIconList[i].sprite = sp;
                ETCImageLoader.LoadSprite(ref mAnotherRewardCostIconList[i], spriteName);
            }
        }

        private void _setAnotherFlipCostIconActive(bool isActive)
        {
            for (int i = 0; i < kRewardButtonCount; ++i)
            {
                mAnotherRewardCostIconList[i].CustomActive(isActive);
            }
        }

        /// <summary>
        /// 设置金币卡牌消耗描述
        /// </summary>
        private void _setAnotherFlipText(string text)
        {
            for (int i = 0; i < kRewardButtonCount; ++i)
            {
                mAnotherRewardCostList[i].text = text;
            }
        }

        /// <summary>
        /// 设置金币卡牌是否开启
        /// </summary>
        private void _setAnotherFlipEnable(bool enable)
        {
            mAnotherGrayGroup.enabled = !enable;

            for (int i = 0; i < kRewardButtonCount; ++i)
            {
                mAnotherRewardImageList[i].raycastTarget = enable;
                mAnotherRewardList[i].interactable = enable;

                if (enable)
                {
                    mAnotherRewardWrapList[i].Load();
                }
            }
        }

        /// <summary>
        /// 更新金币卡牌类型
        /// </summary>
        private void _updateAnotherFlipType()
        {
            _InitCard2Card3LabelRootIsShow();
        }
       
        private void _updateVipRootInfo()
        {
            int left = _getVipGoldLeft();
            int sum = _getVipGoldSum();
            mVipRoot.SetActive(true);
            mVipLeft.text = left.ToString();
            mVipSum.text = sum.ToString();
            mVipLevel.text = PlayerBaseData.GetInstance().VipLevel.ToString();
        }

        private void _bindEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _updateVipLeftCount);

            NetProcess.AddMsgHandler(SceneDungeonOpenChestRes.MsgID, _onSceneDungeonOpenChestResDATA);

        }

        private void _unbindEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _updateVipLeftCount);

            NetProcess.RemoveMsgHandler(SceneDungeonOpenChestRes.MsgID, _onSceneDungeonOpenChestResDATA); 
        }



        private void _updateVipLeftCount(UIEvent ui)
        {
            string key = ui.Param1 as string;

            if (key == CounterKeys.COUNTER_VIP_FREE_GOLD_CHEST_TIMES)
            {
                _updateVipRootInfo();
            }
        }
        #endregion

#region GetFlipType
        private eAnotherCardFlipType _getAnotherFlipType()
        {
            if (BeUtility.CheckDungeonIsLimitTimeHell())
            {
                return eAnotherCardFlipType.GoldCost;
            }

            eAnotherCardFlipType type = eAnotherCardFlipType.None;
            if (!_isAnotherFlipOpen())
            {
                type = eAnotherCardFlipType.NotOpen;
            }
            else 
            {
                if (_isHellMode())
                {
                    if (_canUseCostItem2FlipAnother())
                    {
                        type = eAnotherCardFlipType.TicketCost;
                    }
                    else 
                    {
                        type = eAnotherCardFlipType.NotCostTicketItem;
                    }
                }
                else 
                {
                    if (_getVipGoldLeft() > 0)
                    {
                        type = eAnotherCardFlipType.VipFree;
                    }
                    else if (_canUseCostItem2FlipAnother())
                    {
                        type = eAnotherCardFlipType.GoldCost;
                    }
                    else
                    {
                        type = eAnotherCardFlipType.NotCostGoldItem;
                    }
                }
            }
             
            return type;
        }

        private bool _canFlipAnotherFlipType()
        {
            eAnotherCardFlipType type = _getAnotherFlipType();

            return type == eAnotherCardFlipType.GoldCost ||
                type == eAnotherCardFlipType.TicketCost ||
                type == eAnotherCardFlipType.VipFree;
        }

        /// <summary>
        /// 当前是否深渊模式
        /// </summary>
        private bool _isHellMode()
        {
            return BattleMain.battleType == BattleType.Hell;
        }

        private bool _canUseCostItem2FlipAnother()
        {
            int cost = (int)BattleDataManager.GetInstance().chestInfo.count;
            int id = (int)BattleDataManager.GetInstance().chestInfo.itemId;

            return (ItemDataManager.GetInstance().GetOwnedItemCount(id) >= cost);
        }

        /// <summary>
        /// 翻牌开启等级
        /// </summary>
        private int _getAnotherFlipOpenLevel()
        {
            // TODO 中功能解锁表中获取相应开启等级
            return 10;
        }

        /// <summary>
        /// 黄金翻牌功能是否开启
        /// </summary>
        private bool _isAnotherFlipOpen()
        {
            return (PlayerBaseData.GetInstance().Level >= _getAnotherFlipOpenLevel());
        }

        /// <summary>
        /// 总vip翻牌数目
        /// </summary>
        private int _getVipGoldSum()
        {
            float v = Utility.GetCurVipLevelPrivilegeData(ProtoTable.VipPrivilegeTable.eType.GOLDBOX_FREEOPEN_NUM);
            if (v > 0.0f)
            {
                return (int)v;
            }

            return 0;
        }

        /// <summary>
        /// 剩余vip翻牌数目
        /// </summary>
        private int _getVipGoldLeft()
        {
            int num = _getVipGoldSum();

            if (num <= 0)
            {
                return -1;
            }
            else 
            {
                return (int)(num) - CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_VIP_FREE_GOLD_CHEST_TIMES);
            }
        }
#endregion

        private Image[] mNormalRewardImageList = new Image[kRewardButtonCount];
        private Image[] mNormalRewardVIPImageList = new Image[kRewardButtonCount];
        private Image[] mAnotherRewardImageList = new Image[kRewardButtonCount];
        private Image[] mAnotherWorldRewardImageList = new Image[kRewardButtonCount];

        private GameObject mNormalSelect = null;
        private GameObject mNormalVIPSelect = null;
        private GameObject mAnotherSelect = null;
        private GameObject mAnotherWorldSelect = null;

        private void _startNormalVIPSelect(int idx, UnityAction callback, BattlePlayer player = null)
        {
            for (int i = 0; i < kRewardButtonCount; i++)
            {
                if (idx == i)
                {
                    mNormalRewardVIPImageList[i].color = Color.clear;

                    _hiddenAllChild(mNormalRewardVIPImageList[i].gameObject);

                    string sEffectPath = string.Empty;

                    if (player != null)
                    {
                        if (player.IsPlayerMonthCard())
                        {
                            sEffectPath = kEffectPath;
                        }
                        else
                        {
                            sEffectPath = kEffectHuisePath;
                        }
                    }
                    //mNormalVIPSelect = AssetLoader.instance.LoadResAsGameObject(sEffectPath);
                    //Utility.AttachTo(mNormalVIPSelect, mNormalRewardVIPImageList[i].gameObject);
                    //mNormalVIPSelect.GetComponent<ComAnimatorAutoClose>().mOnFinishCallback.RemoveAllListeners();
                    //mNormalVIPSelect.GetComponent<ComAnimatorAutoClose>().mOnFinishCallback.AddListener(callback);
                    if (callback != null)
                    {
                        callback();
                    }
                }
                else
                {
					if (mIsRealGetNormalReward)
                    	mNormalRewardVIPImageList[i].raycastTarget = false;
                }
            }
        }

        private void _hiddenAllChild(GameObject root)
        {
            if (null == root)
            {
                return ;
            }

            int cnt = root.transform.childCount;
            for (int j = 0; j < cnt; ++j)
            {
                Transform transform = root.transform.GetChild(j);
                if (null != transform && null != transform.gameObject)
                {
                    transform.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 异界选择卡牌
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="callback"></param>
        private void _startAnotherWorldSelect(int idx,UnityAction callback)
        {
            for (int i = 0; i < kRewardButtonCount; i++)
            {
                if (idx == i)
                {
                    if (mAnotherWorldRewardImageList[i] == null)
                    {
                        continue;
                    }

                    mAnotherWorldRewardImageList[i].color = Color.clear;

                    _hiddenAllChild(mAnotherWorldRewardImageList[i].gameObject);
                    setEffectAnotherWorldBtn(idx);
                    //mAnotherWorldSelect = AssetLoader.instance.LoadResAsGameObject(kAnotherWorldEffectPath);
                    //Utility.AttachTo(mAnotherWorldSelect, mAnotherWorldRewardImageList[i].gameObject);
                    //mAnotherWorldSelect.GetComponent<ComAnimatorAutoClose>().mOnFinishCallback.RemoveAllListeners();
                    //mAnotherWorldSelect.GetComponent<ComAnimatorAutoClose>().mOnFinishCallback.AddListener(callback);

                    if (callback != null)
                    {
                        callback();
                    }
                }
            }
        }

		private void _startNormalSelect(int idx, UnityAction callback)
        {
            for (int i = 0; i < kRewardButtonCount; i++)
            {
                if (idx == i)
                {
                    //mNormalSelect = AssetLoader.instance.LoadResAsGameObject(kEffectPath);
                    //Utility.AttachTo(mNormalSelect, mNormalRewardImageList[i].gameObject);
                    //mNormalSelect.GetComponent<ComAnimatorAutoClose>().mOnFinishCallback.RemoveAllListeners();
                    //mNormalSelect.GetComponent<ComAnimatorAutoClose>().mOnFinishCallback.AddListener(callback);
                    mNormalRewardImageList[idx].color = Color.clear;
                    _hiddenAllChild(mNormalRewardImageList[idx].gameObject);
                    //callback = _AddNormalCallback(i, callback);
                    //_RestartAni(mNormalRewardList[i], callback);
                    if (callback != null)
                    {
                        callback();
                    }
                }
                else
                {
					if (mIsRealGetNormalReward)
                    	mNormalRewardImageList[i].raycastTarget = false;
                }
            }
        }

        private void _startAnotherSelect(int idx, UnityAction callback)
        {
            Logger.LogProcessFormat("another with idx {0}", idx);

            for (int i = 0; i < kRewardButtonCount; i++)
            {
                if (idx == i)
                {

                    //mAnotherSelect = AssetLoader.instance.LoadResAsGameObject(kEffectPath);
                    //Utility.AttachTo(mAnotherSelect, mAnotherRewardImageList[i].gameObject);
                    //if (callback != null)
                    //{
                    //    mAnotherSelect.GetComponent<ComAnimatorAutoClose>().mOnFinishCallback.RemoveAllListeners();
                    //    mAnotherSelect.GetComponent<ComAnimatorAutoClose>().mOnFinishCallback.AddListener(callback);
                    //}

                    mAnotherRewardImageList[idx].color = Color.clear;
                    _hiddenAllChild(mAnotherRewardImageList[idx].gameObject);
                    //callback = _AddAnotherCallback(i, callback);
                    //_RestartAni(mAnotherRewardList[i], callback);

                    if (callback != null)
                    {
                        callback();
                    }
                }
                else
                {
					if (mIsRealGetAnotherReward)
                    	mAnotherRewardImageList[i].raycastTarget = false;
                }
            }
        }

        private UnityAction _AddNormalCallback(int idx, UnityAction callback)
        {
            if (callback != null)
            {
                UnityAction unityAction = callback;
                callback = () =>
                {
                    mNormalRewardImageList[idx].color = Color.clear;
                    _hiddenAllChild(mNormalRewardImageList[idx].gameObject);
                    unityAction();
                };
            }
            else
            {
                mNormalRewardImageList[idx].color = Color.clear;
                _hiddenAllChild(mNormalRewardImageList[idx].gameObject);
            }

            return callback;
        }

        private UnityAction _AddAnotherCallback(int idx, UnityAction callback)
        {
            if (callback != null)
            {
                UnityAction unityAction = callback;
                callback = () =>
                {
                    mAnotherRewardImageList[idx].color = Color.clear;
                    _hiddenAllChild(mAnotherRewardImageList[idx].gameObject);
                    unityAction();
                };
            }
            else
            {
                mAnotherRewardImageList[idx].color = Color.clear;
                _hiddenAllChild(mAnotherRewardImageList[idx].gameObject);
            }

            return callback;
        }

        private bool _bindCountCallback()
        {
            if (null == BattleMain.instance)
            {
                return false;
            }

            if (null == BattleMain.instance.GetPlayerManager())
            {
                return false;
            }

            if (null == BattleMain.instance.GetPlayerManager().GetAllPlayers())
            {
                return false;
            }

			mCount10 = mBind.GetCom<ComCountScript>("count10");

            mCountRoot.SetActive(true);

            if(NewbieGuideManager.instance.IsGuidingTask(NewbieGuideTable.eNewbieGuideTask.DungeonRewardGuide))
            {
                mCountRoot.SetActive(false);
                return true;
            }
				
			int duration = 5;
			if (BattleMain.instance.GetPlayerManager().GetAllPlayers().Count > 1)
				duration = 9;

			mCount10.StartCount(_timeOutClick , duration); 
            return true;
        }

        private void _timeOutClick()
        {
            if (countTimeOut)
            {
                return;
            }

            mClose.CustomActive(false);
            if (mIsRealGetNormalReward || mIsAnotherWorldRealGetReward)
            {
                kCloseWaitTime = 0.5f;
            }
            else
            {
                kCloseWaitTime = 1.5f;
            }

            if (!mIsGetAnotherReward)
            {
                _startAnotherSelect(kDefaultSelectedIdx, null);
            }

            _onSceneDungeonOpenChestReq(kDefaultSelectedIdx);

            countTimeOut = true;

            if (mIsRealGetNormalReward || mIsAnotherWorldRealGetReward)
            {
                mCurProcessStatus = eFlipProcess.WaitClose;
            }

            _startDelayClose(kCloseWaitTime);
        }


        private UnityEngine.Coroutine mDelayClose = null;
        private UnityEngine.Coroutine mTimeOut = null;
        private void _startDelayClose(float time)
        {
            if (NewbieGuideManager.GetInstance().GetCurTaskID() == NewbieGuideTable.eNewbieGuideTask.DungeonRewardGuide)
            {
                NewbieGuideManager.GetInstance().ManagerFinishGuide(NewbieGuideManager.GetInstance().GetCurTaskID());
            }

            _stopDelayClose();
            mDelayClose = GameFrameWork.instance.StartCoroutine(_delayClose(time));
        }

        private void _stopDelayClose()
        {
            if (null != mDelayClose)
            {
                GameFrameWork.instance.StopCoroutine(mDelayClose);
            }
            mDelayClose = null;
        }

        private bool _onSceneDungeonOpenChestReq(int id, bool another = false)
        {
            Logger.LogProcessFormat("send req  {0}, another {1}", id, another);

            GameStatisticManager.instance.DoStatInBattleEx(StatInBattleType.CLICK_CARD,
                    _GetDungeonIdWithOutDiff(false),
                    BattleMain.instance.GetDungeonManager().GetDungeonDataManager().CurrentAreaID(),
                    string.Format("{0}", id));

            if (another)
            {
                if (mIsGetAnotherReward)
                {
                    Logger.LogProcessFormat("has already selected another reward");
                    return false;
                }
                else
                {
                    mIsGetAnotherReward = true;
					mAnotherClickIndex = id;
                }
            }
            else
            {

                switch (mCurCarType)
                {
                    case DungeonTable.eCardType.None:
                        break;
                    case DungeonTable.eCardType.Golden_Card:

                        if (mIsGetNormalReward)
                        {
                            Logger.LogProcessFormat("has already selected normal reward");
                            return false;
                        }
                        else
                        {
                            mIsGetNormalReward = true;
                            mNormalClickIndex = id;

                            if (NewbieGuideManager.instance.IsGuidingTask(NewbieGuideTable.eNewbieGuideTask.DungeonRewardGuide))
                            {

                            }
                            else
                            {
                                if (null != mTimeOut) { GameFrameWork.instance.StopCoroutine(mTimeOut); }
                                mTimeOut = GameFrameWork.instance.StartCoroutine(_timeOut(10));
                            }
                        }

                        break;
                    case DungeonTable.eCardType.Yijie_Card:

                        if (mIsGetAnotherWorldReward)
                        {
                            Logger.LogProcessFormat("has already selected another world reward");
                            return false;
                        }
                        else
                        {
                            mIsGetAnotherWorldReward = true;
                            mAnotherWorldClickIndex = id;

                            if (NewbieGuideManager.instance.IsGuidingTask(NewbieGuideTable.eNewbieGuideTask.DungeonRewardGuide))
                            {

                            }
                            else
                            {
                                if (null != mTimeOut) { GameFrameWork.instance.StopCoroutine(mTimeOut); }
                                mTimeOut = GameFrameWork.instance.StartCoroutine(_timeOut(10));
                            }
                        }

                        break;
                    case DungeonTable.eCardType.Hundun_Card:
                        break;
                    default:
                        break;
                }
            }

            SceneDungeonOpenChestReq req = new SceneDungeonOpenChestReq();
            req.pos = (byte)(another ? id + kAnotherRewardButtonOffset : id);
            if (kLocalTest)
            {
                var res = new SceneDungeonOpenChestRes();
                res.pos = req.pos;

                res.chest = new DungeonChest();
                res.chest.goldReward = 20;
                res.chest.itemId = 151491004;

                _onSceneDungeonOpenChestRes(res);
            }
            else
            {
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
            }
            Logger.LogProcessFormat("send req with content {0}，id={1}", ObjectDumper.Dump(req),id);

            return true;
        }

     

        private void setEffectAnotherWorldBtn(int index)
        {
            for (int i = 0; i < mYijieKamianqianEffect.Length; i++)
            {
                if (index != i)
                {
                    continue;
                }

                mYijieKamianqianEffect[i].CustomActive(false);
                mYijieKamianhouEffect[i].CustomActive(false);
            }
        }

       
        private void _onSceneDungeonOpenChestResDATA(MsgDATA data )
        {
            SceneDungeonOpenChestRes res = new SceneDungeonOpenChestRes();
            res.decode(data.bytes);

            _onSceneDungeonOpenChestRes(res);
        }

        //[ProtocolHandle(typeof(SceneDungeonOpenChestRes))]
        private void _onSceneDungeonOpenChestRes(SceneDungeonOpenChestRes res)
        {
            Logger.LogFormat(ObjectDumper.Dump(res));

            var chest = res.chest;
			string name = "";
			bool isSelf = false;

            BattlePlayer battlePlayer = null;
			if (BattleMain.instance != null)
			{
				battlePlayer = BattleMain.instance.GetPlayerManager().GetPlayerByRoleID((ulong)res.owner);
				if (battlePlayer != null)
				{
					name = battlePlayer.playerInfo.name;
					if (battlePlayer.playerActor.isLocalActor)
						isSelf = true;
				}
			}

			if (res.pos < kAnotherRewardButtonOffset)
				normalChestCount++;
			else
				goldChestCount++;

			opened[res.pos] = true;

			int playerNum = BattleMain.instance.GetPlayerManager().GetAllPlayers().Count;

            _onOpenReward(res.pos, chest.num, chest.itemId, chest.goldReward, chest.strenth, name, isSelf, (EEquipType)chest.equipType, battlePlayer);

            if (NewbieGuideManager.instance.IsGuidingTask(NewbieGuideTable.eNewbieGuideTask.DungeonRewardGuide))
                ;
            else if ((!countTimeOut && openChestCount >= playerNum * 2) || (countTimeOut && normalChestCount >= playerNum))
            {
                if (!countTimeOut)
                {
                    CloseTimeCount();
                    mClose.CustomActive(false);
                }
                _startDelayClose(kCloseWaitTime);
            }
        }

		bool IsOpened(int index)
		{
			return opened[index];
		}

        //[UIEventHandle("Root/Card1/CardItem{0}", typeof(Button), typeof(UnityEngine.Events.UnityAction<int>), 0, 3)]
        private void _onClickReward(int idx)
        {
			if (IsOpened(idx))
				return;


			if (mIsGetNormalReward || mIsRealGetNormalReward)
            {
                return;
            }

            //mCentTextRoot.SetActive(false);
            _RestartAni(mNormalRewardList[idx], null, true);

            _onSceneDungeonOpenChestReq(idx);

            if (NewbieGuideManager.instance.IsGuidingTask(NewbieGuideTable.eNewbieGuideTask.DungeonRewardGuide))
            {
                mIsGuide = true;
            }
        }

        private void _onAnotherWorldClickReward(int idx)
        {
            if (IsOpened(idx))
                return;

            if (mIsAnotherWorldRealGetReward || mIsGetAnotherWorldReward)
            {
                return;
            }

            _onSceneDungeonOpenChestReq(idx);

            setEffectAnotherWorldBtn(idx);
        }

        //[UIEventHandle("Root/Card3/CardItem{0}", typeof(Button), typeof(UnityEngine.Events.UnityAction<int>), 0, 3)]
        private void _onClickAnotherReward(int idx)
        {
			if (IsOpened(idx+kAnotherRewardButtonOffset))
				return;
			
			if (mIsGetAnotherReward || mIsRealGetAnotherReward)
            {
                return;
            }

            _RestartAni(mAnotherRewardList[idx], null, true);

            //mCentTextRoot.SetActive(false);

            // cost money            
            _onSceneDungeonOpenChestReq(idx, true);

            if(mIsGuide)
            {
                mTip.gameObject.SetActive(true);
            }         
        }

		private void _showReward(string path,GameObject root, uint num, uint itemID, uint glod,uint strenthLevel, eType type, string playerName, bool isSelf, bool isAnother, EEquipType equipType, BattlePlayer player = null)
        {
            Logger.LogProcessFormat("open reward with item id : {0}, glod num : {1}, root name {2}", itemID, glod, root.name);
            
            //string path = mBind.GetPrefabPath("unit");
            var cardUnit = AssetLoader.instance.LoadResAsGameObject(path);
            var iconEffect = AssetLoader.instance.LoadResAsGameObject(kIconEffectPath);
            Utility.AttachTo(cardUnit, root);

            bool isFlagNewMonthCardRewardItem = false;

            var bind = cardUnit.GetComponent<ComCommonBind>();
            if (null != bind)
            {
                Text name           = bind.GetCom<Text>("name");
                Text coin           = bind.GetCom<Text>("coin");
                GameObject coinroot = bind.GetGameObject("coinroot");
                Image iconbg = bind.GetCom<Image>("iconbg");
                Image icon = bind.GetCom<Image>("icon");
                Text iconcount = bind.GetCom<Text>("iconcount");
                Text iconlevel = bind.GetCom<Text>("iconlevel");
                

                Text upOwner = bind.GetCom<Text>("upOwner");
                Image upOwnerBg = bind.GetCom<Image>("upOwnerBg");

                Image bg = bind.GetCom<Image>("bg");

                GameObject upRewardRoot = bind.GetGameObject("upRewardRoot");
                GameObject effectRoot = bind.GetGameObject("EffectRoot");
                Text strenth = bind.GetCom<Text>("strenth");
                GameObject vipDescRoot = bind.GetGameObject("VipDescRoot");
                Text vipDesc = bind.GetCom<Text>("VipDesc");
                GameObject breathMarkRoot = bind.GetGameObject("breathMark");
                GameObject vipBg = bind.GetGameObject("VipBg");

                if (vipBg != null)
                {
                    vipBg.CustomActive(false);
                }

                if (breathMarkRoot != null)
                {
                    breathMarkRoot.CustomActive(equipType == EEquipType.ET_BREATH);
                }
                string effectPath = null;
                var dropTableItem = TableManager.instance.GetTableItem<ProtoTable.ItemTable>((int)itemID);
                if (dropTableItem != null)
                {
                    //ItemData.QualityInfo qi = ItemData.GetQualityInfo(dropTableItem.Color, dropTableItem.Color2 == 1);
                    ItemData.QualityInfo qi = ItemData.GetQualityInfo(dropTableItem.Color, dropTableItem.Color2);
                    name.text = strenthLevel > 0 ? string.Format("<color={0}>+{1}{2}</color>", qi.ColStr,strenthLevel, dropTableItem.Name) : string.Format("<color={0}>{1}</color>", qi.ColStr, dropTableItem.Name);
                    // icon.sprite   = AssetLoader.instance.LoadRes(dropTableItem.Icon, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref icon, dropTableItem.Icon);
                    // iconbg.sprite = AssetLoader.instance.LoadRes(qi.Background, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref iconbg, qi.Background);
					
                   if (strenth != null)
                    {
                        strenth.CustomActive(strenthLevel > 0);
                        if (strenthLevel > 0)
                        {
                            strenth.text = string.Format("+{0}", strenthLevel);
                        }
                    }
					
                    if (iconEffect != null)
                    {
                        var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(_GetDungeonIdWithOutDiff());
                        if (dungeonTable != null)
                        {
                            if (dungeonTable.SubType == DungeonTable.eSubType.S_DEVILDDOM)
                            {
                                Utility.AttachTo(iconEffect, iconbg.gameObject);
                            } 
                        }
                    }

                    iconlevel.text = string.Format("Lv.{0}", dropTableItem.NeedLevel);

                    if (dropTableItem.Color == ItemTable.eColor.PINK)
                    {
                        effectPath = bind.GetPrefabPath("fense");
                    }
                    else if (dropTableItem.Color == ItemTable.eColor.PURPLE)
                    {
                        effectPath = bind.GetPrefabPath("zise");
                    }
                    else if (dropTableItem.Color == ItemTable.eColor.GREEN)
                    {
                        effectPath = bind.GetPrefabPath("lvse");
                    }
                    else
                    {
                        effectPath = bind.GetPrefabPath("other");        //
                    }
                    if (effectPath != null)
                    {
                        var effectUnit = AssetLoader.instance.LoadResAsGameObject(effectPath);
                        Utility.AttachTo(effectUnit, effectRoot);
                    }
                    
                }
                else 
                {
                    Logger.LogErrorFormat("[翻牌] 道具无法再道具表中找到 {0}", itemID);
                    name.text = "";
                }

                if (num > 0)
                {
                    if (num > 1)
                    {
                        iconcount.text = string.Format("{0}", num);
                    }
                    else
                    {
                        iconcount.text = string.Empty;
                    }
                }
                else
                {
                    iconbg.color = Color.clear;
                    icon.color = Color.clear;
                    iconlevel.text = string.Empty;
                    iconcount.text = string.Empty;
                    name.text = string.Empty;
                }

                upRewardRoot.SetActive(eType.Vip != type);

                if (type == eType.Normal)
                {
                    coin.text = string.Format("{0}", glod);
                }
                else if (type == eType.AnotherWorld)
                {
                    coin.text = string.Format("{0}", glod);
                }
                else 
                {
                    coinroot.SetActive(false);
                }

                upOwner.text = playerName;

                // if (isSelf) { upOwnerBg.sprite = bind.GetSprite("self"); }
                if (isSelf)
                {
                    // upOwnerBg.sprite = bind.GetSprite("self");
                    bind.GetSprite("self", ref upOwnerBg);
                }
                else
                {
                    // upOwnerBg.sprite = bind.GetSprite("notself");
                    bind.GetSprite("notself", ref upOwnerBg);
                }

                if (isSelf)
                {
                    upOwnerBg.gameObject.SetActive(BattleMain.IsModeMultiplayer(BattleMain.mode) && !BattleMain.IsModePvP(BattleMain.battleType));
                }

                //if (isAnother) { bg.sprite = bind.GetSprite("special"); }
                //else { bg.sprite = bind.GetSprite("normal"); }
                if (type != eType.AnotherWorld)
                {
                    if (isAnother)
                    {
                        bind.GetSprite("special", ref bg);
                    }
                    else
                    {
                        bind.GetSprite("normal", ref bg);
                    }
                }

                bg.CustomActiveAlpha(type != eType.Vip);

                GameObject vipFlag = bind.GetGameObject("vipFlag");

                if (player.IsPlayerMonthCard())
                {
                    vipFlag.SetActive(type == eType.Vip);
                    vipBg.CustomActive(type == eType.Vip);
                }
                else
                {
                    if (vipDescRoot != null)
                    {
                        vipDescRoot.CustomActive(type == eType.Vip);
                    }

                    if (dropTableItem != null)
                    {
                        isFlagNewMonthCardRewardItem = MonthCardRewardLockersDataManager.GetInstance().IsNewItemQualityAbleToEnterLockers(dropTableItem.Color);

                        if (vipDesc != null)
                        {
                            if (isFlagNewMonthCardRewardItem)
                            {
                                vipDesc.text = TR.Value("monthCard_greaterthan_purple");
                            }
                            else
                            {
                                vipDesc.text = TR.Value("monthCard_Lessthan_purple");
                            }
                        }
                    }

                    if (type == eType.Vip)
                    {
                        coinroot.SetActive(false);

                        if (vipBg != null)
                        {
                            UIGray uIGray = vipBg.GetComponent<UIGray>();
                            if (uIGray != null)
                            {
                                uIGray.SetEnable(true);
                            }
                            vipBg.CustomActive(true);
                        }
                    }
                }

                if (_ismCrossServiceFlipCardNumberRoot())
                {
                    if (mCrossServiceFlipCardNumberRoot != null)
                    {
                        mCount.SafeSetText(number.ToString());
                    }

                    GameObject doubleFlag = bind.GetGameObject("doubleflag");
                    doubleFlag.CustomActive(GuildDataManager.GetInstance().chestDoubleFlag > 0 && PlayerBaseData.GetInstance().Name == playerName);
                }
            }

            //月卡翻牌 且 有新道具要加入时 尝试刷新月卡翻牌数据 
            if (type == eType.Vip && isFlagNewMonthCardRewardItem)
            {
                MonthCardRewardLockersDataManager.GetInstance().ReqMonthCardRewardLockersItems();
            }
        }

        private bool _ismCrossServiceFlipCardNumberRoot()
        {
            if (GuildDataManager.GetInstance().HasSelfGuild())
            {
                GuildTerritoryTable terriData = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(GuildDataManager.GetInstance().myGuild.nSelfCrossManorID);
                if (terriData != null)
                {
                    string[] dungeons = terriData.ChestDoubleDungeons.Split('|');

                    if (dungeons != null)
                    {
                        for (int i = 0; i < dungeons.Length; i++)
                        {
                            int iDungeonid = 0;
                            if (!int.TryParse(dungeons[i], out iDungeonid))
                            {
                                continue;
                            }

                            if (iDungeonid == _GetDungeonIdWithOutDiff())
                            {
                                //count 表示翻牌福利已经翻了几次
                                int count = CountDataManager.GetInstance().GetCount("guild_terr_dungeon_num");

                                // number 表示翻牌福利还剩几次
                                number = terriData.DailyChestDoubleTimes - (count - 1);

                                if (number > 0 && GuildDataManager.GetInstance().chestDoubleFlag > 0)
                                {
                                    return true;
                                }
                                
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool _isCardSlot(int idx, int offset)
        {
            return (idx >= offset && idx < kRewardButtonCount + offset);
        }

        private bool _isNormalCardSlot(int idx)
        {
            return _isCardSlot(idx, 0);
        }

        private bool _isGoldCardSlot(int idx)
        {
            return _isCardSlot(idx, kAnotherRewardButtonOffset);
        }

        private bool _isVipCardSlot(int idx)
        {
            return _isCardSlot(idx, kVipNormalRewardButtonOffset);
        }

		private void _onOpenReward(int idx, uint num, uint itemID, uint glod, uint strenthLevel, string playerName, bool isOurself,EEquipType equipType, BattlePlayer player = null)
        {
            Logger.LogProcessFormat("[翻牌] 开始翻牌，索引 {0}, 数目 {1}, 道具ID {2}, 金币 {3}, 玩家名字 {4}, 是否自己 {5}", idx, num, itemID, glod, playerName, isOurself);

            int realIdx = idx % kRewardButtonCount;
            string cardPath = "";

            switch (mCurCarType)
            {
                case DungeonTable.eCardType.None:
                    break;
                case DungeonTable.eCardType.Golden_Card:

                    cardPath = mBind.GetPrefabPath("unit");

                    if (_isNormalCardSlot(idx))
                    {
                        openChestCount++;

                        mNormalVipRewards[realIdx].gameObject.SetActive(true);

                        if (null == player || !player.IsPlayerMonthCard())
                        {
                            UIGray gray = mNormalVipRewards[realIdx].gameObject.SafeAddComponent<UIGray>();
                            gray.enabled = true;
                        }

                        DOTweenAnimation animate = mNormalVipRewards[realIdx].GetComponent<DOTweenAnimation>();

                        if (null != animate)
                        {
                            if (null != animate.onComplete)
                            {
                                animate.onComplete.AddListener(() =>
                                {
                                    if (isOurself)
                                    {
                                        mIsRealGetNormalReward = true;

                                        if (mIsRealGetAnotherReward || !_canFlipAnotherFlipType())
                                        {
                                            mCurProcessStatus = eFlipProcess.WaitClose;
                                        }
                                    }
                                });
                            }
                        }

                        _startNormalSelect(realIdx, () =>
                        {
                            _showReward(cardPath, mNormalRewardImageList[realIdx].gameObject, num, itemID, glod, strenthLevel, eType.Normal, playerName, isOurself, false,equipType,player);


                            if (isOurself)
                            {
                                //mIsRealGetNormalReward = true;

                                //if (mIsRealGetAnotherReward || !_canFlipAnotherFlipType())
                                //{
                                //    mCurProcessStatus = eFlipProcess.WaitClose;
                                //}
                            }

                            if (mIsGetNormalReward && mNormalClickIndex == idx)
                                mIsGetNormalReward = false;

                        });
                    }
                    else if (_isVipCardSlot(idx))
                    {

                        DOTweenAnimation animate = mNormalVipRewards[realIdx].GetComponent<DOTweenAnimation>();

                        if (null != animate)
                        {
                            if (null != animate.onComplete)
                            {
                                animate.onComplete.AddListener(() =>
                                {
                                    _startNormalVIPSelect(realIdx, () =>
                                    {
                                        _showReward(cardPath, mNormalVipRewards[realIdx].gameObject, num, itemID, glod, strenthLevel, eType.Vip, playerName, isOurself, true, equipType, player);
                                    },player);
                                });


                            }
                        }
                        else
                        {
                            _startNormalVIPSelect(realIdx, () =>
                            {
                                _showReward(cardPath, mNormalVipRewards[realIdx].gameObject, num, itemID, glod, strenthLevel, eType.Vip, playerName, isOurself, true, equipType, player);
                            }, player);
                        }
                    }
                    else if (_isGoldCardSlot(idx))
                    {
                        openChestCount++;

                        DOTween
                            .To(() => { return 1.0f; },
                                (x) => { mInfomationCanvasGroup.alpha = x; },
                                0.0f,
                                2.0f)
                            .SetEase(Ease.OutQuad);

                        _startAnotherSelect(realIdx, () =>
                        {
                            _showReward(cardPath, mAnotherRewardImageList[realIdx].gameObject, num, itemID, glod, strenthLevel, eType.Another, playerName, isOurself, true, equipType, player);
                            if (isOurself)
                            {
                                mIsRealGetAnotherReward = true;

                                if (mIsRealGetNormalReward)
                                {
                                    mCurProcessStatus = eFlipProcess.WaitClose;
                                }
                            }

                            if (mIsGetAnotherReward && mAnotherClickIndex == realIdx)
                                mIsGetAnotherReward = false;
                        });
                    }
                    else
                    {
                        Logger.LogErrorFormat("[翻牌] 索引超出范围 {0}", idx);
                    }

                    break;
                case DungeonTable.eCardType.Yijie_Card:

                    cardPath = mBind.GetPrefabPath("anotherworldunit");

                    if (_isNormalCardSlot(idx))
                    {
                        openChestCount++;

                        _startAnotherWorldSelect(idx, () =>
                        {
                            _showReward(cardPath, mAnotherWorldRewardImageList[realIdx].gameObject, num, itemID, glod, strenthLevel, eType.AnotherWorld, playerName, isOurself, true, equipType, player);
                            if (isOurself)
                            {
                                mIsAnotherWorldRealGetReward = true;

                                if (mIsAnotherWorldRealGetReward || !_canFlipAnotherFlipType())
                                {
                                    mCurProcessStatus = eFlipProcess.WaitClose;
                                }
                            }

                            if (mIsGetAnotherWorldReward && mAnotherWorldClickIndex == idx)
                                mIsGetAnotherWorldReward = false;
                        });
                    }

                    break;
                case DungeonTable.eCardType.Hundun_Card:
                    break;
                default:
                    break;
            }
        }

        private IEnumerator _timeOut(float time)
        {
            yield return Yielders.GetWaitForSeconds(time);

            if (!mIsRealGetNormalReward && mIsGetNormalReward)
            {
                Logger.LogError("wait the net message time out error dropboxtable config");

                //SystemNotifyManager.SysNotifyMsgBoxOK("missing the DungeonChestTable");
            }

            if (!mIsClosed)
            {
                _onClose();
            }
        }

        private IEnumerator _delayClose(float time)
        {
            yield return Yielders.GetWaitForSeconds(time);
            if (!mIsClosed)
            {
                _onClose();
            }
        }

        private void _onClose()
        {
            mIsClosed = true;
            frameMgr.CloseFrame(this);

            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonRewardFrameClose);
        }

        private void _onHandle()
        {
            if (mCurProcessStatus == eFlipProcess.WaitClose)
            {
                //_onClose();
            }
        }

        private void _InitMidChild()
        {
            eAnotherCardFlipType type = _getAnotherFlipType();

            mNormalRoot.CustomActive(false);
            mVipRoot.CustomActive(false);
            mHellRoot.CustomActive(false);
            mNotOpenRoot.CustomActive(false);

            int id = (int)BattleDataManager.GetInstance().chestInfo.itemId;
            int count = (int)BattleDataManager.GetInstance().chestInfo.count;

            ProtoTable.ItemTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(id);
            if (null != tableData)
            {
                mCostTypeName.text = tableData.Name;
                mHellCostTypeName.text = tableData.Name;

                //Sprite sp = AssetLoader.instance.LoadRes(tableData.Icon, typeof(Sprite)).obj as Sprite;
                //_setAnotherFlipCostSprite(sp);
                _setAnotherFlipCostSprite(tableData.Icon);
            }

            // _setAnotherFlipSprite(mBind.GetSprite("normalcard"));
            _setAnotherFlipSprite("normalcard");
            _setAnotherFlipText(string.Format("{0}", count));

            Logger.LogProcessFormat("[翻牌] 设置翻牌的消耗类型 id {0}, {1}, {2}", id, count, type);

            switch (type)
            {
                case eAnotherCardFlipType.NotOpen:
                case eAnotherCardFlipType.None:
                    _setAnotherFlipEnable(false);
                    mNotOpenRoot.SetActive(true);
                    mNotOpenLevel.text = string.Format("{0}", _getAnotherFlipOpenLevel());
                    _setAnotherFlipText(string.Format("{0}级开启", _getAnotherFlipOpenLevel()));
                    _setAnotherFlipCostIconActive(false);
                    break;
                case eAnotherCardFlipType.NotCostTicketItem:
                    _setAnotherFlipEnable(false);
                    mHellRoot.SetActive(true);
                    // _setAnotherFlipSprite(mBind.GetSprite("hellcard"));
                    _setAnotherFlipSprite("hellcard");
                    _setAnotherFlipCostIconActive(true);
                    break;
                case eAnotherCardFlipType.TicketCost:
                    _setAnotherFlipEnable(true);
                    mHellRoot.SetActive(true);
                    // _setAnotherFlipSprite(mBind.GetSprite("hellcard"));
                    _setAnotherFlipSprite("hellcard");
                    _setAnotherFlipCostIconActive(true);
                    break;
                case eAnotherCardFlipType.NotCostGoldItem:
                    _setAnotherFlipEnable(false);
                    mNormalRoot.SetActive(true);
                    _setAnotherFlipCostIconActive(true);
                    break;
                case eAnotherCardFlipType.GoldCost:
                    _setAnotherFlipEnable(true);
                    mNormalRoot.SetActive(true);
                    _setAnotherFlipCostIconActive(true);
                    break;
                case eAnotherCardFlipType.VipFree:
                    _setAnotherFlipEnable(true);
                    _setAnotherFlipText("免费");
                    mVipRoot.SetActive(true);
                    _updateVipRootInfo();
                    _setAnotherFlipCostIconActive(true);
                    break;
            }
        }
        
        /// <summary>
        /// 如果是异界关卡Card2Card3LabelRoot隐藏
        /// </summary>
        private void _InitCard2Card3LabelRootIsShow()
        {
            mCurCarType = GetCardType();

            switch (mCurCarType)
            {
                case DungeonTable.eCardType.None:
                case DungeonTable.eCardType.Golden_Card:
                case DungeonTable.eCardType.Hundun_Card:
                    {
                        _InitGoldenCardRoot();
                        _InitMidChild();
                    }
                    break;
                case DungeonTable.eCardType.Yijie_Card:
                    {
                        mMidRoot.CustomActive(false);
                        mCentTextRoot.CustomActive(false);
                        _InitAnotherWorldCardRoot();
                    }
                    break;
                default:
                    break;
            }
        }

        private void _InitGoldenCardRoot()
        {
            if (mGoldCardRoot == null)
            {
                string goldCardRootPath = mBind.GetPrefabPath("goldcardroot");
                mGoldCardRoot = AssetLoader.instance.LoadResAsGameObject(goldCardRootPath, true);
                Utility.AttachTo(mGoldCardRoot, mMidRoot);
                ComCommonBind bind = mGoldCardRoot.GetComponent<ComCommonBind>();

                if (bind != null)
                {
                    mNormalGroup = bind.GetCom<HorizontalLayoutGroup>("NormalGroup");
                    mAnotherGrayGroup = bind.GetCom<UIGray>("AnotherGrayGroup");
                    mNormalRewardList[0] = bind.GetCom<Button>("NormalReward0");
                    mNormalRewardList[0].onClick.AddListener(_onNormalReward0ButtonClick);
                    mNormalRewardList[1] = bind.GetCom<Button>("NormalReward1");
                    mNormalRewardList[1].onClick.AddListener(_onNormalReward1ButtonClick);
                    mNormalRewardList[2] = bind.GetCom<Button>("NormalReward2");
                    mNormalRewardList[2].onClick.AddListener(_onNormalReward2ButtonClick);
                    mAnotherRewardList[0] = bind.GetCom<Button>("AnotherReward0");
                    mAnotherRewardList[0].onClick.AddListener(_onAnotherReward0ButtonClick);
                    mAnotherRewardList[1] = bind.GetCom<Button>("AnotherReward1");
                    mAnotherRewardList[1].onClick.AddListener(_onAnotherReward1ButtonClick);
                    mAnotherRewardList[2] = bind.GetCom<Button>("AnotherReward2");
                    mAnotherRewardList[2].onClick.AddListener(_onAnotherReward2ButtonClick);
                    mAnotherRewardCostList[0] = bind.GetCom<Text>("AnotherRewardCost0");
                    mAnotherRewardCostList[1] = bind.GetCom<Text>("AnotherRewardCost1");
                    mAnotherRewardCostList[2] = bind.GetCom<Text>("AnotherRewardCost2");
                    mAnotherRewardWrapList[0] = bind.GetCom<UIPrefabWrapper>("AnotherRewardWrap0");
                    mAnotherRewardWrapList[1] = bind.GetCom<UIPrefabWrapper>("AnotherRewardWrap1");
                    mAnotherRewardWrapList[2] = bind.GetCom<UIPrefabWrapper>("AnotherRewardWrap2");

                    mNormalRoot = bind.GetGameObject("NormalRoot");
                    mVipRoot = bind.GetGameObject("VipRoot");
                    mVipLevel = bind.GetCom<Text>("VipLevel");
                    mVipLeft = bind.GetCom<Text>("VipLeft");
                    mVipSum = bind.GetCom<Text>("VipSum");

                    mCostTypeName = bind.GetCom<Text>("CostTypeName");
                    mAnotherRewardCostIconList[0] = bind.GetCom<Image>("AnotherRewardCostIcon0");
                    mAnotherRewardCostIconList[1] = bind.GetCom<Image>("AnotherRewardCostIcon1");
                    mAnotherRewardCostIconList[2] = bind.GetCom<Image>("AnotherRewardCostIcon2");

                    mHellCostTypeName = bind.GetCom<Text>("HellCostTypeName");
                    mHellRoot = bind.GetGameObject("HellRoot");
                    mCentTextRoot = bind.GetGameObject("centTextRoot");

                    mNotOpenRoot = bind.GetGameObject("NotOpenRoot");
                    mNotOpenLevel = bind.GetCom<Text>("NotOpenLevel");

                    mNormalVipRewards[0] = bind.GetCom<Button>("NormalVipReward0");
                    mNormalVipRewards[0].onClick.AddListener(_onNormalVipReward0ButtonClick);
                    mNormalVipRewards[1] = bind.GetCom<Button>("NormalVipReward1");
                    mNormalVipRewards[1].onClick.AddListener(_onNormalVipReward1ButtonClick);
                    mNormalVipRewards[2] = bind.GetCom<Button>("NormalVipReward2");
                    mNormalVipRewards[2].onClick.AddListener(_onNormalVipReward2ButtonClick);
                    mInfomationCanvasGroup = bind.GetCom<CanvasGroup>("InfomationCanvasGroup");

                    for (int i = 0; i < kRewardButtonCount; ++i)
                    {
                        mNormalRewardImageList[i] = mNormalRewardList[i].GetComponent<Image>();
                        mNormalRewardVIPImageList[i] = mNormalVipRewards[i].GetComponent<Image>();
                        mAnotherRewardImageList[i] = mAnotherRewardList[i].GetComponent<Image>();
                    }
                }
            }
            else
            {
                mGoldCardRoot.CustomActive(true);
            }
        }

        private void _InitAnotherWorldCardRoot()
        {
            if (mmAnotherWorldCardRoot == null)
            {
                string anotherWorldCardPath = mBind.GetPrefabPath("anotherworldcard");
                mmAnotherWorldCardRoot = AssetLoader.instance.LoadResAsGameObject(anotherWorldCardPath, true);
                Utility.AttachTo(mmAnotherWorldCardRoot, mYijieRoot);
                ComCommonBind bind = mmAnotherWorldCardRoot.GetComponent<ComCommonBind>();
                if (bind != null)
                {
                    mAnotherWorldRewardList[0] = bind.GetCom<Button>("AnotherWorldCardItem0");
                    mAnotherWorldRewardList[0].onClick.AddListener(_onAnotherWorldReward0ButtonClick);
                    mAnotherWorldRewardList[1] = bind.GetCom<Button>("AnotherWorldCardItem1");
                    mAnotherWorldRewardList[1].onClick.AddListener(_onAnotherWorldReward1ButtonClick);
                    mAnotherWorldRewardList[2] = bind.GetCom<Button>("AnotherWorldCardItem2");
                    mAnotherWorldRewardList[2].onClick.AddListener(_onAnotherWorldReward2ButtonClick);
                    mmAnotherWorldCardRoot = bind.GetGameObject("AnotherWorldCard");

                    mYijieKamianhouEffect[0] = bind.GetGameObject("yijiekamianhou0");
                    mYijieKamianhouEffect[1] = bind.GetGameObject("yijiekamianhou1");
                    mYijieKamianhouEffect[2] = bind.GetGameObject("yijiekamianhou2");
                    mYijieKamianqianEffect[0] = bind.GetGameObject("yijiekamianqian0");
                    mYijieKamianqianEffect[1] = bind.GetGameObject("yijiekamianqian1");
                    mYijieKamianqianEffect[2] = bind.GetGameObject("yijiekamianqian2");

                    for (int i = 0; i < kRewardButtonCount; ++i)
                    {
                        mAnotherWorldRewardImageList[i] = mAnotherWorldRewardList[i].GetComponent<Image>();
                    }
                }
            }
            else
            {
                mmAnotherWorldCardRoot.CustomActive(true);
            }
        }

        /// <summary>
        /// 判断是否是异界关卡
        /// </summary>
        private DungeonTable.eCardType GetCardType()
        {
            var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(_GetDungeonIdWithOutDiff());
            if (dungeonTable != null)
            {
                return dungeonTable.CardType;
            }

            return DungeonTable.eCardType.Golden_Card;
        }

        private int _GetDungeonIdWithOutDiff(bool withOutDiff = true)
        {
            if (withOutDiff)
            {
                return BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonIDWithOutDiff;
            }
            else
            {
                return BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
            }
        }
    }
}
