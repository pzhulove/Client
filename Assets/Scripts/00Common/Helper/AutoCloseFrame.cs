using UnityEngine;
using System.Collections;

namespace GameClient
{
    public class AutoCloseFrame : MonoBehaviour
    {
        public float CloseTime = 2.0f;

        // Use this for initialization
        void Start()
        {
            Invoke("CloseSelf", CloseTime);
        }

        private void OnDestroy()
        {
            CancelInvoke("CloseSelf");
        }

        void CloseSelf()
        {
            ComClientFrame clientFrame = this.gameObject.GetComponent<ComClientFrame>();
            if (clientFrame != null)
            {
                System.Type framType = System.Type.GetType(clientFrame.mCurrentFrameName);
                if (null != framType)
                {
                    ClientSystemManager.GetInstance().CloseFrame(framType);
                }
            }
        }
    }
}


