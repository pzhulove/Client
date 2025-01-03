using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class RandomPropertiesItem : MonoBehaviour
    {
       
        [SerializeField]
        private Text mArrtDes;
        
        public void OnItemVisible(int iBuffId)
        {
            mArrtDes.text = BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(iBuffId);
        }
    }
}
