using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameClient
{
    class CostItemNotifyData
    {
        public string       strContent;
        public UnityAction  delOnOkCallback;
        public UnityAction  delOnCancelCallback;
    }

    class CostItemNotifyFrame : ClientFrame
    {
        [UIControl("Back/TextPanel/AlertText")]
        Text m_labContent;

        [UIControl("Back/Panel/btOK")]
        Button m_btnOk;

        [UIControl("Back/Panel/btCancel")]
        Button m_btnCancel;

        [UIControl("Back/CanNotify")]
        Toggle m_togCanNotify;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/CostItemNotify/CostItemNotify";
        }

        protected override void _OnOpenFrame()
        {
            CostItemNotifyData data = userData as CostItemNotifyData;
            if (data != null)
            {
                m_labContent.text = data.strContent;

                m_btnOk.onClick.RemoveAllListeners();
                m_btnOk.onClick.AddListener(() =>
                {
                    frameMgr.CloseFrame(this);
                });
                if (data.delOnOkCallback != null)
                {
                    m_btnOk.onClick.AddListener(data.delOnOkCallback);
                }

                m_btnCancel.onClick.RemoveAllListeners();
                m_btnCancel.onClick.AddListener(() =>
                {
                    frameMgr.CloseFrame(this);
                });
                if (data.delOnCancelCallback != null)
                {
                    m_btnCancel.onClick.AddListener(data.delOnCancelCallback);
                }

                m_togCanNotify.onValueChanged.RemoveAllListeners();
                m_togCanNotify.isOn = !CostItemManager.GetInstance().isNotify;
                m_togCanNotify.onValueChanged.AddListener(var =>
                {
                    CostItemManager.GetInstance().isNotify = !var;
                });
            }
        }

        protected override void _OnCloseFrame()
        {

        }


    }
}
