using UnityEngine;
using EItemType = ProtoTable.ItemTable.eType;
using ProtoTable;
using UnityEngine.UI;

namespace GameClient
{
    class TAPSearchItem
    {
        const string pupilApplyItemPath = "UIFlatten/Prefabs/TAP/TAPSearchItem";
        private Image mIcon;
        private Text mName;
        private Button mBtnAccept;
        private Button mBtnRefuse;
        private Text mLevel;
        private GeAvatarRendererEx m_AvatarRenderer;
        private Button mBtnView;
        private Text mDeclaration;
        private Text mJobName;
        private Text mBtText;
        private UIGray mBtGray;
        private Button mViewInfomationBtn;
        private ComPlayerVipLevelShow mVipNumber = null;

        readonly string[] m_ActionTable = new string[2] { "Anim_Show_Idle", "Anim_Show_Idle_special01" };
        readonly int[] m_PlayList = new int[] { 0, 1, 0, 0 };

        private GameObject thisGameObject;
        public GameObject ThisGameObject
        {
            set
            {
                thisGameObject = value;
            }
            get
            {
                return thisGameObject;
            }
        }

        public TAPSearchItem(RelationData relationData,int mlayer)
        {
            CreateGo(relationData, mlayer);
        }

        private void CreateGo(RelationData relationData,int mLayer)
        {
            GameObject pupilApplyItem = AssetLoader.instance.LoadResAsGameObject(pupilApplyItemPath);
            if (pupilApplyItem == null)
            {
                return;
            }
            var mBind = pupilApplyItem.GetComponent<ComCommonBind>();
            if (mBind == null)
            {
                return;
            }
            mName = mBind.GetCom<Text>("Name");
            mIcon = mBind.GetCom<Image>("Icon");
            mBtnAccept = mBind.GetCom<Button>("BtnAccept");
            mBtnRefuse = mBind.GetCom<Button>("BtnRefuse");
            mLevel = mBind.GetCom<Text>("Level");
            m_AvatarRenderer = mBind.GetCom<GeAvatarRendererEx>("ModelRenderTexture");
            mBtnView = mBind.GetCom<Button>("BtnView");
            mDeclaration = mBind.GetCom<Text>("Declaration");
            mJobName = mBind.GetCom<Text>("JobName");
            mBtText = mBind.GetCom<Text>("BtText");
            mBtGray = mBind.GetCom<UIGray>("BtnAcceptGray");
            mViewInfomationBtn = mBind.GetCom<Button>("ViewInformationBtn");
            mVipNumber = mBind.GetCom<ComPlayerVipLevelShow>("VipNumber");
            //Name
            mName.text = relationData.name;

            //People Icon
            var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(relationData.occu);

            if (null != mIcon)
            {
                string path = "";
                if (null != jobItem)
                {
                    ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobItem.Mode);
                    if (resData != null)
                    {
                        path = resData.IconPath;
                    }
                }
                ETCImageLoader.LoadSprite(ref mIcon, path);
            }
            ThisGameObject = pupilApplyItem;

            //accept btn
            
            
            //btn Text
            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Teacher)
            {
                mBtText.text = "收徒";
                mBtnAccept.interactable = true;
                mBtGray.enabled = false;
                mBtnAccept.onClick.RemoveAllListeners();
                mBtnAccept.onClick.AddListener(() =>
                {
                    if (null != relationData)
                    {
                        TAPNewDataManager.GetInstance().SendApplyPupil(relationData.uid);
                        mBtGray.enabled = true;
                        mBtnAccept.interactable = false;
                    }
                });
            }
            else
            {
                mBtText.text = "拜师";
                mBtnAccept.interactable = true;
                mBtGray.enabled = false;
                mBtnAccept.onClick.RemoveAllListeners();
                mBtnAccept.onClick.AddListener(() =>
                {
                    if (null != relationData)
                    {
                        mBtGray.enabled = true;
                        mBtnAccept.interactable = false;
                        TAPNewDataManager.GetInstance().SendApplyTeacher(relationData.uid);
                    }
                });
            }
            

            //refuse btn
            //mBtnRefuse.onClick.RemoveAllListeners();
            //mBtnRefuse.onClick.AddListener(() =>
            //{
            //    if (null != relationData)
            //    {
            //        RelationDataManager.GetInstance().RefuseApplyPupils(new ulong[]{
            //        relationData.uid
            //    });
            //        RelationDataManager.GetInstance().RemoveApplyPupil(relationData.uid);
            //    }
            //});

            //查看玩家信息
            if (mViewInfomationBtn != null)
            {
                mViewInfomationBtn.onClick.RemoveAllListeners();
                mViewInfomationBtn.onClick.AddListener(() =>
                {
                    if (relationData != null)
                    {
                        OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(relationData.uid);
                    }
                });
            }

            //level
            mLevel.text = relationData.level.ToString();

            if (mVipNumber != null)
            {
                mVipNumber.SetVipLevel(relationData.vipLv);
            }


            //declaration
            mDeclaration.text = relationData.declaration;
            if (TAPNewDataManager.GetInstance().IsTeacher() == TAPType.Pupil)
            {
                mDeclaration.text = string.Format("宣言:{0}", TR.Value("tap_teacher_region")); ;
            }
            else
            {
                mDeclaration.text = string.Format("宣言:{0}", TR.Value("tap_pupil_region"));
            }

            JobTable job = TableManager.instance.GetTableItem<JobTable>(relationData.occu);
            if (job == null)
            {
                Logger.LogError("职业ID找不到 " + PlayerBaseData.GetInstance().JobTableID.ToString() + "\n");
            }
            else
            {
                mJobName.text = job.Name;
                ResTable res = TableManager.instance.GetTableItem<ResTable>(job.Mode);

                if (res == null)
                {
                    Logger.LogError("职业ID Mode表 找不到 " + PlayerBaseData.GetInstance().JobTableID.ToString() + "\n");
                }
                else
                {
                    m_AvatarRenderer.LoadAvatar(res.ModelPath,mLayer);
                    
                    //PlayerBaseData.GetInstance().AvatarEquipFromCurrentEquiped(m_AvatarRenderer);
                    PlayerBaseData.GetInstance().AvatarEquipFromItems(m_AvatarRenderer,
                        relationData.avatar.equipItemIds,
                        relationData.occu,
                        (int) (relationData.avatar.weaponStrengthen),
                        null,
                        false,
                        relationData.avatar.isShoWeapon);
                    m_AvatarRenderer.AttachAvatar("Aureole", "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS", "[actor]Orign", false);
                    m_AvatarRenderer.ChangeAction("Anim_Show_Idle", 1.0f, true);
                }
            }
        }
        public void UpdateMode(float timeElapsed)
        {
            if (null != m_AvatarRenderer)
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

                m_AvatarRenderer.m_LightRot = global::Global.Settings.avatarLightDir;
            }
        }
        public void DestoryGo()
        {
            GameObject.Destroy(ThisGameObject);
        }
    }
}