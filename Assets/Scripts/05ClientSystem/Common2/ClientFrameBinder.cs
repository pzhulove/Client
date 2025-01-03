using UnityEngine;
using System.Collections;
using System;

namespace GameClient
{
    public class ClientFrameBinder : MonoBehaviour
    {
        public IClientFrame clientFrame;
        public GameObject frame;
        public Type GetFrameType()
        {
            if(null != clientFrame)
            {
                return clientFrame.GetType();
            }
            return null;
        }

        public void CloseFrame(bool bImmediately = false)
        {
            if(clientFrame != null)
            {
                clientFrame.Close(bImmediately);
            }
        }

        public void OnCloseFrame()
        {
            clientFrame = null;
            frame = null;
        }

        void OnDestroy()
        {
            clientFrame = null;
            frame = null;
        }
    }
}