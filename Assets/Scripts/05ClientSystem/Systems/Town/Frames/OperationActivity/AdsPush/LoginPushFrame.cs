using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System;
using GameClient;

namespace AdsPush
{
    public class LoginPushFrame:ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/PushAds/AdsItem";
        }

        protected override void _OnOpenFrame()
        {
            InitPushData();
        }

        private Vector2 mOriginalSize = new Vector2(1408, 717);
        private void InitPushData()
        {
            //init BgIconPath
            string BgIconPath = LoginPushManager.GetInstance().GetPushIconPath();
            if (BgIconPath != ""&&mContentImg!=null&& mContentImgRect!=null)
            {
                mContentImg.sprite = AssetLoader.instance.LoadRes(BgIconPath, typeof(Sprite)).obj as Sprite;

               if(LoginPushManager.GetInstance().IsSetNative())
                {
                    mContentImg.SetNativeSize();
                }
                else
                {
                    mContentImgRect.sizeDelta = mOriginalSize;
                }
               
            }

            string activityTime = LoginPushManager.GetInstance().GetPushTime();
            if(activityTime == null)
            {
                mTime.CustomActive(false);
            }
            else
            {
                mTime.text = activityTime;
                mTime.CustomActive(true);
            }

            var linkType = LoginPushManager.GetInstance().getLinkType();
            mGoKnowBtnGo.CustomActive(false);

            if(linkType == null)
            {
                mGoKnowBtnGo.CustomActive(false);
                return;
            }

            if (linkType == typeof(ActivityJarFrame))
            {
                mGoKnowBtnGo.CustomActive(true);
                mGoKnowBtn.onClick.RemoveAllListeners();
                mGoKnowBtn.onClick.AddListener(() =>
                {
                    ClientSystemManager.GetInstance().OpenFrame<ActivityJarFrame>();
                });
            }
            //init Bt
            //LoginPushManager.GetInstance().GetBtGo
        }

        protected override void _OnCloseFrame()
        {
            LoginPushManager.GetInstance().TryOpenLoginPushFrame();
        }

        

        #region ExtraUIBind
        private Image mContentImg = null;
        private Button mContentBtn = null;
        private GameObject mContentLoading = null;
        private Button mKnowBtn = null;
        private GameObject mKnowBtnGo = null;
        private Button mGoKnowBtn = null;
        private GameObject mGoKnowBtnGo = null;
        private Text mTime = null;

        private RectTransform mContentImgRect;

        protected override void _bindExUI()
        {
            mContentImg = mBind.GetCom<Image>("ContentImg");
            mContentBtn = mBind.GetCom<Button>("ContentBtn");
            mContentBtn.onClick.AddListener(_onContentBtnButtonClick);
            mContentLoading = mBind.GetGameObject("ContentLoading");
            mKnowBtn = mBind.GetCom<Button>("KnowBtn");
            mKnowBtn.onClick.AddListener(_onKnowBtnButtonClick);
            mKnowBtnGo = mBind.GetGameObject("KnowBtnGo");
            mGoKnowBtn = mBind.GetCom<Button>("GoKnowBtn");
            mGoKnowBtn.onClick.AddListener(_onGoKnowBtnButtonClick);
            mGoKnowBtnGo = mBind.GetGameObject("GoKnowBtnGo");
            mTime = mBind.GetCom<Text>("time");
            mContentImgRect = mBind.GetCom<RectTransform>("ContentImgRect");
        }

        protected override void _unbindExUI()
        {
            mContentImg = null;
            mContentBtn.onClick.RemoveListener(_onContentBtnButtonClick);
            mContentBtn = null;
            mContentLoading = null;
            mKnowBtn.onClick.RemoveListener(_onKnowBtnButtonClick);
            mKnowBtn = null;
            mKnowBtnGo = null;
            mGoKnowBtn.onClick.RemoveListener(_onGoKnowBtnButtonClick);
            mGoKnowBtn = null;
            mGoKnowBtnGo = null;
            mTime = null;
            mContentImgRect = null;
        }
        #endregion

        #region Callback
        private void _onContentBtnButtonClick()
        {
            /* put your code in here */

        }
        private void _onKnowBtnButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        private void _onGoKnowBtnButtonClick()
        {
            /* put your code in here */

        }
        #endregion

    }
}