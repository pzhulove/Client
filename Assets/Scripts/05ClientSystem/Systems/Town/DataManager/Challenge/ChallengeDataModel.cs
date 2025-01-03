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

    public enum EChapterSelectAnimateState
    {
        OnNone,
        OnSelectAnimate,            //正在选中
        OnBackAnimate,              //返回
    }


    //累计挑战关卡的数据
    public class ChallengeDungeonRewardDataModel
    {
        public int DungeonId;           //地下城ID
        public int ChallengeNumber;     //已经挑战的次数
        public int TotalNumber;         //总的次数
        public List<AwardItemData> AwardItemDataList;       //挑战达到总的次数时的奖励
    }

    public enum ChallengeDropDetailType
    {
        None = 0,
        RecommendItem = 1,      //推荐装备
        BestItem = 2,           //极品装备
        OtherDropItem = 3,      //其他掉落装备
    }

    //挑战地图的数据
    //ChallengeMapId 对应的深渊挑战 or 远古挑战
    //ChapterId 相应挑战下面默认选中的章节ID
    public class ChallengeMapParamDataModel
    {
        public DungeonModelTable.eType ModelType;
        public int BaseDungeonId;           //地图上对应地下城关卡的ID， 基础关卡
        public int DetailDungeonId;         //具体的地下城关卡ID， 对应着地下城的难度等级，默认为0；
    }

    //挑战模式的类型
    public enum ChallengeModelType
    {
        None = 0,
        Deep = 1,               //深渊
        Ancient = 2,            //远古
        Other   = 3,            //其他
        Number,
    }

    //周常深渊前置任务的状态
    public enum WeekHellPreTaskState
    {
        None = 0,
        UnReceived = 1,      //未领取
        IsProcessing = 2,   //进行中
        IsFinished = 3,     //已经完成
    }

    //挑战具体地下城的数据
    // BaseChapterId 地下城基础的ID ，普通对应的ID
    // selectedChapterId 具体的某个地下城,难度 1 2 3 4
    // chapterIdList 这个地图下的其他章节
    public class ChallengeChapterParamDataModel
    {
        public DungeonModelTable.eType ModelType;       //模式类型
        public int BaseChapterId;
        public int SelectedChapterId;
        public List<int> ChapterIdList;
    }

    //关卡难度等级的数据
    public class ChallengeChapterLevelDataModel
    {
        public int Index;       //索引，代表着关卡的难度： 0:普通，1：冒险，2：勇士，3：王者
        public int DungeonId;       //地下城ID
        public bool IsSelected;        //是否被选中

        public ChallengeChapterLevelDataModel(int index, 
            int dungeonId,
            bool isSelected)
        {
            Index = index;
            DungeonId = dungeonId;
            IsSelected = isSelected;
        }
    }



}