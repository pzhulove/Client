using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Network;
using DG.Tweening;
using ProtoTable;
using Scripts.UI;
using Spine.Unity;

namespace GameClient
{
    /// <summary>
    /// 转职职业数据
    /// </summary>
    public class ChangeJobData
    {
        public string weaponIconPath;
        public List<int> changeJobList;
    }

    public class CreateRoleFrame : ClientFrame
    {
        private List<ChangeJobData> JobList = new List<ChangeJobData>();
        public static int CurSelectedJobID = 0;

        private int appointment_Max_Role = 0;  //最多预约角色个数

        // 需要随机的名字规则组合
        List<NameTable> akNameFirst = new List<NameTable>();
        List<NameTable> akNameSecond = new List<NameTable>();
        List<NameTable> akNameMiddle = new List<NameTable>();

        List<DOTweenAnimation> allAnimList = new List<DOTweenAnimation>();
        List<Image> imageNameList = new List<Image>();
        GameObject objJobImage = null;

        private List<string> aniNames = new List<string>() { "start", "stand" };

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SelectRole/CreateRoleFrame";
        }

        protected override void _OnOpenFrame()
        {
            _InitMovieCtrl();

            var SVTable = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_APPOINTMENT_MAX_ROLE);
            appointment_Max_Role = SVTable.Value;

            _InitJobData();
            _InitJobItemScrollListBind();
            _RefreshJobList();
        }

        protected override void _OnCloseFrame()
        {
            if(JobList != null)
            {
                JobList.Clear();
            }

            CurSelectedJobID = 0;
            appointment_Max_Role = 0;

            if(mMovieCtrl != null)
            {
                mMovieCtrl.OnReady -= _OnVideoReady;
                mMovieCtrl.OnVideoError -= _OnVideoError;
            }

            if(allAnimList != null)
            {
                allAnimList.Clear();
            }

            if(imageNameList != null)
            {
                imageNameList.Clear();
            }

            _StopMovie();
            _UnInitJobItemScrollListBind();
        }

        void _InitJobData()
        {
            var jobTable = TableManager.GetInstance().GetTable<JobTable>();
            if (jobTable != null)
            {
                var enumerator = jobTable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    JobTable jobItem = enumerator.Current.Value as JobTable;

                    if(jobItem == null)
                    {
                        continue;
                    }

                    if(jobItem.JobType != 0)
                    {
                        continue;
                    }

                    ChangeJobData changeJobData = new ChangeJobData();
                    changeJobData.changeJobList = new List<int>();
                    IList<int> temp = TableManager.GetInstance().GetNextJobsIDByCurJobID(jobItem.ID);

                    List<int> toJobList = new List<int>();
                    if (temp != null)
                    {
                        foreach (var jobId in temp)
                        {
                            var tb = TableManager.GetInstance().GetTableItem<JobTable>(jobId);
                            if (tb == null)
                            {
                                continue;
                            }

                            if (tb.CanCreateRole <= 0)
                            {
                                continue;
                            }

                            toJobList.Add(jobId);
                        }
                    }

                    if(toJobList.Count > 0)
                    {
                        changeJobData.changeJobList.AddRange(toJobList);

                        JobList.Add(changeJobData);
                    }
                }
            }
        }
        
        void _UnInitJobItemScrollListBind()
        {
            if (mBaseJobScrollView != null)
            {
                mBaseJobScrollView.onBindItem -= _OnBindJobItemDelegate;
                mBaseJobScrollView.onItemVisiable -= _OnJobItemVisiableDelegate;
            }
        }

        void _InitJobItemScrollListBind()
        {
            if(mBaseJobScrollView != null)
            {
                mBaseJobScrollView.Initialize();

                mBaseJobScrollView.onBindItem += _OnBindJobItemDelegate;
                mBaseJobScrollView.onItemVisiable += _OnJobItemVisiableDelegate;
            }
        }


        JobItem _OnBindJobItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<JobItem>();
        }

        void _OnJobItemVisiableDelegate(ComUIListElementScript item)
        {
            JobItem jobItem = item.gameObjectBindScript as JobItem;
            if (jobItem == null)
            {
                return;
            }

            if (item.m_index < 0 || item.m_index >= JobList.Count)
            {
                return;
            }

            ChangeJobData changeJobData = JobList[item.m_index];

            jobItem.OnItemVisiable(item.m_index, changeJobData, _OnSelectedJob, item.m_index == CurSelectedJobID);
        }

        void _RefreshJobList()
        {
            if (mBaseJobScrollView != null && JobList != null && JobList.Count > 0)
            {
                mBaseJobScrollView.SetElementAmount(JobList.Count);
            }
        }

        private void _OnSelectedJob(int jobID)
        {
            JobTable tableData = TableManager.GetInstance().GetTableItem<JobTable>(jobID);
            if(tableData == null)
            {
                return;
            }

            if (tableData.Open > 0)
            {
                mBtnCreateGray.enabled = false;
            }
            else
            {
                mBtnCreateGray.enabled = true;
            }

            if (CurSelectedJobID != jobID)
            {
                CurSelectedJobID = jobID;
                PlayerBaseData.GetInstance().PreChangeJobTableID = jobID;

                if (mCharacterDesc != null)
                {
                    mCharacterDesc.text = TR.Value("creat_role_character_desc", tableData.RecommendedAttribute);
                }

                if (mJobDesc != null)
                {
                    mJobDesc.text = tableData.JobDes[0];
                }

                //名字
                int index = 0;
                for (; index < tableData.JobNameImgPaths.Length; ++index)
                {
                    if (index >= imageNameList.Count)
                    {
                        Logger.LogErrorFormat("创角界面职业名字过长，预制体中准备字体不足");
                        break;
                    }
                    imageNameList[index].CustomActive(true);
                    imageNameList[index].SafeSetImage(tableData.JobNameImgPaths[index]);
                }
                for (; index < imageNameList.Count; ++index)
                {
                    imageNameList[index].CustomActive(false);
                }

                if (mJobWeaponIcon != null)
                {
                    Image jobWeaponIcon = mJobWeaponIcon;
                    ETCImageLoader.LoadSprite(ref jobWeaponIcon, tableData.JobWeaponIcon);
                    jobWeaponIcon.SetNativeSize();
                }

                _PlayMovie(tableData.Video);

                if (objJobImage != null)
                {
                    GameObject.DestroyImmediate(objJobImage);
                    objJobImage = null;
                }

                if (tableData.JobImage.Contains("Animation"))
                {
                    _ShowModule(tableData.ID, tableData.JobImage);
                }
                else
                {
                    _HideModule();

                    objJobImage = AssetLoader.instance.LoadResAsGameObject(tableData.JobImage);
                    if (objJobImage != null && mSpineRoot != null)
                    {
                        Utility.AttachTo(objJobImage, mSpineRoot);

                        objJobImage.transform.SetAsFirstSibling();
                    }
                }

                mSpineRoot.SetActive(false);

                ClientSystemManager.GetInstance().delayCaller.DelayCall(100, () =>
                {
                    if (mSpineRoot != null)
                        mSpineRoot.CustomActive(true);
                });

                if (mCommonRoleAbilityChart != null)
                    mCommonRoleAbilityChart._OnRefreshAbilityChartList(tableData.ID);

                PlayAnim();
            }
        }

        void PlayAnim()
        {
            for (int i = 0; i < allAnimList.Count; i++)
            {
                allAnimList[i].DORestart();
            }
        }

        SkeletonAnimation spineAnimtion = null;

        DelayCallUnitHandle delayHandle;
        const string ACTION_START = "start";
        const string ACTION_IDLE = "stand";

        void setSpineAnimatin()
        {
            // if (mObjRender == null || mObjRender.LoadedGameObject == null)
            //     return;

            // spineAnimtion = mObjRender.LoadedGameObject.GetComponent<SkeletonAnimation>();

            // if (spineAnimtion == null)
            //     return;
            // /*
            // 如果有start和stand就先播start，再播stand循环
            // 否则就循环播start
            // */
            // if (spineAnimtion.HasAnimation(ACTION_START))
            // {
            //     var startAnimation = spineAnimtion.GetAnimation(ACTION_START);

            //     Logger.LogProcessFormat("play start!!!!");
            //     spineAnimtion.AnimationName = ACTION_START;
            //     spineAnimtion.loop = false;
                
            //     delayHandle = ClientSystemManager.GetInstance().delayCaller.DelayCall((int)(startAnimation.Duration*1000), () =>
            //     {
            //         Logger.LogProcessFormat("play stand!!!!");
            //         spineAnimtion.AnimationName = ACTION_IDLE;
            //         spineAnimtion.loop = true;
            //     });
                
            // }
            // //否则什么也做
        }

        void _ShowModule(int jobID, string path)
        {
            if (mObjRender != null)
            {
                mObjRender.gameObject.CustomActive(true);
                mObjRender.ClearObject();

                
                delayHandle.SetRemove(true);
                try
                {
                    mObjRender.LoadObject(path, 28);
                    mObjRender.AddAni(aniNames);
                    setSpineAnimatin();
                }
                catch (System.Exception e)
                {
                    Logger.LogErrorFormat("create spineModule failed: {0}", e.ToString());
                }
            }
        }

        void _HideModule()
        {
            if (mObjRender != null)
            {
                mObjRender.gameObject.CustomActive(false);
                delayHandle.SetRemove(true);
            }
        }

        void _InitMovieCtrl()
        {
            if (mMovieCtrl != null)
            {
                mMovieCtrl.OnReady += _OnVideoReady;
                mMovieCtrl.OnVideoError += _OnVideoError;
            }
        }

        void _PlayMovie(string path)
        {
            if(path == "0")
            {
                if (mMovieCtrl != null)
                {
                    mMovieCtrl.CustomActive(false);
                }

                if(mObjNoPreview != null)
                {
                    mObjNoPreview.CustomActive(true);
                }
            }
            else
            {
                if (mObjNoPreview != null)
                {
                    mObjNoPreview.CustomActive(false);
                }

                if (mMovieCtrl != null)
                {
                    mMovieCtrl.CustomActive(true);
                    _StopMovie();

                    mMovieCtrl.Load(path);
                    if (mTxtMoive != null)
                    {
                        mTxtMoive.CustomActive(false);
                    }
                }
            }
        }

        void _StopMovie()
        {
            if (mMovieCtrl != null)
            {
                if (mMovieCtrl.GetCurrentState() != MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY)
                {
                    mMovieCtrl.Stop();
                    mMovieCtrl.UnLoad();
                }
            }
        }

        void _OnVideoReady()
        {
            if (mTxtMoive != null)
            {
                mTxtMoive.CustomActive(false);
            }

            if (mImgMovie != null)
            {
                mImgMovie.CustomActive(true);
                mImgMovie.color = Color.black;
            }

            DOTween.To(
                () =>
                {
                    if (mImgMovie != null)
                    {
                        return mImgMovie.color;
                    }

                    return Color.white;
                },
                (value) =>
                {
                    if (mImgMovie != null)
                    {
                        mImgMovie.color = value;
                    }
                },
                Color.white, 1.0f);
            if (mMovieCtrl != null)
            {
                mMovieCtrl.SetVolume(0f);
            }

            DOTween.To(
                () =>
                {
                    if (mMovieCtrl != null)
                    {
                        return mMovieCtrl.GetVolume();
                    }

                    return 1.0f;
                },
                (value) =>
                {
                    if (mMovieCtrl != null)
                    {
                        mMovieCtrl.SetVolume(value);
                    }
                },
                1.0f,
                1.0f);

            if (mMovieCtrl != null)
            {
                mMovieCtrl.Play();
            }
        }

        void _OnVideoError(MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCode, MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCodeExtra)
        {
            if (mTxtMoive != null)
            {
                mTxtMoive.CustomActive(true);
            }
                
            if (mImgMovie != null)
            {
                mImgMovie.CustomActive(false);
            }

            Logger.LogErrorFormat("play moive code:{0}, extra:{1}", errorCode, errorCodeExtra);
        }

        int _GetLength(string content)
        {
            int iLen = 0;
            for (int i = 0; i < content.Length; ++i)
            {
                if (content[i] >= '\u4e00' && content[i] <= '\u9fa5')
                {
                    iLen += 2;
                }
                else
                {
                    iLen += 1;
                }
            }
            return iLen;
        }

        void _CreateRoleReq()
        {
            GateCreateRoleReq req = new GateCreateRoleReq();

            req.name = mInputField.text;
            req.occupation = (System.Byte)CurSelectedJobID;

            NetManager netMgr = NetManager.Instance();

            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            ClientSystemLogin curLogin = ClientSystemManager.instance.CurrentSystem as ClientSystemLogin;
            if (null != curLogin)
            {
                curLogin.MarkNewActor(req.name);
            }
        }

        [MessageHandle(GateCreateRoleRet.MsgID)]
        void _OnGateCreateRoleRet(MsgDATA msg)
        {
            Logger.Log("OnGateSendRoleInfo ..\n");

            GateCreateRoleRet ret = new GateCreateRoleRet();
            ret.decode(msg.bytes);

            if (ret.result != 0)
            {
                SystemNotifyManager.SystemNotify((int)ret.result);
            }
            else
            {
                PlayerBaseData.GetInstance().PreChangeJobTableID = CurSelectedJobID;
            }
        }

        public void OnDragDemoRole(float delta)
        {
            //TO OPEN
            //ComCreateRoleScene._DragRotActor(delta);
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        #region ExtraUIBind
        private Button mClose = null;
        private Button mBtnCreate = null;
        private Button mBtnRandomName = null;
        private InputField mInputField = null;
        private ComUIListScript mBaseJobScrollView = null;
        private MediaPlayerCtrl mMovieCtrl = null;
        private RawImage mImgMovie = null;
        private Text mTxtMoive = null;
        private GameObject mSpineRoot = null;
        private GeObjectRenderer mObjRender = null;
        private CommonRoleAbilityChart mCommonRoleAbilityChart = null;
        private Text mWeaponDesc = null;
        private Text mCharacterDesc = null;
        private Text mJobDesc = null;
        private ImageEx mJobWeaponIcon = null;
        private GameObject mAnimationroot1 = null;
        private GameObject mAnimationroot2 = null;
        private GameObject mMovie = null;
        private GameObject mRoleAbilityChartScrollView = null;
        private ImageEx mName1 = null;
        private ImageEx mName2 = null;
        private ImageEx mName3 = null;
        private ImageEx mName4 = null;
        private UIGray mBtnCreateGray = null;
        private GameObject mObjNoPreview = null;
        private GameObject mRightAniRoot = null;
        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mBtnCreate = mBind.GetCom<Button>("BtnCreate");
            mBtnCreate.onClick.AddListener(_onBtnCreateButtonClick);
            mBtnRandomName = mBind.GetCom<Button>("BtnRandomName");
            mBtnRandomName.onClick.AddListener(_onBtnRandomNameButtonClick);
            mInputField = mBind.GetCom<InputField>("InputField");
            mBaseJobScrollView = mBind.GetCom<ComUIListScript>("BaseJobScrollView");
            mMovieCtrl = mBind.GetCom<MediaPlayerCtrl>("MovieCtrl");
            mImgMovie = mBind.GetCom<RawImage>("imgMovie");
            mTxtMoive = mBind.GetCom<Text>("txtMoive");
            mSpineRoot = mBind.GetGameObject("SpineRoot");
            mObjRender = mBind.GetCom<GeObjectRenderer>("objRender");
            mCommonRoleAbilityChart = mBind.GetCom<CommonRoleAbilityChart>("AbilityChartScrollView");
            mWeaponDesc = mBind.GetCom<Text>("WeaponDesc");
            mCharacterDesc = mBind.GetCom<Text>("CharacterDesc");
            mJobDesc = mBind.GetCom<Text>("JobDesc");
            mJobWeaponIcon = mBind.GetCom<ImageEx>("JobWeaponIcon");
            mAnimationroot1 = mBind.GetGameObject("animationroot1");
            mAnimationroot2 = mBind.GetGameObject("animationroot2");
            mMovie = mBind.GetGameObject("Movie");
            mRoleAbilityChartScrollView = mBind.GetGameObject("RoleAbilityChartScrollView");
            mName1 = mBind.GetCom<ImageEx>("Name1");
            mName2 = mBind.GetCom<ImageEx>("Name2");
            mName3 = mBind.GetCom<ImageEx>("Name3");
            mName4 = mBind.GetCom<ImageEx>("Name4");
            mBtnCreateGray = mBind.GetCom<UIGray>("BtnCreateGray");
            mObjNoPreview = mBind.GetGameObject("ObjNoPreview");
            mRightAniRoot = mBind.GetGameObject("RightAniRoot");

            imageNameList.Add(mName1);
            imageNameList.Add(mName2);
            imageNameList.Add(mName3);
            imageNameList.Add(mName4);

            DOTweenAnimation[] anim = mRightAniRoot.GetComponents<DOTweenAnimation>();
            allAnimList.AddRange(anim);
            anim = mAnimationroot1.GetComponents<DOTweenAnimation>();
            allAnimList.AddRange(anim);
            anim = mAnimationroot2.GetComponents<DOTweenAnimation>();
            allAnimList.AddRange(anim);
            anim = mMovie.GetComponents<DOTweenAnimation>();
            allAnimList.AddRange(anim);
            anim = mRoleAbilityChartScrollView.GetComponents<DOTweenAnimation>();
            allAnimList.AddRange(anim);


#if UNITY_ANDROID
			var lscale = mImgMovie.transform.localScale;
			lscale.y = -1;
			mImgMovie.transform.localScale = lscale;
#endif
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mBtnCreate.onClick.RemoveListener(_onBtnCreateButtonClick);
            mBtnCreate = null;
            mBtnRandomName.onClick.RemoveListener(_onBtnRandomNameButtonClick);
            mBtnRandomName = null;
            mInputField = null;
            mBaseJobScrollView = null;
            mMovieCtrl = null;
            mImgMovie = null;
            mTxtMoive = null;
            mSpineRoot = null;
            mObjRender = null;
            mCommonRoleAbilityChart = null;
            mWeaponDesc = null;
            mCharacterDesc = null;
            mJobDesc = null;
            mJobWeaponIcon = null;
            mAnimationroot1 = null;
            mAnimationroot2 = null;
            mMovie = null;
            mRoleAbilityChartScrollView = null;
            mName1 = null;
            mName2 = null;
            mName3 = null;
            mName4 = null;
            mBtnCreateGray = null;
            mObjNoPreview = null;
            mRightAniRoot = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);

            if (!ClientApplication.HasRoles())
            {
                ClientApplication.DisconnectGateServerAtLogin();
            }
        }

        private void _onBtnCreateButtonClick()
        {
            var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(CurSelectedJobID);
            if(jobTable != null)
            {
                if(jobTable.Open <= 0)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("create_role_un_open"));
                    return;
                }
            }

            if (string.IsNullOrEmpty(mInputField.text))
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("need_role_name"));
                return;
            }

            int iCharLength = _GetLength(mInputField.text);
            if (mInputField.text.Length > 7) // 限制最大7个字符
            {
                SystemNotifyManager.SystemNotify(8504);
                return;
            }

            // 预约角色活动这里的判断,请对比1.0代码，因为1.5没有基础职业了. by wangbo 2020.06.23
            if (ClientApplication.playerinfo.appointmentRoleNum >= appointment_Max_Role)
            {
                string ncontent = TR.Value("appointmentRoleFullDes");
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(ncontent, () =>
                {
                    _CreateRoleReq();
                }
                );
                return;
            }

            _CreateRoleReq();
        }

        private void _onBtnRandomNameButtonClick()
        {
            Logger.LogProcessFormat("OnClickRandomName A");
            if (akNameFirst.Count == 0 && akNameSecond.Count == 0 && akNameMiddle.Count == 0)
            {
                var nameTable = TableManager.GetInstance().GetTable<NameTable>();
                var enumerator = nameTable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var nameItem = enumerator.Current.Value as NameTable;
                    if (nameItem.NameType == 0)
                    {
                        akNameFirst.Add(enumerator.Current.Value as NameTable);
                    }
                    else if (nameItem.NameType == 1)
                    {
                        akNameSecond.Add(enumerator.Current.Value as NameTable);
                    }
                    else
                    {
                        akNameMiddle.Add(enumerator.Current.Value as NameTable);
                    }
                }
            }

            if (akNameFirst.Count > 0 && akNameSecond.Count > 0 && akNameMiddle.Count > 0)
            {
                Logger.LogProcessFormat("OnClickRandomName B");
                Int32 iRandomFirst = UnityEngine.Random.Range(0, akNameFirst.Count);
                Int32 iRandomSecond = UnityEngine.Random.Range(0, akNameSecond.Count);
                Int32 iRandomMiddle = UnityEngine.Random.Range(0, akNameMiddle.Count);
                if (iRandomFirst >= akNameFirst.Count || iRandomSecond >= akNameSecond.Count || iRandomMiddle >= akNameMiddle.Count)
                {
                    Logger.LogErrorFormat("iRandomFist is {0},iRandomMiddle is {1},iRandomSecond is {2}", iRandomFirst, iRandomMiddle, iRandomSecond);
                }
                string name = akNameFirst[iRandomFirst].NameText + akNameMiddle[iRandomMiddle].NameText + akNameSecond[iRandomSecond].NameText;
                mInputField.text = name;
                Logger.LogProcessFormat("OnClickRandomName B name = {0}", name);
            }
            else
            {
                Logger.LogErrorFormat("OnClickRandomName akNameFirst.Count = {0},akNameMiddle = {1},akNameSecond.Count = {2}", akNameFirst.Count, akNameMiddle.Count, akNameSecond.Count);
            }
        }
        #endregion
    }
}