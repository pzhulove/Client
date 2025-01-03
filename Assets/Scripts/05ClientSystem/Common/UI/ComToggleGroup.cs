using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameClient
{
    [RequireComponent(typeof(ToggleGroup))]
    public class ComToggleGroup : MonoBehaviour
    {
        public ToggleGroupEvent onSelectChanged;

        ComToggle[] m_comToggles;
        public ComToggle[] comToggles { get { return m_comToggles; } }

        int m_currToggleTag;
        bool m_bBlockSignal = false;

        public ComToggleGroup()
        {
            onSelectChanged = new ToggleGroupEvent();
        }
        
        public void Initialize()
        {
            _InitToggles();
        }

        public void SetCurrentToggle(int a_tag)
        {
            m_currToggleTag = a_tag;
            // _InitToggles();
            _InitCurrToggle();
        }

        void Awake()
        {
            // _InitToggles();
            _InitCurrToggle();
        }

        void _InitToggles()
        {
            if (m_comToggles == null)
            {
                m_comToggles = GetComponentsInChildren<ComToggle>();
                for (int i = 0; i < m_comToggles.Length; ++i)
                {
                    ComToggle comToggle = m_comToggles[i];
                    comToggle.Initialize();
                    if (comToggle.toggle != null)
                    {
                        comToggle.toggle.onValueChanged.AddListener((bool a_check) =>
                        {
                            if (m_bBlockSignal == false)
                            {
                                onSelectChanged.Invoke(comToggle.userData, a_check);
                            }
                        });
                    }
                }
            }
        }

        void _InitCurrToggle()
        {
            if (m_comToggles != null)
            {
                m_bBlockSignal = true;
                ToggleGroup toggleGroup = GetComponent<ToggleGroup>();
                if (toggleGroup.allowSwitchOff == false)
                {
                    toggleGroup.allowSwitchOff = true;
                    for (int i = 0; i < m_comToggles.Length; ++i)
                    {
                        if (m_comToggles[i].userData == m_currToggleTag)
                        {
                            m_comToggles[i].toggle.isOn = false;
                            //m_comToggles[i].toggle.isOn = true;
                        }
                        else
                        {
                            m_comToggles[i].toggle.isOn = true;
                            m_comToggles[i].toggle.isOn = false;
                        }
                    }
                    toggleGroup.allowSwitchOff = false;
                }
                else
                {
                    for (int i = 0; i < m_comToggles.Length; ++i)
                    {
                        if (m_comToggles[i].userData == m_currToggleTag)
                        {
                            m_comToggles[i].toggle.isOn = false;
                            //m_comToggles[i].toggle.isOn = true;
                        }
                        else
                        {
                            m_comToggles[i].toggle.isOn = true;
                            m_comToggles[i].toggle.isOn = false;
                        }
                    }
                }
                m_bBlockSignal = false;

                for (int i = 0; i < m_comToggles.Length; ++i)
                {
                    if (m_comToggles[i].userData == m_currToggleTag)
                    {
                        m_comToggles[i].toggle.isOn = true;
                        break;
                    }
                }
            }
        }

        public class ToggleGroupEvent : UnityEvent<int, bool> { }
    }
}
