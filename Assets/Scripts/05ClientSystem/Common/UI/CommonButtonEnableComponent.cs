using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace GameClient
{
    public class CommonButtonEnableComponent : MonoBehaviour
    {

        private Button button;
        private float _intervalTime = 0.0f;

        public float disableTime = 0.5f;        //点击之后，按钮持续不能点击的时间

        private void Start()
        {
            button = gameObject.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClick);
            }
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnButtonClick);
            }
        }

        private void Update()
        {
            if (_intervalTime <= 0)
            {
                ResetButtonEnable();
                _intervalTime = 0;
            }
            else
            {
                _intervalTime -= Time.deltaTime;
            }
        }

        private void OnButtonClick()
        {

            SetButtonDisable();
            _intervalTime = disableTime;
        }


        private void ResetButtonEnable()
        {
            if (button != null)
            {
                if (button.interactable == false)
                {
                    button.interactable = true;
                }
            }
        }

        private void SetButtonDisable()
        {
            if (button != null)
            {
                button.interactable = false;
            }
        }

    }
}