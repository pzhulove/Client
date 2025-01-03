using UnityEngine.UI;

namespace GameClient
{
    class PkSeekWaitingData
    {
        public PkSeekWaiting.OnClickCancel onClickCancel;
        public PkRoomType roomtype;
    }

    class PkSeekWaiting : ClientFrame
    {
        public delegate void OnClickCancel();
        protected PlayerInfo playerInfo = ClientApplication.playerinfo;

        PkSeekWaitingData m_kData = null;

        float ShowTime = 0f;
        float time = 0f;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk/Seek";
        }

        protected override void _OnOpenFrame()
        {
            m_kData = userData as PkSeekWaitingData;

            InitInterface();
            BindUIEvent();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideEnd);
        }

        protected override void _OnCloseFrame()
        {
            ShowTime = 0f;
            time = 0f;
            m_kData = null;

            UnBindUIEvent();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideStart);
        }

        protected void BindUIEvent()
        {
        }

        protected void UnBindUIEvent()
        {
        }

        void InitInterface()
        {
        }

        public void SetEstimateTime(string content)
        {
            EstimateTime.text = content;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            time += timeElapsed;

            if(time < 1f)
            {
                return;
            }

            ShowTime += time;

//             float fShowTime = preitime - time;
//             int iShowTime = (int)fShowTime;

            if (m_kData != null)
            {
                if(m_kData.roomtype == PkRoomType.TraditionPk || m_kData.roomtype == PkRoomType.Pk3v3 || m_kData.roomtype == PkRoomType.Pk3v3Cross 
                    || m_kData.roomtype == PkRoomType.Pk2v2Cross)
                {
                    countDownText.text = string.Format("匹配中...({0})", Function.GetLastsTimeStr(ShowTime));
                }
                else
                {
                    countDownText.text = ((int)ShowTime).ToString();
                }
            }

            time = 0f;
        }

        [UIControl("CountDown")]
        protected Text countDownText;

        [UIControl("Text")]
        protected Text EstimateTime;
    }
}
