using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.ComponentModel;
using System;
using Protocol;

namespace GameClient
{
    public enum AttributeType2
    {
        lightAttack = 0,      // 光属性攻击
        fireAttack,           // 火属性攻击
        iceAttack,            // 冰属性攻击
        darkAttack,           // 暗属性攻击
        lightDefence,         // 光属性抗性
        fireDefence,          // 火属性抗性
        iceDefence,           // 冰属性抗性
        darkDefence,
        count
    }

    class ActorShowGroup : ClientFrame
    {
        public ActorShowEquipData m_kData;

        //查询类型和查询区域
        public static uint m_queryPlayerType = 0;
        public static uint m_zoneId = 0;

        bool IsInitPetShow = false;

		#region ExtraUIBind
		private Text mName = null;
		private Text mLevel = null;
		private Text mJob = null;
		private Text mGuildName = null;
		private Text mGuildJob = null;
		private Text mPkLevel = null;
		private Text mPkPoint = null;
		private Text mPkCount = null;
		private Text mPkWinRate = null;
        private Text mNickName = null;
		private UINumber mVip = null;
        private GameObject mVipGo = null;
		private Text mSpecial = null;
		private GameObject mBtnAddFriend = null;
		private GameObject mBtnChat = null;
		private GameObject mObjInfoRoot = null;
		private Button mBtnHideInfo = null;
		private Button mBtnCheckInfo = null;
        private Image mPkLevelBg = null;
        private Image mPkLevelNum = null;
		private GameObject mPetRoot = null;
        private Text mAdventureTeamName = null;
        private Text mAdventureTeamScore = null;
        private Text mAdventureTeamRank = null;
        private GameObject fashionItemParent = null;
        private Image emblemName = null;
        private Toggle mTabA = null;
        private SpriteAniRenderChenghao mChenghao = null;
        private Image mTitleAnimation = null;
        private GameObject mTitleName = null;
        private Image mSelfIcon = null;
        private Image mOtherIcon = null;
        private Text mSelfScoreValue = null;
        private Text mOtherScoreValue = null;
        private Text mDifferenceScoreValue = null;
        
        protected override void _bindExUI()
		{
			mName = mBind.GetCom<Text>("playerName");
			mLevel = mBind.GetCom<Text>("level");
			mJob = mBind.GetCom<Text>("job");
			mGuildName = mBind.GetCom<Text>("guildName");
			mGuildJob = mBind.GetCom<Text>("guildJob");
			mPkLevel = mBind.GetCom<Text>("pkLevel");
			mPkPoint = mBind.GetCom<Text>("pkPoint");
			mPkCount = mBind.GetCom<Text>("pkCount");
			mPkWinRate = mBind.GetCom<Text>("pkWinRate");
            mNickName = mBind.GetCom<Text>("nickName");
            mVipGo = mBind.GetGameObject("VipGo");
            mVip = mBind.GetCom<UINumber>("vipNum");
			mSpecial = mBind.GetCom<Text>("special");
			mBtnAddFriend = mBind.GetGameObject("btnAddFriend");
			mBtnChat = mBind.GetGameObject("btnChat");
			mObjInfoRoot = mBind.GetGameObject("objInfoRoot");
			mBtnCheckInfo = mBind.GetCom<Button>("btnCheckInfo");
			mBtnCheckInfo.onClick.AddListener(_onBtnCheckInfoButtonClick);
			mBtnHideInfo = mBind.GetCom<Button>("btnHideInfo");
			mBtnHideInfo.onClick.AddListener(_onBtnHideInfoButtonClick);
            mPkLevelBg = mBind.GetCom<Image>("pkLevelBg");
            mPkLevelNum = mBind.GetCom<Image>("pkLevelNum");
			mPetRoot = mBind.GetGameObject("petRoot");
            mAdventureTeamName = mBind.GetCom<Text>("adventureTeamName");
            mAdventureTeamScore = mBind.GetCom<Text>("adventureTeamScore");
            mAdventureTeamRank = mBind.GetCom<Text>("adventureTeamRank");
            fashionItemParent = mBind.GetGameObject("fashionItemParent");
            emblemName = mBind.GetCom<Image>("playerEmblemName");
            mTabA = mBind.GetCom<Toggle>("TabA");
            mTabA.onValueChanged.AddListener(_onTabAToggleValueChange);
            mChenghao = mBind.GetCom<SpriteAniRenderChenghao>("Chenghao");
            mTitleAnimation = mBind.GetCom<Image>("TitleAnimation");
            mTitleName = mBind.GetGameObject("TitleName");
            mSelfIcon = mBind.GetCom<Image>("SelfIcon");
            mOtherIcon = mBind.GetCom<Image>("OtherIcon");
            mSelfScoreValue = mBind.GetCom<Text>("SelfScoreValue");
            mOtherScoreValue = mBind.GetCom<Text>("OtherScoreValue");
            mDifferenceScoreValue = mBind.GetCom<Text>("DifferenceScoreValue");
        }

		protected override void _unbindExUI()
		{
			mName = null;
			mLevel = null;
			mJob = null;
			mGuildName = null;
			mGuildJob = null;
			mPkLevel = null;
			mPkPoint = null;
			mPkCount = null;
			mPkWinRate = null;
            mNickName = null;
            mVipGo = null;
			mVip = null;
			mSpecial = null;
			mBtnAddFriend = null;
			mBtnChat = null;
			mBtnCheckInfo.onClick.RemoveListener(_onBtnCheckInfoButtonClick);
			mBtnCheckInfo = null;
			mBtnHideInfo.onClick.RemoveListener(_onBtnHideInfoButtonClick);
			mBtnHideInfo = null;
            mPkLevelBg = null;
            mPkLevelNum = null;
            mAdventureTeamName = null;
            mAdventureTeamScore = null;
            mAdventureTeamRank = null;
            fashionItemParent = null;
            emblemName = null;
            mTabA.onValueChanged.RemoveListener(_onTabAToggleValueChange);
            mTabA = null;
            mChenghao = null;
            mTitleAnimation = null;
            mTitleName = null;
            mSelfIcon = null;
            mOtherIcon = null;
            mSelfScoreValue = null;
            mOtherScoreValue = null;
            mDifferenceScoreValue = null;
        }

		private void _onBtnCheckInfoButtonClick()
		{
			ShowInfo();
			mBtnHideInfo.gameObject.CustomActive(true);
			mBtnCheckInfo.gameObject.CustomActive(false);
		}

		private void _onBtnHideInfoButtonClick()
		{
			ShowInfo(false);
			mBtnHideInfo.gameObject.CustomActive(false);
			mBtnCheckInfo.gameObject.CustomActive(true);
		}

        private void _onTabAToggleValueChange(bool changed)
        {
            /* put your code in here */

        }
        #endregion



        public override string GetPrefabPath()
        {
			return "UIFlatten/Prefabs/CheckInfo/ActorShowGroup";
        }

        #region actorModel

        [UIControl("ActorShowModel/Model/ModelRenderTexture", typeof(GeAvatarRendererEx))]
        GeAvatarRendererEx m_AvatarRenderer;

		bool infoGeted = false;

		void ShowInfo(bool show=true)
		{
			if (mObjInfoRoot.activeSelf == show)
				return;

			mObjInfoRoot.CustomActive(show);
			SetInfo(mObjInfoRoot);
		}

		float GetValue(DisplayAttribute attribute, string childName)
		{
			var fieldInfo = attribute.GetType().GetField(childName);

			float value = 0;
			if (fieldInfo != null)
			{
				value = (float)fieldInfo.GetValue(attribute);
			}

			return value;
		}

		void _onReceiveOtherPlayerInfo(WorldQueryPlayerDetailsRet res)
		{
			var attribute = BeUtility.GetPlayerActorAttributeByRaceInfo(res.info);
			SetAttributeInfo(attribute, BeUtility.GetMainPlayerActorAttribute(), mObjInfoRoot);
			infoGeted = true;
		}

		IEnumerator _requestOtherPlayerInfo()
		{
			var msg = new MessageEvents();
			var req = new WorldQueryPlayerDetailsReq();
			var res = new WorldQueryPlayerDetailsRet();

			req.roleId = m_kData.m_guid;
		    req.zoneId = m_kData.m_zoneId;
		    req.queryType = m_kData.m_queryPlayerType;
			req.name = "";

			yield return (MessageUtility.WaitWithResend<WorldQueryPlayerDetailsReq, WorldQueryPlayerDetailsRet>(Network.ServerType.GATE_SERVER, msg, req, res, true, 10));

			if (msg.IsAllMessageReceived())
			{
				_onReceiveOtherPlayerInfo(res);
			}
		}

		void SetAttributeInfo(DisplayAttribute otherAttr, DisplayAttribute myattr, GameObject root)
		{
            if(otherAttr != null && myattr != null && root != null)
            {
                var bind = root.GetComponent<ComCommonBind>();
                if (bind != null)
                {
                    GameObject content = bind.GetGameObject("Content");
                    Utility.SetPersonalInfo(myattr, otherAttr, content);
                }
            }			
		}

		void SetInfo(GameObject root)
		{
			if (!infoGeted)
			{
				GameFrameWork.instance.StartCoroutine(_requestOtherPlayerInfo());
			}
		}

        void _InitActorModel()
        {
            //是否有称号
            bool isTitle = false;

            for (int i = 0; i < m_kData.m_akEquipts.Count; i++)
            {
                ItemData itemData = m_kData.m_akEquipts[i];
                if (itemData == null)
                    continue;

                if (itemData.SubType != (int)ProtoTable.ItemTable.eSubType.TITLE)
                    continue;

                isTitle = true;

                if (mChenghao != null)
                {
                    var titlePathList = itemData.TableData.Path2.ToList();
                    var titlePath = titlePathList[0];
                    var titleName = titlePathList[1];

                    var titleCount = 0;
                    var titleScale = 0.0f;
                    //解析成功
                    if (int.TryParse(titlePathList[2], out titleCount)
                        && float.TryParse(titlePathList[3], out titleScale))
                    {
                        mChenghao.Reset(titlePath,
                            titleName,
                            titleCount,
                            titleScale,
                            itemData.TableData.ModelPath);
                    }
                }

                if (mTitleAnimation != null)
                    mTitleAnimation.enabled = true;
            }

            if (isTitle)
            {
                if (mTitleName != null)
                    mTitleName.CustomActive(false);
            }

            mName.text = mNickName.text = m_kData.m_kName;
            RelationData relationData = null;
            RelationDataManager.GetInstance().FindPlayerIsRelation(m_kData.m_guid, ref relationData);
            if (relationData != null)
            {
                if (relationData.remark != null && relationData.remark != "")
                {
                    mName.text = relationData.remark;
                }
            }
			mLevel.text = m_kData.m_iLevel.ToString();

			if (m_kData.HasGuild())
			{
				mGuildName.text = m_kData.guildName;
				mGuildJob.text = Utility.GetGuildPositionName(m_kData.guildJob);
			}
			else {
				mGuildName.text = "-";
				mGuildJob.text = "-";
			}

			//段位信息
			//int tmp = 0;
			mPkLevel.text = SeasonDataManager.GetInstance().GetRankName((int)m_kData.pkValue);//Utility.GetNameByPkPoints(m_kData.pkValue, ref tmp);

			mPkPoint.text = m_kData.matchScore.ToString();
			mPkCount.text = m_kData.m_pkInfo.totalNum.ToString();
			if (m_kData.m_pkInfo.totalNum > 0)
				mPkWinRate.text = string.Format("{0:P1}", m_kData.m_pkInfo.totalWinNum / (float)m_kData.m_pkInfo.totalNum);
			else 
				mPkWinRate.text = "0";

            if (m_kData.HasVip())
            {
                mVipGo.CustomActive(true);
                mVip.Value = m_kData.vip;
            }
       
            var jobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(m_kData.m_iJob);
            if (jobItem != null)
            {
				mJob.text = jobItem.Name;
            }
            //model
            if (jobItem == null)
            {
                Logger.LogError("职业ID找不到 " + m_kData.m_iJob + "\n");
            }
            else
            {
                ProtoTable.ResTable res = TableManager.instance.GetTableItem<ProtoTable.ResTable>(jobItem.Mode);
                if (res == null)
                {
                    Logger.LogError("模型资源表 找不到 " + jobItem.Mode + "\n");
                }
                else
                {
                    if(mOtherIcon != null)
                    {
                        ETCImageLoader.LoadSprite(ref mOtherIcon, res.IconPath);
                    }

                    Utility.CreateActor(m_AvatarRenderer, jobItem.ID, 0,530,764,false);
					_ShowFashion();

                    m_AvatarRenderer.AttachAvatar("Aureole", "Effects/Scene_effects/EffectUI/Eff_chuangjue/Prefab/EffUI_chuangjue_beibao", "[actor]Orign", false);
                    m_AvatarRenderer.ChangeAction("Anim_Show_Idle", 1.0f, true);
                }
            }

            {
                var selfJobItem = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(PlayerBaseData.GetInstance().JobTableID);
                if(selfJobItem != null)
                {
                    ProtoTable.ResTable res = TableManager.instance.GetTableItem<ProtoTable.ResTable>(selfJobItem.Mode);
                    if (res == null)
                    {
                        Logger.LogError("模型资源表 找不到 " + selfJobItem.Mode + "\n");
                    }
                    else
                    {
                        if(mSelfIcon != null)
                        {
                            ETCImageLoader.LoadSprite(ref mSelfIcon, res.IconPath);
                        }
                    }
                }
            }
            
			if (mSelfScoreValue != null)
            {
                mSelfScoreValue.text = PlayerBaseData.GetInstance().TotalEquipScore.ToString();
            }

            if (mOtherScoreValue != null)
            {
                mOtherScoreValue.text = m_kData.totalEquipScore.ToString();
            }

            float differenceValue = Utility.CalculateDifference(PlayerBaseData.GetInstance().TotalEquipScore, m_kData.totalEquipScore);
            if (mDifferenceScoreValue != null)
            {
                string format = "{0}";
                string tr;

                if (differenceValue > 0)
                {
                    format = "+" + format;
                    tr = "ckxi_color_green";
                }
                else if (differenceValue == 0)
                {
                    tr = "ckxi_color_normal";
                }
                else
                {
                    tr = "ckxi_color_red";
                }

                mDifferenceScoreValue.text = TR.Value(tr, string.Format(format, differenceValue));
            }

            if (mPkLevelBg)
            {
                ETCImageLoader.LoadSprite(ref mPkLevelBg, SeasonDataManager.GetInstance().GetMainSeasonLevelSmallIcon((int)m_kData.pkValue));
            }

            if (mPkLevelNum)
            {
                ETCImageLoader.LoadSprite(ref mPkLevelNum, SeasonDataManager.GetInstance().GetSubSeasonLevelIcon((int)m_kData.pkValue));
                var pkLevelStr = SeasonDataManager.GetInstance().GetRankName((int)m_kData.pkValue);
                if (string.Compare(pkLevelStr, "最强王者") != 0)
                    mPkLevelNum.rectTransform.localScale = Vector3.one / 2f;
            }

            //佣兵团数据
            if (m_kData.HasAdventureTeam())
            {
                if (mAdventureTeamName)
                {
                    mAdventureTeamName.text = AdventureTeamDataManager.ChangeColorByGrade(m_kData.adventureTeamName, m_kData.adventureTeamGrade);
                }
                if (mAdventureTeamScore)
                {
                    bool isEmpty = string.IsNullOrEmpty(m_kData.adventureTeamGrade);
                    mAdventureTeamScore.CustomActive(!isEmpty);
                    if(!isEmpty)
                    {
                        string adventureTeamGrade = string.Format("({0})", m_kData.adventureTeamGrade);
                        string grade = AdventureTeamDataManager.ChangeColorByGrade(adventureTeamGrade, m_kData.adventureTeamGrade);
                        mAdventureTeamScore.text = grade;
                    }
                }
                //if (mAdventureTeamRank)
                //{
                //    bool isEmpty = m_kData.adventureTeamRank == 0;
                //    mAdventureTeamRank.CustomActive(!isEmpty);
                //    if(!isEmpty)
                //    {
                //        string rank = m_kData.adventureTeamRank.ToString();
                //        mAdventureTeamRank.text = string.Format(TR.Value("adventure_team_actor_show_rank"), rank);
                //    }
                //}
            }
            else
            {
                if (mAdventureTeamName)
                {
                    mAdventureTeamName.text = "-";
                }
                if (mAdventureTeamScore)
                {
                    mAdventureTeamScore.text = "";
                }
                if (mAdventureTeamRank)
                {
                    mAdventureTeamRank.text = "";
                }
            }
        }
        
        private void _SetText(Text text, string content)
        {
            if (text == null || content == null)
                return;
            text.font.RequestCharactersInTexture(content, text.fontSize, text.fontStyle);
            CharacterInfo characterInfo;
            float width = 1f;
            for (int i = 0; i < content.Length; i++)
            {
                text.font.GetCharacterInfo(content[i], out characterInfo, text.fontSize);
                width += characterInfo.advance;
            }
            text.rectTransform.sizeDelta = new Vector2(width, text.rectTransform.sizeDelta.y);
            text.text = content;
            return;
        }

		void _ShowFashion()
		{
            if (m_kData.avatar != null)
            {
                PlayerBaseData.GetInstance().AvatarEquipFromItems(m_AvatarRenderer,
                    m_kData.avatar.equipItemIds,
                    m_kData.m_iJob,
                    m_kData.avatar.weaponStrengthen,
                    null, 
                    false,
                    m_kData.avatar.isShoWeapon);
            }
            
        }
		
        #endregion

        #region actorEquipments
        void _InitEquipments()
        {
            if(!m_bEquipInited)
            {
                m_akCachedEquiptItemObjects.Clear();
                _InitEquiptSlots();
                _InitEquipts();
                m_bEquipInited = true;
            }
        }
        #region initSlot
        bool m_bEquipInited;
        GameObject m_goLeft;
        GameObject m_goRight;
        GameObject m_goMiddle;

        void _InitEquiptSlots()
        {
            m_goLeft = Utility.FindChild(frame, "ShowAppearanceRoot/InfoView/ViewPort/Content/Weapon/Left");
            m_goRight = Utility.FindChild(frame, "ShowAppearanceRoot/InfoView/ViewPort/Content/Weapon/Right");
            m_goMiddle = Utility.FindChild(frame, "ShowAppearanceRoot/InfoView/ViewPort/Content/SpecialWeapon/Left");
            List<ComItem> akGoItem = new List<ComItem>();
            int iCount = (int)EEquipWearSlotType.Equipassist1 - ((int)EEquipWearSlotType.EquipInvalid + 1);            
            int equipCount = 10; // 左右两边各5个
            for (int i = 0; i < iCount; ++i)
            {
                if(i < equipCount)
                {
                    ComItem comItem = CreateComItem(i < equipCount / 2 ? m_goLeft : m_goRight);
                    akGoItem.Add(comItem);
                }
                else
                {
                    akGoItem.Add(null); // 这里解释下为什么要加入null的原因 左右两边各5个装备，但是Equipassist1的值是从15开始的，10到15之间的几个枚举已经弃用了
                }
            }
            // marked by ckm
            // for (int i = iCount; i < (int)EEquipWearSlotType.EquipMax - 1; ++i)
            for (int i = iCount; i < (int)EEquipWearSlotType.Equipassist3; ++i)
            {
                ComItem comItem = CreateComItem(m_goMiddle);
                akGoItem.Add(comItem);
            }

            // 添加辟邪玉
            ComItem comItemBxy = CreateComItem(m_goMiddle);
            akGoItem.Add(comItemBxy);

            // for (int i = (int)EEquipWearSlotType.EquipInvalid + 1; i < (int)EEquipWearSlotType.EquipMax; ++i)
            for (int i = (int)EEquipWearSlotType.EquipInvalid + 1; i < (int)EEquipWearSlotType.Equipassist3 + 1; ++i)
            {
                MapIndex mapIndex = Utility.GetEnumAttribute<EEquipWearSlotType, MapIndex>((EEquipWearSlotType)i);
                if (mapIndex != null)
                {
                    if (mapIndex.Index >= 0 && mapIndex.Index < akGoItem.Count)
                    {
                        var comItem = akGoItem[mapIndex.Index];
                        if (comItem == null)
                        {
                            continue;
                        }

                        GameObject goParent = comItem.transform.parent.gameObject;
                        GameObject goLocal = comItem.transform.gameObject;
                        EEquipWearSlotType eEEquipWearSlotType = (EEquipWearSlotType)i;
                        m_akCachedEquiptItemObjects.Create((EEquipWearSlotType)i, new object[] { goParent, goLocal, eEEquipWearSlotType, this, null });
                    }
                }
            }

            MapIndex mapIndexBxy = Utility.GetEnumAttribute<EEquipWearSlotType, MapIndex>((EEquipWearSlotType)199);
            if (mapIndexBxy != null)
            {
                var comItem = akGoItem[101];
                if (comItem != null)
                {
                    GameObject goParent = comItem.transform.parent.gameObject;
                    GameObject goLocal = comItem.transform.gameObject;
                    EEquipWearSlotType eEEquipWearSlotType = (EEquipWearSlotType)199;
                    m_akCachedEquiptItemObjects.Create((EEquipWearSlotType)199, new object[] { goParent, goLocal, eEEquipWearSlotType, this, null });
                }
            }
        }

        void _InitEquipts()
        {
            if (m_kData.m_akEquipts != null)
            {
                for (int i = 0; i < m_kData.m_akEquipts.Count; ++i)
                {
                    var itemData = m_kData.m_akEquipts[i];
                    if (itemData != null)
                    {
                        if (m_akCachedEquiptItemObjects.HasObject(itemData.EquipWearSlotType))
                        {
                            m_akCachedEquiptItemObjects.RefreshObject(itemData.EquipWearSlotType, new object[] { itemData });
                        }
                    }
                }
            }
        }
        #endregion
        #region equit
        public class EquipItemObject : CachedObject
        {
            protected GameObject goLocal;
            protected GameObject goParent;
            protected EEquipWearSlotType eEEquipWearSlotType;
            protected ActorShowGroup THIS;

            ComItem comItem;
            ItemData itemData;

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goLocal = param[1] as GameObject;
                eEEquipWearSlotType = (EEquipWearSlotType)param[2];
                THIS = param[3] as ActorShowGroup;
                itemData = param[4] as ItemData;
                comItem = goLocal.GetComponent<ComItem>();
                comItem.Setup(itemData, OnItemClicked);

                Enable();
                _UpdateItem();
            }

            public override void OnRecycle()
            {
                Disable();
            }
            public override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }

            public override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override void OnDecycle(object[] param) { OnCreate(param); }
            public override void OnRefresh(object[] param)
            {
                OnCreate(new object[] { goParent, goLocal, eEEquipWearSlotType, THIS, param[0] });
            }
            public override bool NeedFilter(object[] param) { return false; }

            void _UpdateItem()
            {
                string part = Utility.GetEnumDescription<EEquipWearSlotType>(eEEquipWearSlotType);
                part = TR.Value(part);
                comItem.SetupSlot(ComItem.ESlotType.Opened, part);
                // marked by ckm
                // if(eEEquipWearSlotType == EEquipWearSlotType.Equipassist2 || eEEquipWearSlotType == EEquipWearSlotType.Equipassist3)
                // {
                //     comItem.SetupSlot(ComItem.ESlotType.Locked, "");
                // }
            }

            void OnItemClicked(GameObject obj, ItemData item)
            {
                if (item != null)
                {
                    LinkManager.GetInstance().AttachDatas = THIS.m_kData;
                    Parser.ItemParser.OnItemLink(item.GUID, (int) item.TableID,
                        m_queryPlayerType,
                        m_zoneId);
                }
            }
        }

        CachedObjectDicManager<EEquipWearSlotType, EquipItemObject> m_akCachedEquiptItemObjects = new CachedObjectDicManager<EEquipWearSlotType, EquipItemObject>();
        #endregion
        #endregion

		string strPetItem = "UIFlatten/Prefabs/Pet/PetItem";

		void ShowPetsInfo()
		{
            if(IsInitPetShow)
            {
                return;
            }

            IsInitPetShow = true;

            for (int i=0; i<m_kData.pets.Length; ++i)
			{
				var pet = m_kData.pets[i];

				var go = AssetLoader.instance.LoadResAsGameObject(strPetItem, false);
				if (go != null)
				{
					Utility.AttachTo(go, mPetRoot);
				}

				if (pet.dataID > 0)
				{
					var petinfo = new PetInfo();
					petinfo.dataId = (uint)pet.dataID;
					petinfo.level = (ushort)pet.level;
					petinfo.hunger = (ushort) pet.hunger;
					petinfo.skillIndex = (byte)pet.skillIndex;
                    petinfo.petScore = (uint)pet.petScore;

                    PetDataManager.GetInstance().SetPetItemData(go, petinfo, m_kData.m_iJob);
				}
			}
        }

        #region actorFashion
        void _InitAllFashions()
        {
            if(!m_bFashionInited)
            {
                m_akCachedEquiptFashionObjects.Clear();
                _InitFashionSlots();
                _InitFashions();
                m_bFashionInited = true;
            }
        }

        GameObject m_goFashionLeft;
        GameObject m_goFashionRight;
        bool m_bFashionInited;

        void _InitFashionSlots()
        {
            m_goFashionLeft = Utility.FindChild(frame, "ShowAppearanceRoot/InfoView/ViewPort/Content/Fashion/Left");
            m_goFashionRight = Utility.FindChild(frame, "ShowAppearanceRoot/InfoView/ViewPort/Content/Fashion/Right");
            List<ComItem> akGoItem = new List<ComItem>();
            //int iCount = (int)EFashionWearSlotType.Max - ((int)EFashionWearSlotType.Invalid + 1);
            //for (int i = 0; i < iCount; ++i)
            //{
            //    //ComItem comItem = CreateComItem(i < iCount / 2 ? m_goFashionRight : m_goFashionLeft);
            //    ComItem comItem = CreateComItem(m_goFashionLeft);
            //    akGoItem.Add(comItem);
            //}

            //for (int i = (int)EFashionWearSlotType.Invalid + 1; i < (int)EFashionWearSlotType.Max; ++i)
            //{
            //    MapIndex mapIndex = Utility.GetEnumAttribute<EFashionWearSlotType, MapIndex>((EFashionWearSlotType)i);
            //    if (mapIndex.Index >= 0 && mapIndex.Index < akGoItem.Count)
            //    {
            //        var comItem = akGoItem[mapIndex.Index];
            //        GameObject goParent = comItem.transform.parent.gameObject;
            //        GameObject goLocal = comItem.transform.gameObject;
            //        EFashionWearSlotType eEFashionWearSlotType = (EFashionWearSlotType)i;
            //        m_akCachedEquiptFashionObjects.Create(eEFashionWearSlotType, new object[] { goParent, goLocal, eEFashionWearSlotType, this, null });
            //    }
            //}

            //if(fashionItemParent != null)
            {
            for (int i = (int)EFashionWearNewSlotType.Invalid + 1; i < (int)EFashionWearNewSlotType.Max; i++)
            {
                    //ComItem comItem = CreateComItem(fashionItemParent);
                    ComItem comItem = CreateComItem(i <= 5 ? m_goFashionLeft : m_goFashionRight);
                akGoItem.Add(comItem);
                }
            }

            for (int i = (int)EFashionWearNewSlotType.Invalid + 1; i < (int)EFashionWearNewSlotType.Max; i++)
            {
                MapIndex mapIndex =
                    Utility.GetEnumAttribute<EFashionWearNewSlotType, MapIndex>((EFashionWearNewSlotType)i);
                if (mapIndex.Index >= 0 && mapIndex.Index < akGoItem.Count)
                {
                    var comItem = akGoItem[mapIndex.Index];
                    var goParent = comItem.transform.parent.gameObject;
                    var goLocal = comItem.transform.gameObject;
                    EFashionWearNewSlotType fashionWearNewSlotType = (EFashionWearNewSlotType)i;
                    EFashionWearSlotType fashionWearSlotType = PackageDataManager.GetInstance()
                        .GetFashionWearSlotTypeByItemFashionWearNewSlotType(fashionWearNewSlotType);
                    m_akCachedEquiptFashionObjects.Create(fashionWearSlotType, new object[] { goParent, goLocal, fashionWearSlotType, this, null });
                }
            }

        }

        void _InitFashions()
        {
            if (m_kData.m_akFashions != null)
            {
                for (int i = 0; i < m_kData.m_akFashions.Count; ++i)
                {
                    var itemData = m_kData.m_akFashions[i];
                    if (itemData != null)
                    {
                        if (m_akCachedEquiptFashionObjects.HasObject(itemData.FashionWearSlotType))
                        {
                            m_akCachedEquiptFashionObjects.RefreshObject(itemData.FashionWearSlotType, new object[] { itemData });
                        }
                    }
                }
            }
        }
       
        #region fashions
        public class FashionItemObject : CachedObject
        {
            protected GameObject goLocal;
            protected GameObject goParent;
            protected EFashionWearSlotType eEFashionWearSlotType;
            protected ActorShowGroup THIS;

            ComItem comItem;
            ItemData itemData;

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goLocal = param[1] as GameObject;
                eEFashionWearSlotType = (EFashionWearSlotType)param[2];
                THIS = param[3] as ActorShowGroup;
                itemData = param[4] as ItemData;
                comItem = goLocal.GetComponent<ComItem>();
                comItem.Setup(itemData, OnItemClicked);

                Enable();
                _UpdateItem();
            }

            public override void OnRecycle()
            {
                Disable();
            }
            public override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }

            public override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override void OnDecycle(object[] param) { OnCreate(param); }
            public override void OnRefresh(object[] param)
            {
                OnCreate(new object[] { goParent, goLocal, eEFashionWearSlotType, THIS, param[0] });
            }
            public override bool NeedFilter(object[] param) { return false; }

            void _UpdateItem()
            {
                string part = Utility.GetEnumDescription<EFashionWearSlotType>(eEFashionWearSlotType);
                part = TR.Value(part);
                comItem.SetupSlot(ComItem.ESlotType.Opened, part);
            }

            void OnItemClicked(GameObject obj, ItemData item)
            {
                if (item != null)
                {
                    //EquipSuitDataManager.GetInstance().CalculateEquipSuitInfos(THIS.m_kData.m_akFashions);
                    //ItemTipManager.GetInstance().ShowTip(item);
                    LinkManager.GetInstance().AttachDatas = THIS.m_kData;
                    Parser.ItemParser.OnItemLink(item.GUID, (int) item.TableID,
                        m_queryPlayerType,
                        m_zoneId);
                }
            }
        }

        CachedObjectDicManager<EFashionWearSlotType, FashionItemObject> m_akCachedEquiptFashionObjects = new CachedObjectDicManager<EFashionWearSlotType, FashionItemObject>();
        #endregion
        #endregion

        GameObject m_goActorModel;
        GameObject m_goNameInfo;
        GameObject m_goJobInfo;
        GameObject m_goModel;
        GameObject m_goActorShowEquip;
        GameObject m_goActorShowFashion;
        GameObject m_goActorShowFollowPet;
        Text m_kHasPetHint;
        GameObject m_goPkInfo;
        bool m_bMarked = false;
		bool bIsMyFriend = false;

        protected override void _OnOpenFrame()
        {
            m_kData = userData as ActorShowEquipData;

            //设置全局的查询类型和查询区域Id
            if (m_kData != null)
            {
                m_queryPlayerType = m_kData.m_queryPlayerType;
                m_zoneId = m_kData.m_zoneId;
            }
            else
            {
                m_queryPlayerType = 0;
                m_zoneId = 0;
            }

            m_bEquipInited = false;
            m_bFashionInited = false;

            m_goActorModel = Utility.FindChild(frame, "ActorShowModel");
            m_goModel = Utility.FindChild(m_goActorModel, "Model");
            m_goActorShowEquip = Utility.FindChild(frame, "ActorShowEquip");
            m_goActorShowFashion = Utility.FindChild(frame, "ActorShowFashion");
            m_bMarked = false;

            m_eTabType = TabType.TT_COUNT;

            _InitTabs();

            if (mTabA != null)
                mTabA.isOn = true;

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRefreshFriendList, _OnRereshFriend);
        }

        protected override void _OnCloseFrame()
        {
            IsInitPetShow = false;
            m_akCachedEquiptFashionObjects.Clear();
            m_akCachedEquiptItemObjects.Clear();
            m_akTabObjects.Clear();
            m_akTabObjects.DestroyAllObjects();
			infoGeted = false;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRefreshFriendList, _OnRereshFriend);

            m_queryPlayerType = 0;
            m_zoneId = 0;
        }

        void _OnRereshFriend(UIEvent uiEvent)
        {
            OnSetFilter(m_eTabType);
        }

        #region tabs
        GameObject m_goTabParent;
        GameObject m_goTabPrefab;
        enum TabType
        {
            [DescriptionAttribute("角色")]
            TT_ACTOR = 0,
            [DescriptionAttribute("时装")]
            TT_FASHION,
//            [DescriptionAttribute("随从")]
//            TT_FOLLOWPET,
            TT_COUNT,
        }
        Toggle[] m_akToggle = new Toggle[(int)TabType.TT_COUNT];
        TabType m_eTabType;

        void _InitTabs()
        {
            m_akTabObjects.Clear();
            //m_goTabParent = Utility.FindChild(frame, "tabs");
            //m_goTabPrefab = Utility.FindChild(m_goTabParent, "tab");
            //m_goTabPrefab.CustomActive(false);

			/*
            for(int i = 0; i < m_akToggle.Length; ++i)
            {
                var current = m_akTabObjects.Create((TabType)i, new object[] { m_goTabParent, m_goTabPrefab, i, this });
                m_akToggle[i] = current.toggle;
            }*/

            //m_akToggle[(int)TabType.TT_ACTOR].isOn = true;

			OnSetFilter(TabType.TT_ACTOR);
        }

        public class TabObject : CachedObject
        {
            protected GameObject goLocal;
            protected GameObject goParent;
            protected GameObject goPrefab;
            ActorShowGroup THIS;

            Text Label;
            Text CheckLabel;
            public Toggle toggle;
            TabType eTabType;

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                eTabType = (TabType)param[2];
                THIS = param[3] as ActorShowGroup;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    Label = Utility.FindComponent<Text>(goLocal, "Label");
                    CheckLabel = Utility.FindComponent<Text>(goLocal, "Checkmark/Label");
                    toggle = goLocal.GetComponent<Toggle>();
                    toggle.onValueChanged.RemoveAllListeners();
                    toggle.onValueChanged.AddListener((bool bValue) =>
                    {
                        if(bValue)
                        {
                            OnValueChanged();
                        }
                    });
                }

                Enable();
                _UpdateItem();
            }

            void OnValueChanged()
            {
                THIS.OnSetFilter(eTabType);
            }

            public override void OnRecycle()
            {
                Disable();
            }
            public override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }

            public override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override void OnDecycle(object[] param) { OnCreate(param); }
            public override void OnRefresh(object[] param) { OnCreate(param); }
            public override bool NeedFilter(object[] param) { return false; }

            void _UpdateItem()
            {
                string desc = Utility.GetEnumDescription(eTabType);
                Label.text = desc;
                CheckLabel.text = desc;
            }
        }

        CachedObjectDicManager<TabType, TabObject> m_akTabObjects = new CachedObjectDicManager<TabType, TabObject>();
        #endregion

        [UIEventHandle("BGRoot/close")]
        void OnClickClose()
        {
            frameMgr.CloseFrame(this);
        }

		[UIEventHandle("buttonRoot/BtnAddFriend")]
        void OnAddFriend()
        {
            RelationDataManager.GetInstance().AddFriendByID(m_kData.m_guid);
            m_bMarked = true;
        }

		[UIEventHandle("buttonRoot/BtnChat")]
		void OnChat()
        {
            // 自己不能和自己聊天
            if(m_kData.m_guid == PlayerBaseData.GetInstance().RoleID)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("can_not_talk_to_yourself"));
                return;
            }

            //禁止私聊
            if (ChatUtility.IsForbidPrivateChat() == true)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("chat_private_forbid_in_scene"));
                return;
            }

            var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>((int)ProtoTable.FunctionUnLock.eFuncType.Friend);
            if (null != functionData && PlayerBaseData.GetInstance().Level < functionData.FinishLevel)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_friend_chat_need_lv", functionData.FinishLevel));
                return;
            }
            if (ClientSystemManager.GetInstance().IsFrameOpen<ChatFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<ChatFrame>();
            }
            if (bIsMyFriend)
			{
				var relationData = RelationDataManager.GetInstance().GetRelationByRoleID(m_kData.m_guid);
                if (ClientSystemManager.GetInstance().IsFrameOpen<RelationFrameNew>())
                {
                    RelationDataManager.GetInstance().OnAddPriChatList(relationData, false);
                    RelationFrameData relationFrameData = new RelationFrameData();
                    relationFrameData.eCurrentRelationData = relationData;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPStartTalk, relationFrameData);
                }
                else
                {
                    ChatManager.GetInstance().OpenPrivateChatFrame(relationData);
                }
				
				OnClickClose();
			}
			else {
				var relationData = new RelationData();
				relationData.level = (ushort)m_kData.m_iLevel;
				relationData.uid = m_kData.m_guid;
				relationData.name = m_kData.m_kName;
				relationData.occu = (byte)m_kData.m_iJob;
                relationData.vipLv = (byte)m_kData.vip;

                if (ClientSystemManager.GetInstance().IsFrameOpen<RelationFrameNew>())
                {
                    RelationDataManager.GetInstance().OnAddPriChatList(relationData, false);
                    RelationFrameData relationFrameData = new RelationFrameData();
                    relationFrameData.eCurrentRelationData = relationData;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnTAPStartTalk, relationFrameData);
                }
                else
                {
                    ChatManager.GetInstance().OpenPrivateChatFrame(relationData);
                }
                OnClickClose();
			}
		}

        [UIEventHandle("buttonRoot/BtnReport")]
        void OnReport()
        {
            var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.FunctionUnLock>((int)ProtoTable.FunctionUnLock.eFuncType.ReportingFunction);
            if(null != functionData && PlayerBaseData.GetInstance().Level < functionData.FinishLevel)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relation_report_need_lv", functionData.FinishLevel));
                return;
            }

            if(m_kData.m_guid != PlayerBaseData.GetInstance().RoleID)
            {
                InformantInfo info = new InformantInfo();
                info.roleId = m_kData.m_guid.ToString();
                info.roleName = m_kData.m_kName;
                info.roleLevel = m_kData.m_iLevel.ToString();
                info.vipLevel = m_kData.vip.ToString();
                info.jobId = m_kData.m_iJob.ToString();
                info.jobName = BaseWebViewManager.GetInstance().GetJobNameByJobId((int)m_kData.m_iJob);
                BaseWebViewManager.GetInstance().TryOpenReportFrame(info);
            }
            else
            {
                //无法举报自己
                SystemNotifyManager.SystemNotify(9937);
            }
            this.Close();
        }

        public void ShowButton(bool isFriend)
        {
            UIGray gray = mBtnAddFriend.SafeAddComponent<UIGray>();
            if (gray != null)
            {
                gray.SetEnable(false);
            }

            Button btnAddFriend = mBtnAddFriend.GetComponent<Button>();
            if (btnAddFriend != null)
            {
                btnAddFriend.interactable = true;
            }

            if (isFriend)
            {
                mBtnAddFriend.CustomActive(false);
                mBtnChat.CustomActive(true);
            }
            else
            {
                mBtnAddFriend.CustomActive(true);
                mBtnChat.CustomActive(true);

                if (frameMgr.IsFrameOpen<Pk3v3CrossWaitingRoom>())
                {
                    if (gray != null)
                    {
                        gray.SetEnable(true);
                    }

                    if (btnAddFriend != null)
                    {
                        btnAddFriend.interactable = false;
                    }
                }
            }
        }

        void OnSetFilter(TabType eTabType)
        {
            TabType ePreType = m_eTabType;
            m_eTabType = eTabType;

            if(m_eTabType == TabType.TT_ACTOR)
            {
				if (ePreType == TabType.TT_COUNT)// || ePreType == TabType.TT_FOLLOWPET)
                {
                    _InitActorModel();
                }

                m_goModel.CustomActive(true);
                m_goActorModel.CustomActive(true);
                _InitEquipments();
                bIsMyFriend = false;
                var relationData = RelationDataManager.GetInstance().GetRelationByRoleID(m_kData.m_guid);
                if(relationData != null && relationData.type == (int)RelationType.RELATION_FRIEND)
                {
                    bIsMyFriend = true;
                }
				ShowButton(bIsMyFriend);
				_InitAllFashions();

				ShowPetsInfo();

                // btnReport.CustomActive(BaseWebViewManager.GetInstance().IsReportFuncOpen());
                // 关闭举报功能，ckm
                btnReport.CustomActive(false);

                SetInfo(mObjInfoRoot);
            }
            else if(m_eTabType == TabType.TT_FASHION)
            {

            }
        }


        #region OtherInfos
        
        [UIControl("buttonRoot/BtnReport")]
        Button btnReport;

        #endregion
    }
}