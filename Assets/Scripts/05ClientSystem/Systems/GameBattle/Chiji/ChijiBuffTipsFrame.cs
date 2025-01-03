using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ChijiBuffTipsFrame : ClientFrame
    {
        private int ChiJiBuffID = 0;
        private bool isUpdate = false;
        private float chijiBuffTipsTime = 3f;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiBuffTips";
        }

        protected override void _OnOpenFrame()
        {
            ChiJiBuffID = (int)userData;

            InitInterface();
            SetupFramePosition(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        }

        protected override void _OnCloseFrame()
        {
            ChiJiBuffID = 0;
            isUpdate = false;
            chijiBuffTipsTime = 3;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            if (isUpdate == false)
            {
                return;
            }

            chijiBuffTipsTime -= timeElapsed;
            if (chijiBuffTipsTime <= 0)
            {
                Close();
            }
        }

        private void InitInterface()
        {
            isUpdate = true;
            var chijiBuffTable = TableManager.GetInstance().GetTableItem<ChijiBuffTable>(ChiJiBuffID);
            if (chijiBuffTable == null)
            {
                return;
            }

            if (mBuffName != null)
            {
                mBuffName.text = chijiBuffTable.Name;
            }

            if (mBuffDesc != null)
            {
                mBuffDesc.text = chijiBuffTable.Description;
            }
        }

        void SetupFramePosition(Vector2 pos)
        {
            RectTransform rectContent = mContent.GetComponent<RectTransform>();
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

		#region ExtraUIBind
		private Text mBuffName = null;
		private Text mBuffDesc = null;
		private GameObject mContent = null;
		private Button mClose = null;
		
		protected override void _bindExUI()
		{
			mBuffName = mBind.GetCom<Text>("BuffName");
			mBuffDesc = mBind.GetCom<Text>("BuffDesc");
			mContent = mBind.GetGameObject("Content");
			mClose = mBind.GetCom<Button>("Close");
			if (null != mClose)
			{
				mClose.onClick.AddListener(_onCloseButtonClick);
			}
		}
		
		protected override void _unbindExUI()
		{
			mBuffName = null;
			mBuffDesc = null;
			mContent = null;
			if (null != mClose)
			{
				mClose.onClick.RemoveListener(_onCloseButtonClick);
			}
			mClose = null;
		}
        #endregion
        #region Callback
        private void _onCloseButtonClick()
        {
            Close();
        }
        #endregion
    }
}
