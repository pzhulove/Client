using UnityEngine;
using UnityEngine.UI;
using Protocol;
using System;
using System.Collections.Generic;
using Network;

namespace GameClient
{
    class RoleData
    {
        public RoleInfo roleInfo;
    }

    class RoleFieldData
    {
        public RoleInfo roleInfo;
        public RoleSelectFieldState fieldState = RoleSelectFieldState.Default;
    }

    public enum RoleSelectFieldState
    {
        None                = 0,
        Default             = 1,
        BaseHasRole         = 2,
        NewUnlockHasRole    = 3,
        NewUnlockNoRole     = 4,
        LockHasRole         = 5,
        LockNoRole          = 6,
    }

    class RoleObject : CachedSelectedObject<RoleData,RoleObject>
    {		
        static protected readonly string[] m_ActionTable = new string[3] { "Anim_Show_Idle", "Anim_Show_Idle_special01", "Anim_Show_Idle_special02" };
        private const string likePath = "UI/Image/NewPacked/Chuangjue.png:Role_Img_Xing01";
        private const string unLikePath = "UI/Image/NewPacked/Chuangjue.png:Role_Img_Xing02";
        Text Name;
        Text Lv;
        Text Job;
        GeAvatarRendererEx AvatarRender;
        GeAttach effectSelected;
		GeAttach effectSelected2;
        RawImage rawImage;
        GameObject SceneLightRoot = null;
		bool playQueueIdle = false;
		GameObject objImgSelect = null;
		GameObject objImgDisSelect = null;
        GameObject objBookingActivities = null;
        GameObject mOldPlayer;
        Button likeBtn;
        Image responseImg;
        ComSelectRoleField mRoleField = null;

        public override void Initialize()
        {
            Name = Utility.FindComponent<Text>(goLocal,"nameRoot/Name");
            Lv = Utility.FindComponent<Text>(goLocal, "Horizen/Lv");
            Job = Utility.FindComponent<Text>(goLocal, "Horizen/Job");
            AvatarRender = Utility.FindComponent<GeAvatarRendererEx>(goLocal, "AvatarRenderer");
            rawImage = Utility.FindComponent<RawImage>(goLocal, "AvatarRenderer");
			objImgSelect = Utility.FindThatChild("select", goLocal);
			objImgDisSelect = Utility.FindThatChild("disSelect", goLocal);
            objBookingActivities = Utility.FindGameObject(goLocal,"BookingActivities");
            mOldPlayer = Utility.FindGameObject(goLocal, "OldPlayer");
            likeBtn = Utility.FindComponent<Button>(goLocal, "like");
            responseImg = Utility.FindComponent<Image>(goLocal, "like/response");
            if (likeBtn != null)
            {
                likeBtn.CustomActive(false);

                likeBtn.onClick.RemoveAllListeners();
                likeBtn.onClick.AddListener(OnLikeBtnClick);
            }
                
            objImgSelect.CustomActive(false);
			objImgDisSelect.CustomActive(true);

            if (goLocal) { mRoleField = goLocal.GetComponent<ComSelectRoleField>(); }

            SetTxtColor(true);

			playQueueIdle = false;
        }

        public override void UnInitialize()
        {
            rawImage = null;
            effectSelected = null;
            if (AvatarRender != null)
            {
                AvatarRender.ClearAvatar();
                AvatarRender = null;
            }
            Job = null;
            Lv = null;
            Name = null;
			playQueueIdle = false;

            mRoleField = null;
        }

        private void OnLikeBtnClick()
        {
            if(Value.roleInfo != null)
            {
                GateRoleCollectionReq req = new GateRoleCollectionReq();
                req.roleId = Value.roleInfo.roleId;

                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);

                WaitNetMessageManager.GetInstance().Wait<GateRoleCollectionRes>(msgRet =>
                {
                    if (msgRet.result != (uint)ProtoErrorCode.SUCCESS)
                    {
                        SystemNotifyManager.SystemNotify((int)msgRet.result);
                    }
                    else
                    {
                        if(Value.roleInfo != null)
                        {
                            //如果是当前角色
                            if(Value.roleInfo.roleId == msgRet.roleId)
                            {
                                Value.roleInfo.isCollection = msgRet.isCollection;
                                UpdateResponseImg();
                            }
                        }
                    }
                }, true, 15.0f);
            }
        }

        public override void OnUpdate()
        {
            ProtoTable.JobTable occuItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>((int)Value.roleInfo.occupation);

            if (Value == null || occuItem == null)
            {
                _DisableAvatar();
            }
            else
            {
                _CreateRoleActor(AvatarRender,occuItem.Mode);
                rawImage.enabled = true;

                Job.text = occuItem.Name;
                Lv.text = "Lv." + Value.roleInfo.level;
                Name.text = Value.roleInfo.name;
                if(Value.roleInfo.isVeteranReturn > 0 && mOldPlayer != null)
                {
                    mOldPlayer.CustomActive(true);
                }
                else
                {
                    mOldPlayer.CustomActive(false);
                }

                if (likeBtn != null)
                    likeBtn.CustomActive(true);

                UpdateResponseImg();

                objBookingActivities.CustomActive(ClientApplication.playerinfo.GetRoleHasApponintmentOccu(Value.roleInfo));
            }
        }

        private void UpdateResponseImg()
        {
            if (responseImg != null)
            {
                if (Value.roleInfo.isCollection > 0)
                {
                    ETCImageLoader.LoadSprite(ref responseImg, likePath);
                }
                else
                {
                    ETCImageLoader.LoadSprite(ref responseImg, unLikePath);
                }

                responseImg.SetNativeSize();
            }
        }

        public override void OnDisplayChanged(bool bShow)
        {
            if (effectSelected != null)
            {
                effectSelected.SetVisible(bShow);
                effectSelected2.SetVisible(!bShow);

                //objImgDisSelect.CustomActive(!bShow);
                objImgSelect.CustomActive(bShow);

                SetTxtColor(!bShow);
            }

            if (AvatarRender != null/* && AvatarRender.IsCurActionEnd()*/)
            {
                if (bShow)
                {
                    //AvatarRender.ChangeAction(m_ActionTable[1]);
                    ChangeAction(AvatarRender, m_ActionTable[1]);
                    playQueueIdle = true;
                }
                else
                {
                    playQueueIdle = false;
                    //AvatarRender.ChangeAction(m_ActionTable[0]);
                    ChangeAction(AvatarRender, m_ActionTable[0]);
                }
            }
        }

        public override void OnFrameUpdate()
        {
            if (AvatarRender != null && AvatarRender.IsCurActionEnd() && playQueueIdle)
            {
                playQueueIdle = false;
                //AvatarRender.ChangeAction(m_ActionTable[0]);
                ChangeAction(AvatarRender, m_ActionTable[0]);
            }
        }

        public void ChangeAction(GeAvatarRendererEx avatar, string actionName)
        {
            if (avatar == null)
                return;

            bool loop = false;
            if (actionName.ToLower().Contains("idle"))
                loop = true;
            avatar.ChangeAction(actionName, 1.0f, loop);
        }

		public void SetTxtColor(bool isGray = true)
		{
            if (isGray)
            {
                Name.color = new Color(198 / 255.0f, 193 / 255.0f, 179 / 255.0f);
                Lv.color = new Color(198 / 255.0f, 193 / 255.0f, 179 / 255.0f);
                Job.color = new Color(198 / 255.0f, 193 / 255.0f, 179 / 255.0f);
            }
            else
            {
                Name.color = Color.white;
                Lv.color = Color.white;
                //5DD1F6FF
                Job.color = Color.white;
                //Job.color = new Color(0x5D/255.0f, 0xD1/255.0f, 0xf6/255.0f, 1.0f);
            }
        }

        public RoleSelectFieldState GetCurrRoleFieldState()
        {
            if (mRoleField != null)
            {
                return mRoleField.GetRoleSelectFieldState();
            }

            return RoleSelectFieldState.Default;
        }

        void _DisableAvatar()
        {
            AvatarRender.ClearAvatar();
            rawImage.enabled = false;
            if (effectSelected != null)
            {
                effectSelected.SetVisible(false);
            }

            if (likeBtn != null)
                likeBtn.CustomActive(false);
        }

        void _LoadActorLight(bool createRole)
        {
            if (null != SceneLightRoot)
                GameObject.Destroy(SceneLightRoot);

            if (createRole)
            {/// 创角灯光
                SceneLightRoot = AssetLoader.instance.LoadResAsGameObject("Scene/Start/Perfab/Light_chuangjue");
            }
            else
            {/// 选角灯光
                SceneLightRoot = AssetLoader.instance.LoadResAsGameObject("Scene/Start/Perfab/Light_xuanjue");
            }

            //if (null != SceneLightRoot)
            //{
            //    SceneLightRoot.name = "Light";
            //    if (null != SceneRoot)
            //        SceneLightRoot.transform.SetParent(SceneRoot.transform, false);

            //    Transform rootLightTM = SceneLightRoot.transform.FindChild("Character Light");
            //    if (null != rootLightTM)
            //        ActorLightRoot = rootLightTM.gameObject;
            //}
        }

        void _CreateRoleActor(GeAvatarRendererEx actor, int iModeId)
        {
            if (actor == null)
            {
                Logger.LogErrorFormat("actor is null!");
                return;
            }

            ProtoTable.ResTable res = TableManager.instance.GetTableItem<ProtoTable.ResTable>(iModeId);
            if (res == null)
            {
                Logger.LogErrorFormat("角色模型无法找到 ProtoTable.ResTable ID = [{0}]", iModeId);
                return;
            }

            //ComCreateRoleScene.LoadLi

            actor.LoadAvatar(res.ModelPath);
			//actor.ChangeAction(m_ActionTable[0]);

            //actor.RotateY(64.199f);

            _LoadEquipments(actor);

			ChangeAction(actor, m_ActionTable[0]);

            effectSelected = AvatarRender.AttachAvatar("Aureole",
                        "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS",
                        "[actor]Orign", false);
			effectSelected2 = AvatarRender.AttachAvatar("DisSelect",
				"Effects/Scene_effects/EffectUI/EffUI_chuangjue_fazhen_JS_jingtai",
				"[actor]Orign", false);


            effectSelected.SetVisible(false);
			effectSelected2.SetVisible(true);
        }

        void _LoadEquipments(GeAvatarRendererEx actor)
        {
            PlayerBaseData.GetInstance().AvatarEquipFromItems(actor,
                Value.roleInfo.avatar.equipItemIds,
                Value.roleInfo.occupation,
                Value.roleInfo.avatar.weaponStrengthen,
                null,
                false,
                Value.roleInfo.avatar.isShoWeapon);
        }
    }

    class RoleFunctionBinder : FunctionBinder<SelectRoleFrame>
    {
        //const int STATE_DEFAULT = 0;
        //const int STATE_HAS_ROLE = 1;

        public CachedObjectListManager<RoleObject> m_akRoleObjects = new CachedObjectListManager<RoleObject>();
        public static readonly int m_iMaxRoles = 8;                          //默认界面 一页为 8个角色栏位
        private int totalRoleFieldCount = 0;                                 //新增角色栏位概念 之前只是按角色显示

        public GameObject m_goParent;
        public GameObject[] m_goPrefabs = new GameObject[m_iMaxRoles];
        public Text m_roleCount;
        public Text m_roleFieldCount;
        Button btnArrowLeft;
        Button btnArrowRight;
        ComDotController dotRoot;

        Delegate delegateRoleSelected = null;

        int m_iCurPage = 1;                                                  //页数 默认为 1

        protected override void Initialize()
        {
            m_goParent = Utility.FindChild(frame, "Roles");
            for (int i = 0; i < m_goPrefabs.Length; ++i)
            {
                m_goPrefabs[i] = Utility.FindChild(m_goParent, string.Format("Role{0}", i));
                //m_goPrefabs[i].CustomActive(false);
            }
            delegateRoleSelected = Delegate.CreateDelegate(typeof(RoleObject.OnSelectedDelegate), this, "OnRoleSelected");
            m_roleCount = Utility.FindComponent<Text>(frame, "Roles/FixeBG/Count");
            m_roleFieldCount = Utility.FindComponent<Text>(frame, "Roles/FixeBG/FieldCount");
            btnArrowLeft = Utility.FindComponent<Button>(frame, "Roles/ArrowLeft");
            btnArrowLeft.onClick.AddListener(() =>
            {
                _MinusPage();
            });
            btnArrowRight = Utility.FindComponent<Button>(frame, "Roles/ArrowRight");
            btnArrowRight.onClick.AddListener(() =>
            {
                _AddPage();
            });
            dotRoot = Utility.FindComponent<ComDotController>(frame, "Roles/DotParent");
            if (dotRoot != null)
            {
                int iMaxPage = _GetMaxPage();
                dotRoot.InitDots(iMaxPage);
                dotRoot.CustomActive(false);
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SetDefaultSelectedID, _OnSetDefaultSelectedID);
            m_iCurPage = 1;
        }

        protected override void UnInitialize()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SetDefaultSelectedID, _OnSetDefaultSelectedID);

            delegateRoleSelected = null;
            for (int i = 0; i < m_goPrefabs.Length; ++i)
            {
                m_goPrefabs[i] = null;
            }
            m_goParent = null;
            RoleObject.Clear();
            m_akRoleObjects.DestroyAllObjects();
            btnArrowLeft.onClick.RemoveAllListeners();
            btnArrowRight.onClick.RemoveAllListeners();
        }

        protected override void Refresh()
        {
            ulong lastSelected = 0;
            if (RoleObject.Selected != null)
            {
                lastSelected = RoleObject.Selected.Value.roleInfo.roleId;
            }
            int iBindIndex = -1;

            //m_akRoleObjects.RecycleAllObject();
            m_akRoleObjects.DestroyAllObjects();

            ClientApplication.playerinfo.SortRoleInfoList();
            RoleInfo[] roleinfos = ClientApplication.playerinfo.roleinfo;

            List<RoleInfo> akRoles = new List<RoleInfo>();
            for (int i = 0; i < roleinfos.Length; ++i)
            {
                if (RecoveryRoleCachedObject.OnFilterAlive(roleinfos[i]))
                {
                    akRoles.Add(roleinfos[i]);
                    if (roleinfos[i].roleId == lastSelected && lastSelected > 0)
                    {
                        iBindIndex = i;
                    }
                }
            }

            _RefreshRoleNumFormat(akRoles.Count);

            //int iFindRoleIndex = -1;
            //for (int i = 0; i < akRoles.Count; ++i)
            //{
            //    if (akRoles[i].roleId == lastSelected && lastSelected > 0)
            //    {
            //        iFindRoleIndex = i;
            //        break;
            //    }
            //}
            //m_iCurPage = _TranslateIndexToPage(iFindRoleIndex);

            m_iCurPage = IntMath.Max(1, m_iCurPage);
            int iMaxPage = _GetMaxPage();
            m_iCurPage = IntMath.Min(m_iCurPage, iMaxPage);

            //List<RoleInfo> temps = new List<RoleInfo>();
            List<RoleFieldData> roleFieldTemps = new List<RoleFieldData>();
            int iEnd = m_iCurPage * m_iMaxRoles;
            int iStart = (m_iCurPage - 1) * m_iMaxRoles;
            //for (int i = iStart, end = iEnd; i < akRoles.Count && i < end; ++i)
            for (int i = iStart, end = iEnd; i < end; ++i)
            {
                var state = _GetRoleFieldStateByPageIndex(i, akRoles.Count);
                if (i < akRoles.Count)
                {                    
                    roleFieldTemps.Add(new RoleFieldData { roleInfo = akRoles[i], fieldState = state });
                }
                else
                {
                    roleFieldTemps.Add(new RoleFieldData { roleInfo = null, fieldState = state });
                }
            }
            //akRoles.Clear();
            //akRoles.AddRange(temps);

            for (int i = 0; i < m_goPrefabs.Length; ++i)
            {
                //m_goPrefabs[i].CustomActive(false);
                SetState(RoleSelectFieldState.Default, m_goPrefabs[i]);
            }

            //for (int i = 0; i < akRoles.Count && i < m_goPrefabs.Length; ++i)
            for (int i = 0; i < roleFieldTemps.Count && i < m_goPrefabs.Length; ++i)
            {
                if (roleFieldTemps[i] == null)
                {
                    continue;
                }

                //if (akRoles.Count == 1)
                var prefab = m_goPrefabs[i];
                /*				if (akRoles.Count == 1)
                                    prefab = m_goPrefabs[1];*/

                SetState(roleFieldTemps[i].fieldState, prefab);

                if (roleFieldTemps[i].fieldState == RoleSelectFieldState.BaseHasRole ||
                    roleFieldTemps[i].fieldState == RoleSelectFieldState.NewUnlockHasRole ||                    
                    roleFieldTemps[i].fieldState == RoleSelectFieldState.LockHasRole)
                {
                    var roleData = new RoleData { roleInfo = roleFieldTemps[i].roleInfo };
                    {
                        m_akRoleObjects.Create(new object[] { m_goParent, prefab, roleData, delegateRoleSelected, true });
                    }
                }
            }

            _OnSelectedIndex(iBindIndex);

            _OnPageChanged();
        }

        public bool IsEmpty()
        {
            return m_akRoleObjects.ActiveObjects.Count <= 0;
        }

        public void ClearRoleSelected()
        {
            if(RoleObject.Selected != null)
            {
                RoleObject.Selected = null;
            }
        }

        public void OnRoleSelected(RoleData data)
        {
            if(data != null && data.roleInfo != null)
            {
                OnRoleSelected(data.roleInfo.roleId);
            }
        }

        public void OnRoleSelected(ulong roleId)
        {
            int iIndex = -1;
            var roleinfos = ClientApplication.playerinfo.roleinfo;
            for (int i = 0; i < roleinfos.Length; ++i)
            {
                if (roleinfos[i].roleId == roleId)
                {
                    VoiceManager.instance.PlayVoiceByOccupation(ProtoTable.VoiceTable.eVoiceType.SELECTROLE, roleinfos[i].occupation);
                    ClientApplication.playerinfo.curSelectedRoleIdx = i;
                    iIndex = i;
                    break;
                }
            }

            _OnSelectedIndex(iIndex);
        }

        public void MoveToPageByRoleId(ulong roleId)
        {
            var rolesInfo = GamePool.ListPool<object>.Get();
            _GetRolesInfo(ref rolesInfo);
            int iFindIndex = -1;
            for (int i = 0; i < rolesInfo.Count; ++i)
            {
                if ((rolesInfo[i] as RoleInfo).roleId == roleId)
                {
                    iFindIndex = i;
                    break;
                }
            }
            m_iCurPage = _TranslateIndexToPage(iFindIndex);
            GamePool.ListPool<object>.Release(rolesInfo);
        }

        public ulong SetTheLatestLoginRoleAsDefault()
        {
            List<object> akRoles = GamePool.ListPool<object>.Get();
            _GetRolesInfo(ref akRoles);

            int iIndex = -1;
            ulong iRoleID = 0;
            for (int i = 0; i < akRoles.Count; ++i)
            {
                if (iIndex == -1)
                {
                    iIndex = i;
                    iRoleID = (akRoles[i] as RoleInfo).roleId;
                    continue;
                }

                if ((akRoles[i] as RoleInfo).offlineTime > (akRoles[iIndex] as RoleInfo).offlineTime)
                {
                    iIndex = i;
                    iRoleID = (akRoles[i] as RoleInfo).roleId;
                }
            }
            GamePool.ListPool<object>.Release(akRoles);

            m_iCurPage = _TranslateIndexToPage(iIndex);
            return iRoleID;
        }

        public void OnUpdate()
        {
            var values = m_akRoleObjects.ActiveObjects;
            for (int i = 0; i < values.Count; ++i)
            {
                values[i].OnFrameUpdate();
            }
        }

        void _OnSetDefaultSelectedID(UIEvent uiEvent)
        {
            _OnSelectedIndex(uiEvent.EventParams.CurrentSelectedID);
        }

        void _OnSelectedIndex(int iIndex)
        {
            RoleObject target = _GetSelectedRoleObject(iIndex);
            if (target != null)
            {
                target.OnSelected();
            }
            else
            {
                RoleObject.Clear();
            }
        }

        RoleObject _GetSelectedRoleObject(int iIndex)
        {
            var roles = ClientApplication.playerinfo.roleinfo;
            RoleObject target = null;
            if (iIndex >= 0 && iIndex < roles.Length)
            {
                var values = m_akRoleObjects.ActiveObjects;
                for (int i = 0; i < values.Count; ++i)
                {
                    if(null == values[i] || null == values[i].Value || null == values[i].Value.roleInfo)
                    {
                        continue;
                    }

                    if (values[i].Value.roleInfo.roleId == roles[iIndex].roleId)
                    {
                        target = values[i];
                        break;
                    }
                }
            }
            return target;
        }

        void _AddPage()
        {
            int iMaxPage = _GetMaxPage();
            if (m_iCurPage < iMaxPage)
            {
                ulong roleId = 0;
                if(RoleObject.Selected != null && RoleObject.Selected.Value != null && null != RoleObject.Selected.Value.roleInfo)
                {
                    roleId = RoleObject.Selected.Value.roleInfo.roleId;
                }

                ++m_iCurPage;
                Refresh();
                _OnPageChanged();

                if(m_akRoleObjects.ActiveObjects.Count > 0)
                {
                    bool bActived = false;
                    var values = m_akRoleObjects.ActiveObjects;
                    for(int i = 0; i < values.Count; ++i)
                    {
                        if (null != values[i] && null != values[i].Value && null != values[i].Value.roleInfo &&
                            values[i].Value.roleInfo.roleId == roleId)
                        {
                            OnRoleSelected(values[i].Value);
                            bActived = true;
                            break;
                        }
                    }
                    //向后翻页 默认选中第一个
                    for (int i = 0; i < values.Count && !bActived; ++i)
                    {
                        if(null != values[i] && null != values[i].Value && null != values[i].Value.roleInfo)
                        {
                            OnRoleSelected(values[i].Value);
                            break;
                        }
                    }
                }
            }
        }

        void _MinusPage()
        {
            if(m_iCurPage > 1)
            {
                ulong roleId = 0;
                if (RoleObject.Selected != null && RoleObject.Selected.Value != null && null != RoleObject.Selected.Value.roleInfo)
                {
                    roleId = RoleObject.Selected.Value.roleInfo.roleId;
                }

                --m_iCurPage;
                Refresh();
                _OnPageChanged();

                if (m_akRoleObjects.ActiveObjects.Count > 0)
                {
                    var values = m_akRoleObjects.ActiveObjects;
                    if(values.Count > 0)
                    {
                        bool bActived = false;
                        for (int i = values.Count - 1; i >= 0; --i)
                        {
                            if (null != values[i] && null != values[i].Value && null != values[i].Value.roleInfo &&
                                values[i].Value.roleInfo.roleId == roleId)
                            {
                                OnRoleSelected(values[i].Value);
                                bActived = true;
                                break;
                            }
                        }
                        //向前翻页 默认选中最后一个
                        for (int i = values.Count - 1; i >= 0 && !bActived; --i)
                        {
                            if (null != values[i] && null != values[i].Value && null != values[i].Value.roleInfo)
                            {
                                OnRoleSelected(values[i].Value);
                                break;
                            }
                        }
                    }
                }
            }
        }

        void _OnPageChanged()
        {
            int iMaxPage = _GetMaxPage();
            btnArrowLeft.CustomActive(m_iCurPage > 1);
            btnArrowRight.CustomActive(m_iCurPage < iMaxPage);

            if (dotRoot != null)
            {
                dotRoot.CustomActive(iMaxPage >= 2);
                dotRoot.SetDots(m_iCurPage, iMaxPage);
            }
        }

        /// <summary>
        /// 计算 最大 页数
        /// </summary>
        /// <returns></returns>
        int _GetMaxPage()
        {
            //拥有的角色数量
            /*
            var rolesInfo = GamePool.ListPool<object>.Get();
            _GetRolesInfo(ref rolesInfo);
            int num = rolesInfo.Count;
            GamePool.ListPool<object>.Release(rolesInfo);
            */
            //需要展示的角色栏位
            int num = _GetTotalNeedShowRoleFieldCount();

            if (num <= 0)
            {
                return 1;
            }
            return (num - 1)/ m_iMaxRoles + 1;
        }       

        /// <summary>
        /// 获取拥有的角色信息 (RoleInfo)
        /// </summary>
        /// <param name="akRoles"></param>
        void _GetRolesInfo(ref List<object> akRoles)
        {
            if(null != akRoles)
            {
                RoleInfo[] roleinfos = ClientApplication.playerinfo.roleinfo;
                for (int i = 0; i < roleinfos.Length; ++i)
                {
                    if (RecoveryRoleCachedObject.OnFilterAlive(roleinfos[i]))
                    {
                        akRoles.Add(roleinfos[i]);
                    }
                }
            }
        }       

		protected void CreateRole(bool changed)
		{
            /*
			if (!changed)
				return;
			
			SelectRoleFrame frame = clientFrame as SelectRoleFrame;
			if (frame != null)
				frame._onCreateButton();
                */
		}

		protected void SetState(RoleSelectFieldState state, GameObject prefab)
		{
			if (prefab == null)
				return;

            var comField = prefab.GetComponent<ComSelectRoleField>();
            if (comField != null)
			{
                comField.SetRoleSelectFieldState(state);

                switch (state)
                {
                    case RoleSelectFieldState.None:
                        comField.SetNoneStateShow();
                        break;
                    case RoleSelectFieldState.Default:
                        comField.SetDefaultStateShow();
                        break;
                    case RoleSelectFieldState.BaseHasRole:
                        comField.SetBaseHasRoleStateShow();
                        break;
                    case RoleSelectFieldState.LockHasRole:
                        comField.SetLockHasRoleStateShow();
                        break;
                    case RoleSelectFieldState.NewUnlockHasRole:
                        comField.SetNewUnlockHasRoleStateShow();
                        break;                    
                    case RoleSelectFieldState.LockNoRole:
                        comField.SetLockNoRoleStateShow();
                        break;
                    case RoleSelectFieldState.NewUnlockNoRole:
                        comField.SetNewUnlockNoRoleStateShow();
                        break;
                }
			}
		}

        int _GetMaxPage(int iCount)
        {
            if (iCount <= 0)
            {
                return 1;
            }

            if (iCount % m_iMaxRoles == 0)
            {
                return iCount / m_iMaxRoles;
            }

            return 1 + iCount / m_iMaxRoles;
        }

        int _TranslateIndexToPage(int iIndex)
        {
            if (iIndex <= 0)
            {
                iIndex = 0;
            }

            return iIndex / m_iMaxRoles + 1;
        }

        #region Modify on adventure team

        /// <summary>
        /// 初始化拥有角色数目 / 非额外角色栏位数  => 用于格式文本
        /// </summary>
        /// <param name="roleNum"> 拥有角色数 </param>
        void _RefreshRoleNumFormat(int roleNum)
        {
            int currOwnRoleFieldNum = _GetTotalEnableRoleFieldCount();

            if (m_roleCount)
            {
                m_roleCount.text = TR.Value("select_role_count", roleNum.ToString(), currOwnRoleFieldNum.ToString());
            }

            //int totalEnableRoleFieldNum = Mathf.Max(_GetTotalEnableRoleFieldCount(), _GetStandardRoleFieldCount());
            //if (m_roleFieldCount)
            //{
            //    m_roleFieldCount.text = TR.Value("select_role_field_count", currOwnRoleFieldNum.ToString(), totalEnableRoleFieldNum.ToString());
            //}
        }

        /// <summary>
        /// 通过序号 和 帐号拥有总角色数 获取 当前序号对应 角色栏位的 状态
        /// </summary>
        /// <param name="index"> 每页的序号，!!! 注 ： 第一页从0开始 !!! </param>
        /// <param name="roleCount"> 帐号拥有的角色 </param>
        /// <returns></returns>
        RoleSelectFieldState _GetRoleFieldStateByPageIndex(int index, int roleCount)
        {
            RoleSelectFieldState roleFieldState = RoleSelectFieldState.None;
           
            if (index < _GetStandardBaseRoleFieldNum())
            {
                roleFieldState = index < roleCount ? RoleSelectFieldState.BaseHasRole : RoleSelectFieldState.Default;
            }
            else if (index >= _GetStandardBaseRoleFieldNum() && index < _GetTotalEnableRoleFieldCount())
            {
                //新解锁栏位 需要放到 福利栏位（合服出来的）之前                
                if (_GetStandardExtendNewUnlockRoleFieldNum() > 0 &&
                    index >= (_GetTotalEnableRoleFieldCount() - _GetStandardExtendNewUnlockRoleFieldNum()))
                {                    
                    roleFieldState = index < roleCount ? RoleSelectFieldState.NewUnlockHasRole : RoleSelectFieldState.NewUnlockNoRole;
                }
                else
                {
                    roleFieldState = index < roleCount ? RoleSelectFieldState.BaseHasRole : RoleSelectFieldState.Default;
                }
            }
            else if (index >= _GetTotalEnableRoleFieldCount())
            {
                //表示还有可解锁栏位
                //相当于 _GetStandardRoleFieldCount > _GetStandardEnableRoleFieldCount
                if (_GetTotalLockRoleFieldCount() > 0)
                {
                    //角色数超出标准加可解锁 角色栏位 
                    if (roleCount > _GetStandardRoleFieldCount())
                    {
                        //如果角色总数超出标准栏位 则将锁定栏位后置到最后 调整序号
                        //不同于 新解锁栏位的前置
                        //int lockFieldStartIndex = roleCount - _GetStandardLockRoleFieldCount();
                        //if (index < lockFieldStartIndex)
                        //{
                        //    roleFieldState = RoleSelectFieldState.BaseHasRole;
                        //}
                        //else
                        //{
                        //    roleFieldState = index < roleCount ? RoleSelectFieldState.LockHasRole : RoleSelectFieldState.None;
                        //}

                        //福利栏位和未解锁栏位统一标志
                        roleFieldState = index < roleCount ? RoleSelectFieldState.LockHasRole : RoleSelectFieldState.None;
                    }
                    //角色数未达到标准加可解锁 角色栏位，还可以解锁
                    else
                    {
                        if (index < roleCount)
                        {
                            roleFieldState = RoleSelectFieldState.LockHasRole;
                        }
                        else
                        {
                            roleFieldState = index < _GetStandardRoleFieldCount() ? RoleSelectFieldState.LockNoRole : RoleSelectFieldState.None;
                        }                        
                    }
                }
                //没有可解锁栏位 并且 没有可用栏位 
                else
                {
                    //如果还有角色 就占据 福利栏位 , 否则 就是 空栏位                    
                    //roleFieldState = index < roleCount ? RoleSelectFieldState.BaseHasRole : RoleSelectFieldState.None;
                    
                    //福利栏位 默认也展示为 未解锁有角色 
                    roleFieldState = index < roleCount ? RoleSelectFieldState.LockHasRole : RoleSelectFieldState.None;
                }
            }
            return roleFieldState;
        }

        /// <summary>
        /// 获得 标准栏位数 （纯服务器数据）
        /// </summary>
        /// <returns></returns>
        int _GetStandardBaseRoleFieldNum()
        {
            int baseRoleFieldNum = m_iMaxRoles;
            if (ClientApplication.playerinfo != null)
            {
                if (baseRoleFieldNum != (int)ClientApplication.playerinfo.baseRoleFieldNum)
                {
                    //Logger.LogErrorFormat("[RoleFunctionBinder] _GetStandardBaseRoleFieldNum netData {0} is not equal to localData {1}", (int)ClientApplication.playerinfo.baseRoleFieldNum , baseRoleFieldNum);
                    return baseRoleFieldNum;
                }
                return (int)ClientApplication.playerinfo.baseRoleFieldNum;
            }
            return baseRoleFieldNum;
        }

        /// <summary>
        /// 获得 可扩展栏位数（包含可解锁和未解锁） （纯服务器数据）
        /// </summary>
        /// <returns></returns>
        int _GetStandardExtendRoleFieldNum()
        {
            if (ClientApplication.playerinfo != null)
            {
                return (int)ClientApplication.playerinfo.extendRoleFieldNum;
            }
            return 0;
        }

        /// <summary>
        /// 获得 已经解锁的可扩展栏位数 由于合服，可能会大于可扩展栏位数 （纯服务器数据）
        /// </summary>
        /// <returns></returns>
        int _GetStandardExtendUnlockRoleFieldNum()
        {
            if (ClientApplication.playerinfo != null)
            {
                return (int)ClientApplication.playerinfo.unLockedExtendRoleFieldNum;
            }
            return 0;
        }

        /// <summary>
        /// 新解锁栏位数 （看策划需求 到底啥时候要这个数据清空  可能 1.只针对本次登录有效)
        /// </summary>
        /// <returns></returns>
        int _GetStandardExtendNewUnlockRoleFieldNum()
        {
            if (ClientApplication.playerinfo != null)
            {
                return (int)ClientApplication.playerinfo.newUnLockExtendRoleFieldNum;
            }
            return 0;
        }

        /// <summary>
        /// 全部 角色栏位 （未解锁）    
        /// 
        /// 考虑合服状态下 已解锁的栏位（购买开启） 可能大于 基础可解锁栏位
        /// </summary>
        /// <returns></returns>
        int _GetTotalLockRoleFieldCount()
        {
            int lockNum = 0;
            if (ClientApplication.playerinfo != null)
            {
                lockNum = _GetStandardExtendRoleFieldNum() - _GetStandardExtendUnlockRoleFieldNum();
                if (lockNum < 0)
                {
                    lockNum = 0;
                }
            }
            return lockNum;
        }

        /// <summary>
        /// 全部 角色栏位( 可用，已解锁 ) ： 默认栏位 + 可扩展（开）栏位 （合服时能需要叠加每个服解锁的栏位）
        /// 
        /// !!! 可能大于 小于 等于  _GetStandardRoleFieldCount() : （服务器下发）标准角色栏位 ： 默认栏位 + 可扩展栏位 （合服时可扩展栏位 不叠加） 
        /// </summary>
        /// <returns></returns>
        int _GetTotalEnableRoleFieldCount()
        {
            int sRoleFieldNum = m_iMaxRoles;
            if (ClientApplication.playerinfo != null)
            {
                sRoleFieldNum = _GetStandardBaseRoleFieldNum() + _GetStandardExtendUnlockRoleFieldNum();
            }
            return sRoleFieldNum;
        }

        /// <summary>
        ///  （服务器下发）标准角色栏位 ： 默认栏位 + 可扩展栏位 （合服时可扩展栏位 不叠加） 
        /// </summary>
        /// <returns></returns>
        int _GetStandardRoleFieldCount()
        {
            int sRoleFieldNum = m_iMaxRoles;
            if (ClientApplication.playerinfo != null)
            {
                sRoleFieldNum = _GetStandardBaseRoleFieldNum() + _GetStandardExtendRoleFieldNum();
            }
            return sRoleFieldNum;
        }

        /// <summary>
        /// 需要展示的角色栏位数
        /// 可以按含义分 标准角色栏位 + 福利栏位 （如合服时 需要额外放置其他服角色的栏位 仅客户端展示）
        /// </summary>
        /// <returns></returns>
        int _GetTotalNeedShowRoleFieldCount()
        {
            int tRoleFieldNum = Mathf.Max(_GetTotalEnableRoleFieldCount(), _GetStandardRoleFieldCount());

            var rolesInfo = GamePool.ListPool<object>.Get();
            _GetRolesInfo(ref rolesInfo);
            int roleCount = rolesInfo.Count;
            GamePool.ListPool<object>.Release(rolesInfo);

            if (tRoleFieldNum < roleCount)
            {
                tRoleFieldNum = roleCount;
            }

            return tRoleFieldNum;
        }

        #endregion
    }
}