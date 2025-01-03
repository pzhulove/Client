using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SevenDaysLoginAwardItem : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mComUIList = null;
        [SerializeField] private TextEx mTextDay = null;
        [SerializeField] private StateController mStateController = null;
        [SerializeField] private CanvasGroup mCanvasMain = null;
        [SerializeField] private float mAlphaOver = 0.6f;

        private SevenDaysFrame mSevenDaysFrame = null;
        private SevenDaysData mSevenDaysLoginData = null;
        private bool mInited = false;
        public void Init(SevenDaysData sevenDaysLoginData, int index, SevenDaysFrame sevenDaysFrame)
        {
            if (sevenDaysLoginData == null)
            {
                return;
            }

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

            mSevenDaysLoginData = sevenDaysLoginData;
            if (sevenDaysLoginData.itemDatas != null && mComUIList != null)
            {
                mComUIList.SetElementAmount(sevenDaysLoginData.itemDatas.Count);
            }

            if (mStateController != null)
            {
                mStateController.Key = ((int)sevenDaysLoginData.taskStatus).ToString();
            }

            if (mCanvasMain != null && sevenDaysLoginData != null)
            {
                if (sevenDaysLoginData.taskStatus == Protocol.TaskStatus.TASK_OVER)
                {
                    mCanvasMain.alpha = mAlphaOver;
                }
                else
                {
                    mCanvasMain.alpha = 1f;
                }
            }

            mTextDay.SafeSetText(string.Format("第{0}天", index + 1));
        }

        private void _OnUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mSevenDaysLoginData == null || mSevenDaysLoginData.itemDatas == null)
            {
                return;
            }

            if (item.m_index >= mSevenDaysLoginData.itemDatas.Count)
            {
                return;
            }

            ComItemNew comItemNew = item.GetComponentInChildren<ComItemNew>();
            if (comItemNew != null)
            {
                ItemData itemData = mSevenDaysLoginData.itemDatas[item.m_index];
                comItemNew.Setup(itemData, null, true);
                comItemNew.SetCount(itemData.Count.ToString());
            }
        }

        public void BtnGetAwardClick()
        {
            if (mSevenDaysLoginData == null || mSevenDaysFrame == null || mSevenDaysLoginData.taskStatus != Protocol.TaskStatus.TASK_FINISHED)
            {
                return;
            }

            mSevenDaysFrame.SubmitActive(mSevenDaysLoginData.activeId);
        }
    }
}
