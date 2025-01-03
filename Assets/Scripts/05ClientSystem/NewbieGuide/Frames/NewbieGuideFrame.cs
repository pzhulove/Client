using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ProtoTable;
using System;

using DG.Tweening;
using Protocol;
using Network;

namespace GameClient
{
    public class NewbieGuideComic1 : NewbieGuideFinalShow
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Comic/Comic1";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            InitNewbieGuideState();
        }

        private void InitNewbieGuideState()
        {
            RoleInfo roleInfo = ClientApplication.playerinfo.GetSelectRoleBaseInfoByLogin();
            if (roleInfo.playerLabelInfo.noviceGuideChooseFlag == (int)NoviceGuideChooseFlag.NGCF_POPUP)
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("newbieguide_jump_tips"), () =>
                {
                    NewbieGuideDataManager.GetInstance().SetRoleNewbieguideState((int)roleInfo.roleId, NoviceGuideChooseFlag.NGCF_PASS);
                    SceneNoviceGuideChooseReq req = new SceneNoviceGuideChooseReq();
                    req.roleId = roleInfo.roleId;
                    req.chooseFlag = (int)NoviceGuideChooseFlag.NGCF_PASS;
                    NetManager netMgr = NetManager.Instance();
                    netMgr.SendCommand(ServerType.GATE_SERVER, req);
                    ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
                }, () =>
                {
                    NewbieGuideDataManager.GetInstance().SetRoleNewbieguideState((int)roleInfo.roleId, NoviceGuideChooseFlag.NGCF_NOT_PASS);
                    SceneNoviceGuideChooseReq req = new SceneNoviceGuideChooseReq();
                    req.chooseFlag = (int)NoviceGuideChooseFlag.NGCF_NOT_PASS;
                    req.roleId = roleInfo.roleId;
                    NetManager netMgr = NetManager.Instance();
                    netMgr.SendCommand(ServerType.GATE_SERVER, req);
                });
            }
        }
    }

    public class NewbieGuideComic2 : NewbieGuideFinalShow
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Comic/Comic2";
        }
    }

    public class ChapterChangeComic : NewbieGuideFinalShow
    {
        public override string GetPrefabPath()
        {
            return userData as string;
        }
    }

    public class NewbieGuideFinalShow : ClientFrame
    {
        enum eState
        {
            None,
            Playing,
            Stop,
        }

        eState mState = eState.None;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/NewbieGuide/GuideFinish";
        }

        private float _finalTime()
        {
            DOTweenAnimation[] alls = frame.GetComponentsInChildren<DOTweenAnimation>(true);

            float maxTime = 0;

            for (int i = 0; i < alls.Length; ++i)
            {
                DOTweenAnimation unit = alls[i];

                float time = unit.delay + unit.duration * (unit.loops + 1);
                maxTime = Mathf.Max(maxTime, time);
            }

            return maxTime;
        }

        protected float mLeftTime = 0.0f;

        protected override void _OnOpenFrame()
        {
            mLeftTime = _finalTime();
            mState = eState.Playing;
            _loadButton();
        }

        protected override void _OnCloseFrame()
        {
            _unloadButton();
        }

        private void _loadButton()
        {
            if (null != mBind)
            {
                Button button = mBind.GetCom<Button>("button");
                if(button == null)
                    return;
                button.onClick.AddListener(()=>
                {
                    mState = eState.Stop;
                    ClientSystemManager.instance.CloseFrame(this);
                });
            }

        }

        private void _unloadButton()
        {

            if (null != mBind)
            {
                Button button = mBind.GetCom<Button>("button");
                if(button != null)
                {
                    button.onClick.RemoveAllListeners();
                }
            }
        }


        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float time)
        {
            if (mState == eState.Playing)
            {
                mLeftTime -= time;
                if (mLeftTime < 0)
                {
                    ClientSystemManager.instance.CloseFrame(this);
                    mState = eState.Stop;
                }
            }
        }
    }

    public class NewbieGuideImageTips : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/NewbieGuide/NewbieGuideImageTips";
        }

        protected override void _OnOpenFrame()
        {
            if (null != mBind)
            {
                Button button = mBind.GetCom<Button>("button");
                if (null != button)
                {
                    button.onClick.AddListener(()=>
                    {
                        ClientSystemManager.instance.CloseFrame(this);
                    });
                }
            }
        }

        protected override void _OnCloseFrame()
        {
            if (null != mBind)
            {
                Button button = mBind.GetCom<Button>("button");
                if (null != button)
                {
                    button.onClick.RemoveAllListeners();
                }
            }
        }


        public void SetSprite(string path)
        {
            if (null != mBind)
            {
                Image  img = mBind.GetCom<Image>("image");

                //Sprite sp  = AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;

                //if (null != sp)
                //{
                //    img.sprite = sp;
                //    img.SetNativeSize();
                //}
                ETCImageLoader.LoadSprite(ref img, path);
                if(null != img.sprite)
                {
                    img.SetNativeSize();
                }
            }
        }
    }

    public class NewbieGuideNextRoom : ClientFrame
    {        
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/ArrowRightGoTips";
        } 
    }


	public class NewbieGuideBattleTipsFrame : ClientFrame
	{
		public override string GetPrefabPath()
		{
			return "UIFlatten/Prefabs/NewbieGuide/BattleTip";
		} 

#region ExtraUIBind
		private Text mText = null;
		private int delayCallTime;

		protected override void _bindExUI()
		{
			mText = mBind.GetCom<Text>("Text");
			delayCallTime = Convert.ToInt32(mBind.GetPrefabPath("delayCloseTime"));
		}

		protected override void _unbindExUI()
		{
			mText = null;
		}
#endregion

		public void SetTipsText(string text)
		{
			if(mText != null)
			{
				mText.text = text;
			}
		}

		protected override void _OnOpenFrame()
		{
			ClientSystemManager.GetInstance().delayCaller.DelayCall(delayCallTime, ()=>{
				Close();
			});
		}

		public void Close()
		{
			ClientSystemManager.GetInstance().CloseFrame(this);
		}
	}

    public class NewbieGuidTipsFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            bool bLeft = false;
            if( userData != null )
            {
                bLeft = (bool)userData;
            }
            return bLeft ? "UIFlatten/Prefabs/NewbieGuide/NewbieGuidTip2" : "UIFlatten/Prefabs/NewbieGuide/NewbieGuidTip";
        } 

        #region ExtraUIBind
            private Text mText = null;

            protected override void _bindExUI()
            {
                mText = mBind.GetCom<Text>("Text");
            }

            protected override void _unbindExUI()
            {
                mText = null;
            }
        #endregion
        ComShowText showCom;
        protected override void _OnOpenFrame()
        {
            showCom = frame.AddComponent<ComShowText>();
        }

        protected override void _OnCloseFrame()
        {
            showCom = null;
        }
        public void SetTipsText(string text)
        {
            if(mText != null)
            {
                mText.text = text;
            }
        }

        public void SetTipsTextEx(string text,float speed,float delay)
        {
            if(mText != null)
            {
                //mText.text = text;
                showCom.Begin(mText,text,speed,delay,this);
            }
        }
    }
    public class NewbieGuideUseSkillFrame : ClientFrame
    {

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/NewbieGuide/NewbieGuideUseSkill";
        }

        protected override void _OnCloseFrame()
        {
            if (mBind != null)
            {
                Button button = mBind.GetCom<Button>("button");
                button.onClick.RemoveAllListeners();
            }

            base._OnCloseFrame();
        }

        public void SetSkill(int skillID, Vector3 pos, Vector2 sizeDelta)
        {
            if (mBind != null)
            {
                RectTransform root   = mBind.GetCom<RectTransform>("skillroot");
                Button        button = mBind.GetCom<Button>("button");
                Image         image  = mBind.GetCom<Image>("image");

                SkillTable item = TableManager.instance.GetTableItem<SkillTable>(skillID);
                if (null != item)
                {
                    // image.sprite = AssetLoader.instance.LoadRes(item.Icon, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref image, item.Icon);
                }

                root.sizeDelta          = new Vector2(sizeDelta.x, sizeDelta.y);
                root.transform.position = pos;

                button.onClick.AddListener(()=>
                {
                    ClientSystemManager.instance.CloseFrame(this);
                });
            }
        }
    }

    public class NewbieGuideMoveTipsFrame : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/NewbieGuide/NewbieGuideMoveTips";
        }

        [UIObject("bg/ETCJoystick")]
        GameObject joystick;

        [UIObject("bg/Btn_Attack")]
        GameObject attackButton;

        public void SetShow(bool bShowJoyStick,bool bShowSkillButton)
        {
            joystick.CustomActive(bShowJoyStick);
            attackButton.CustomActive(bShowSkillButton);
        }

        protected override void _OnOpenFrame()
        {
            var actor = (ActorOccupation)PlayerBaseData.GetInstance().JobTableID;
            var jobitem = TableManager.instance.GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);

            if (null != jobitem)
            {
                _setSkill(jobitem.NormalAttackID);
            }
        }

        private void _setSkill(int skill)
        {
            if (null != mBind)
            {
                SkillTable item = TableManager.instance.GetTableItem<SkillTable>(skill);

                if (null != item)
                {
                    Image icon  = mBind.GetCom<Image>("image");

                    // icon.sprite = AssetLoader.instance.LoadRes(item.Icon, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref icon, item.Icon);
                }
            }
        }
    }

    public class NewbieGuideMsgTipsFrame : ClientFrame
    {
        [UIControl("Content/Text", typeof(Text))]
        private Text mFileName;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/NewbieGuide/NewbieGuideMsgTips";
        }

        public void SetMessage(string msg)
        {
            mFileName.text = msg;
        }
    }

    public class NewbieGuideNewSkillTipsFrame : ClientFrame
    {
        [UIControl("Skill{0}/FgImage", typeof(Image), 1)]
        private Image[] mFiledNameList = new Image[2];

        [UIControl("Skill{0}/SkillName", typeof(Text), 1)]
        private Text[] mTexts = new Text[2];

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/NewbieGuide/NewbieGuideAddSkill2";
        }

        public void SetSkill(int[] skill)
        {
            for (int i = 0; i < skill.Length; ++i)
            {
                var item = TableManager.instance.GetTableItem<SkillTable>(skill[i]);
                if (null != item)
                {
                    // mFiledNameList[i].sprite = AssetLoader.instance.LoadRes(item.Icon, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref mFiledNameList[i], item.Icon);
                    mTexts[i].text = item.Name;
                }
            }
        }
    }

    public class NewbieGuideFinalSkillTipsFrame : ClientFrame
    {
        [UIControl("Skill/FgImage", typeof(Image))]
        private Image mIcon;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/NewbieGuide/NewbieGuideUseSkill";
        }

        [UIEventHandle("Skill")]
        private void _onClose()
        {
            frameMgr.CloseFrame(this);
        }

        public void SetSkill(int skill)
        {
            SkillTable item = TableManager.instance.GetTableItem<SkillTable>(skill);
            if (null != item)
            {
                // mIcon.sprite = AssetLoader.instance.LoadRes(item.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref mIcon, item.Icon);
            }
        }
    }
}
