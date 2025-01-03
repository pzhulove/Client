using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using UnityEngine.Assertions;
using DG.Tweening;

namespace GameClient
{
    public enum EEffectEvent
    {
        Invalid = -1,

        SeasonLevel_StartChangeLevel,
        SeasonLevel_ChangeLevelIcon,
        SeasonLevel_ChangeLevelName,
        SeasonLevel_ChangeLevelStar,
        SeasonLevel_FinishChangeLevel,
    }


    [System.Serializable]
    public class EffectInfo
    {
        public GameObject effect = null;
        public string tag = string.Empty;
        public float startTime = 0.0f;
    }

    [System.Serializable]
    public class EventInfo
    {
        public EEffectEvent effectEvent = EEffectEvent.Invalid;
        public float startTime = 0.0f;
    }

    public class ComEffectController : MonoBehaviour
    {
        public bool test = false;
        public EffectInfo[] arrEffectInfos = new EffectInfo[0];
        public EventInfo[] arrEventInfos = new EventInfo[0];
        public float duration = 0.0f;
        public bool loop = false;


        class EffectRuntime
        {
            public GameObject objEffect = null;
            public float fRemainTime = 0.0f;
            public string strTag = string.Empty;
            public bool bStarted = false;

            public bool bOldActive = false;
        }

        class EventRuntime
        {
            public EEffectEvent eEventID = EEffectEvent.Invalid;
            public float fRemainTime = 0.0f;
            public bool bStarted = false;
        }

        bool m_bPlaying = false;
        float m_fRemainTime = 0.0f;
        List<EffectRuntime> m_arrEffectRuntimes = new List<EffectRuntime>();
        List<EventRuntime> m_arrEventRuntimes = new List<EventRuntime>();
        Dictionary<EEffectEvent, List<Action>> m_eventProcessors = new Dictionary<EEffectEvent, List<Action>>();

        void OnValidate()
        {
            if (test)
            {
                Stop();
                Play();
                test = false;
            }
        }

        void Update()
        {
            if (m_bPlaying)
            {
                #region effect
                for (int i = 0; i < m_arrEffectRuntimes.Count; ++i)
                {
                    if (m_arrEffectRuntimes[i].bStarted == false)
                    {
                        m_arrEffectRuntimes[i].fRemainTime -= Time.deltaTime;
                        if (m_arrEffectRuntimes[i].fRemainTime <= 0.0f)
                        {
                            GameObject objRoot = m_arrEffectRuntimes[i].objEffect;
                            objRoot.SetActive(true);

                            {
                                GeUIEffectParticle[] pars = objRoot.GetComponentsInChildren<GeUIEffectParticle>();
                                for (int j = 0; j < pars.Length; ++j)
                                {
                                    pars[j].StartEmit();
                                }
                            }

                            {
                                ParticleSystem[] pars = objRoot.GetComponentsInChildren<ParticleSystem>();
                                for (int j = 0; j < pars.Length; ++j)
                                {
                                    pars[j].Play();
                                }
                            }

                            {
                                DOTweenAnimation[] anims = objRoot.GetComponentsInChildren<DOTweenAnimation>();
                                for (int j = 0; j < anims.Length; ++j)
                                {
                                    if (anims[j].id == m_arrEffectRuntimes[i].strTag)
                                    {
                                        anims[j].isActive = true;
                                        if (anims[j].tween == null)
                                        {
                                            anims[j].CreateTween();
                                        }
                                        anims[j].tween.Restart();
                                    }
                                }
                            }

                            m_arrEffectRuntimes[i].bStarted = true;
                        }
                    }
                }
                #endregion

                #region event
                for (int i = 0; i < m_arrEventRuntimes.Count; ++i)
                {
                    if (m_arrEventRuntimes[i].bStarted == false)
                    {
                        m_arrEventRuntimes[i].fRemainTime -= Time.deltaTime;
                        if (m_arrEventRuntimes[i].fRemainTime <= 0.0f)
                        {
                            try
                            {
                                List<Action> eventHandlers;
                                m_eventProcessors.TryGetValue(m_arrEventRuntimes[i].eEventID, out eventHandlers);
                                if (eventHandlers != null)
                                {
                                    List<Action> temp = new List<Action>(eventHandlers);
                                    for (int j = 0; j < eventHandlers.Count; ++j)
                                    {
                                        temp[j]();
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Logger.LogError(e.ToString());
                            }

                            m_arrEffectRuntimes[i].bStarted = true;
                        }
                    }
                }
                #endregion

                if (loop == false)
                {
                    m_fRemainTime -= Time.deltaTime;
                    if (m_fRemainTime <= 0.0f)
                    {
                        Stop();
                    }
                }
            }
        }

        public void RegisterEvent(EEffectEvent a_event, Action a_callback)
        {
            if (m_eventProcessors.ContainsKey(a_event) == false)
            {
                m_eventProcessors.Add(a_event, new List<Action>());
            }

            List<Action> eventHandlers = m_eventProcessors[a_event];
            if (eventHandlers.Contains(a_callback) == false)
            {
                eventHandlers.Add(a_callback);
            }
        }

        public void UnRegisterEvent(EEffectEvent a_event, Action a_callback)
        {
            List<Action> eventHandlers;
            m_eventProcessors.TryGetValue(a_event, out eventHandlers);
            if (eventHandlers != null)
            {
                eventHandlers.Remove(a_callback);
            }
        }

        public void Play()
        {
            m_bPlaying = true;
            m_fRemainTime = duration;

            m_arrEffectRuntimes.Clear();
            for (int i = 0; i < arrEffectInfos.Length; ++i)
            {
                EffectRuntime info = new EffectRuntime();
                info.objEffect = arrEffectInfos[i].effect;
                info.fRemainTime = arrEffectInfos[i].startTime;
                info.strTag = arrEffectInfos[i].tag;
                info.bStarted = false;
                info.bOldActive = info.objEffect.activeSelf;
                m_arrEffectRuntimes.Add(info);
            }

            m_arrEventRuntimes.Clear();
            for (int i = 0; i < arrEventInfos.Length; ++i)
            {
                EventRuntime runtime = new EventRuntime();
                runtime.eEventID = arrEventInfos[i].effectEvent;
                runtime.fRemainTime = arrEventInfos[i].startTime;
                runtime.bStarted = false;
                m_arrEventRuntimes.Add(runtime);
            }
        }

        public void Stop()
        {
            m_bPlaying = false;
            m_fRemainTime = 0.0f;
            for (int i = 0; i < m_arrEffectRuntimes.Count; ++i)
            {
                EffectRuntime info = m_arrEffectRuntimes[i];
                if (info.bStarted)
                {
                    GameObject objRoot = m_arrEffectRuntimes[i].objEffect;

                    {
                        GeUIEffectParticle[] pars = objRoot.GetComponentsInChildren<GeUIEffectParticle>();
                        for (int j = 0; j < pars.Length; ++j)
                        {
                            pars[j].StopEmit();
                        }
                    }

                    {
                        ParticleSystem[] pars = objRoot.GetComponentsInChildren<ParticleSystem>();
                        for (int j = 0; j < pars.Length; ++j)
                        {
                            pars[j].Stop();
                        }
                    }

                    {
                        DOTweenAnimation[] anims = objRoot.GetComponentsInChildren<DOTweenAnimation>();
                        for (int j = 0; j < anims.Length; ++j)
                        {
                            if (anims[j].id == m_arrEffectRuntimes[i].strTag)
                            {
                                anims[j].DORewind();
                                anims[j].isActive = false;
                            }
                        }
                    }

                    objRoot.SetActive(info.bOldActive);
                }
            }

            m_arrEffectRuntimes.Clear();

            m_arrEventRuntimes.Clear();
        }

        public float Duration()
        {
            return duration;
        }

        public void Clear()
        {
            Stop();
            m_eventProcessors.Clear();
        }
    }
}
