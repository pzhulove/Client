using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

    public class ComGrayCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        UIGray comGray;
        bool isGrayEnabled;
        void OnEnable()
        {
            comGray = this.gameObject.GetComponent<UIGray>();
            SetGrayEnable(true);
        }

        void OnDisable()
        {
            comGray = null;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            SetGrayEnable(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SetGrayEnable(true);
        }

        private void SetGrayEnable(bool isEnable)
        {
            if (isGrayEnabled == isEnable)
                return;
            isGrayEnabled = isEnable;
            if (comGray)
            {
                comGray.enabled = isGrayEnabled;
            }
        }
    }

