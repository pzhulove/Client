using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GameClient
{
    [RequireComponent(typeof(Button))]
    public class CloseFrameButton : MonoBehaviour
    {
        public string frameName = "";

        Button btn = null;
        // Use this for initialization
        void Start()
        {
            btn = this.gameObject.GetComponent<Button>();
            btn.SafeAddOnClickListener(OnClickClose);
        }

        void OnClickClose()
        {
            if(string.IsNullOrEmpty(frameName))
            {
                GameObject go = gameObject;
                while(go != null)
                {
                    ComClientFrame comClientFrame = go.GetComponent<ComClientFrame>();
                    if (comClientFrame != null)
                    {
                        ClientSystemManager.GetInstance().CloseFrame(comClientFrame.GetClientFrame());
                        break;
                    }
                    go = go.GetComponent<RectTransform>().parent.gameObject;
                }
                return;
            }
            ClientSystemManager.GetInstance().CloseFrame(frameName);
        }

        private void OnDestroy()
        {
            btn.SafeRemoveOnClickListener(OnClickClose);
        }       
    }
}


