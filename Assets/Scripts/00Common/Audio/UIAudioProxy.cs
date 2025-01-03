using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIAudioProxy : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public enum AudioTigger
    {
        None,
        OnCall,
        OnOpen,
        OnPointerDown,
        OnClose,
        OnPointerUp,
        Count,
    }

    protected static readonly int INVALID_CALL_ID = -1;
    protected static readonly string INVALID_TAG = "";

    [System.Serializable]
    public class UIAudioDesc
    {
        public string m_AudioRes = null;
        public string m_AudioTag = INVALID_TAG;
        public AudioType m_AudioType = AudioType.AudioEffect;
        public bool m_Loop = false;
        public AudioTigger m_Trigger = AudioTigger.None;
        public int m_TimeDelayMS = 0;
        public float m_Volume = 1.0f;
        public int m_CallID = INVALID_CALL_ID;
    }

    [SerializeField][HideInInspector]
    protected UIAudioDesc[] m_UIAudioDescList = new UIAudioDesc[0];

    List<uint> m_AudioHandleList = new List<uint>();

    public UIAudioDesc[] UIAudioDescList
    {
        get
        {
            return m_UIAudioDescList;
        }

        set
        {
            m_UIAudioDescList = value;
        }
    }

    protected class UIAudioTrigger
    {
        public UIAudioDesc m_UIAudioDesc = null;
        public int m_TimeStampMS = 0;
    }
    protected List<UIAudioTrigger> m_UIAudioTriggerList = new List<UIAudioTrigger>();
    protected List<UIAudioTrigger> m_UIIdleTriggerList = new List<UIAudioTrigger>();
    List<uint>[] m_type2Handles = new List<uint>[(int)AudioTigger.Count];

    public UnityEvent eventEnable = null;
    public UnityEvent eventDisable = null;
    private Dictionary<int, uint> mOnCallList = null;

    protected int m_CurTimeMS = 0;
    protected UIGray m_Gray = null;

    //protected UnityE

    // Use this for initialization
    void Start ()
    {
        m_Gray = gameObject.transform.GetComponent<UIGray>();
    }
	
    void OnEnable()
    {
        _CheckTrigger(AudioTigger.OnOpen);
        if(null != eventEnable)
        {
            eventEnable.Invoke();
        }
    }

    void OnDisable()
    {
        _CheckTrigger(AudioTigger.OnClose);
        if (null != eventDisable)
        {
            eventDisable.Invoke();
        }
    }

    public void StopAudio(AudioTigger triggerType)
    {
        int iIndex = (int)triggerType;
        if (iIndex >= 0 && iIndex < m_type2Handles.Length)
        {
            if(null != m_type2Handles[iIndex])
            {
                for(int i = 0; i < m_type2Handles[iIndex].Count; ++i)
                {
                    AudioManager.instance.Stop(m_type2Handles[iIndex][i]);
                }
                m_type2Handles[iIndex].Clear();
            }
        }
    }

    void OnDestroy()
    {
        for (int i = 0, icnt = m_AudioHandleList.Count; i < icnt; ++i)
            AudioManager.instance.Stop(m_AudioHandleList[i]);

        for(int i = 0; i < m_type2Handles.Length; ++i)
        {
            if(null != m_type2Handles[i])
            {
                m_type2Handles[i].Clear();
                m_type2Handles[i] = null;
            }
        }

        m_AudioHandleList.Clear();
        if(null != mOnCallList)
        {
            mOnCallList.Clear();
        }

        eventEnable = null;
        eventDisable = null;
    }

    public void TriggerAudio(int trggierID)
    {
        if (INVALID_CALL_ID == trggierID)
            return;

        _CheckTrigger(AudioTigger.OnCall, trggierID);
    }

    public void StopTriggerAudio(int trggierID)
    {
        int iIndex = (int)AudioTigger.OnCall;
        if(null != mOnCallList && trggierID != -1)
        {
            if(mOnCallList.ContainsKey(trggierID))
            {
                uint handle = mOnCallList[trggierID];
                AudioManager.instance.Stop(handle);
                mOnCallList.Remove(trggierID);

                if (iIndex >= 0 && iIndex < m_type2Handles.Length)
                {
                    if (null != m_type2Handles[iIndex])
                    {
                        m_type2Handles[iIndex].Remove(handle);
                    }
                }
            }
        }
    }

    void Update()
    {
        m_CurTimeMS += (int)(1000.0f * Time.deltaTime);

        for(int i = 0,icnt = m_UIAudioTriggerList.Count;i<icnt;++i)
        {
            UIAudioTrigger curTrigger = m_UIAudioTriggerList[i];

            if(null == curTrigger || null == curTrigger.m_UIAudioDesc)
            {
                m_UIAudioTriggerList.RemoveAt(i);
                break;
            }

            if(m_CurTimeMS - curTrigger.m_TimeStampMS > curTrigger.m_UIAudioDesc.m_TimeDelayMS)
            {
                m_UIAudioTriggerList.RemoveAt(i);

                var list = m_type2Handles[(int)curTrigger.m_UIAudioDesc.m_Trigger];
                if (null == list)
                {
                    list = new List<uint>();
                    m_type2Handles[(int)curTrigger.m_UIAudioDesc.m_Trigger] = list;
                }
                uint iAudioHandle = AudioManager.instance.PlaySound(curTrigger.m_UIAudioDesc.m_AudioRes, curTrigger.m_UIAudioDesc.m_AudioType, curTrigger.m_UIAudioDesc.m_Volume, curTrigger.m_UIAudioDesc.m_Loop);
                list.Add(iAudioHandle);
                m_AudioHandleList.Add(iAudioHandle);

                curTrigger.m_TimeStampMS = 0;
                curTrigger.m_UIAudioDesc = null;

                m_UIIdleTriggerList.Add(curTrigger);
                break;
            }
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (null == m_Gray || !m_Gray.isActiveAndEnabled)
            _CheckTrigger(AudioTigger.OnPointerDown);

        if (transform.parent != null)
        {
            var parentPointerDown = transform.parent.GetComponentInParent<IPointerDownHandler>();
            if (parentPointerDown != null)
                parentPointerDown.OnPointerDown(eventData);
        }
    }
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (null == m_Gray || !m_Gray.isActiveAndEnabled)
            _CheckTrigger(AudioTigger.OnPointerUp);
        if (transform.parent != null)
        {
            var parentPointerDown = transform.parent.GetComponentInParent<IPointerUpHandler>();
            if (parentPointerDown != null)
                parentPointerDown.OnPointerUp(eventData);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (transform.parent != null)
        {
            var parentPointerClick = transform.parent.GetComponentInParent<IPointerClickHandler>();
            if (parentPointerClick != null)
                parentPointerClick.OnPointerClick(eventData);
        }
    }

    void _CheckTrigger(AudioTigger triggerType,int triggerID = -1)
    {
        if(triggerType <= AudioTigger.None || triggerType >= AudioTigger.Count)
        {
            return;
        }
        for (int i = 0,icnt = m_UIAudioDescList.Length; i< icnt;++i)
        {
            UIAudioDesc curAudioDesc = m_UIAudioDescList[i];

            if(null == curAudioDesc) continue;

            if (curAudioDesc.m_Trigger == triggerType && curAudioDesc.m_CallID == triggerID)
            {
                var list = m_type2Handles[(int)triggerType];
                if(null == list)
                {
                    list = new List<uint>();
                    m_type2Handles[(int)triggerType] = list;
                }

                if (curAudioDesc.m_TimeDelayMS == 0)
                {
                    uint iAudioHandle = AudioManager.instance.PlaySound(curAudioDesc.m_AudioRes, curAudioDesc.m_AudioType, curAudioDesc.m_Volume, curAudioDesc.m_Loop);
                    list.Add(iAudioHandle);
                    m_AudioHandleList.Add(iAudioHandle);

                    if(triggerType == AudioTigger.OnCall && -1 != triggerID)
                    {
                        if(null != eventEnable && null != eventDisable)
                        {
                            if(null == mOnCallList)
                            {
                                mOnCallList = new Dictionary<int, uint>();
                            }
                            if(mOnCallList.ContainsKey(triggerID))
                            {
                                AudioManager.instance.Stop(mOnCallList[triggerID]);
                                mOnCallList[triggerID] = iAudioHandle;
                            }
                            else
                            {
                                mOnCallList.Add(triggerID, iAudioHandle);
                            }
                        }
                    }
                }
                else
                {
                    UIAudioTrigger curTrigger = _AllocTrigger();
                    curTrigger.m_UIAudioDesc = curAudioDesc;
                    curTrigger.m_TimeStampMS = m_CurTimeMS;

                    m_UIAudioTriggerList.Add(curTrigger);
                }
            }
        }
    }

    UIAudioTrigger _AllocTrigger()
    {
        UIAudioTrigger curTrigger = null;
        if (m_UIIdleTriggerList.Count > 0)
        {
            curTrigger = m_UIIdleTriggerList[0];
            m_UIIdleTriggerList.RemoveAt(0);
            return curTrigger;
        }

        UIAudioTrigger newTrigger = new UIAudioTrigger();
        return newTrigger;
    }

}
