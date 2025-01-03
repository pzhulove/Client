using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{
    public enum MatchState
    {
        None = 0,
        TeamJoin,
        TeamMatch,
    }

    public class TeamMatchWaitingData
    {
        public MatchState matchState;
        public int TeamDungeonTableID;

        public void Clear()
        {
            matchState = MatchState.None;
            TeamDungeonTableID = 0;
        }
    }

    class TeamMatchWaitingFrame : ClientFrame
    {
        TeamMatchWaitingData data = new TeamMatchWaitingData();
        float fAddUpTime = 0.0f;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Team/TeamMatchWaitingFrame";
        }

        protected override void _OnOpenFrame()
        {
            data = userData as TeamMatchWaitingData;

            InitInterface();
            BindUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            _Clear();
        }

        protected override void _bindExUI()
        {
            state = mBind.GetCom<Text>("state");
            targetDungeon = mBind.GetCom<Text>("targetDungeon");
        }

        protected override void _unbindExUI()
        {
            state = null;
            targetDungeon = null;
        }

        void _Clear()
        {
            data.Clear();
            fAddUpTime = 0.0f;

            UnBindUIEvent();
        }

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamAddMemberSuccess, OnAddMemberSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TeamJoinSuccess, OnTeamJoinSuccess);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamAddMemberSuccess, OnAddMemberSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TeamJoinSuccess, OnTeamJoinSuccess);
        }

        [UIEventHandle("middle/btCancel")]
        void OnCancel()
        {
            WorldTeamMatchCancelReq msg = new WorldTeamMatchCancelReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            frameMgr.CloseFrame(this);
        }

        void InitInterface()
        {
            TeamDungeonTable tabledata = TableManager.GetInstance().GetTableItem<TeamDungeonTable>(data.TeamDungeonTableID);
            if (tabledata == null)
            {
                return;
            }

            if (data.matchState == MatchState.TeamJoin)
            {
                //state.text = "组队中...";
                state.text = TR.Value("team_matching_teaming");
            }
            else if (data.matchState == MatchState.TeamMatch)
            {
                //state.text = "匹配中...";
                state.text = TR.Value("team_matching_matching");
            }

            targetDungeon.text = TR.Value("team_matching_target_info", tabledata.Name);
        }

        void OnAddMemberSuccess(UIEvent iEvent)
        {
            frameMgr.CloseFrame(this);
        }

        void OnTeamJoinSuccess(UIEvent iEvent)
        {
            frameMgr.CloseFrame(this);
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            fAddUpTime += timeElapsed;
            timer.text = Function.GetLastsTimeStr(fAddUpTime);
        }

        //[UIControl("middle/state")]
        Text state;

        [UIControl("middle/timer")]
        Text timer;

        //[UIControl("middle/Text/targetDungeon")]
        Text targetDungeon;
    }
}
