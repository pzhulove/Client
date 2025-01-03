using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    class ComRecoveryRole : MonoBehaviour
    {
        public Image headIcon;
        public Text LvInfo;
        public Text JobName;
        public Text RoleName;
        public Text LifeTime;
        public Button button;
        public UIGray grayRecovery;
        public delegate void OnTick();
        public OnTick onTick;
        void Start()
        {
            CancelInvoke("_Tick");
            InvokeRepeating("_Tick", 0,1.0f);
        }

        void OnDestroy()
        {
            CancelInvoke("_Tick");
        }

        void _Tick()
        {
            if(onTick != null)
            {
                onTick.Invoke();
            }
        }
    }
}