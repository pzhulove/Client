using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{
    class TeamMatchPlayersFrame : ClientFrame
    {
        const int MemberNum = 3;

        string dungeonName = "";
        float MaxWaitTime = 5.0f;
        float fAddUpTime = 0.0f;

        WorldTeamMatchResultNotify resultData = new WorldTeamMatchResultNotify();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Team/TeamMatchPlayersFrame";
        }

        protected override void _OnOpenFrame()
        {
            resultData = userData as WorldTeamMatchResultNotify;

            InitInterface();
            BindUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            Clear();
            UnBindUIEvent();
        }

        void Clear()
        {
            resultData = new WorldTeamMatchResultNotify();

            dungeonName = "";
            MaxWaitTime = 15.0f;
            fAddUpTime = 0.0f;
        }

        void BindUIEvent()
        {
        }

        void UnBindUIEvent()
        {
        }

        [UIEventHandle("middle/cancel")]
        void OnCancel()
        {
            WorldTeamMatchCancelReq msg = new WorldTeamMatchCancelReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            frameMgr.CloseFrame(this);
        }

        void InitInterface()
        {
            DungeonTable Dungeondata = TableManager.GetInstance().GetTableItem<DungeonTable>((int)resultData.dungeonId);
            if (Dungeondata == null)
            {
                return;
            }

            dungeonName = Dungeondata.Name;

            for (int i = 0; i < MemberNum; i++)
            {
                if (i < resultData.players.Length && resultData.players[i].id != 0)
                {
                    JobTable data = TableManager.GetInstance().GetTableItem<JobTable>(resultData.players[i].occu);
                    if (data == null)
                    {
                        continue;
                    }

                    //Sprite spr = AssetLoader.instance.LoadRes(data.JobHalfBody, typeof(Sprite)).obj as Sprite;
                    //if (spr != null)
                    //{
                    //    icons[i].sprite = spr;
                    //}
                    ETCImageLoader.LoadSprite(ref icons[i], data.JobHalfBody);

                    Lvs[i].text = string.Format("Lv.{0}", resultData.players[i].level);
                    Names[i].text = resultData.players[i].name;
                    Occus[i].text = data.Name;

                    members[i].gameObject.SetActive(true);
                }
                else
                {
                    members[i].gameObject.SetActive(false);
                }
            }
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            fAddUpTime += timeElapsed;

            if (fAddUpTime > 1.0f)
            {
                MaxWaitTime -= 1.0f;
                fAddUpTime = 0.0f;

                int iInt = (int)(MaxWaitTime);

                DungeonName.text = string.Format("{0}秒后进入{1}", iInt, dungeonName);
            }
        }

        [UIControl("middle/title/text")]
        Text DungeonName;

        [UIControl("middle/memberroot/mem{0}", typeof(Image), 1)]
        Image[] members = new Image[MemberNum];

        [UIControl("middle/memberroot/mem{0}/Mask/OccuHead", typeof(Image), 1)]
        Image[] icons = new Image[MemberNum];

        [UIControl("middle/memberroot/mem{0}/Lv", typeof(Text), 1)]
        Text[] Lvs = new Text[MemberNum];

        [UIControl("middle/memberroot/mem{0}/nameback/Name", typeof(Text), 1)]
        Text[] Names = new Text[MemberNum];

        [UIControl("middle/memberroot/mem{0}/nameback/Occu", typeof(Text), 1)]
        Text[] Occus = new Text[MemberNum];
    }
}
