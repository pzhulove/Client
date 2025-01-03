using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using DG.Tweening;
using Protocol;
using Network;
using System.Net;
using System.IO;
using System.Text;
using XUPorterJSON;

namespace GameClient
{
    public class SecretAgreementFrame : ClientFrame
    {
        UserAgreementFrameData frameData = new UserAgreementFrameData();
        Coroutine mCo = null;
        Coroutine mSel = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Login/Publish/SecretAgreementFrame";
        }

        protected override void _OnOpenFrame()
        {
            if(userData != null)
            {
                frameData = (UserAgreementFrameData)userData;
            }

            mClose.gameObject.CustomActive(frameData.frameType == UserAgreementFrameType.LookInfo);
            mBtReject.gameObject.CustomActive(frameData.frameType == UserAgreementFrameType.FirstOpen);
            mBtAccept.gameObject.CustomActive(frameData.frameType == UserAgreementFrameType.FirstOpen);

            _StopContentCoroutine();
            _StopSelectCoroutine();
            mCo = GameFrameWork.instance.StartCoroutine(_wt());
        }

        protected override void _OnCloseFrame()
        {
            _StopContentCoroutine();
            _StopSelectCoroutine();
        }

        private IEnumerator _wt()
        {
            BaseWaitHttpRequest wt = new BaseWaitHttpRequest();
            wt.url = string.Format("http://{0}/secret_txt", Global.USER_AGREEMENT_SERVER_ADDRESS);

            yield return wt;

            mText.text = wt.GetResultString();
			mScroll.verticalNormalizedPosition = 1.0f;
        }

        private IEnumerator _sel()  
        {
            BaseWaitHttpRequest wt = new BaseWaitHttpRequest();
            wt.url = string.Format("http://{0}/agreement_agree?pf={1}&id={2}", Global.USER_AGREEMENT_SERVER_ADDRESS, frameData.PlatFormType, frameData.OpenUid);

            if(frameData.PlatFormType == "" || frameData.OpenUid == "")
            {
                Logger.LogErrorFormat("Agree [User Agreement] url = {0}", wt.url);
            }

            yield return wt;

            if (wt.GetResult() == BaseWaitHttpRequest.eState.Success)
            {
                frameMgr.CloseFrame(this);
            }
        }

        private void _StopContentCoroutine()
        {
            if (null != mCo)
            {
                GameFrameWork.instance.StopCoroutine(mCo);
            }

            mCo = null;
        }

        private void _StopSelectCoroutine()
        {
            if (mSel != null)
            {
                GameFrameWork.instance.StopCoroutine(mSel);
            }

            mSel = null;
        }

        #region ExtraUIBind
        private Button mClose = null;
        private Text mText = null;
        private ScrollRect mScroll = null;
        private Button mBtReject = null;
        private Button mBtAccept = null;

        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mText = mBind.GetCom<Text>("text");
            mScroll = mBind.GetCom<ScrollRect>("scroll");
            mBtReject = mBind.GetCom<Button>("btReject");
            mBtReject.onClick.AddListener(_onBtRejectButtonClick);
            mBtAccept = mBind.GetCom<Button>("btAccept");
            mBtAccept.onClick.AddListener(_onBtAcceptButtonClick);
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mText = null;
            mScroll = null;
            mBtReject.onClick.RemoveListener(_onBtRejectButtonClick);
            mBtReject = null;
            mBtAccept.onClick.RemoveListener(_onBtAcceptButtonClick);
            mBtAccept = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        private void _onBtRejectButtonClick()
        {
            Application.Quit();
        }
        private void _onBtAcceptButtonClick()
        {
            _StopSelectCoroutine();
            mSel = GameFrameWork.instance.StartCoroutine(_sel());
        }
        #endregion
    }
}
