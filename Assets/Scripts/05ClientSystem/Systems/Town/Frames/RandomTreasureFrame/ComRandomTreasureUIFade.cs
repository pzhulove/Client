using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class ComRandomTreasureUIFade : MonoBehaviour
    {
        #region Model Params

        private UnityAction fadeInStart;
        private UnityAction fadeInEnd;
        private UnityAction fadeOutStart;
        private UnityAction fadeOutEnd;

        private bool bInited = false;

        #endregion
        
        #region View Params

        [SerializeField]
        private GameObject mMapsContentRoot = null;
        [Header("保持Y值最大为1，只调整X轴作为动画时间")]
        [SerializeField]
        private AnimationCurve mAnimCurve = null;
        [SerializeField]
        private float mAnimSpeed = 1f;
        [SerializeField]
        private bool bHeightDoAnim = true;
        [SerializeField]
        private float mAnimOpenDelay = 0f;
        [SerializeField]
        private float mAnimCloseDelay = 0f;

        private RectTransform mMapsContentRootRect = null;
        private float mRectOriginalHeight = 0f;
        private float mRectOriginalWidth = 0f;
        private float mRectCurrentHeight = 0f;
        private float mRectCurrentWidth = 0f;
        private bool bShow = false;
        private bool bDirty = false;

        private Keyframe mAnimCurveLastKey = new Keyframe();
        private float mAnimTimer = 0f;
        private float mAnimTotalTime = 0f;

        private UnityEngine.Coroutine waitToCloseAtlas = null;
        private UnityEngine.Coroutine waitToOpenAtlas = null;

        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        //void Awake()
        //{
        //}
        
        //Unity life cycle
        //void Start () 
        //{

        //}
        
        //Unity life cycle
        void Update () 
        {
            if (!bDirty)
            {
                return;
            }
            if (mMapsContentRootRect == null || mAnimCurve == null)
            {
                return;
            }
            if (bHeightDoAnim)
            {
                if (mAnimTimer > mAnimTotalTime)
                {
                    bDirty = false;
                    mAnimTimer = 0f;
                    _ExecuteFadeEndHandler();
                    _RecoverAtlasRect();
                    return;
                }
                if (bShow)
                {
                    mRectCurrentHeight = mAnimCurve.Evaluate(mAnimTimer / mAnimTotalTime) * mRectOriginalHeight;
                }
                else
                {
                    mRectCurrentHeight = mRectOriginalHeight - (mAnimCurve.Evaluate(mAnimTimer / mAnimTotalTime) * mRectOriginalHeight);
                }
                mMapsContentRootRect.sizeDelta = new Vector2(mRectOriginalWidth, mRectCurrentHeight);
                //Logger.LogError("bShow = " + bShow);
                //Logger.LogError("mAnimTimer = " + mAnimTimer);
                //Logger.LogError("mAnimCurve.Evaluate = " + mAnimCurve.Evaluate(mAnimTimer / mAnimTotalTime));
                //Logger.LogError("mRectCurrentHeight = " + mRectCurrentHeight);
            }
            else
            {
                if (mAnimTimer > mAnimTotalTime)
                {
                    bDirty = false;
                    mAnimTimer = 0f;
                    _ExecuteFadeEndHandler();
                    _RecoverAtlasRect();
                    return;
                }
                if (bShow)
                {
                    mRectCurrentWidth = mAnimCurve.Evaluate(mAnimTimer / mAnimTotalTime) * mRectOriginalWidth;
                }
                else
                {
                    mRectCurrentWidth = mRectOriginalWidth - (mAnimCurve.Evaluate(mAnimTimer / mAnimTotalTime) * mRectOriginalWidth);
                }
                mMapsContentRootRect.sizeDelta = new Vector2(mRectCurrentWidth, mRectOriginalHeight);
            }

            mAnimTimer += Time.deltaTime * mAnimTotalTime * mAnimSpeed;
        }
        
        //Unity life cycle
        void OnDestroy () 
        {
            fadeInStart = null;
            fadeInEnd = null;
            fadeOutStart = null;
            fadeOutEnd = null;

            _Clear();
        }

        private void _Clear()
        {
            mMapsContentRootRect = null;
            mRectOriginalHeight = 0f;
            mRectOriginalWidth = 0f;
            mRectCurrentHeight = 0f;
            mRectCurrentWidth = 0f;
            bShow = false;
            bDirty = false;

            mAnimCurveLastKey = new Keyframe();
            mAnimTimer = 0f;
            mAnimTotalTime = 0f;

            if (waitToCloseAtlas != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToCloseAtlas);
                waitToCloseAtlas = null;
            }

            if (waitToOpenAtlas != null)
            {
                GameFrameWork.instance.StopCoroutine(waitToOpenAtlas);
                waitToOpenAtlas = null;
            }

            bInited = false;
        }

        private void _ShowAtlasRectAnim(bool bShow)
        {
            if (this.bShow != bShow)
            {
                bDirty = true;
                this.bShow = bShow;
                _ExecuteFadeStartHandler();
            }
        }

        private void _ExecuteFadeEndHandler()
        {
            if (bShow)
            {
                if (fadeOutEnd != null)
                {
                    fadeOutEnd();
                }
            }
            else
            {
                if(fadeInEnd != null)
                {
                    fadeInEnd();
                }
            }
        }

        private void _ExecuteFadeStartHandler()
        {
            if (bShow)
            {
                if (fadeOutStart != null)
                {
                    fadeOutStart();
                }
            }
            else
            {
                if (fadeInStart != null)
                {
                    fadeInStart();
                }
            }
        }

        private void _RestartAtlasRect(bool toShow)
        {
            mRectCurrentWidth = mRectOriginalWidth;
            mRectCurrentHeight = mRectOriginalHeight;
            if (bHeightDoAnim)
            {
                //尝试打开，从0.1开始做动画，防止使用排版组件的父节点宽或高为0时 排版内容无法隐藏的问题
                if (toShow)
                {
                    mRectCurrentHeight = 0.1f;
                }
            }
            else
            {
                if (toShow)
                {
                    mRectCurrentWidth = 0.1f;
                }
            }
            if (mMapsContentRootRect)
            {
                mMapsContentRootRect.sizeDelta = new Vector2(mRectCurrentWidth, mRectCurrentHeight);
            }
        }

        private void _RecoverAtlasRect()
        {
            if (bHeightDoAnim)
            {
                if (bShow)
                {
                    mRectCurrentHeight = mRectOriginalHeight;
                }
                else
                {
                    mRectCurrentHeight = 0f;
                }
                if (mMapsContentRootRect)
                {
                    mMapsContentRootRect.sizeDelta = new Vector2(mRectOriginalWidth, mRectCurrentHeight);
                }
            }
            else
            {
                if (bShow)
                {
                    mRectCurrentWidth = mRectOriginalWidth;
                }
                else
                {
                    mRectCurrentWidth = 0f;
                }
                if (mMapsContentRootRect)
                {
                    mMapsContentRootRect.sizeDelta = new Vector2(mRectCurrentWidth, mRectOriginalHeight);
                }
            }
        }

        private void _OnFadeInStart()
        {
        }

        private void _OnFadeOutStart()
        {
        }

        private void _OnFadeInEnd()
        {
        }

        private void _OnFadeOutEnd()
        {
        }

        private void _ReadyOpenAtlas()
        {
            _RestartAtlasRect(true);
            _ShowAtlasRectAnim(true);
        }

        private void _ReadyCloseAtlas()
        {
            _RestartAtlasRect(false);
            _ShowAtlasRectAnim(false);
        }

        IEnumerator _WaitToOpenAtlas()
        {
            if (mAnimOpenDelay <= 0f)
            {
                _ReadyOpenAtlas();
                yield break;
            }
            yield return new WaitForSecondsRealtime(mAnimOpenDelay);
            _ReadyOpenAtlas();
        }

        IEnumerator _WaitToCloseAtlas()
        {
            if (mAnimCloseDelay <= 0f)
            {
                _ReadyCloseAtlas();
                yield break;                
            }
            yield return new WaitForSecondsRealtime(mAnimCloseDelay);
            _ReadyCloseAtlas();
        }

        #endregion
        
        #region  PUBLIC METHODS

        public void InitView()
        {
            if (bInited)
            {
                return;
            }
            if (mMapsContentRoot)
            {
                mMapsContentRootRect = mMapsContentRoot.GetComponent<RectTransform>();
                if (mMapsContentRootRect)
                {
                    mRectOriginalHeight = mMapsContentRootRect.sizeDelta.y;
                    mRectOriginalWidth = mMapsContentRootRect.sizeDelta.x;
                }
            }
            if (mAnimCurve != null && mAnimCurve.length > 0)
            {
                mAnimCurveLastKey = mAnimCurve[mAnimCurve.length - 1];
                mAnimTotalTime = mAnimCurveLastKey.time;
            }
            //初始化重置
            _RestartAtlasRect(true);
            bInited = true;
        }

        public void StartOpenAtlas(UnityAction openAtlasStart, UnityAction openAtlasEnd, bool bDelay = false)
        {
            this.fadeOutStart = openAtlasStart;
            this.fadeOutEnd = openAtlasEnd;
            if (bDelay)
            {
                if (waitToOpenAtlas != null)
                {
                    GameFrameWork.instance.StopCoroutine(waitToOpenAtlas);
                }
                waitToOpenAtlas = GameFrameWork.instance.StartCoroutine(_WaitToOpenAtlas());
            }
            else {
                _ReadyOpenAtlas();
            }
        }

        public void StartCloseAtlas(UnityAction closeAtlasStart, UnityAction closeAtlasEnd, bool bDelay = false)
        {
            this.fadeInStart = closeAtlasStart;
            this.fadeInEnd = closeAtlasEnd;
            if (bDelay)
            {
                if (waitToCloseAtlas != null)
                {
                    GameFrameWork.instance.StopCoroutine(waitToCloseAtlas);
                }
                waitToCloseAtlas = GameFrameWork.instance.StartCoroutine(_WaitToCloseAtlas());
            }
            else {
                _ReadyCloseAtlas();
            }
        }
        
        #endregion

#if UNITY_EDITOR

        public void OpenAtlas()
        {
            Logger.LogError("[ComRandomTreasureUIFade Editor] - Open");
            _RestartAtlasRect(true);
            _ShowAtlasRectAnim(true);
        }

        public void CloseAtlas()
        {
            Logger.LogError("[ComRandomTreasureUIFade Editor] - Close");
            _RestartAtlasRect(false);
            _ShowAtlasRectAnim(false);
        }

#endif
    }
}