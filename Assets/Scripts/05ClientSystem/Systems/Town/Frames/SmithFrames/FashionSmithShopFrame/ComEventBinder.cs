using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    class ComEventBinder : MonoBehaviour
    {
        public EUIEventID[] mEvents = new EUIEventID[0];
        public void SendEvent(int iIndex)
        {
            if(iIndex >= 0 && iIndex < mEvents.Length)
            {
                UIEventSystem.GetInstance().SendUIEvent(mEvents[iIndex]);
            }
        }
    }
}