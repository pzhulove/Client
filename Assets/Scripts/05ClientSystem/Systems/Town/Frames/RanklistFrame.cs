using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using UnityEngine.Assertions;

namespace GameClient
{

    class RanklistFrame : ClientFrame
    {
        class Ranklist
        {
            public List<SortListType> arrTypes = new List<SortListType>();
            public BaseSortList data;
            public GameObject objRanklistRoot;
            public ComUIListScript rankListControl;
            public GameObject objMyRank;
            public ComToggleGroup comToggleGroup;
        }

        class RewardInfo
        {
            public int minScore;
            public int maxScore;
            public List<ItemData> items;
        }

        class subTypeToggleData
        {
            public int Id;
            public string Name;
        }
        List<subTypeToggleData> jobsList = new List<subTypeToggleData>();
        List<subTypeToggleData> weaponsList = new List<subTypeToggleData>();
        List<subTypeToggleData> towerList = new List<subTypeToggleData>();//爬塔没有全部这个选项，奇怪。

        [UIObject("Select")]
        GameObject m_objLineSelect;

        [UIControl("Ranks/Viewport/Content")]
        ComToggleGroup m_comRankGroup;

        [UIControl("Ranks")]
        ScrollRect m_comScrollRect;

        string[] m_rankIcons =
        {
            "UI/Image/NewPacked/Paihangbang.png:Paihangbang_Icon_Mingci_01",
            "UI/Image/NewPacked/Paihangbang.png:Paihangbang_Icon_Mingci_02",
            "UI/Image/NewPacked/Paihangbang.png:Paihangbang_Icon_Mingci_03",
        };
        
        string[] m_rankBgs =
        {
            "UI/Image/NewPacked/Paihangbang.png:Paihangbang_ItemBg_03",
            "UI/Image/NewPacked/Paihangbang.png:Paihangbang_ItemBg_04",
            "UI/Image/NewPacked/Paihangbang.png:Paihangbang_ItemBg_05",
        };

        List<Ranklist> m_arrRanklist = new List<Ranklist>();
        SortListType m_currRanklist = SortListType.SORTLIST_LEVEL;
        

        Dictionary<SortListType, int> m_dictSortTypeWithJobID = new Dictionary<SortListType, int> {
            { SortListType.SORTLIST_LEVEL,0},
            { SortListType.SORTLIST_TOWER,0},
            { SortListType.SORTLIST_1V1_SEASON,0},
            { SortListType.SORTLIST_GUILD_LEVEL,0},
            { SortListType.SORTLIST_WEAPON,0},
            { SortListType.SORTLIST_ACHIEVEMENT_SCORE,0},
            { SortListType.SORTLIST_ADVENTURE_TEAM_GRADE,0},
            { SortListType.SORTLIST_CHIJI_SCORE,0},
        };

        private void InitJobList()
        {
            jobsList = new List<subTypeToggleData>();
            towerList = new List<subTypeToggleData>();
            subTypeToggleData defaultType = new subTypeToggleData
            {
                Id = 0,
                Name = "全部",
            };
            jobsList.Add(defaultType);
            var jobTable = TableManager.GetInstance().GetTable<ProtoTable.JobTable>();
            if (jobTable != null)
            {
                var enumerator = jobTable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var jobItem = enumerator.Current.Value as JobTable;

                    if (jobItem == null)
                    {
                        continue;
                    }

                    //测试角色不添加到列表
                    if (jobItem.ID == 16)
                    {
                        continue;
                    }

                    if (jobItem.Open == 0)
                    {
                        continue;
                    }

                    if (jobItem.JobType == 0)
                    {
                        continue;
                    }

                    subTypeToggleData job = new subTypeToggleData
                    {
                        Id = jobItem.ID,
                        Name = jobItem.Name
                    };
                    jobsList.Add(job);
                    towerList.Add(job);
                }
            }
        }

        private void InitWeaponsList()
        {
            weaponsList = new List<subTypeToggleData>();
            subTypeToggleData defaultType = new subTypeToggleData
            {
                Id = 0,
                Name = "全部",
            };
            weaponsList.Add(defaultType);
            var weaponTable = TableManager.GetInstance().GetTable<RankWeaponTable>();
            if (weaponTable != null)
            {
                var enumerator = weaponTable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var weaponItem = enumerator.Current.Value as RankWeaponTable;

                    if (weaponItem == null)
                    {
                        continue;
                    }

                    if (!weaponItem.IsShow)
                    {
                        continue;
                    }
                    subTypeToggleData weapon = new subTypeToggleData
                    {
                        Id = weaponItem.ID,
                        Name = weaponItem.Name
                    };
                    weaponsList.Add(weapon);
                }
            }
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Ranklist/Ranklist";
        }

        protected override void _OnOpenFrame()
        {
            if (userData != null)
            {
                m_currRanklist = (SortListType)userData;
            }
            
            if (m_currRanklist == SortListType.SORTLIST_CHIJI_SCORE)
            {
                if (m_comScrollRect != null)
                {
                    m_comScrollRect.verticalNormalizedPosition = 0f;
                }
            }
            InitJobList();
            InitWeaponsList();
            _InitRanklists();
            _RegisterUIEvent();

            m_comRankGroup.SetCurrentToggle((int)m_currRanklist);


            m_objLineSelect.SetActive(false);
//             m_contentScroll.onValueChanged.AddListener((value) =>
//             {
//                 m_objLineSelect.transform.SetParent(m_objTempRoot.transform, false);
//                 m_objLineSelect.SetActive(false);
//             });

            if (mToggle250 != null)
            {
                mToggle250.CustomActive(ChijiDataManager.GetInstance().MainFrameChijiButtonIsShow());
            }
        }

        protected override void _OnCloseFrame()
        {
            m_arrRanklist.Clear();
            jobsList.Clear();
            weaponsList.Clear();
            towerList.Clear();
            m_currRanklist = SortListType.SORTLIST_LEVEL;
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RanklistUpdated, _OnRanklistUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildRequestJoinSuccess, _OnRequestJoinSuccess);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RanklistUpdated, _OnRanklistUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildRequestJoinSuccess, _OnRequestJoinSuccess);
        }

        void _InitRanklists()
        {
            m_comRankGroup.Initialize();
            if(m_comRankGroup.comToggles != null)
            {
                for (int i = 0; i < m_comRankGroup.comToggles.Length; ++i)
                {
                    Ranklist ranklist = new Ranklist();
                    ranklist.arrTypes.Add((SortListType)m_comRankGroup.comToggles[i].userData);
                    ranklist.data = null;
                    ranklist.objRanklistRoot = Utility.FindGameObject(frame, string.Format("Content/List{0}", (int)ranklist.arrTypes[0]));
                    m_arrRanklist.Add(ranklist);
                }
            }
            

            m_comRankGroup.onSelectChanged.AddListener((int a_type, bool a_checked) =>
            {
                Ranklist ranklist = _GetRanklist((SortListType)a_type);
                if (ranklist != null)
                {
                    if (a_checked)
                    {
                        if (ranklist.comToggleGroup == null)
                        {
                            var wrapper = ranklist.objRanklistRoot.GetComponent<UIPrefabWrapper>();
                            if (wrapper != null)
                            {
                                ranklist.objRanklistRoot = wrapper.LoadUIPrefab(ranklist.objRanklistRoot.transform);
                                _InitRankList(ranklist);
                            }

                            ranklist.objRanklistRoot.CustomActive(true);
                            if(ranklist.rankListControl!=null)
                            {
                                ranklist.rankListControl.SetElementAmount(0);
                            }
                            if(ranklist.objMyRank != null)
                            {
                                ranklist.objMyRank.SetActive(false);
                            }
                        }
                        m_currRanklist = (SortListType)a_type;
                        Assert.IsTrue(m_dictSortTypeWithJobID.ContainsKey(m_currRanklist));
                        int subType = 0;
                        int requestCount = 100;
                        if (m_currRanklist == SortListType.SORTLIST_GUILD_LEVEL ||
                            m_currRanklist == SortListType.SORTLIST_CHIJI_SCORE)
                        {
                            requestCount = 20;
                        }
                        if (ranklist.comToggleGroup != null)
                        {
                            subType = SetDefaultSubType(ranklist);
                            ranklist.comToggleGroup.SetCurrentToggle(subType);
                        }
                        else
                        {
                            _RequestRanklist(a_type,subType, 0,requestCount);                     
                        }
                    }
                    else
                    {
                        if (ranklist.objRanklistRoot != null)
                        {
                            ranklist.objRanklistRoot.SetActive(false);
                        }
                    }
                }
            });
        }

        private int SetDefaultSubType(Ranklist ranklist)
        {
            int subType = 0;
            if (ranklist == null)
            {
                return subType;
            }
            if(ranklist.arrTypes != null && ranklist.arrTypes.Count > 0)
            {
                if (ranklist.arrTypes.Count > 1)
                {
                    subType = (int)ranklist.arrTypes[1];
                }

                //第一次打开默认选中等级榜，需要特殊处理一下。直接请求我的职业。
                if (m_currRanklist == SortListType.SORTLIST_LEVEL || m_currRanklist == SortListType.SORTLIST_TOWER
                    || m_currRanklist == SortListType.SORTLIST_1V1_SEASON)
                {
                    int MyJobId = PlayerBaseData.GetInstance().JobTableID;
                    if (ranklist.arrTypes.Contains((SortListType)MyJobId))
                    {
                        subType = MyJobId;
                    }
                }

                if (m_currRanklist == SortListType.SORTLIST_WEAPON)
                {
                    var weapon = ItemDataManager.GetInstance().GetWearEquipDataBySlotType(EEquipWearSlotType.EquipWeapon);
                    if (weapon != null)
                    {
                        if (ranklist.arrTypes.Contains((SortListType)weapon.ThirdType))
                        {
                            subType = (int)weapon.ThirdType;
                        } 
                    }
                }
            }

            return subType;
        }

        void _InitRankList(Ranklist ranklist)
        {
            try
            {
                ranklist.objMyRank = Utility.FindGameObject(ranklist.objRanklistRoot, "Content/MyRank");
                ranklist.rankListControl = Utility.GetComponetInChild<ComUIListScript>(ranklist.objRanklistRoot, "Content");
                if(ranklist.rankListControl!=null)
                {
                    ranklist.rankListControl.Initialize();
                    ranklist.rankListControl.onItemVisiable = (item) =>
                    {
                        if (ranklist.data != null)
                        {
                            if (item.m_index >= 0 && item.m_index < ranklist.data.entries.Count)
                            {
                                _SetupOneLine(item.gameObject, m_currRanklist,(int)ranklist.data.type.subType, ranklist.data.entries[item.m_index], true, false);
                            }
                        }
                    };
                    ranklist.rankListControl.OnItemRecycle = (item) =>
                    {
                        if (Utility.FindGameObject(item.gameObject, "Valid/Select", false))
                        {
                            if (m_objLineSelect != null)
                            {
                                m_objLineSelect.transform.SetParent(frame.transform, false);
                                m_objLineSelect.SetActive(false);
                            }

                        }
                        // GameObject[] objEffects = new GameObject[3];
                        // objEffects[0] = Utility.FindGameObject(item.gameObject, string.Format("Effect{0}", 1), false);
                        // objEffects[1] = Utility.FindGameObject(item.gameObject, string.Format("Effect{0}", 2), false);
                        // objEffects[2] = Utility.FindGameObject(item.gameObject, string.Format("Effect{0}", 3), false);
                        // for (int j = 0; j < 3; ++j)
                        // {
                        //     if (objEffects[j] != null)
                        //     {
                        //         objEffects[j].SetActive(false);
                        //     }
                        // }
                    };
                }
              
                ranklist.comToggleGroup = Utility.GetComponetInChild<ComToggleGroup>(ranklist.objRanklistRoot, "Filter/Viewport/Content");
                if (ranklist.comToggleGroup != null)
                {
                    InitSubTypeUserData(ranklist);
                    ranklist.comToggleGroup.Initialize();

                    ranklist.comToggleGroup.onSelectChanged.AddListener((int a_type, bool a_checked) =>
                    {
                        if (a_checked)
                        {
                            if (ranklist.objRanklistRoot != null)
                                ranklist.objRanklistRoot.SetActive(true);

                            if (ranklist.rankListControl != null)
                                ranklist.rankListControl.SetElementAmount(0);

                            if (ranklist.objMyRank != null)
                                ranklist.objMyRank.SetActive(false);
                            m_currRanklist = ranklist.arrTypes[0];
                            _RequestRanklist((int)m_currRanklist, a_type, 0,100);
                        }
                        else
                        {
                            ranklist.objRanklistRoot.SetActive(false);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("_InitRankList:{0}",e.Message);
            }
        }


        //subtype toggle data read from table,don`t write on prefab.
        void InitSubTypeUserData(Ranklist rankList)
        {
            var subTypeToggleScroll = Utility.GetComponetInChild<ComUIListScript>(rankList.objRanklistRoot, "Filter");
            if (subTypeToggleScroll == null)
            {
                return;
            }

            List<subTypeToggleData> useList = null;
            SortListType type = rankList.arrTypes[0];
            if (type == SortListType.SORTLIST_LEVEL || type == SortListType.SORTLIST_1V1_SEASON || type == SortListType.SORTLIST_TOWER)
            {
                useList = jobsList;
            }

            if (type == SortListType.SORTLIST_TOWER)
            {
                useList = towerList;
            }
            else if(type == SortListType.SORTLIST_WEAPON)
            {
                useList = weaponsList;
            }
            if (useList == null)
            {
                return;
            }

            subTypeToggleScroll.Initialize();
            subTypeToggleScroll.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0 && item.m_index < useList.Count)
                {
                    var toggleData = useList[item.m_index];
                    if (toggleData != null)
                    {
                        var text = Utility.GetComponetInChild<Text>(item.gameObject,"Text");
                        text.SafeSetText(toggleData.Name);
                        var comToggle = item.GetComponent<ComToggle>();
                        if (comToggle != null)
                        {
                            comToggle.userData = toggleData.Id;
                            if (rankList.arrTypes.Contains((SortListType)toggleData.Id) == false)
                            {
                                rankList.arrTypes.Add((SortListType)toggleData.Id);
                            }
                        }
                        
                    }
                }
            };

            subTypeToggleScroll.SetElementAmount(useList.Count);
        }

        void _SetupOneLine(GameObject a_objLine, SortListType a_eType,int subType, BaseSortListEntry a_data, bool a_bCanSelect, bool a_bFilter)
        {
            if(a_objLine == null || a_data == null)
            {
                return;
            }

            GameObject objValid = Utility.FindGameObject(a_objLine, "Valid");
            GameObject objInvalid = Utility.FindGameObject(a_objLine, "Invalid");

            if(objValid == null || objInvalid == null)
            {
                return;
            }

            if (a_bCanSelect)
            {
                Button btnSelect = Utility.GetComponetInChild<Button>(objValid, "BtnSelect");

                if(btnSelect == null)
                {
                    return;
                }

                btnSelect.onClick.RemoveAllListeners();
                btnSelect.onClick.AddListener(() =>
                {
                    m_objLineSelect.transform.SetParent(objValid.transform, false);
                    m_objLineSelect.SetActive(true);

                    if (a_eType == SortListType.SORTLIST_GUILD_LEVEL||
                        a_eType == SortListType.SORTLIST_ADVENTURE_TEAM_GRADE)
                    {
                        //GuildDataManager.GetInstance().RequestJoinGuild(a_data.id);
                    }
                    else if (a_eType == SortListType.SORTLIST_WEAPON)
                    {
                        var itemSortListEntry = a_data as ItemSortListEntry;
                        if (itemSortListEntry != null)
                        {
                            OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(itemSortListEntry.ownerId);
                        }
                        else
                        {
                            Logger.LogErrorFormat("RankListFrame itemSortListEntry is null and data id is {0}",
                                a_data.id);
                        }
                    }
                    else
                    {
                        OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(a_data.id);
                    }
                });
            }

            switch (a_eType)
            {
                #region season
                case SortListType.SORTLIST_1V1_SEASON:
                    {
                        bool bValid = true;
                        if (a_bFilter)
                        {
                            bValid = subType == 0 || subType == PlayerBaseData.GetInstance().JobTableID;
                        }
                        objValid.SetActive(bValid);
                        objInvalid.SetActive(!bValid);

                        if (bValid)
                        {
                            SeasonSortListEntry data = a_data as SeasonSortListEntry;
                            if (data != null)
                            {
                                _SetupRanking(objValid, data.ranking);
                                Text name = Utility.GetComponetInChild<Text>(objValid, "Name");
                                name.text = data.name;
                                RelationData relationData = null;
                                RelationDataManager.GetInstance().FindPlayerIsRelation(data.id, ref relationData);
                                if (relationData != null)
                                {
                                    if (string.IsNullOrEmpty(relationData.remark) == false)
                                        name.text = relationData.remark;
                                }
                                Utility.GetComponetInChild<Text>(objValid, "Job").text = _GetJobName(data.occu);
                                Utility.GetComponetInChild<Text>(objValid, "Level").text = data.level.ToString();

                                int nSeasonRankID = (int)data.seasonLevel;
                                SeasonLevelTable seasonTable = TableManager.GetInstance().GetTableItem<SeasonLevelTable>(nSeasonRankID);
                                if (seasonTable != null)
                                {
                                    Utility.GetComponetInChild<Text>(objValid, "PKScore/Rank").text = SeasonDataManager.GetInstance().GetRankName(nSeasonRankID);

                                    int nStarCount = nSeasonRankID == SeasonDataManager.GetInstance().GetMaxRankID() ? (int)data.seasonStar : seasonTable.Star;
                                    Utility.GetComponetInChild<Text>(objValid, "PKScore/StarCount").text = string.Format("x{0}", nStarCount);
                                }
                                else
                                {
                                    Logger.LogErrorFormat("【段位榜】排名：{0} 段位ID:{1}找不到", data.ranking, nSeasonRankID);
                                }
                            }
                        }                        
                        break;
                    }
                #endregion
                #region level
                case SortListType.SORTLIST_LEVEL:
                    {
                        bool bValid = true;
                        if (a_bFilter)
                        {
                            bValid = subType == 0 || subType == PlayerBaseData.GetInstance().JobTableID;
                        }
                        objValid.SetActive(bValid);
                        objInvalid.SetActive(!bValid);

                        if (bValid)
                        {
                            LevelSortListEntry data = a_data as LevelSortListEntry;
                            if(null != data)
                            {
                                _SetupRanking(objValid, data.ranking);
                                Text name = Utility.GetComponetInChild<Text>(objValid, "Name");
                                name.text = data.name;
                                RelationData relationData = null;
                                RelationDataManager.GetInstance().FindPlayerIsRelation(data.id, ref relationData);
                                if (relationData != null)
                                {
                                    if (string.IsNullOrEmpty(relationData.remark) == false)
                                        name.text = relationData.remark;
                                }
                                Utility.GetComponetInChild<Text>(objValid, "Job").text = _GetJobName(data.occu);
                                Utility.GetComponetInChild<Text>(objValid, "Level").text = data.level.ToString();
                            }
                        }
                        break;
                    }
                #endregion
                #region tower
                case SortListType.SORTLIST_TOWER:
                    {
                        bool bValid = true;
                        if (a_bFilter)
                        {
                            bValid = subType == 0 || subType == PlayerBaseData.GetInstance().JobTableID;
                        }
                        objValid.SetActive(bValid);
                        objInvalid.SetActive(!bValid);

                        if (bValid)
                        {
                            DeathTowerSortListEntry data = a_data as DeathTowerSortListEntry;
                            if (data != null)
                            {
                                _SetupRanking(objValid, data.ranking);
                                Text name = Utility.GetComponetInChild<Text>(objValid, "Name");
                                name.text = data.name;
                                RelationData relationData = null;
                                RelationDataManager.GetInstance().FindPlayerIsRelation(data.id, ref relationData);
                                if (relationData != null)
                                {
                                    if (string.IsNullOrEmpty(relationData.remark) == false)
                                        name.text = relationData.remark;
                                }
                                Utility.GetComponetInChild<Text>(objValid, "Job").text = _GetJobName(data.occu);
                                Utility.GetComponetInChild<Text>(objValid, "Level").text = data.level.ToString();
                                Utility.GetComponetInChild<Text>(objValid, "Floor").text = data.layer.ToString();
                                Utility.GetComponetInChild<Text>(objValid, "ClearTime").text = _GetTimeUsed((int)data.usedTime);
                            }
                        }
                        break;
                    }
                #endregion
                #region guild
                case SortListType.SORTLIST_GUILD_LEVEL:
                    {
                        bool bValid = true;
                        if (a_bFilter)
                        {
                            bValid = GuildDataManager.GetInstance().HasSelfGuild();
                        }
                        objValid.SetActive(bValid);
                        objInvalid.SetActive(!bValid);

                        if (bValid)
                        {
                            GuildLevelSortListEntry data = a_data as GuildLevelSortListEntry;
                            if (data != null)
                            {
                                _SetupRanking(objValid, data.ranking);
                                Utility.GetComponetInChild<Text>(objValid, "Name").text = data.name;
                                Text leader = Utility.GetComponetInChild<Text>(objValid, "Leader");
                                leader.text = data.leader;
                                RelationData relationData = null;
                                RelationDataManager.GetInstance().FindPlayerIsRelation(data.id, ref relationData);
                                if (relationData != null)
                                {
                                    if (string.IsNullOrEmpty(relationData.remark) == false)
                                        leader.text = relationData.remark;
                                }
                                Utility.GetComponetInChild<Text>(objValid, "MemberCount").text = data.memberCount.ToString();
                                Utility.GetComponetInChild<Text>(objValid, "Level").text = data.level.ToString();
                            }
                        }
                        break;
                    }
                #endregion
                #region weapon
                case SortListType.SORTLIST_WEAPON:
                    {
                        ItemSortListEntry data = a_data as ItemSortListEntry;
                        if (data != null)
                        {
                            ItemData itemData = null;
                            if (data.ownerId == PlayerBaseData.GetInstance().RoleID)
                            {
                                itemData = ItemDataManager.GetInstance().GetItem(data.id);
                            }
                            else
                            {
                                itemData = ItemDataManager.CreateItemDataFromTable((int)data.itemId);
                                if (itemData != null)
                                {
                                    itemData.StrengthenLevel = (int)data.strengthen;
                                    itemData.EquipType = (EEquipType)data.equipType;
                                    itemData.GrowthAttrType = (EGrowthAttrType)data.growthAttr;
                                }
                            }

                            bool bValid = itemData != null;
                            objValid.SetActive(bValid);
                            objInvalid.SetActive(!bValid);

                            if (bValid)
                            {
                                _SetupRanking(objValid, data.ranking);

                                Utility.FindGameObject(objValid, "Icon").SetActive(true);
                                ComItem comItem = objValid.GetComponentInChildren<ComItem>();
                                if (comItem == null)
                                {
                                    comItem = CreateComItem(Utility.FindGameObject(objValid, "Icon"));
                                }

                                comItem.Setup(itemData, (var1, var2) =>
                                {
                                    if (data.ownerId == PlayerBaseData.GetInstance().RoleID)
                                    {
                                        //ItemTipManager.GetInstance().ShowTipWithCompareItem(var2, _GetCompareItem(var2));
                                        ItemTipManager.GetInstance().ShowTip(var2);
                                    }
                                    else
                                    {
                                        WorldSortListWatchDataReq kSend = new WorldSortListWatchDataReq();
                                        kSend.type = SortListDecoder.MakeSortListType(a_eType,(uint)subType);
                                        kSend.id = data.id;
                                        NetManager.Instance().SendCommand<WorldSortListWatchDataReq>(ServerType.GATE_SERVER, kSend);

                                        WaitNetMessageManager.GetInstance().Wait(WorldChatLinkDataRet.MsgID, msgData =>
                                        {
                                            int iPos = 0;
                                            byte type = 0;
                                            BaseDLL.decode_int8(msgData.bytes, ref iPos, ref type);
                                            switch (type)
                                            {
                                                case 1:
                                                    {
                                                        var items = ItemDecoder.Decode(msgData.bytes, ref iPos, msgData.bytes.Length);
                                                        if (items != null && items.Count > 0)
                                                        {
                                                            var curItemData = ItemDataManager.GetInstance().CreateItemDataFromNet(items[0]);
                                                            if (curItemData != null)
                                                            {
                                                                var curItemSuitObj = EquipSuitDataManager.GetInstance().GetSelfEquipSuitObj(curItemData.SuitID);
                                                                ItemTipManager.GetInstance().ShowOtherPlayerTip(curItemData, curItemSuitObj);
                                                            }
                                                        }
                                                        break;
                                                    }
                                            }
                                        });
                                    }

                                });

                                Utility.GetComponetInChild<Text>(objValid, "Name").text = itemData.Name;
                                Utility.GetComponetInChild<Text>(objValid, "Slot").text = itemData.GetThirdTypeDesc();
                                Utility.GetComponetInChild<Text>(objValid, "Level").text = itemData.LevelLimit.ToString();
                                Text owner = Utility.GetComponetInChild<Text>(objValid, "Owner");
                                owner.text = data.ownerName;
                                RelationData relationData = null;
                                RelationDataManager.GetInstance().FindPlayerIsRelation(data.id, ref relationData);
                                if (relationData != null)
                                {
                                    if (string.IsNullOrEmpty(relationData.remark) == false)
                                        owner.text = relationData.remark;
                                }
                                Utility.GetComponetInChild<Text>(objValid, "Score").text = data.score.ToString();
                            }
                        }
                        break;
                    }
                #endregion
                #region achievement
                case SortListType.SORTLIST_ACHIEVEMENT_SCORE:
                    {
                        AchievementScoreSortListEntry data = a_data as AchievementScoreSortListEntry;
                        if (null != data)
                        {
                            _SetupRanking(objValid, data.ranking);
                            Text name = Utility.GetComponetInChild<Text>(objValid, "Name");
                            name.text = data.name;
                            RelationData relationData = null;
                            RelationDataManager.GetInstance().FindPlayerIsRelation(data.id, ref relationData);
                            if (relationData != null)
                            {
                                if (string.IsNullOrEmpty(relationData.remark) == false)
                                    name.text = relationData.remark;
                            }
                            Utility.GetComponetInChild<Text>(objValid, "Job").text = _GetJobName(data.occu);
                            Utility.GetComponetInChild<Text>(objValid, "Score").text = data.score.ToString();
                        }
                        break;
                    }
                #endregion
                #region adventureteam
                case SortListType.SORTLIST_ADVENTURE_TEAM_GRADE:
                    {
                        AdventureTeamGradeSortListEntry data = a_data as AdventureTeamGradeSortListEntry;
                        if (data != null)
                        {
                            bool bValid = true;
                            if (string.IsNullOrEmpty(data.adventureTeamName))
                            {
                                bValid = false;
                            }
                            objValid.CustomActive(bValid);
                            objInvalid.CustomActive(!bValid);

                            if (bValid)
                            {
                                _SetupRanking(objValid, data.ranking);
                                Utility.GetComponetInChild<Text>(objValid, "Name").text = data.adventureTeamName;
                                Utility.GetComponetInChild<Text>(objValid, "Level").text = data.adventureTeamLevel.ToString();
                                Utility.GetComponetInChild<Text>(objValid, "Point").text = data.adventureTeamScore.ToString();
                                var comGrade = Utility.GetComponetInChild<ComAdventureTeamGrade>(objValid, "Grade");
                                if (comGrade)
                                {
                                    bool isSucc = comGrade.SetGradeImg((ProtoTable.AdventureTeamGradeTable.eGradeEnum)data.adventureTeamGrade);
                                    comGrade.CustomActive(isSucc);
                                }
                                Image headImg = Utility.GetComponetInChild<Image>(objValid, "Rank/HeadImg");
                                string resPath = AdventureTeamDataManager.GetInstance().GetAdventureTeamTitleResPathByRanking(data.ranking);
                                if (headImg && !string.IsNullOrEmpty(resPath))
                                {
                                    ETCImageLoader.LoadSprite(ref headImg, resPath);
                                    headImg.CustomActive(true);
                                }
                                else
                                {
                                    headImg.sprite = null;
                                    headImg.CustomActive(false);
                                }
                            }
                        }
                        break;
                    }
                #endregion

                #region ChijiScore
                case SortListType.SORTLIST_CHIJI_SCORE:
                    {
                        ChijiScoreSortListEntry data = a_data as ChijiScoreSortListEntry;
                        if (data != null)
                        {
                            bool bValid = true;
                            if (data.name == null)
                            {
                                bValid = false;
                            }

                            if (bValid)
                            {
                                if (null != data)
                                {
                                    _SetupRanking(objValid, data.ranking);
                                    Utility.GetComponetInChild<Text>(objValid, "Name").text = data.name;
                                    Utility.GetComponetInChild<Text>(objValid, "Job").text = _GetJobName((int)data.occu);
                                    Utility.GetComponetInChild<Text>(objValid, "Score").text = data.score.ToString();
                                }
                            }
                        }
                    }
                    break;
                #endregion

                default: break;
            }
        }

        void _UpdateRanklistData(BaseSortList a_listData)
        {
            Ranklist ranklist = _GetRanklist((SortListType)a_listData.type.mainType);
            if (ranklist != null)
            {
                ranklist.data = a_listData;

                // 更新自己的排名数据，服务器发的有问题
                switch (ranklist.data.type.mainType)
                {
                    #region season
                    case (uint)SortListType.SORTLIST_1V1_SEASON:
                        {
                            ushort myRank = 0;
                            for (int i = 0; i < ranklist.data.entries.Count; ++i)
                            {
                                if (ranklist.data.entries[i].id == PlayerBaseData.GetInstance().RoleID)
                                {
                                    myRank = ranklist.data.entries[i].ranking;
                                    break;
                                }
                            }
                            SeasonSortListEntry selfEntry = new SeasonSortListEntry();
                            selfEntry.ranking = myRank;
                            selfEntry.name = PlayerBaseData.GetInstance().Name;
                            selfEntry.occu = (byte)PlayerBaseData.GetInstance().JobTableID;
                            selfEntry.level = PlayerBaseData.GetInstance().Level;
                            selfEntry.seasonLevel = (uint)SeasonDataManager.GetInstance().seasonLevel;
                            selfEntry.seasonStar = (uint)SeasonDataManager.GetInstance().seasonStar;
                            ranklist.data.selfEntry = selfEntry;
                            break;
                        }
                    #endregion
                    #region level
                    case (uint)SortListType.SORTLIST_LEVEL:
                        {
                            ushort myRank = 0;
                            for (int i = 0; i < ranklist.data.entries.Count; ++i)
                            {
                                if (ranklist.data.entries[i].id == PlayerBaseData.GetInstance().RoleID)
                                {
                                    myRank = ranklist.data.entries[i].ranking;
                                    break;
                                }
                            }
                            LevelSortListEntry selfEntry = new LevelSortListEntry();
                            selfEntry.ranking = myRank;
                            selfEntry.name = PlayerBaseData.GetInstance().Name;
                            selfEntry.occu = (byte)PlayerBaseData.GetInstance().JobTableID;
                            selfEntry.level = PlayerBaseData.GetInstance().Level;
                            ranklist.data.selfEntry = selfEntry;
                            break;
                        }
                    #endregion
                    #region tower
                    case (uint)SortListType.SORTLIST_TOWER:
                        {
                            ushort myRank = 0;
                            for (int i = 0; i < ranklist.data.entries.Count; ++i)
                            {
                                if (ranklist.data.entries[i].id == PlayerBaseData.GetInstance().RoleID)
                                {
                                    myRank = ranklist.data.entries[i].ranking;
                                    break;
                                }
                            }
                            DeathTowerSortListEntry selfEntry = new DeathTowerSortListEntry();
                            selfEntry.ranking = myRank;
                            selfEntry.name = PlayerBaseData.GetInstance().Name;
                            selfEntry.occu = (byte)PlayerBaseData.GetInstance().JobTableID;
                            selfEntry.level = PlayerBaseData.GetInstance().Level;
                            selfEntry.layer = (uint)CountDataManager.GetInstance().GetCount("tower_top_floor_total");
                            selfEntry.usedTime = (uint)CountDataManager.GetInstance().GetCount("tower_used_time_total");
                            ranklist.data.selfEntry = selfEntry;
                            break;
                        }
                    #endregion
                    #region guild
                    case (uint)SortListType.SORTLIST_GUILD_LEVEL:
                        {
                            GuildLevelSortListEntry selfEntry = new GuildLevelSortListEntry();
                            if (GuildDataManager.GetInstance().HasSelfGuild())
                            {
                                ushort myRank = 0;
                                for (int i = 0; i < ranklist.data.entries.Count; ++i)
                                {
                                    if (ranklist.data.entries[i].id == GuildDataManager.GetInstance().myGuild.uGUID)
                                    {
                                        myRank = ranklist.data.entries[i].ranking;
                                        break;
                                    }
                                }

                                selfEntry.ranking = myRank;
                                selfEntry.name = GuildDataManager.GetInstance().myGuild.strName;
                                selfEntry.leader = GuildDataManager.GetInstance().myGuild.leaderData.strName;
                                selfEntry.memberCount = (uint)GuildDataManager.GetInstance().myGuild.nMemberCount;
                                selfEntry.level = (ushort)GuildDataManager.GetInstance().myGuild.nLevel;
                                //selfEntry.fund = (uint)GuildDataManager.GetInstance().myGuild.nFund;
                                ranklist.data.selfEntry = selfEntry;
                            }
                            else
                            {
                                selfEntry.ranking = 0;
                                selfEntry.name = "-";
                                selfEntry.leader = "-";
                                selfEntry.memberCount = 0;
                                selfEntry.level = 0;
                                //selfEntry.fund = 0;
                                ranklist.data.selfEntry = selfEntry;
                            }

                            break;
                        }
                    #endregion
                    #region weapon
                    case (uint)SortListType.SORTLIST_WEAPON:
                        {
                            ushort myRank = 0;
                            for (int i = 0; i < ranklist.data.entries.Count; ++i)
                            {
                                ItemSortListEntry rankData = ranklist.data.entries[i] as ItemSortListEntry;
                                if (rankData != null && rankData.ownerId == PlayerBaseData.GetInstance().RoleID)
                                {
                                    myRank = ranklist.data.entries[i].ranking;
                                    break;
                                }
                            }

                            if (myRank == 0)
                            {
                                bool haveType = false;
                                foreach (var item in weaponsList)
                                {
                                    if (item != null)
                                    {
                                        if (item.Id == ranklist.data.type.subType)
                                        {
                                            haveType = true;
                                            break;
                                        }
                                    }
                                }
                                if(haveType)
                                    ranklist.data.selfEntry = _GetBestItem((ItemTable.eThirdType)ranklist.data.type.subType);
                            }
                            break;
                        }
                    #endregion
                    #region achievement
                    case (uint)SortListType.SORTLIST_ACHIEVEMENT_SCORE:
                        {
                            ushort myRank = 0;
                            uint myScore = (uint)PlayerBaseData.GetInstance().AchievementScore;
                            for (int i = 0; i < ranklist.data.entries.Count; ++i)
                            {
                                if (ranklist.data.entries[i].id == PlayerBaseData.GetInstance().RoleID)
                                {
                                    myRank = ranklist.data.entries[i].ranking;
                                    myScore = (ranklist.data.entries[i] as AchievementScoreSortListEntry).score;
                                    break;
                                }
                            }
                            AchievementScoreSortListEntry selfEntry = new AchievementScoreSortListEntry();
                            selfEntry.ranking = myRank;
                            selfEntry.name = PlayerBaseData.GetInstance().Name;
                            selfEntry.occu = (byte)PlayerBaseData.GetInstance().JobTableID;
                            selfEntry.score = myScore;
                            ranklist.data.selfEntry = selfEntry;
                            break;
                        }
                    #endregion
                    #region adventureteam
                    case (uint)SortListType.SORTLIST_ADVENTURE_TEAM_GRADE:
                        {
                            ushort myRank = 0;
                            for (int i = 0; i < ranklist.data.entries.Count; ++i)
                            {
                                if (ranklist.data.entries[i].id == ClientApplication.playerinfo.accid)
                                {
                                    myRank = ranklist.data.entries[i].ranking;
                                    break;
                                }
                            }
                            AdventureTeamGradeSortListEntry selfEntry = new AdventureTeamGradeSortListEntry();
                            selfEntry.ranking = myRank;
                            selfEntry.adventureTeamLevel = (ushort)AdventureTeamDataManager.GetInstance().GetAdventureTeamLevel();
                            selfEntry.adventureTeamName = AdventureTeamDataManager.GetInstance().GetAdventureTeamName();
                            selfEntry.adventureTeamGrade = (uint)AdventureTeamDataManager.GetInstance().GetAdventureTeamGradeTableId();
                            selfEntry.adventureTeamScore = (uint)AdventureTeamDataManager.GetInstance().GetAdventureTeamScore();
                            ranklist.data.selfEntry = selfEntry;
                            break;
                        }
                    #endregion
                    #region ChijiScore 
                    case (uint)SortListType.SORTLIST_CHIJI_SCORE:
                        {
                            ushort myRank = 0;
                            uint myChijiScore = (uint)PlayerBaseData.GetInstance().ChijiScore;
                            for (int i = 0; i < ranklist.data.entries.Count; ++i)
                            {
                                if (ranklist.data.entries[i].id == PlayerBaseData.GetInstance().RoleID)
                                {
                                    myRank = ranklist.data.entries[i].ranking;
                                    break;
                                }
                            }

                            ChijiScoreSortListEntry selfEntry = new ChijiScoreSortListEntry();
                            selfEntry.ranking = myRank;
                            selfEntry.name = PlayerBaseData.GetInstance().Name;
                            selfEntry.occu = (byte)PlayerBaseData.GetInstance().JobTableID;
                            selfEntry.score = myChijiScore;
                            ranklist.data.selfEntry = selfEntry;
                        }
                        break;
                    #endregion
                    default: break;
                }
            }

        }

        void _SetupRanking(GameObject a_objLine, int a_nRanking)
        {
            Text labLevel = Utility.GetComponetInChild<Text>(a_objLine, "Rank/Lab");
            Image imgLevel = Utility.GetComponetInChild<Image>(a_objLine, "Rank/Icon");
            if (a_nRanking < 1)
            {
                labLevel.gameObject.SetActive(true);
                imgLevel.gameObject.SetActive(false);
                labLevel.text = TR.Value("ranklist_not_on_the_list");
            }
            else if (a_nRanking >= 1 && a_nRanking <= 3)
            {
                labLevel.gameObject.SetActive(false);
                imgLevel.gameObject.SetActive(true);
                // imgLevel.sprite = AssetLoader.instance.LoadRes(m_rankIcons[a_nRanking - 1], typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref imgLevel, m_rankIcons[a_nRanking - 1]);
                imgLevel.SetNativeSize();
            }
            else
            {
                labLevel.gameObject.SetActive(true);
                imgLevel.gameObject.SetActive(false);
                labLevel.text = a_nRanking.ToString();
            }

            // GameObject[] objEffects = new GameObject[3];
            // objEffects[0] = Utility.FindGameObject(a_objLine, string.Format("Effect{0}", 1), false);
            // objEffects[1] = Utility.FindGameObject(a_objLine, string.Format("Effect{0}", 2), false);
            // objEffects[2] = Utility.FindGameObject(a_objLine, string.Format("Effect{0}", 3), false);
            // for (int i = 0; i < 3; ++i)
            // {
            //     if (objEffects[i] != null)
            //     {
            //         objEffects[i].SetActive(false);
            //     }
            // }

            Image imgBG = Utility.GetComponetInChild<Image>(a_objLine, "BG");
            //新设计保留底图
            if (imgBG != null)
            {
                //imgBG.gameObject.SetActive(false);
                ETCImageLoader.LoadSprite(ref imgBG, "UI/Image/NewPacked/Paihangbang.png:Paihangbang_ItemBg_01");
            }

            if (a_nRanking == 1 || a_nRanking == 2 || a_nRanking == 3)
            {
                // if (objEffects[a_nRanking-1] == null)
                // {
                //     objEffects[a_nRanking - 1] = AssetLoader.GetInstance().LoadResAsGameObject(string.Format("Effects/Scene_effects/EffectUI/EffUI_ph0{0}", a_nRanking));
                //     objEffects[a_nRanking - 1].name = string.Format("Effect{0}", a_nRanking);
                //     objEffects[a_nRanking - 1].transform.SetParent(a_objLine.transform, false);
                // }
                // objEffects[a_nRanking - 1].SetActive(true);

                if (imgBG != null)
                {
                    //imgBG.gameObject.SetActive(true);
                    //imgBG.sprite = AssetLoader.GetInstance().LoadRes(
                    //    string.Format("UI/Image/Packed/p_UI_List.png:UI_Paihangbang_Tubiao_Di_0{0}", a_nRanking), typeof(Sprite)).obj as Sprite;
                    if (a_nRanking-1 < m_rankBgs.Length && null != m_rankBgs[a_nRanking-1])
                    {
                        ETCImageLoader.LoadSprite(ref imgBG, m_rankBgs[a_nRanking-1]);    
                    }
                    
                }
            }
        }

        string _GetTimeUsed(int a_time)
        {
            a_time /= 1000;

            int second = 0;
            int minute = 0;
            int hour = 0;
            second = a_time % 60;
            int temp = a_time / 60;
            if (temp > 0)
            {
                minute = temp % 60;
                hour = temp / 60;
            }

            return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
        }

        ItemData _GetCompareItem(ItemData item)
        {
            ItemData compareItem = null;
            if (item != null)
            {
                List<ulong> guids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
                if (guids != null)
                {
                    for (int i = 0; i < guids.Count; ++i)
                    {
                        ItemData tempItem = ItemDataManager.GetInstance().GetItem(guids[i]);
                        if (
                            tempItem != null &&
                            tempItem.GUID != item.GUID &&
                            tempItem.IsWearSoltEqual(item)
                            )
                        {
                            compareItem = tempItem;
                            break;
                        }
                    }
                }
            }
            return compareItem;
        }

        ItemSortListEntry _GetBestItem(ItemTable.eThirdType a_eType)
        {
            ItemData item = null;
            List<ulong> guids = ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.Equip, ItemTable.eSubType.WEAPON);
            for (int i = 0; i < guids.Count; ++i)
            {
                ItemData temp = ItemDataManager.GetInstance().GetItem(guids[i]);
                if (temp == null) continue;
                if (a_eType == ItemTable.eThirdType.TT_NONE || temp.ThirdType == a_eType)
                {
                    if (item == null || item.finalRateScore < temp.finalRateScore)
                    {
                        item = temp;
                    }
                }

            }
            guids = ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.WearEquip, ItemTable.eSubType.WEAPON);
            for (int i = 0; i < guids.Count; ++i)
            {
                ItemData temp = ItemDataManager.GetInstance().GetItem(guids[i]);
                if (temp == null) continue;
                if (a_eType == ItemTable.eThirdType.TT_NONE || temp.ThirdType == a_eType)
                {
                    if (item == null || item.finalRateScore < temp.finalRateScore)
                    {
                        item = temp;
                    }
                }
            }

            if (item != null)
            {
                ItemSortListEntry selfEntry = new ItemSortListEntry();
                selfEntry.ranking = 0;
                selfEntry.ownerName = PlayerBaseData.GetInstance().Name;
                selfEntry.ownerId = PlayerBaseData.GetInstance().RoleID;
                selfEntry.name = item.Name;
                selfEntry.id = item.GUID;
                selfEntry.itemId = (uint)item.TableID;
                selfEntry.level = (ushort)item.LevelLimit;
                selfEntry.score = (uint)item.finalRateScore;
                selfEntry.strengthen = (uint)item.StrengthenLevel;
                selfEntry.equipType = (byte)item.EquipType;
                selfEntry.growthAttr = (byte)item.GrowthAttrType;

                return selfEntry;
            }
            else
            {
                ItemSortListEntry selfEntry = new ItemSortListEntry();
                selfEntry.ranking = 0;
                selfEntry.ownerName = string.Empty;
                selfEntry.ownerId = 0;
                selfEntry.name = string.Empty;
                selfEntry.id = 0;
                selfEntry.itemId = 0;
                selfEntry.level = 0;
                selfEntry.score = 0;
                selfEntry.strengthen = 0;

                return selfEntry;
            }
        }

        string _GetJobName(int a_nJobID)
        {
            ProtoTable.JobTable jobTable = TableManager.instance.GetTableItem<ProtoTable.JobTable>(a_nJobID);
            if (jobTable != null)
            {
                return jobTable.Name;
            }
            return string.Empty;
        }

       Ranklist _GetRanklist(SortListType a_eType)
        {
            for (int i = 0; i < m_arrRanklist.Count; ++i)
            {
                Ranklist rankList = m_arrRanklist[i];
                if (rankList.arrTypes.Count > 0)
                {
                    if (rankList.arrTypes[0] == a_eType)
                    {
                        return rankList;
                    }
                }
            }
            return null;
        }

        void _OnRanklistUpdate(UIEvent a_event)
        {
            BaseSortList list = a_event.Param1 as BaseSortList;
            if(null==list)
            {
                Logger.LogError("baseSortlist is null");
            }
            if (list.type.mainType != (uint)m_currRanklist)
            {
                return;
            }

            Ranklist ranklist = _GetRanklist(m_currRanklist);
            if (ranklist != null)
            {
                _UpdateRanklistData(list);

                // list
                if (ranklist != null)
                {
                    if (ranklist.rankListControl != null)
                    {
                        ranklist.rankListControl.SetElementAmount(ranklist.data.entries.Count);
                        if (ranklist.data.entries.Count > 0)
                        {
                            ranklist.rankListControl.EnsureElementVisable(0);
                        }
                    }

                    // self
                    ranklist.objMyRank.SetActive(true);
                    _SetupOneLine(ranklist.objMyRank, m_currRanklist,(int)list.type.subType, ranklist.data.selfEntry, false, true);
                }
            }
        }

        void _OnRequestJoinSuccess(UIEvent a_event)
        {
            ulong uGuildGUID = (ulong)a_event.Param1;
            Ranklist ranklist = _GetRanklist(SortListType.SORTLIST_GUILD_LEVEL);
            if (ranklist != null)
            {
                for (int i = 0; i < ranklist.data.entries.Count; ++i)
                {
                    if (ranklist.data.entries[i].id == uGuildGUID)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_request_join_success", ranklist.data.entries[i].name));
                        break;
                    }
                }
            }
        }
        
        void _RequestRanklist(int a_type,int subType, int a_startIndex,int a_count)
        {
            //如果是武器排行榜，SortListType的三个参数1：SortListType.SORTLIST_WEAPON，2：道具表的ThirdTypeName，3：0
            WorldSortListReq msg = new WorldSortListReq();
            msg.type = SortListDecoder.MakeSortListType((SortListType)a_type,(uint)subType);
            msg.start = (ushort)a_startIndex;
            msg.num = (ushort)a_count;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait(WorldSortListRet.MsgID, msgRet =>
            {
                if (msgRet == null)
                {
                    return;
                }
                int pos = 0;
                BaseSortList arrRecods = SortListDecoder.Decode(msgRet.bytes, ref pos, msgRet.bytes.Length);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RanklistUpdated, arrRecods);
            });
        }

        #region ExtraUIBind
        private GameObject mToggle250 = null;

        protected override void _bindExUI()
        {
            mToggle250 = mBind.GetGameObject("Toggle250");
        }

        protected override void _unbindExUI()
        {
            mToggle250 = null;
        }
        #endregion
    }
}
