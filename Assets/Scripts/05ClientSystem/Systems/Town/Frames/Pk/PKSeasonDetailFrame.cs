using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using ProtoTable;
using UnityEngine.Assertions;
using Scripts.UI;

namespace GameClient
{
    enum ESeasonDetailType
    {
        Base,
        Attr,
        Daily,
    }

    class PKSeasonDetailFrame : ClientFrame
    {
        #region ExtraUIBind
        private PKSeasonDetaiFashionItem mPkSeasonDetaiFashionItem = null;
        private CommonTabToggleGroup mCommonVerticalTab = null;
        private GameObject mBase = null;
        private GameObject mAttr = null;
        private GameObject mDaily = null;

        protected sealed override void _bindExUI()
        {
            mPkSeasonDetaiFashionItem = mBind.GetCom<PKSeasonDetaiFashionItem>("pkSeasonDetaiFashionItem");
            mCommonVerticalTab = mBind.GetCom<CommonTabToggleGroup>("CommonVerticalTab");
            mBase = mBind.GetGameObject("Base");
            mAttr = mBind.GetGameObject("Attr");
            mDaily = mBind.GetGameObject("Daily");
        }

        protected sealed override void _unbindExUI()
        {
            mPkSeasonDetaiFashionItem = null;
            mCommonVerticalTab = null;
            mBase = null;
            mAttr = null;
            mDaily = null;
        }
        #endregion
        
        [UIControl("Content/Groups/Base/SeasonGroup/RightTop/Text_T")]
        Text m_labSeasonTimeLeft;

        [UIControl("Content/Groups/Base/SeasonGroup/RightBottom/Scroll View")]
        ComUIListScript m_rewardList;

        [UIObject("Content/Groups/Base/SeasonGroup/RightTop/TitleGroup/Name/Numbers")]
        GameObject m_objDigitRoot;

        [UIObject("Content/Groups/Base/SeasonGroup/RightTop/TitleGroup/Name/Numbers/Number")]
        GameObject m_objDigitTemplate;

        [UIControl("Content/Groups/Attr/WeekGroup/RightTop/Text_T")]
        Text m_labMyAttrTitle;

        [UIControl("Content/Groups/Attr/WeekGroup/RightTop/Text_B")]
        Text m_labMyAttrContent;
        
        [UIControl("Content/Groups/Attr/WeekGroup/RightBottom/Scroll View")]
        ComUIListScript m_attrList;

        DelayCallUnitHandle m_repeatCallSeasonTime;

        Text m_labAttrTime;
        DelayCallUnitHandle m_repeatCallAttrTime;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk/PKSeasonDetail";
        }

        protected override void _OnOpenFrame()
        {
            _InitBaseGroup();
            _InitAttrGroup();
            _InitDailyGroup();
            _BindGameEvent();

            ESeasonDetailType type = ESeasonDetailType.Base;
            if (userData != null)
            {
                type = (ESeasonDetailType)userData;
            }

            _InitTabs(type);

            UpdateDailyRedPoint();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideEnd);
        }

        protected override void _OnCloseFrame()
        {
            _ClearBaseGroup();
            _ClearAttrGroup();
            _ClearDailyGroup();
            _UnBindGameEvent();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.pkGuideStart);
        }

        void _BindGameEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PKSelfLevelUpdated, _OnSeasonRankUpdated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MissionUpdated, _OnMissionUpdated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SeasonStarted, _OnSeasonStarted);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3VoteEnterBattle, OnPk3v3VoteEnterBattle);
        }

        void _UnBindGameEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PKSelfLevelUpdated, _OnSeasonRankUpdated);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MissionUpdated, _OnMissionUpdated);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SeasonStarted, _OnSeasonStarted);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3VoteEnterBattle, OnPk3v3VoteEnterBattle);
        }

        void _InitTabs(ESeasonDetailType type)
        {
            if(mCommonVerticalTab != null)
            {
               mCommonVerticalTab.InitComTab(OnTabBtnClick, (int)type);
            }
        }

        void UpdateDailyRedPoint()
        {
            if(mCommonVerticalTab != null)
            {
                bool isFlag = RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.DailyProve);
                mCommonVerticalTab.OnSetRedPoint((int)ESeasonDetailType.Daily, isFlag);
            }
        }

        void OnTabBtnClick(CommonTabData commonTabData)
        {
            if(commonTabData == null)
            {
                return;
            }

            mBase.CustomActive(false);
            mAttr.CustomActive(false);
            mDaily.CustomActive(false);

            switch ((ESeasonDetailType)commonTabData.id)
            {
                case ESeasonDetailType.Base:
                    mBase.CustomActive(true);
                    _SelectMySeasonReward();
                    break;
                case ESeasonDetailType.Attr:
                    mAttr.CustomActive(true);
                    _SelectMySeasonAttr();
                    break;
                case ESeasonDetailType.Daily:
                    mDaily.CustomActive(true);
                    break;
            }
        }

        void _InitBaseGroup()
        {
            for (int i = 0; i < m_objDigitRoot.transform.childCount; ++i)
            {
                m_objDigitRoot.transform.GetChild(i).gameObject.SetActive(false);
            }
            int nIdx = 0;
            int nNumber = SeasonDataManager.GetInstance().seasonID;
            while (nNumber > 0)
            {
                int nDigit = nNumber % 10;
                GameObject objDigit = null;
                if (nIdx < m_objDigitRoot.transform.childCount)
                {
                    objDigit = m_objDigitRoot.transform.GetChild(nIdx).gameObject;
                }
                else
                {
                    objDigit = GameObject.Instantiate(m_objDigitTemplate);
                    objDigit.transform.SetParent(m_objDigitRoot.transform, false);
                    objDigit.transform.SetAsFirstSibling();
                }
                objDigit.SetActive(true);

                //objDigit.GetComponent<Image>().sprite =
                //    AssetLoader.GetInstance().LoadRes(TR.Value("pk_rank_detail_digit_path", nDigit), typeof(Sprite)).obj as Sprite;
                Image img = objDigit.GetComponent<Image>();
                ETCImageLoader.LoadSprite(ref img, TR.Value("pk_rank_detail_digit_path", nDigit));

                nNumber = nNumber / 10;
                nIdx++;
            }

            m_repeatCallSeasonTime = ClientSystemManager.GetInstance().delayCaller.RepeatCall(1000, () =>
            {
                m_labSeasonTimeLeft.text = TR.Value("pk_rank_detail_time_left", 
                    SeasonDataManager.GetInstance().seasonID, _GetTimeLeft(SeasonDataManager.GetInstance().seasonEndTime));
            }, 9999999, true);

            m_rewardList.Initialize();
            m_rewardList.onBindItem = (obj) =>
            {
                ComSeasonReward comSeasonReward = obj.GetComponent<ComSeasonReward>();
                comSeasonReward.arrComItems.Add(CreateComItem(comSeasonReward.objReward0));
                comSeasonReward.arrComItems.Add(CreateComItem(comSeasonReward.objReward1));
                comSeasonReward.arrComItems.Add(CreateComItem(comSeasonReward.objReward2));
                comSeasonReward.arrComItems.Add(CreateComItem(comSeasonReward.objReward3));
                comSeasonReward.arrComItems.Add(CreateComItem(comSeasonReward.objReward4));
                comSeasonReward.arrComItems.Add(CreateComItem(comSeasonReward.objReward5));
                return comSeasonReward;
            };
            m_rewardList.onItemVisiable = item =>
            {
                List<SeasonLevel> arrSeasonRewards = SeasonDataManager.GetInstance().GetSeasonRewards();
                if (item.m_index >= 0 && item.m_index < arrSeasonRewards.Count)
                {
                    SeasonLevel data = arrSeasonRewards[item.m_index];
                    ComSeasonReward ui = item.gameObjectBindScript as ComSeasonReward;
                    // ui.imgSeasonIcon.sprite = AssetLoader.GetInstance().LoadRes(data.levelTable.Icon, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref ui.imgSeasonIcon, data.levelTable.Icon);
                    ui.labSeasonName.text = 
                        SeasonDataManager.GetInstance().GetRankName(data.levelTable.ID, false);
                    for (int i = 0; i < ui.arrComItems.Count; ++i)
                    {
                        if (i < data.levelTable.SeasonRewards.Count)
                        {
                            string[] values = data.levelTable.SeasonRewards[i].Split(',');
                            if (values.Length == 2)
                            {
                                ItemData itemData = ItemDataManager.CreateItemDataFromTable(int.Parse(values[0]));
                                if (itemData != null)
                                {
                                    itemData.Count = int.Parse(values[1]);

                                    ui.arrComItems[i].gameObject.SetActive(true);
                                    ui.arrComItems[i].Setup(itemData, (var1, var2) =>
                                    {
                                        ItemTipManager.GetInstance().ShowTip(var2);
                                    });

                                    continue;
                                }
                            }
                        }

                        ui.arrComItems[i].gameObject.transform.parent.gameObject.CustomActive(false);
                        //ui.CustomActive(false);
                    }


                    ui.objMySeasonLevel.SetActive(
                        SeasonDataManager.GetInstance().IsMainLevelSame(data.levelTable.ID, CountDataManager.GetInstance().GetCount(string.Format("season_max_level")))
                        );
                }
            };

            m_rewardList.SetElementAmount(SeasonDataManager.GetInstance().GetSeasonRewards().Count);
            
            //初始化至尊王者送时装的显示信息
            if (mPkSeasonDetaiFashionItem != null)
            {
                mPkSeasonDetaiFashionItem.InitItem();
            }
        }

        void _ClearBaseGroup()
        {
            ClientSystemManager.GetInstance().delayCaller.StopItem(m_repeatCallSeasonTime);
        }

        string _GetTimeLeft(int a_nEndTime)
        {
            int nTimeLeft = a_nEndTime - (int)TimeManager.GetInstance().GetServerTime();
            if (nTimeLeft > 0)
            {
                int second = 0;
                int minute = 0;
                int hour = 0;
                int day = 0;
                second = nTimeLeft % 60;
                int temp = nTimeLeft / 60;
                if (temp > 0)
                {
                    minute = temp % 60;
                    temp = temp / 60;
                    if (temp > 0)
                    {
                        hour = temp % 24;
                        day = temp / 24;
                    }
                }

                string value = "";
                value += string.Format("{0}天", day);
                value += string.Format("{0:D2}小时", hour);
                value += string.Format("{0:D2}分", minute);
                value += string.Format("{0:D2}秒", second);
                return value;
            }
            else
            {
                return string.Empty;
            }
        }

        void _SelectMySeasonReward()
        {
            List<SeasonLevel> arrSeasonRewards = SeasonDataManager.GetInstance().GetSeasonRewards();
            for (int i = 0; i < arrSeasonRewards.Count; ++i)
            {
                if (
                    SeasonDataManager.GetInstance().IsMainLevelSame(
                        arrSeasonRewards[i].levelTable.ID, 
                        SeasonDataManager.GetInstance().seasonLevel
                    )
                    )
                {
                    m_rewardList.EnsureElementVisable(i);
                    break;
                }
            }
        }

        void _InitAttrGroup()
        {
            m_labMyAttrTitle.text = TR.Value("pk_rank_detail_attr_desc",
                SeasonDataManager.GetInstance().GetRankName(SeasonDataManager.GetInstance().seasonAttrMappedSeasonID));
            SeasonLevel seasonLevel = SeasonDataManager.GetInstance().GetSeasonAttr(SeasonDataManager.GetInstance().seasonAttrID);
            if (seasonLevel != null)
            {
                m_labMyAttrContent.text = seasonLevel.strFormatAttr;
            }
            else
            {
                m_labMyAttrContent.text = string.Empty;
            }

            m_attrList.Initialize();
            m_attrList.onItemVisiable = var =>
            {
                
                List<SeasonLevel> seasonAttrs = SeasonDataManager.GetInstance().GetSeasonAttrs();
                if (var.m_index >= 0 && var.m_index < seasonAttrs.Count)
                {
                    SeasonLevel data = seasonAttrs[var.m_index];
                    ComSeasonAttr ui = var.gameObject.GetComponent<ComSeasonAttr>();
                    // ui.imgSeasonIcon.sprite = AssetLoader.GetInstance().LoadRes(data.levelTable.Icon, typeof(Sprite)).obj as Sprite;
                    ETCImageLoader.LoadSprite(ref ui.imgSeasonIcon, data.levelTable.Icon);
                    ui.labSeasonName.text = TR.Value(string.Format("pk_rank_attr_name{0}", data.attrTable.ID));
                    ui.labSeasonAttr.text = data.strFormatAttr;

                    if (data.attrTable.ID == SeasonDataManager.GetInstance().GetNextAttrID())
                    {
                        ui.objWillGet.SetActive(true);
                        m_labAttrTime = ui.labWillGetDesc;
                    }
                    else
                    {
                        ui.objWillGet.SetActive(false);
                    }
                }
            };

            m_attrList.SetElementAmount(SeasonDataManager.GetInstance().GetSeasonAttrs().Count);

            m_repeatCallAttrTime = ClientSystemManager.GetInstance().delayCaller.RepeatCall(1000, () =>
            {
                if (m_labAttrTime != null)
                {
                    m_labAttrTime.text = TR.Value("pk_rank_detail_attr_time_left", _GetTimeLeft(SeasonDataManager.GetInstance().seasonAttrEndTime));
                }
            }, 9999999, true);
            
            SeasonDataManager.GetInstance().RequestSelfPKRank();
        }

        void _ClearAttrGroup()
        {
            ClientSystemManager.GetInstance().delayCaller.StopItem(m_repeatCallAttrTime);

            m_labAttrTime = null;
        }

        void _SelectMySeasonAttr()
        {
            List<SeasonLevel> seasonAttrs = SeasonDataManager.GetInstance().GetSeasonAttrs();
            for (int i = 0; i < seasonAttrs.Count; ++i)
            {
                if (seasonAttrs[i].attrTable.ID == SeasonDataManager.GetInstance().GetNextAttrID())
                {
                    m_attrList.EnsureElementVisable(i);
                    break;
                }
            }
        }

        void _InitDailyGroup()
        {
            _SetupDailyRewards();
        }

        void _ClearDailyGroup()
        {

        }

        void _SetupDailyRewards()
        {
            UpdateDailyRedPoint();

            int nIdx = 1;
            var missionList = MissionManager.GetInstance().taskGroup.Values.ToList();
            for (int i = 0; i < missionList.Count; ++i)
            {
                uint nTaskID = missionList[i].taskID;
                if (!Utility.IsDailyProve(nTaskID))
                {
                    continue;
                }

                MissionTable missionTableData = TableManager.GetInstance().GetTableItem<MissionTable>((int)nTaskID);
                Utility.DailyProveTaskConfig missionDetailData = Utility.GetDailyProveTaskConfig((int)nTaskID);

                GameObject objRoot = Utility.FindGameObject(frame, string.Format("Content/Groups/Daily/Daily{0}", nIdx));
                Utility.GetComponetInChild<Text>(objRoot, "Description").text = MissionManager.GetInstance().GetMissionName(nTaskID);
                Utility.GetComponetInChild<Text>(objRoot, "ProgressBar/Text").text = string.Format("{0}/{1}", missionDetailData.iPreValue, missionDetailData.iAftValue);
                //Utility.GetComponetInChild<Slider>(objRoot, "ProgressBar").value = missionDetailData.Amount;
                var awards = missionTableData.Award.Split(',');
                if (awards.Length > 0)
                {
                    var award = awards[0].Split('_');
                    if (award.Length == 2)
                    {
                        ItemData data = ItemDataManager.CreateItemDataFromTable(int.Parse(award[0]));
                        if (data != null)
                        {
                            data.Count = int.Parse(award[1]);

                            GameObject objItemRoot = Utility.FindGameObject(objRoot, "award");
                            ComItem comItem = objItemRoot.GetComponentInChildren<ComItem>();
                            if (comItem == null)
                            {
                                comItem = CreateComItem(objItemRoot);
                            }
                            comItem.Setup(data, (var1, var2) =>
                            {
                                ItemTipManager.GetInstance().ShowTip(var2);
                            });
                        }
                    }
                }

                GameObject objAcquired = Utility.FindGameObject(objRoot, "Acquired");
                GameObject objUnAcquired = Utility.FindGameObject(objRoot, "UnAcquired");
                GameObject objUnComplete = Utility.FindGameObject(objRoot, "UnComplete");
                objAcquired.SetActive(false);
                objUnAcquired.SetActive(false);
                objUnComplete.SetActive(false);
                if (missionList[i].status >= (int)TaskStatus.TASK_OVER)
                {
                    objAcquired.SetActive(true);
                }
                else
                {
                    if (missionList[i].status == (int)TaskStatus.TASK_FINISHED)
                    {
                        objUnAcquired.SetActive(true);
                    }
                    else
                    {
                        objUnComplete.SetActive(true);
                    }
                }

                Button btnReceive = Utility.GetComponetInChild<Button>(objRoot, "UnAcquired");
                btnReceive.onClick.RemoveAllListeners();
                btnReceive.onClick.AddListener(() =>
                {
                    MissionManager.GetInstance().sendCmdSubmitTask(nTaskID, TaskSubmitType.TASK_SUBMIT_UI, 0);
                });

                nIdx++;
                if (nIdx > 2)
                {
                    break;
                }
            }
        }

        void _OnSeasonRankUpdated(UIEvent a_event)
        {
            m_attrList.SetElementAmount(SeasonDataManager.GetInstance().GetSeasonAttrs().Count);
        }

        void _OnMissionUpdated(UIEvent a_event)
        {
            if (!Utility.IsDailyProve((uint)a_event.Param1))
            {
                return;
            }

            _SetupDailyRewards();
        }
        void OnPk3v3VoteEnterBattle(UIEvent iEvent)
        {
            frameMgr.CloseFrame(this);
        }
        void _OnSeasonStarted(UIEvent a_event)
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }
    }
}
