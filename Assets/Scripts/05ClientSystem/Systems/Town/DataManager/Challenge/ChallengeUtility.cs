using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{
    //挑战助手类
    public static class ChallengeUtility
    {

        public static void OnOpenChallengeDungeonRewardFrame(ChallengeDungeonRewardDataModel rewardDataModel)
        {
            OnCloseChallengeDungeonRewardFrame();

            ClientSystemManager.GetInstance()
                .OpenFrame<ChallengeDungeonRewardFrame>(FrameLayer.Middle, rewardDataModel);
        }

        public static void OnCloseChallengeDungeonRewardFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<ChallengeDungeonRewardFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<ChallengeDungeonRewardFrame>();
        }

        public static void OnOpenTeamListFrame()
        {
            //组队功能未解锁
            if (Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Team) == false)
            {
                var functionUnlockTable = TableManager.GetInstance()
                    .GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.Team);
                if (functionUnlockTable != null)
                    SystemNotifyManager.SystemNotify(functionUnlockTable.CommDescID);
                return;
            }
			//打开组队界面
            OnCloseTeamListFrame();

            TeamListFrame.TryOpenTeamListOrTeamMyFrame();
            //ClientSystemManager.GetInstance().OpenFrame<TeamListFrame>(FrameLayer.Middle);
        }

        public static void OnCloseTeamListFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<TeamListFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<TeamListFrame>();
        }

        public static void OnOpenChallengeDropDetailFrame(int dungeonId)
        {
            OnCloseChallengeDropDetailFrame();

            ClientSystemManager.GetInstance().OpenFrame<ChallengeDropDetailFrame>(FrameLayer.Middle, dungeonId);
        }

        public static void OnCloseChallengeDropDetailFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<ChallengeDropDetailFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<ChallengeDropDetailFrame>();
        }

        public static void OnCloseChallengeModelFrame()
        {
            //if(ClientSystemManager.GetInstance().IsFrameOpen<ChallengeModelFrame>() == true)
                //ClientSystemManager.GetInstance().CloseFrame<ChallengeModelFrame>();
        }
        
        public static void OnCloseChallengeMapFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<ChallengeMapFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<ChallengeMapFrame>();
        }

        public static DungeonModelTable GetChallengeDungeonModelTableByModelType(DungeonModelTable.eType modelType)
        {
            var dungeonModelTables = TableManager.GetInstance().GetTable<DungeonModelTable>();

            if (dungeonModelTables != null)
            {
                var iter = dungeonModelTables.GetEnumerator();
                while (iter.MoveNext())
                {
                    var dungeonModelTable = iter.Current.Value as DungeonModelTable;

                    if (dungeonModelTable != null)
                    {
                        if (dungeonModelTable.Type == modelType)
                            return dungeonModelTable;
                    }
                }
            }

            //如果没有找到，默认返回第一个
            var firstDungeonModelTable = TableManager.GetInstance().GetTableItem<DungeonModelTable>(1);
            return firstDungeonModelTable;
        }

        /// <summary>
        /// 根据地下城模式表里的模式类型获取他对应的地图城镇表id
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public static int GetChallengeDungeonMapIdByModelType(DungeonModelTable.eType modelType)
        {
            var dungeonTable = GetChallengeDungeonModelTableByModelType(modelType);

            if (dungeonTable != null)
                return dungeonTable.mapID;

            return 0;
        }

        public static int GetChallengeDungeonLockLevelByModelType(DungeonModelTable.eType modelType)
        {
            var dungeonTable = GetChallengeDungeonModelTableByModelType(modelType);

            if (dungeonTable != null)
                return dungeonTable.Level;

            return 0;
        }

        public static int adjustDungeonId(int dungeonId)
        {
            if (dungeonId == 8600000 || dungeonId == 8610000 || dungeonId == 8620000 || dungeonId == 8630000
                ||dungeonId == 8640000 || dungeonId == 8650000 || dungeonId == 8660000 || dungeonId == 8670000
                ||dungeonId == 8680000 || dungeonId == 8690000 || dungeonId == 8700000 || dungeonId == 8710000)
            {
                return 8720000;
            }
            return dungeonId;
        }

        public static bool isYunShangChangAn(int dungeonId)
        {
            if (dungeonId == 8600000 || dungeonId == 8610000 || dungeonId == 8620000 || dungeonId == 8630000
                ||dungeonId == 8640000 || dungeonId == 8650000 || dungeonId == 8660000 || dungeonId == 8670000
                ||dungeonId == 8680000 || dungeonId == 8690000 || dungeonId == 8700000 || dungeonId == 8710000
                ||dungeonId == 8720000)
            // if (dungeonId == 8720000)
            {
                return true;
            } 
            return false;
        }

        public static void OnOpenClientFrameStackCmd(int dungeonId, DungeonTable dungeonTable)
        {
            //如dungeonId 是 2100002。基础ID是2100000， 表示勇者难度

            dungeonId = adjustDungeonId(dungeonId);

            if (dungeonTable == null)
                return;

            //深渊，远古和周常深渊
            var modelType = DungeonModelTable.eType.DeepModel;
            if (dungeonTable.SubType == DungeonTable.eSubType.S_YUANGU)
            {
                if (isYunShangChangAn(dungeonId))
                {
                    modelType = DungeonModelTable.eType.YunShangChangAnModel;
                }
                else
                {
                    modelType = DungeonModelTable.eType.AncientModel;
                }
            }
            else if (dungeonTable.SubType == DungeonTable.eSubType.S_WEEK_HELL
                     || dungeonTable.SubType == DungeonTable.eSubType.S_WEEK_HELL_ENTRY
                     || dungeonTable.SubType == DungeonTable.eSubType.S_WEEK_HELL_PER)
            {
                modelType = DungeonModelTable.eType.WeekHellModel;
            }
            else if (dungeonTable.SubType == DungeonTable.eSubType.S_NANBUXIGU)
            {
                 modelType = DungeonModelTable.eType.ZhengzhanAntuenModel;
            }

            DungeonID baseDungeonID = new DungeonID(dungeonId);
            var baseDungeonIdWithDiff = baseDungeonID.dungeonIDWithOutDiff;

            var paramDataModel = new ChallengeMapParamDataModel()
            {
                ModelType = modelType,
                BaseDungeonId = baseDungeonIdWithDiff,
                DetailDungeonId = dungeonId,
            };

            //地图界面
            ClientSystemManager.instance.Push2FrameStack(new OpenClientFrameStackCmd(typeof(ChallengeMapFrame),
                paramDataModel));

            ////模式界面
            //ClientSystemManager.instance.Push2FrameStack(
            //    new OpenClientFrameStackCmd(typeof(ChallengeModelFrame)));
        }

        public static void OnOpenChallengeMapFrame(DungeonModelTable.eType modelType, 
            int baseDungeonId,
            int detailDungeonId = 0)
        {

            if (modelType == DungeonModelTable.eType.Type_None)
            {
                // 默认选中深渊
                // modelType = DungeonModelTable.eType.DeepModel;
                // 默认选中云上长安
                modelType = DungeonModelTable.eType.YunShangChangAnModel;
            }

            OnCloseChallengeMapFrame();
            OnCloseChallengeChapterFrame();

            var mapId = ChallengeUtility.GetChallengeDungeonMapIdByModelType(modelType);

            var paramDataModel = new ChallengeMapParamDataModel()
            {
                ModelType = modelType,
                BaseDungeonId = baseDungeonId,
                DetailDungeonId = detailDungeonId,
            };

            ClientSystemManager.GetInstance().OpenFrame<ChallengeMapFrame>(FrameLayer.Middle, paramDataModel);
        }

        public static void OnCloseChallengeChapterFrame()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<ChallengeChapterFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<ChallengeChapterFrame>();
        }

        public static void OnOpenChallengeChapterFrame(DungeonModelTable.eType modelType,int baseChapterId,int selectedChapterId, List<int> chapterIdList = null)
        {
            OnCloseChallengeChapterFrame();

            //周常活动和限时深渊不能活动滑动
            if (DungeonUtility.IsLimitTimeHellDungeon(baseChapterId) == true
                || DungeonUtility.IsWeekHellEntryDungeon(baseChapterId) == true)
                chapterIdList = null;

            var chapterParamDataModel = new ChallengeChapterParamDataModel
            {
                ModelType = modelType,
                BaseChapterId =  baseChapterId,
                SelectedChapterId = selectedChapterId,
                ChapterIdList = chapterIdList,
            };

            ClientSystemManager.GetInstance().OpenFrame<ChallengeChapterFrame>(FrameLayer.Middle, chapterParamDataModel);
        }

        public static string GetColorString(string color, string name)
        {
            return string.Format("<color={0}>{1}</color>", color, name);
        }

        //普通到王者的经验加成分别为0 - 50
        public static int GetExpRate(int levelId)
        {
            if (levelId == 0)
                return 0;
            else if (levelId == 1)
                return 10;
            else if (levelId == 2)
                return 20;
            else if (levelId == 3)
                return 50;

            return 0;
        }

        //普通到王者的掉落加成分别为0 - 20
        public static int GetDropRate(int levelId)
        {
            if (levelId == 0)
                return 0;
            else if (levelId == 1)
                return 5;
            else if (levelId == 2)
                return 10;
            else if (levelId == 3)
                return 20;

            return 0;
        }

        //周常深渊和活动中的堕落深渊，只有一个王者难度，并且只有一个掉落加成
        //王者掉落的加成是200
        public static int GetOnlyKingHardLevelDropRate()
        {
            return 200;
        }

        //得到难度关卡的名字
        public static string GetLevelName(int levelId)
        {
            if (levelId == 0)
            {
                return TR.Value("challenge_chapter_level_normal");
            }
            else if (levelId == 1)
            {
                return TR.Value("challenge_chapter_level_adventure");
            }
            else if (levelId == 2)
            {
                return TR.Value("challenge_chapter_level_warrior");
            }
            else if (levelId == 3)
            {
                return TR.Value("challenge_chapter_level_king");
            }

            return TR.Value("challenge_chapter_level_normal");
        }

        //得到前置难度关卡的名字
        public static string GetPreLevelName(int levelId)
        {
            if (levelId <= 1)
            {
                return TR.Value("challenge_chapter_level_normal");
            }
            else if (levelId == 2)
            {
                return TR.Value("challenge_chapter_level_adventure");
            }
            else if (levelId == 3)
            {
                return TR.Value("challenge_chapter_level_warrior");
            }

            return TR.Value("challenge_chapter_level_normal");
        }

        //关卡是否锁住
        public static bool IsDungeonLock(int dungeonId)
        {
            ComChapterDungeonUnit.eState dungeonState = ChapterUtility.GetDungeonState(dungeonId);

            if (dungeonState == ComChapterDungeonUnit.eState.Locked)
            {
                return true;
            }

            if (dungeonState == ComChapterDungeonUnit.eState.Unlock)
            {
                var dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonId);
                if (dungeonTable != null)
                {
                    if (PlayerBaseData.GetInstance().Level < dungeonTable.MinLevel)
                        return true;
                }
            }

            return false;
        }

        public static List<ActivityDungeonSub> GetDailyUnitActivitySubs(int dungeonId)
        {
            ActivityDungeonTab tab = ActivityDungeonDataManager.GetInstance().GetTabByDungeonIDDefaultFirst(
                ActivityDungeonTable.eActivityType.Daily,
                dungeonId);

            if (null != tab)
                return tab.subs;

            return null;
        }

        public static ActivityDungeonSub GetDungeonSubDataByDungeonId(int dungeonId,
            List<ActivityDungeonSub> activityDungeonSubList)
        {
            if (activityDungeonSubList == null || activityDungeonSubList.Count <= 0)
                return null;

            for (var i = 0; i < activityDungeonSubList.Count; i++)
            {
                var dungeonSub = activityDungeonSubList[i];
                if(dungeonSub == null)
                    continue;

                if (dungeonSub.dungeonId == dungeonId)
                    return dungeonSub;
            }

            return null;
        }

        public static void SortChapterIdListByLevel(List<int> chapterIdList)
        {
            if (chapterIdList == null || chapterIdList.Count <= 1)
                return;

            chapterIdList.Sort((x, y) =>
            {
                var dungeonTableX = TableManager.GetInstance().GetTableItem<DungeonTable>(x);
                if (dungeonTableX == null)
                    return -1;

                var dungeonTableY = TableManager.GetInstance().GetTableItem<DungeonTable>(y);
                if (dungeonTableY == null)
                    return 1;

                return dungeonTableX.Level.CompareTo(dungeonTableY.Level);
            });
        }

        public static void UpdateButtonState(Button button, UIGray gray, bool flag)
        {
            if (button != null)
                button.interactable = flag;

            if (gray != null)
                gray.enabled = !flag;
        }

        public static int FloatToInt(float f)
        {
            var i = 0;
            if (f > 0) //正数
                i = (int)(f * 10 + 5) / 10;
            else if (f < 0) //负数
                i = (int)(f * 10 - 5) / 10;
            else i = 0;

            return i;
        }

        public static string GetDropDetailTitleName(ChallengeDropDetailType dropDetailType)
        {
            switch (dropDetailType)
            {
                case ChallengeDropDetailType.RecommendItem:
                    return TR.Value("challenge_chapter_drop_recommend_item");
                case ChallengeDropDetailType.BestItem:
                    return TR.Value("challenge_chapter_drop_best_item");
                case ChallengeDropDetailType.OtherDropItem:
                    return TR.Value("challenge_chapter_drop_other_drop_item");
            }
            return TR.Value("challenge_chapter_drop_recommend_item");
        }

        public static bool IsItemValid(int itemId)
        {
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);

            if (itemTable == null)
                return false;

            return true;
        }

        //装备是否为推荐的装备（道具适用的职业是否为自身的职业);
        public static bool IsRecommendItemByProfession(int itemId, int jobTableId, int baseJobTableId)
        {
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);

            if (itemTable == null)
                return false;

            //只需要判断Occu字段，如果字段中存在0， 则全职业都显示
            if (itemTable.Occu.Count > 0)
            {
                for (var i = 0; i < itemTable.Occu.Count; i++)
                {
                    //如果职业字段中存在0， 则与职业无关，必然显示
                    if (itemTable.Occu[i] == 0)
                        return true;

                    //与职业ID相同，则显示
                    if (itemTable.Occu[i] == jobTableId || itemTable.Occu[i] == baseJobTableId) 
                        return true;
                }
            }

            return false;
        }

        public static int GetPlayerBaseJobTableId()
        {
            var jobTableId = PlayerBaseData.GetInstance().JobTableID;
            var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(jobTableId);
            if (jobTable != null && jobTable.JobType == 1)
            {
                return jobTable.prejob;
            }

            return jobTableId;
        }

        //显示周常深渊的前置任务没领取的提示界面
        public static void OnShowWeekHellPreTaskUnReceivedTip(int dungeonId)
        {
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = TR.Value("activity_week_hell_pre_task_unreceived"),
                IsShowNotify = false,
                LeftButtonText = TR.Value("activity_week_hell_pre_task_not_received"),
                RightButtonText = TR.Value("activity_week_hell_pre_task_yes_received"),
                OnRightButtonClickCallBack = () =>
                {
                    OnReceiveWeekHellPreTask(dungeonId);
                },
            };
            
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }

        public static void OnReceiveWeekHellPreTask(int dungeonId)
        {
            OnCloseChallengeMapFrame();

            var preTaskId = DungeonUtility.GetWeekHellPreTaskId(dungeonId);
            if (preTaskId <= 0)
            {
                Logger.LogErrorFormat("PreTaskId is not more than zero and weekHellDungeonId is {0}", dungeonId);
                return;
            }

            MissionManager.GetInstance().AutoTraceTask(preTaskId);
        }

        public static ActivityDungeonTable GetActivityDungeonTableByDungeonId(int dungeonId)
        {
            var activityDungeonTables = TableManager.GetInstance().GetTable<ActivityDungeonTable>();
            var iter = activityDungeonTables.GetEnumerator();
            while (iter.MoveNext())
            {
                var curActivityDungeonTable = iter.Current.Value as ActivityDungeonTable;
                if (curActivityDungeonTable != null && curActivityDungeonTable.DungeonID == dungeonId)
                    return curActivityDungeonTable;
            }

            return null;
        }

        //判断章节是否可以滑动：
        //周常深渊不可滑动，限时深渊不可滑动，非解锁的深渊不可滑动；其他情况均可
        public static bool IsChallengeChapterCanSlider(ActivityDungeonSub activityDungeonSub)
        {
            if (activityDungeonSub == null)
                return false;

            if (DungeonUtility.IsLimitTimeHellDungeon(activityDungeonSub.dungeonId) == true)
                return false;

            if (DungeonUtility.IsWeekHellEntryDungeon(activityDungeonSub.dungeonId) == true)
                return false;

            if (activityDungeonSub.state != eActivityDungeonState.Start)
                return false;

            return true;
        }

        //根据地下城模式的类型获得消耗品的列表
        public static List<int> GetDungeonModelCostDataList(DungeonModelTable.eType modelType)
        {
            List<int> costItemList = null;

            var dungeonModelTables = TableManager.GetInstance().GetTable<DungeonModelTable>();

            if (dungeonModelTables != null)
            {
                var iter = dungeonModelTables.GetEnumerator();
                while (iter.MoveNext())
                {
                    var dungeonModelTable = iter.Current.Value as DungeonModelTable;

                    if(dungeonModelTable == null)
                        continue;

                    if (modelType == dungeonModelTable.Type)
                    {
                        var costItemArray = dungeonModelTable.CostItem;
                        if (costItemArray != null)
                        {
                            costItemList = costItemArray.ToList();
                        }

                        break;
                    }
                }
            }

            return costItemList;
        }

        //根据地下城模式的类型获得精力值显示的标志
        public static bool GetDungeonModelShowSpriteFlag(DungeonModelTable.eType modelType)
        {
            var dungeonModelTables = TableManager.GetInstance().GetTable<DungeonModelTable>();

            if (dungeonModelTables != null)
            {
                var iter = dungeonModelTables.GetEnumerator();
                while (iter.MoveNext())
                {
                    var dungeonModelTable = iter.Current.Value as DungeonModelTable;

                    if (dungeonModelTable == null)
                        continue;

                    if (modelType == dungeonModelTable.Type)
                    {
                        if (dungeonModelTable.IsShowSpriteConsume == 1)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            return false;
        }


        //得到团本地下城的数据
        public static DungeonModelTable GetDungeonModelTableOfTeamDuplication()
        {
            var dungeonModelTables = TableManager.GetInstance().GetTable<DungeonModelTable>();

            if (dungeonModelTables != null)
            {
                var iter = dungeonModelTables.GetEnumerator();
                while (iter.MoveNext())
                {
                    var dungeonModelTable = iter.Current.Value as DungeonModelTable;

                    if (dungeonModelTable == null)
                        continue;

                    if (dungeonModelTable.Type == DungeonModelTable.eType.TeamDuplicationModel)
                    {
                        return dungeonModelTable;
                    }
                }
            }

            return null;
        }

    }
}