using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GameClient
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleCD : MonoBehaviour
    {
        [SerializeField]
        private float cd = 1.0f; 

        Toggle toggle = null;
        bool interactable = true;

        private void Awake()
        {
            interactable = true;          
            toggle = this.gameObject.GetComponent<Toggle>();
        }
   
        public void SetCallBack(UnityEngine.Events.UnityAction<bool> callBack)
        {
            if(toggle == null)
            {
                return;
            }

            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener((val) => 
            {         
                if(val)
                {
                    if(!interactable)
                    {
                        return;
                    }

                    if (callBack != null)
                    {
                        callBack.Invoke(val);
                    }

                    interactable = false;
                    InvokeMethod.Invoke(this, cd, () =>
                    {
                        interactable = true;
                    });
                }
                else
                {
                    interactable = true;
                    if (callBack != null)
                    {
                        callBack.Invoke(val);
                    }
                }
            });
        }

        // Use this for initialization
        void Start()
        {
            
        }

        private void OnDestroy()
        {
            interactable = true;
            InvokeMethod.RemoveInvokeCall(this);
        }       
    }
}


