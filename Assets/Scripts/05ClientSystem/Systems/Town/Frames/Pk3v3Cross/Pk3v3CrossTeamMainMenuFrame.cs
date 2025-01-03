using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{  
    class Pk3v3CrossTeamMainMenuFrame : ClientFrame
    {
        class TeamMainMenuData
        {
            public Vector2 uiPos;
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3Cross/Pk3v3CrossTeamMainMenu";
        }

        protected override void _OnOpenFrame()
        {
            _Initialize();
            _BindUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            _Clear();
        }

        void _Clear()
        {
            _UnBindUIEvent();
        }

        void _BindUIEvent()
        {
            
        }

        void _UnBindUIEvent()
        {
           
        }
        void SendCancelOnePersonMatchGameReq()
        {
            WorldMatchCancelReq req = new WorldMatchCancelReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        [UIEventHandle("Content/funcs/CreateTeam/Button")]
        void _OnCreateTeamClicked()
        {
            Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
            if (pkInfo != null && pkInfo.nCurPkCount >= Pk3v3CrossDataManager.MAX_PK_COUNT)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("挑战次数已达上限，操作失败");
                return;
            }

            if (frameMgr.IsFrameOpen<PkSeekWaiting>())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("匹配中无法进行该操作，请取消后再试");
                return;
            }

            Pk3v3CrossDataManager.GetInstance().SendCreateRoomReq(RoomType.ROOM_TYPE_THREE_SCORE_WAR);
            //frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Content/funcs/MyTeam/Button")]
        void _OnMyTeamClicked()
        {
            ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossMyTeamFrame>();
            //frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Content/funcs/TeamList/Button")]
        void _OnTeamListClicked()
        {
            ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossTeamListFrame>();
            //frameMgr.CloseFrame(this);
        }

        [UIEventHandle("TeamList")]
        void _OnTeamListClicked1()
        {
            ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossTeamListFrame>();
            //frameMgr.CloseFrame(this);
        }

        void _Initialize()
        {     
            _SetupFuncBtns();         
        }

        void _SetupFuncBtns()
        {
            bool hasMyTeam = TeamDataManager.GetInstance().HasTeam();
            hasMyTeam = false;

            Utility.FindGameObject(m_content, "funcs/CreateTeam").SetActive(hasMyTeam == false);
            Utility.FindGameObject(m_content, "funcs/MyTeam").SetActive(hasMyTeam);
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            if (Pk3v3CrossDataManager.GetInstance().HasTeam())
            {
                ClientSystemManager.GetInstance().CloseFrame<Pk3v3CrossTeamMainMenuFrame>();
            }
        }

        [UIObject("Content")]
        GameObject m_content;        
    }
}
