using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    [RequireComponent(typeof(Toggle))]
    public class ComToggleEx : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] objSelect = new GameObject[1];

        [SerializeField]
        private GameObject[] objUnselect = new GameObject[1];

        Toggle m_toggle = null;

        void Awake()
        {
            _InitToggle();

            if (m_toggle != null)
            {
                m_toggle.onValueChanged.AddListener(_UpdateCheckMask);
            }
        }

        void Start()
        {
            if (m_toggle != null)
            {
                _UpdateCheckMask(m_toggle.isOn);
            }                
        }

        private void OnDestroy()
        {
            if (m_toggle != null)
            {
                m_toggle.onValueChanged.RemoveListener(_UpdateCheckMask);
            }

            m_toggle = null;
            objSelect = null;
            objUnselect = null;
        }

        void _InitToggle()
        {
            if (m_toggle == null)
            {
                m_toggle = GetComponent<Toggle>();
            }
        }

        public void _UpdateCheckMask(bool a_check)
        {
            if (objSelect != null)
            {
                for (int i = 0; i < objSelect.Length; i++)
                {
                    objSelect[i].CustomActive(a_check);
                }

            }

            if (objUnselect != null)
            {
                for (int i = 0; i < objUnselect.Length; i++)
                {
                    objUnselect[i].CustomActive(!a_check);
                }
            }
        }
    }
}
