using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    class StrengthLvControl : MonoBehaviour
    {
        public List<GameObject> listVisible = null;
        public void SetVisible(bool bVisible)
        {
            if(listVisible != null)
            {
                for (int i = 0; i < listVisible.Count; ++i)
                {
                    listVisible[i].CustomActive(bVisible);
                }
            }
        }
    }
}