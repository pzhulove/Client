using System;
using Protocol;
using Network;
using ProtoTable;
using System.Collections.Generic;


namespace GameClient
{
    public class ChallengeDataManager : DataManager<ChallengeDataManager>
    {

        public int BindDeepTicket = 200000002;
        public int DeepTicket = 200000004;
        public int BindAncientTicket = 200000001;
        public int AncientTicket = 200000003;
        public int BindTeamTicket = 330000203;      //混沌凭证
        public int TeamTicket = 330000202;          //混沌预兆

        //对应地图城镇表，打开对应的地图
        public readonly int DeepMapId = 6028;      //深渊地图ID
        public readonly int AncientMapId = 6029;   //远古地图ID

        public readonly int ChallengeOpenLevel = 30;         //挑战按钮开放等级

        public readonly int ChallengeChapterHelpId = 10000;         //深渊远古详情中对应的问号ID

        public readonly int ChallengeWeekHellEntryDungeonId = 800000;   //深渊地下城入口

        private List<ChallengeDropDetailType> _dropDetailTypeList;

        private List<ChallengeDungeonRewardDataModel> _challengeDungeonRewardDataModelList;

        public override void Initialize()
        {
            BindNetEvents();
        }

        private void BindNetEvents()
        {
        }

        public override void Clear()
        {
            ClearData();
            UnBindNetEvents();
        }

        private void UnBindNetEvents()
        {
        }

        public void ClearData()
        {
            _dropDetailTypeList = null;
            _challengeDungeonRewardDataModelList = null;
        }

        public ChallengeDungeonRewardDataModel GetChallengeDungeonRewardDataByDungeonId(int dungeonId)
        {

            //ChallengeDungeonRewardDataModel dataModel = new ChallengeDungeonRewardDataModel();
            //dataModel.DungeonId = dungeonId;

            //dataModel.TotalNumber = 3;
            //dataModel.ChallengeNumber = UnityEngine.Random.Range(1, 4);
            //dataModel.AwardItemDataList = new List<AwardItemData>();
            //int number = UnityEngine.Random.Range(1, 5);
            //Logger.LogErrorFormat("RewardData number is {0}", number);
            //for (var i = 0; i < number; i++)
            //{
            //    var awardItemData = new AwardItemData();
            //    awardItemData.ID = 154413001;
            //    awardItemData.Num = i + 100;
            //    dataModel.AwardItemDataList.Add(awardItemData);
            //}

            //return dataModel;

            if (_challengeDungeonRewardDataModelList == null || _challengeDungeonRewardDataModelList.Count <= 0)
                return null;

            for (var i = 0; i < _challengeDungeonRewardDataModelList.Count; i++)
            {
                var rewardDataModel = _challengeDungeonRewardDataModelList[i];
                if (rewardDataModel != null && rewardDataModel.DungeonId == dungeonId)
                    return rewardDataModel;
            }

            return null;
        }

        public List<ChallengeDropDetailType> GetDropDetailTypeList()
        {
            if (_dropDetailTypeList == null)
            {
                _dropDetailTypeList = new List<ChallengeDropDetailType>
                {
                    ChallengeDropDetailType.RecommendItem,
                    ChallengeDropDetailType.BestItem,
                    ChallengeDropDetailType.OtherDropItem
                };
            }

            return _dropDetailTypeList;
        }
    }
}
