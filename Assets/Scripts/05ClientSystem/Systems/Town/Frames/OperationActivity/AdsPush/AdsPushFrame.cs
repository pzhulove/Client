//using UnityEngine;
//using System.Collections;
//using UnityEngine.UI;
//using AdsPush;

//namespace GameClient
//{
//    public class AdsPushFrame : ClientFrame
//    {
//        private GameObject adsItemParent;

//        private AdsPushData currPushData;
//        private AdsPushItem currPushItem;
//        public bool IsDestroy { get; set; }

//        private const string prefabPath = "UIFlatten/Prefabs/OperateActivity/PushAds/AdsPushFrame";
//        public override string GetPrefabPath()
//        {
//            return prefabPath;
//        }

//        protected override void _bindExUI()
//        {
//            adsItemParent = mBind.GetGameObject("AdsItemParent");
//        }

//        protected override void _unbindExUI()
//        {
//            adsItemParent = null;
//        }

//        protected override void _OnOpenFrame()
//        {
//            IsDestroy = false;
//        }

//        protected override void _OnCloseFrame()
//        {
//            IsDestroy = true;
//            if (currPushItem != null)
//                currPushItem.Destory();
//        }

//        #region PUBLIC METHODS

//        public void SetAdsDataForThis(AdsPushData data)
//        {
//            this.currPushData = data;
//            if (currPushData != null)
//            {
//                currPushItem = new AdsPushItem();
//                currPushItem.Init(adsItemParent, this, currPushData);
//            }
//        }

//        public AdsPushItem GetCurrFrameItem()
//        {
//            return currPushItem;
//        }

//        #endregion
//    }

//    public class AdsPushItem
//    {
//        public static string PrefabPath = "UIFlatten/Prefabs/OperateActivity/PushAds/AdsItem";
//        protected GameObject goParent;
//        protected GameObject goSelf;
//        protected AdsPushData adsData;
//        protected AdsPushFrame adsFrame;

//        protected ComCommonBind mBind;

//        private Button KnowBtn;
//        private Button GoKnonBtn;
//        private Button ImgBtnCom;
//        private Image ImgSprite;
//        private GameObject ImgLoadingGo;
//        private GameObject GoKnonBtnGo;
//        private GameObject KnowBtnGo;

//        public AdsPushType currItemType { get; private set; }

//        public void Init(GameObject parent, AdsPushFrame frame, AdsPushData adsData)
//        {
//            this.goParent = parent;
//            this.adsFrame = frame;
//            this.adsData = adsData;
//            this.currItemType = AdsPushType.None;
//            this.goSelf = AssetLoader.instance.LoadResAsGameObject(PrefabPath);
//            if (goSelf != null)
//            {
//                mBind = goSelf.GetComponent<ComCommonBind>();
//            }
//            if (frame != null && mBind != null)
//            {
//                ImgSprite = mBind.GetCom<Image>("ContentImg");

//                KnowBtn = mBind.GetCom<Button>("KnowBtn");
//                KnowBtn.onClick.RemoveAllListeners();
//                KnowBtn.onClick.AddListener(OnKnowBtnClick);
//                GoKnonBtn = mBind.GetCom<Button>("GoKnowBtn");
//                GoKnonBtn.onClick.RemoveAllListeners();
//                GoKnonBtn.onClick.AddListener(OnGoKnowBtnClick);
//                ImgBtnCom = mBind.GetCom<Button>("ContentBtn");
//                ImgBtnCom.onClick.RemoveAllListeners();
//                ImgBtnCom.onClick.AddListener(OnGoKnowBtnClick);

//                ImgLoadingGo = mBind.GetGameObject("ContentLoading");
//                GoKnonBtnGo = mBind.GetGameObject("GoKnowBtnGo");
//                KnowBtnGo = mBind.GetGameObject("KnowBtnGo");

//                ImgLoadingGo.CustomActive(true);
//            }
//            Utility.AttachTo(goSelf, goParent);

//            SetDataToView();
//        }

//        private void SetDataToView()
//        {
//            if (adsData == null || adsData.AdsPushRawData == null)
//                return;
//            if (adsData.Type == AdsPushType.Local_NoUrl)
//            {
//                ShowTypeNoUrl();
//                currItemType = AdsPushType.Local_NoUrl;
//            }
//            else if (adsData.Type == AdsPushType.Local_WithUrl)
//            {
//                ShowTypeHasUrl();
//                currItemType = AdsPushType.Local_WithUrl;
//            }
//            if (ImgSprite != null)
//            {
//                // GameFrameWork.instance.StartCoroutine(DownloadImg(adsData.AdsPushRawData.AdsImgPath));
//                LoadLocalImg(adsData.AdsPushRawData.AdsImgPath);//from xzl change
//                //LoadLocalImg(LoginPushManager.);
//            }
//        }

//        public void Destory()
//        {
//            if (goSelf != null)
//                GameObject.Destroy(goSelf);
//            Reset();
//        }

//        public void Reset()
//        {
//            this.goParent = null;
//            this.adsFrame = null;
//            this.adsData = null;

//            if (KnowBtn)
//                KnowBtn.onClick.RemoveAllListeners();
//            KnowBtn = null;
//            if (GoKnonBtn)
//                GoKnonBtn.onClick.RemoveAllListeners();
//            GoKnonBtn = null;
//            if (ImgBtnCom)
//                ImgBtnCom.onClick.RemoveAllListeners();
//            ImgBtnCom = null;
//            ImgSprite = null;
//            ImgLoadingGo = null;
//            GoKnonBtnGo = null;
//            KnowBtnGo = null;
//        }

//        private void OnImgBtnClick()
//        {
//            if (adsFrame != null)
//                adsFrame.Close();
//        }

//        private void OnKnowBtnClick()
//        {
//            if (adsFrame != null)
//                adsFrame.Close();
//            AdsPushFrameManager.instance.TryStopOpenAdsFrame();

//        }

//        private void OnGoKnowBtnClick()
//        {
//            if (adsFrame != null)
//                adsFrame.Close();
//            AdsPushFrameManager.instance.TryStopOpenAdsFrame();
//            if (adsData != null)
//            {
//                if (adsData.Type == AdsPushType.Local_WithUrl)
//                {
//                    var url = adsData.AdsPushRawData.AdsContentUrl;
//                    if (!string.IsNullOrEmpty(adsData.AdsPushRawData.AdsContentUrl))
//                    {
//                        ActiveManager.GetInstance().OnClickLinkInfo(url);
//                    }
//                }
//            }
//        }

//        private void ShowTypeHasUrl()
//        {
//            //if (KnowBtnGo != null)
//            //    KnowBtnGo.CustomActive(false);
//            if (GoKnonBtnGo != null)
//                GoKnonBtnGo.CustomActive(true);
//            //if (ImgBtnCom)
//            //    ImgBtnCom.enabled = true;
//        }

//        private void ShowTypeNoUrl()
//        {
//            //if (KnowBtnGo != null)
//            //    KnowBtnGo.CustomActive(true);
//            if (GoKnonBtnGo != null)
//                GoKnonBtnGo.CustomActive(false);
//            //if (ImgBtnCom)
//            //    ImgBtnCom.enabled = false;
//        }

//        void LoadLocalImg(string imgPath)
//        {
//            Sprite spr = AssetLoader.instance.LoadRes(imgPath, typeof(Sprite), false).obj as Sprite;
//            if (ImgSprite != null)
//            {
//                ImgSprite.sprite = spr;
//            }
//            ImgLoadingGo.CustomActive(false);
//        }
//    }

//}