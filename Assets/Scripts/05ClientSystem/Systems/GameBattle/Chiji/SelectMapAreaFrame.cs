using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Network;
using Protocol;

namespace GameClient
{
    public class SelectMapAreaFrame : ClientFrame
    {
        const int AreaNum = 6;

        private int LeftTime = 15;
        private float fTimeIntrval = 0.0f;

        private int iSliderTime = 15;
        private float fSliderTimer = 0.0f;

        private int AreaIndex = 0;
            
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/SelectMapAreaFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            if (mTime != null)
            {
                mTime.text = LeftTime.ToString();
            }

            for (int i = 0; i < tgArea.Length; i++)
            {
                tgArea[i].image.alphaHitTestMinimumThreshold = 1.0f;
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            _ClearData();
        }

        private void _ClearData()
        {
            LeftTime = 15;
            fTimeIntrval = 0.0f;
            AreaIndex = 0;
            iSliderTime = 15;
            fSliderTimer = 0;
        }

        [UIEventHandle("Areas/Area_{0}", typeof(Toggle), typeof(UnityEngine.Events.UnityAction<int, bool>), 0, AreaNum - 1)]
        void OnSelectArea(int iIndex, bool value)
        {
            if (iIndex < 0 || !value)
            {
                return;
            }

            AreaIndex = iIndex; 
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            fTimeIntrval += timeElapsed;
            fSliderTimer += timeElapsed;

            if (fTimeIntrval >= 1.0f)
            {
                LeftTime -= (int)fTimeIntrval;

                if (mTime != null)
                {
                    mTime.text = string.Format("{0}", LeftTime);
                }

                fTimeIntrval = 0.0f;
            }

            if (mSlider != null)
            {
                mSlider.value = 1.0f - fSliderTimer / iSliderTime;
            }

            if(LeftTime <= 0)
            {
                ChijiDataManager.GetInstance().SendSelectAreaId(AreaIndex);
                frameMgr.CloseFrame(this);
            }
        }

        [UIControl("Areas/Area_{0}", typeof(Toggle), 0)]
        Toggle[] tgArea = new Toggle[AreaNum];

        #region ExtraUIBind
        private Button mBtOK = null;
        private Text mTime = null;
        private Slider mSlider = null;

        protected override void _bindExUI()
        {
            mBtOK = mBind.GetCom<Button>("btOK");
            if (null != mBtOK)
            {
                mBtOK.onClick.AddListener(_onBtOKButtonClick);
            }
            mTime = mBind.GetCom<Text>("Time");
            mSlider = mBind.GetCom<Slider>("Slider");
        }

        protected override void _unbindExUI()
        {
            if (null != mBtOK)
            {
                mBtOK.onClick.RemoveListener(_onBtOKButtonClick);
            }
            mBtOK = null;
            mTime = null;
            mSlider = null;
        }
        #endregion

        #region Callback
        private void _onBtOKButtonClick()
        {
            ChijiDataManager.GetInstance().SendSelectAreaId(AreaIndex + 1);
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
