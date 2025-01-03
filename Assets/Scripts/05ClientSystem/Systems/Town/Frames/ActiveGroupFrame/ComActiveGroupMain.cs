using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Scripts.UI;
using Protocol;
using Network;

namespace GameClient
{
    public class ComActiveGroupMain : MonoBehaviour
    {
        public GameObject goParent;
        public ComActiveGroupMainTab mPrefabMainTab;
        public Text levelName;
        public Image levelImg;
        public Image levelInfoIcon;
        public string fmtPointString;
        public Text achievementPoint;
        public string fmtPointProcess;
        public Text processInfo;
        public Slider slider;
        public Text rank;
        public string rankDisable;
        public string rankEnableFmt;
        public GameObject[] itemParents;
        List<ComItem> comItems = new List<ComItem>(4);
        public string levelAwardDescFmt;
        public Text levelAwardDesc;
        public ComUIListScript comAchievementList;
        public ComAchievementGroupSubTabItems comSubTabItems;
        public GameObject goExpandParentPrefab;

        public StateController comTabStatus;
        public Text tabItemName;
        public Image tabItemIcon;
        public Text tabProcessInfo;
        public Slider tabSlider;
        public string[] dropCaptions = new string[0];
        public AchievementFilter[] dropValues = new AchievementFilter[0];
        public Dropdown tabDropDown;

        public string[] awardKeys = new string[0];
        public StateController comAwardStatus;

        public Vector2 anchorPos0 = Vector2.zero;
        public Vector2 anchorPos1 = Vector2.zero;
        public float sizeY0 = 0.0f;
        public float sizeY1 = 0.0f;
        public RectTransform rectScrollView;

        public RectTransform menuRect;
        public Vector2 menuRange;

        public enum AchievementFilter
        {
            AF_ALL = 0,
            AF_UNFINISHED,
            AF_FINISHED,
        }
        AchievementFilter mAchievementFilter = AchievementFilter.AF_ALL;

        public ChatType[] mChatTypes = new ChatType[0];
        public string[] mFmtContents = new string[0];
        public int[] mLinkIDs = new int[3] { 18, 19, 20 };
        public string mShareHint = string.Empty;
        AchievementGroupSubItemTable _menuData = null;
        public void OnMenuClicked(int channel)
        {
            if(null != _menuData)
            {
                if(channel >= 0 && channel < mChatTypes.Length)
                {
                    var chatType = mChatTypes[channel];
                    string content = string.Empty;
                    int fmtIndex = _menuData.Type;
                    if(fmtIndex >= 0 && fmtIndex < mFmtContents.Length)
                    {
                        content = mFmtContents[fmtIndex];
                    }
                    if(!string.IsNullOrEmpty(content))
                    {
                        string linkName = string.Empty;
                        if(_menuData.Type >= 0 && _menuData.Type < mLinkIDs.Length)
                        {
                            linkName = string.Format("{{X {0} {1}}}", mLinkIDs[_menuData.Type], _menuData.ID);
                        }
                        content = string.Format(content, linkName);
                        ChatManager.GetInstance().TrySendChatContent(chatType, () =>
                        {
                            _OnShareAchievementItem(chatType, content);
                        });
                    }
                }
                _menuData = null;
            }
        }

        void _OnShareAchievementItem(ChatType eChatType,string content)
        {
            ChatManager.GetInstance().SendChat(eChatType, content);

        }

        void _OnNetSyncChat(MsgDATA msg)
        {
            SceneSyncChat msgData = new SceneSyncChat();
            msgData.decode(msg.bytes);

            if (null == msgData || msgData.objid != PlayerBaseData.GetInstance().RoleID)
            {
                return;
            }

            SystemNotifyManager.SysNotifyTextAnimation(mShareHint, CommonTipsDesc.eShowMode.SI_IMMEDIATELY);
        }





        public void UpdateTabProcess()
        {
            if(null != tabProcessInfo)
            {
                int pre = AchievementGroupDataManager.GetInstance().GetSubItemsAValue(_MainItem, _MenuItem, true);
                int aft = AchievementGroupDataManager.GetInstance().GetSubItemsAValue(_MainItem, _MenuItem, false);
                float radio = Mathf.Clamp01(pre * 1.0f / aft);
                tabProcessInfo.text = string.Format(fmtPointProcess, pre, aft);

                if(null != tabSlider)
                {
                    tabSlider.value = radio;
                }
            }
        }

        public void UpdateTabBaseInfo()
        {
            if(null != tabItemName)
            {
                if(null != _MenuItem)
                {
                    tabItemName.text = _MenuItem.Name;
                }
                else if(null != _MainItem)
                {
                    tabItemName.text = _MainItem.PureName;
                }
            }

            if (null != tabItemIcon)
            {
                if(null != _MenuItem)
                {
                    ETCImageLoader.LoadSprite(ref tabItemIcon, _MenuItem.Icon);
                }
                else if(null != _MainItem)
                {
                    ETCImageLoader.LoadSprite(ref tabItemIcon, _MainItem.Icon);
                }
            }
        }

        public void InitTabDropDown()
        {
            mAchievementFilter = AchievementFilter.AF_ALL;
            if (null != tabDropDown)
            {
                tabDropDown.onValueChanged.RemoveListener(_OnDropDownChanged);
                tabDropDown.options.Clear();
                for(int i = 0; i < dropCaptions.Length; ++i)
                {
                    tabDropDown.options.Add(new Dropdown.OptionData(dropCaptions[i]));
                }
                tabDropDown.value = 0;
                if (null != tabDropDown.captionText && dropCaptions.Length > tabDropDown.value && tabDropDown.value >= 0)
                {
                    tabDropDown.captionText.text = dropCaptions[tabDropDown.value];
                }
                tabDropDown.onValueChanged.AddListener(_OnDropDownChanged);
            }
        }

        void _OnDropDownChanged(int option)
        {
            if (option >= 0 && option < dropValues.Length)
            {
                mAchievementFilter = dropValues[option];
                _AnsyInvoke();
            }
        }

        public void ChangeTabStatus(bool bEnable)
        {
            if(null != comTabStatus)
            {
                comTabStatus.Key = bEnable ? "Enable" : "Disable";
            }
            ResetScrollViewSize(bEnable);
        }

        public void ResetScrollViewSize(bool bMin)
        {
            Vector2 anchorPos = bMin ? anchorPos0 : anchorPos1;
            float sizeDeltaY = bMin ? sizeY0 : sizeY1;
            if(null != rectScrollView)
            {
                rectScrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeDeltaY);
                rectScrollView.anchoredPosition = anchorPos;
            }
        }

        void Awake()
        {
            if (null != itemParents)
            {
                for (int i = 0; i < itemParents.Length; ++i)
                {
                    if (null != itemParents[i])
                    {
                        comItems.Add(ComItemManager.Create(itemParents[i]));
                    }
                }
            }

            _InitAchievementList();


            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAchievementSecondMenuTabChanged, _OnAchievementSecondMenuTabChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAchievementGroupSubTabChanged, _OnAchievementGroupSubTabChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAchievementGroupSubTabChangedRepeated, _OnAchievementGroupSubTabChangedRepeated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAchievementMaskPropertyChanged, _OnAchievementMaskPropertyChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnShareAchievementItem, _OnShareAchievementItem);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAchievementScoreChanged, _OnAchievementScoreChanged);
            MissionManager.GetInstance().onAddNewMission += _OnAddNewMission;
            MissionManager.GetInstance().onUpdateMission += _OnUpdateMission;
            MissionManager.GetInstance().onDeleteMission += _OnDeleteMission;

            NetProcess.AddMsgHandler(SceneSyncChat.MsgID, _OnNetSyncChat);

            _InitRank();
            //InvokeMethod.InvokeInterval(this, 0.0f, 1.0,99999999.0f,_InitRank, _UpdateRank, null);
        }

        private void _mainToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                _UpdateRank();
            }
        }

        void _InitRank()
        {
            if (null != rank)
            {
                rank.text = rankDisable;
            }
        }

        bool _query = false;
        void _UpdateRank()
        {
            if(!_query)
            {
                _query = true;
                _RequestRanklist(0, 100, (AchievementScoreSortListEntry data) =>
                  {
                      if(null != data)
                      {
                          if(null != rank)
                          {
                              if(data.id == PlayerBaseData.GetInstance().RoleID)
                              {
                                  rank.text = string.Format(rankEnableFmt, data.ranking);
                              }
                              else
                              {
                                  rank.text = rankDisable;
                              }
                          }

                          //Logger.LogErrorFormat("query returned !!! ranking = {0} name={1} score={2}", data.ranking, data.name,data.score);
                      }
                      else
                      {
                          if (null != rank)
                          {
                              rank.text = rankDisable;
                          }
                      }
                      _query = false;
                  },
                () =>
                {
                    _query = false;
                });
            }
        }

        public void UpdateAwardStatus()
        {
            if(null != comAwardStatus && awardKeys.Length == 3)
            {
                var id = AchievementGroupDataManager.GetInstance().GetFirstUnAcquiredID();
                if(id == 0)
                {
                    //已经全部领取
                    comAwardStatus.Key = awardKeys[2];
                }
                else
                {
                    var item = TableManager.GetInstance().GetTableItem<AchievementLevelInfoTable>(id);
                    if(null == item)
                    {
                        comAwardStatus.Key = awardKeys[0];
                        Logger.LogErrorFormat("it is impossible !!!");
                    }
                    else
                    {
                        int iPoint = PlayerBaseData.GetInstance().AchievementScore;
                        if (iPoint >= item.Max)
                        {
                            //能够领取
                            comAwardStatus.Key = awardKeys[1];
                        }
                        else
                        {
                            //还达到领取要求
                            comAwardStatus.Key = awardKeys[0];
                        }
                    }
                }
            }
        }

        void _OnAddNewMission(uint taskID)
        {
            UpdateAchievementPoint();
            _AnsyInvoke();
            UpdateTabProcess();
        }

        void _OnUpdateMission(uint taskID)
        {
            UpdateAchievementPoint();
            _AnsyInvoke();
            UpdateTabProcess();
        }

        void _OnDeleteMission(uint taskID)
        {
            UpdateAchievementPoint();
            _AnsyInvoke();
            UpdateTabProcess();
        }

        void _OnAchievementGroupSubTabChanged(UIEvent uiEvent)
        {
            AchievementGroupMainItemTable mainItem = uiEvent.Param1 as AchievementGroupMainItemTable;
            _MainItem = mainItem;
            _MenuItem = null;
            if(null != tabDropDown)
            {
                tabDropDown.value = (int)AchievementFilter.AF_ALL;
            }
            GameObject expandParent = uiEvent.Param2 as GameObject;
            if(null != mainItem)
            {
                if(null != comSubTabItems)
                {
                    comSubTabItems.SetFlags(mainItem);
                }
            }

            if(null != expandParent && null != comSubTabItems)
            {
                Utility.AttachTo(comSubTabItems.gameObject, expandParent);
            }

            ChangeTabStatus(null != _MainItem && _MainItem.ChildTabs.Count == 1);

            _AnsyInvoke();
            UpdateTabProcess();
            UpdateTabBaseInfo();
        }

        void _OnAchievementGroupSubTabChangedRepeated(UIEvent uiEvent)
        {
            AchievementGroupMainItemTable mainItem = uiEvent.Param1 as AchievementGroupMainItemTable;
            _MainItem = mainItem;
            _MenuItem = null;
            if (null != tabDropDown)
            {
                tabDropDown.value = (int)AchievementFilter.AF_ALL;
            }
            GameObject expandParent = uiEvent.Param2 as GameObject;

            if (null != comSubTabItems)
            {
                comSubTabItems.SetFlags(null);
            }

            ChangeTabStatus(null != _MainItem && _MainItem.ChildTabs.Count == 1);

            _AnsyInvoke();
            UpdateTabProcess();
            UpdateTabBaseInfo();
        }

        void _OnAchievementMaskPropertyChanged(UIEvent uiEvent)
        {
            UpdateAwardStatus();
            _UpdateLevelAwardDesc();
        }

        void _OnAchievementScoreChanged(UIEvent uiEvent)
        {
            UpdateAwardStatus();
            UpdateAchievementPoint();
            _UpdateLevelAwardDesc();
        }

        void _OnShareAchievementItem(UIEvent uiEvent)
        {
            _menuData = uiEvent.Param1 as AchievementGroupSubItemTable;
            if(null != menuRect)
            {
                Vector2 _pos = Vector2.one;
                GameObject goCanvas = GameObject.Find("UIRoot/UI2DRoot");
                if(null != goCanvas)
                {
                    Canvas canvas = goCanvas.GetComponent<Canvas>();
                    if(null != canvas)
                    {
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(menuRect.parent.transform as RectTransform,
                            Input.mousePosition, canvas.worldCamera, out _pos);
                    }
                }

                _pos.y = Mathf.Clamp(_pos.y, menuRange.x, menuRange.y);
                _pos.x = menuRect.anchoredPosition.x;
                menuRect.anchoredPosition = _pos;
            }
        }

        void _OnAchievementSecondMenuTabChanged(UIEvent uiEvent)
        {
            _MenuItem = uiEvent.Param1 as AchievementGroupSecondMenuTable;
            if (null != tabDropDown)
            {
                tabDropDown.value = (int)AchievementFilter.AF_ALL;
            }

            ChangeTabStatus(false);
            _AnsyInvoke();
            UpdateTabProcess();
            UpdateTabBaseInfo();
        }

        void _AnsyInvoke()
        {
            InvokeMethod.RemoveInvokeCall(this);
            InvokeMethod.Invoke(this, 0.05f, _UpdateItems);
        }

        AchievementGroupMainItemTable _MainItem = null;
        AchievementGroupSecondMenuTable _MenuItem = null;
        int _SortAll(AchievementGroupSubItemTable l, AchievementGroupSubItemTable r)
        {
            var left = MissionManager.GetInstance().GetMission((uint)l.ID);
            var right = MissionManager.GetInstance().GetMission((uint)r.ID);
            if (left.status != right.status)
            {
                if (left.status == (int)Protocol.TaskStatus.TASK_OVER)
                {
                    return 1;
                }

                if (right.status == (int)Protocol.TaskStatus.TASK_OVER)
                {
                    return -1;
                }

                return (int)right.status - (int)left.status;
            }

            if (l.sort0 != r.sort0)
            {
                return l.sort0 - r.sort0;
            }

            if (left.taskID != right.taskID)
            {
                return left.taskID < right.taskID ? -1 : 1;
            }

            return 0;
        }

        int _SortTabItem(AchievementGroupSubItemTable l, AchievementGroupSubItemTable r)
        {
            var left = MissionManager.GetInstance().GetMission((uint)l.ID);
            var right = MissionManager.GetInstance().GetMission((uint)r.ID);
            if (left.status != right.status)
            {
                if (left.status == (int)Protocol.TaskStatus.TASK_OVER)
                {
                    return 1;
                }

                if (right.status == (int)Protocol.TaskStatus.TASK_OVER)
                {
                    return -1;
                }

                return (int)right.status - (int)left.status;
            }

            if (l.sort1 != r.sort1)
            {
                return l.sort1 - r.sort1;
            }

            if (left.taskID != right.taskID)
            {
                return left.taskID < right.taskID ? -1 : 1;
            }

            return 0;
        }

        void _GetValidItems(ref List<AchievementGroupSubItemTable> items)
        {
            for (int i = 0; i < mListItems.Count; ++i)
            {
                if (null == mListItems[i])
                {
                    mListItems.RemoveAt(i--);
                    continue;
                }

                var missionValue = MissionManager.GetInstance().GetMission((uint)mListItems[i].ID);
                if (null == missionValue || null == missionValue.missionItem)
                {
                    //Logger.LogErrorFormat("can not get mission id = {0}", mListItems[i].ID);
                    mListItems.RemoveAt(i--);
                    continue;
                }
            }
        }

        void _UpdateItems()
        {
            mListItems = null;
            if (_MainItem.ChildTabs.Count == 1)
            {
                AchievementGroupDataManager.GetInstance().GetAllItems(ref mListItems);
                _GetValidItems(ref mListItems);
                mListItems.Sort(_SortAll);
            }
            else
            {
                AchievementGroupDataManager.GetInstance().GetSubItemsByTag(_MainItem,_MenuItem, ref mListItems);
                _GetValidItems(ref mListItems);
                mListItems.Sort(_SortTabItem);
            }

            if(null != mListItems)
            {
                if (mAchievementFilter != AchievementFilter.AF_ALL)
                {
                    for (int i = 0; i < mListItems.Count; ++i)
                    {
                        if (null == mListItems[i])
                        {
                            mListItems.RemoveAt(i--);
                            continue;
                        }

                        var missionValue = MissionManager.GetInstance().GetMission((uint)mListItems[i].ID);
                        if (null == missionValue || null == missionValue.missionItem)
                        {
                            Logger.LogErrorFormat("can not get mission id = {0}", mListItems[i].ID);
                            mListItems.RemoveAt(i--);
                            continue;
                        }

                        if (mAchievementFilter == AchievementFilter.AF_FINISHED)
                        {
                            if(missionValue.status < (int)Protocol.TaskStatus.TASK_FINISHED)
                            {
                                mListItems.RemoveAt(i--);
                                continue;
                            }
                        }
                        else if(mAchievementFilter == AchievementFilter.AF_UNFINISHED)
                        {
                            if (missionValue.status > (int)Protocol.TaskStatus.TASK_UNFINISH)
                            {
                                mListItems.RemoveAt(i--);
                                continue;
                            }
                        }
                    }
                }
            }

            if (null != comAchievementList)
            {
                if (null != mListItems)
                {
                    comAchievementList.SetElementAmount(mListItems.Count);
                    comAchievementList.EnsureElementVisable(0);
                }
            }
        }

        struct MainTabData
        {
            public AchievementGroupMainItemTable value;
            public ComActiveGroupMainTab tab;
            public GameObject expandParent;
        };
        List<MainTabData> mDatas = new List<MainTabData>(12);
        public void CreateMainTabs()
        {
            if(null == mPrefabMainTab)
            {
                return;
            }
            mPrefabMainTab.CustomActive(false);
            goExpandParentPrefab.CustomActive(false);
            mDatas.Clear();
            var mainItems = TableManager.GetInstance().GetTable<AchievementGroupMainItemTable>();
            var enumerator = mainItems.GetEnumerator();
            while(enumerator.MoveNext())
            {
                AchievementGroupMainItemTable item = enumerator.Current.Value as AchievementGroupMainItemTable;
                if(null != item)
                {
                    ComActiveGroupMainTab goLocal = GameObject.Instantiate(mPrefabMainTab) as ComActiveGroupMainTab;
                    goLocal.name = item.Name;
                    Utility.AttachTo(goLocal.gameObject, goParent);
                    goLocal.mainItem = item;
                    goLocal.CustomActive(true);
                    GameObject goExpandParent = GameObject.Instantiate(goExpandParentPrefab) as GameObject;
                    goExpandParent.name = item.Name + "_p";
                    Utility.AttachTo(goExpandParent.gameObject, goParent);
                    goLocal.expandParent = goExpandParent;
                    goExpandParent.CustomActive(true);
                    mDatas.Add(new MainTabData { tab = goLocal, value = item, expandParent = goExpandParent });
                    goLocal.SetBinderID(item.ID, -1);
                }
            }

            for (int i = 0; i < mDatas.Count; ++i)
            {
                mDatas[i].tab.UpdateItemValue();
                mDatas[i].tab.OnValueChanged(false);
            }

            _registerMainTab();
        }

        private void _registerMainTab()
        {
            if (null != mDatas && mDatas.Count > 0)
            {
                if (null != mDatas[0].tab &&
                    null != mDatas[0].tab.toggle)
                {
                    mDatas[0].tab.toggle.onValueChanged.AddListener(_mainToggleValueChanged);
                }
            }
        }

        private void _unregisterMainTab()
        {
            if (null != mDatas && mDatas.Count > 0)
            {
                if (null != mDatas[0].tab &&
                    null != mDatas[0].tab.toggle)
                {
                    mDatas[0].tab.toggle.onValueChanged.RemoveListener(_mainToggleValueChanged);
                }
            }
        }


        public void SelectTab(int iTabId)
        {
            for (int i = 0; i < mDatas.Count; ++i)
            {
                for(int j = 0; j < mDatas[i].value.ChildTabs.Count; ++j)
                {
                    if(mDatas[i].value.ChildTabs[j] == iTabId)
                    {
                        mDatas[i].tab.OnValueChanged(true);
                        return;
                    }
                }
            }
        }

        public void UpdateAchievementPoint()
        {
            int point = PlayerBaseData.GetInstance().AchievementScore;

            AchievementLevelInfoTable levelItem = AchievementGroupDataManager.GetInstance().GetAchievementLevelByPoint(point);

            if (null != levelName && null != levelItem)
            {
                levelName.text = levelItem.Name;
            }

            if (null != levelImg && null != levelItem)
            {
                ETCImageLoader.LoadSprite(ref levelImg, levelItem.Icon);
            }

            if(null != levelInfoIcon && null != levelItem)
            {
                ETCImageLoader.LoadSprite(ref levelInfoIcon, levelItem.TextIcon);
            }

            if (null != achievementPoint)
            {
                achievementPoint.text = string.Format(fmtPointString, point);
            }

            if(null != processInfo && null != levelItem)
            {
                int pre = point;
                int aft = AchievementGroupDataManager.GetInstance().GetSubItemsAValue(null,null, false);
                float radio = Mathf.Clamp01(pre * 1.0f / aft);
                processInfo.text = string.Format(fmtPointProcess, pre, aft);
                if(null != slider)
                {
                    slider.value = radio;
                }
            }

            _UpdateLevelAwardDesc();
        }

        void _UpdateLevelAwardDesc()
        {
            int point = GetPointSum();

            AchievementLevelInfoTable levelItem = AchievementGroupDataManager.GetInstance().GetAchievementLevelByPoint(point);

            if (null != levelItem)
            {
                for (int i = 0; i < itemParents.Length; ++i)
                {
                    itemParents[i].CustomActive(i < levelItem.AwardID.Count);
                }

                for (int i = 0; i < levelItem.AwardID.Count; ++i)
                {
                    if (i < comItems.Count)
                    {
                        ItemData itemData = ItemDataManager.CreateItemDataFromTable(levelItem.AwardID[i]);
                        if (null != itemData)
                        {
                            if (i < levelItem.AwardCount.Count)
                            {
                                itemData.Count = levelItem.AwardCount[i];
                            }
                        }
                        comItems[i].Setup(itemData, _OnItemClicked);
                    }
                }
            }

            if (null != levelAwardDesc)
            {
                levelAwardDesc.text = string.Format(levelAwardDescFmt, point);
            }
        }

        void _OnItemClicked(GameObject obj, ItemData item)
        {
            if (null != item)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }
        }

        public int GetPointSum()
        {
            int iId = AchievementGroupDataManager.GetInstance().GetFirstUnAcquiredID();
            if(iId == 0)
            {
                iId = AchievementGroupDataManager.GetInstance().GetLastAcquiredID();
            }
            if(iId != 0)
            {
                var levelItem = TableManager.GetInstance().GetTableItem<AchievementLevelInfoTable>(iId);
                if(null != levelItem)
                {
                    return levelItem.Max;
                }
            }

            return 0;
        }

        IEnumerator AnsyGetRank()
        {
            if (null != rank)
            {
                int rankNum = 25;
                yield return new WaitForEndOfFrame();

                if(-1 == rankNum)
                {
                    rank.text = rankDisable;
                }
                else
                {
                    rank.text = string.Format(rankEnableFmt, rankNum);
                }
            }
        }

        void _RequestRanklist(int a_startIndex, int a_count,UnityEngine.Events.UnityAction<AchievementScoreSortListEntry> ok, UnityEngine.Events.UnityAction failed)
        {
            WorldSortListReq msg = new WorldSortListReq();
            msg.type = SortListDecoder.MakeSortListType(SortListType.SORTLIST_ACHIEVEMENT_SCORE);
            msg.start = (ushort)a_startIndex;
            msg.num = (ushort)a_count;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait(WorldSortListRet.MsgID, msgRet =>
            {
                if (msgRet != null)
                {
                    int pos = 0;
                    BaseSortList arrRecods = SortListDecoder.Decode(msgRet.bytes, ref pos, msgRet.bytes.Length);
                    if (null != arrRecods)
                    {
                        for(int i = 0; i < arrRecods.entries.Count; ++i)
                        {
                            AchievementScoreSortListEntry current = arrRecods.entries[i] as AchievementScoreSortListEntry;
                            if (null != current && current.id == PlayerBaseData.GetInstance().RoleID)
                            {
                                arrRecods.selfEntry = current;
                                break;
                            }
                        }
                        AchievementScoreSortListEntry entry = arrRecods.selfEntry as AchievementScoreSortListEntry;
                        if (null != entry)
                        {
                            if (null != ok)
                            {
                                ok.Invoke(entry);
                                return;
                            }
                        }
                    }
                }
                if (null != failed)
                {
                    failed.Invoke();
                }
            },
            true,15,()=>
            {
                if(null != failed)
                {
                    failed.Invoke();
                }
            });
        }

        public void OnClickLookUpRank()
        {
            ClientSystemManager.instance.OpenFrame<RanklistFrame>(FrameLayer.Middle, SortListType.SORTLIST_ACHIEVEMENT_SCORE);
        }

        public void OnClickGetLevelAwards()
        {
            if (ItemDataManager.GetInstance().IsPackageFull())
            {
                SystemNotifyManager.SystemNotify(9058);
                return;
            }

            var id = AchievementGroupDataManager.GetInstance().GetFirstUnAcquiredID();
            if(0 != id)
            {
                AchievementGroupDataManager.GetInstance().SendGetAward(id);
                GameClient.AchievementAwardPlayFrame.CommandOpen(new AchievementAwardPlayFrameData { iId = id });
            }
        }

        List<AchievementGroupSubItemTable> mListItems = new List<AchievementGroupSubItemTable>(32);

        protected void _InitAchievementList()
        {
            if(null != comAchievementList)
            {
                comAchievementList.Initialize();
                comAchievementList.onBindItem = (GameObject go) =>
                 {
                     if(null != go)
                     {
                         return go.GetComponent<ComAchievementSubItem>();
                     }
                     return null;
                 };

                comAchievementList.onItemVisiable = (ComUIListElementScript item) =>
                {
                    if(null != mListItems)
                    {
                        if (null != item && item.m_index >= 0 && item.m_index < mListItems.Count)
                        {
                            ComAchievementSubItem comSubItem = item.gameObjectBindScript as ComAchievementSubItem;
                            if(null != comSubItem)
                            {
                                comSubItem.OnItemVisible(mListItems[item.m_index]);
                            }
                        }
                    }
                };
            }
        }

        public void StopInvoke()
        {
            InvokeMethod.RemoveInvokeCall(this);
            InvokeMethod.RmoveInvokeIntervalCall(this);
        }

        void OnDestroy()
        {
            NetProcess.RemoveMsgHandler(SceneSyncChat.MsgID, _OnNetSyncChat);

            _unregisterMainTab();

            goParent = null;
            mPrefabMainTab = null;
            mListItems = null;
            if (null != comAchievementList)
            {
                comAchievementList.onBindItem = null;
                comAchievementList.onItemVisiable = null;
            }
            for(int i = 0; i < comItems.Count; ++i)
            {
                ComItemManager.Destroy(comItems[i]);
            }
            comItems.Clear();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAchievementGroupSubTabChanged, _OnAchievementGroupSubTabChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAchievementGroupSubTabChangedRepeated, _OnAchievementGroupSubTabChangedRepeated);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAchievementSecondMenuTabChanged, _OnAchievementSecondMenuTabChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAchievementMaskPropertyChanged, _OnAchievementMaskPropertyChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnShareAchievementItem, _OnShareAchievementItem);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAchievementScoreChanged, _OnAchievementScoreChanged);
            MissionManager.GetInstance().onAddNewMission -= _OnAddNewMission;
            MissionManager.GetInstance().onUpdateMission -= _OnUpdateMission;
            MissionManager.GetInstance().onDeleteMission -= _OnDeleteMission;
            if(null != tabDropDown)
            {
                tabDropDown.onValueChanged.RemoveListener(_OnDropDownChanged);
                tabDropDown = null;
            }
            mAchievementFilter = AchievementFilter.AF_ALL;
            _menuData = null;
            _query = false;
            StopInvoke();
        }
    }
}