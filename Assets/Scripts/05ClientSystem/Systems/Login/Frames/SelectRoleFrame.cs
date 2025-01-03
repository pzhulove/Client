using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using DG.Tweening;
using Protocol;
using Network;

namespace GameClient
{
    public class SelectRoleFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SelecteRoleNew/SelectRoleFrame";
        }

        private const int kMaxRoleCount = 3;
        private const string kRoleUnitPath = "UIFlatten/Prefabs/SelectRole/SelectRoleSlot";

        #region ExtraUIBind
        private Button mClosebutton = null;
        private Button mStartbutton = null;
        private Text mStarttext = null;
        private Button mBackbutton = null;
        private Button mCreatebutton = null;
        private GameObject mRoleroot = null;
        private ToggleGroup mToggleroot = null;

        private Button mDeletebutton = null;
        private Button mRoleRecovery = null;
        private GameObject mBookingActivities = null;

        private GameObject mAdventureTeamInfoRoot = null;
        private ComAdventureTeamBriefInfo mAdventureTeamInfo = null;
        //private Text mCurrentRoleName;
        //private Text mCurrentRoleLV;
        //private Text mCurrentRoleJob;

        // [UIControl("SelectArray/Slot{0}",typeof(Toggle),1)]
        //private Toggle[] mRoleSolt = new Toggle[kMaxRoleCount];

        //private ComRoleSlot[] mComRoleSlot  = new ComRoleSlot[kMaxRoleCount];

        //UIRoot/UI2DRoot/Top/SelectRoleFrameEx2/SelectArray/Slot3/Bg/toph1/level


        //UIRoot/UI2DRoot/Top/SelectRoleFrameEx2/SelectArray/Slot1/Bg/iconroot
        protected sealed override void _bindExUI()
        {
            mRoleRecovery = mBind.GetCom<Button>("rolerecovery");
            //mClosebutton = mBind.GetCom<Button>("closebutton");
            mStartbutton = mBind.GetCom<Button>("startbutton");
            //mStarttext = mBind.GetCom<Text>("starttext");
            mBackbutton = mBind.GetCom<Button>("backbutton");

            mCreatebutton = mBind.GetCom<Button>("createbutton");
            //mRoleroot = mBind.GetGameObject("roleroot");
            //mCurrentRoleName = mBind.GetCom<Text>("CurrentName");
            //mCurrentRoleLV = mBind.GetCom<Text>("CurrentLv");
            //mCurrentRoleJob = mBind.GetCom<Text>("CurrentJob");
            mDeletebutton = mBind.GetCom<Button>("deleteRole");
            mBookingActivities = mBind.GetGameObject("BookingActivities");
            //mToggleroot = mBind.GetCom<ToggleGroup>("toggleroot");

            mAdventureTeamInfoRoot = mBind.GetGameObject("AdventureTeamInfoRoot");
            mAdventureTeamInfo = mBind.GetCom<ComAdventureTeamBriefInfo>("AdventureTeamInfo");
        }

        protected sealed override void _unbindExUI()
        {
            mRoleRecovery = null;
            mClosebutton = null;
            mStartbutton = null;
            //mStarttext = null;
            mCreatebutton = null;
            mDeletebutton = null;
            mBookingActivities = null;
            //mRoleroot = null;
            //mToggleroot = null;
            //mCurrentRoleName = null;
            //mCurrentRoleLV = null;

            mAdventureTeamInfoRoot = null;
            mAdventureTeamInfo = null;
        }
        #endregion

        protected GeAvatarRendererEx m_kAvatarRender;
        protected RoleInfo m_kCurrentRoleInfo;

        RoleFunctionBinder m_kRoleFunctionBinder = new RoleFunctionBinder();
        IClientFrame m_kDeleteRoleConfirmFrame = null;
        protected sealed override void _OnOpenFrame()
        {
            mBookingActivities.CustomActive(ClientApplication.playerinfo.GetHasApponintmentActiviti());
            ClientSystemLogin.mSwitchRole = false;

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleInfoUpdate, _OnUpdateRole);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleRecoveryUpdate, _OnRecoveryUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleDeleteOk, _OnDeleteRoleOk);

            _init();

            m_kRoleFunctionBinder.install(this, frame);
            ulong roleId = m_kRoleFunctionBinder.SetTheLatestLoginRoleAsDefault();
            m_kRoleFunctionBinder.refresh();
            m_kRoleFunctionBinder.OnRoleSelected(roleId);
            _TryOpenCreateActorFrame();
            _TryOpenOldPlayerFrame();

            _InitAdventureTeamInfo();

            //提示帐号转移
            BaseWebViewManager.GetInstance().ShowConvertAccountTips();
        }

        private void _init()
        {
            mStartbutton.onClick.AddListener(_onStartButton);
            //mClosebutton.onClick.AddListener(_onCloseButton);
            mRoleRecovery.onClick.AddListener(_onClickRecovery);
            mBackbutton.onClick.AddListener(_onCloseButton);
            mCreatebutton.onClick.AddListener(_onCreateButton);
            mDeletebutton.onClick.AddListener(_onDeleteButton);
        }

        private void _uninit()
        {
            mStartbutton.onClick.RemoveListener(_onStartButton);
            mRoleRecovery.onClick.RemoveListener(_onClickRecovery);
            //mClosebutton.onClick.RemoveListener(_onCloseButton);
            mBackbutton.onClick.RemoveListener(_onCloseButton);
            mCreatebutton.onClick.RemoveListener(_onCreateButton);
            mDeletebutton.onClick.RemoveListener(_onDeleteButton);
        }

        protected sealed override void _OnCloseFrame()
        {
            _uninit();

            if(m_kDeleteRoleConfirmFrame != null)
            {
                m_kDeleteRoleConfirmFrame.Close(true);
                m_kDeleteRoleConfirmFrame = null;
            }
            if(m_kRoleRecoveryFrame != null)
            {
                m_kRoleRecoveryFrame.Close(true);
                m_kRoleRecoveryFrame = null;
            }
            m_kRoleFunctionBinder.uninstall();

            // for (int i = 0; i < mRoleSolt.Length; ++i)
            //mComRoleSlot[i].createRoleBtn.onClick.RemoveListener(_onCreateButton);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleInfoUpdate, _OnUpdateRole);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleRecoveryUpdate, _OnRecoveryUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleDeleteOk, _OnDeleteRoleOk);

            AudioManager.instance.StopAll(AudioType.AudioStream);
            AudioManager.instance.StopAll(AudioType.AudioEffect);
            AudioManager.instance.StopAll(AudioType.AudioVoice);

            ClientSystemLogin curLogin = ClientSystemManager.instance.CurrentSystem as ClientSystemLogin;
            if (null != curLogin)
                curLogin.OnReturnToLogin();
				
            _ClearNewUnlockRoleField();
        }

        void _OnUpdateRole(UIEvent uiEvent)
        {
            m_kRoleFunctionBinder.refresh();
            _TryOpenCreateActorFrame();
        }

        void _TryOpenCreateActorFrame()
        {
            if(m_kRoleFunctionBinder.IsEmpty())
            {
                _onCreateButton();
            }
        }

        void _OnRecoveryUpdate(UIEvent uiEvent)
        {
            //m_kRoleFunctionBinder.refresh();
            ulong roleId = (ulong)uiEvent.Param1;
            m_kRoleFunctionBinder.MoveToPageByRoleId(roleId);
            m_kRoleFunctionBinder.refresh();
            m_kRoleFunctionBinder.OnRoleSelected(roleId);
        }

        void _OnDeleteRoleOk(UIEvent uiEvent)
        {
            ulong roleId = m_kRoleFunctionBinder.SetTheLatestLoginRoleAsDefault();
            m_kRoleFunctionBinder.refresh();
            m_kRoleFunctionBinder.OnRoleSelected(roleId);
        }

        public void OnDragSelectRole(float delta, bool selectRole = true)
        {
            //if(selectRole)
            //ComCreateRoleScene._DragRot(delta);
        }

        public void SetSelectedID(int idx)
        {
            //if (idx >= 0 && idx < mCacheToggle.Count)
            //{
            //    mCacheToggle[idx].isOn = true;
            //}
        }

        void OnRoleSelected(RoleInfo roleInfo)
        {
            if (m_kCurrentRoleInfo != null)
            {
                //ComCreateRoleScene.SelectRole(ClientApplication.playerinfo.curSelectedRoleIdx);
            }
        }

        void _onCloseButton()
        {
            ClientApplication.DisconnectGateServerAtLogin();
            frameMgr.CloseFrame(this);
        }

        int HistoryDeletedRoleCounts()
        {
            var roles = ClientApplication.playerinfo.roleinfo;
            int iCount = 0;
            for(int i = 0; i < roles.Length; ++i)
            {
                if(RecoveryRoleCachedObject.OnFilter(roles[i]))
                {
                    ++iCount;
                }
            }
            return iCount;
        }

        void _onDeleteButton()
        {
            if(HistoryDeletedRoleCounts() >= 3)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("delete_role_fulls_hint"));
                return;
            }

            //尝试删除未解锁的但是由于角色数超过标准值 而用到的栏位类型

            if (RoleObject.Selected != null && RoleObject.Selected.Value != null)
            {
                if ((RoleObject.Selected as RoleObject).GetCurrRoleFieldState() == RoleSelectFieldState.LockHasRole)
                {
                    string tr_select_role_delete_used_lock_field_tip = TR.Value("select_role_delete_used_lock_field_tip");
                    SystemNotifyManager.SysNotifyMsgBoxOkCancel(tr_select_role_delete_used_lock_field_tip, () => 
                    {
                        _TryOpenDeleteRoleConfirmFrame();
                    });
                }
                else
                {
                    _TryOpenDeleteRoleConfirmFrame();
                }
            }
        }

        void _TryOpenDeleteRoleConfirmFrame()
        {
            if (m_kDeleteRoleConfirmFrame != null)
            {
                m_kDeleteRoleConfirmFrame.Close(true);
                m_kDeleteRoleConfirmFrame = null;
            }
            m_kDeleteRoleConfirmFrame = ClientSystemManager.GetInstance().OpenFrame<DeleteRoleConfirmFrame>(Utility.FindChild(frame, "ChildRoot"), RoleObject.Selected.Value);
        }

		public sealed override bool IsNeedUpdate()
		{
			return true;
		}

		protected sealed override void _OnUpdate (float timeElapsed)
		{
			if (m_kRoleFunctionBinder != null)
				m_kRoleFunctionBinder.OnUpdate();
		}

        void _onCreateButton()
        {
            //if(RecoveryRoleCachedObject.HasOwnedRoles >= RecoveryRoleCachedObject.ms_max_roles_count)
            if (RecoveryRoleCachedObject.HasOwnedRoles >= RecoveryRoleCachedObject.EnabledRoleField)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("create_roles_full"));
                return;
            }
            OpenTargetFrame<CreateRoleFrame>();
        }

        private void _TryOpenOldPlayerFrame()
        {
            if (ClientApplication.veteranReturn > 0)
            {
                if (!ClientSystemManager.GetInstance().IsFrameOpen<OldPlayerFrame>())
                {
                    ClientSystemManager.GetInstance().OpenFrame<OldPlayerFrame>();

                }
            }
        }

        IClientFrame m_kRoleRecoveryFrame = null;
        private void _onClickRecovery()
        {
            if(m_kRoleRecoveryFrame != null)
            {
                m_kRoleRecoveryFrame.Close(true);
                m_kRoleRecoveryFrame = null;
            }
            m_kRoleRecoveryFrame = ClientSystemManager.GetInstance().OpenFrame<RoleRecoveryFrame>(Utility.FindChild(frame, "ChildRoot"));
        }

        private void _onStartButton()
        {
            int s = -1;

            if (RoleObject.Selected != null)
            {
                RoleInfo[] roleinfos = ClientApplication.playerinfo.roleinfo;

                for (int i = 0; i < roleinfos.Length; ++i)
                {
                    if (roleinfos[i].roleId == RoleObject.Selected.Value.roleInfo.roleId)
                    {
                        s = i;
                        break;
                    }
                }
            }

            if (s != -1)
            {
                ClientApplication.playerinfo.curSelectedRoleIdx = s;
                GameFrameWork.instance.StartCoroutine(ClientSystemLogin.StartEnterGame());
            }
            else
            {
                RoleInfo[] infos = ClientApplication.playerinfo.roleinfo;
                if (infos.Length <= 0)
                {
                    SystemNotifyManager.SysNotifyMsgBoxOK("请先创建角色!");
                }
                else
                {
                    SystemNotifyManager.SysNotifyMsgBoxOK("请先选择一个角色!");
                }
            }
        }

        #region Adventure Team
        void _InitAdventureTeamInfo()
        {
            bool isShow = false;
            if (ClientApplication.playerinfo != null && ClientApplication.playerinfo.adventureTeamInfo != null)
            {                
                isShow = !string.IsNullOrEmpty(ClientApplication.playerinfo.adventureTeamInfo.adventureTeamName);
                mAdventureTeamInfoRoot.CustomActive(isShow);
            }
            if (mAdventureTeamInfo != null && isShow)
            {
                mAdventureTeamInfo.RefreshView();
            }
        }

        void _ClearNewUnlockRoleField()
        {
            //登录游戏 打开选角界面 重置新解锁角色栏位数
            ClientApplication.playerinfo.newUnLockExtendRoleFieldNum = 0;
        }

        #endregion
    }
}
