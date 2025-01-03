using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using RenderHeads.Media.AVProVideo;

namespace GameClient
{
	public class ControlSelectFrame : ClientFrame {

		private bool leftPlay = true;

		#region ExtraUIBind
		private Toggle mSelect1 = null;
		private Toggle mSelect2 = null;
		private MediaPlayerCtrl mMovieCtrl1 = null;
		private MediaPlayerCtrl mMovieCtrl2 = null;
		private Button mBtnOK = null;


		protected override void _bindExUI()
		{
			mSelect1 = mBind.GetCom<Toggle>("select1");
			mSelect1.onValueChanged.AddListener(_onSelect1ToggleValueChange);
			mSelect2 = mBind.GetCom<Toggle>("select2");
			mSelect2.onValueChanged.AddListener(_onSelect2ToggleValueChange);
			mMovieCtrl1 = mBind.GetCom<MediaPlayerCtrl>("movieCtrl1");
			mMovieCtrl2 = mBind.GetCom<MediaPlayerCtrl>("MovieCtrl2");
			mBtnOK = mBind.GetCom<Button>("btnOK");
			mBtnOK.onClick.AddListener(_onBtnOKButtonClick);
		}

		protected override void _unbindExUI()
		{
			mSelect1.onValueChanged.RemoveListener(_onSelect1ToggleValueChange);
			mSelect1 = null;
			mSelect2.onValueChanged.RemoveListener(_onSelect2ToggleValueChange);
			mSelect2 = null;
			mMovieCtrl1 = null;
			mMovieCtrl2 = null;
		}
		#endregion   

		#region Callback
		private void _onSelect1ToggleValueChange(bool changed)
		{
			/* put your code in here */
			if (!changed)
				return;

			leftPlay = true;

			if (mMovieCtrl2.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY)
			{
				mMovieCtrl2.Stop();
			}
				
			
				
			if (mMovieCtrl2.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY)
			{
				mMovieCtrl1.Stop();
				mMovieCtrl1.Play();
			}

		}
		private void _onSelect2ToggleValueChange(bool changed)
		{
			/* put your code in here */
			if (!changed)
				return;

			leftPlay = false;

			if (mMovieCtrl2.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY)
			{
				mMovieCtrl2.Stop();
				mMovieCtrl2.Play();
			}

			if (mMovieCtrl1.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY)
			{
				mMovieCtrl1.Stop();
			}


		}

		private void _onBtnOKButtonClick()
		{
			/* put your code in here */
			//mMovieCtrl1.CloseVideo();
			//mMovieCtrl2.CloseVideo();

			CloseVideo(mMovieCtrl1);
			CloseVideo(mMovieCtrl2);

			bool doublePress = mSelect2.isOn;
			PlayerLocalSetting.SetValue(SettingFrame.KEY_DOUBLE_PRESS, doublePress);
			Global.Settings.hasDoubleRun = doublePress;

			Close(true);
		}
		#endregion


		public override string GetPrefabPath()
		{
			return "UIFlatten/Prefabs/NewbieGuide/ControlSelectFrame";
		}

		protected override void _OnOpenFrame()
		{
			bool doublePress = false;
			if (PlayerLocalSetting.GetValue(SettingFrame.KEY_DOUBLE_PRESS) != null)
			{
				doublePress = (bool)PlayerLocalSetting.GetValue(SettingFrame.KEY_DOUBLE_PRESS);
			}

			if (doublePress)
			{
				mSelect2.isOn = true;
				leftPlay = false;
			}
			else
				mSelect1.isOn = true;
			

			InitVideo(mMovieCtrl1,  true);
			InitVideo(mMovieCtrl2, false);
		}

		public void InitVideo(MediaPlayerCtrl mpc, bool isLeft)
		{
			
			if (mpc != null)
			{
				bool left = isLeft;
				var rowImage = mpc.m_TargetMaterial[0].GetComponent<RawImage>();
				if (rowImage != null)
				{
					rowImage.enabled = false;
				}
				mpc.OnReady += ()=>{
					rowImage.enabled = true;

					if (leftPlay && left || !leftPlay && !left)
						mpc.Play();
					else
					{
						mpc.Play();
						ClientSystemManager.instance.delayCaller.DelayCall(10, ()=>{
							mpc.Pause();	
							//Logger.LogErrorFormat("mpc pause left:{0}", isLeft);
						});
					}
						
				};
			}
		}

		public void CloseVideo(MediaPlayerCtrl mpc)
		{
            if (mpc != null)
            {
                if (mpc.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY)
                {
                    mpc.Stop();
                    mpc.UnLoad();
                }
            }
		}
	}
}

