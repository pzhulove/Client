using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    [ExecuteAlways]
    class ComSetImageColor : MonoBehaviour
    {
        public Color[] colors = new Color[0];
        public Image img; 
        public void SetColor(int iIndex)
        {
            if(iIndex >= 0 && iIndex < colors.Length)
            {
                var color = colors[iIndex];
                if(null != img)
                {
                    img.color = color;
                }
            }
        }
    }
}