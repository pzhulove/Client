using DG.Tweening;
using Protocol;
using Network;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GameClient
{
    class PkLoadingFrame : ClientFrame
    {
        public float animTime = 0f;
#if UNITY_EDITOR
        protected int _UpdateSpeed = 1000;
#else
        protected int _UpdateSpeed = 10;
#endif

        protected int _targetProgress = 0;
        protected int _currentProgress = -1;
        ComGameBase  comBase;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk/PkLoading";
        }

        protected override void _OnOpenFrame()
        {
            mLeftProgress.value = 0.0f;
            mRightProgress.value = 0.0f;

            getAnimationTime();
            InitUI();

            comBase = frame.AddComponent<ComGameBase>();
            //StartCoroutine(LoadStartLoading());
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
        }

        void ClearData()
        {
            animTime = 0f;
        }

        void InitUI()
        {
            if(ClientApplication.racePlayerInfo.Length < 2)
            {
                Logger.LogError("init pk players num < 2");
                return;
            }

            var left = ClientApplication.racePlayerInfo[0];

            if(left == null)
            {
                Logger.LogError("init pk Left player is null");
                return;
            }

            var right = ClientApplication.racePlayerInfo[1];
            if(right == null)
            {
                Logger.LogError("init pk right player is null");
                return;
            }

            // 名字
            mLeftName.text = left.name;
            mRightName.text = right.name;       
            
            // 等级
//             left_lv.text = "Lv."+left.level.ToString();
//             right_lv.text = "Lv."+right.level.ToString();

            //决斗积分
//             left_matchScore.text = left.matchScore.ToString();
//             right_matchScore.text = right.matchScore.ToString();

            // 职业
            mLeftJob.text = Utility.GetJobName(left.occupation,0);
            mRightJob.text = Utility.GetJobName(right.occupation, 0);

            // 人物icon
            if (mLeftPerson)
            {
                var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(left.occupation);

                if (jobData != null && jobData.JobPortrayal != "" && jobData.JobPortrayal != "-")
                {
                    //Sprite Icon = AssetLoader.instance.LoadRes(jobData.JobPortrayal,typeof(Sprite)).obj as Sprite;
                    //if(Icon != null)
                    //{
                    //    mLeftPerson.sprite = Icon;
                    //}          
                    ETCImageLoader.LoadSprite(ref mLeftPerson, jobData.JobPortrayal);
                }
            }

            if(mRightPerson)
            {
                var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(right.occupation);

                if (jobData != null && jobData.JobPortrayal != "" && jobData.JobPortrayal != "-")
                {
                    //Sprite Icon = AssetLoader.instance.LoadRes(jobData.JobPortrayal, typeof(Sprite)).obj as Sprite;
                    //if(Icon != null)
                    //{
                    //    mRightPerson.sprite = Icon;
                    //}               
                    ETCImageLoader.LoadSprite(ref mRightPerson, jobData.JobPortrayal);
                }
            }         

            if(LeftComPlayerPKLevel != null)
            {
                LeftComPlayerPKLevel.SetPkLevel((int)left.seasonLevel);
            }

            if(RightComPlayerPKLevel != null)
            {
                RightComPlayerPKLevel.SetPkLevel((int)right.seasonLevel);
            }

            if(left.guildName != "")
            {
                mLeftGuild.text = string.Format("公会:{0}", left.guildName);
            }
            else
            {
                mLeftGuild.text = "";
            }

            if(right.guildName != "")
            {
                mRightGuild.text = string.Format("公会:{0}", right.guildName);
            }
            else
            {
                mRightGuild.text = "";
            }         

            //服务器
            if (right.zoneId != left.zoneId && right.zoneId != 0 && left.zoneId != 0)
            {
                mRightServerText.text = right.serverName;
                mLeftServerText.text = left.serverName;
            }
            else
            {
                mRightServerText.text ="";
                mLeftServerText.text ="";
            }
            frame.GetComponentsInChildren<DOTweenAnimation>();
        }

        protected void getAnimationTime()
        {
            var anims = GamePool.ListPool<DOTweenAnimation>.Get();
            frame.GetComponentsInChildren<DOTweenAnimation>(anims);

            animTime = 0;
            for (int i = 0; i < anims.Count; ++i)
            {
                var cur = anims[i];
                float time = cur.delay + cur.duration;

                if (animTime < time)
                {
                    animTime = time;
                }
            }

            GamePool.ListPool<DOTweenAnimation>.Release(anims);
        }

        public IEnumerator LoadStartLoading()
        {
            float time = Time.realtimeSinceStartup;

            //等待帧动画完成,等待开始消息
            while ((Time.realtimeSinceStartup - time) < animTime)
            {
                yield return Yielders.EndOfFrame;
            }

            /*
            while(!(ClientSystemManager.GetInstance().CurrentSystem is ClientSystemBattle))
            {
                yield return Yielders.EndOfFrame;
            }
            */
            //LoginRaceServer();

            yield return Yielders.GetWaitForSeconds(0.5f);

            comBase.StartCoroutine(UpdateProgress());
        }

        public IEnumerator UpdateProgress()
        {
            while (_targetProgress <= 100)
            {
                while (_currentProgress < _targetProgress)
                {
                    _currentProgress += _UpdateSpeed;
                    if (_currentProgress > _targetProgress)
                    {
                        _currentProgress = _targetProgress;
                    }

                    _SetProgress(_currentProgress);
                    yield return Yielders.EndOfFrame;
                }

                if (_targetProgress == 100)
                {
                    yield break;
                }

                yield return Yielders.EndOfFrame;

                _targetProgress = (int)(ClientSystemManager.GetInstance().SwitchProgress * 100.0f);
                //loadinfo.text = ClientSystemManager.GetInstance().GetSwitchSystemInfo(); 
            }
        }

        protected void _SetProgress(int progress)
        {
            if (progress < 0)
            {
                progress = 0;
            }
            if (progress > 100)
            {
                progress = 100;
            }

            if (mLeftProgress == null || mRightProgress == null)
            {
                return;
            }

            mLeftProgress.value = progress / 100.0f;
            mRightProgress.value = progress / 100.0f;

            mLeftProgressText.text = string.Format("{0}%", progress);
            mRightProgressText.text = string.Format("{0}%", progress);
        }

        #region ExtraUIBind
        private Text mLeftGuild = null;
        private Text mRightGuild = null;
        private Image mRightPerson = null;
        private Image mLeftPerson = null;
        private Text mRightName = null;
        private Text mLeftName = null;
        private Text mRightJob = null;
        private Text mLeftJob = null; 
        private Slider mRightProgress = null;
        private Slider mLeftProgress = null;
        private Text mRightProgressText = null;
        private Text mLeftProgressText = null;
        private Text mRightServerText = null;
        private Text mLeftServerText = null;
        private ComPlayerPKLevel LeftComPlayerPKLevel = null;
        private ComPlayerPKLevel RightComPlayerPKLevel = null;

        protected override void _bindExUI()
        {
            mLeftGuild = mBind.GetCom<Text>("LeftGuild");
            mRightGuild = mBind.GetCom<Text>("RightGuild");
            mRightPerson = mBind.GetCom<Image>("RightPerson");
            mLeftPerson = mBind.GetCom<Image>("LeftPerson");
            mRightName = mBind.GetCom<Text>("RightName");
            mLeftName = mBind.GetCom<Text>("LeftName");
            mRightJob = mBind.GetCom<Text>("RightJob");
            mLeftJob = mBind.GetCom<Text>("LeftJob");       
            mRightProgress = mBind.GetCom<Slider>("RightProgress");
            mLeftProgress = mBind.GetCom<Slider>("LeftProgress");
            mRightProgressText = mBind.GetCom<Text>("RightProgressText");
            mLeftProgressText = mBind.GetCom<Text>("LeftProgressText");
            mRightServerText = mBind.GetCom<Text>("RightServerText");
            mLeftServerText = mBind.GetCom<Text>("LeftServerText");
            LeftComPlayerPKLevel = mBind.GetCom<ComPlayerPKLevel>("LeftComPlayerPKLevel");
            RightComPlayerPKLevel = mBind.GetCom<ComPlayerPKLevel>("RightComPlayerPKLevel");
        }

        protected override void _unbindExUI()
        {
            mLeftGuild = null;
            mRightGuild = null;
            mRightPerson = null;
            mLeftPerson = null;
            mRightName = null;
            mLeftName = null;
            mRightJob = null;
            mLeftJob = null;       
            mRightProgress = null;
            mLeftProgress = null;
            mRightProgressText = null;
            mLeftProgressText = null;
            mRightServerText = null;
            mLeftServerText = null;
            LeftComPlayerPKLevel = null;
            RightComPlayerPKLevel = null;
        }
        #endregion

        #region not use
        // 段位
        //             int RemainPoints = 0;
        //             int TotalPoints = 0;
        //             int pkIndex = 0;
        //             bool bMaxLv = false;
        // 
        //             string PkLeftPath = Utility.GetPathByPkPoints(left.pkValue, ref RemainPoints, ref TotalPoints, ref pkIndex, ref bMaxLv);
        //             if (PkLeftPath != "" && PkLeftPath != "-" && PkLeftPath != "0")
        //             {
        //                 Sprite Icon = AssetLoader.instance.LoadRes(PkLeftPath, typeof(Sprite)).obj as Sprite;
        // 
        //                 if(Icon != null)
        //                 {
        //                     left_pkLv.sprite = Icon;
        //                 }           
        // 
        //                 left_pkLv.gameObject.SetActive(true);
        //             }
        //             else
        //             {
        //                 left_pkLv.gameObject.SetActive(false);
        //             }

        //             int iPkLevelType = 0;

        //             left_pkLv.text = Utility.GetNameByPkPoints(left.pkValue, ref iPkLevelType);
        // 
        //             UnityEngine.UI.Extensions.Gradient com = left_pkLv.GetComponent<UnityEngine.UI.Extensions.Gradient>();
        //             if (com != null)
        //             {
        //                 Utility.GetPKValueNumAndColor(iPkLevelType, ref com.vertex1, ref com.vertex2);
        //             }


        //             right_pkLv.text = Utility.GetNameByPkPoints(right.pkValue, ref iPkLevelType);
        // 
        //             UnityEngine.UI.Extensions.Gradient comRight = right_pkLv.GetComponent<UnityEngine.UI.Extensions.Gradient>();
        //             if (comRight != null)
        //             {
        //                 Utility.GetPKValueNumAndColor(iPkLevelType, ref comRight.vertex1, ref comRight.vertex2);
        //             }

        //             string PkRightPath = Utility.GetPathByPkPoints(right.pkValue, ref RemainPoints, ref TotalPoints, ref pkIndex, ref bMaxLv);
        //             if (PkRightPath != "" && PkRightPath != "-" && PkRightPath != "0")
        //             {
        //                 Sprite Icon = AssetLoader.instance.LoadRes(PkRightPath, typeof(Sprite)).obj as Sprite;
        //                 if(Icon != null)
        //                 {
        //                     right_pkLv.sprite = Icon;
        //                 }
        //                 
        //                 right_pkLv.gameObject.SetActive(true);
        //             }
        //             else
        //             {
        //                 right_pkLv.gameObject.SetActive(false);
        //             }

        //public void LoginRaceServer()
        //{
        //	if (ReplayServer.GetInstance().IsReplay())
        //		return;
        //    Logger.LogWarning("!!!!!!!!!!!!!!!LoginRaceServer.. \n");
        //    FrameSync.instance.Init();
        //    RelaySvrLoginReq req = new RelaySvrLoginReq();
        //    req.accid = ClientApplication.playerinfo.accid;
        //    req.roleid = PlayerBaseData.GetInstance().RoleID;
        //    req.seat = ClientApplication.playerinfo.seat;
        //    req.session = ClientApplication.playerinfo.session;
        //    //NetManager.instance.SendCommand(ServerType.RELAY_SERVER, req);
        //    Logger.LogWarningFormat("RelaySvrLoginReq accid {0} roleid {1} seatid {2} sessionid {3} \n", req.accid, req.roleid, req.seat, req.session);
        //}

        // 这个协议返回的是所有玩家的登陆结果
        //[MessageHandle(RelaySvrNotifyGameStart.MsgID)]
        //void OnRelaySvrNotifyGameStart(MsgDATA msg)
        //{
        //    RelaySvrNotifyGameStart ret = new RelaySvrNotifyGameStart();
        //    ret.decode(msg.bytes);

        //    Logger.LogWarningFormat("OnRelaySvrNotifyGameStart {0}\n", ObjectDumper.Dump(ret));

        //    ClearData();
        //    //FrameSync.instance.OnRelaySvrNotifyGameStart(ret);
        //    frameMgr.CloseFrame(this);
        //           
        //    var obj = ClientSystemManager.instance.PlayUIEffect(FrameLayer.Top, "UIFlatten/Prefabs/Pk/StartFight");

        //	{
        //		var actor = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
        //		if (actor != null)
        //		{
        //			DoublePressConfigCommand cmd = new DoublePressConfigCommand();
        //			cmd.hasDoublePress = actor.hasDoublePress;
        //			FrameSync.instance.FireFrameCommand(cmd);
        //		}
        //	}

        //	//同步的倒计时同步
        //	BattleMain.instance.GetDungeonManager().GetBeScene().StartPKCountDown(Global.PK_COUNTDOWN_TIME, ()=>{
        //		if (obj != null)
        //		{
        //			GameObject.Destroy(obj);
        //			obj = null;
        //				var curSystem = ClientSystemManager.instance.CurrentSystem as ClientSystemBattle;
        //				if (curSystem != null)
        //				{
        //					curSystem.StartTimer(Global.PK_TOTAL_TIME);
        //				}
        //				var players = BattleMain.instance.GetPlayerManager().GetAllPlayers();
        //				for(int i=0; i<players.Count; ++i)
        //				{
        //					var actor = players[i].playerActor;
        //					if (actor != null)
        //						actor.pkRestrainPosition = false;
        //				}
        //			}
        //		});
        //    if (BattleMain.battleType == BattleType.MutiPlayer || BattleMain.battleType == BattleType.GuildPVP)
        //    {
        //		//机器人开启AI！！！！
        //		ClientSystemManager.instance.delayCaller.DelayCall(Global.PK_COUNTDOWN_TIME * 1000, () =>
        //        {
        //			var players = BattleMain.instance.GetPlayerManager().GetAllPlayers();
        //			for(int i=0; i<players.Count; ++i)
        //			{
        //				var actor = players[i].playerActor;
        //				if (actor != null && actor.aiManager != null)
        //				{
        //					actor.pauseAI = false;

        //					if (RecordServer.GetInstance().IsRecord())
        //					{
        //						RecordServer.GetInstance().RecordProcess("start frame!!!");
        //					}

        //					if (RecordServer.GetInstance().NeedRecord())
        //					{
        //						RecordServer.GetInstance().RecordStartFrame();
        //					}

        //					break;
        //				}
        //			}
        //        });
        //    }
        //}
        #endregion
    }
}
