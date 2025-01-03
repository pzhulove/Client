using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SevenDaysLoginView : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mComUIList = null;
        [SerializeField] private TextEx mTextCountDown = null;
        [SerializeField] private TextEx mTextName = null;
        [SerializeField] private ImageEx mImageIcon = null;

        private SevenDaysFrame mSevenDaysFrame = null;
        private List<SevenDaysData> mSevenDaysLoginDatas = null;
        private bool mInited = false;
        public void Init(List<SevenDaysData> sevenDaysLoginDatas, SevenDaysFrame sevenDaysFrame)
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

            mSevenDaysLoginDatas = sevenDaysLoginDatas;
            if (mComUIList != null && mSevenDaysLoginDatas != null)
            {
                mComUIList.SetElementAmount(mSevenDaysLoginDatas.Count);
            }

            _InitGrandAward();
        }

        private void _InitGrandAward()
        {
            if (mSevenDaysLoginDatas != null && mSevenDaysLoginDatas.Count >= SevendaysDataManager.MaxDay 
                && mSevenDaysLoginDatas[SevendaysDataManager.MaxDay - 1].itemDatas != null 
                && mSevenDaysLoginDatas[SevendaysDataManager.MaxDay - 1].itemDatas.Count > 0
                && mSevenDaysLoginDatas[SevendaysDataManager.MaxDay - 1].itemDatas[0] != null)
            {
                ItemData itemData = mSevenDaysLoginDatas[SevendaysDataManager.MaxDay - 1].itemDatas[0];
                mTextName.SafeSetColor(GameUtility.Item.GetItemColor(itemData.Quality));
                mTextName.SafeSetText(itemData.Name);
                mImageIcon.SafeSetImage(itemData.Icon);
            }
        }

        private void _OnUpdate(ComUIListElementScript item)
        {
            if (item == null)
            {
                return;
            }

            if (mSevenDaysLoginDatas == null)
            {
                return;
            }

            if (item.m_index >= mSevenDaysLoginDatas.Count)
            {
                return;
            }

            var script = item.GetComponentInChildren<SevenDaysLoginAwardItem>();
            if (script != null)
            {
                script.Init(mSevenDaysLoginDatas[item.m_index], item.m_index, mSevenDaysFrame);
            }
        }

        public void BtnGrandAwardClick()
        {
            if (mSevenDaysLoginDatas != null && mSevenDaysLoginDatas.Count >= SevendaysDataManager.MaxDay
                && mSevenDaysLoginDatas[SevendaysDataManager.MaxDay - 1].itemDatas != null
                && mSevenDaysLoginDatas[SevendaysDataManager.MaxDay - 1].itemDatas.Count > 0
                && mSevenDaysLoginDatas[SevendaysDataManager.MaxDay - 1].itemDatas[0] != null
                && mSevenDaysFrame != null)
            {
                ItemData model = mSevenDaysLoginDatas[SevendaysDataManager.MaxDay - 1].itemDatas[0];
                mSevenDaysFrame.ShowTips(model);
            }
        }
    }
}
