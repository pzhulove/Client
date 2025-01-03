using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class UIAudioEffectProxy : MonoBehaviour, IPointerDownHandler
{
    public AudioClip m_AudioEffOnPress = null;
    public AudioClip m_AudioEffOnEnable = null;
    public AudioClip m_AudioEffOnDisable = null;

    protected UIGray m_Gray = null;

    // Use this for initialization
    void Start ()
    {
        m_Gray = gameObject.transform.GetComponent<UIGray>();
    }
	
    void OnEnable()
    {
        if (null != m_AudioEffOnEnable)
        {
            AudioManager.instance.PlaySound(m_AudioEffOnEnable, AudioType.AudioEffect);
        }
    }

    void OnDisable()
    {
        if (null != m_AudioEffOnDisable)
        {
            AudioManager.instance.PlaySound(m_AudioEffOnDisable, AudioType.AudioEffect);
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (null != m_AudioEffOnPress)
        {
            if (null != m_Gray)
            {
                if (!m_Gray.isActiveAndEnabled)
                    AudioManager.instance.PlaySound(m_AudioEffOnPress, AudioType.AudioEffect);
            }
            else
                AudioManager.instance.PlaySound(m_AudioEffOnPress, AudioType.AudioEffect);
        }
    }
}
