using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    class ComMenuFrame : ClientFrame
    {
        
        GameObject m_content;
        Button m_close;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/ComMenu";
        }

        protected override void _OnOpenFrame()
        {
            _Initialize();
        }

        protected override void _OnCloseFrame()
        {
            
        }


        #region private
        void _Initialize()
        {
            m_content = Utility.FindGameObject(frame, "Content");
            m_close = Utility.GetComponetInChild<Button>(frame, "BG");
            m_close.onClick.AddListener(_OnCloseClicked);
            _SetupFramePosition(new Vector2(Input.mousePosition.x, Input.mousePosition.y));  
        } 

        void _SetupFramePosition(Vector2 pos)
        {
            RectTransform rectContent = m_content.GetComponent<RectTransform>();
            RectTransform rectParent = rectContent.transform.parent as RectTransform;
            Vector2 localPos;
            bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, pos, ClientSystemManager.GetInstance().UICamera, out localPos);
            if (!success)
            {
                return;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectContent);

            Vector2 bounder = new Vector2(10.0f, 10.0f);
            float xMin = bounder.x;
            float xMax = rectParent.rect.size.x - bounder.x - rectContent.rect.size.x;
            float yMax = bounder.y;
            float yMin = -(rectParent.rect.size.y - bounder.y - rectContent.rect.size.y);

            localPos.x = Mathf.Clamp(localPos.x, xMin, xMax);
            localPos.y = Mathf.Clamp(localPos.y, yMin, yMax);

            rectContent.anchoredPosition = localPos;
        } 

        public void _OnCloseClicked()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnCloseMenu);
        }
        #endregion
    }
}
