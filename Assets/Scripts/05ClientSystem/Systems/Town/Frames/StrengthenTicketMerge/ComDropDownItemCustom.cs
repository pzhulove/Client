using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComDropDownItemCustom : MonoBehaviour
    {        
        #region Model Params

        public delegate void OnToggleClick(bool isOn, int index);
        public OnToggleClick onToggleClick;

        private int mCurrSelectIndex = 0;
        private bool bInit = false;
        
        #endregion
        
        #region View Params

        [SerializeField]
        private Text m_Desc = null;
        [SerializeField]
        private Image m_Icon = null;
        [SerializeField]
        private Toggle m_Select = null;

        #endregion
        
        #region PRIVATE METHODS

        //Unity life cycle
        void Start () 
        {
            if (m_Select)
            {
                m_Select.onValueChanged.AddListener(_OnSelectToggleChanged);
            }
        }
        
        //Unity life cycle
        void OnDestroy () 
        {
            if (m_Select)
            {
                m_Select.onValueChanged.RemoveListener(_OnSelectToggleChanged);
                m_Select = null;
            }

            Clear();
        }

        void _OnSelectToggleChanged(bool isOn)
        {
            if (onToggleClick != null)
            {
                onToggleClick(isOn, mCurrSelectIndex);
            }            
        }
        
        #endregion
        
        #region  PUBLIC METHODS

        public void Init(int index, string desc, string bgPath = null)
        {
            if (bInit)
            {
                return;
            }
            mCurrSelectIndex = index;
            if (m_Desc)
            {
                m_Desc.text = desc;
            }
            if (m_Icon && !string.IsNullOrEmpty(bgPath))
            {
                ETCImageLoader.LoadSprite(ref m_Icon, bgPath);
            }
            bInit = true;
        }

        public void Clear()
        {
            onToggleClick = null;
            bInit = false;
        }

        public void SetToggleOn()
        {
            if (m_Select)
            {
                m_Select.isOn = true;
            }
        }
        public void SetToggleOff()
        {
            if (m_Select)
            {
                m_Select.isOn = false;
            }
        }

        public string GetToggleDesc()
        {
            if (m_Desc)
            {
                return m_Desc.text;
            }
            return "";
        }

        public Toggle GetToggle()
        {
            return m_Select;
        }
        
        #endregion
    }
}