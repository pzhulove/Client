using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public abstract class CommonTabToggleView : MonoBehaviour
    {
        //private IList<ClientEventNode> mEventNodes = new List<ClientEventNode>();
        public void Select(ClientFrame clientFrame = null)
        {
            if (!init)
            {
                Init(clientFrame);
            }
            OnVisible();
        }


        public void Unselect()
        {
            OnInvisible();
        }

        private bool init = false;
        private void Init(ClientFrame clientFrame = null)
        {
            if (init)
            {
                return;
            }
            OnAddUIEvent();
            OnInit(clientFrame);
            init = true;
        }

        /// <summary>
        /// 协程封装
        /// </summary>
        /// <param name="action"></param>
        /// <param name="coroutineType"></param>
        /// <param name="seconds"></param>
        /// <param name="corTag"></param>
        public Coroutine StartCor(IEnumerator enumerator)
        {
            if (this == null)
            {
                return null;
            }

            if (false == this.enabled)
            {
                return null;
            }

            if (gameObject == null)
            {
                return null;
            }

            if (false == gameObject.activeInHierarchy)
            {
                return null;
            }

            if (enumerator == null)
            {
                return null;
            }

            return StartCoroutine(enumerator);
        }

        protected virtual void OnAddUIEvent()
        {
            //mEventNodes.Bind(eventID, handler);
        }

        protected virtual void OnRemoveUIEvent()
        {

        }

        protected virtual void OnDestroy()
        {
            OnRemoveUIEvent();
            //mEventNodes.UnBindAll();
        }
        protected abstract void OnInit(ClientFrame clientFrame = null);

        protected abstract void OnVisible();

        protected abstract void OnInvisible();

    }
}