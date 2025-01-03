using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{
    class DungeonLoadingFrame : ClientFrame
    {
        protected StringBuilder strBuilder;
        protected int _UpdateSpeed = 10;
        protected int _targetProgress = 0;
        protected int _currentProgress = -1;

        protected GameObject _tips = null;

#region ExtraUIBind
        private GameObject mLoadingParent = null;
        private Slider mLoadProcess = null;
        private Text mLoadText = null;
        private Text mTipsText = null;
        private GameObject mBackRoot = null;
        private Image mBack = null;
        private ComDungeonPlayerLoadProgress[] mOthers = new ComDungeonPlayerLoadProgress[2];
        private Text mLevel = null;
        private Image mIcon = null;
        private Text mName = null;
        private GameObject mTeamRoot = null;
        private Slider mSingleLoadProcess = null;
        private GameObject mSingleRoot = null;
        private ReplaceHeadPortraitFrame mReplaceHeadPortraitFrame = null;

        protected override void _bindExUI()
        {
            mLoadingParent = mBind.GetGameObject("LoadingParent");
            mLoadProcess = mBind.GetCom<Slider>("loadProcess");
            mLoadText = mBind.GetCom<Text>("loadText");
            mTipsText = mBind.GetCom<Text>("TipsText");
            mBackRoot = mBind.GetGameObject("backRoot");
            mBack = mBind.GetCom<Image>("back");
            mOthers[0] = mBind.GetCom<ComDungeonPlayerLoadProgress>("other0");
            mOthers[1] = mBind.GetCom<ComDungeonPlayerLoadProgress>("other1");

            mLevel = mBind.GetCom<Text>("level");
            mIcon = mBind.GetCom<Image>("icon");
            mName = mBind.GetCom<Text>("name");

            mTeamRoot = mBind.GetGameObject("teamRoot");
            mSingleLoadProcess = mBind.GetCom<Slider>("singleLoadProcess");
            mSingleRoot = mBind.GetGameObject("singleRoot");
            mReplaceHeadPortraitFrame = mBind.GetCom<ReplaceHeadPortraitFrame>("ReplaceHeadPortraitFrame");
        }

        protected override void _unbindExUI()
        {
            mLoadingParent = null;
            mLoadProcess = null;
            mLoadText = null;
            mTipsText = null;
            mBackRoot = null;
            mBack = null;
            mOthers[0] = null;
            mOthers[1] = null;

            mLevel = null;
            mIcon = null;
            mName = null;

            mTeamRoot = null;
            mSingleLoadProcess = null;
            mSingleRoot = null;
            mReplaceHeadPortraitFrame = null;
        }
#endregion   

        protected override bool _IsLoadingFrame()
        {
            return true;
        }

        void LoadTips()
        {
            var path = LoadingResourceManager.GetRandomDugeonLoadingRes();
            if (string.IsNullOrEmpty(path) == false)
            {
                if (null != _tips)
                    GameObject.Destroy(_tips);
                _tips = AssetLoader.instance.LoadResAsGameObject(path);
                var mLoadingParent = mBind.GetGameObject("LoadingParent");

                if (mLoadingParent && _tips)
                {
                    _tips.transform.SetParent(mLoadingParent.transform, false);

                    SetBackgroundImg(_tips);
                }
            }
        }

        private static int sIndex = 0;

        protected override void _OnOpenFrame()
        {
            LoadTips();
            
#if UNITY_EDITOR
            _UpdateSpeed = Global.Settings.loadingProgressStepInEditor;
#else
            _UpdateSpeed = Global.Settings.loadingProgressStep;
#endif

            GameObject.DontDestroyOnLoad(frame);
            strBuilder = StringBuilderCache.Acquire();
            _targetProgress = 0;
            _currentProgress = -1;
            StartCoroutine(UpdateProgress());

            _loadDungeonLoadingMap();
            _updateBackground();

            int MaxNum = TableManager.GetInstance().GetTableItemCount<ProtoTable.TipsTable>();

            sIndex += UnityEngine.Random.Range(1, MaxNum);
            sIndex *= 0x1234;
            sIndex %= MaxNum;
            sIndex++;

            var TipsData = TableManager.GetInstance().GetTableItemByIndex<ProtoTable.TipsTable>(sIndex);
            if (TipsData != null)
            {
                mTipsText.text = "温馨提示:" + TipsData.ObjectName;
            }

            if (userData != null)
            {
                GameObject obj = null;
                string path = "";
                int jobid = (int)userData;

                ProtoTable.JobTable jobTable = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(jobid);
                
                if(jobTable != null && jobTable.JobType == 0)
                {
                    path = jobTable.LoadingPrefab;
                }

                obj = AssetLoader.instance.LoadResAsGameObject(path);

                if (obj != null && mBackRoot != null)
                    Utility.AttachTo(obj, mBackRoot);
            }
            else
            {
                var path = LoadingResourceManager.GetRandomDugeonLoadingRes();
                if (string.IsNullOrEmpty(path) == false)
                {
                    var tips = AssetLoader.instance.LoadResAsGameObject(path);
                    if (mLoadingParent && tips)
                    {
                        tips.transform.SetParent(mLoadingParent.transform, false);

                        SetBackgroundImg(tips);
                    }
                }
            }

            _setAllOtherPlayerSeat();

            _loadMainPlayer();

            _updateModeByDungeonMode();
        }

        private void _updateModeByDungeonMode()
        {
            mTeamRoot.SetActive(_isTeamFight());
            mSingleRoot.SetActive(!_isTeamFight());

            if (BattleMain.instance == null)
                return;

            var mRaidBattle = BattleMain.instance.GetBattle() as RaidBattle;
            if (mRaidBattle != null)
            {
                mTeamRoot.SetActive(true);
                mSingleRoot.SetActive(false);
            }
        }

        private bool _isTeamFight()
        {
            if (null == TeamDataManager.GetInstance())
            {
                return false;
            }

            return TeamDataManager.GetInstance().HasTeam();
        }

        private void _loadMainPlayer()
        {
			if (BattleMain.instance == null)
				return;


            BattlePlayer mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();

            if (null == mainPlayer)
            {
                return ;
            }

            mName.text = mainPlayer.playerInfo.name;
            mLevel.text = mainPlayer.playerInfo.level.ToString();
            // mIcon.sprite = _getMainPlayerSprite((int)mainPlayer.playerInfo.occupation);
            _getMainPlayerSprite(ref mIcon, (int)mainPlayer.playerInfo.occupation);

            if (mReplaceHeadPortraitFrame != null)
            {
                if (mainPlayer.playerInfo.playerLabelInfo.headFrame != 0)
                {
                    mReplaceHeadPortraitFrame.ReplacePhotoFrame((int)mainPlayer.playerInfo.playerLabelInfo.headFrame);
                }
                else
                {
                    mReplaceHeadPortraitFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
                }
            }
        }

        private void _getMainPlayerSprite(ref Image image, int id)
        {
            JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(id);
            if (null == jobData)
            {
                return;
            }

            ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
            if (null == resData)
            {
                return;
            }

            // return AssetLoader.instance.LoadRes(resData.IconPath, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref image, resData.IconPath);
        }

        private void _setAllOtherPlayerSeat()
        {
			if (BattleMain.instance == null)
				return;

            List<BattlePlayer> battlePlayers = BattleMain.instance.GetPlayerManager().GetAllPlayers();

            int idx = 0;

            for (int i = 0; i < battlePlayers.Count; ++i)
            {
                if (battlePlayers[i].playerInfo.accid != ClientApplication.playerinfo.accid)
                {
                    if (idx < mOthers.Length)
                    {
                        mOthers[idx++].SetSeat(battlePlayers[i].playerInfo.seat);
                    }
                }
            }

            for (int i = idx; i < mOthers.Length; ++i)
            {
                mOthers[i].gameObject.SetActive(false);
            }
        }

        private void _updateBackground()
        {
            var dungeonID = BattleDataManager.GetInstance().BattleInfo.dungeonId;
            var table = TableManager.instance.GetTableItem<ProtoTable.DungeonTable>(dungeonID);
            if (table != null)
            {
                // mBack.sprite = AssetLoader.instance.LoadRes(table.LoadingBgPath, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref mBack, table.LoadingBgPath);
            }
        }

        string dName;
        private void _loadDungeonLoadingMap()
        {
            var dungeonID = BattleDataManager.GetInstance().BattleInfo.dungeonId;
            var table = TableManager.instance.GetTableItem<ProtoTable.DungeonTable>(dungeonID);
            if (table != null)
            {
                var data = AssetLoader.instance.LoadRes(table.DungeonLoadingConfig).obj as DDungeonMapData;
                if (data == null)
                {
                    Logger.LogWarningFormat("dungeon loading config data is nil with path {0}", table.DungeonLoadingConfig);
                }
                else
                {
                    //mLoadMap.SetDungeonMap(data);
                    //mLoadMap.SetDungeonID(dungeonID);
                }

                dName = table.Name;

            }
            else
            {
                //Logger.LogErrorFormat("table is nil with id : {0}", dungeonID);
            }
        }

        protected override void _OnCloseFrame()
        {
            StringBuilderCache.Release(strBuilder);
            mIcon.sprite = null;
            mIcon.material = null;
            mBack.sprite = null;
            mBack.material = null;

            if(null != _tips)
            {
                GameObject.Destroy(_tips);
                _tips = null;
            }
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Loading/DungeonLoadingFrame";
        }

        public IEnumerator UpdateProgress()
        {
            while (_targetProgress <= 100)
            {
                while (_currentProgress < _targetProgress)
                {
                    _currentProgress += _UpdateSpeed;
                    if (_currentProgress > _targetProgress)
                    {
                        _currentProgress = _targetProgress;
                    }

                    _SetProgress(_currentProgress);
                    yield return Yielders.EndOfFrame;
                }

                if (_targetProgress == 100)
                {
                    yield return GameFrameWork.instance.OpenFadeFrame(() => { frameMgr.CloseFrame(this); }, null);
                    GameFrameWork.instance.TownNameShow(dName);
                    break;
                }

                yield return Yielders.EndOfFrame;

                _targetProgress = (int)(ClientSystemManager.GetInstance().SwitchProgress * 100.0f);
            }
        }

        protected void _SetProgress(int progress)
        {
            if (progress < 0)
            {
                progress = 0;
            }
            if (progress > 100)
            {
                progress = 100;
            }
            mLoadProcess.value = progress / 100.0f;
            mSingleLoadProcess.value = mLoadProcess.value;
            strBuilder.Clear();
            strBuilder.AppendFormat("{0}%", progress);
            mLoadText.text = strBuilder.ToString();
        }

        private void SetBackgroundImg(GameObject tips)
        {
            if (tips)
            {
                ComCommonBind bind = tips.GetComponent<ComCommonBind>();
                if (bind == null)
                    return;

                Image bgImg = bind.GetCom<Image>("BgImg");
                if (bgImg != null)
                {
                    string imgPath = PluginManager.GetSDKLogoPath(SDKInterface.SDKLogoType.LoadingLogo);
                    if (string.IsNullOrEmpty(imgPath))
                        return;
                    ETCImageLoader.LoadSprite(ref bgImg, imgPath);
                }
            }
        }
    }
}
