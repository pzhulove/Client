using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    [AddComponentMenu("UI/Effects/ButtonEnable")]
    public class ComButtonEnbale : MonoBehaviour
    {
        Color m_disableColor = new Color(0.6f, 0.6f, 0.6f, 1.0f);
        public bool bEnable = true;

        void OnValidate()
        {
            _UpdateEnable();
        }

        public void SetEnable(bool a_bEnable)
        {
            if (bEnable == a_bEnable)
            {
                return;
            }
            bEnable = a_bEnable;
            _UpdateEnable();
        }

        protected void _UpdateEnable()
        {
            Selectable[] selectables = gameObject.GetComponentsInChildren<Selectable>(true);
            if (selectables != null)
            {
                for (int i = 0; i < selectables.Length; ++i)
                {
                    selectables[i].interactable = bEnable;
                    ColorBlock colors = selectables[i].colors;
                    colors.disabledColor = Color.white;
                    selectables[i].colors = colors;
                }
            }

            Graphic[] graphics = gameObject.GetComponentsInChildren<Graphic>(true);
            for (int i = 0; i < graphics.Length; ++i)
            {
                ComModifyColor comModifyColor = graphics[i].GetComponent<ComModifyColor>();
                if (comModifyColor == null)
                {
                    comModifyColor = graphics[i].gameObject.AddComponent<ComModifyColor>();
                }
                comModifyColor.colAddColor = bEnable ? Color.white : m_disableColor;
            }
        }
    }
}
