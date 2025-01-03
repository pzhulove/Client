using UnityEngine;
using System.Collections.Generic;

public class EffectAudioProxy : MonoBehaviour
{
    public enum AudioTigger
    {
        None,
        OnCall,
        OnPlay,
    }

    protected static readonly int INVALID_CALL_ID = -1;
    protected static readonly string INVALID_TAG = "";

    [System.Serializable]
    public class EffAudioDesc
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

    [SerializeField]
    protected EffAudioDesc[] m_EffAudioDescList = new EffAudioDesc[0];

    List<uint> m_AudioHandleList = new List<uint>();

    public EffAudioDesc[] EffAudioDescList
    {
        get
        {
            return m_EffAudioDescList;
        }

        set
        {
            m_EffAudioDescList = value;
        }
    }

    protected class EffAudioTrigger
    {
        public EffAudioDesc m_EffAudioDesc = null;
        public int m_TimeStampMS = 0;
    }
    protected List<EffAudioTrigger> m_UIAudioTriggerList = new List<EffAudioTrigger>();
    protected List<EffAudioTrigger> m_UIIdleTriggerList = new List<EffAudioTrigger>();

    protected int m_CurTimeMS = 0;

    // Use this for initialization
    void Start()
    {
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
    }

    void OnDestroy()
    {
        for (int i = 0, icnt = m_AudioHandleList.Count; i < icnt; ++i)
            AudioManager.instance.Stop(m_AudioHandleList[i]);

        m_AudioHandleList.Clear();
    }

    public bool TriggerAudio(AudioTigger tiggerType, int trggierID = -1)
    {
        if (AudioTigger.OnCall == tiggerType)
        {
            if (INVALID_CALL_ID == trggierID)
                return false;

            _CheckTrigger(AudioTigger.OnCall, trggierID);
        }
        else
            _CheckTrigger(tiggerType);

        return true;
    }

    void Update()
    {
        m_CurTimeMS += (int)(1000.0f * Time.deltaTime);

        for (int i = 0, icnt = m_UIAudioTriggerList.Count; i < icnt; ++i)
        {
            EffAudioTrigger curTrigger = m_UIAudioTriggerList[i];

            if (null == curTrigger || null == curTrigger.m_EffAudioDesc)
            {
                m_UIAudioTriggerList.RemoveAt(i);
                break;
            }

            if (m_CurTimeMS - curTrigger.m_TimeStampMS > curTrigger.m_EffAudioDesc.m_TimeDelayMS)
            {
                m_UIAudioTriggerList.RemoveAt(i);
                m_AudioHandleList.Add(AudioManager.instance.PlaySound(curTrigger.m_EffAudioDesc.m_AudioRes, curTrigger.m_EffAudioDesc.m_AudioType, curTrigger.m_EffAudioDesc.m_Volume, curTrigger.m_EffAudioDesc.m_Loop));

                curTrigger.m_TimeStampMS = 0;
                curTrigger.m_EffAudioDesc = null;

                m_UIIdleTriggerList.Add(curTrigger);
                break;
            }
        }
    }

    void _CheckTrigger(AudioTigger triggerType, int triggerID = -1)
    {
        for (int i = 0, icnt = m_EffAudioDescList.Length; i < icnt; ++i)
        {
            EffAudioDesc curAudioDesc = m_EffAudioDescList[i];

            if (null == curAudioDesc) continue;

            if (curAudioDesc.m_Trigger == triggerType && curAudioDesc.m_CallID == triggerID)
            {
                if (curAudioDesc.m_TimeDelayMS == 0)
                    m_AudioHandleList.Add(AudioManager.instance.PlaySound(curAudioDesc.m_AudioRes, curAudioDesc.m_AudioType, curAudioDesc.m_Volume, curAudioDesc.m_Loop));
                else
                {
                    EffAudioTrigger curTrigger = _AllocTrigger();

                    curTrigger.m_EffAudioDesc = curAudioDesc;
                    curTrigger.m_TimeStampMS = m_CurTimeMS;

                    m_UIAudioTriggerList.Add(curTrigger);
                }
            }
        }
    }

    EffAudioTrigger _AllocTrigger()
    {
        EffAudioTrigger curTrigger = null;
        if (m_UIIdleTriggerList.Count > 0)
        {
            curTrigger = m_UIIdleTriggerList[0];
            m_UIIdleTriggerList.RemoveAt(0);
            return curTrigger;
        }

        EffAudioTrigger newTrigger = new EffAudioTrigger();
        return newTrigger;
    }
}
