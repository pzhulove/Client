using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComCarouselCell : MonoBehaviour
    {
        [HideInInspector]
        public System.Object BindScript;
        
        [HideInInspector]
        public int Index;

        [HideInInspector]
        public RectTransform Rect;

        public void Init(int index, RectTransform rect, string name)
        {
            this.Index = index;
            this.Rect = rect;
            gameObject.name = string.Format("{0}_{1}",name,index.ToString());
        }

        private void OnDestroy()
        {
            BindScript = null;
            Index = 0;
            Rect = null;
        }


    }
}
