using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

namespace GameClient
{
    public class DoTweenTrigger : MonoBehaviour
    {
        public List<GameObject> doTweenTriggers = null;
        List<DOTweenAnimation> akDoTweenComponents = new List<DOTweenAnimation>();//FadeIn
        List<DOTweenAnimation> akDoTweenComponentsOut = new List<DOTweenAnimation>();//FadeOut
        UnityEngine.Events.UnityEvent callbackFadeIn = new UnityEngine.Events.UnityEvent();
        UnityEngine.Events.UnityEvent callbackFadeOut = new UnityEngine.Events.UnityEvent();

        System.Int32 iEndCount = 0;
        bool bInitialize = false;
        enum FadeType
        {
            FT_In = 0,
            FT_Out,
        }
        FadeType eFadeType = FadeType.FT_In;
        // Use this for initialization
        void Start()
        {

        }

        void OnDestroy()
        {
            doTweenTriggers.Clear();

            for(int i = 0,icnt = akDoTweenComponents.Count;i<icnt;++i)
            {
                DOTweenAnimation cur = akDoTweenComponents[i];
                if(null == cur) continue;

                cur.DOKill();
            }

            for (int i = 0, icnt = akDoTweenComponentsOut.Count; i < icnt; ++i)
            {
                DOTweenAnimation cur = akDoTweenComponentsOut[i];
                if (null == cur) continue;

                cur.DOKill();
            }

            akDoTweenComponents.Clear();
            akDoTweenComponentsOut.Clear();

            callbackFadeIn.RemoveAllListeners();
            callbackFadeOut.RemoveAllListeners();
        }

        public void AddTriggerObject(GameObject go)
        {
            if(go != null)
            {
                if(doTweenTriggers == null)
                {
                    doTweenTriggers = new List<GameObject>();
                }
                doTweenTriggers.Add(go);
            }
        }

        public void Initialize()
        {
            if(!bInitialize)
            {
                bInitialize = true;
            }
            else
            {
                return;
            }
            iEndCount = 0;
            akDoTweenComponents.Clear();
            akDoTweenComponentsOut.Clear();
            if (doTweenTriggers != null)
            {
                for (int i = 0; i < doTweenTriggers.Count; ++i)
                {
                    if(doTweenTriggers[i] == null)
                    {
                        continue;
                    }
                    Component[] components = doTweenTriggers[i].GetComponents(typeof(DOTweenAnimation));
                    if(components != null && components.Length > 0)
                    {
                        for(int j = 0; j < components.Length; ++j)
                        {
                            DOTweenAnimation animation = components[j] as DOTweenAnimation;
                            if (animation != null)
                            {
                                animation.id = FadeType.FT_In.ToString();
                                animation.isFrom = true;
                                animation.CreateTween();
                                akDoTweenComponents.Add(animation);

                                var copyedComponent = Utility.CopyComponent<DOTweenAnimation>(animation, animation.target.gameObject, true);
                                if (copyedComponent != null)
                                {
                                    copyedComponent.isFrom = false;
                                    copyedComponent.id = FadeType.FT_Out.ToString();
                                    copyedComponent.CreateTween();
                                    akDoTweenComponentsOut.Add(copyedComponent);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CopyFrom(DoTweenTrigger target)
        {
            if(target != null)
            {
                iEndCount = 0;
                doTweenTriggers = null;
                akDoTweenComponents.Clear();
                for(int i = 0; i < target.akDoTweenComponents.Count; ++i)
                {
                    var copyComponent = Utility.CopyComponent(target.akDoTweenComponents[i], gameObject,true);
                    if(copyComponent != null)
                    {
                        copyComponent.target = transform;
                        copyComponent.CreateTween();
                        akDoTweenComponents.Add(copyComponent);
                    }
                }
                
                callbackFadeIn.RemoveAllListeners();
                callbackFadeOut.RemoveAllListeners();
            }
        }

        public void FadeIn(UnityEngine.Events.UnityAction callback = null)
        {
            if(callback != null)
            {
                callbackFadeIn.RemoveAllListeners();
                callbackFadeIn.AddListener(callback);
            }

            if (!transform.gameObject.activeSelf)
            {
                transform.gameObject.SetActive(true);
            }
            iEndCount = 0;
            float fDelayTime = 0.0f;

            for (int i = 0; i < akDoTweenComponents.Count; ++i)
            {
                akDoTweenComponents[i].DORewind();
                akDoTweenComponents[i].DOPlayById(FadeType.FT_In.ToString());
                fDelayTime = Mathf.Max((akDoTweenComponents[i].delay + akDoTweenComponents[i].duration), fDelayTime);
            }
            eFadeType = FadeType.FT_In;

            InvokeMethod.Invoke(fDelayTime, OnFadeInOver);
        }

        public void FadeOut(UnityEngine.Events.UnityAction callback = null)
        {
            if (callback != null)
            {
                callbackFadeOut.RemoveAllListeners();
                callbackFadeOut.AddListener(callback);
            }

            iEndCount = 0;
            float fDelayTime = 0.0f;
            for (int i = 0; i < akDoTweenComponentsOut.Count; ++i)
            {
                akDoTweenComponentsOut[i].DOPlayById(FadeType.FT_Out.ToString());
                fDelayTime = Mathf.Max((akDoTweenComponentsOut[i].delay + akDoTweenComponentsOut[i].duration), fDelayTime);
            }
            eFadeType = FadeType.FT_Out;

            InvokeMethod.Invoke(fDelayTime, OnFadeOutOver);
        }

        void OnFadeOutOver()
        {
            if(callbackFadeOut != null)
            {
                callbackFadeOut.Invoke();
            }
        }

        void OnFadeInOver()
        {
            if (callbackFadeIn != null)
            {
                callbackFadeIn.Invoke();
            }
        }
    }
}