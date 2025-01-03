using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    [ExecuteAlways]
    class ComSetTextColor : MonoBehaviour
    {
        public Color[] colors = new Color[0];
        public Text text; 
        public void SetColor(int iIndex)
        {
            if(iIndex >= 0 && iIndex < colors.Length)
            {
                var color = colors[iIndex];
                if(null != text)
                {
                    text.color = color;
                }
            }
        }
    }
}