using UnityEngine;
using System;

namespace GameClient
{
    public class PackageLockItem : MonoBehaviour
    {
        private Action mOnClick;
        public void Init(Action onClick)
        {
            mOnClick = onClick;
        }

        public void OnClick()
        {
            mOnClick?.Invoke();
        }
    }
}