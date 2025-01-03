using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    [RequireComponent(typeof(Image))]
    public class ImageFillAnimation : MonoBehaviour
    {
        [SerializeField]
        float interval = 0.03f;

        [SerializeField]
        float step = 0.2f;

        Image img = null;
        float curValue = 0.0f;
        bool isUpdate = false;
        Coroutine coroutine = null; 

        private void Awake()
        {
            img = GetComponent<Image>();
            curValue = 1.0f;      
            isUpdate = false;
            coroutine = null;

            coroutine = GameFrameWork.instance.StartCoroutine(UpdateAnimation());           
        }

        // Start is called before the first frame update
        void Start()
        {
          
        }

        private void OnDestroy()
        {
            img = null;
            curValue = 1.0f;
            isUpdate = false;

            if (coroutine != null)
            {
                GameFrameWork.instance.StopCoroutine(coroutine);
                coroutine = null;
            }                 
        }

        // Update is called once per frame
        void Update()
        {
            if (isUpdate)
            {
                curValue += step;
                if (curValue > 1.0f)
                {
                    curValue = 0.0f;
                }
            }

            UpdateImageByFillValue();
        }

        public void StartAnimation()
        {
            if(img == null)
            {
                return;
            }

            isUpdate = true;
            curValue = 0.0f;
        }

        IEnumerator UpdateAnimation()
        {
//             while(true)
//             {
//                 if (isUpdate)
//                 {
//                     yield return Yielders.GetWaitForSeconds(interval);
// 
//                     curValue += step;
//                     if(curValue > 1.0f)
//                     {
//                         curValue = 0.0f;                     
//                     }
//                 }
// 
//                 yield return null;
//             }

            yield return null;
        }

        public void PauseAnimation()
        {
            if (img == null)
            {
                return;
            }

            isUpdate = false;
        }

        public void ResumeAnimaiton()
        {
            if (img == null)
            {
                return;
            }

            isUpdate = true;
        }

        public void StopAnimation()
        {
            if (img == null)
            {
                return;
            }

            isUpdate = false;
            curValue = 1.0f;
        }

        void UpdateImageByFillValue()
        {
            if (img == null)
            {
                return;
            }

            img.fillAmount = curValue;
        }
    }
}


