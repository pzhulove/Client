using UnityEngine;
using System.Collections;
namespace GameClient
{
    public class ComFagureController : MonoBehaviour
    {
        public GameObject goTarget = null;
        public int iLow = 0;
        public int iHigh = 60;

        void  Awake()
        {
            _OnFagureChanged(null);
        }

        // Use this for initialization
        void Start()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FatigueChanged, _OnFagureChanged);
        }

        void _OnFagureChanged(UIEvent uiEvent)
        {
            if(null != goTarget)
            {
                goTarget.CustomActive(PlayerBaseData.GetInstance().fatigue >= iLow && PlayerBaseData.GetInstance().fatigue <= iHigh);
            }
        }

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FatigueChanged, _OnFagureChanged);
        }
    }
}