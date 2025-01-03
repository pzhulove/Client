using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GameClient
{
    [RequireComponent(typeof(Text))]
    class MatchCounterDown : MonoBehaviour
    {
        Text text;
        int iTime = 0;
        static int iLimitTime = 30;
        // Use this for initialization
        void Start()
        {
            text = GetComponent<Text>();
        }

        void OnEnable()
        {
            iTime = iLimitTime;
            CancelInvoke("_OnUpdate");
            InvokeRepeating("_OnUpdate", 0.0f, 1.0f);
        }

        void _OnUpdate()
        {
            if(text == null)
            {
                text = GetComponent<Text>();
            }
            if(text != null)
            {
                text.text = string.Format("匹配中...({0:D2}s)", iTime--);
            }
            if(iTime < 0)
            {
                iTime = iLimitTime;
            }
        }
    }
}