using UnityEngine;

namespace GameClient
{
    public class AnnouncementFrame : ClientFrame
    {
        public delegate void PlayEnd();
        public PlayEnd ResPlayEnd;

        float ShowAreaWidth = 880f;
        float TextLength = 0f;
        float rollSpeed = 30 * 4.0f;

        AnnounceData contentData;

        protected override void _OnOpenFrame()
        {
            //城镇中的时候把跑马灯的位置调低
            ClientSystemTown townSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if(townSystem!=null)
            {
                mAnnouncementframe.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -217);
            }
            else
            {
                mAnnouncementframe.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -158);
            }
            


            contentData = (AnnounceData)userData;

            mContent.SetText(contentData.content, true);

            mContent.gameObject.transform.localPosition = new Vector3(
                    ShowAreaWidth / 2f,
                    mContent.gameObject.transform.localPosition.y,
                    mContent.gameObject.transform.localPosition.z);

            TextLength = mContent.LinkTextWidth;
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/AnnouncementFrame";
        }

        protected override void _OnLoadPrefabFinish()
        {
            ComClientFrame com = mComClienFrame as ComClientFrame;

            if (null != com)
            {
                //com.bInitWithGameBindSystem = false;
            }
        }

        void ClearData()
        {
            ShowAreaWidth = 880f;
            TextLength = 0f;
            rollSpeed = 30 * 4.0f;
            ResPlayEnd = null;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
			if (mContent == null)
            {
                return;
            }

            ClientSystemTown townSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            ClientSystemBattle battleSystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemBattle;

            if (townSystem == null && battleSystem == null)
            {
                return;
            }

            if (mContent.transform.localPosition.x < (-(ShowAreaWidth / 2f + TextLength)))
            {
                if (ResPlayEnd != null)
                {
                    ResPlayEnd();
                    return;
                }
            }

            mContent.transform.localPosition = new Vector3(
            mContent.transform.localPosition.x - rollSpeed * (timeElapsed),
            mContent.transform.localPosition.y,
            mContent.transform.localPosition.z);
        }

        #region ExtraUIBind
        private LinkParse mContent = null;
        private GameObject mAnnouncementframe = null;

        protected override void _bindExUI()
        {
            mContent = mBind.GetCom<LinkParse>("Content");
            mAnnouncementframe = mBind.GetGameObject("Announcementframe");
        }

        protected override void _unbindExUI()
        {
            mContent = null;
            mAnnouncementframe = null;
        }
        #endregion
    }
}
