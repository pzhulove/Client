using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class SevenDaysChargeView : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mComUIListScript = null;

        private List<SevenDaysChargeData> mSevenDaysChargeDatas = null;
        private SevenDaysFrame mSevenDaysFrame = null;
        private bool mInited = false;

        public void Init(SevenDaysFrame sevenDaysFrame)
        {
            if (!mInited)
            {
                if (mComUIListScript != null)
                {
                    mComUIListScript.Initialize();
                    mComUIListScript.onItemVisiable = _OnUpdate;
                    mComUIListScript.OnItemUpdate = _OnUpdate;
                }

                mSevenDaysFrame = sevenDaysFrame;

                mInited = true;
            }

            mSevenDaysChargeDatas = SevendaysDataManager.GetInstance().GetSevenDaysChargeDatas();
            if (mComUIListScript != null && mSevenDaysChargeDatas != null)
            {
                mComUIListScript.SetElementAmount(mSevenDaysChargeDatas.Count);
            }
        }

        private void _OnUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mSevenDaysChargeDatas == null || mSevenDaysChargeDatas.Count <= item.m_index)
            {
                return;
            }

            SevenDaysChargeItem charge = item.GetComponent<SevenDaysChargeItem>();
            if (charge != null)
            {
                charge.Init(mSevenDaysFrame, mSevenDaysChargeDatas[item.m_index]);
            }
        }
    }
}
