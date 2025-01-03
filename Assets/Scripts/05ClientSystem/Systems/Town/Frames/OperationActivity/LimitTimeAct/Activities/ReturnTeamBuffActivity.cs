using Protocol;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    public sealed class ReturnTeamBuffActivity : LimitTimeCommonActivity
    {
        protected override void _OnItemClick(int taskId, int param,ulong param2)
        {
            //todo 跳转到新的界面
            if(param == 0)
            {
                ClientSystemManager.GetInstance().OpenFrame<ChallengeMapFrame>();
            }
            else
            {
                TeamListFrame.TryOpenTeamListOrTeamMyFrame();
                //ClientSystemManager.instance.OpenFrame<TeamListFrame>(FrameLayer.Middle);
            }
        }
        

        protected override string _GetItemPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/ReturnTeamBuffItem";
        }
    }
}