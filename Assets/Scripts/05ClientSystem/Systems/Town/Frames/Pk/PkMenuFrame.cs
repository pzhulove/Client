using ProtoTable;

namespace GameClient
{
    public class PkMenuFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk/PkMenuFrame";
        }

        protected override void _OnOpenFrame()
        {
            InitInterface();
            BindUIEvent();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideEnd);
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
            UnBindUIEvent();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideStart);
        }

        void ClearData()
        {
        }

        void BindUIEvent()
        {
        }

        void UnBindUIEvent()
        {
        }

        [UIEventHandle("middle/Close")]
        void OnClose()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("middle/Scroll View/Viewport/Content/btMission")]
        void OnMission()
        {
            ClientSystemManager.GetInstance().OpenFrame<MissionFrameNew>(FrameLayer.Middle);
            OnClose();
        }

        [UIEventHandle("middle/Scroll View/Viewport/Content/btForge")]
        void OnForge()
        {
            if (!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Forge))
            {
                SystemNotifyManager.SysNotifyFloatingEffect("等级不足,功能尚未解锁");
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<SmithShopNewFrame>(FrameLayer.Middle);
            OnClose();
        }

        [UIEventHandle("middle/Scroll View/Viewport/Content/btMall")]
        void OnMall()
        {
            if (!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Mall))
            {
                SystemNotifyManager.SysNotifyFloatingEffect("等级不足,功能尚未解锁");
                return;
            }

            //ClientSystemManager.GetInstance().OpenFrame<MallFrame>(FrameLayer.Middle);
            ClientSystemManager.GetInstance().OpenFrame<MallNewFrame>(FrameLayer.Middle);
            OnClose();
        }

        [UIEventHandle("middle/Scroll View/Viewport/Content/btAuction")]
        void OnAuction()
        {
            if (!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Auction))
            {
                SystemNotifyManager.SysNotifyFloatingEffect("等级不足,功能尚未解锁");
                return;
            }

            //ClientSystemManager.GetInstance().OpenFrame<AuctionFrame>(FrameLayer.Middle);
            frameMgr.OpenFrame<AuctionNewFrame>(FrameLayer.Middle);
            OnClose();
        }

        [UIEventHandle("middle/Scroll View/Viewport/Content/btShop")]
        void OnShop()
        {
            if (!Utility.IsFunctionCanUnlock(FunctionUnLock.eFuncType.Shop))
            {
                SystemNotifyManager.SysNotifyFloatingEffect("等级不足,功能尚未解锁");
                return;
            }

            ShopFrame.OpenLinkFrame("5|0|1");
            OnClose();
        }

        [UIEventHandle("middle/Scroll View/Viewport/Content/btSkill")]
        void OnSkill()
        {
            SkillFrameParam frameParam = new SkillFrameParam();
            frameParam.frameType = SkillFrameType.Normal;
            frameParam.tabTypeIndex = SkillFrameTabType.PVP;

            ClientSystemManager.GetInstance().OpenFrame<SkillFrame>(FrameLayer.Middle, frameParam);

            OnClose();
        }

        [UIEventHandle("middle/Scroll View/Viewport/Content/btBag")]
        void OnBag()
        {
            ClientSystemManager.GetInstance().OpenFrame<PackageNewFrame>(FrameLayer.Middle);
            OnClose();
        }

        [UIEventHandle("middle/Scroll View/Viewport/Content/btSetting")]
        void OnSetting()
        {
            ClientSystemManager.GetInstance().OpenFrame<SettingFrame>(FrameLayer.Middle);
            OnClose();
        }

        [UIEventHandle("middle/Scroll View/Viewport/Content/btAchievement")]
        void OnAchievement()
        {
            ClientSystemManager.GetInstance().OpenFrame<ActiveGroupMainFrame>(FrameLayer.Middle);
            OnClose();
        }

        void InitInterface()
        {

        }
    }
}
