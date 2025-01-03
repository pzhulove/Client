using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class TextJump : MonoBehaviour
    {
        public string[] jumpTexts = null;
        int iIndex = 0;
        public Text text;
        public int interval = 6;
        int iTick = 0;
        // Use this for initialization
        void Start()
        {
            iIndex = -1;
            iTick = 0;
            if(interval < 1)
            {
                interval = 1;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(iTick % interval != 0)
            {
                iTick = (1 + iTick) % interval;
                return;
            }

            if(null != jumpTexts && null != text && jumpTexts.Length > 0)
            {
                iIndex = (1 + iIndex) % jumpTexts.Length;
                text.text = jumpTexts[iIndex];
            }

            iTick = (1 + iTick) % interval;
        }
    }
}