using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SevenDaysTargetContentItem : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mComUIList = null;
        [SerializeField] private TextEx mTextName = null;
        [SerializeField] private TextEx mTextDesc = null;
        [SerializeField] private TextEx mTextProgress = null;
        [SerializeField] private TextEx mTextScore = null;
        [SerializeField] private StateController mStateController = null;

        private SevenDaysTargetData mSevenDaysTargetData;
        private SevenDaysFrame mSevenDaysFrame;
        private bool mInited = false;
        public void Init(SevenDaysTargetData data, SevenDaysFrame sevenDaysFrame)
        {
            if (!mInited)
            {
                if (mComUIList != null)
                {
                    mComUIList.Initialize();
                    mComUIList.onItemVisiable = _OnUpdateItem;
                    mComUIList.OnItemUpdate = _OnUpdateItem;
                }
                mSevenDaysFrame = sevenDaysFrame;

                mInited = true;
            }

            if (data == null)
            {
                return;
            }

            mSevenDaysTargetData = data;

            mTextProgress.SafeSetText(data.curValueStr + "/" + data.targetValue);
            mTextDesc.SafeSetText(data.desc);
            mTextName.SafeSetText(data.name);
            if (data.scoreNum > 0)
            {
                mTextScore.SafeSetText(string.Format(TR.Value("seven_day_score_format"),data.scoreNum.ToString()));
            }
            else
            {
                mTextScore.SafeSetText(string.Empty);
            }

            if (mStateController != null)
            {
                mStateController.Key = ((int)data.taskStatus).ToString();
            }

            if (mComUIList != null)
            {
                mComUIList.SetElementAmount(data.itemDatas.Count);
            }
        }

        private void _OnUpdateItem(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mSevenDaysTargetData == null || mSevenDaysTargetData.itemDatas == null || item.m_index >= mSevenDaysTargetData.itemDatas.Count)
            {
                return;
            }

            ComItemNew comItemNew = item.GetComponentInChildren<ComItemNew>();
            if (comItemNew != null)
            {
                comItemNew.Setup(mSevenDaysTargetData.itemDatas[item.m_index], null, true);
                comItemNew.SetCount(mSevenDaysTargetData.itemDatas[item.m_index].Count.ToString());
            }
        }

        public void BtnGoClick()
        {
            if (mSevenDaysFrame == null || mSevenDaysTargetData == null || mSevenDaysTargetData.activeTable == null)
            {
                return;
            }

            mSevenDaysFrame.GoFunction(mSevenDaysTargetData.activeTable);
        }

        public void BtnGetAwardClick()
        {
            if (mSevenDaysFrame == null || mSevenDaysTargetData == null || mSevenDaysTargetData.activeTable == null)
            {
                return;
            }

            mSevenDaysFrame.SubmitActive(mSevenDaysTargetData.activeTable.ID);
        }
    }
}
