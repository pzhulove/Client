using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameClient;
using UnityEngine.Events;

class MyDropDown : MonoBehaviour
{
    public Button m_button;
    public GameObject m_template;
    public Button[] onActions;
    public Text[] descs;
    void Start()
    {
        if(null != m_button)
        {
            m_button.onClick.RemoveListener(_OnVisibleChanged);
            m_button.onClick.AddListener(_OnVisibleChanged);
        }
    }

    void _OnVisibleChanged()
    {
        m_template.CustomActive(!m_template.activeSelf);
    }

    public void CloseTemplate()
    {
        m_template.CustomActive(false);
    }

    public void BindItems(string[] content,ChatType[] eChatTypes,UnityAction<ChatType> actions)
    {
        for(int i = 0; i < onActions.Length; ++i)
        {
            Button btn = onActions[i];
            btn.CustomActive(false);
        }
        int iCount = 0;
        if(content != null && eChatTypes != null)
        {
            iCount = System.Math.Min(content.Length, eChatTypes.Length);
            for(int i = 0;i < iCount;i++)
            {
                Button btn = onActions[i];
                btn.CustomActive(true);
            }
        }
        for(int i = 0; i < iCount; ++i)
        {
            if(null != onActions[i])
            {
                onActions[i].onClick.RemoveAllListeners();
            }

            if(i < content.Length && i < eChatTypes.Length && i < descs.Length)
            {
                ChatType eValue = eChatTypes[i];
                onActions[i].onClick.AddListener(() =>
                {
                    if(null != actions)
                    {
                        actions.Invoke(eValue);
                        CloseTemplate();
                    };
                });

                if(null != descs && null != descs[i])
                {
                    descs[i].text = content[i];
                }
            }
        }
    }

    public void OnDestroy()
    {
        if (null != m_button)
        {
            m_button.onClick.RemoveListener(_OnVisibleChanged);
        }

        if(null != onActions)
        {
            for (int i = 0; i < onActions.Length; ++i)
            {
                if (null != onActions[i])
                {
                    onActions[i].onClick.RemoveAllListeners();
                }
            }
        }
    }
}