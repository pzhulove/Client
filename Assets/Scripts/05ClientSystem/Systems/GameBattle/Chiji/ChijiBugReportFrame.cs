using UnityEngine.UI;

namespace GameClient
{
    public class ChijiBugReportFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiBugReportFrame";
        }

        protected override void _OnOpenFrame()
        {
        }

        protected override void _OnCloseFrame()
        {
            _ClearData();
        }

        private void _ClearData()
        {
        }

        #region ExtraUIBind
        private Button mClose = null;
        private Button mBtReport = null;
        private Button mBtRecordReport = null;

        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mBtReport = mBind.GetCom<Button>("btReport");
            mBtReport.onClick.AddListener(_onBtReportButtonClick);
            mBtRecordReport = mBind.GetCom<Button>("btReportRecord");
            if(!mBtRecordReport.IsNull())
                mBtRecordReport.onClick.AddListener(_onBtReportRecordButtonClick);
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mBtReport.onClick.RemoveListener(_onBtReportButtonClick);
            mBtReport = null;
            if (!mBtRecordReport.IsNull())
                mBtRecordReport.onClick.RemoveListener(_onBtReportRecordButtonClick);
            mBtRecordReport = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }

        private void _onBtReportButtonClick()
        {
            SystemNotifyManager.BaseMsgBoxOkCancel("是否上传数据", () =>
            {
                ExceptionManager.GetInstance().SaveLog();
                ClientSystemManager.instance.OpenFrame<UploadingCompressFrame>();
            }, null, "确定", "取消");

            frameMgr.CloseFrame(this);
        }
        private void _onBtReportRecordButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<PKReporterFrame>(FrameLayer.Middle);
        }
        #endregion
    }
}
