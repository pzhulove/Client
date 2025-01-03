using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections;

namespace GameClient
{
    enum ChangeNumType
    {
        Add = 1,
        BackSpace = 2,
        EnSure=3,
    }
    class VirtualKeyboardFrame : ClientFrame
    {
        
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Mall/VirtualKeyboard";
        }

        protected override void _OnOpenFrame()
        {
            if (userData != null)
            {
                if (userData is float)
                {
                    float offsetX = (float)userData;
                    var pos = mBind.transform.position;
                    pos.x += offsetX;
                    mBind.transform.position = pos;
                    var rectTransform = (this.mCloseBg.transform as RectTransform);
                    //移动之后，close按钮的范围也要相应调整
                    if (rectTransform != null)
                    {
                        var parentRect = (mBind.transform as RectTransform);
                        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.rect.width + Mathf.Abs(parentRect.offsetMin.x * 2));
                    }
                }
                else if (userData is Vector3)
                {
                    Vector3 vtr3 = (Vector3)userData;
                    mBind.transform.position = vtr3;
                }
            }
        }

        protected override void _OnCloseFrame()
        {
            
        }

        #region ExtraUIBind
        private Button mKeyDel = null;
        private Button mKeyClose = null;
        private Button mKey0 = null;
        private Button mKey9 = null;
        private Button mKey8 = null;
        private Button mKey7 = null;
        private Button mKey6 = null;
        private Button mKey5 = null;
        private Button mKey4 = null;
        private Button mKey3 = null;
        private Button mKey2 = null;
        private Button mKey1 = null;
        private Button mCloseBg = null;

        protected override void _bindExUI()
        {
            mKeyDel = mBind.GetCom<Button>("KeyDel");
            mKeyDel.onClick.AddListener(_onKeyDelButtonClick);
            mKeyClose = mBind.GetCom<Button>("KeyClose");
            mKeyClose.onClick.AddListener(_onKeyCloseButtonClick);
            mKey0 = mBind.GetCom<Button>("Key0");
            mKey0.onClick.AddListener(_onKey0ButtonClick);
            mKey9 = mBind.GetCom<Button>("Key9");
            mKey9.onClick.AddListener(_onKey9ButtonClick);
            mKey8 = mBind.GetCom<Button>("Key8");
            mKey8.onClick.AddListener(_onKey8ButtonClick);
            mKey7 = mBind.GetCom<Button>("Key7");
            mKey7.onClick.AddListener(_onKey7ButtonClick);
            mKey6 = mBind.GetCom<Button>("Key6");
            mKey6.onClick.AddListener(_onKey6ButtonClick);
            mKey5 = mBind.GetCom<Button>("Key5");
            mKey5.onClick.AddListener(_onKey5ButtonClick);
            mKey4 = mBind.GetCom<Button>("Key4");
            mKey4.onClick.AddListener(_onKey4ButtonClick);
            mKey3 = mBind.GetCom<Button>("Key3");
            mKey3.onClick.AddListener(_onKey3ButtonClick);
            mKey2 = mBind.GetCom<Button>("Key2");
            mKey2.onClick.AddListener(_onKey2ButtonClick);
            mKey1 = mBind.GetCom<Button>("Key1");
            mKey1.onClick.AddListener(_onKey1ButtonClick);
            mCloseBg = mBind.GetCom<Button>("CloseBg");
            mCloseBg.onClick.AddListener(_onCloseBgButtonClick);
        }

        protected override void _unbindExUI()
        {
            mKeyDel.onClick.RemoveListener(_onKeyDelButtonClick);
            mKeyDel = null;
            mKeyClose.onClick.RemoveListener(_onKeyCloseButtonClick);
            mKeyClose = null;
            mKey0.onClick.RemoveListener(_onKey0ButtonClick);
            mKey0 = null;
            mKey9.onClick.RemoveListener(_onKey9ButtonClick);
            mKey9 = null;
            mKey8.onClick.RemoveListener(_onKey8ButtonClick);
            mKey8 = null;
            mKey7.onClick.RemoveListener(_onKey7ButtonClick);
            mKey7 = null;
            mKey6.onClick.RemoveListener(_onKey6ButtonClick);
            mKey6 = null;
            mKey5.onClick.RemoveListener(_onKey5ButtonClick);
            mKey5 = null;
            mKey4.onClick.RemoveListener(_onKey4ButtonClick);
            mKey4 = null;
            mKey3.onClick.RemoveListener(_onKey3ButtonClick);
            mKey3 = null;
            mKey2.onClick.RemoveListener(_onKey2ButtonClick);
            mKey2 = null;
            mKey1.onClick.RemoveListener(_onKey1ButtonClick);
            mKey1 = null;
            mCloseBg.onClick.RemoveListener(_onCloseBgButtonClick);
            mCloseBg = null;
        }
        #endregion

        #region Callback
        private void _onKeyDelButtonClick()
        {
            /* put your code in here */
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeNum, ChangeNumType.BackSpace);
        }
        private void _onKeyCloseButtonClick()
        {
            /* put your code in here */
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeNum, ChangeNumType.EnSure);
            frameMgr.CloseFrame(this);
        }
        private void _onKey0ButtonClick()
        {
            /* put your code in here */
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeNum, ChangeNumType.Add, 0);
        }
        private void _onKey9ButtonClick()
        {
            /* put your code in here */
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeNum, ChangeNumType.Add, 9);
        }
        private void _onKey8ButtonClick()
        {
            /* put your code in here */
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeNum, ChangeNumType.Add, 8);
        }
        private void _onKey7ButtonClick()
        {
            /* put your code in here */
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeNum, ChangeNumType.Add, 7);
        }
        private void _onKey6ButtonClick()
        {
            /* put your code in here */
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeNum, ChangeNumType.Add, 6);
        }
        private void _onKey5ButtonClick()
        {
            /* put your code in here */
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeNum, ChangeNumType.Add, 5);
        }
        private void _onKey4ButtonClick()
        {
            /* put your code in here */
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeNum, ChangeNumType.Add, 4);
        }
        private void _onKey3ButtonClick()
        {
            /* put your code in here */
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeNum, ChangeNumType.Add, 3);
        }
        private void _onKey2ButtonClick()
        {
            /* put your code in here */
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeNum, ChangeNumType.Add, 2);
        }
        private void _onKey1ButtonClick()
        {
            /* put your code in here */
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeNum, ChangeNumType.Add, 1);
        }

        private void _onCloseBgButtonClick()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeNum, ChangeNumType.EnSure);
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}