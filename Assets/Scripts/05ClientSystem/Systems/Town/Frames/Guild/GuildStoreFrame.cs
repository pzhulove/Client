using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Network;
using Protocol;
using Scripts.UI;

namespace GameClient
{
    class GuildStoreFrame : ClientFrame
    {
        DelayCallUnitHandle m_repeatCallLeftTime;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildStoreFrame";
        }

        public static void CommandOpen(object argv = null)
        {
            ansyOpen(argv);
        }

        public static void ansyOpen(object argv = null)
        {
            if (!GuildDataManager.GetInstance().queried)
            {
                WorldGuildStorageListReq msg = new WorldGuildStorageListReq();
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);
                //Logger.LogErrorFormat("[guild_power] send WorldGuildStorageListReq !!!");

                WaitNetMessageManager.GetInstance().Wait<WorldGuildStorageListRes>(msgRet =>
                {
                    //Logger.LogErrorFormat("[guild_power] recv WorldGuildStorageListRes !!!");
                    if (msgRet.result != 0)
                    {
                        SystemNotifyManager.SystemNotify((int)msgRet.result);
                    }
                    else
                    {
                        GuildDataManager.GetInstance().queried = true;
                        GuildDataManager.GetInstance().storeDatas.Clear();
                        if (null != msgRet.items)
                        {
                            for (int i = 0; i < msgRet.items.Length; ++i)
                            {
                                if (msgRet.items[i].num > 0)
                                {
                                    var itemData = ItemDataManager.CreateItemDataFromTable((int)msgRet.items[i].dataId);
                                    if (null != itemData)
                                    {
                                        itemData.GUID = msgRet.items[i].uid;
                                        itemData.Count = msgRet.items[i].num;
                                        itemData.StrengthenLevel = msgRet.items[i].str;
                                        itemData.EquipType = (EEquipType)msgRet.items[i].equipType;
                                        GuildDataManager.GetInstance().storeDatas.Add(itemData);
                                    }
                                }
                            }
                        }
                        //Logger.LogErrorFormat("maxSize = {0}", msgRet.maxSize);
                        GuildDataManager.GetInstance().storeageCapacity = (int)msgRet.maxSize;
                        //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnOpenGuildHouseMain, argv);
                        ClientSystemManager.GetInstance().OpenFrame<GuildStoreFrame>();

                        if (null != msgRet.itemRecords)
                        {
                            for (int i = 0; i < msgRet.itemRecords.Length; ++i)
                            {
                                var record = msgRet.itemRecords[i];
                                if (null != record)
                                {
                                    GuildDataManager.GetInstance().AddRecord(record);
                                }
                            }
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildSotrageOperationRecordsChanged);
                        }
                    }
                });
            }
            else
            {
                //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnOpenGuildHouseMain, argv);
                ClientSystemManager.GetInstance().OpenFrame<GuildStoreFrame>();
            }
        }

        [UIObject("SettingRoot/Ctrls/BtnSetting")]
        GameObject goSetting;
        [UIObject("SettingRoot/Ctrls/BtnClear")]
        GameObject goClear;
        [UIObject("SettingRoot/Ctrls/BtnContribute")]
        GameObject goContribute;
        [UIControl("", typeof(StateController))]
        StateController comStatus;
        [UIControl("Space", typeof(Text))]
        Text space;
        [UIControl("RecordHead", typeof(Text))]
        Text recordHead;
        [UIControl("Records", typeof(ComUIListScript))]
        ComUIListScript comSotreRecordsList;
        [UIControl("Records/Scroll View/Viewport/Content/Prefab/Record/Text", typeof(NewSuperLinkText))]
        NewSuperLinkText comSuperLinkText;

        void _InitRecordsList()
        {
            comSotreRecordsList.Initialize();
            comSotreRecordsList.onBindItem = (GameObject itemObject) =>
            {
                var com = itemObject.GetComponent<ComSotrageOperateRecord>();
                if (null != com)
                {
                    com.OnCreate();
                }
                return com;
            };

            comSotreRecordsList.onItemVisiable = (ComUIListElementScript item) =>
            {
                if (null != item && item.m_index >= 0 && item.m_index < GuildDataManager.GetInstance().GuildStorageOperationRecords.Count)
                {
                    var com = item.gameObjectBindScript as ComSotrageOperateRecord;
                    if (null != com)
                    {
                        int reverseIndex = (GuildDataManager.GetInstance().GuildStorageOperationRecords.Count - 1 - item.m_index);
                        com.OnItemVisible(GuildDataManager.GetInstance().GuildStorageOperationRecords[reverseIndex]);
                    }
                }
            };
        }

        TextGenerator cachedTextGenerator = new TextGenerator(256);
        TextGenerationSettings textGeneratorSetting = new TextGenerationSettings();
        void _InitGeneratorSetting()
        {
            Vector2 extents = new Vector2(comSuperLinkText.rectTransform.rect.size.x, 0);
            var settings = comSuperLinkText.GetGenerationSettings(extents);
            textGeneratorSetting.font = settings.font;
            textGeneratorSetting.fontSize = settings.fontSize;
            textGeneratorSetting.fontStyle = settings.fontStyle;
            textGeneratorSetting.lineSpacing = settings.lineSpacing;
            textGeneratorSetting.horizontalOverflow = HorizontalWrapMode.Wrap;
            textGeneratorSetting.verticalOverflow = VerticalWrapMode.Overflow;
            textGeneratorSetting.alignByGeometry = false;
            textGeneratorSetting.resizeTextForBestFit = settings.resizeTextForBestFit;
            textGeneratorSetting.richText = settings.richText;
            textGeneratorSetting.scaleFactor = 1.0f;
            textGeneratorSetting.updateBounds = settings.updateBounds;
            textGeneratorSetting.generationExtents = extents;
        }

        void _RefreshStorageOperationRecords()
        {
            List<Vector2> elementsSize = GamePool.ListPool<Vector2>.Get();
            for (int i = 0; i < GuildDataManager.GetInstance().GuildStorageOperationRecords.Count; ++i)
            {
                int reverseIndex = (GuildDataManager.GetInstance().GuildStorageOperationRecords.Count - 1 - i);
                var data = GuildDataManager.GetInstance().GuildStorageOperationRecords[reverseIndex] as SotrageOperateRecordData;
                if (!data.measured)
                {
                    var stringBuilder = StringBuilderCache.Acquire();
                    LinkParse._TryToken(stringBuilder, data.value, 0, null);
                    string tokenedValue = stringBuilder.ToString();
                    StringBuilderCache.Release(stringBuilder);
                    float h = cachedTextGenerator.GetPreferredHeight(tokenedValue, textGeneratorSetting);
                    float w = textGeneratorSetting.generationExtents.x;
                    data.h = h;
                    data.w = w;
                    //Logger.LogErrorFormat("size = {0}|{1}", data.w, data.h);
                }
                elementsSize.Add(new Vector2(data.w, data.h));
            }
            comSotreRecordsList.SetElementAmount(GuildDataManager.GetInstance().GuildStorageOperationRecords.Count, elementsSize);
            GamePool.ListPool<Vector2>.Release(elementsSize);
        }

        protected override void _OnOpenFrame()
        {
            _AddButton("SettingRoot/Ctrls/BtnSetting", _OnClickSetting);
            _AddButton("SettingRoot/Ctrls/BtnClear", _OnClickClear);
            _AddButton("SettingRoot/Ctrls/BtnShop", _OnGotoShop);
            _AddButton("SettingRoot/Ctrls/BtnContribute", _OnClickContribute);
            _AddButton("Close", () => { frameMgr.CloseFrame(this); });
            _AddButton("AwardRoot/BtnAward", _OnClickAward);

            _InitGeneratorSetting();
            _InitRecordsList();
            _RefreshStorageOperationRecords();
            _UpdateButtonStatus();
            _UpdateStatus();
            _RefreshGuildItems();
            _UpdateSpace();
            _UpdateRecordHead();
            _UpdateClearCtrl();
            _UpdateContributeCtrl();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBattleStateChanged, _OnGuildBattleStateChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildHouseItemAdd, _OnStorageItemAdd);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildHouseItemRemoved, _OnStorageRemoved);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildHouseItemUpdate, _OnStorageItemUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildSotrageOperationRecordsChanged, _OnGuildSotrageOperationRecordsChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildLotteryResultRes, _OnAwardStateChanged);
            GuildDataManager.GetInstance().onGuildPowerChanged += _OnGuildPowerChanged;

            _UpdateLotteryLeftTime();

            if(storeItemList != null)
            {
                storeItemList.verticalNormalizedPosition = 1.0f;
            }
        }

        void _SortStorageItem()
        {
            m_akHouseItemDatas.ActiveObjects.Sort((x, y) =>
            {
                if ((x.Value.itemData == null) != (y.Value.itemData == null))
                {
                    return x.Value.itemData == null ? 1 : -1;
                }
                else
                {
                    if (x.Value.itemData == null)
                    {
                        return 0;
                    }
                }

                if (x.Value.itemData.Quality != y.Value.itemData.Quality)
                {
                    return y.Value.itemData.Quality - x.Value.itemData.Quality;
                }

                if (x.Value.itemData.GUID < y.Value.itemData.GUID)
                {
                    return -1;
                }

                if (x.Value.itemData.GUID == y.Value.itemData.GUID)
                {
                    return 0;
                }

                return 1;
            });

            for (int i = 0; i < m_akHouseItemDatas.ActiveObjects.Count; ++i)
            {
                m_akHouseItemDatas.ActiveObjects[i].SetSiblingIndex(1 + i);
            }
        }

        void _OnStorageItemAdd(UIEvent uiEvent)
        {
            ItemData itemData = uiEvent.Param1 as ItemData;
            if (null != itemData)
            {
                var find = m_akHouseItemDatas.Find(x=>
                {
                    return (x.Value.itemData == null);
                });

                if(null != find)
                {
                    find.OnRefresh(new object[] 
                    {
                        new GuildHouseItemData
                        {
                            itemData = itemData,
                        }
                    });
                }
                else
                {
                    m_akHouseItemDatas.Create(new object[] {
                        goParent,
                        goPrefab,
                        new GuildHouseItemData
                        {
                            itemData = itemData,
                        },
                        false,
                        });
                }
            }
            _SortStorageItem();
            _UpdateSpace();
        }

        void _OnStorageRemoved(UIEvent uiEvent)
        {
            ItemData itemData = uiEvent.Param1 as ItemData;
            if (null != itemData)
            {
                var find = m_akHouseItemDatas.Find(x =>
                {
                    return null != x &&
                    null != x.Value &&
                    null != x.Value.itemData &&
                    itemData.GUID == x.Value.itemData.GUID;
                });

                if(null != find)
                {
                    find.OnRefresh(new object[]
                    {
                        new GuildHouseItemData
                        {
                            itemData = null,
                        }
                    });
                }
            }
            _SortStorageItem();
            _UpdateSpace();
        }

        void _OnStorageItemUpdate(UIEvent uiEvent)
        {
            ItemData itemData = uiEvent.Param1 as ItemData;
            if (null != itemData)
            {
                var find = m_akHouseItemDatas.Find(x =>
                {
                    return null != x &&
                    null != x.Value &&
                    null != x.Value.itemData &&
                    itemData.GUID == x.Value.itemData.GUID;
                });

                if (null != find)
                {
                    find.OnRefresh(new object[]
                    {
                        new GuildHouseItemData
                        {
                            itemData = itemData,
                        }
                    });
                }
            }
            _UpdateSpace();
        }

        void _OnGuildSotrageOperationRecordsChanged(UIEvent uiEvent)
        {
            _RefreshStorageOperationRecords();
        }

        void _OnAwardStateChanged(UIEvent uiEvent)
        {
            _UpdateStatus();
            //_UpdateLotteryLeftTime();
        }

        void _OnGuildBattleStateChanged(UIEvent uiEvent)
        {
            _UpdateStatus();
        }

        void _UpdateStatus()
        {
            if (null != comStatus)
            {
                var eEGuildBattleState = GuildDataManager.GetInstance().GetGuildBattleState();
                if (eEGuildBattleState == EGuildBattleState.LuckyDraw)
                {
                    if (GuildDataManager.GetInstance().HasGuildBattleLotteryed())
                    {
                        comStatus.Key = "award_passed";
                    }
                    else
                    {
                        comStatus.Key = "award";
                    }
                }
                else
                {
                    comStatus.Key = "normal";
                }
            }
        }

        void _UpdateLotteryLeftTime()
        {
                m_repeatCallLeftTime = ClientSystemManager.GetInstance().delayCaller.RepeatCall(1000, () =>
                {
                    _UpdateLeftTime();
                }, 9999999, true);
        }

        void _UpdateLeftTime()
        {
            if(mLotteryLeftTime == null)
            {
                return;
            }

            if (GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.LuckyDraw)
            {
                uint nTimeLeft = GuildDataManager.GetInstance().GetGuildBattleStateEndTime() - TimeManager.GetInstance().GetServerTime();

                if (nTimeLeft > 0)
                {
                    mLotteryLeftTime.text = "抽奖剩余时间  " + Function.GetLeftTime((int)GuildDataManager.GetInstance().GetGuildBattleStateEndTime(), (int)TimeManager.GetInstance().GetServerTime(), ShowtimeType.OnlineGift);
                    mLotteryLeftTime.gameObject.CustomActive(true);
                }
                else
                {
                    mLotteryLeftTime.gameObject.CustomActive(false);
                }
            }
            else
            {
                mLotteryLeftTime.gameObject.CustomActive(false);
            }
        }

        void _UpdateSpace()
        {
            if (null != space)
            {
                space.text = string.Format("{0}/{1}", GuildDataManager.GetInstance().storeDatas.Count, GuildDataManager.GetInstance().storeageCapacity);
            }
        }

        void _UpdateContributeCtrl()
        {
            GuildPost ePost = (GuildPost)GuildDataManager.GetInstance().GetServerDuty(PlayerBaseData.GetInstance().eGuildDuty);
            bool bHasPower = ePost >= GuildDataManager.GetInstance().contributePower;
            goContribute.CustomActive(bHasPower);
        }

        void _UpdateClearCtrl()
        {
            GuildPost ePost = (GuildPost)GuildDataManager.GetInstance().GetServerDuty(PlayerBaseData.GetInstance().eGuildDuty);
            bool bHasPower = ePost >= GuildDataManager.GetInstance().clearPower;
            goClear.CustomActive(bHasPower);
        }

        void _OnGuildPowerChanged(PowerSettingType ePowerSettingType, int iPowerValue)
        {
            switch (ePowerSettingType)
            {
                case PowerSettingType.PST_WIN_POWER:
                case PowerSettingType.PST_LOSE_POWER:
                    {
                        _UpdateRecordHead();
                    }
                    break;
                case PowerSettingType.PST_CONTRIBUTE_POWER:
                    {
                        _UpdateContributeCtrl();
                    }
                    break;
                case PowerSettingType.PST_DELETE_POWER:
                    {
                        _UpdateClearCtrl();
                    }
                    break;
            }
        }

        void _UpdateRecordHead()
        {
            if (null != recordHead)
            {
                recordHead.text = TR.Value("guild_store_house_award_hint", GuildDataManager.GetInstance().winPower, GuildDataManager.GetInstance().losePower);
            }
        }

        void _UpdateButtonStatus()
        {
            EGuildDuty eEGuildDuty = PlayerBaseData.GetInstance().eGuildDuty;
            goSetting.CustomActive(eEGuildDuty == EGuildDuty.Leader);
            goClear.CustomActive(eEGuildDuty >= EGuildDuty.Elder);
        }

        [UIObject("middleback/Goods/Scroll View/Viewport/Content")]
        GameObject goParent;
        [UIObject("middleback/Goods/Scroll View/Viewport/Content/Prefab")]
        GameObject goPrefab;

        void _RefreshGuildItems()
        {
            goPrefab.CustomActive(false);
            m_akHouseItemDatas.RecycleAllObject();
            var datas = GuildDataManager.GetInstance().storeDatas;
            datas.Sort((x, y) =>
            {
                if (x.Quality != y.Quality)
                {
                    return y.Quality - x.Quality;
                }

                if (x.GUID < y.GUID)
                {
                    return -1;
                }

                if (x.GUID == y.GUID)
                {
                    return 0;
                }

                return 1;
            });
            int iStorageCount = GuildDataManager.GetInstance().storeageCapacity;
            for (int i = 0; i < iStorageCount; ++i)
            {
                if (i < datas.Count)
                {
                    var itemData = datas[i];
                    m_akHouseItemDatas.Create(new object[]
                    {
                        goParent,
                        goPrefab,
                        new GuildHouseItemData
                        {
                            itemData = itemData,
                        },
                        false,
                    });
                }
                else
                {
                    m_akHouseItemDatas.Create(new object[]
                    {
                        goParent,
                        goPrefab,
                        new GuildHouseItemData
                        {
                            itemData = null,
                        },
                        false,
                    });
                }
            }
            _SortStorageItem();
        }

        void _OnClickSetting()
        {
            if (PlayerBaseData.GetInstance().eGuildDuty != EGuildDuty.Leader)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_store_house_setting_need_power"));
                return;
            }

            var argv = new GuildStoreHousePowerSettingFrameData
            {
                toggle0s = new List<int> { 0, 30, 50, 80, 100 },
                toggle1s = new List<int> { 0, 30, 50, 80, 100 },
            };

            GuildStoreHousePowerSettingFrame.CommandOpen(argv);
        }

        void _OnClickClear()
        {
            EGuildDuty eEGuildDuty = PlayerBaseData.GetInstance().eGuildDuty;
            GuildPost ePost = (GuildPost)GuildDataManager.GetInstance().GetServerDuty(eEGuildDuty);
            if (ePost < GuildDataManager.GetInstance().clearPower)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_store_house_clear_need_power"));
                return;
            }

            GuildStoreHouseClearFrame.CommandOpen(GuildStoreHouseClearFrame.ReadyStoreRemoveData());
        }

        void _OnGotoShop()
        {
            //ShopDataManager.GetInstance().OpenShop(11);
            frameMgr.OpenFrame<GuildStoreShopFrame>(FrameLayer.Middle);
        }

        void _OnClickContribute()
        {
            EGuildDuty eEGuildDuty = PlayerBaseData.GetInstance().eGuildDuty;
            GuildPost ePost = (GuildPost)GuildDataManager.GetInstance().GetServerDuty(eEGuildDuty);
            if (ePost < GuildDataManager.GetInstance().contributePower)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_store_house_store_need_power"));
                return;
            }

            GuildStoreHouseClearFrame.CommandOpen(GuildStoreHouseClearFrame.ReadyStoreAddData());
        }

        void _OnClickAward()
        {
            var eEGuildBattleState = GuildDataManager.GetInstance().GetGuildBattleState();
            if (eEGuildBattleState != EGuildBattleState.LuckyDraw)
            {
                Logger.LogErrorFormat("battle state is not LuckyDraw !");
                return;
            }

            GuildDataManager.GetInstance().SendGuildBattleLotteryReq();
        }

        protected override void _OnCloseFrame()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBattleStateChanged, _OnGuildBattleStateChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildHouseItemAdd, _OnStorageItemAdd);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildHouseItemRemoved, _OnStorageRemoved);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildHouseItemUpdate, _OnStorageItemUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildSotrageOperationRecordsChanged, _OnGuildSotrageOperationRecordsChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildLotteryResultRes, _OnAwardStateChanged);
            GuildDataManager.GetInstance().onGuildPowerChanged -= _OnGuildPowerChanged;
            m_akHouseItemDatas.DestroyAllObjects();
            if (null != comSotreRecordsList)
            {
                comSotreRecordsList.onBindItem = null;
                comSotreRecordsList.onItemVisiable = null;
                comSotreRecordsList = null;
            }
            comSuperLinkText = null;

            ClientSystemManager.GetInstance().delayCaller.StopItem(m_repeatCallLeftTime);
        }

        CachedObjectListManager<GuildHouseItem> m_akHouseItemDatas = new CachedObjectListManager<GuildHouseItem>();

        #region ExtraUIBind
        private Text mLotteryLeftTime = null;
        private ScrollRect storeItemList = null;

        protected override void _bindExUI()
        {
            mLotteryLeftTime = mBind.GetCom<Text>("LotteryLeftTime");
            storeItemList = mBind.GetCom<ScrollRect>("storeItemList");
        }

        protected override void _unbindExUI()
        {
            mLotteryLeftTime = null;
            storeItemList = null;
        }
        #endregion
    }
}
