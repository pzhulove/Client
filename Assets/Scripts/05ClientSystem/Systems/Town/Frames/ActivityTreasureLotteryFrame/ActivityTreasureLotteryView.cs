using Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    namespace ActivityTreasureLottery
    {

        public enum EActivityTreasureLotteryView
        {
            ActivityView,
            MyLotteryView,
            HistoryView
        }
        public class ActivityTreasureLotteryView : MonoBehaviour, IDisposable
        {
            //选中的item index
            public int SelectId { get { return mMainView.SelectId; } }

            //购买的数量
            public uint BuyCount { get { return mMainView.BuyCount; } }

            public uint LeftCount
            {
                get { return this.mMainView.LeftCount; }
            }

            //当前子分页类型
            public EActivityTreasureLotteryView CurrentSubView { get; private set; }

            //关闭按钮回调
            public UnityAction OnButtonCloseCallBack
            {
                set
                {
                    if (mButtonClose != null)
                    {
                        if (mOnButtonCloseCallBack != null)
                        {
                            mButtonClose.onClick.RemoveListener(mOnButtonCloseCallBack);
                        }

                        if (value != null)
                        {
                            mOnButtonCloseCallBack = value;
                            mButtonClose.onClick.AddListener(mOnButtonCloseCallBack);
                        }
                    }
                }
            }
            public UnityAction OnButtonBuyCallBack
            {
                set
                {
                    mMainView.OnButtonBuyCallBack = value;
                }
            }
            public UnityAction OnButtonBuyAllCallBack
            {
                set
                {
                    mMainView.OnButtonBuyAllCallBack = value;
                }
            }

            UnityAction mOnButtonCloseCallBack;
            #region SerializeField UI组件
            [SerializeField]
            ActivityTreasureLotteryActivityView mMainView;

            [SerializeField]
            ActivityTreasureLotteryHistroyView mHistroyView;

            [SerializeField]
            ActivityTreasureLotteryMyLotteryView mMyLotteryView;

            [SerializeField]
            Text mTextTimeLeft = null;

            [SerializeField]
            Toggle mToggleMain;

            [SerializeField]
            Toggle mToggleMyLottery;

            [SerializeField]
            Toggle mToggleRank;

            [SerializeField]
            Button mButtonClose;

            [SerializeField]
            private ComUIListScript mScrollList = null;
            #endregion
            IActivityTreasureLotteryActivityViewBase mCurrentView;
            IActivityTreasureLotteryDataMananger mDataManager;
            string mMainViewPrefabPath;
            string mMyLotteryViewPrefabPath;
            UnityAction mOnChangeSubView;
            public void Init(IActivityTreasureLotteryDataMananger dataManager, string mainViewItemPrefabPath, UnityAction onChangeSubView, EActivityTreasureLotteryView viewType = EActivityTreasureLotteryView.ActivityView)
            {
                mDataManager = dataManager;
                mMainViewPrefabPath = mainViewItemPrefabPath;
                mOnChangeSubView = onChangeSubView;
                BindEvents();
                ShowSubView(viewType);
            }

            public void Dispose()
            {
                UnBindEvents();
                DisposePreView();
                mDataManager = null;
                mOnChangeSubView = null;
            }

            public void UpdateData()
            {
                if (mCurrentView != null)
                {
                    mCurrentView.UpdateData();
                }
            }

            public void MainUpdateData()
            {
                if (mMainView != null)
                {
                    mMainView.UpdateWnd(mDataManager, mScrollList);
                }
            }

            public void ShowSubView(EActivityTreasureLotteryView viewType)
            {
                if (mToggleRank == null || mToggleMyLottery == null || mToggleMain == null)
                {
                    return;
                }

                switch (viewType)
                {
                    case EActivityTreasureLotteryView.ActivityView:
                        mToggleRank.isOn = false;
                        mToggleMyLottery.isOn = false;
                        mToggleMain.isOn = true;
                        break;
                    case EActivityTreasureLotteryView.HistoryView:
                        mToggleRank.isOn = true;
                        mToggleMyLottery.isOn = false;
                        mToggleMain.isOn = false;
                        break;
                    case EActivityTreasureLotteryView.MyLotteryView:
                        mToggleRank.isOn = false;
                        mToggleMyLottery.isOn = true;
                        mToggleMain.isOn = false;
                        break;
                }
            }

            void OnDestroy()
            {
                Dispose();
            }

            void BindEvents()
            {
                if (mToggleRank != null)
                {
                    mToggleRank.onValueChanged.AddListener(OnToggleHistory);
                }

                if (mToggleMain != null)
                {
                    mToggleMain.onValueChanged.AddListener(OnToggleMain);
                }

                if (mToggleMyLottery != null)
                {
                    mToggleMyLottery.onValueChanged.AddListener(OnToggleMyLottery);
                }
            }

            void UnBindEvents()
            {
                if (mToggleRank != null)
                {
                    mToggleRank.onValueChanged.RemoveListener(OnToggleHistory);
                }

                if (mToggleMain != null)
                {
                    mToggleMain.onValueChanged.RemoveListener(OnToggleMain);
                }

                if (mToggleMyLottery != null)
                {
                    mToggleMyLottery.onValueChanged.RemoveListener(OnToggleMyLottery);
                }
            }

            void OnToggleMain(bool value)
            {
                if (value)
                {
                    DisposePreView();

                    if (mMainView != null)
                    {
                        mMainView.Init(mDataManager, mMainViewPrefabPath, mScrollList);
                        CurrentSubView = EActivityTreasureLotteryView.ActivityView;
                        mCurrentView = mMainView;
                        mOnChangeSubView();
                    }
                }
            }
            void OnToggleMyLottery(bool value)
            {
                if (value)
                {
                    DisposePreView();
                    if (mMyLotteryView != null)
                    {
                        mMyLotteryView.Init(mDataManager, mMainViewPrefabPath, mScrollList);
                        CurrentSubView = EActivityTreasureLotteryView.MyLotteryView;
                        mCurrentView = mMyLotteryView;
                        mOnChangeSubView();
                    }
                }
            }
            void OnToggleHistory(bool value)
            {
                if (value)
                {
                    DisposePreView();
                    //mHistroyView.gameObject.SetActive(true);
                    if(mHistroyView != null)
                    {
                        mHistroyView.Init(mDataManager, mMainViewPrefabPath, mScrollList);
                        CurrentSubView = EActivityTreasureLotteryView.HistoryView;
                        mCurrentView = mHistroyView;
                        mOnChangeSubView();
                    }
                }
            }

            void DisposePreView()
            {
                if (mCurrentView != null)
                {
                    mCurrentView.Dispose();
                    mCurrentView = null;
                }
            }

            void Update()
            {
                if(mDataManager != null)
                {
                    switch (mDataManager.GetState())
                    {
                        case ETreasureLotterState.Prepare:
                            mTextTimeLeft.SafeSetText(TR.Value("activity_treasure_lottery_time_begin") + mDataManager.GetRemainTime());
                            break;
                        case ETreasureLotterState.Open:
                            mTextTimeLeft.SafeSetText(TR.Value("activity_treasure_lottery_time_end") + mDataManager.GetRemainTime());
                            break;
                    }
                }
            }
        }
    }
}