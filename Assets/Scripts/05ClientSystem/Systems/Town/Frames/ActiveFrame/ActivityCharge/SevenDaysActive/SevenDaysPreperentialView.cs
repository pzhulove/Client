using ProtoTable;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SevenDaysPreperentialView : MonoBehaviour
    {
        private enum TabType
        {
            Charge,
            Gift,
        }

        [SerializeField] private ComUIListScript mComUIListScript = null;
        [SerializeField] private UIPrefabWrapper mGiftWrapper = null;
        [SerializeField] private UIPrefabWrapper mChargeWrapper = null;

        private List<string> mNames = new List<string>() {"七日累充", "七日礼包" };

        private SevenDaysGiftView mSevenDaysGiftView = null;
        private SevenDaysChargeView mSevenDaysChargeView = null;
        private SevenDaysFrame mSevenDaysFrame = null;
        private int mCurSelectToggle = -1;
        private bool mInited = false;
        
        public void Init(SevenDaysFrame sevenDaysFrame)
        {
            if (!mInited)
            {
                if (mComUIListScript != null)
                {
                    mComUIListScript.Initialize();
                    mComUIListScript.onItemVisiable = _OnItemUpdate;
                    mComUIListScript.OnItemUpdate = _OnItemUpdate;
                    mComUIListScript.onItemSelected = _OnItemSelect;
                    mComUIListScript.onItemChageDisplay = _OnChangeDisplay;
                }

                mSevenDaysFrame = sevenDaysFrame;

                mInited = true;
            }

            if (mComUIListScript != null)
            {
                int preId = mCurSelectToggle < 0 ? 0 : mCurSelectToggle;
                mComUIListScript.SetElementAmount(mNames.Count);
                mComUIListScript.SelectElement(preId, false);
            }
        }

        public void UpdateGiftAward(int giftId, List<ItemData> itemDatas)
        {
            if (mCurSelectToggle == (int)TabType.Gift && mSevenDaysGiftView != null)
            {
                mSevenDaysGiftView.UpdateGiftAward(giftId, itemDatas);
            }
        }

        public void UpdateWndByActiveType(SevenDaysActiveTable.eSevenDaysActiveType eSevenDaysActiveType)
        {
            if (mComUIListScript != null)
            {
                mComUIListScript.SetElementAmount(mNames.Count);
            }

            switch (eSevenDaysActiveType)
            {
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Gift:
                    {
                        if (mCurSelectToggle == (int)TabType.Gift)
                        {
                            _UpdateGiftView();
                        }
                    }
                    break;
                case SevenDaysActiveTable.eSevenDaysActiveType.SevenDays_Charge:
                    {
                        if (mCurSelectToggle == (int)TabType.Charge)
                        {
                            _UpdateChargeView();
                        }
                    }
                    break;
            }
        }

        private void _OnItemUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mNames == null || mNames.Count <= item.m_index)
            {
                return;
            }

            SevenDaysPrePerentialTabItem script = item.GetComponent<SevenDaysPrePerentialTabItem>();
            if (script != null)
            {
                bool isShowRedPoint = false;
                if (item.m_index == (int)TabType.Charge)
                {
                    isShowRedPoint = SevendaysDataManager.GetInstance().IsChargeShowRedPoint();
                }
                else if (item.m_index == (int)TabType.Gift)
                {
                    isShowRedPoint = SevendaysDataManager.GetInstance().IsGiftShowRedPoint();
                }
                script.Init(mNames[item.m_index], mCurSelectToggle == item.m_index, isShowRedPoint);
            }
        }

        private void _OnChangeDisplay(ComUIListElementScript item, bool isSelect)
        {
            if (item == null)
            {
                return;
            }

            SevenDaysPrePerentialTabItem script = item.GetComponent<SevenDaysPrePerentialTabItem>();
            if (mNames == null || mNames.Count <= item.m_index || script == null)
            {
                return;
            }

            bool isShowRedPoint = false;
            if (item.m_index == (int)TabType.Charge)
            {
                isShowRedPoint = SevendaysDataManager.GetInstance().IsChargeShowRedPoint();
            }
            else if (item.m_index == (int)TabType.Gift)
            {
                isShowRedPoint = SevendaysDataManager.GetInstance().IsGiftShowRedPoint();
            }
            script.Init(mNames[item.m_index], isSelect, isShowRedPoint);
        }

        private void _OnItemSelect(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mCurSelectToggle == item.m_index)
            {
                return;
            }

            mCurSelectToggle = item.m_index;
            if (mCurSelectToggle == (int)TabType.Gift)
            {
                if (mSevenDaysGiftView == null)
                {
                    mGiftWrapper.SetCallback(_GiftWrapperCallBack);
                    mGiftWrapper.Load();
                }
                else
                {
                    _UpdateGiftView();
                }
                SevendaysDataManager.GetInstance().SetGiftRedPointFlag();
                mGiftWrapper.CustomActive(true);
                mChargeWrapper.CustomActive(false);
            }
            else if (mCurSelectToggle == (int)TabType.Charge)
            {
                if (mSevenDaysChargeView == null)
                {
                    mChargeWrapper.SetCallback(_ChargeWrapperCallBack);
                    mChargeWrapper.Load();
                }
                else
                {
                    _UpdateChargeView();
                }

                mGiftWrapper.CustomActive(false);
                mChargeWrapper.CustomActive(true);
            }
        }

        

        private void _UpdateGiftView()
        {
            if (mSevenDaysGiftView == null)
            {
                return;
            }

            mSevenDaysGiftView.Init(mSevenDaysFrame);
        }

        private void _UpdateChargeView()
        {
            if (mSevenDaysChargeView == null)
            {
                return;
            }

            mSevenDaysChargeView.Init(mSevenDaysFrame);
        }

        private void _ChargeWrapperCallBack(string str, object asset, object userData)
        {
            if (asset == null)
            {
                return;
            }

            GameObject go = asset as GameObject;
            go.transform.SetParent(mChargeWrapper.transform, false);
            mSevenDaysChargeView = go.GetComponent<SevenDaysChargeView>();
            _UpdateChargeView();
        }

        private void _GiftWrapperCallBack(string str, object asset, object userData)
        {
            if (asset == null)
            {
                return;
            }

            GameObject go = asset as GameObject;
            go.transform.SetParent(mGiftWrapper.transform, false);
            mSevenDaysGiftView = go.GetComponent<SevenDaysGiftView>();
            _UpdateGiftView();
        }
    }
}
