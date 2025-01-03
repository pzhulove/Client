using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
	public class MonthCardRewardLockersFrame : ClientFrame
    {
        public enum BtnAccquireStatus
        {
            BuyMonthCard,
            EnableGetReward,
            DisableGetReward,
        }

        private List<MonthCardRewardLockersItem> mLockersItems;

        private BtnAccquireStatus mBtnAccquireStatus = BtnAccquireStatus.BuyMonthCard;

        private string tr_month_card_grid_count_full = "";
        private string tr_month_card_grid_count_notfull = "";
        private string tr_month_card_accquire_btn_get = "";
        private string tr_month_card_accquire_btn_buy = "";

        private UnityEngine.Coroutine waitToCloseFrame = null;

        public static void CommonOpen()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<MonthCardRewardLockersFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<MonthCardRewardLockersFrame>();
            }
            ClientSystemManager.GetInstance().OpenFrame<MonthCardRewardLockersFrame>(FrameLayer.Middle);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/MonthCardRewardLockersFrame";
        }

        protected override void _OnOpenFrame()
        {
            MonthCardRewardLockersDataManager.GetInstance().ResetRedPoint();

            _BindUIEvent();

            _InitTR();
            _ReqRefreshView();
        }

        protected override void _OnCloseFrame()
        {
            mLockersItems = null;
            if (mRefreshCD)
            {
                mRefreshCD.StopTimer();
            }
            _UnInitTR();

            if (mBtnAccquireCD)
            {
                mBtnAccquireCD.StopBtCD();
            }

            mBtnAccquireStatus = BtnAccquireStatus.BuyMonthCard;

            if (waitToCloseFrame != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToCloseFrame);
                waitToCloseFrame = null;
            }

            _UnBindUIEvent();
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMonthCardRewardUpdate, _OnMonthCardRewardUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMonthCardRewardAccquired, _OnMonthCardRewardAccquired);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMonthCardUpdate, _OnMonthCardUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMonthCardRewardCDUpdate, _OnMonthCardRewardCDUpdate);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMonthCardRewardUpdate, _OnMonthCardRewardUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMonthCardRewardAccquired, _OnMonthCardRewardAccquired);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMonthCardUpdate, _OnMonthCardUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMonthCardRewardCDUpdate, _OnMonthCardRewardCDUpdate);
        }

        private void _InitTR()
        {
            tr_month_card_grid_count_full = TR.Value("month_card_grid_count_full");
            tr_month_card_grid_count_notfull = TR.Value("month_card_grid_count_notfull");
            tr_month_card_accquire_btn_get = TR.Value("month_card_accquire_btn_get");
            tr_month_card_accquire_btn_buy = TR.Value("month_card_accquire_btn_buy");
        }

        private void _UnInitTR()
        {
            tr_month_card_grid_count_full = "";
            tr_month_card_grid_count_notfull = "";
            tr_month_card_accquire_btn_get = "";
            tr_month_card_accquire_btn_buy = "";
        }      

        /// <summary>
        /// 需要优先刷时间
        /// </summary>
        private void _RefreshItemExpiredTime()
        {
            if (null == mRefreshCD)
            {
                return;
            }
            uint lastTime = MonthCardRewardLockersDataManager.GetInstance().UniformExpriedLastTime;
            //lastTime -= 1; //尝试减一 保证倒计时显示为1时结束

            if (lastTime <= 0)
            {
                if (mRefreshCD.componetText)
                {
                    mRefreshCD.componetText.enabled = false;
                }
                mRefreshCD.StopTimer();
            }
            else
            {
                if (mRefreshCD.componetText)
                {
                    mRefreshCD.componetText.enabled = true;
                }

                mRefreshCD.SetCountdown((int)lastTime);
                mRefreshCD.StartTimer();
            }
        }

        private void _RefreshItemGridCount()
        {
            int storeCount = 0;
            int totalCount = 0;

            if (mLockersItems != null)
            {
                storeCount = mLockersItems.Count;
            }
            if (mComItemBoard)
            {
                totalCount = mComItemBoard.GetItemGridTotalCount();
            }
            if (mGridCount)
            {
                if (storeCount >= totalCount)
                {
                    mGridCount.text = string.Format(tr_month_card_grid_count_full, storeCount.ToString(), totalCount.ToString());
                }
                else
                {
                    mGridCount.text = string.Format(tr_month_card_grid_count_notfull, storeCount.ToString(), totalCount.ToString());
                }
            }
        }

        private void _RefreshBtnAccquireStatus()
        {
            //ÔÂ¿¨¿ÉÓÃÊ±
            if (PayManager.GetInstance().HasMonthCardEnabled())
            {
                if (mLockersItems == null || mLockersItems.Count <= 0)
                {
                    mBtnAccquireStatus = BtnAccquireStatus.DisableGetReward;
                }
                else
                {
                    mBtnAccquireStatus = BtnAccquireStatus.EnableGetReward;
                }
            }
            else
            {
                mBtnAccquireStatus = BtnAccquireStatus.BuyMonthCard;
            }

            mBtnBuyMonthCard.CustomActive(mBtnAccquireStatus == BtnAccquireStatus.BuyMonthCard);
            mBtnAccquire.CustomActive(mBtnAccquireStatus != BtnAccquireStatus.BuyMonthCard);

            _SetBtnAccquireText();
            _SetBtnAccquireEnable();
        }

        private void _SetBtnAccquireText()
        {
            if (mBtnAccquireText != null)
            {
                mBtnAccquireText.text = mBtnAccquireStatus == BtnAccquireStatus.BuyMonthCard ? tr_month_card_accquire_btn_buy : tr_month_card_accquire_btn_get;
            }
        }

        private void _SetBtnAccquireEnable()
        {
            bool bBtnEnable = mBtnAccquireStatus == BtnAccquireStatus.DisableGetReward ? false : true;
            if (mBtnAccquire)
            {
                mBtnAccquire.enabled = bBtnEnable;
            }
            if (mBtnAccquireGray)
            {
                mBtnAccquireGray.SetEnable(!bBtnEnable);
            }
        }

        private void _ReqRefreshView()
        {
            MonthCardRewardLockersDataManager.GetInstance().ReqMonthCardRewardLockersItems();
        }

        private void _OnMonthCardRewardUpdate(UIEvent _uiEvent)
        {
            mLockersItems = MonthCardRewardLockersDataManager.GetInstance().GetMonthCardRewardLockersItems();

            if (mComItemBoard)
            {
                mComItemBoard.RefreshItemGrids(mLockersItems);
            }

            _RefreshItemGridCount();
            _RefreshBtnAccquireStatus();
        }

        private void _OnMonthCardRewardCDUpdate(UIEvent _uiEvent)
        {
            _RefreshItemExpiredTime();
        }

        private void _OnMonthCardRewardAccquired(UIEvent _uiEvent)
        {
            _ReqRefreshView();

            //this.Close();

            if (waitToCloseFrame != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToCloseFrame);
            }
            waitToCloseFrame = GameFrameWork.instance.StartCoroutine(_WaitToCloseFrame());
        }

        IEnumerator _WaitToCloseFrame()
        {
            yield return Yielders.GetWaitForSeconds(1f);
            ClientSystemManager.GetInstance().CloseFrame<MonthCardRewardLockersFrame>();
        }

        private void _OnMonthCardUpdate(UIEvent uiEvent)
        {
            _RefreshBtnAccquireStatus();
        }

        #region ExtraUIBind
        private Button mMaskClose = null;
		private ComMonthCardRewardLockersBoard mComItemBoard = null;
		private Text mGridCount = null;
		private SimpleTimer mRefreshCD = null;
		private Button mBtnAccquire = null;
		private UIGray mBtnAccquireGray = null;
		private SetComButtonCD mBtnAccquireCD = null;
		private Text mBtnAccquireText = null;
        private Button mBtnBuyMonthCard = null;
		
		protected override void _bindExUI()
		{
			mMaskClose = mBind.GetCom<Button>("MaskClose");
			if (null != mMaskClose)
			{
				mMaskClose.onClick.AddListener(_onMaskCloseButtonClick);
			}
			mComItemBoard = mBind.GetCom<ComMonthCardRewardLockersBoard>("ComItemGrids");
			mGridCount = mBind.GetCom<Text>("GridCount");
			mRefreshCD = mBind.GetCom<SimpleTimer>("RefreshCD");
			mBtnAccquire = mBind.GetCom<Button>("BtnAccquire");
			if (null != mBtnAccquire)
			{
				mBtnAccquire.onClick.AddListener(_onBtnAccquireButtonClick);
			}
			mBtnAccquireGray = mBind.GetCom<UIGray>("BtnAccquireGray");
			mBtnAccquireCD = mBind.GetCom<SetComButtonCD>("BtnAccquireCD");
			mBtnAccquireText = mBind.GetCom<Text>("BtnAccquireText");
            mBtnBuyMonthCard = mBind.GetCom<Button>("BtnBuyMonthCard");
            if (null != mBtnBuyMonthCard)
            {
                mBtnBuyMonthCard.onClick.AddListener(_onBtnAccquireButtonClick);
            }
        }
		
		protected override void _unbindExUI()
		{
			if (null != mMaskClose)
			{
				mMaskClose.onClick.RemoveListener(_onMaskCloseButtonClick);
			}
			mMaskClose = null;
			mComItemBoard = null;
			mGridCount = null;
			mRefreshCD = null;
			if (null != mBtnAccquire)
			{
				mBtnAccquire.onClick.RemoveListener(_onBtnAccquireButtonClick);
			}
			mBtnAccquire = null;
			mBtnAccquireGray = null;
			mBtnAccquireCD = null;
			mBtnAccquireText = null;
            mBtnBuyMonthCard = null;
        }
		#endregion

        #region Callback
        private void _onMaskCloseButtonClick()
        {
            this.Close();
        }

        private void _onBtnAccquireButtonClick()
        {
            if (mBtnAccquireStatus == BtnAccquireStatus.BuyMonthCard)
            {
                ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle, VipTabType.PAY);
            }
            else
            {
                if (mBtnAccquireCD.IsBtnWork() == false || mBtnAccquireCD == null)
                {
                    return;
                }

                MonthCardRewardLockersDataManager.GetInstance().ReqGetMonthCardRewardLockersItems();

                if (mBtnAccquireCD)
                {
                    mBtnAccquireCD.StartBtCD();
                }                    
            }
        }
    
        #endregion
    }
}
