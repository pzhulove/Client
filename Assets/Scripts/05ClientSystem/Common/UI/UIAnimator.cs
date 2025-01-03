using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameClient
{
    public class UIAnimator : MonoBehaviour
    {
        enum UIAnimationState
        {
            None = 0,
            FadeIn,
            FadeOut,
            Custom
        }

        private static int FadeIn_StateHash = -1;
        private static int FadeOut_StateHash = -1;
        private static string FadeInName = null;
        private static string FadeOutName = null;

        public string m_PredefineFadeIn = "FadeIn_Scale";
        public string m_PredefineFadeOut = "FadeOut_Scale";

        [SerializeField] private bool mIsCustom = false;  //是否自定义动画
        [SerializeField] private List<DOTweenAnimation> mCustomFadeIn;        // DOTweenAnimation淡入动画
        [SerializeField] private List<DOTweenAnimation> mCustomFadeOut;        // DOTweenAnimation淡入动画

        private bool m_UsePredefinedFade;

        private List<DOTweenAnimation> m_DoTweenAnimationFadeOut = null;        // DOTweenAnimation淡出动画
        private List<DOTweenAnimation> m_DoTweenAnimationFadeIn = null;        // 由m_DoTweenAnimationFadeOut自动生成的淡入动画
        private List<DOTweenAnimation> m_DoTweenAnimationCustom = null;        // DOTweenAnimation自定义动画，不受窗口淡入淡出控制播放
        private List<Animator> m_Animator = null;                               // Animator

        private UIAnimationState m_AnimationState = UIAnimationState.None;      // 当前动画状态
        private bool m_Initialized = false;
        private IClientFrame m_Window;

        private float m_FadeInLength = -1f;                                     // 淡入动画时间
        private float m_FadeOutLength = -1f;                                    // 淡出动画时间
        private Dictionary<string, float> m_CustomAnimationLength = null;       // 其他Custom动画时间

        private UnityEvent m_CallbackFadeIn = new UnityEvent();   // 淡入结束通知
        private UnityEvent m_CallbackFadeOut = new UnityEvent();   // 淡出结束通知
        private Dictionary<string, UnityEvent> m_CallbackCustom;   // Custom动画结束通知
        private float mDelta;
        private float mDurtaion;

        // Use this for initialization
        private void Start()
        {

        }

        private void FixedUpdate()
        {
            switch(m_AnimationState)
            {
                case UIAnimationState.FadeOut:
                    mDelta += Time.deltaTime;
                    if (mDelta >= mDurtaion)
                    {
                        OnFadeOutOver();
                    }
                    break;
            }
        }

        private void OnDestroy()
        {
            KillDOTweenAnimations(m_DoTweenAnimationFadeOut);
            KillDOTweenAnimations(m_DoTweenAnimationFadeIn);
            KillDOTweenAnimations(m_DoTweenAnimationCustom);
            KillDOTweenAnimations(mCustomFadeIn);
            KillDOTweenAnimations(mCustomFadeOut);

            m_Animator = null;

            m_CallbackFadeIn.RemoveAllListeners();
            m_CallbackFadeOut.RemoveAllListeners();
            if (m_CallbackCustom != null)
            {
                foreach (KeyValuePair<string, UnityEvent> customEvent in m_CallbackCustom)
                {
                    customEvent.Value.RemoveAllListeners();
                }
                m_CallbackCustom.Clear();
            }
        }

        private void KillDOTweenAnimations(List<DOTweenAnimation> dotweens)
        {
            if (dotweens == null)
                return;

            for (int i = 0, icnt = dotweens.Count; i < icnt; ++i)
            {
                DOTweenAnimation cur = dotweens[i];
                if (null == cur) continue;

                cur.DOKill();
            }

            dotweens.Clear();
        }

        public void Initialize(IClientFrame window)
        {
            if (m_Initialized)
                return;

            m_Initialized = true;
            m_Window = window;

            FadeInName = UIAnimationState.FadeIn.ToString();
            FadeOutName = UIAnimationState.FadeOut.ToString();

            if (FadeIn_StateHash == -1)
            {
                FadeIn_StateHash = Animator.StringToHash(FadeInName);
                FadeOut_StateHash = Animator.StringToHash(FadeOutName);
            }

            m_UsePredefinedFade = !string.IsNullOrEmpty(m_PredefineFadeIn) || !string.IsNullOrEmpty(m_PredefineFadeOut);

            if (!mIsCustom)
            {
                //RegisterAnimators(GetComponentsInChildren<Animator>(true));
                //RegisterDOTweenAnimation(GetComponentsInChildren<DOTweenAnimation>(true));

                if (m_UsePredefinedFade)
                {
                    InitPredefinedAnimation();
                }
            }
        }

        /// <summary>
        /// 创建预定义动画
        /// </summary>
        /// <param name="predefineAnimation"></param>
        private void InitPredefinedAnimation()
        {
            //if (!string.IsNullOrEmpty(m_PredefineFadeOut))
            //{
            //    GameObject goPrefab = UIManager.GetInstance().GetPredefinedAnimationObject(m_PredefineFadeOut);
            //    if(goPrefab != null)
            //    {
            //        Component[] components = goPrefab.GetComponents<DOTweenAnimation>();
            //        if (components.Length > 0)
            //        {
            //            if(m_DoTweenAnimationFadeOut == null)
            //                m_DoTweenAnimationFadeOut = new List<DOTweenAnimation>();

            //            for (int j = 0; j < components.Length; ++j)
            //            {
            //                var current = global::Utility.CopyComponent<DOTweenAnimation>(components[j] as DOTweenAnimation, base.gameObject, true);
            //                if (current != null)
            //                {
            //                    if (current.animationType == DOTweenAnimationType.Fade)
            //                    {
            //                        current.target = gameObject.GetOrAddComponent<CanvasGroup>();
            //                    }
            //                    else
            //                    {
            //                        current.target = base.transform;
            //                    }
            //                    current.id = FadeOutName;
            //                    //current.CreateTween();
            //                    //current.SetIsCreateTween();
            //                    m_DoTweenAnimationFadeOut.Add(current);
            //                }
            //            }
            //        }
            //    }
            //}

            if (!string.IsNullOrEmpty(m_PredefineFadeIn))
            {
                GameObject goPrefab = UIManager.GetInstance().GetPredefinedAnimationObject(m_PredefineFadeIn);
                if (goPrefab != null)
                {
                    Component[] components = goPrefab.GetComponents<DOTweenAnimation>();
                    if (components.Length > 0)
                    {
                        if (m_DoTweenAnimationFadeIn == null)
                            m_DoTweenAnimationFadeIn = new List<DOTweenAnimation>();

                        for (int j = 0; j < components.Length; ++j)
                        {
                            var current = global::Utility.CopyComponent<DOTweenAnimation>(components[j] as DOTweenAnimation, base.gameObject, true);
                            if (current != null)
                            {
                                if (current.animationType == DOTweenAnimationType.Fade)
                                {
                                    current.target = gameObject.GetOrAddComponent<CanvasGroup>();
                                }
                                else
                                {
                                    current.target = base.transform;
                                }                            
                                current.id = FadeInName;
                                current.CreateTween();
                                if (current.target is CanvasGroup)
                                {
                                    var cg = current.target as CanvasGroup;
                                    //Debug.LogFormat("fade in {0}, {1} {2}", cg.alpha, current.endValueFloat, current.IsTweenCreated());
                                }
                                current.SetIsCreateTween();
                                m_DoTweenAnimationFadeIn.Add(current);
                            }
                        }
                    }
                }
            }
        }

        public void FadeIn(UnityAction callback = null)
        {
            //Debug.Log("UIAnimator Fade in " + gameObject.name );
            if (callback != null)
            {
                m_CallbackFadeIn.RemoveAllListeners();
                m_CallbackFadeIn.AddListener(callback);
            }

            if (!transform.gameObject.activeSelf)
            {
                transform.gameObject.SetActive(true);
            }

            if (mIsCustom)
            {
         //       Debug.Log("UIAnimator Fade in custom");
                if (mCustomFadeIn != null)
                {
                    for (int i = 0; i < mCustomFadeIn.Count; ++i)
                    {
                        if (mCustomFadeIn[i] != null && mCustomFadeIn[i].isActive)
                        {
                            mCustomFadeIn[i].DORewind();
                            mCustomFadeIn[i].DOPlay();
                        }
                    }
                }
            }
            else
            {
                if (m_DoTweenAnimationFadeIn != null)
                {
                    for (int i = 0; i < m_DoTweenAnimationFadeIn.Count; ++i)
                    {
                        //Debug.LogFormat("fade in {0}", m_DoTweenAnimationFadeIn[i].name);
                        if (m_DoTweenAnimationFadeIn[i] != null && m_DoTweenAnimationFadeIn[i].isActive)
                        {
                            m_DoTweenAnimationFadeIn[i].DORewind();
                            m_DoTweenAnimationFadeIn[i].DOPlayById(FadeInName);
                        }
                    }
                }

                if (!m_UsePredefinedFade)
                {
                    if (m_Animator != null)
                    {
                        for (int i = 0; i < m_Animator.Count; ++i)
                        {
                            if (m_Animator[i] != null && m_Animator[i].isActiveAndEnabled)
                            {
                                m_Animator[i].Play(FadeIn_StateHash);
                            }
                        }
                    }
                }
            }

            /*
                        if(m_UsePredefinedFade && WindowBlackMask.GetOwnerWindow() == m_Window)
                        {
                            WindowBlackMask.FadeIn();
                        }*/

            m_AnimationState = UIAnimationState.FadeIn;

            // 换成1.0方式
            Invoke("OnFadeInOver", GetFadeInLength());
        }

        public void FadeOut(UnityAction callback = null)
        {
            if (!string.IsNullOrEmpty(m_PredefineFadeOut))
            {
                GameObject goPrefab = UIManager.GetInstance().GetPredefinedAnimationObject(m_PredefineFadeOut);
                if (goPrefab != null)
                {
                    Component[] components = goPrefab.GetComponents<DOTweenAnimation>();
                    if (components.Length > 0)
                    {
                        if (m_DoTweenAnimationFadeOut == null)
                        {
                            m_DoTweenAnimationFadeOut = new List<DOTweenAnimation>();
                        }
                        else
                        {
                            m_DoTweenAnimationFadeOut.Clear();
                        }

                        for (int j = 0; j < components.Length; ++j)
                        {
                            var current = global::Utility.CopyComponent<DOTweenAnimation>(components[j] as DOTweenAnimation, base.gameObject, true);
                            if (current != null)
                            {
                                if (current.animationType == DOTweenAnimationType.Fade)
                                {
                                    current.target = gameObject.GetOrAddComponent<CanvasGroup>();
                                }
                                else
                                {
                                    current.target = base.transform;
                                }
                                current.id = FadeOutName;
                                current.CreateTween();
                                //current.SetIsCreateTween();
                                m_DoTweenAnimationFadeOut.Add(current);
                            }
                        }
                    }
                }
            }

            //    Debug.Log("UIAnimator Fade out");
            if (callback != null)
            {
                m_CallbackFadeOut.RemoveAllListeners();
                m_CallbackFadeOut.AddListener(callback);
            }

            if (!transform.gameObject.activeSelf)
            {
                transform.gameObject.SetActive(true);
            }

            if (mIsCustom)
            {
                if (mCustomFadeOut != null)
                {
                    for (int i = 0; i < mCustomFadeOut.Count; ++i)
                    {
                        if (mCustomFadeOut[i] != null && mCustomFadeOut[i].isActive)
                        {
                            mCustomFadeOut[i].DORewind();
                            mCustomFadeOut[i].DOPlay();
                        }
                    }
                }
            }
            else
            {
                if (m_DoTweenAnimationFadeOut != null)
                {
                    for (int i = 0; i < m_DoTweenAnimationFadeOut.Count; ++i)
                    {
                 //       Debug.LogFormat("fade out {0}", m_DoTweenAnimationFadeOut[i].name);
                        if (m_DoTweenAnimationFadeOut[i] != null && m_DoTweenAnimationFadeOut[i].isActive)
                        {
                            //m_DoTweenAnimationFadeOut[i].DORewind();
                            m_DoTweenAnimationFadeOut[i].DOPlayById(FadeOutName);
                        }
                    }
                }

                if (!m_UsePredefinedFade)
                {
                    if (m_Animator != null)
                    {
                        for (int i = 0; i < m_Animator.Count; ++i)
                        {
                            if (m_Animator[i] != null && m_Animator[i].isActiveAndEnabled)
                            {
                                m_Animator[i].Play(FadeOut_StateHash);
                            }
                        }
                    }
                }

                //if (m_UsePredefinedFade && WindowBlackMask.GetOwnerWindow() == m_Window)
                //{
                //    WindowBlackMask.FadeOut();
                //}
            }

            m_AnimationState = UIAnimationState.FadeOut;
            mDelta = 0;
            mDurtaion = GetFadeOutLength();
            // 换成1.0方式
            Invoke("OnFadeOutOver", GetFadeOutLength());
        }

        public void PlayCustomAnimation(string animationName, UnityAction callback = null)
        {
            if (m_CallbackCustom == null)
            {
                m_CallbackCustom = new Dictionary<string, UnityEvent>();
            }

            UnityEvent callBacks;
            if (!m_CallbackCustom.TryGetValue(animationName, out callBacks))
            {
                callBacks = new UnityEvent();
                m_CallbackCustom.Add(animationName, callBacks);
            }

            if (callback != null)
            {
                callBacks.RemoveAllListeners();
                callBacks.AddListener(callback);
            }

            if (!transform.gameObject.activeSelf)
            {
                transform.gameObject.SetActive(true);
            }

            if (m_DoTweenAnimationCustom != null)
            {
                for (int i = 0; i < m_DoTweenAnimationCustom.Count; ++i)
                {
                    if (m_DoTweenAnimationCustom[i] != null && m_DoTweenAnimationCustom[i].id == animationName && m_DoTweenAnimationCustom[i].isActive)
                    {
                        m_DoTweenAnimationCustom[i].DORewind();
                        m_DoTweenAnimationCustom[i].DOPlayById(animationName);
                    }
                }
            }

            if (m_Animator != null)
            {
                for (int i = 0; i < m_Animator.Count; ++i)
                {
                    if (m_Animator[i] != null && m_Animator[i].HasState(0, Animator.StringToHash(animationName)) && m_Animator[i].isActiveAndEnabled)
                    {
                        m_Animator[i].Play(animationName);
                    }
                }
            }

            m_AnimationState = UIAnimationState.Custom;

            // 换成1.0方式
            // Invoke("OnFadeOutOver", GetFadeOutLength());
        }


        private void OnFadeOutOver()
        {
            if (m_AnimationState != UIAnimationState.FadeOut)
                return;
            if (m_CallbackFadeOut != null)
            {
                m_CallbackFadeOut.Invoke();
            }

            m_AnimationState = UIAnimationState.None;
        }

        private void OnFadeInOver()
        {
            if (m_AnimationState != UIAnimationState.FadeIn)
                return;
            if (m_CallbackFadeIn != null)
            {
                m_CallbackFadeIn.Invoke();
            }

            m_AnimationState = UIAnimationState.None;
        }

        private float GetCustomAnimationLength(string name)
        {
            if (m_CustomAnimationLength == null)
            {
                m_CustomAnimationLength = new Dictionary<string, float>();
            }

            float fLength = -1f;
            if (m_CustomAnimationLength.TryGetValue(name, out fLength))
            {
                return fLength;
            }

            if (m_DoTweenAnimationCustom != null)
            {
                for (int i = 0; i < m_DoTweenAnimationCustom.Count; ++i)
                {
                    if (m_DoTweenAnimationCustom[i] != null && m_DoTweenAnimationCustom[i].id == name)
                    {
                        fLength = Mathf.Max((m_DoTweenAnimationCustom[i].delay + m_DoTweenAnimationCustom[i].duration), fLength);
                    }
                }
            }

            if (m_Animator != null)
            {
                for (int i = 0; i < m_Animator.Count; ++i)
                {
                    if (m_Animator[i].HasState(0, Animator.StringToHash(name)))
                    {
                        AnimatorStateInfo stateInfo = m_Animator[i].GetCurrentAnimatorStateInfo(0);
                        fLength = Mathf.Max(stateInfo.length, fLength);
                    }
                }
            }

            m_CustomAnimationLength.Add(name, fLength);

            return fLength;
        }

        private float GetFadeInLength()
        {
            if (m_FadeInLength >= 0)
                return m_FadeInLength;

            if (mIsCustom)
            {
                if (mCustomFadeIn != null)
                {
                    for (int i = 0; i < mCustomFadeIn.Count; ++i)
                    {
                        if (mCustomFadeIn[i] == null)
                            continue;

                        m_FadeInLength = Mathf.Max((mCustomFadeIn[i].delay + mCustomFadeIn[i].duration), m_FadeInLength);
                    }
                }
            }
            else
            {
                if (m_DoTweenAnimationFadeIn != null)
                {
                    for (int i = 0; i < m_DoTweenAnimationFadeIn.Count; ++i)
                    {
                        if (m_DoTweenAnimationFadeIn[i] == null)
                            continue;

                        m_FadeInLength = Mathf.Max((m_DoTweenAnimationFadeIn[i].delay + m_DoTweenAnimationFadeIn[i].duration), m_FadeInLength);
                    }
                }

                if (!m_UsePredefinedFade && m_Animator != null)
                {
                    for (int i = 0; i < m_Animator.Count; ++i)
                    {
                        if (m_Animator[i].HasState(0, FadeIn_StateHash))
                        {
                            AnimatorStateInfo stateInfo = m_Animator[i].GetCurrentAnimatorStateInfo(0);
                            m_FadeInLength = Mathf.Max(stateInfo.length, m_FadeInLength);
                        }
                    }
                }
            }

            return m_FadeInLength;
        }

        private float GetFadeOutLength()
        {
            if (m_FadeOutLength >= 0)
                return m_FadeOutLength;

            if (mIsCustom)
            {
                if (mCustomFadeOut != null)
                {
                    for (int i = 0; i < mCustomFadeOut.Count; ++i)
                    {
                        if (mCustomFadeOut[i] == null)
                            continue;

                        m_FadeOutLength = Mathf.Max((mCustomFadeOut[i].delay + mCustomFadeOut[i].duration), m_FadeOutLength);
                    }
                }
            }
            else
            {
                if (m_DoTweenAnimationFadeOut != null)
                {
                    for (int i = 0; i < m_DoTweenAnimationFadeOut.Count; ++i)
                    {
                        if (m_DoTweenAnimationFadeOut[i] == null)
                            continue;

                        m_FadeOutLength = Mathf.Max((m_DoTweenAnimationFadeOut[i].delay + m_DoTweenAnimationFadeOut[i].duration), m_FadeOutLength);
                    }
                }

                if (!m_UsePredefinedFade && m_Animator != null)
                {
                    for (int i = 0; i < m_Animator.Count; ++i)
                    {
                        if (m_Animator[i].HasState(0, FadeOut_StateHash))
                        {
                            AnimatorStateInfo stateInfo = m_Animator[i].GetCurrentAnimatorStateInfo(0);
                            m_FadeOutLength = Mathf.Max(stateInfo.length, m_FadeOutLength);
                        }
                    }
                }
            }

            return m_FadeOutLength;
        }

        public void RegisterDOTweenAnimation(DOTweenAnimation[] animations)
        {
            if (animations.Length == 0)
                return;

            if (m_DoTweenAnimationFadeOut == null)
            {
                m_DoTweenAnimationFadeOut = new List<DOTweenAnimation>();
                m_DoTweenAnimationFadeIn = new List<DOTweenAnimation>();
                m_DoTweenAnimationCustom = new List<DOTweenAnimation>();
            }

            bool hasFade = false;
            bool hasCustom = false;
            foreach (DOTweenAnimation animation in animations)
            {
                // id没有设置就是fade动画，否则视为custom动画
                if (string.IsNullOrEmpty(animation.id))
                {
                    // 有PredefineFade动画，忽略无名称的Dotween动画
                    if (m_UsePredefinedFade)
                        continue;

                    animation.id = FadeOutName;
                    animation.isFrom = false;
                    animation.CreateTween();    // 修改了任何属性，都需要重新CreateTween
                    m_DoTweenAnimationFadeOut.Add(animation);

                    var copyedComponent = global::Utility.CopyComponent<DOTweenAnimation>(animation, animation.gameObject, true);
                    if (copyedComponent != null)
                    {
                        copyedComponent.isFrom = true;
                        copyedComponent.id = FadeInName;
                        copyedComponent.CreateTween();
                        m_DoTweenAnimationFadeIn.Add(copyedComponent);
                    }

                    hasFade = true;
                }
                else
                {
                    m_DoTweenAnimationCustom.Add(animation);
                    hasCustom = true;
                }
            }

            if (hasFade)
            {
                m_FadeInLength = m_FadeOutLength = -1f;
            }

            if (hasCustom && m_CustomAnimationLength != null)
            {
                m_CustomAnimationLength.Clear();
            }
        }

        public void RegisterAnimators(Animator[] animator)
        {
            if (animator.Length == 0)
                return;

            if (m_Animator == null)
            {
                m_Animator = new List<Animator>(animator.Length);
            }

            m_Animator.AddRange(animator);

            // 动画时间失效
            for (int i = 0; i < animator.Length; ++i)
            {
                if (animator[i].HasState(0, FadeIn_StateHash))
                {
                    m_FadeInLength = -1f;
                }

                if (animator[i].HasState(0, FadeOut_StateHash))
                {
                    m_FadeOutLength = -1f;
                }
            }

            if (m_CustomAnimationLength != null)
                m_CustomAnimationLength.Clear();
        }
    }
}