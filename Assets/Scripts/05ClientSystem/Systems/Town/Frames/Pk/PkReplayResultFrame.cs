using DG.Tweening;
using Protocol;
using Network;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GameClient
{
	class PkReplayResultFrame : ClientFrame
    {

		#region ExtraUIBind
		private Text mTxtWin = null;
		private Text mTxtLose = null;
		private Button mBtnWatchAgain = null;
		private Button mBtnReturn = null;

		protected override void _bindExUI()
		{
			//mTxtWin = mBind.GetCom<Text>("txtWin");
			//mTxtLose = mBind.GetCom<Text>("txtLose");
			mBtnWatchAgain = mBind.GetCom<Button>("btnWatchAgain");
			mBtnWatchAgain.onClick.AddListener(_onBtnWatchAgainButtonClick);
			mBtnReturn = mBind.GetCom<Button>("btnReturn");
			mBtnReturn.onClick.AddListener(_onBtnReturnButtonClick);
		}

		protected override void _unbindExUI()
		{
			mTxtWin = null;
			mTxtLose = null;
			mBtnWatchAgain.onClick.RemoveListener(_onBtnWatchAgainButtonClick);
			mBtnWatchAgain = null;
			mBtnReturn.onClick.RemoveListener(_onBtnReturnButtonClick);
			mBtnReturn = null;
		}
		#endregion   

		#region Callback
		private void _onBtnWatchAgainButtonClick()
		{
			/* put your code in here */
			Close();
			//ReplayServer.GetInstance().StartReplay();
		}
		private void _onBtnReturnButtonClick()
		{
			/* put your code in here */
			ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
		}
		#endregion

        public override string GetPrefabPath()
        {
			return "UIFlatten/Prefabs/Pk/PKReplayResult";
        }

        protected override void _OnOpenFrame()
        {

        }

        protected override void _OnCloseFrame()
        {

        }
			

    }
}
