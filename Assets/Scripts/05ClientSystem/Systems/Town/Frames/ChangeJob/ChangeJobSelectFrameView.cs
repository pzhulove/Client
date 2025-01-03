using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Network;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ChangeJobSelectFrameView : MonoBehaviour
    {
        [SerializeField] private Text mTextDesp;
        [SerializeField] private Text mTextSubDesp;
        [SerializeField] private List<Image> mImgNames;
        [SerializeField] private CommonRoleAbilityChart mAbility;
        [SerializeField] private Text mTextBtn;
        [SerializeField] private Button mBtnClose;
        [SerializeField] private Text mTextTitle;
        [SerializeField] private Text mTextWeapon;
        [SerializeField] private Text mTextWore;
        [SerializeField] private StateController mStateCtrl;
        [SerializeField] private ComUIListScript mListScript;

        [SerializeField] private GameObject mSpineRoot;
        [SerializeField] private GeObjectRenderer mObjRender;
        private GameObject mObjJobImage;

        [SerializeField] private MediaPlayerCtrl mMovieCtrl;
        [SerializeField] private GameObject mObjNoMovie;
        [SerializeField] private RawImage mImgMovie;
        [SerializeField] private Text mTxtMoive;

        //切换职业还是转职
        private ChangeJobType mChangeType = ChangeJobType.ChangeJobMission;
        private List<int> mJobList = new List<int>();
        private int mSelectJobId;

        public void OnInit(ChangeJobType type)
        {
            mChangeType = type;
            _InitMovieCtrl();
            _SetChangeType();
            _InitJobList();
            _SetJobListData();
        }

        public void OnUninit()
        {
            _UninitMovieCtrl();
            mListScript.UnInitialize();
        }

        private void _SetChangeType()
        {
            if (mChangeType == ChangeJobType.SwitchJob)
            {
                mTextBtn.SafeSetText("切换职业");
                mTextTitle.SafeSetText("切换职业");
                mBtnClose.CustomActive(true);
            }
        }
        
        private void _InitJobList()
        {
            mListScript.Initialize();
            mListScript.onItemVisiable = _OnJobItemShow;
            mListScript.OnItemUpdate = _OnJobItemShow;
        }

        //获取到职业列表并选中默认
        private void _SetJobListData()
        {
            JobTable tableData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if(tableData == null)
            {
                Logger.LogErrorFormat("ChangeJobSelectFrame 初始化失败, JobID = {0}", PlayerBaseData.GetInstance().JobTableID);
                return;
            }
            if(tableData.JobType < 1)
            {
                Logger.LogErrorFormat("ChangeJobSelectFrame 初始化失败,传入职业id是基础职业, JobID = {0}", PlayerBaseData.GetInstance().JobTableID);
                return;
            }
            mJobList.Clear();
            var list = TableManager.GetInstance().GetNextJobsIDByCurJobID(tableData.prejob);
            foreach (int id in list)
            {
                var table = TableManager.GetInstance().GetTableItem<JobTable>(id);
                if (null != table && table.CanCreateRole > 0)
                {
                    mJobList.Add(id);
                }
            }
            if (0 == mJobList.Count)
            {
                Logger.LogErrorFormat("id={0}的职业找不到可创建的子职业", tableData.prejob);
                return;
            }
            if (!mJobList.Contains(PlayerBaseData.GetInstance().JobTableID))
            {
                Logger.LogErrorFormat("职业系中未找到当前的职业");
                return;
            }
            _OnSelectedJob(PlayerBaseData.GetInstance().JobTableID);
        }

        //侧边职业栏列表显示
        private void _OnJobItemShow(ComUIListElementScript item)
        {
            if (null == item)
                return;
            ChangeJobNewItem jobItem = item.GetComponent<ChangeJobNewItem>();
            if (null == jobItem)
                return;
            if (item.m_index < 0 || item.m_index >= mJobList.Count)
                return;
            jobItem.OnItemVisiable(mJobList[item.m_index], _OnSelectedJob, mSelectJobId == mJobList[item.m_index]);
            jobItem.ShowCurJob(mChangeType == ChangeJobType.SwitchJob && mJobList[item.m_index] == PlayerBaseData.GetInstance().JobTableID);
        }
        
        //选择职业
        private void _OnSelectedJob(int jobID)
        {
            //重复选择直接退
            if (mSelectJobId == jobID)
                return;
            mSelectJobId = jobID;
            mListScript.SetElementAmount(mJobList.Count);
            _ShowJobInfo();
        }

        //展示职业相关信息
        private void _ShowJobInfo()
        {
            JobTable tableData = TableManager.GetInstance().GetTableItem<JobTable>(mSelectJobId);
            if (null == tableData)
            {
                Logger.LogErrorFormat("职业表中找不到id={0}的职业", mSelectJobId);
                return;
            }
            //描述
            mTextDesp.text = TR.Value("creat_role_character_desc", tableData.RecommendedAttribute);
            //子描述
            mTextSubDesp.text = tableData.JobDes[0];
            //武器
            mTextWeapon.SafeSetText(tableData.RecWeapon);
            //衣服
            mTextWore.SafeSetText(tableData.RecDefence);
            //名字
            int index = 0;
            for(; index < tableData.JobNameImgPaths.Length; ++index)
            {
                if (index >= mImgNames.Count)
                {
                    Logger.LogErrorFormat("转职界面职业名字过长 预制体中准备字体不足");
                    break;
                }
                mImgNames[index].CustomActive(true);
                mImgNames[index].SafeSetImage(tableData.JobNameImgPaths[index]);
            }
            for(; index < mImgNames.Count; ++index)
            {
                mImgNames[index].CustomActive(false);
            }
            //能力值
            mAbility._OnRefreshAbilityChartList(mSelectJobId);
            //视频
            _PlayMovie(tableData.Video);
            //动画
            if (mObjJobImage != null)
            {
                GameObject.DestroyImmediate(mObjJobImage);
                mObjJobImage = null;
            }
            if (tableData.JobImage.Contains("Animation"))
            {
                _ShowModule(tableData.ID, tableData.JobImage);
                mSpineRoot.SetActive(false);

                ClientSystemManager.GetInstance().delayCaller.DelayCall(100, () =>
                {
                    mSpineRoot.CustomActive(true);
                });
            }
            else
            {
                _HideModule();
                mObjJobImage = AssetLoader.instance.LoadResAsGameObject(tableData.JobImage);
                if (mObjJobImage != null && mSpineRoot != null)
                {
                    mSpineRoot.CustomActive(true);
                    Utility.AttachTo(mObjJobImage, mSpineRoot);

                    mObjJobImage.transform.SetAsFirstSibling();
                }
            }
            //按钮/提示显示
            if (mChangeType == ChangeJobType.SwitchJob && mSelectJobId == PlayerBaseData.GetInstance().JobTableID)
            {
                mStateCtrl.Key = "curjob";
            }
            else
            {
                mStateCtrl.Key = "changejob";
            }
        }
        void _ShowModule(int jobID, string path)
        {
            if (mObjRender != null)
            {
                mObjRender.gameObject.CustomActive(true);
                mObjRender.ClearObject();

                try
                {
                    mObjRender.LoadObject(path, 28);
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
            }
        }

#region 视频相关
        private void _InitMovieCtrl()
        {
            if (mMovieCtrl != null)
            {
                mMovieCtrl.OnReady += _OnVideoReady;
                mMovieCtrl.OnVideoError += _OnVideoError;
            }
#if UNITY_ANDROID
			if (mImgMovie != null)
			{
				var lscale = mImgMovie.transform.localScale;
				lscale.y = -1;
				mImgMovie.transform.localScale = lscale;
			}
#endif
        }
        private void _OnVideoReady()
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
        private void _OnVideoError(MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCode, MediaPlayerCtrl.MEDIAPLAYER_ERROR errorCodeExtra)
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
        private void _PlayMovie(string path)
        {
            if (mMovieCtrl != null)
            {
                _StopMovie();
                mObjNoMovie.CustomActive(string.IsNullOrEmpty(path) || path.Equals("0"));
                if (string.IsNullOrEmpty(path) || path.Equals("0"))
                {
                    mMovieCtrl.CustomActive(false);
                    return;
                }
                else
                {
                    mMovieCtrl.CustomActive(true);
                }
                mMovieCtrl.Load(path);
                if (mTxtMoive != null)
                {
                    mTxtMoive.CustomActive(false);
                }
            }
        }
        private void _StopMovie()
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
        private void _UninitMovieCtrl()
        {
            _StopMovie();
            if (mMovieCtrl != null)
            {
                mMovieCtrl.OnReady -= _OnVideoReady;
                mMovieCtrl.OnVideoError -= _OnVideoError;
            }
        }
#endregion

        //点击 转职/切换职业 按钮
        public void OnClickChangeJob()
        {
            JobTable table = TableManager.GetInstance().GetTableItem<JobTable>(mSelectJobId);

            if (table == null)
            {
                Logger.LogErrorFormat("not get JobTable [tableID is {0}]", mSelectJobId);
            }

            if(mChangeType == ChangeJobType.ChangeJobMission)
            {
                // 转职任务选同一职业也是可以的
                string sContent = TR.Value("changejob_tip", table.Name);
                SystemNotifyManager.SysNotifyMsgBoxOkCancel(sContent, _ClickCommit);
            }
            else
            {
                // 切换职业禁止选同一职业
                if(table.ID == PlayerBaseData.GetInstance().JobTableID)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("请选择不同职业");
                    return;
                }

                if(PlayerBaseData.GetInstance().Level > 30)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("30级后无法再切换职业");
                    return;
                }

                var itemId = Utility.GetSystemIntValueByType1(SystemValueTable.eType.SVT_CHANGE_OCCU_CONSUME_ID, 600000007);
                var itemNum = Utility.GetSystemIntValueByType1(SystemValueTable.eType.SVT_CHANGE_OCCU_CONSUME_NUM, 100);
                var costInfo = new CostItemManager.CostInfo()
                {
                    nMoneyID = itemId,
                    nCount = itemNum,
                };
                CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, _ReqChangeJob);
            }
        }

        private void _ReqChangeJob()
        {
            JobTable table = TableManager.GetInstance().GetTableItem<JobTable>(mSelectJobId);
            SystemNotifyManager.SysNotifyMsgBoxOkCancel(TR.Value("changejob_cost_tip", table.Name), _ClickChangeJob);
        }

        private void _ClickChangeJob()
        {
            SceneChangeOccu req = new SceneChangeOccu();
            req.occu = (byte)mSelectJobId;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
            ClientSystemManager.GetInstance().CloseFrame<ChangeJobSelectFrame>();
            ClientSystemManager.GetInstance().CloseFrame<SkillFrame>();
        }

        //转职任务提交
        private void _ClickCommit()
        {
            MissionManager.GetInstance().AcceptChangeJobMissions(mSelectJobId);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChangeJobSelectDialog, mSelectJobId);
            ClientSystemManager.GetInstance().CloseFrame<ChangeJobSelectFrame>();
        }

        //体验职业按钮
        public void OnClickExprience()
        {
            BeUtility.StartChangeOccuBattle(mSelectJobId);
        }
    }
}
