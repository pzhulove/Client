using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class FashionMergeNewPropertyView : MonoBehaviour
    {
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseFrame);
            }
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
        }

        private void UnBindUiEventSystem()
        {
            if(closeButton != null)
                closeButton.SafeRemoveAllListener();
        }

        public void InitData()
        {

        }

        private void OnCloseFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<FashionMergeNewPropertyFrame>();
        }

    }

}