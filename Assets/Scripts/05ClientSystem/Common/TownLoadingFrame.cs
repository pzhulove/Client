using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using System.Collections;

using UnityEngine.UI;

namespace GameClient
{
    class TownLoadingFrame : ClientFrame, ITownFadingFrame
    {      
        protected int mUpdateSpeed     = 10;

        protected int mTargetProgress  = 0;
        protected int mCurrentProgress = -1;

        protected float mFadeOutTime   = 0.0f;
        protected float mFadeInTime    = 0.0f;

        protected List<BeBaseActor> m_LoadingCheck = new List<BeBaseActor>();
        protected float mMaxWaitingTime = 10.0f;/// 最多等十秒钟

        public int CurrentProgress
        {
            get { return mCurrentProgress; }
        }

        protected override void _OnOpenFrame()
        {
#if UNITY_EDITOR
            mUpdateSpeed = Global.Settings.loadingProgressStepInEditor;
#else
            mUpdateSpeed = Global.Settings.loadingProgressStep;
#endif

            //GameObject.DontDestroyOnLoad(frame);
            mTargetProgress = 0;
            mCurrentProgress = -1;
            mFadeInTime = 0.0f;
            mFadeOutTime = 0.0f;
            mMaxWaitingTime = 10.0f;

            _randomTips();

            StartCoroutine(_updateProgress());
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/TownLoadingFrame";
        }

        private void _randomTips()
        {
            int MaxNum = TableManager.GetInstance().GetTableItemCount<ProtoTable.TipsTable>();
            int TableIndex = UnityEngine.Random.Range(1, MaxNum);

            var TipsData = TableManager.GetInstance().GetTableItemByIndex<ProtoTable.TipsTable>(TableIndex);
            if (TipsData != null)
            {
                tipsText.text = "温馨提示:" + TipsData.ObjectName;
            }
        }

        protected IEnumerator _updateProgress()
        {
            while (mTargetProgress <= 100)
            {
                while (mCurrentProgress < mTargetProgress)
                {
                    mCurrentProgress += mUpdateSpeed;
                    if (mCurrentProgress > mTargetProgress)
                    {
                        mCurrentProgress = mTargetProgress;
                    }

                    _setProgress(mCurrentProgress);
                    yield return Yielders.EndOfFrame;
                }


                if (mTargetProgress == 100 && _IsLoadingFinished())
                {
                    yield return GameFrameWork.instance.OpenFadeFrame(()=>
                    {
                        frameMgr.CloseFrame(this);
                    },
                    ()=>
                    {

                    }); 

                    break;
                }

                yield return Yielders.EndOfFrame;
            }
        }

        protected void _setProgress(int progress)
        {
            if (progress < 0)
            {
                progress = 0;
            }
            if (progress > 100)
            {
                progress = 100;
            }
            loadProcess.value = progress / 100.0f;
        }


        public void FadingOut(float fadeOutTime)
        {
            m_state = EFrameState.FadeOut;
            mFadeOutTime = fadeOutTime;
            mTargetProgress = 85;
        }

        public void FadingIn(float fadeInTime)
        {
            m_state = EFrameState.FadeIn;
            mFadeInTime = fadeInTime;
            mTargetProgress = 20;
        }

        public bool IsClosed()
        {
            return m_state == EFrameState.Close;
        }

        public bool IsOpened()
        {
            return m_state == EFrameState.Open;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        public void AddCheckActor(BeBaseActor actor)
        {
            m_LoadingCheck.Add(actor);
        }

        private bool _IsLoadingFinished()
        {
            bool loadFinish = true;

            for (int i = 0,icnt = m_LoadingCheck.Count;i<icnt ; ++i)
            {
                BeBaseActor curActor = m_LoadingCheck[i];
                if (null != curActor)
                {
                    if (null != curActor.GraphicActor)
                        loadFinish = loadFinish ? curActor.GraphicActor.IsAvatarLoadFinished() : loadFinish;
                }
            }

            if (mMaxWaitingTime <= 0f)
                loadFinish = true;

            return loadFinish;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            mMaxWaitingTime -= timeElapsed;
            switch (m_state)
            {
                case EFrameState.FadeIn:
                    mFadeInTime -= timeElapsed;
                    if (mFadeInTime < 0)
                    {
                        m_state = EFrameState.Open;
                        mTargetProgress = 60;
                    }
                    break;
                case EFrameState.FadeOut:
                    mFadeOutTime -= timeElapsed;
                    if (mFadeOutTime < 0.0f)
                    {
                        mTargetProgress = 100;
                    }
                    break;
            }
        }

        [UIControl("loading")]
        Slider loadProcess;

        [UIControl("loadText")]
        Text tipsText;
    }
}
