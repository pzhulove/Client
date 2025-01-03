using UnityEngine.UI;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;
using System.Collections.Generic;

namespace GameClient
{
    public enum PetTipsType
    {
        None = 0,
        PetItemTip,
        OnUsePetTip,
    }

    public class PetItemTipsData
    {
        public PetTipsType petTipsType; 
        public PetInfo petinfo;
        public int PlayerJobID;
        public bool bFunc;

        public void ClearData()
        {
            petTipsType = PetTipsType.PetItemTip;
            petinfo = null;
            PlayerJobID = 0;
            bFunc = false;
        }
    }

    class PetItemTipsFrame : ClientFrame
    {
        const int MaxStarNum = 5;

        PetItemTipsData PetTipsData = new PetItemTipsData();
             
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pet/PetItemTips";
        }

        [UIObject("Func")]
        GameObject goFunc;
        [UIObject("mark")]
        GameObject goTake;

        protected override void _OnOpenFrame()
        {
            if(userData == null)
            {
                return;
            }

            PetTipsData = userData as PetItemTipsData;

            InitInterface();
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
        }

        void ClearData()
        {
            PetTipsData.ClearData();
        }

        void InitInterface()
        {
            PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)PetTipsData.petinfo.dataId);
            if (petData == null)
            {
                return;
            }

            goFunc.CustomActive(PetTipsData.bFunc);
            if(PetTipsData.bFunc)
            {
                if (PetTipsData.petTipsType == PetTipsType.PetItemTip)
                {
                    mBtWear.gameObject.CustomActive(true);

                    if(ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle != null)
                    {
                        mBtUpLevel.gameObject.CustomActive(false);
                    }
                    else
                    {
                        mBtUpLevel.gameObject.CustomActive(true);
                    }
                }
                else if (PetTipsData.petTipsType == PetTipsType.OnUsePetTip)
                {
                    mSpecialText.text = "卸下";
                    mBtWear.gameObject.CustomActive(true);
                    if (ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle != null)
                    {
                        mBtUpLevel.gameObject.CustomActive(false);
                    }
                    else
                    {
                        mBtUpLevel.gameObject.CustomActive(true);
                    }
                }
            }

            bool bToken = null != PetDataManager.GetInstance().GetOnUsePetList().Find(x =>
            {
                return x.id == PetTipsData.petinfo.id;
            });
            goTake.CustomActive(bToken);

            mName.text = PetDataManager.GetInstance().GetColorName(petData.Name, petData.Quality);
            mSatiety.text = PetTipsData.petinfo.hunger.ToString();
            mLevel.text = PetTipsData.petinfo.level.ToString();
            mQuality.text = PetDataManager.GetInstance().GetQualityDesc(petData.Quality);
            mPetType.text = PetDataManager.GetInstance().GetPetTypeDesc(petData.PetType);
            mScore.text = PetTipsData.petinfo.petScore.ToString();

            if (!string.IsNullOrEmpty(petData.IconPath) && petData.IconPath != "-")
            {
                // mIcon.sprite = AssetLoader.instance.LoadRes(petData.IconPath, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref mIcon, petData.IconPath);
            }

           // string tittleBg = PetDataManager.GetInstance().GetQualityTipTitleBackGround(petData.Quality);
           // if (!string.IsNullOrEmpty(tittleBg) && tittleBg != "-")
            {
                // mIcon.sprite = AssetLoader.instance.LoadRes(petData.IconPath, typeof(Sprite)).obj as Sprite;
                //ETCImageLoader.LoadSprite(ref mTitleBg, tittleBg);
                if(mTitleBg != null)
                {
                    mTitleBg.color = PetDataManager.GetInstance().GetQualityTipTitleBackGroundByColor(petData.Quality);
                }
            }


            string sIconBack = PetDataManager.GetInstance().GetQualityIconBack(petData.Quality);
            if (!string.IsNullOrEmpty(sIconBack) && sIconBack != "-")
            {
                // mIconBack.sprite = AssetLoader.instance.LoadRes(sIconBack, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref mIconBack, sIconBack);
            }
            if (PetTipsData.petTipsType != PetTipsType.None)
            {
                if (ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle != null)
                {
                    mBtProperty.gameObject.CustomActive(false);
                }
                else
                {
                    mBtProperty.gameObject.CustomActive(true);
                }
            }

            int iShowStarNum = PetDataManager.GetInstance().ShowPetHalfStarNum(PetTipsData.petinfo.level);
            for(int i = 0; i < stars.Length; i++)
            {
                if(i < iShowStarNum)
                {
                    stars[i].gameObject.CustomActive(true);
                }
                else
                {
                    stars[i].gameObject.CustomActive(false);
                }
            }

            if (PetTipsData.petinfo.skillIndex <= petData.Skills.Count)
            {
                mProperty.text = PetDataManager.GetInstance().GetPetPropertyTips(petData, PetTipsData.petinfo.level);

                if (petData.PetType == PetTable.ePetType.PT_ATTACK)
                {
                    mAttrDes.text = "技能选择";
                }
                else
                {
                    mAttrDes.text = "属性选择";
                }

                mCurSkill.text = PetDataManager.GetInstance().GetPetCurSkillTips(petData, PetTipsData.PlayerJobID, PetTipsData.petinfo.skillIndex, PetTipsData.petinfo.level);
                mSelSkill.text = PetDataManager.GetInstance().GetCanSelectSkillTips(petData, PetTipsData.PlayerJobID, PetTipsData.petinfo.skillIndex, PetTipsData.petinfo.level);
            }                
        }

        void ClickSellOK()
        {
            SendSellPetReq();
            frameMgr.CloseFrame(this);
        }

        void SendWearPetReq()
        {
            if(PetTipsData.petinfo.id <= 0)
            {
                return;
            }

            PetTable petdata = TableManager.GetInstance().GetTableItem<PetTable>((int)PetTipsData.petinfo.dataId);
            if(petdata == null)
            {
                return;
            }

            SceneSetPetSoltReq req = new SceneSetPetSoltReq();       
            req.petType = (byte)petdata.PetType;

            if(PetTipsData.petTipsType == PetTipsType.OnUsePetTip)
            {
                req.petId = 0;
            }
            else
            {
                req.petId = PetTipsData.petinfo.id;
            }

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void SendSellPetReq()
        {
            if(PetTipsData.petinfo.id <= 0)
            {
                return;
            }

            SceneSellPetReq req = new SceneSellPetReq();
            req.id = PetTipsData.petinfo.id;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        [UIControl("BG/InfoView/ViewPort/Content/IconRoot/stars/star{0}", typeof(Image), 1)]
        Image[] stars = new Image[MaxStarNum * 2];

        #region ExtraUIBind
        private Button mBtWear = null;
        private Text mName = null;
        private Image mIcon = null;
        private Button mBtUpLevel = null;
        private Button mBtProperty = null;
        private Text mLevel = null;
        private Text mQuality = null;
        private Text mPetType = null;
        private Image mIconBack = null;
        private Image mTitleBack = null;
        private Text mProperty = null;
        private Text mSatiety = null;
        private Text mSpecialText = null;
        private Text mCurSkill = null;
        private Image mTitleBg = null;
        private Text mSelSkill = null;
        private Text mScore = null;
        private Text mAttrDes = null;

        protected override void _bindExUI()
        {
            mBtWear = mBind.GetCom<Button>("btWear");
            mBtWear.onClick.AddListener(_onBtWearButtonClick);
            mName = mBind.GetCom<Text>("Name");
            mIcon = mBind.GetCom<Image>("Icon");
            mBtUpLevel = mBind.GetCom<Button>("btUpLevel");
            mBtUpLevel.onClick.AddListener(_onBtUpLevelButtonClick);
            mBtProperty = mBind.GetCom<Button>("btProperty");
            mBtProperty.onClick.AddListener(_onBtPropertyButtonClick);
            mLevel = mBind.GetCom<Text>("Level");
            mQuality = mBind.GetCom<Text>("Quality");
            mPetType = mBind.GetCom<Text>("PetType");
            mIconBack = mBind.GetCom<Image>("IconBack");
            mTitleBack = mBind.GetCom<Image>("TitleBack");
            mProperty = mBind.GetCom<Text>("Property");
            mSatiety = mBind.GetCom<Text>("Satiety");
            mSpecialText = mBind.GetCom<Text>("SpecialText");
            mCurSkill = mBind.GetCom<Text>("CurSkill");
            mTitleBg = mBind.GetCom<Image>("TitleBg");
            mSelSkill = mBind.GetCom<Text>("SelSkill");
            mScore = mBind.GetCom<Text>("Score");
            mAttrDes = mBind.GetCom<Text>("AttrDes");
        }

        protected override void _unbindExUI()
        {
            mBtWear.onClick.RemoveListener(_onBtWearButtonClick);
            mBtWear = null;
            mName = null;
            mIcon = null;
            mBtUpLevel.onClick.RemoveListener(_onBtUpLevelButtonClick);
            mBtUpLevel = null;
            mBtProperty.onClick.RemoveListener(_onBtPropertyButtonClick);
            mBtProperty = null;
            mLevel = null;
            mQuality = null;
            mPetType = null;
            mIconBack = null;
            mTitleBack = null;
            mProperty = null;
            mSatiety = null;
            mSpecialText = null;
            mCurSkill = null;
            mTitleBg = null;
            mSelSkill = null;
            mScore = null;
            mAttrDes = null;
        }
        #endregion

        #region Callback
        private void _onBtWearButtonClick()
        {
            SendWearPetReq();
            //frameMgr.CloseFrame(this);

            ItemTipManager.GetInstance().CloseAll();
        }

        private void _onBtUpLevelButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<PetUpgradeFrame>(FrameLayer.Middle, PetTipsData.petinfo);
            //frameMgr.CloseFrame(this);

            ItemTipManager.GetInstance().CloseAll();
        }

        private void _onBtPropertyButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<PetPropertyFrame>(FrameLayer.Middle, PetTipsData.petinfo);
            //frameMgr.CloseFrame(this);

            ItemTipManager.GetInstance().CloseAll();
        }
        #endregion
    }
}

