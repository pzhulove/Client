using UnityEngine;
using UnityEngine.UI;
using GameClient;
using System;
using System.Diagnostics;
using UnityEngine.Events;

public enum eFrameType
{
    Null,
    Popup,
    FullScreen
}

public interface IComClientFrame
{
    int GetZOrder();

    string GetGroupTag();

    FrameLayer GetLayer();

    void SetCurrentFrame(string name);

    void SetGroupTag(string tag);

    bool IsNeedFade();

    bool IsNeedClearWhenChangeScene();

    void SetIsNeedClearWhenChangeScene(bool bFlag);

    bool IsInitWithGameBindSystem();

    eFrameType GetFrameType();

    bool IsFullScreenFrame();

    bool IsFullScreenFrameNeedBeClose();

    void InitAnimator();
}

[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Canvas))]
public class ComClientFrame : MonoBehaviour, IComClientFrame
{
    public FrameLayer mLayer = FrameLayer.Middle;   // 界面层级
    public eFrameType mFrameType;                   // 界面类型 
    public string mCurrentFrameName = "";           // 界面的Frame名称
    public int mZOrder = -1;

    [Header("界面分组(system/frame),非全屏界面需要把主界面弹开时填frame:")]
    public string mGroupTag = "";                   // 界面分组类型，system/frame

    [Header("使用黑底作为界面底部背景:")]
    public bool bUseBlackMask;                      // 是否使用黑底作为界面底部背景，一般情况下IsUseScreenShotMask和bUseBlackMask二选一就行了
    [Header("点击黑底是否关闭界面,要bUseBlackMask一起置为true才生效:")]
    public bool bBlackMaskClickAutoClose;           // 点击黑底是否关闭界面，使用该选项一定要bUseBlackMask一起置为true才生效
    [Header("以截屏做为界面底部背景:")]
    public bool IsUseScreenShotMask = false;        // 是否以截屏做为界面底部背景
    [Header("使用毛玻璃特效:")]
    public bool mNeedGlass = false;

    [Header("非全屏界面淡入淡出效果:")]
    public bool IsPredefineFade = false;            // 界面打开/关闭时，是否使用预定义Fadein/FadeOut动画效果.1.5大部分用于弹出式非全屏界面
    [Header("主界面icon淡入淡出:")]
    public bool mHiddenNeedFade = false;            // 目前专用于主界面icon的淡入淡出效果，目前只有主界面在用了，其余界面一概使用IsPredefineFade. by Wangbo 2020.10.09

    [Header("界面在打开状态下，切换场景是否关闭:")]
    public bool mClearWhenChangeScene = false;      // 界面在打开状态下，切换场景是否关闭
    [Header("界面关闭时是否需要GC:")]
    public bool bNewCloseNeedGC = false;            // 界面关闭时是否需要GC

    private bool bInitWithGameBindSystem = true;    // 不再暴露出去，就应该是true. by Wangbo 2020.10.09
    private IClientFrame mClientFrame = null;

    // 下面这些基本上应该没用了，先置为private测一下没问题后续可以删掉相关代码
    private bool bIsFullScreenFrameNeedBeClosed = false;
    private GameObject mGlassBg = null;

    //private bool bUseFadeIn = false;  // 这个功能废弃，使用2.0的IsPredefineFade的功能 by Wangbo 2020.07.11
    //private bool bUseFadeOut = false;

    void Awake()
    {
        // if (mNeedGlass)
        // {
        // }

        mClientFrame = null;
    }

    void OnDestroy()
    {
        _destroyGlass();

        mClientFrame = null;
    }

    void OnTransformParentChanged()
    {
        if (mNeedGlass)
        {
            _createGlass();
        }
    }

    void OnEnable()
    {
        if (mNeedGlass)
        {
            _createGlass();
        }
    }

    void OnDisable()
    {
        _destroyGlass();
    }

    //不使用该方法 使用和黑屏一样的方法创建毛玻璃特效
    private void _createGlass()
    {
        return;

        if (this.gameObject.transform.parent != null && mGlassBg == null && this.gameObject.activeSelf && enabled)
        {
            mGlassBg = new GameObject("__glass");

            Utility.AttachTo(mGlassBg, this.gameObject.transform.parent.gameObject);
            mGlassBg.layer = 5;

            int index = this.transform.GetSiblingIndex();
            mGlassBg.transform.SetSiblingIndex(index);

            mGlassBg.AddComponent<Canvas>();
            mGlassBg.AddComponent<RawImage>();
            //mGlassBg.AddComponent<FrostedGlass>();

            RectTransform trans = mGlassBg.GetComponent<RectTransform>();

            trans.anchorMax = Vector2.one * 0.5f;
            trans.anchorMin = Vector2.one * 0.5f;
            trans.sizeDelta = new Vector2(1920, 1082);
        }
    }

    private void _destroyGlass()
    {
        if (null != mGlassBg)
        {
            GameObject.Destroy(mGlassBg);
            mGlassBg = null;
        }
    }

    [Conditional("UNITY_EDITOR")]
    private void _updateRoot()
    {
        if (Application.isPlaying)
        {
            if (mLayer == FrameLayer.LayerMax)
            {
                mLayer = FrameLayer.Middle;
            }

            Type type = Type.GetType(mCurrentFrameName);
            if (null != type)
            {
                var frame = ClientSystemManager.instance.GetFrame(type);
                if (null != frame)
                {
                    frame.UpdateRoot();
                }
                else
                {
                    Logger.LogErrorFormat("can't find {0}", mCurrentFrameName);
                }
            }
        }
    }

    [Conditional("UNITY_EDITOR")]
    private void _updateMutexLayer()
    {
    }

    public void OnValidate()
    {
        _updateRoot();
        _updateMutexLayer();
    }

    #region Animator
    protected UIAnimator m_Animator = null;                                 // UI动画系统

    public void InitAnimator()
    {
        if (IsPredefineFade /*&& mFrameType == eFrameType.Popup*/)
        {
            m_Animator = gameObject.GetOrAddComponent<UIAnimator>();
            if (string.IsNullOrEmpty(m_Animator.m_PredefineFadeIn) && string.IsNullOrEmpty(m_Animator.m_PredefineFadeOut))
            {
                m_Animator.m_PredefineFadeIn = "FadeIn_Scale";
                m_Animator.m_PredefineFadeOut = "FadeOut_Scale";
            }
        }

        if (m_Animator)
        {
            m_Animator.Initialize(mClientFrame);
        }
    }

    public void FadeIn(UnityAction onFadeInOver)
    {
        if (m_Animator != null)
        {
            m_Animator.FadeIn(onFadeInOver);
        }
        else
        {
            onFadeInOver();
        }
    }

    public void FadeOut(UnityAction onFadeOutOver)
    {
        if (m_Animator != null)
        {
            m_Animator.FadeOut(onFadeOutOver);
        }
        else
        {
            onFadeOutOver();
        }
    }
    #endregion


    #region 接口
    public int GetZOrder()
    {
        return mZOrder;
    }

    public void SetCurrentFrame(string name)
    {
        mCurrentFrameName = name;
    }

    public void SetClientFrame(IClientFrame clientFrame)
    {
        mClientFrame = clientFrame;
    }


    public IClientFrame GetClientFrame()
    {
        return mClientFrame;
    }

    public string GetGroupTag()
    {
        return mGroupTag;
    }

    public bool IsNeedFade()
    {
        return mHiddenNeedFade;
    }

    public FrameLayer GetLayer()
    {
        return mLayer;
    }

    public bool IsNeedClearWhenChangeScene()
    {
        return mClearWhenChangeScene;
    }

    public void SetGroupTag(string tag)
    {
        mGroupTag = tag;
    }

    public void SetIsNeedClearWhenChangeScene(bool bFlag)
    {
        mClearWhenChangeScene = bFlag;
    }

    public bool IsInitWithGameBindSystem()
    {
        return bInitWithGameBindSystem;
    }

    public eFrameType GetFrameType()
    {
        return mFrameType;
    }

    public bool IsFullScreenFrameNeedBeClose()
    {
        return bIsFullScreenFrameNeedBeClosed;
    }

    public bool IsFullScreenFrame()
    {
        return eFrameType.FullScreen == mFrameType; 
    }
    #endregion
}
