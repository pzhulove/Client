using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    [RequireComponent(typeof(Toggle))]
    public class ComToggle : MonoBehaviour
    {
        public GameObject objSelect;
        public GameObject objUnselect;
        public int userData = -1;

        Toggle m_toggle;
        public Toggle toggle { get { return m_toggle; } }

        public void Initialize()
        {
            _InitToggle();
        }

        void Awake()
        {
            _InitToggle();

            if (toggle != null)
            {
                toggle.onValueChanged.AddListener(_UpdateCheckMask);                
            }
        }

        void Start()
        {
            if (toggle != null)
            {
                _UpdateCheckMask(toggle.isOn);
            }                
        }

        private void OnDestroy()
        {
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveListener(_UpdateCheckMask);
            }
        }

        void _InitToggle()
        {
            if (m_toggle == null)
            {
                m_toggle = GetComponent<Toggle>();
            }
        }

        void _UpdateCheckMask(bool a_check)
        {
            if (objSelect != null)
            {
                objSelect.SetActive(a_check);
            }
            if (objUnselect != null)
            {
                objUnselect.SetActive(!a_check);
            }
        }
    }
}
