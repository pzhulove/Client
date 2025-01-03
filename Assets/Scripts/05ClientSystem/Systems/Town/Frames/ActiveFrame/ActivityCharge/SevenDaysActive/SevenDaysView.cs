using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SevenDaysView : MonoBehaviour
    {
        public enum TabType
        {
            Login,
            Target,
            Preferential,
        }

        [SerializeField] private CommonTabToggleGroup mCommonTabToggleGroup = null;

        [SerializeField] private UIPrefabWrapper mLoginWrapper = null;
        [SerializeField] private UIPrefabWrapper mTargetWrapper = null;
        [SerializeField] private UIPrefabWrapper mPreperentialWrapper = null;

        [SerializeField] private TextEx mTextCountDown = null;
        [SerializeField] private StateController mStateController = null;

        private SevenDaysLoginView mSevenDaysLoginView = null;
        private SevenDaysTargetView mSevenDaysTargetView = null;
        private SevenDaysPreperentialView mSevenDaysPreperentialView = null;

        private List<SevenDaysData> mSevenDaysLoginDatas = null;

        private int mCurSelectIndex = -1;
        private SevenDaysFrame mSevenDaysFrme = null;
        private bool mInited = false;

        public void Init(SevenDaysFrame sevenDaysFrame)
        {
            if (!mInited)
            {
                mSevenDaysFrme = sevenDaysFrame;
                if (mCommonTabToggleGroup != null)
                {
                    mCommonTabToggleGroup.InitComTab(_ToggleClick);
                }
                _UpdateToggleRedPoint();

                mInited = true;
            }
        }

        public void UpdateView(SevenDaysActiveTable.eSevenDaysActiveType activeType = SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Login)
        {
            _UpdateWndByActiveType(activeType);
        }

        public void UpdateScore()
        {
            if (mCurSelectIndex == (int)TabType.Target)
            {
                if (mSevenDaysTargetView != null)
                {
                    mSevenDaysTargetView.UpdateScore();
                }
            }
        }

        public void UpdateGiftAward(int giftId, List<ItemData> itemDatas)
        {
            if (mSevenDaysPreperentialView != null)
            {
                mSevenDaysPreperentialView.UpdateGiftAward(giftId, itemDatas);
            }
        }

        private void _UpdateToggleRedPoint()
        {
            if (mCommonTabToggleGroup == null)
            {
                return;
            }

            mCommonTabToggleGroup.OnSetRedPoint((int)TabType.Login, SevendaysDataManager.GetInstance().IsLoginShowRedPoint());
            mCommonTabToggleGroup.OnSetRedPoint((int)TabType.Target, SevendaysDataManager.GetInstance().IsTargetShowRedPoint());
            mCommonTabToggleGroup.OnSetRedPoint((int)TabType.Preferential, SevendaysDataManager.GetInstance().IsPreferentialShowRedPoint());
        }

        //活动类型为null表示
        private void _UpdateWndByActiveType(SevenDaysActiveTable.eSevenDaysActiveType eSevenDaysActiveType)
        {
            switch(eSevenDaysActiveType)
            {
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Login:
                    {
                        if (mCurSelectIndex == (int)TabType.Login)
                        {
                            _UpdateLoginView();
                            if (mCommonTabToggleGroup != null)
                            {
                                mCommonTabToggleGroup.OnSetRedPoint((int)TabType.Login, SevendaysDataManager.GetInstance().IsLoginShowRedPoint());
                            }
                        }
                    }
                    break;
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Target:
                    {
                        if (mCurSelectIndex == (int)TabType.Target)
                        {
                            if(mSevenDaysTargetView != null)
                            {
                                mSevenDaysTargetView.UpdateView();
                                if (mCommonTabToggleGroup != null)
                                {
                                    mCommonTabToggleGroup.OnSetRedPoint((int)TabType.Target, SevendaysDataManager.GetInstance().IsTargetShowRedPoint());
                                }
                            }
                        }
                    }
                    break;
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Score:
                    {
                        if (mCurSelectIndex == (int)TabType.Target)
                        {
                            if (mSevenDaysTargetView != null)
                            {
                                mSevenDaysTargetView.UpdateScoreAndTab();
                                if (mCommonTabToggleGroup != null)
                                {
                                    mCommonTabToggleGroup.OnSetRedPoint((int)TabType.Target, SevendaysDataManager.GetInstance().IsTargetShowRedPoint());
                                }
                            }
                        }
                    }
                    break;
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Gift:
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Charge:
                    {
                        if (mCurSelectIndex == (int)TabType.Preferential)
                        {
                            if (mSevenDaysPreperentialView != null)
                            {
                                mSevenDaysPreperentialView.UpdateWndByActiveType(eSevenDaysActiveType);
                                if (mCommonTabToggleGroup != null)
                                {
                                    mCommonTabToggleGroup.OnSetRedPoint((int)TabType.Preferential, SevendaysDataManager.GetInstance().IsPreferentialShowRedPoint());
                                }
                            }
                        }
  
                    }
                    break;
            }
        }

        private void _UpdateLoginView()
        {
            if (mSevenDaysLoginView == null)
            {
                return;
            }

            mSevenDaysLoginDatas = SevendaysDataManager.GetInstance().GetSevenDaysLoginDatas();
            mSevenDaysLoginView.Init(mSevenDaysLoginDatas, mSevenDaysFrme);
        }

        private void _UpdateTargetView()
        {
            if (mSevenDaysTargetView == null)
            {
                return;
            }

            mSevenDaysTargetView.Init(mSevenDaysFrme);
        }

        private void _UpdatePreperentialView()
        {
            if (mSevenDaysPreperentialView == null)
            {
                return;
            }

            mSevenDaysPreperentialView.Init(mSevenDaysFrme);
        }


        private void _ToggleClick(CommonTabData data)
        {
            if (data == null)
            {
                return;
            }

            mCurSelectIndex = data.id;

            if (data.id == (int)TabType.Login)
            {
                if (mSevenDaysLoginView == null)
                {
                    mLoginWrapper.SetCallback(_LoginWrapperCallBack);
                    mLoginWrapper.Load();
                }
                else
                {
                    _UpdateLoginView();
                }
            }
            else if (data.id == (int)TabType.Target)
            {
                if (mSevenDaysTargetView == null)
                {
                    mTargetWrapper.SetCallback(_TargetWrapperCallBack);
                    mTargetWrapper.Load();
                }
                else
                {
                    _UpdateTargetView();
                }
            }
            else if (data.id == (int)TabType.Preferential)
            {
                if (mSevenDaysPreperentialView == null)
                {
                    mPreperentialWrapper.SetCallback(_PreperentialWrapperCallBack);
                    mPreperentialWrapper.Load();
                }
                else
                {
                    _UpdatePreperentialView();
                }
            }
            

            if (mStateController != null)
            {
                mStateController.Key = data.id.ToString();
            }
        }

        private void _LoginWrapperCallBack(string str, object asset, object userData)
        {
            if (asset == null)
            {
                return;
            }

            GameObject go = asset as GameObject;
            go.transform.SetParent(mLoginWrapper.transform, false);
            mSevenDaysLoginView = go.GetComponent<SevenDaysLoginView>();
            _UpdateLoginView();
        }

        private void _TargetWrapperCallBack(string str, object asset, object userData)
        {
            if (asset == null)
            {
                return;
            }

            GameObject go = asset as GameObject;
            go.transform.SetParent(mTargetWrapper.transform, false);
            mSevenDaysTargetView = go.GetComponent<SevenDaysTargetView>();
            _UpdateTargetView();
        }

        private void _PreperentialWrapperCallBack(string str, object asset, object userData)
        {
            if (asset == null)
            {
                return;
            }

            GameObject go = asset as GameObject;
            go.transform.SetParent(mPreperentialWrapper.transform, false);
            mSevenDaysPreperentialView = go.GetComponent<SevenDaysPreperentialView>();
            _UpdatePreperentialView();
        }

        public void Close()
        {
            if (mSevenDaysFrme != null)
            {
                mSevenDaysFrme.Close();
            }
        }
    }
}
