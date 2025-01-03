using UnityEngine.UI;
using Scripts.UI;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;

namespace GameClient
{
    class OpenPetEggFrame : ClientFrame
    {
        string AttachPointPath = "Dummy";

        ItemData PetEggData = null;
        GameObject OpenEffectObj = null;
        GeAnimDescProxy AnimControl = null;

        bool bInitActor = false;
        float fTimeIntval = 0.0f;
        float fTimeValue = 0.80f;

        bool bIsFirstClick = true;

        float SpecialActionTimeLen = 0.0f;
        bool bStartIdleAction = false;
        float SpecialActionTimeIntrval = 0.0f;

        protected BeTownPet m_BeTownPet = null;
        protected PetTable m_PetTableData = null;
        protected GameObject m_PetEntityRoot = null;

        private string[] eggNames = new[]
            {"Skill_UI_Eggs_blue_kaidan", "Skill_UI_Eggs_pink_kaidan", "Skill_UI_Eggs_golden_kaidan"};

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pet/OpenPetEggFrame";
        }

        protected override void _OnOpenFrame()
        {
            if(userData == null)
            {
                return;
            }

            PetEggData = (ItemData)userData;

            InitInterface();
            BindUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            UnBindUIEvent();
            ClearData();
            if (m_BeTownPet != null)
            {
                m_BeTownPet.Dispose();
                m_BeTownPet = null;
            }

            //重置下层级
            if (m_PetEntityRoot != null)
            {
                m_PetEntityRoot.SetLayer(0);
            }
        }

        void ClearData()
        {
            OpenEffectObj = null;
            AnimControl = null;
            mAvatarRenderer.ClearAvatar();
            bInitActor = false;
            fTimeIntval = 0.0f;
            bIsFirstClick = true;
            SpecialActionTimeLen = 0.0f;
            bStartIdleAction = false;
            SpecialActionTimeIntrval = 0.0f;
        }

        protected void BindUIEvent()
        {     
        }

        protected void UnBindUIEvent()
        {        
        }

        void InitInterface()
        {
            string realPath = "_NEW_RESOURCES/Actor/Pet/Pet_ChongWuDan/Prefabs/";

            if (PetEggData.Quality <= ItemTable.eColor.BLUE)
            {
                realPath = realPath + eggNames[0];
            }
            else if(PetEggData.Quality == ItemTable.eColor.PURPLE)
            {
                realPath = realPath + eggNames[1];
            }
            else
            {
                realPath = realPath + eggNames[2];
            }

            OpenEffectObj = AssetLoader.instance.LoadResAsGameObject(realPath);

            if (OpenEffectObj != null)
            {
                Utility.AttachTo(OpenEffectObj, mEffectPos);
            }
        }

        void UpdateActor(int iPetID)
        {
            PetTable Pet = TableManager.instance.GetTableItem<PetTable>(iPetID);
            if (Pet == null)
            {
                Logger.LogErrorFormat("can not find PetTable with id:{0}", iPetID);
            }
            else
            {
                ResTable res = TableManager.instance.GetTableItem<ResTable>(Pet.ModeID);

                if (res == null)
                {
                    Logger.LogErrorFormat("can not find ResTable with id:{0}", Pet.ModeID);
                }
                else
                {
                    if(OpenEffectObj == null)
                    {
                        return;
                    }

                    GameObject attach = Utility.FindGameObject(OpenEffectObj, AttachPointPath);
                    if(attach == null)
                    {
                        return;
                    }

                    GameObject ModelObj = AssetLoader.instance.LoadResAsGameObject(res.ModelPath);
                     if (ModelObj == null)
                    {
                        return;
                    }

                    Utility.AttachTo(ModelObj, attach);

                    Transform[] amr = ModelObj.GetComponentsInChildren<Transform>(true);
                    for (int i = 0; i < amr.Length; i++)
                    {
                        amr[i].gameObject.layer = 5;
                    }

                    Vector3 localPosition = ModelObj.GetComponent<Transform>().localPosition;
                    Quaternion Qua = ModelObj.GetComponent<Transform>().localRotation;

                    ModelObj.GetComponent<Transform>().localPosition = new Vector3(localPosition.x, Pet.OpenEggHeight, localPosition.z);
                    ModelObj.GetComponent<Transform>().localRotation = Quaternion.Euler(Qua.x, Pet.OpenEggRotation / 1000.0f, Qua.z);
                    ModelObj.GetComponent<Transform>().localScale = new Vector3(Pet.OpenEggModelScale[0], Pet.OpenEggModelScale[1], Pet.OpenEggModelScale[2]);

                    AnimControl =  ModelObj.GetComponent<GeAnimDescProxy>();
                    if(AnimControl != null)
                    {
                        AnimControl.ChangeAction(Pet.OpenEggAction, false, 1);

                        GeAnimDesc AnimDesc = AnimControl.FindAnimDescByName(Pet.OpenEggAction);
                        if(AnimDesc != null)
                        {         
                            SpecialActionTimeLen = AnimDesc.timeLen;
                            bStartIdleAction = true;
                        }                      
                    }

                    var backup = mAvatarRenderer.avatarPos;
                    mAvatarRenderer.CreateEffect("Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS", 999999, backup);
                }
            }     
        }

        /// <summary>
        /// 创建宠物模型
        /// </summary>
        protected void CreatePetModel(int petId)
        {
            if (ClientSystemManager.instance == null)
                return;
            if (m_PetTableData == null)
            {
                Logger.LogErrorFormat("can not find PetTable with id:{0}", petId);
                return;
            }
            var clientSystemTown = ClientSystemManager.instance.CurrentSystem as ClientSystemTown;
            if (clientSystemTown == null)
                return;
            BeTownPetData petData = new BeTownPetData
            {
                PetID = petId
            };
            m_BeTownPet = new BeTownPet(petData,null, true, true);
            var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(m_PetTableData.ModeID);
            if (null == resData)
                return;
            if (clientSystemTown.Scene == null)
                return;
            m_BeTownPet.InitPetForOpenEgg(clientSystemTown.Scene, PetGeActorPostLoad);
        }

        private void PetGeActorPostLoad(GeActorEx pet)
        {
            if (m_BeTownPet == null)
                return;
            if (m_PetTableData == null)
                return;
            GameObject attach = Utility.FindGameObject(OpenEffectObj, AttachPointPath);
            if (attach == null)
                return;
            if (m_BeTownPet.GeActor == null)
                return;
            m_PetEntityRoot = m_BeTownPet.GeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
            if (m_PetEntityRoot == null)
                return;
            Utility.AttachTo(m_PetEntityRoot, attach);

            Transform[] amr = m_PetEntityRoot.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < amr.Length; i++)
            {
                amr[i].gameObject.layer = 5;
            }
            Vector3 localPosition = m_PetEntityRoot.GetComponent<Transform>().localPosition;
            Quaternion Qua = m_PetEntityRoot.GetComponent<Transform>().localRotation;

            m_PetEntityRoot.GetComponent<Transform>().localPosition = new Vector3(localPosition.x, m_PetTableData.OpenEggHeight, localPosition.z);
            m_PetEntityRoot.GetComponent<Transform>().localRotation = Quaternion.Euler(Qua.x, m_PetTableData.OpenEggRotation / 1000.0f, Qua.z);
            m_PetEntityRoot.GetComponent<Transform>().localScale = new Vector3(m_PetTableData.OpenEggModelScale[0], m_PetTableData.OpenEggModelScale[1], m_PetTableData.OpenEggModelScale[2]);

            bStartIdleAction = true;
            var backup = mAvatarRenderer.avatarPos;

            mAvatarRenderer.CreateEffect("Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS", 999999, backup);

            if (m_BeTownPet.BeTownActionPlay != null)
            {
                SpecialActionTimeLen = m_BeTownPet.BeTownActionPlay.GetActionTime(BePetActionSwitchHelper.GetActionNameByType(PetActionNameType.OpenEggIdle));
            }
            ChangeAction(PetActionNameType.OpenEggIdle);
        }

        /// <summary>
        /// 播放开蛋动画
        /// </summary>
        protected void ChangeAction(PetActionNameType type)
        {
            if (m_BeTownPet == null)
                return;
            if (m_BeTownPet.BeTownActionPlay == null)
                return;
            m_BeTownPet.BeTownActionPlay.PlayAction(BePetActionSwitchHelper.GetActionNameByType(type));
        }

        void UpdateActorInfo()
        {
            PetInfo pet = PetDataManager.GetInstance().GetNewOpenPet();

            if (pet.dataId == 0)
            {
                return;
            }

            m_PetTableData = TableManager.GetInstance().GetTableItem<PetTable>((int)pet.dataId);
            if(m_PetTableData == null)
            {
                return;
            }

            //mEffectPos.CustomActive(false);
            mAvatarRenderer.gameObject.CustomActive(true);

            AudioManager.instance.PlaySound(3321);

            if (!m_PetTableData.UseNewFunction)
            {
                UpdateActor((int)pet.dataId);
            }
            else
            {
                CreatePetModel((int)pet.dataId);
            }

            mTitle.gameObject.CustomActive(true);
            mName.text = PetDataManager.GetInstance().GetColorName(m_PetTableData.Name, m_PetTableData.Quality);
            mName.gameObject.CustomActive(true);
            mGoTo.gameObject.CustomActive(true);
            mQuitTip.gameObject.CustomActive(true);
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            fTimeIntval += timeElapsed;
            if(fTimeIntval > fTimeValue && !bInitActor && bIsFirstClick)
            {
                bInitActor = true;

                UpdateActorInfo();
            }
            

            if(bStartIdleAction)
            {
                SpecialActionTimeIntrval += timeElapsed;
                if (SpecialActionTimeIntrval >= SpecialActionTimeLen)
                {
                    if (AnimControl != null && !m_PetTableData.UseNewFunction)
                    {
                        AnimControl.ChangeAction("Anim_Idle01", true, 1);
                    }
                    else
                    {
                        ChangeAction(PetActionNameType.Idle);
                    }
                    
                    bStartIdleAction = false;
                }       
            }

            if (null != mAvatarRenderer)
            {
                while (global::Global.Settings.avatarLightDir.x > 360)
                    global::Global.Settings.avatarLightDir.x -= 360;
                while (global::Global.Settings.avatarLightDir.x < 0)
                    global::Global.Settings.avatarLightDir.x += 360;

                while (global::Global.Settings.avatarLightDir.y > 360)
                    global::Global.Settings.avatarLightDir.y -= 360;
                while (global::Global.Settings.avatarLightDir.y < 0)
                    global::Global.Settings.avatarLightDir.y += 360;

                while (global::Global.Settings.avatarLightDir.z > 360)
                    global::Global.Settings.avatarLightDir.z -= 360;
                while (global::Global.Settings.avatarLightDir.z < 0)
                    global::Global.Settings.avatarLightDir.z += 360;

                mAvatarRenderer.m_LightRot = global::Global.Settings.avatarLightDir;
            }
        }

        #region ExtraUIBind
        private GameObject mEffectPos = null;
        private GeAvatarRendererEx mAvatarRenderer = null;
        private Text mName = null;
        private Button mBack = null;
        private Button mGoTo = null;
        private GameObject mTitle = null;
        private Text mQuitTip = null;

        protected override void _bindExUI()
        {
            mEffectPos = mBind.GetGameObject("EffectPos");
            mAvatarRenderer = mBind.GetCom<GeAvatarRendererEx>("AvatarRenderer");
            mName = mBind.GetCom<Text>("Name");
            mBack = mBind.GetCom<Button>("Back");
            mBack.onClick.AddListener(_onBackButtonClick);
            mGoTo = mBind.GetCom<Button>("GoTo");
            mGoTo.onClick.AddListener(_onGoToButtonClick);
            mTitle = mBind.GetGameObject("Title");
            mQuitTip = mBind.GetCom<Text>("QuitTip");
        }

        protected override void _unbindExUI()
        {
            mEffectPos = null;
            mAvatarRenderer = null;
            mName = null;
            mBack.onClick.RemoveListener(_onBackButtonClick);
            mBack = null;
            mGoTo.onClick.RemoveListener(_onGoToButtonClick);
            mGoTo = null;
            mTitle = null;
            mQuitTip = null;
        }
        #endregion

        #region Callback
        private void _onBackButtonClick()
        {
            if(bIsFirstClick && fTimeIntval < fTimeValue)
            {
                bIsFirstClick = false;
                UpdateActorInfo();
             
                return;
            }

            frameMgr.CloseFrame(this);
        }

        private void _onGoToButtonClick()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<PackageNewFrame>())
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PackageSwitch2OneGroup, EPackageOpenMode.Pet);
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<PackageNewFrame>(FrameLayer.Middle, EPackageOpenMode.Pet);
            }


            //ClientSystemManager.GetInstance().CloseFrame<PackageNewFrame>();
            if (ClientSystemManager.GetInstance().IsFrameOpen<PetPacketFrame>())
            {
                var petFrame = ClientSystemManager.GetInstance().GetFrame(typeof(PetPacketFrame)) as PetPacketFrame;
                if (petFrame != null)
                {
                    petFrame.SetPetToggle();
                }
            }
            //else
            //{
            //    ClientSystemManager.GetInstance().OpenFrame<PetPacketFrame>(FrameLayer.Middle);
            //}  

            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}

