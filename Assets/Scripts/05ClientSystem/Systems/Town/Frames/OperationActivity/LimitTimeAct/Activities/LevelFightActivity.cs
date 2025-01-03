using Network;
using Protocol;
using UnityEngine;

namespace GameClient
{
    public sealed class LevelFightActivity : LimitTimeCommonActivity
    {
        private const int SHOW_RANK_MAX_COUNT = 100;

        public override void Show(Transform root)
        {
            base.Show(root);
            if (mView != null)
            {
                var view = mView as LevelFightActivityView;
                if (view != null)
                {
                    view.SetEndTime((int)mDataModel.EndTime);
                    view.OnButtonClick = _OnGoToRank;
                    view.ShowPlayerName(false);
                    //todo   这个说要去掉，暂时不管。

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
                    //        view.SetRank(arrRecods.selfEntry.ranking);
                    //    }
                    //});
                }
            }
        }

        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/LevelFightActivity";
        }

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/LevelFightItem";
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