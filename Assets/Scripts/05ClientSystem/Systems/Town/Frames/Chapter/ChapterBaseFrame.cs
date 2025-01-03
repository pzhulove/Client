using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Protocol;
using Network;
using ProtoTable;


namespace GameClient
{
    public class ChapterBaseFrame : ChapterCommonBoardFrame
    {
        private const string            kPrefabCharactor 	    = "UI/Prefabs/Chapter/ChapterCommonCharactor";
        private const string 			kPrefabCommonBg     	= "UI/Prefabs/Chapter/ChapterCommonBg";
        private const string 			kPrefabInfo    	    	= "UI/Prefabs/Chapter/ChapterCommonInfo";
        private const string 			kPrefabStart   			= "UI/Prefabs/Chapter/ChapterCommonStart";

        private const string            kBackgroud              = "UI/Image/Dungeon/Select/Select-the_difficulty_beijing.png";

        protected const string          kNameStart          	= "ChapterStart";
        protected const string 			kNameInfo        		= "ChapterInfo";


        protected IChapterInfoCommon    mChapterInfoCommon      = null;
        protected IChapterInfoDrops     mChapterInfoDrops       = null;
        protected IChapterInfoDiffculte mChapterInfoDiffculte   = null;
        protected IChapterCharactor     mChapterCharactor       = null;
        protected IChapterPassReward    mChapterPassReward      = null;
        protected IChapterScore         mChapterScore           = null;
        protected IChapterMonsterInfo   mChapterMonsterInfo     = null;

        protected IChapterProcess       mChapterProcess         = null;
        protected IChapterInfoDrugs     mChapterInfoDrugs       = null;
        protected IChapterDungeonMap    mChapterDungeonMap      = null;
        protected IChapterNodeState     mChapterNodeState       = null;
        protected IChapterConsume       mChapterConsume         = null;


        protected IChapterActivityTimes mChapterActivityTimes   = null;

        protected IChapterMask          mChapterMask            = null;   

        protected DungeonTable          mDungeonTable           = null;
        protected DungeonUIConfigTable  mDungeonUIConfigTable   = null;

        public static int               sDungeonID = -1;
        protected DungeonID             mDungeonID = new DungeonID(0);

        protected sealed override void _loadData()
        {
            mDungeonID.dungeonID = sDungeonID;
            _loadTableData();
        }

        protected void _loadTableData()
        {
            mDungeonTable = _getDungeonTable<DungeonTable>(mDungeonID.dungeonID);
            mDungeonUIConfigTable = _getDungeonTable<DungeonUIConfigTable>(mDungeonID.dungeonIDWithOutDiff);
        }

        protected override void _loadBg()
        {
            Sprite bg = null;
            string spritePath = "";
            if (null != mDungeonUIConfigTable)
            {
                bg = AssetLoader.instance.LoadRes(mDungeonUIConfigTable.BackgroundPath, typeof(Sprite)).obj as Sprite;
                spritePath = mDungeonUIConfigTable.BackgroundPath;
            }

            if (null == bg)
            {
                bg = AssetLoader.instance.LoadRes(kBackgroud, typeof(Sprite)).obj as Sprite;
                spritePath = kBackgroud;
            }

            // mCommonBoard.SetBackgroundImage(bg);
            mCommonBoard.SetBackgroundImage(spritePath);

            mCommonBoard.AttachPrefab(kPrefabCommonBg, ComCommonBoard.ePosition.Background);
        }

        protected void _loadCharactor()
        {
            GameObject charactorPanel = null;

            if (null != mDungeonUIConfigTable)
            {
                charactorPanel = mCommonBoard.AttachPrefab(mDungeonUIConfigTable.CharactorConfig, ComCommonBoard.ePosition.Right, 0);
            }
            
            if (null == charactorPanel)
            {
                charactorPanel = mCommonBoard.AttachPrefab(kPrefabCharactor, ComCommonBoard.ePosition.Right, 0);
            }

            if (null != charactorPanel)
            {
                var com = charactorPanel.GetComponent<ComChapterCharactor>();
                mChapterCharactor = com;
            }

            if (null != mChapterCharactor)
            {
                var curpid = PlayerBaseData.GetInstance().JobTableID;
                var item = TableManager.instance.GetTableItem<JobTable>(curpid);
                if (null != item)
                {
                    // mChapterCharactor.SetCharactor(AssetLoader.instance.LoadRes(item.JobPortrayal, typeof(Sprite)).obj as Sprite);
                    mChapterCharactor.SetCharactor(item.JobPortrayal);
                }
            }
        }

        protected override void _loadRightPanel()
        {
            _loadCharactor();

            GameObject rightPanel = null;

            if (null != mDungeonUIConfigTable)
            {
                rightPanel = mCommonBoard.AttachPrefab(mDungeonUIConfigTable.RightPannelConfig, ComCommonBoard.ePosition.Right);
            }

            if (null == rightPanel)
            {
                rightPanel = mCommonBoard.AttachPrefab(kPrefabStart, ComCommonBoard.ePosition.Right);
            }

            if (null != rightPanel)
            {
                rightPanel.name = kNameStart;
            }
        }

        protected override void _loadLeftPanel()
        {
            GameObject leftPanel = null;
            if (null != mDungeonUIConfigTable)
            {
                leftPanel = mCommonBoard.AttachPrefab(mDungeonUIConfigTable.LeftPannelConfig, ComCommonBoard.ePosition.Left);
            }

            // use common panel if not exist
            if (null == leftPanel)
            {
                leftPanel = mCommonBoard.AttachPrefab(kPrefabInfo, ComCommonBoard.ePosition.Left);
            }

            if (null != leftPanel)
            {
                leftPanel.name        = kNameInfo;

                var com               = leftPanel.GetComponent<ComCommonChapterInfo>();
                mChapterInfoCommon    = com;
                mChapterInfoDiffculte = com;
                mChapterInfoDrops     = com;
                mChapterPassReward    = com;
                mChapterScore         = com;
                mChapterMonsterInfo   = com;
                mChapterActivityTimes = com;
                mChapterProcess       = com;
                mChapterInfoDrugs     = com;
                mChapterDungeonMap    = com;
                mChapterNodeState     = com;
                mChapterConsume       = com;
            }
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            _updateCommonInfo();
            _updateDropInfo();
            _updateDiffculteInfo();
            _updatePassReward();
            _updateScore();
            _updateMonsterData();
			_updateChapterActivityTimes();
           
            //NetProcess.AddMsgHandler(WorldNotifyRaceStart.MsgID, _onNetStart);
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();

            //AssetGabageCollector.instance.ClearUnusedAsset();
            //NetProcess.RemoveMsgHandler(WorldNotifyRaceStart.MsgID, _onNetStart);
        }

        private void _updateChapterActivityTimes()
        {
            if (null != mChapterActivityTimes)
            {
                mChapterActivityTimes.SetActivityTimes(mDungeonID.dungeonID);
            }
        }

        protected virtual void _updateCommonInfo()
        {
            if (null != mChapterInfoCommon)
            {
                mChapterInfoCommon.SetName(mDungeonTable.Name);
                mChapterInfoCommon.SetDescription(mDungeonTable.Description);
                mChapterInfoCommon.SetRecommnedLevel(mDungeonTable.RecommendLevel);
                mChapterInfoCommon.SetRecommnedWeapon(mDungeonTable.RecommendLevel);

                ActivityDungeonSub sub = ActivityDungeonDataManager.GetInstance().GetSubByDungeonID(mDungeonID.dungeonIDWithOutDiff);
                if (null != sub)
                {
                    mChapterInfoCommon.SetOpenTime(sub.table.OpenTime);
                }
            }
        }

        protected virtual void _updateDropInfo()
        {
            if (null != mChapterInfoDrops)
            {
                if (mDungeonTable.SubType == DungeonTable.eSubType.S_HELL || mDungeonTable.SubType == DungeonTable.eSubType.S_LIMIT_TIME_HELL || mDungeonTable.SubType == DungeonTable.eSubType.S_LIMIT_TIME__FREE_HELL)
                {
                    mChapterInfoDrops.SetDropList(mDungeonTable.HellDropItems,mDungeonTable.ID);
                }
                else
                {
                    mChapterInfoDrops.SetDropList(mDungeonTable.DropItems,mDungeonTable.ID);
                }
            }
        }

        protected virtual void _updatePassReward()
        {
            if (null != mChapterPassReward)
            {
                mChapterPassReward.SetExp(mDungeonTable.ExpReward);
                mChapterPassReward.SetGold(0);
            }
        }

        private List<int> _monsterList()
        {
            List<int> monsterList = new List<int>();

            var config = AssetLoader.instance.LoadRes(mDungeonTable.DungeonConfig, typeof(DDungeonData)).obj as DDungeonData;

            if (null != config)
            {
                for (int i = 0; i < config.areaconnectlist.Length; ++i)
                {
                    var node = config.areaconnectlist[i];
                    if (node.isboss)
                    {
                        var scenedata = AssetLoader.instance.LoadRes(node.sceneareapath, typeof(DSceneData)).obj as DSceneData;
                        if (scenedata != null)
                        {
                            var monlist = scenedata._monsterinfo;
                            for (int j = 0; j < monlist.Length; ++j)
                            {
                                var resid = monlist[j].resid;
                                var fkflag = true;
                                var fkfinalid = resid % 100 + resid / 10000 * 10000;

                                var fkunit = TableManager.instance.GetTableItem<UnitTable>(fkfinalid);
                                if (null != fkunit)
                                {
                                    if (fkunit.Type == UnitTable.eType.SKILL_MONSTER)
                                    {
                                        fkflag = false;
                                    }
                                }

                                if (fkflag)
                                {
                                    monsterList.Add(fkfinalid);
                                }
                            }
                        }
                    }
                }
            }

            config = null;

            monsterList.Sort((x, y) =>
            {
                return y - x;
            });

            var len = monsterList.Count;
            if (len > 5)
            {
                monsterList.RemoveRange(5, len - 5);
            }

            config = null;

            return monsterList;
        }

        protected virtual void _updateMonsterData()
        {
            if (null != mChapterMonsterInfo)
            {
                mChapterMonsterInfo.SetMonsterList(_monsterList());
            }
        }

        protected virtual void _updateScore()
        {
            if (null != mChapterScore)
            {
                mChapterScore.SetFightTime(TableManager.GetValueFromUnionCell(mDungeonTable.TimeSplitArg, mDungeonTable.TimeSplitArg.eValues.everyValues.Count - 1) * 1000);
                mChapterScore.SetHitCount(TableManager.GetValueFromUnionCell(mDungeonTable.HitSplitArg, mDungeonTable.HitSplitArg.eValues.everyValues.Count - 1));
                mChapterScore.SetRebornCount(TableManager.GetValueFromUnionCell(mDungeonTable.RebornSplitArg, mDungeonTable.RebornSplitArg.eValues.everyValues.Count - 1));
            }
        }

        protected virtual void _updateDiffculteInfo()
        {
            if (null != mChapterInfoDiffculte)
            {

                mChapterInfoDiffculte.SetTopDiffculte(ChapterUtility.GetDungeonTopHard(sDungeonID));


                DungeonID id = new DungeonID(mDungeonID.dungeonID);

                int topHard = ChapterUtility.GetDungeonTopHard(id.dungeonID);

                List<int> list = new List<int>();

                List<string> descList = new List<string>();

                for (int i = 0; i <= topHard; ++i)
                {
                    id.diffID = i;
                    var data = TableManager.instance.GetTableItem<DungeonTable>(id.dungeonID);
                    if (null != data)
                    {
                        list.Add(data.MinLevel);
                        descList.Add(data.HardDescription);
                    }
                    else 
                    {
                        list.Add(0);
                        descList.Add(string.Empty);
                    }
                }

                id.dungeonID = sDungeonID;

                var state = ChapterUtility.GetDungeonState(id.dungeonID);
                bool isLock = state == ComChapterDungeonUnit.eState.Locked;
                mChapterInfoDiffculte.SetLock(isLock);
                mChapterInfoDiffculte.SetDiffculte(id.diffID,id.dungeonID);
        
                mChapterInfoDiffculte.SetLevelLimite(list.ToArray());
                mChapterInfoDiffculte.SetLevelDescription(descList.ToArray());
                mChapterInfoDiffculte.SetDiffculteCallback(_commonDiffCallback);

                _onDiffChange(id.diffID);
            }
        }

        private void _commonDiffCallback(int idx)
        {
            mDungeonID.diffID = idx;
            int id = mDungeonID.dungeonID;

            _loadTableData();

            if (null != mChapterInfoDiffculte)
            {
                var state = ChapterUtility.GetDungeonState(id);
                bool isLock = state == ComChapterDungeonUnit.eState.Locked;
                mChapterInfoDiffculte.SetLock(isLock);
                mChapterInfoDiffculte.SetDiffculte(mChapterInfoDiffculte.GetDiffculte(), id);
            }

            _updateCommonInfo();
            _updateDropInfo();
            _updatePassReward();
            _updateScore();
            _updateMonsterData();

            _onDiffChange(idx);
        }

        protected virtual void _onDiffChange(int idx)
        {

        }

        /// <summary>
        /// 获得最大难度
        /// </summary>
        /// <param name="dungeonID"></param>
        /// <returns></returns>
        private int _getTopHard(int dungeonID)
        {
            var hardList = BattleDataManager.GetInstance().ChapterInfo.allHardInfo;
            var topHard = 0;

            mDungeonID.dungeonID = dungeonID;

            var idx = hardList.BinarySearch(new Battle.DungeonHardInfo { id = mDungeonID.dungeonIDWithOutPrestory });
            if (idx >= 0)
            {
                topHard = hardList[idx].unlockedHardType;
            }

            return topHard;
        }

        private T _getDungeonTable<T>(int id)
        {
            var dungeonItem = TableManager.instance.GetTableItem<T>(id);
            if (dungeonItem != null)
            {
                return dungeonItem;
            }

            return default(T);
        }

		private bool mIsSendMessage = false;

        protected IEnumerator _commonStart()
        {
            if (!mIsSendMessage)
            {
                int diff = 0;
                if (null != mChapterInfoDiffculte)
                {
                    diff = mChapterInfoDiffculte.GetDiffculte();
                }

                mDungeonID.dungeonID = sDungeonID;
                mDungeonID.diffID = diff;

                DungeonTable dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(sDungeonID);
                if (dungeonTable != null && dungeonTable.SubType == DungeonTable.eSubType.S_GUILD_DUNGEON)
                {
                    if (!GuildDataManager.GetInstance().IsGuildDungeonOpen(sDungeonID))
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guildDungeonNotOpenNow"));
                        yield break;
                    }
                }
                else
                {
                if (!ChapterUtility.GetDungeonCanEnter(mDungeonID.dungeonID))
                {
                    yield break;
                    }
                }

                // TODO 条件检查
                // TODO 绑点替换
                // 使用buff药
                //
                SceneDungeonStartReq req = new SceneDungeonStartReq
                {
                    dungeonId = (uint)(mDungeonID.dungeonID)
                };

                if (null != mChapterInfoDrugs)
                {
                    //&& CostItemManager.GetInstance().IsEnough2Cost(
                    List<CostItemManager.CostInfo> costs = ChapterBuffDrugManager.GetInstance().GetAllMarkedBuffDrugsCost(mDungeonID.dungeonID);
                    
                    {
                        req.buffDrugs = ChapterBuffDrugManager.GetInstance().GetAllMarkedBuffDrugsByDungeonID(mDungeonID.dungeonID).ToArray();
                        // 这里去提示玩家药水不足，需要使用绑点购买药水
                        // 玩家也可以选择不使用绑点购买药水
                        int index = -1;
                        for (int i = 0; i < req.buffDrugs.Length; i++)
                        {
                            if (req.buffDrugs[i] > 0 && ItemDataManager.GetInstance().GetOwnedItemCount((int)req.buffDrugs[i]) == 0)
                            {
                                index = i;
                                break;
                            }
                        }
                        if (index != -1)
                        {
                            int moneyCost = 0;
                            for (int i = 0; i < costs.Count; i++)
                            {
                                moneyCost += costs[i].nCount;
                            }
                            int ret = -1;
                            LoginToggleMsgBoxOKCancelFrame.TryShowMsgBox(LoginToggleMsgBoxOKCancelFrame.LoginToggleMsgType.EnterDungeonBuyDrug, TR.Value("enter_dungeon_buy_drug_tip", moneyCost),                                
                                () =>
                                {
                                    ret = 1;
                                },
                                () =>
                                {
                                    ret = 0;
                                },
                                TR.Value("enter_dungeon_buy_drug"),
                                TR.Value("enter_dungeon_not_buy_drug"),
                                true);
                            while (ret == -1)
                            {
                                yield return null;
                            }
                            if (ret == 0)
                            {
                                for (int i = 0; i < req.buffDrugs.Length; i++)
                                {
                                    if (req.buffDrugs[i] > 0 && ItemDataManager.GetInstance().GetOwnedItemCount((int)req.buffDrugs[i]) == 0)
                                    {
                                        ChapterBuffDrugManager.GetInstance().UnMarkBuffDrug2Use((int)req.buffDrugs[i]);
                                        req.buffDrugs[i] = 0;
                                    }
                                }
                            }
                        }
                        int result = -1;

                        // 这里去检查绑点是否足够，不够的话提示玩家用点券代替绑点
                        costs = ChapterBuffDrugManager.GetInstance().GetAllMarkedBuffDrugsCost(mDungeonID.dungeonID);
                        bool isEnough2Cost = CostItemManager.GetInstance().IsEnough2Cost(costs);

                        CostItemManager.GetInstance().TryCostMoneiesDefault(costs, 
                                ()=>{ result = 1;},
                                ()=>{ result = 0;});

                        while (isEnough2Cost && -1 == result)
                        {
                            yield return null;
                        }

                        if (result == 1)
                        {
                            req.buffDrugs = ChapterBuffDrugManager.GetInstance().GetAllMarkedBuffDrugsByDungeonID(mDungeonID.dungeonID).ToArray();
                            ChapterBuffDrugManager.GetInstance().ResetAllMarkedBuffDrugs();
                        }
                        else
                        {
                            yield break;
                        }
                    }
                }

                //req.isHell = (byte)((ChapterSelectFrame.sMode == eChapterMode.Hell) ? 1 : 0);

                var dungeonData = TableManager.instance.GetTableItem<ProtoTable.DungeonTable>((int)req.dungeonId);
                if (null != dungeonData)
                {
                    if (dungeonData.Type == ProtoTable.DungeonTable.eType.L_DEADTOWER)
                    {
                        var item = CountDataManager.GetInstance().GetCount("tower");
                    }
                }


                var msg = new MessageEvents();
                var res = new SceneDungeonStartRes();

				mIsSendMessage = true;
                //NetManager.instance.SendCommand(ServerType.GATE_SERVER, req);

                yield return (MessageUtility.Wait<SceneDungeonStartReq, SceneDungeonStartRes>(ServerType.GATE_SERVER, msg, req, res, false, 5));

                mIsSendMessage = false;
            }
        }

        private void _setStat(int id, List<int> buff)
        {
            DungeonID did = new DungeonID(id);


            GameStatisticManager.instance.DoStatLevelChoose(StatLevelChooseType.ENTER_LEVEL,
                    ChapterSelectFrame.sSceneID,
                    did.dungeonID,
                    did.diffID,
                    buff);
        }
    }
}
