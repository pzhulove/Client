using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using GameClient;

using ProtoTable;
using Protocol;
using Network;


namespace _Settings
{
    public class SysNotifyMsgText
    {
        public const string CHANGE_NAME_FUNC_NOT_OPEN = "改名功能暂未开放";
        public const string CHANGE_ROLE_TIP = "角色切换功能请稍等";
        public static readonly string[] GuildJobNames = new string[6] {
            "-",
            "普通成员",
            "精英",
            "长老",
            "副会长",
            "会长"
        };

        public const string UNENABLE_OPEN_TITLE_BOOK = "称号簿开启等级不足";
    }
    public class RoleInfoSettings : SettingsBindUI
    {
        #region UIView

        Image headImg;
        Text nameText;
        Text serverFieldText;
        Text mVersion = null;
        Text levelText;
        Text expNumText;
        Slider expNumSlider;
        Text jobText;
        Text winRateText;
        Text pkNumText;

        Text jobTitleText;
        Button addNameTitle;
        Text nameTitleText;
        SpriteAniRenderChenghao aniRenderer;
        Image aniImg;
        Text equipRankText;
        ReplaceHeadPortraitFrame mReplaceHeadPortraitFrame;
        Button mMoreBtn;
        Text mTitleName;
        Image mTitleImg;
        Button mTitleButton;
        Image mHonorImg;

        #endregion

       // private RoleInfoSettingsData roleInfoData;

        public RoleInfoSettings(GameObject root, ClientFrame frame)
            : base(root, frame)
        { 

        }

        protected override string GetCurrGameObjectPath()
        {
            return "UIRoot/UI2DRoot/Middle/SettingPanel/Panel/Contents/roleInfo";
        }

        protected override void InitBind()
        {
            headImg = mBind.GetCom<Image>("headImg");
            nameText = mBind.GetCom<Text>("Name");
            serverFieldText = mBind.GetCom<Text>("ServerField");
            mVersion = mBind.GetCom<Text>("Version");
            levelText = mBind.GetCom<Text>("Level");
            expNumText = mBind.GetCom<Text>("ExpNums");
            expNumSlider = mBind.GetCom<Slider>("ExpSlider");
            jobText = mBind.GetCom<Text>("Job");
            winRateText = mBind.GetCom<Text>("WinRate");
            pkNumText = mBind.GetCom<Text>("PkNum");
            jobTitleText = mBind.GetCom<Text>("JobTitle");
            addNameTitle = mBind.GetCom<Button>("AddNameTitleBtn");
            if (addNameTitle)
            {
                addNameTitle.onClick.RemoveListener(OnAddNameTitleBtnClick);
                addNameTitle.onClick.AddListener(OnAddNameTitleBtnClick);
            }
            nameTitleText = mBind.GetCom<Text>("NameTitle");
            mTitleImg = mBind.GetCom<Image>("Tittle_Img");
            aniRenderer = mBind.GetCom<SpriteAniRenderChenghao>("AniRenderer");
            aniImg = mBind.GetCom<Image>("AniImg");
            equipRankText = mBind.GetCom<Text>("EquipRank");

            if (equipRankText)
            {
                equipRankText.transform.parent.CustomActive(false);
                equipRankText.CustomActive(false);
            }

            mReplaceHeadPortraitFrame = mBind.GetCom<ReplaceHeadPortraitFrame>("ReplaceHeadPortraitFrame");

            mMoreBtn = mBind.GetCom<Button>("More");
            if (mMoreBtn != null)
            {
                mMoreBtn.onClick.RemoveAllListeners();
                mMoreBtn.onClick.AddListener(_onMoreButtonClick);
            }
            mTitleName = mBind.GetCom<Text>("TitleName");
            mTitleButton = mBind.GetCom<Button>("TitleButton");
            if (mTitleButton != null)
            {
                mTitleButton.onClick.RemoveListener(OnTitleButtonBtnClick);
                mTitleButton.onClick.AddListener(OnTitleButtonBtnClick);
            }
            mHonorImg = mBind.GetCom<Image>("jobTitleHonor");
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UseHeadPortraitFrameSuccess, _OnUpdateHeadPortraitFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.HeadPortraitFrameChange, _OnHeadPortraitFrameChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TitleNameUpdate, _OnTitleNameUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.TitleBookCloseFrame, _OnTitleBookClose);
        }

        protected override void UnInitBind()
        {
            headImg = null;
            nameText = null;
            serverFieldText = null;
            mVersion = null;
            levelText = null;
            expNumText = null;
            expNumSlider = null;
            jobText = null;
            winRateText = null;
            pkNumText = null;
            jobTitleText = null;
            if (addNameTitle)
            {
                addNameTitle.onClick.RemoveListener(OnAddNameTitleBtnClick);
            }
            addNameTitle = null;
            nameTitleText = null;
            aniRenderer = null;
            aniImg = null;
            equipRankText = null;
            mReplaceHeadPortraitFrame = null;

            if (mMoreBtn != null)
            {
                mMoreBtn.onClick.RemoveListener(_onMoreButtonClick);
            }
            mMoreBtn = null;
            mTitleName = null;
            mTitleImg = null;
            if(mTitleButton != null)
            {
                mTitleButton.onClick.RemoveAllListeners();
            }
            mTitleButton = null;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UseHeadPortraitFrameSuccess, _OnUpdateHeadPortraitFrame);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.HeadPortraitFrameChange, _OnHeadPortraitFrameChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TitleNameUpdate, _OnTitleNameUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.TitleBookCloseFrame, _OnTitleBookClose);
        }

        private void _OnUpdateHeadPortraitFrame(UIEvent uiEvent)
        {
            UpdateHeadPortraitFrame();
        }

        private void _OnHeadPortraitFrameChanged(UIEvent uiEvent)
        {
            UpdateHeadPortraitFrame();
        }

        private void _OnTitleNameUpdate(UIEvent uiEvent)
        {
            UpdateTitleInformation();
        }
        private void _OnTitleBookClose(UIEvent uiEvent)
        {
            SetNameTitleImg();
        }
        protected override void OnShowOut()
        {
            //RoleInfoSettingManager.GetInstance().SendWatchPlayerInfo(PlayerBaseData.GetInstance().RoleID);
            //roleInfoData = RoleInfoSettingManager.GetInstance().ActorShowData;

            SetAllViewContent();

            TittleBookManager.GetInstance().onAddTittle += OnAddNameTitleSucc;
        }

        protected override void OnHideIn()
        {
            //roleInfoData = null;
            TittleBookManager.GetInstance().onAddTittle -= OnAddNameTitleSucc;
        }

        void OnAddNameTitleBtnClick()
        {
            if (PlayerBaseData.GetInstance().Level >= 7)
            {
                ClientSystemManager.GetInstance().OpenFrame<TitleBookFrame>(FrameLayer.Middle);
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation(SysNotifyMsgText.UNENABLE_OPEN_TITLE_BOOK);
            }
        }

        void _onMoreButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<PersonalSettingFrame>(FrameLayer.Middle, 1);
        }

        void OnAddNameTitleSucc(ulong uid)
        {
            SetNameTitleImg();
        }

        void OnTitleButtonBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<PersonalSettingFrame>(FrameLayer.Middle, 0);
        }

        #region SetUIView Content

        void SetAllViewContent()
        {
            SetPlayerIcon();
            SetPlayerName();
            SetServerName();
            SetVersion();
            SetRoleLevel();
            SetRoleExp();
            SetJobName();
            SetWinRate();
            SetTotalPkCount();
            SetGuildJob();
            SetNameTitleImg();
            SetEquipGrade();
            UpdateHeadPortraitFrame();
            UpdateTitleInformation();
        }

        void UpdateHeadPortraitFrame()
        {
            if (mReplaceHeadPortraitFrame != null)
            {
                if (HeadPortraitFrameDataManager.WearHeadPortraitFrameID != 0)
                {
                    mReplaceHeadPortraitFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.WearHeadPortraitFrameID);
                }
                else
                {
                    mReplaceHeadPortraitFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
                }
            }
        }

        void UpdateTitleInformation()
        {
         
            if(!TitleDataManager.GetInstance().IsHaveTitle())
            {
             
                mTitleName.SafeSetText("");
                mTitleImg.CustomActive(false);
                mHonorImg.CustomActive(false);
                return;
            }
            var weardTitleInfo = PlayerBaseData.GetInstance().WearedTitleInfo;
            if(weardTitleInfo!=null)
            {
                if(weardTitleInfo.style==(int)TitleDataManager.eTitleStyle.Txt)
                {
                    mTitleName.CustomActive(true);
                    mTitleImg.CustomActive(false);
                    mHonorImg.CustomActive(false);
                    mTitleName.SafeSetText(weardTitleInfo.name);
                }
                else if(weardTitleInfo.style==(int)TitleDataManager.eTitleStyle.Img)
                {
                    mHonorImg.CustomActive(false);
                    mTitleName.CustomActive(false);
                    mTitleImg.CustomActive(true);

                    var titleItem= TableManager.GetInstance().GetTableItem<NewTitleTable>((int)weardTitleInfo.titleId);
                    if(titleItem!=null)
                    {
                        if (mTitleImg != null)
                        {
                            ETCImageLoader.LoadSprite(ref mTitleImg, titleItem.path);
                            mTitleImg.SetNativeSize();
                        }
                    }
                }
                else if (weardTitleInfo.style == (int)TitleDataManager.eTitleStyle.Group)
                {
                    mHonorImg.CustomActive(true);
                    mTitleName.CustomActive(true);
                    mTitleImg.CustomActive(false);

                    mTitleName.SafeSetText(weardTitleInfo.name);

                    var titleItem = TableManager.GetInstance().GetTableItem<NewTitleTable>((int)weardTitleInfo.titleId);
                    if (titleItem != null)
                    {
                        if (mHonorImg != null)
                        {
                            ETCImageLoader.LoadSprite(ref mHonorImg, titleItem.path);
                            mHonorImg.SetNativeSize();
                        }
                    }
                }
            }
           
           
        }
        void SetPlayerIcon()
        {
            string path = "";

            JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (jobData != null)
            {
                ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                if (resData != null)
                {
                    path = resData.IconPath;
                }
            }
            if(headImg!=null)
                // headImg.sprite = AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref headImg, path);
        }

        void SetPlayerName()
        {
            if (nameText)
                nameText.text = PlayerBaseData.GetInstance().Name;
        }

        void SetServerName()
        {
            if (serverFieldText)
                serverFieldText.text = ClientApplication.adminServer.name;
        }

        void SetVersion()
        {
            if (mVersion)
            {
                string versionFormat = TR.Value("system_version_desc");
#if MG_TEST || MG_TEST2 || MGSPTIYAN
                string versionDesc = string.Format(versionFormat, VersionManager.GetInstance().Version());
                bool isSmallPackage = SDKInterface.instance.IsSmallPackage();
                string smallPkgDesc = isSmallPackage ? "游戏包为分包" : "游戏包为整包";
                string memoryInfo = string.Format("剩余内存:{0},app占用内存:{1}", PluginManager.GetInstance().GetAvailMemory(), PluginManager.GetInstance().GetCurrentProcessMemory());
                string simulatorInfo = PluginManager.GetInstance().IsSimulator() ? PluginManager.GetInstance().GetSimulatorName() : "MP";
                mVersion.text = string.Format("{0}，{1}，{2}，{3}", simulatorInfo, memoryInfo, smallPkgDesc, versionDesc);
#else
                mVersion.text = string.Format(versionFormat, VersionManager.GetInstance().Version());
#endif
            }
        }

        void SetRoleLevel()
        {
            if (levelText)
                levelText.text = "等级："+PlayerBaseData.GetInstance().Level;
        }

        void SetRoleExp()
        {
            var level = PlayerBaseData.GetInstance().Level;
            var currLevelExp = TableManager.GetInstance().GetExpByLevel(level);
            if (expNumText)
            {
                if (0 != currLevelExp)
                    expNumText.text = string.Format("EXP:{0}/{1}({2}%)", PlayerBaseData.GetInstance().CurExp, currLevelExp, PlayerBaseData.GetInstance().CurExp * 100 / currLevelExp);
                else
                    expNumText.text = string.Format("EXP:{0}/{1}({2}%)", PlayerBaseData.GetInstance().CurExp, currLevelExp, 100);
            }
            if (expNumSlider)
            {
                expNumSlider.maxValue = currLevelExp;
                expNumSlider.value = PlayerBaseData.GetInstance().CurExp;
            }

            var maxLevel = 60;
            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_PLAYER_MAX_LEVEL_LIMIT);
            if (null != systemValue)
            {
                maxLevel = systemValue.Value;
                if (level == maxLevel)
                {
                    currLevelExp = PlayerBaseData.GetInstance().CurExp;
                    if (expNumText)
                        expNumText.text = string.Format("EXP:{0}/{1}({2}%)", currLevelExp, currLevelExp, 100);
                    if (expNumSlider)
                    {
                        expNumSlider.maxValue = currLevelExp;
                        expNumSlider.value = currLevelExp;
                    }
                }
            }
        }

        void SetJobName()
        {
            JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (jobData != null)
            {
                if (jobText)
                    jobText.text = jobData.Name;
            }
        }

        void SetWinRate()
        {
            if (winRateText)
            {
                PkStatistic pkData = PlayerBaseData.GetInstance().GetPkStatisticDataByPkType(PkType.Pk_Season_1v1);
                if (pkData != null)
                {
                    if (pkData.totalNum > 0)
                    {
                        winRateText.text = string.Format("{0:P1}", pkData.totalWinNum / (float)pkData.totalNum);
                    }
                    else
                    {
                        winRateText.text = "0%";
                    }
                }
                else
                {
                    winRateText.text = "0%";
                }
            }
        }

        void SetTotalPkCount()
        {
            if (pkNumText)
            {
                PkStatistic pkData = PlayerBaseData.GetInstance().GetPkStatisticDataByPkType(PkType.Pk_Season_1v1);
                if (pkData != null)
                {
                    pkNumText.text = pkData.totalNum.ToString();
                }
                else
                {
                    pkNumText.text ="0";
                }
            }
        }

        void SetGuildJob()
        {
            string guildDutyName = "";
            guildDutyName = SysNotifyMsgText.GuildJobNames[(int)PlayerBaseData.GetInstance().eGuildDuty];
            /*
            switch (PlayerBaseData.GetInstance().eGuildDuty)
            {
                case EGuildDuty.Invalid:
                    break;
                case EGuildDuty.Normal:
                    break;
                case EGuildDuty.Elite:
                    break;
                case EGuildDuty.Elder:
                    break;
                case EGuildDuty.Assistant:
                    break;
                case EGuildDuty.Leader:
                    break;
            }
             * */
            if (jobTitleText)
            {
                jobTitleText.text = guildDutyName;
            }
        }

        void SetNameTitleImg()
        {
            if(addNameTitle)
            {
                List<ulong> equipItemIdList = ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.WearEquip, ItemTable.eSubType.TITLE);
                if (equipItemIdList != null)
                {
                    if(equipItemIdList.Count > 0)
                    {
                        addNameTitle.CustomActive(false);
                        for (int i = 0; i < equipItemIdList.Count; i++)
                        {
                            ItemData tempItem = ItemDataManager.GetInstance().GetItem(equipItemIdList[i]);
                            if (tempItem != null)
                            {
                                var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)tempItem.TableID);
                                if (itemTable != null && itemTable.Path2.Count == 4)
                                {
                                    if (aniRenderer)
                                    {
                                        aniRenderer.CustomActive(true);
                                        aniRenderer.Reset(itemTable.Path2[0], itemTable.Path2[1], int.Parse(itemTable.Path2[2]), float.Parse(itemTable.Path2[3]), itemTable.ModelPath);
                                        if (aniImg)
                                            aniImg.enabled = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (aniRenderer)
                            aniRenderer.CustomActive(false);
                        if (aniImg)
                            aniImg.enabled = false;
                        addNameTitle.CustomActive(true);
                    }
                }
            }
        }

        void SetEquipGrade()
        {

            equipRankText.text = "";
            //int equipWearedTotalGrade = 0;
            //List<ulong> equipItemIdList = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            //if (equipItemIdList != null)
            //{
            //    for (int i = 0; i < equipItemIdList.Count; i++)
            //    {
            //        ItemData tempItem = ItemDataManager.GetInstance().GetItem(equipItemIdList[i]);
            //        if (tempItem != null)
            //        {
            //            equipWearedTotalGrade += tempItem.finalRateScore;
            //        }
            //    }
            //    if (equipRankText)
            //        equipRankText.text = equipWearedTotalGrade.ToString();
            //}
        }


        #endregion
    }


    public class RoleInfoSettingsData
    {
        public PkStatisticInfo m_pkInfo;
        public uint pkValue;
        public uint matchScore;

        public string guildName;
        public int guildJob;
        public bool HasGuild()
        {
            return guildName != null && guildName.Length > 1;
        }
    }
    public class RoleInfoSettingManager : DataManager<RoleInfoSettingManager>
    {
        private RoleInfoSettingsData actorShowData;
        public RoleInfoSettingsData ActorShowData
        {
            get { return actorShowData; }
            set { actorShowData = value; }
        }
        public override void Initialize()
        {
        }

        public override void Clear()
        {
            actorShowData = null;
        }


        /*
        public void SendWatchPlayerInfo(ulong roleID)
        {
            WorldQueryPlayerReq kCmd = new WorldQueryPlayerReq();
            kCmd.name = "";
            kCmd.roleId = roleID;
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, kCmd);
        }

        [MessageHandle(WorldQueryPlayerRet.MsgID)]
        void OnRecvWatchPlayerRet(MsgDATA msg)
        {
            WorldQueryPlayerRet ret = new WorldQueryPlayerRet();
            ret.decode(msg.bytes);

            actorShowData = new RoleInfoSettingsData();
            //加入PK信息
            actorShowData.m_pkInfo = ret.info.pkInfo;
            actorShowData.pkValue = ret.info.seasonLevel; //ret.info.pkValue;
            actorShowData.matchScore = ret.info.matchScore;

            //加入公会信息
            actorShowData.guildName = ret.info.guildTitle.name;
            actorShowData.guildJob = ret.info.guildTitle.post;
        }
         * */
    }
}

