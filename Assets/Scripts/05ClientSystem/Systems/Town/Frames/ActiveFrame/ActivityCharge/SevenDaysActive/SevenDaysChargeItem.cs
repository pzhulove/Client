using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SevenDaysChargeItem : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mComUIList = null;
        [SerializeField] private StateController mStateController = null;
        [SerializeField] private TextEx mTextChargeTarget = null;
        [SerializeField] private TextEx mTextProgress = null;

        private SevenDaysFrame mSevenDaysFrame = null;
        private SevenDaysChargeData mSevenDaysChargeData = null;
        private bool mInited = false;

        public void Init(SevenDaysFrame sevenDaysFrame, SevenDaysChargeData sevenDaysChargeData)
        {
            if (!mInited)
            {
                if (mComUIList != null)
                {
                    mComUIList.Initialize();
                    mComUIList.onItemVisiable = _OnUpdate;
                    mComUIList.OnItemUpdate = _OnUpdate;
                }

                mSevenDaysFrame = sevenDaysFrame;

                mInited = true;
            }

            mSevenDaysChargeData = sevenDaysChargeData;
            if (mSevenDaysChargeData == null)
            {
                return;
            }

            mTextProgress.SafeSetText(mSevenDaysChargeData.curValueStr + "/" + mSevenDaysChargeData.targetCharge);
            mTextChargeTarget.SafeSetText(string.Format(TR.Value("seven_day_charge_target_format"), mSevenDaysChargeData.targetCharge));

            if (mComUIList != null && mSevenDaysChargeData.itemDatas != null)
            {
                mComUIList.SetElementAmount(mSevenDaysChargeData.itemDatas.Count);
            }

            if (mStateController != null)
            {
                mStateController.Key = ((int)mSevenDaysChargeData.taskStatus).ToString();
            }
        }

        private void _OnUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mSevenDaysChargeData == null || mSevenDaysChargeData.itemDatas == null
                || mSevenDaysChargeData.itemDatas.Count <= item.m_index || mSevenDaysChargeData.itemDatas[item.m_index] == null)
            {
                return;
            }

            ComItemNew com = item.GetComponentInChildren<ComItemNew>();
            if (com != null)
            {
                com.Setup(mSevenDaysChargeData.itemDatas[item.m_index], null, true);
                com.SetCount(mSevenDaysChargeData.itemDatas[item.m_index].Count.ToString());
            }
        }

        public void BtnGoClick()
        {
            if (mSevenDaysFrame == null || mSevenDaysChargeData == null
                || mSevenDaysChargeData.activeTable == null)
            {
                return;
            }

            mSevenDaysFrame.GoFunction(mSevenDaysChargeData.activeTable);
        }

        public void BtnGetAward()
        {
            if (mSevenDaysFrame == null || mSevenDaysChargeData == null)
            {
                return;
            }

            mSevenDaysFrame.SubmitActive(mSevenDaysChargeData.activeId);
        }
    }
}
