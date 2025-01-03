using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ProtoTable;
using System.Collections.Generic;

namespace GameClient
{
    class ComBattery : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> fills = new List<GameObject>();

        private void Awake()
        {
           
        }

        private void Start()
        {
            
        }   

        private void OnDestroy()
        {
          
        }

        public void SetUp(float rate)
        {
            if (fills == null || fills.Count == 0)
            {
                return;
            }

            float value = 1.0f / fills.Count;
            int num = Mathf.CeilToInt(rate / value);

            for (int i = 0; i < fills.Count; i++)
            {
                fills[i].CustomActive(false);
            }

            for (int i = 0;i < num && i < fills.Count;i++)
            {
                fills[i].CustomActive(true);
            }
        }
    }
}