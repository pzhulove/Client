using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace GameClient
{
    public class ActiveSpecialRedBinder : MonoBehaviour
    {
        public Toggle toggle;
        public string prefabKey;
        public int iMainId;
        public void SendCheckRedPoint()
        {
            if(toggle != null && toggle.isOn)
            {
                if(!string.IsNullOrEmpty(prefabKey) && iMainId > 0)
                {
                    UIEventSystem.GetInstance().SendUIEvent(new UIEventSpecialRedPointNotify(iMainId, prefabKey));
                }
            }
        }
    }
}