using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GameClient
{
    [RequireComponent(typeof(Button))]
    public class ButtonCD : MonoBehaviour
    {
        [SerializeField]
        private float cd = 1.0f; 

        Button btn = null;
        bool interactable = true;

        private void Awake()
        {
            interactable = true;
            btn = this.gameObject.GetComponent<Button>();      
        }

        public void SetCallBack(UnityEngine.Events.UnityAction callback)
        {
            if(btn == null)
            {
                return;
            }

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => 
            {
                if(!interactable)
                {
                    return;
                }

                if(callback != null)
                {
                    callback.Invoke();
                }

                interactable = false;
                InvokeMethod.Invoke(this,cd, () => 
                {
                    interactable = true;
                });
            });

            return;
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


