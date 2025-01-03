using Protocol;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    public sealed class ChangeFashionActivity : LimitTimeCommonActivity
    {
        protected override string _GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/ChangeFashionActivity";
        }

        public override void Show(Transform root)
        {
            base.Show(root);
            var tempView = mView as ChangeFashionActivityView;
            if(tempView != null)
            {
                tempView.SetGoFashionCallBack(GoToFashionMergeFrame);
                tempView.SetLookUpCallBack(LookUpFashionMode);
            }
        }

        private void GoToFashionMergeFrame()
        {
            if(mDataModel == null)
            {
                return;
            }
            bool isPrePare = mDataModel.State == OpActivityState.OAS_PREPARE;
            if (isPrePare)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("activity_havenot_open_tips"));
            }
            else
            {
                FashionMergeNewFrame.OpenLinkFrame(string.Format("1|1|{0}|{1}|{2}", (int)FashionType.FT_NATIONALDAY, (int)FashionMergeManager.GetInstance().FashionPart, 0));
            }
        }

        private void LookUpFashionMode(int id)
        {
            _ShowAvartar(id);
        }
        void _ShowAvartar(int id)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<PlayerTryOnFrame>())
            {
                var tryOnFrame = ClientSystemManager.GetInstance().GetFrame(typeof(PlayerTryOnFrame)) as PlayerTryOnFrame;
                if (tryOnFrame != null)
                {
                    tryOnFrame.Reset(id);
                }
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<PlayerTryOnFrame>(FrameLayer.Middle, id);
            }
        }
    }
}