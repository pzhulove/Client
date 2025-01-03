using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComCardEffect : MonoBehaviour
    {
        public bool bFinished { get; set; }

        void Awake()
        {
            bFinished = true;
        }

        public void SetEnable(int a_nFinish)
        {
            bFinished = a_nFinish > 0;
        }
    }
}
