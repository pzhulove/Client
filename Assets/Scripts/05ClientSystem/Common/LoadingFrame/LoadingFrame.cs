using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System;


namespace GameClient
{
    class LoadingFrame : ClientFrame
    {      
        protected int _UpdateSpeed = 10;

        protected StringBuilder strBuilder;
        protected int _targetProgress = 0;
        protected int _currentProgress = -1;

        void LoadTips()
        {
            var targetType = userData as Type;

            if(targetType == typeof(ClientSystemTown))
            {
                var path = LoadingResourceManager.GetRandomCityLoadingRes();
                if(string.IsNullOrEmpty(path) == false)
                {
                    var tips = AssetLoader.instance.LoadResAsGameObject(path);
                    var mLoadingParent = mBind.GetGameObject("LoadingParent");
                    
                    if(mLoadingParent && tips)
                    {
                        tips.transform.SetParent(mLoadingParent.transform,false);

                        SetBackgroundImg(tips);
                    }
                }
            }
            else if(targetType == typeof(ClientSystemBattle))
            {
                var path = LoadingResourceManager.GetRandomDugeonLoadingRes();
                if(string.IsNullOrEmpty(path) == false)
                {
                    var tips = AssetLoader.instance.LoadResAsGameObject(path);
                    var mLoadingParent = mBind.GetGameObject("LoadingParent");
                    
                    if(mLoadingParent && tips)
                    {
                        tips.transform.SetParent(mLoadingParent.transform,false);

                        SetBackgroundImg(tips);
                    }
                }
            }
            else if(targetType == null)
            {

                //var specialPath = AdsPush.AdsPushServerDataManager.GetInstance().GetLimitTimeActivityLoadingPath();
                var specialPath = AdsPush.LoginPushManager.GetInstance().GetLoadingIconPath();
                if (string.IsNullOrEmpty(specialPath) == false)
                {
                    var tips = AssetLoader.instance.LoadResAsGameObject(specialPath);
                    var mLoadingParent = mBind.GetGameObject("LoadingParent");

                    if (mLoadingParent && tips)
                    {
                        tips.transform.SetParent(mLoadingParent.transform, false);
                    }
                }
                else
                {
                    SetBackgroundImg(this.frame);
                }
            }
        }
        protected override void _OnOpenFrame()
        {
            LoadTips();
#if UNITY_EDITOR
            _UpdateSpeed = Global.Settings.loadingProgressStepInEditor;
#else
            _UpdateSpeed = Global.Settings.loadingProgressStep;
#endif

            //GameObject.DontDestroyOnLoad(frame);
            strBuilder = StringBuilderCache.Acquire();
            _targetProgress = 0;
            _currentProgress = -1;
            StartCoroutine(UpdateProgress());

            int MaxNum = TableManager.GetInstance().GetTableItemCount<ProtoTable.TipsTable>();
            int TableIndex = UnityEngine.Random.Range(1, MaxNum);

            var TipsData = TableManager.GetInstance().GetTableItemByIndex<ProtoTable.TipsTable>(TableIndex);
            if(TipsData != null)
            {
                TipsText.text = "温馨提示:" + TipsData.ObjectName;
            }
        }

        protected override void _OnCloseFrame()
        {
            StringBuilderCache.Release(strBuilder);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Loading/LoadingFrame";
        }

        protected override bool _IsLoadingFrame()
        {
            return true;
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
                    //frameMgr.CloseFrame(this);
                    yield return  GameFrameWork.instance.OpenFadeFrame(()=>{
                        frameMgr.CloseFrame(this);
                        GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.SystemLoadingCompelete);
                        },
                        ()=>{
                            GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.SystemLoadingCompelete);
                        }
                        ); 

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
            loadProcess.value = progress / 100.0f;
            strBuilder.Clear();
            strBuilder.AppendFormat("{0}%", progress);
            loadingProgressText.text = strBuilder.ToString();
        }

        protected override bool GetNeedOpenSound()
        {
            return false;
        }

        protected override bool GetNeedCloseSound()
        {
            return false;
        }

        private void SetBackgroundImg(GameObject tips)
        {
            if (tips)
            {
                ComCommonBind bind = tips.GetComponent<ComCommonBind>();
                    
                if (bind == null)
                    return;

                // BgImg这个image根本就没被用上，现在的所有机制都是把需要的背景图单独做成预制体直接挂到loadingframe上的
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

        [UIControl("loading")]
        Slider loadProcess;

        [UIControl("loading/loadingText")]
        Text loadingProgressText;

        [UIControl("loadText")]
        Text TipsText;
    }
}
