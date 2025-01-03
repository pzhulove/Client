using Network;
using Protocol;
using UnityEngine;

namespace GameClient
{
    public sealed class LevelFightShowActivity : LimitTimeCommonActivity
    {
        private const int SHOW_RANK_MAX_COUNT = 100;

        public override void Init(uint activityId)
        {
            var data = ActivityDataManager.GetInstance().GetLimitTimeActivityData(activityId);
            if (data != null)
            {
                mDataModel = new LevelFightShowActivityDataModel(data, _GetItemPrefabPath(), null);
            }
        }

        public override void Show(Transform root)
        {
            base.Show(root);
            if (mView != null)
            {
                var view = mView as LevelFightActivityView;
                if (view != null)
                {
                    if (mDataModel != null && mDataModel.TaskDatas != null)
                        view.ShowResultText(mDataModel.TaskDatas.Count);
                    view.OnButtonClick = _OnGoToRank;
                    view.ShowPlayerName(false);
                    _UpdateRankList();
                }
            }
        }

        public override void UpdateData()
        {
            var data = ActivityDataManager.GetInstance().GetLimitTimeActivityData(mDataModel.Id);
            if (data != null)
            {
                mDataModel = new LevelFightShowActivityDataModel(data, _GetItemPrefabPath(), null);
                _UpdateRankList();
            }
        }

        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/LevelFightActivity";
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/LevelFightShowItem";
        }

        void _UpdateRankList()
        {
            //WorldSortListReq msg = new WorldSortListReq
            //{
            //    type = (byte)SortListType.SORTLIST_XINFUCHONGJISAI,
            //    num = SHOW_RANK_MAX_COUNT
            //};
            //NetManager netMgr = NetManager.Instance();
            //netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            //WaitNetMessageManager.GetInstance().Wait(WorldSortListRet.MsgID, msgRet =>
            //{
            //    if (msgRet == null)
            //    {
            //        return;
            //    }
            //    int pos = 0;
            //    BaseSortList arrRecods = SortListDecoder.Decode(msgRet.bytes, ref pos, msgRet.bytes.Length);
                
            //    if (arrRecods != null && arrRecods.selfEntry != null)
            //    {
            //        if (mDataModel is LevelFightShowActivityDataModel)
            //        {
            //            ((LevelFightShowActivityDataModel) mDataModel).UpdateRecords(arrRecods);
            //        }

            //        var view = mView as LevelFightActivityView;
            //        if (view != null)
            //        {
            //            mView.UpdateData(mDataModel);
            //            view.SetRank(arrRecods.selfEntry.ranking);
            //        }
            //    }
            //});
        }

        void _OnGoToRank()
        {
            if (!ClientSystemManager.GetInstance().IsFrameOpen<RanklistFrame>())
            {
                ClientSystemManager.GetInstance().OpenFrame<RanklistFrame>();
                ClientSystemManager.GetInstance().CloseFrame<LimitTimeActivityFrame>();
            }
        }
    }
}