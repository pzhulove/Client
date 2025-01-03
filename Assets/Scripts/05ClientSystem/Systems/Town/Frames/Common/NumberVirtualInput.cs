using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace GameClient
{
    [RequireComponent(typeof(Text))]
    public class NumberVirtualInput : MonoBehaviour,IPointerClickHandler,IPointerDownHandler,IPointerUpHandler
    {   
        public enum InputMode
        {
            NUMBER, // 数字
            STRING, // 字符串
        }

        Text txtNumber = null;
        bool bFocus = false;

        [SerializeField]
        InputMode inputMode = InputMode.STRING;

        [SerializeField]
        Text txtPlaceHolder = null;

        [SerializeField]
        uint minValue = uint.MinValue;

        [SerializeField]
        uint maxValue = uint.MaxValue;  

        [SerializeField]
        uint maxCount = 10;

        [SerializeField]
        Vector3 offsetPos = new Vector3(300, 60, 0);

        [SerializeField]
        bool clearByFirstInput = false; // 第一次输入（点击删除按钮或者点击数字按钮）时清除初始值

        ulong iInputCount = 0;

        // Use this for initialization
        void Start()
        {
            iInputCount = 0;

            bFocus = false;
            txtNumber = gameObject.GetComponent<Text>();           

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChangeNum2, OnChangeNum);
        }

        // Update is called once per frame
        void Update()
        {
            if(bFocus && !ClientSystemManager.GetInstance().IsFrameOpen<VirtualKeyboardFrame2>())
            {
                bFocus = false;
            }

            if (txtPlaceHolder != null && txtNumber != null)
            {
                txtPlaceHolder.CustomActive(string.IsNullOrEmpty(txtNumber.text));
            }
        }

        void OnDestroy()
        {
            iInputCount = 0;

            txtNumber = null;
            bFocus = false;
            txtPlaceHolder = null;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChangeNum2, OnChangeNum);
        }

        void OnChangeNum(UIEvent uiEvent)
        {
            if(txtNumber == null)
            {
                return;
            }

            if(txtPlaceHolder == null)
            {
                return;
            }

            if(!bFocus)
            {
                return;
            }

            ChangeNumType changeNumType = (ChangeNumType)uiEvent.Param1;
            ulong CurNum = 0;
            ulong.TryParse(txtNumber.text, out CurNum);

            if (changeNumType == ChangeNumType.BackSpace || changeNumType == ChangeNumType.Add)
            {
                iInputCount++;

                if (iInputCount == 1 && clearByFirstInput)
                {
                    txtNumber.SafeSetText("");
                }
            }
            else if(changeNumType == ChangeNumType.EnSure)
            {
                iInputCount = 0;
            }

            if (changeNumType == ChangeNumType.BackSpace)
            {
                if(inputMode == InputMode.STRING)
                {
                    if(txtNumber.text.Length > 0)
                    {
                    txtNumber.text = txtNumber.text.Remove(txtNumber.text.Length - 1, 1);
                    }
                }
                else if(inputMode == InputMode.NUMBER)
                {
                    if(txtNumber.text.Length > 0)
                    {
                    txtNumber.text = txtNumber.text.Remove(txtNumber.text.Length - 1, 1);
                    }
                    uint uValue = 0;
                    uint.TryParse(txtNumber.text, out uValue);
                    if(uValue <= minValue)
                    {
                        txtNumber.text = minValue.ToString();
                    }
                }
            }
            else if (changeNumType == ChangeNumType.Add)
            {
                int addNum = (int)uiEvent.Param2;
                if (addNum < 0 || addNum > 9)
                {
                    Logger.LogErrorFormat("传入数字不合法，请控制在0-9之间");
                    return;
                }

                if(inputMode == InputMode.STRING)
                {
                    if(txtNumber.text.Length < maxCount)
                    {
                        txtNumber.text += addNum.ToString();
                    }                    
                }
                else if(inputMode == InputMode.NUMBER)
                {
                    txtNumber.text += addNum.ToString();
                    uint uValue = 0;
                    uint.TryParse(txtNumber.text, out uValue);
                    if(uValue >= maxValue)
                    {
                        txtNumber.text = maxValue.ToString();
                    }
                    else
                    {
                        txtNumber.text = uValue.ToString();
                    }
                }                
            }
            else if(changeNumType == ChangeNumType.EnSure)
            {
                bFocus = false;
            }

            if (txtPlaceHolder != null)
            {
                txtPlaceHolder.CustomActive(string.IsNullOrEmpty(txtNumber.text));
            }

            if(changeNumType == ChangeNumType.Add || changeNumType == ChangeNumType.BackSpace)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VirtualInputNumberChange);
            }
            else if(changeNumType == ChangeNumType.EnSure)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.VirtualInputEnsure);
            }
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(txtNumber != null && !bFocus)
            {
                bFocus = true;

                if(txtPlaceHolder != null)
                {
                    txtPlaceHolder.CustomActive(false);
                }

                ClientSystemManager.GetInstance().OpenFrame<VirtualKeyboardFrame2>(FrameLayer.Middle,offsetPos);
            }            
        }

        public void OnPointerDown(PointerEventData eventData)
        {

        }

        public void OnPointerUp(PointerEventData eventData)
        {

        }

        public void SetInputMode(InputMode mode)
        {
            inputMode = mode;
        }

        public void SetNumberLimit(uint iMin,uint iMax)
        {
            minValue = iMin;
            maxValue = iMax;
        }

        public void SetStringLenLimit(uint iMax)
        {
            maxCount = iMax;
        }
    }
}


