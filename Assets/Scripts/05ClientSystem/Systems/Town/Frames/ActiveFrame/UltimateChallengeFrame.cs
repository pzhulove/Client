using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using Network;

namespace GameClient
{
    // 终极试炼界面
    public class UltimateChallengeFrame : ClientFrame
    {
        #region inner define

        #endregion

        #region val
        List<ActivityDataManager.UltimateChallengeFloorData> itemDataList = null;

        ActivityDataManager.UltimateChallengeFloorData bottonFloorData = new ActivityDataManager.UltimateChallengeFloorData() { floor = 0, tableID = 0 };
        int currentFloorIndex = 0;

        #endregion

        #region ui bind
        ComUIListScript itemList = null;
        Slider HP = null;
        Slider MP = null;
        Text maxFloor = null;
        Text enterCount = null;
        Text challengeCount = null;
        Button btnChallenge = null;
        Text currentFloorInfo = null;
        Button btnRefresh = null;
        ComChapterInfoDrop Drop = null;
        ComBufItem bufItem0 = null;
        ComBufItem bufItem1 = null;
        Text Level = null;
        Text Name = null;
        Image Icon = null;
        ComItem moneyIcon = null;
        Text moneyNum = null;
        Text HPText = null;
        Text MPText = null;
        GameObject finishedAllFloor = null;
        ShopNewMoneyItem firstMoney = null;
        Button shop = null;
        ComEffect effect0 = null;
        ComEffect effect1 = null;
        Text todayOpenFloor = null;
        GameObject btnChallengeText1 = null;
        GameObject btnChallengeText2 = null;
        Text sustainFloor = null;
        Text bufName = null;
        #endregion

        #region override

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/UltimateChallenge";
        }

        protected override void _OnOpenFrame()
        {
            itemDataList = null;
            InitItems();
            UpdateUltimateChallengeFloorInfoItems();
            UpdateUltimateChallengeCountInfos();
            BindUIEvent();

            UpdateDropItems();
            UpdateBufItems();


            int currentFloorIndex = ActivityDataManager.GetInstance().GetUltimateChallengeTodayStartFloor() - 1;
            if (itemDataList != null && currentFloorIndex < itemDataList.Count && currentFloorIndex >= 0)
            {
                UltimateChallengeDungeonTable ultimateChallengeDungeonTable = TableManager.GetInstance().GetTableItem<UltimateChallengeDungeonTable>(itemDataList[currentFloorIndex].tableID);
                if (ultimateChallengeDungeonTable != null)
                {
                    ActivityDataManager.GetInstance().SendSceneDungeonZjslRefreshBuffReq(ultimateChallengeDungeonTable.dungeonID, false);
                }
            }

            if (firstMoney != null)
            {
                firstMoney.InitMoneyItem(600002541);
            }

            effect0.CustomActive(false);
            effect1.CustomActive(false);
        }

        protected override void _OnCloseFrame()
        {
            itemDataList = null;
            UnBindUIEvent();
        }

        protected override void _bindExUI()
        {
            itemList = mBind.GetCom<ComUIListScript>("itemList");

            HP = mBind.GetCom<Slider>("HP");
            MP = mBind.GetCom<Slider>("MP");
            maxFloor = mBind.GetCom<Text>("maxFloor");
            enterCount = mBind.GetCom<Text>("enterCount");
            challengeCount = mBind.GetCom<Text>("challengeCount");
            btnChallenge = mBind.GetCom<Button>("btnChallenge");
            btnChallenge.SafeSetOnClickListener(() =>
            {
                if (!ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(ServiceType.SERVICE_ZJSL_TOWER))
                {
                    SystemNotifyManager.SystemNotify(4500005);
                    return;
                }

                int currentFloorIndex = ActivityDataManager.GetInstance().GetUltimateChallengeTodayStartFloor() - 1;
                if (itemDataList != null && currentFloorIndex < itemDataList.Count && currentFloorIndex >= 0 && itemDataList[currentFloorIndex] != null)
                {
                    UltimateChallengeDungeonTable ultimateChallengeDungeonTable = TableManager.GetInstance().GetTableItem<UltimateChallengeDungeonTable>(itemDataList[currentFloorIndex].tableID);
                    if (ultimateChallengeDungeonTable != null)
                    {
                        SceneDungeonStartReq req = new SceneDungeonStartReq();
                        req.dungeonId = (uint)(ultimateChallengeDungeonTable.dungeonID);
                        NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
                    }
                }
            });
            currentFloorInfo = mBind.GetCom<Text>("currentFloorInfo");
            btnRefresh = mBind.GetCom<Button>("btnRefresh");
            btnRefresh.SafeSetOnClickListener(() =>
            {
                int itemID = 0;
                int itemNum = 0;
                GetCostInfo(ref itemID, ref itemNum);

                if (ItemDataManager.GetInstance().GetOwnedItemCount(itemID) < itemNum)
                {
                    List<int> notEnoughItemIDs = new List<int>();
                    if (notEnoughItemIDs != null)
                    {
                        notEnoughItemIDs.Add(itemID);
                        ItemComeLink.OnLink(notEnoughItemIDs[0], 0);
                    }

                    return;
                }

                UnityEngine.Events.UnityAction sendRefreshBufMsg = () =>
                {
                    int currentFloorIndex = ActivityDataManager.GetInstance().GetUltimateChallengeTodayStartFloor() - 1;

                    if (itemDataList != null && currentFloorIndex < itemDataList.Count && currentFloorIndex >= 0)
                    {
                        UltimateChallengeDungeonTable ultimateChallengeDungeonTable = TableManager.GetInstance().GetTableItem<UltimateChallengeDungeonTable>(itemDataList[currentFloorIndex].tableID);
                        if (ultimateChallengeDungeonTable != null)
                        {
                            ActivityDataManager.GetInstance().SendSceneDungeonZjslRefreshBuffReq(ultimateChallengeDungeonTable.dungeonID);
                        }
                    }
                };

                if (ActivityDataManager.GetInstance().NotPopUpRefreshBufMsgBox)
                {
                    sendRefreshBufMsg();
                }
                else
                {
                    frameMgr.OpenFrame<UltimateChallengeRefreshBufMsgBoxFrame>(FrameLayer.Middle, new UltimateChallengeRefreshBufMsgBoxFrame.MsgBoxData()
                    {
                        content = TR.Value("refresh_buf_tip", ItemDataManager.GetInstance().GetOwnedItemName(itemID), itemNum),
                        cancelCallBack = null,
                        okCallBack = sendRefreshBufMsg,
                    });
                }
            });

            Drop = mBind.GetCom<ComChapterInfoDrop>("Drop");

            bufItem0 = mBind.GetCom<ComBufItem>("bufItem0");
            bufItem1 = mBind.GetCom<ComBufItem>("bufItem1");

            Level = mBind.GetCom<Text>("Level");
            Name = mBind.GetCom<Text>("Name");
            Icon = mBind.GetCom<Image>("Icon");

            moneyIcon = mBind.GetCom<ComItem>("moneyIcon");
            moneyNum = mBind.GetCom<Text>("moneyNum");

            HPText = mBind.GetCom<Text>("HPText");
            MPText = mBind.GetCom<Text>("MPText");

            finishedAllFloor = mBind.GetGameObject("finishedAllFloor");

            firstMoney = mBind.GetCom<ShopNewMoneyItem>("firstMoney");
            shop = mBind.GetCom<Button>("shop");
            shop.SafeSetOnClickListener(() =>
            {
                ShopNewDataManager.GetInstance().OpenShopNewFrame(30);
            });

            effect0 = mBind.GetCom<ComEffect>("effect0");
            effect1 = mBind.GetCom<ComEffect>("effect1");

            todayOpenFloor = mBind.GetCom<Text>("todayOpenFloor");
            btnChallengeText1 = mBind.GetGameObject("btnChallengeText1");
            btnChallengeText2 = mBind.GetGameObject("btnChallengeText2");

            sustainFloor = mBind.GetCom<Text>("sustainFloor");
            bufName = mBind.GetCom<Text>("bufName");
        }

        protected override void _unbindExUI()
        {
            itemList = null;
            HP = null;
            MP = null;
            maxFloor = null;
            enterCount = null;
            challengeCount = null;
            btnChallenge = null;
            currentFloorInfo = null;
            Drop = null;
            bufItem0 = null;
            bufItem1 = null;
            Level = null;
            Name = null;
            Icon = null;
            moneyIcon = null;
            moneyNum = null;
            HPText = null;
            MPText = null;
            finishedAllFloor = null;
            firstMoney = null;
            shop = null;
            effect0 = null;
            effect1 = null;
            todayOpenFloor = null;
            btnChallengeText1 = null;
            btnChallengeText2 = null;
            sustainFloor = null;
            bufName = null;
        }

        #endregion

        #region method

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnOnCountValueChange);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RefreshDungeonBufSuccess, _OnRefreshDungeonBufSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RefreshInspireBufSuccess, _OnRefreshInspireBufSuccess);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnOnCountValueChange);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RefreshDungeonBufSuccess, _OnRefreshDungeonBufSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RefreshInspireBufSuccess, _OnRefreshInspireBufSuccess);
        }

        string GetFloorDungeonName()
        {
            int currentFloorIndex = ActivityDataManager.GetInstance().GetUltimateChallengeTodayStartFloor() - 1;

            // 全部打通了 这里有个特殊处理 还是要显示第20层的名称
            if (currentFloorIndex == itemDataList.Count)
            {
                currentFloorIndex = itemDataList.Count - 1;
            }

            if (itemDataList != null && currentFloorIndex < itemDataList.Count && currentFloorIndex >= 0)
            {
                UltimateChallengeDungeonTable ultimateChallengeDungeonTable = TableManager.GetInstance().GetTableItem<UltimateChallengeDungeonTable>(itemDataList[currentFloorIndex].tableID);
                if (ultimateChallengeDungeonTable != null)
                {
                    DungeonTable dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(ultimateChallengeDungeonTable.dungeonID);
                    if (dungeonTable != null)
                    {
                        return dungeonTable.Name;
                    }
                }
            }

            return "";
        }

        void GetCostInfo(ref int costItemID, ref int costNum)
        {
            int currentFloorIndex = ActivityDataManager.GetInstance().GetUltimateChallengeTodayStartFloor() - 1;
            // 全部打通了 这里有个特殊处理 还是要显示第20层的刷新消耗
            if (currentFloorIndex == itemDataList.Count)
            {
                currentFloorIndex = itemDataList.Count - 1;
            }
            if (itemDataList != null && currentFloorIndex < itemDataList.Count && currentFloorIndex >= 0)
            {
                UltimateChallengeDungeonTable ultimateChallengeDungeonTable = TableManager.GetInstance().GetTableItem<UltimateChallengeDungeonTable>(itemDataList[currentFloorIndex].tableID);
                if (ultimateChallengeDungeonTable != null)
                {
                    string[] cost = ultimateChallengeDungeonTable.expendItem.Split('_');
                    if (cost.Length >= 2)
                    {
                        int id = int.Parse(cost[0]);
                        int iCount = int.Parse(cost[1]);
                        costItemID = id;
                        costNum = iCount;
                    }
                }
            }

            return;
        }

        void InitItems()
        {
            if (itemList == null)
            {
                return;
            }

            itemList.Initialize();

            itemList.onBindItem = (GameObject go) =>
            {
                UltimateChallengeFloorInfoItem item = null;
                if (go)
                {
                    item = go.GetComponent<UltimateChallengeFloorInfoItem>();
                }

                return item;
            };


            itemList.onItemVisiable = (var1) =>
            {
                UpdateUltimateChallengeFloorInfoItem(var1);

            };

            itemList.onItemSelected = (var1) =>
            {
                if (var1 != null)
                {
                    //                     currentSelectFloorIndex = var1.m_index;
                    // 
                    //                     UpdateDropItems();
                    //                     UpdateBufItems();
                }
            };
        }

        void UpdateUltimateChallengeFloorInfoItem(ComUIListElementScript var1)
        {
            if (var1 == null)
            {
                return;
            }

            UltimateChallengeFloorInfoItem item = var1.gameObjectBindScript as UltimateChallengeFloorInfoItem;
            if (item == null)
            {
                return;
            }

            int iIndex = var1.m_index;
            if (iIndex >= 0 && itemDataList != null && iIndex < itemDataList.Count)
            {
                item.SetUp(itemDataList[itemDataList.Count - iIndex - 1]);
            }
            else if (iIndex == itemDataList.Count) // 底座
            {
                item.SetUp(bottonFloorData);
            }
        }

        void CalItemDataList()
        {
            itemDataList = null;
            itemDataList = new List<ActivityDataManager.UltimateChallengeFloorData>();
            //itemDataList = ActivityDataManager.GetInstance().GetMonthlySignInItemDatas();

            {
                Dictionary<int, object> dicts = TableManager.instance.GetTable<UltimateChallengeDungeonTable>();
                if (dicts != null)
                {
                    var iter = dicts.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        UltimateChallengeDungeonTable adt = iter.Current.Value as UltimateChallengeDungeonTable;
                        if (adt == null)
                        {
                            continue;
                        }

                        ActivityDataManager.UltimateChallengeFloorData ultimateChallengeFloorData = new ActivityDataManager.UltimateChallengeFloorData();
                        if (ultimateChallengeFloorData == null)
                        {
                            continue;
                        }

                        ultimateChallengeFloorData.floor = adt.level;
                        ultimateChallengeFloorData.tableID = adt.ID;
                        itemDataList.Add(ultimateChallengeFloorData);
                    }
                }
            }
           
            return;
        }

        void UpdateUltimateChallengeFloorInfoItems()
        {
            if (itemList == null)
            {
                return;
            }

            CalItemDataList();

            if (itemDataList != null)
            {
                itemList.SetElementAmount(itemDataList.Count + 1);

                itemList.m_scrollRect.verticalNormalizedPosition = (float)(ActivityDataManager.GetInstance().GetUltimateChallengeTodayStartFloor() - 1) / (float)itemDataList.Count;
            }

            AdjustSrollBarValue(ActivityDataManager.GetInstance().GetUltimateChallengeTodayStartFloor());
        }

        void AdjustSrollBarValue(int floor)
        {
            if (itemList == null || itemList.m_scrollRect == null)
            {
                return;
            }

            int iCount = Math.Max(1, floor);

            RectTransform rectTransform = itemList.GetComponent<RectTransform>();
            float fValue = (iCount - 3) * ((itemList.contentSize.y + itemList.m_elementSpacing.y) / itemList.m_elementAmount) / (itemList.contentSize.y - rectTransform.rect.height);
            fValue += 0.03f;

            fValue = Math.Min(1.0f, fValue);
            fValue = Math.Max(0.0f, fValue);

            itemList.m_scrollRect.verticalNormalizedPosition = fValue;            
        }

        void UpdateUltimateChallengeCountInfos()
        {
            if (itemDataList == null)
            {
                return;
            }

            //sliderFatigue.SafeSetValue(ActivityDataManager.GetInstance().GetUltimateChallengeHPPercent() / 100.0f);
            HP.SafeSetValue(ActivityDataManager.GetInstance().GetUltimateChallengeHPPercent() / 100.0f);
            MP.SafeSetValue(ActivityDataManager.GetInstance().GetUltimateChallengeMPPercent() / 100.0f);
            HPText.SafeSetText(string.Format("{0}%", ActivityDataManager.GetInstance().GetUltimateChallengeHPPercent()));
            MPText.SafeSetText(string.Format("{0}%", ActivityDataManager.GetInstance().GetUltimateChallengeMPPercent()));

            maxFloor.SafeSetText(TR.Value("best_max_floor_record", ActivityDataManager.GetInstance().GetUltimateChallengeMaxFloorRecord()));

            enterCount.SafeSetText(TR.Value("enter_count", ActivityDataManager.GetInstance().GetUltimateChallengeLeftEnterCount(), ActivityDataManager.GetInstance().GetUltimateChallengeMaxEnterCount()));
            challengeCount.SafeSetText(TR.Value("challenge_count", ActivityDataManager.GetInstance().GetUltimateChallengeLeftCount(), ActivityDataManager.GetInstance().GetUltimateChallengeMaxCount()));
            currentFloorInfo.SafeSetText(TR.Value("current_floor_info", GetFloorDungeonName()));

            Name.SafeSetText(PlayerBaseData.GetInstance().Name);
            Level.SafeSetText(string.Format("{0}", PlayerBaseData.GetInstance().Level.ToString()));

            var jobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (jobData != null)
            {
                var resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                if (resData != null)
                {
                    Icon.SafeSetImage(resData.IconPath);
                }
            }

            DateTime dateTime = Function.ConvertIntDateTime(TimeManager.GetInstance().GetServerDoubleTime());
            if(dateTime.Hour < 6)
            {
                dateTime = dateTime.AddDays(-1); // 各种次数是6点刷新，6点之前都还是前一天
            }           

            int index = (int)dateTime.DayOfWeek;
            if (dateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                index = 7;
            }
            int maxOpenFloorCount = index * ActivityDataManager.GetInstance().GetUltimateChallengeDungeonDailyOpenFloor();
            bool bFlag = ActivityDataManager.GetInstance().GetUltimateChallengeLeftEnterCount() <= 0 // 没有进入次数
                || ActivityDataManager.GetInstance().GetUltimateChallengeLeftCount() <= 0            // 没有挑战次数 
                || ActivityDataManager.GetInstance().GetUltimateChallengeTodayStartFloor() > itemDataList.Count // 打通了20层
                || ActivityDataManager.GetInstance().GetUltimateChallengeTodayStartFloor() > maxOpenFloorCount; // 打通了开放的层

            btnChallenge.SafeSetGray(bFlag);
            btnRefresh.SafeSetGray(bFlag);

            bool bFinishedTodayFloor = ActivityDataManager.GetInstance().GetUltimateChallengeTodayStartFloor() > maxOpenFloorCount;
            btnChallengeText1.CustomActive(!bFinishedTodayFloor);
            btnChallengeText2.CustomActive(bFinishedTodayFloor);


            int itemID = 0;
            int itemNum = 0;
            GetCostInfo(ref itemID, ref itemNum);
            moneyIcon.Setup(ItemDataManager.CreateItemDataFromTable(itemID), (go, itemData) =>
            {
                if (null != itemData)
                {
                    ItemTipManager.GetInstance().CloseAll();
                    ItemTipManager.GetInstance().ShowTip(itemData);
                }
            });
            moneyNum.SafeSetText(string.Format("{0}", itemNum));
            moneyNum.color = ItemDataManager.GetInstance().GetOwnedItemCount(itemID) >= itemNum ? Color.green : Color.red;

            bool dead = ActivityDataManager.GetInstance().GetUltimateChallengeLeftCount() == 0;
            UIGray uIGray = Icon.gameObject.SafeAddComponent<UIGray>(false);
            if (uIGray != null)
            {
                uIGray.enabled = false;
                uIGray.enabled = dead;
            }

            if (dead)
            {
                HP.SafeSetValue(0.0f);
                MP.SafeSetValue(0.0f);
                HPText.SafeSetText(string.Format("{0}%", 0));
                MPText.SafeSetText(string.Format("{0}%", 0));
            }

            // 已经打通了
            if (ActivityDataManager.GetInstance().GetUltimateChallengeTodayStartFloor() > itemDataList.Count)
            {
                finishedAllFloor.CustomActive(true);
                btnChallenge.CustomActive(false);

                challengeCount.CustomActive(false);
                enterCount.CustomActive(false);
            }
            else
            {
                finishedAllFloor.CustomActive(false);
                btnChallenge.CustomActive(true);

                challengeCount.CustomActive(true);
                enterCount.CustomActive(true);
            }

            todayOpenFloor.SafeSetText(TR.Value("today_open_floor", Math.Min(maxOpenFloorCount,itemDataList.Count)));            
        }

        void UpdateDropItems()
        {
            if (Drop == null)
            {
                return;
            }

            if (itemDataList == null)
            {
                return;
            }

            int currentFloorIndex = ActivityDataManager.GetInstance().GetUltimateChallengeTodayStartFloor() - 1;

            // 全部打通了 这里有个特殊处理 还是要显示第20层的掉落信息
            if (currentFloorIndex == itemDataList.Count)
            {
                currentFloorIndex = itemDataList.Count - 1;
            }

            if (!(currentFloorIndex >= 0 && currentFloorIndex < itemDataList.Count))
            {
                return;
            }

            if (itemDataList[currentFloorIndex] == null)
            {
                return;
            }

            UltimateChallengeDungeonTable ultimateChallengeDungeonTable = TableManager.GetInstance().GetTableItem<UltimateChallengeDungeonTable>(itemDataList[currentFloorIndex].tableID);
            if (ultimateChallengeDungeonTable == null)
            {
                return;
            }

            DungeonTable dungeonTable = TableManager.GetInstance().GetTableItem<DungeonTable>(ultimateChallengeDungeonTable.dungeonID);
            if (dungeonTable != null)
            {
                Drop.SetDropList(dungeonTable.DropItems, 0);
            }
        }

        void UpdateBufItems()
        {
            if (bufItem0 != null)
            {
                bufItem0.SetBufData(ActivityDataManager.GetInstance().GetUltimateChallengeDungeonBufID(), ActivityDataManager.GetInstance().GetUltimateChallengeDungeonBufLv());
                bufItem0.CustomActive(ActivityDataManager.GetInstance().GetUltimateChallengeDungeonBufID() > 0);
            }

            if (bufItem1 != null)
            {
                bufItem1.SetBufData(ActivityDataManager.GetInstance().GetUltimateChallengeInspireBufID(), ActivityDataManager.GetInstance().GetUltimateChallengeInspireBufLv());
                bufItem1.CustomActive(ActivityDataManager.GetInstance().GetUltimateChallengeInspireBufID() > 0);
            }

            _SetText(sustainFloor,(TR.Value("buf_sustain_floor", ActivityDataManager.GetInstance().GetUltimateChallengeDungeonBufBeginFloor() + ActivityDataManager.GetInstance().GetUltimateChallengeDungeonBufDurationFloor(ActivityDataManager.GetInstance().GetUltimateChallengeDungeonBufID()) - 1)));
            _SetText(bufName, bufName.text);      
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

        #endregion

        #region ui event

        void _OnOnCountValueChange(UIEvent uiEvent)
        {
            UpdateUltimateChallengeCountInfos();
            //UpdateDropItems();
            UpdateBufItems();
            return;
        }

        void _OnRefreshDungeonBufSuccess(UIEvent uiEvent)
        {         
            if(effect0 != null)
            {
                effect0.CustomActive(true);
                effect0.Stop("UsedEffect");
                effect0.Play("UsedEffect");
            }

            return;
        }

        void _OnRefreshInspireBufSuccess(UIEvent uiEvent)
        {         
            if(effect1 != null)
            {
                effect1.CustomActive(true);
                effect1.Stop("UsedEffect");
                effect1.Play("UsedEffect");
            }

            return;
        }
        #endregion
    }
}
