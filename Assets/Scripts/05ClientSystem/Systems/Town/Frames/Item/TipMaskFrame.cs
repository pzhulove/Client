using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameClient
{
    /*
    class TipMaskData
    {
        public TextAnchor textAnchor;
        public bool enableMask;
    }

    class TipMaskFrame : ClientFrame
    {
        ComButtonEx m_btnMask;
        GameObject m_objContent;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Tip/Mask";
        }

        protected override void _OnOpenFrame()
        {
            Logger.LogWarningFormat("TipMaskFrame ==> _OnOpenFrame");
            TipMaskData maskData = userData as TipMaskData;
            if (maskData == null)
            {
                Logger.LogError("open tip mask frame, user data is invalid!!");
                return;
            }

            m_objContent = Utility.FindGameObject(frame, "Anchor/Content");
            RectTransform contentRect = m_objContent.GetComponent<RectTransform>();
            switch (maskData.textAnchor)
            {
                case TextAnchor.MiddleLeft:
                    {
                        contentRect.anchorMin = new Vector2(0.0f, 0.5f);
                        contentRect.anchorMax = new Vector2(0.0f, 0.5f);
                        contentRect.pivot = new Vector2(0.0f, 0.5f);
                        break;
                    }
                case TextAnchor.MiddleCenter:
                    {
                        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
                        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
                        contentRect.pivot = new Vector2(0.5f, 0.5f);
                        break;
                    }
                case TextAnchor.MiddleRight:
                    {
                        contentRect.anchorMin = new Vector2(1.0f, 0.5f);
                        contentRect.anchorMax = new Vector2(1.0f, 0.5f);
                        contentRect.pivot = new Vector2(1.0f, 0.5f);
                        break;
                    }
            }
            frame.GetComponent<HorizontalLayoutGroup>().childAlignment = maskData.textAnchor;

            //m_btnMask = frame.GetComponent<ComButtonEx>();
            //m_btnMask.onMouseDown.RemoveAllListeners();
            //m_btnMask.onMouseDown.AddListener((var)=> { ItemTipManager.GetInstance().CloseContent(); });
            //m_btnMask.onClick.RemoveAllListeners();
            //m_btnMask.onClick.AddListener(() => { ItemTipManager.GetInstance().CloseNormalTip(); });

            //             EventTrigger eventTrigger = frame.AddComponent<EventTrigger>();
            //             eventTrigger.triggers = new List<EventTrigger.Entry>();
            //             EventTrigger.Entry entry = new EventTrigger.Entry();
            //             entry.eventID = EventTriggerType.PointerDown;
            //             entry.callback = new EventTrigger.TriggerEvent();
            //             entry.callback.AddListener((value) => { _OnMaskClicked(); });
            //             eventTrigger.triggers.Add(entry);

            if (maskData.enableMask == false)
            {
                m_btnMask.penetrable = true;
                //frame.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }
            else
            {
                m_btnMask.penetrable = false;
                //frame.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            }
        }

        protected override void _OnCloseFrame()
        {
            m_btnMask = null;
            m_objContent = null;

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemTipClosed);
        }

        public GameObject GetContentRoot()
        {
            return m_objContent;
        }
    }
    */
}
