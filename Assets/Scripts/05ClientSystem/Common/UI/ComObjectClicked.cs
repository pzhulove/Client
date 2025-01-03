using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace GameClient
{
    class ComObjectClicked : MonoBehaviour
    {
        public ClickEvent OnClicked = new ClickEvent();

        public class ClickEvent : UnityEvent { }

        void OnMouseDown()
        {
            OnClicked.Invoke();
        }
    }
}
