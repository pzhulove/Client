using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using EJarType = ProtoTable.JarBonus.eType;
using Protocol;
using ProtoTable;

namespace GameClient
{

    class JarFrame : ClientFrame
    {

        public enum ToggleType
        {
            MagicJar,
            ArtifactJar,
            FashionJar,
            GoldJar,
        }

        #region OpenLink
        /// <summary>
        /// strParam Paramater
        /// 0 Magic;   1: Gold;  2：Magic_Lv55
        /// 小于0，大于2 默认打开0(Magic)
        /// </summary>
        /// <param name="strParam"></param>
        public static void OpenLinkFrame(string strParam)
        {
            try
            {
                //limit
                if (strParam == null || strParam.Length <= 0)
                {
                    mCurSelectToggleId = -1;
                    ClientSystemManager.GetInstance().OpenFrame<JarFrame>(FrameLayer.Middle);
                    return;
                }

                string[] paramArray = strParam.Split('|');
                if (paramArray.Length > 0)
                {
                    int nType = int.Parse(paramArray[0]);
                    //EPocketJarType eType = (EPocketJarType)nType;
                    mCurSelectToggleId = nType;
                }
                ClientSystemManager.GetInstance().OpenFrame<JarFrame>(FrameLayer.Middle);
            }
            catch(Exception e)
            {
                Logger.LogError("PocketJarFrame.OpenLinkFrame : ==>" + e.ToString());
            }
        }
        #endregion

        delegate void OnRedPointChanged();


        [UIControl("BG/Title/Moneys/MagicTicket", typeof(ComCommonConsume))]
        ComCommonConsume m_comMagicTicket;

        EPocketJarType mCurPocketJarType = EPocketJarType.Invalid;

        private static int mCurSelectToggleId = -1;

        //
        CommonTabToggleGroup comTabToggle;
        Dictionary<int, CommonTabToggleView> dicPage = new Dictionary<int, CommonTabToggleView>();
        List<CommonTabData> commonTabDatas = new List<CommonTabData>();

        Transform pageRoot;


        #region ClientFrame
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Jar/JarFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            _RegisterUIEvent();
            BindUIEvent();
            _InitUI();
        }

        protected sealed override void _OnCloseFrame()
        {
            mCurPocketJarType = EPocketJarType.Invalid;
            mCurSelectToggleId = -1;

            _ClearUI();
            UnBindUIEvent();
            _UnRegisterUIEvent();
            dicPage.Clear();
            commonTabDatas.Clear();
        }
        
        protected override void _bindExUI()
        {
            //preViewModel = mBind.GetCom<Button>("preViewModel");
            //preViewModel.SafeSetOnClickListener(() => 
            //{
            //    List<PreViewItemData> preViewDataList = new List<PreViewItemData>();
            //    if(preViewDataList == null)
            //    {
            //        return;                    
            //    }
            //
            //    if(m_magicJarData == null)
            //    {
            //        return;
            //    }
            //
            //    if(m_magicJarData.arrBonusItems == null)
            //    {
            //        return;
            //    }
            //
            //    List<ItemSimpleData> items = m_magicJarData.arrBonusItems;
            //    for (int i = 0; i < items.Count; i++)
            //    {
            //        CalcPreviewModelItemIDList(items[i].ItemID, ref preViewDataList);
            //    }
            //
            //    // 先排序 具体规则见 GetRank
            //    SortedDictionary<int, List<PreViewItemData>> preViewDataDic = new SortedDictionary<int, List<PreViewItemData>>();
            //    List<PreViewItemData> preViewDataListTemp = new List<PreViewItemData>();
            //    if (preViewDataDic != null && preViewDataListTemp != null)
            //    {
            //        for (int i = 0; i < preViewDataList.Count; i++)
            //        {
            //            PreViewItemData preViewItemData = preViewDataList[i];
            //            if (preViewItemData == null)
            //            {
            //                continue;
            //            }
            //
            //            int iRank = GetRank(preViewItemData.itemId);
            //            if(!preViewDataDic.ContainsKey(iRank))
            //            {
            //                preViewDataDic[iRank] = new List<PreViewItemData>();                            
            //            }
            //
            //            preViewDataDic[iRank].Add(preViewItemData);
            //        }
            //
            //        var iter = preViewDataDic.GetEnumerator();               
            //        while (iter.MoveNext())
            //        {
            //            List<PreViewItemData> adt = iter.Current.Value as List<PreViewItemData>;
            //            if (adt == null)
            //            {
            //                continue;
            //            }
            //
            //            preViewDataListTemp.AddRange(adt);
            //        }
            //
            //        preViewDataList = preViewDataListTemp;
            //    }
            //
            //    // 相同道具，选择时限最长的
            //    int id = 0;                
            //    Dictionary<long, List<PreViewItemData>> resID2ItemData = new Dictionary<long, List<PreViewItemData>>();
            //    if (resID2ItemData != null)
            //    {
            //        for (int i = 0; i < preViewDataList.Count; i++)
            //        {
            //            PreViewItemData preViewItemData = preViewDataList[i];
            //            if (preViewItemData == null)
            //            {
            //                continue;
            //            }
            //
            //            ItemData itemData = ItemDataManager.CreateItemDataFromTable(preViewItemData.itemId);
            //            if (itemData == null)
            //            {
            //                continue;
            //            }
            //
            //            if (itemData.TableData == null)
            //            {
            //                continue;
            //            } 
            //          
            //            long uid = HashToLong(itemData.TableData.ResID, itemData.TableData.EquipPropID);
            //
            //            if (uid == 0) // 既没有资源id，也没有属性id 呵呵
            //            {
            //                uid = id++;
            //            }
            //
            //            if (!resID2ItemData.ContainsKey(uid))
            //            {
            //                resID2ItemData[uid] = new List<PreViewItemData>();
            //            }
            //
            //            resID2ItemData[uid].Add(preViewItemData);
            //        }
            //
            //        preViewDataList.Clear();
            //
            //        var iter = resID2ItemData.GetEnumerator();
            //        while (iter.MoveNext())
            //        {
            //            List<PreViewItemData> adt = iter.Current.Value as List<PreViewItemData>;
            //            if (adt == null)
            //            {
            //                continue;
            //            }
            //
            //            int iIndex = -1;
            //            int iTimeLeft = 0;
            //            PreViewItemData preViewItemDataTemp = null;
            //            for (int i = 0;i < adt.Count;i++)
            //            {
            //                PreViewItemData preViewItemData = adt[i];
            //                if (preViewItemData == null)
            //                {
            //                    continue;
            //                }
            //
            //                ItemData itemData = ItemDataManager.CreateItemDataFromTable(preViewItemData.itemId);
            //                if (itemData == null)
            //                {
            //                    continue;
            //                }
            //
            //                if (itemData.TableData == null)
            //                {
            //                    continue;
            //                }
            //
            //                if(itemData.TableData.TimeLeft == 0)
            //                {
            //                    iIndex = i;
            //                    break;
            //                }
            //
            //                if(itemData.TableData.TimeLeft > iTimeLeft)
            //                {
            //                    iIndex = i;
            //                }
            //            }
            //
            //            if(iIndex >= 0 && iIndex < adt.Count)
            //            {
            //                preViewItemDataTemp = adt[iIndex];
            //                preViewDataList.Add(preViewItemDataTemp);
            //            }
            //        }
            //    }
            //  
            //    PreViewDataModel preViewDataModel = new PreViewDataModel();
            //    if (preViewDataModel == null)
            //    {
            //        return;
            //    }
            //
            //    preViewDataModel.isCreatItem = true;
            //    preViewDataModel.preViewItemList = preViewDataList;
            //
            //    ClientSystemManager.GetInstance().OpenFrame<PreviewModelFrame>(FrameLayer.Middle, preViewDataModel);
            //});
        }

        protected override void _unbindExUI()
        {
        }
        #endregion


        static long HashToLong(int n1, int n2)
        {
            long ret = n1;
            ret <<= 32;
            ret += n2;
            return ret;
        }

        // 排序规则为 光环、翅膀、称号、宠物
        int GetRank(int itemID)
        {
            ItemData itemData = ItemDataManager.CreateItemDataFromTable(itemID);
            if (itemData == null)
            {
                return -1;
            }

            if (itemData.SubType == (int)ItemTable.eSubType.FASHION_AURAS)
            {
                return 0;
            }

            if(itemData.SubType == (int)ItemTable.eSubType.FASHION_HAIR)
            {
                return 1;
            }

            if(itemData.Type == ItemTable.eType.FUCKTITTLE)
            {
                return 2;
            }

            if (itemData.SubType == (int)ItemTable.eSubType.PetEgg)
            {
                return 3;
            }

            return -1;
        }

        // 计算某个道具可以进行模型预览(礼包里面嵌套礼包会进行递归处理)
        void CalcPreviewModelItemIDList(int itemID,ref List<PreViewItemData> preViewIDList)
        {
            if(preViewIDList == null)
            {
                return;
            }

            ItemData item = ItemDataManager.CreateItemDataFromTable(itemID);
            if (item == null)
            {
                return;
            }

            // 开始计算id list
            List<GiftTable> arrGifts = item.GetGifts();
            if (arrGifts != null) // 如果是礼包，则检查里面的道具是否可以进行模型预览
            {
                for (int j = 0; j < arrGifts.Count; j++)
                {
                    CalcPreviewModelItemIDList(arrGifts[j].ItemID, ref preViewIDList);
                }                
            }
            else // 不是礼包，则判断该道具是否可以进行模型预览
            {
                if (CanPreviewModel(itemID))
                {
                    preViewIDList.Add(new PreViewItemData() { itemId = itemID});
                }
            }

            return;
        }

        bool CanPreviewModel(int itemID)
        {
            ItemData itemData = ItemDataManager.CreateItemDataFromTable(itemID);
            if (itemData == null)
            {
                return false;
            }

            // 时装翅膀、时装光环、称号、宠物支持模型预览
            if (itemData.SubType == (int)ItemTable.eSubType.FASHION_AURAS 
            || itemData.SubType == (int)ItemTable.eSubType.FASHION_HAIR
            || itemData.Type == ItemTable.eType.FUCKTITTLE
            || itemData.SubType == (int)ItemTable.eSubType.PetEgg)
            {
                return true;
            }

            return false;
        }

        #region UIEvent
        //UI系统之间事件
        protected void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MagicJarUseSuccess, _OnUpdateJar);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerDataBaseUpdated, _OnUpdateJar);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MagicJarScoreChanged, _OnMagicJarScoreChanged);
        }

        protected void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MagicJarUseSuccess, _OnUpdateJar);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerDataBaseUpdated, _OnUpdateJar);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MagicJarScoreChanged, _OnMagicJarScoreChanged);
        }

        //UI控件之间事件的绑定，应该分离到View
        void BindUIEvent()
        {
            pageRoot = mBind.GetCom<Transform>("Groups");
            comTabToggle = mBind.GetCom<CommonTabToggleGroup>("Tab");

            if (comTabToggle != null)
            {
                _InitToggleListData();

                comTabToggle.InitComTab((CommonTabData tabData) => {

                    foreach (var item in dicPage)
                    {
                        item.Value.CustomActive(false);
                        item.Value.Unselect();
                    }

                    CommonTabToggleView view;
                    if (!dicPage.TryGetValue(tabData.id, out view))
                    {
                        var path = mBind.GetPrefabPath(tabData.id.ToString());
                        var obj = AssetLoader.instance.LoadResAsGameObject(path);
                        if(obj != null)
                        {
                            Utility.AttachTo(obj, pageRoot.gameObject);
                            view = obj.GetComponent<CommonTabToggleView>();
                            dicPage[tabData.id] = view;
                            view.Select(this);
                        }
                    }
                    else
                    {
                        view.CustomActive(true);
                        view.Select(this);
                    }

                }, mCurSelectToggleId, commonTabDatas);
            }

        }

        private void _InitToggleListData()
        {
            bool activityIsUnLock = true;
            bool jarIsUnLock = true;
//#if APPLE_STORE
            //add by mjx for ios appstore
            activityIsUnLock = activityIsUnLock && !IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_JAR);
            jarIsUnLock = jarIsUnLock && !IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.LIMITTIME_JAR);
//#endif
            activityIsUnLock = activityIsUnLock && JarDataManager.GetInstance().HasActivityJar() && Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.ActivityJar);
            jarIsUnLock = jarIsUnLock && Utility.IsUnLockFunc((int)FunctionUnLock.eFuncType.Jar);

            List<CommonTabData> tempCommonTabDatas = comTabToggle.GetTabDatas();
            foreach (var v in tempCommonTabDatas)
            {
                if (v == null)
                {
                    continue;
                }

                if (activityIsUnLock && (v.id == (int)ToggleType.ArtifactJar || v.id == (int)ToggleType.FashionJar))
                {
                    commonTabDatas.Add(v);
                }

                if (jarIsUnLock && (v.id == (int)ToggleType.MagicJar || v.id == (int)ToggleType.GoldJar))
                {
                    commonTabDatas.Add(v);
                }
            }

        }

        void UnBindUIEvent()
        {
        }

        public sealed override bool IsNeedUpdate()
        {
            return true;
        }

        protected sealed override void _OnUpdate(float timeElapsed)
        {
            _UpdateFreeCD(m_magicJarData, timeElapsed);
        }

        #endregion

        void _InitUI()
        {
            _InitGoldJarUI();
            _UpdateRedPoint();
            if (userData != null)
            {
                mCurPocketJarType = (EPocketJarType)userData;
            }
            else
            {
                
            }
        }

        void _ClearUI()
        {
            _ClearGoldJarUI();
            _ClearMagicJarUI();
        }

        void _UpdateRedPoint()
        {
            //GameObject magicRedpoint = Utility.FindGameObject(m_togMagicJar.gameObject, "RedPoint");
            //if(magicRedpoint != null)
            //{
            //    magicRedpoint.CustomActive(JarDataManager.GetInstance().CheckRedPoint(EJarType.MagicJar));
            //}
            //
            //GameObject goldRedPoint = Utility.FindGameObject(m_togGoldJar.gameObject, "RedPoint");
            //if(goldRedPoint != null)
            //{
            //    goldRedPoint.CustomActive(JarDataManager.GetInstance().CheckRedPoint(EJarType.GoldJar));
            //}
            //
            //GameObject magicLv55RedPoint = Utility.FindGameObject(m_togMagic_Jar_Lv55.gameObject, "RedPoint");
            //if (magicLv55RedPoint != null)
            //{
            //    magicLv55RedPoint.CustomActive(JarDataManager.GetInstance().CheckRedPoint(EJarType.MagicJar_Lv55));
            //}
        }

        void _OnUpdateJar(UIEvent a_event)
        {
            _UpdateGoldGoods(m_currGoldJarData);
            //_UpdateMagicJarGoods(m_magicJarData);
            _UpdateRedPoint();
        }

        void _OnMagicJarScoreChanged(UIEvent a_event)
        {
            m_labMagicScore.SafeSetText(PlayerBaseData.GetInstance().MagicJarScore.ToString());
        }

        //[UIEventHandle("BG/Title/Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }

        #region gold jar
        [UIObject("Content/Groups/GoldGroup/Tabs")]
        GameObject m_objGoldLevelTabRoot;

        [UIObject("Content/Groups/GoldGroup/Tabs/Tab")]
        GameObject m_objGoldLevelTabTemplate;

        [UIControl("Content/Groups/GoldGroup/Page/Right/Items")]
        ComUIListScript m_comGoldItemList;

        [UIControl("Content/Groups/GoldGroup/Page/Left/Jar")]
        Image m_imgGoldJarIcon;

        [UIControl("Content/Groups/GoldGroup/Page/Left/Jar/Name")]
        Text m_labGoldJarName;

        [UIControl("Content/Groups/GoldGroup/Page/Left/BuyDesc/Title")]
        Text m_labGoldBuyItemTitle;

        [UIObject("Content/Groups/GoldGroup/Page/Left/BuyDesc/ItemRoot")]
        GameObject m_objGoldBuyItemRoot;

        [UIControl("Content/Groups/GoldGroup/Page/Left/BuyDesc/Desc")]
        Text m_labGoldBuyItemDesc;

        [UIObject("Content/Groups/GoldGroup/Page/Right/Buy")]
        GameObject m_objGoldBuyFuncRoot;

        bool m_bGoldJarInited = false;
        JarData m_currGoldJarData = null;
        List<GameObject> m_arrLevelTypeObjs = new List<GameObject>();
        List<OnRedPointChanged> m_arrLevelRedPoints = new List<OnRedPointChanged>();

        protected void _InitGoldJarUI()
        {
            JarDataManager.GetInstance().RequestQuaryJarShopSocre();

            m_objGoldLevelTabTemplate.SetActive(false);
            m_objGoldLevelTabTemplate.transform.SetParent(frame.transform, false);

            m_comGoldItemList.Initialize();

            m_comGoldItemList.onBindItem = var =>
            {
                return CreateComItem(Utility.FindGameObject(var, "Item"));
            };

            m_comGoldItemList.onItemVisiable = var =>
            {
                if (m_currGoldJarData != null)
                {
                    List<ItemSimpleData> items = m_currGoldJarData.arrBonusItems;

                    if (var.m_index >= 0 && var.m_index < items.Count)
                    {
                        ItemData item = ItemDataManager.CreateItemDataFromTable(items[var.m_index].ItemID);
                        if (item != null)
                        {
                            item.Count = items[var.m_index].Count;
                            ComItem comItem = var.gameObjectBindScript as ComItem;

                            comItem.Setup(item, (var1, var2) =>
                            {
                                ItemTipManager.GetInstance().ShowTip(var2);
                                Utility.DoStartFrameOperation("PocketJarFrame", string.Format("GoldenPot_ItemId_{0}", var2.TableID));
                            });

                            Utility.GetComponetInChild<Text>(var.gameObject, "Name").text = item.GetColorName();
                        }
                    }
                }
            };

            _ClearGoldLevelTypes();
            _ClearGoldGoods();

            ProtoTable.JobTable jobTable = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (jobTable != null)
            {
                _InitGoldLevelTypes(jobTable.SuitArmorType);
            }
            else
            {
                Logger.LogErrorFormat("袖珍罐：不存在职业id{0}", PlayerBaseData.GetInstance().JobTableID);
            }
        }

        protected void _ClearGoldJarUI()
        {
            m_currGoldJarData = null;
            m_arrLevelTypeObjs.Clear();
            m_arrLevelRedPoints.Clear();
        }

        void _InitGoldLevelTypes(int a_nSubType)
        {
            if(m_arrLevelRedPoints != null)
            {
                m_arrLevelRedPoints.Clear();
            }

            List<Toggle> toggles = new List<Toggle>();
            bool isToggleInited = false;

            List<JarTreeNode> arrLevelTypes = JarDataManager.GetInstance().GetGoldJarLevels(a_nSubType);

            if(arrLevelTypes != null)
            {
                for (int i = 0; i < arrLevelTypes.Count; ++i)
                {
                    int nLevelType = arrLevelTypes[i].nKey;
                    GameObject objLevelType = null;

                    if(m_arrLevelTypeObjs == null)
                    {
                        m_arrLevelTypeObjs = new List<GameObject>();
                    }

                    if (i < m_arrLevelTypeObjs.Count)
                    {
                        objLevelType = m_arrLevelTypeObjs[i];
                    }
                    else
                    {
                        if(m_objGoldLevelTabTemplate != null && m_objGoldLevelTabRoot != null)
                        {
                            objLevelType = GameObject.Instantiate(m_objGoldLevelTabTemplate);
                            objLevelType.transform.SetParent(m_objGoldLevelTabRoot.transform, false);
                            m_arrLevelTypeObjs.Add(objLevelType);
                        }
                    }

                    if(objLevelType != null)
                    {
                        objLevelType.SetActive(true);

                        Toggle toggle = objLevelType.GetComponent<Toggle>();

                        if(toggle != null)
                        {
                            toggle.onValueChanged.RemoveAllListeners();
                            toggle.onValueChanged.AddListener(var =>
                            {
                                if (isToggleInited)
                                {
                                    if (var)
                                    {
                                        m_currGoldJarData = JarDataManager.GetInstance().GetGoldJarData(a_nSubType, nLevelType);
                                        _ClearGoldGoods();
                                        _InitGoldGoods(m_currGoldJarData);

                                        Utility.DoStartFrameOperation("PocketJarFrame", string.Format("GoldenPot_Level_{0}", TR.Value("goldjar_level_type", nLevelType)));
                                    }
                                }
                            });
                            toggles.Add(toggle);
                        }
                    }

                    string strLevelName = TR.Value("goldjar_level_type", nLevelType);

                    Text Checkmark = Utility.GetComponetInChild<Text>(objLevelType, "Checkmark/Label");
                    if(Checkmark != null)
                    {
                        Checkmark.text = strLevelName;
                    }

                    Text Background = Utility.GetComponetInChild<Text>(objLevelType, "Background/Label");
                    if(Background != null)
                    {
                        Background.text = strLevelName;
                    }

                    if(m_arrLevelRedPoints != null)
                    {
                        m_arrLevelRedPoints.Add(() =>
                        {
                            GameObject RedPoint = Utility.FindGameObject(objLevelType, "RedPoint");

                            if(RedPoint != null)
                            {
                                RedPoint.SetActive(JarDataManager.GetInstance().CheckGoldJarLevelRedPoint(a_nSubType, nLevelType));
                            }
                        });
                    }
                }
            }

            if(m_arrLevelRedPoints != null)
            {
                for (int i = 0; i < m_arrLevelRedPoints.Count; ++i)
                {
                    if(m_arrLevelRedPoints[i] != null)
                    {
                        m_arrLevelRedPoints[i].Invoke();
                    }
                }
            }

            isToggleInited = true;

            int nIdx = _GetLevelMatchedJarIndex(arrLevelTypes);
            if (arrLevelTypes != null && nIdx >= 0 && nIdx < arrLevelTypes.Count && nIdx < toggles.Count)
            {
                toggles[nIdx].isOn = true;
            }
            else
            {
                if (toggles.Count > 0)
                {
                    toggles[0].isOn = true;
                }
            }
        }

        int _GetLevelMatchedJarIndex(List<JarTreeNode> a_arrLevelTypes)
        {
            if (a_arrLevelTypes == null)
            {
                return -1;
            }

            int nCurrLevel = 0;
            int nIndex = -1;

            int nPlayerLevel = PlayerBaseData.GetInstance().Level;
            for (int i = 0; i < a_arrLevelTypes.Count; ++i)
            {
                int nLevelType = a_arrLevelTypes[i].nKey;
                JarData jarData = a_arrLevelTypes[i].value as JarData;

                if (nLevelType <= nPlayerLevel && nLevelType > nCurrLevel)
                {
                    nCurrLevel = nLevelType;
                    nIndex = i;
                }
            }

            return nIndex;
        }

        void _ClearGoldLevelTypes()
        {
            for (int i = 0; i < m_arrLevelTypeObjs.Count; ++i)
            {
                m_arrLevelTypeObjs[i].SetActive(false);
            }
        }

        void _InitGoldGoods(JarData a_data)
        {
            if (a_data == null)
            {
                return;
            }

            m_comGoldItemList.SetElementAmount(a_data.arrBonusItems.Count);

            // m_imgGoldJarIcon.sprite = AssetLoader.GetInstance().LoadRes(a_data.strJarImagePath, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref m_imgGoldJarIcon, a_data.strJarImagePath);
            m_labGoldJarName.text = a_data.strName;
            ComItem comItem = CreateComItem(m_objGoldBuyItemRoot);
            Assert.IsTrue(a_data.arrBuyItems != null && a_data.arrBuyItems.Count > 0);
            ItemData buyItem = a_data.arrBuyItems[0];
            comItem.Setup(buyItem, (var1, var2) =>
            {
                ItemTipManager.GetInstance().ShowTip(var2);
            });
            m_labGoldBuyItemTitle.text = TR.Value("goldjar_buy_item_title", buyItem.GetColorName());
            m_labGoldBuyItemDesc.text = TR.Value("goldjar_buy_desc");

            for (int i = 0; i < a_data.arrBuyInfos.Count; ++i)
            {
                GameObject objBuy = m_objGoldBuyFuncRoot.transform.GetChild(i).gameObject;
                objBuy.SetActive(true);
                JarBuyInfo buyInfo = a_data.arrBuyInfos[i];

                GameObject objEffect = Utility.FindGameObject(objBuy, "RedPoint");
                objEffect.SetActive(false);

                Button btnBuy = objBuy.GetComponent<Button>();
                btnBuy.onClick.RemoveAllListeners();
                btnBuy.onClick.AddListener(() =>
                {
                    ShowItemsFrame.bSkipExplode = false;

                    Assert.IsTrue(buyInfo.arrCosts.Count >= 1);
                    CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                    for (int j = 0; j < buyInfo.arrCosts.Count; ++j)
                    {
                        JarBuyCost cost = buyInfo.arrCosts[j];
                        int nCount = cost.GetRealCostCount(buyInfo.nBuyCount);
                        if (nCount <= ItemDataManager.GetInstance().GetOwnedItemCount((int)cost.item.TableID))
                        {
                            costInfo.nMoneyID = cost.item.TableID;
                            costInfo.nCount = nCount;
                            break;
                        }
                        else
                        {
                            if (j == buyInfo.arrCosts.Count - 1)
                            {
                                costInfo.nMoneyID = cost.item.TableID;
                                costInfo.nCount = nCount;
                            }
                        }
                    }

                    CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                    {
                        JarDataManager.GetInstance().RequestBuyJar(a_data, buyInfo);
                    });

                    Utility.DoStartFrameOperation("PocketJarFrame", string.Format("GoldenPot_Buy{0}", buyInfo.nBuyCount));
                });

                Utility.GetComponetInChild<Text>(objBuy, "Time").text = TR.Value("magicjar_buy_times", buyInfo.nBuyCount);
                Text labCount = Utility.GetComponetInChild<Text>(objBuy, "Price/Count");
                Image imgIcon = Utility.GetComponetInChild<Image>(objBuy, "Price/Icon");
                for (int j = 0; j < buyInfo.arrCosts.Count; ++j)
                {
                    JarBuyCost cost = buyInfo.arrCosts[j];
                    int nCount = cost.GetRealCostCount(buyInfo.nBuyCount);
                    if (nCount <= ItemDataManager.GetInstance().GetOwnedItemCount((int)cost.item.TableID))
                    {
                        labCount.text = nCount.ToString();
                        // imgIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);
                        break;
                    }
                    else
                    {
                        if (j == buyInfo.arrCosts.Count - 1)
                        {
                            labCount.text = TR.Value("color_red", nCount);
                            // imgIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);
                        }
                    }
                }
            }
        }

        void _ClearGoldGoods()
        {
            m_comGoldItemList.SetElementAmount(0);
        }

        void _UpdateGoldGoods(JarData a_data)
        {
            for (int i = 0; i < a_data.arrBuyInfos.Count; ++i)
            {
                GameObject objBuy = m_objGoldBuyFuncRoot.transform.GetChild(i).gameObject;
                objBuy.SetActive(true);
                JarBuyInfo buyInfo = a_data.arrBuyInfos[i];

                GameObject objEffect = Utility.FindGameObject(objBuy, "RedPoint");
                objEffect.SetActive(JarDataManager.GetInstance().CheckRedPoint(a_data, buyInfo));

                Text labCount = Utility.GetComponetInChild<Text>(objBuy, "Price/Count");
                Image imgIcon = Utility.GetComponetInChild<Image>(objBuy, "Price/Icon");
                for (int j = 0; j < buyInfo.arrCosts.Count; ++j)
                {
                    JarBuyCost cost = buyInfo.arrCosts[j];
                    int nCount = cost.GetRealCostCount(buyInfo.nBuyCount);
                    if (nCount <= ItemDataManager.GetInstance().GetOwnedItemCount((int)cost.item.TableID))
                    {
                        labCount.text = nCount.ToString();
                        // imgIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);
                        break;
                    }
                    else
                    {
                        if (j == buyInfo.arrCosts.Count - 1)
                        {
                            labCount.text = TR.Value("color_red", nCount);
                            // imgIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);
                        }
                    }
                }
            }

            for (int i = 0; i < m_arrLevelRedPoints.Count; ++i)
            {
                m_arrLevelRedPoints[i].Invoke();
            }
        }
        #endregion

        #region magic jar
        [UIControl("Content/Groups/MagicGroup/Page/Right/Items")]
        ComUIListScript m_comMagicItemList;

        [UIControl("Content/Groups/MagicGroup/Page/Left/Jar")]
        Image m_imgMagicJarIcon;

        [UIControl("Content/Groups/MagicGroup/Page/Left/Jar/Name")]
        Text m_labMagicJarName;

        [UIControl("Content/Groups/MagicGroup/Page/Left/BuyDesc/Title")]
        Text m_labMagicBuyItemTitle;

        [UIObject("Content/Groups/MagicGroup/Page/Left/BuyDesc/ItemRoot")]
        GameObject m_objMagicBuyItemRoot;

        [UIControl("Content/Groups/MagicGroup/Page/Left/BuyDesc/Desc")]
        Text m_labMagicBuyItemDesc;

        [UIObject("Content/Groups/MagicGroup/Page/Right/Buy")]
        GameObject m_objMagicBuyRoot;

        [UIControl("Content/Groups/MagicGroup/Page/Right/Title/MagicScore")]
        Text m_labMagicScore;

        [UIControl("Content/Groups/MagicGroup/Page/Right/Title/Text")]
        Text m_labMagicScoreDesc;

        JarData m_magicJarData
        {
            get
            {
                if (mCurPocketJarType == EPocketJarType.Magic_Lv55)
                {
                    return JarDataManager.GetInstance().GetMagicJarData_Lv55();
                }
                else
                {
                    return JarDataManager.GetInstance().GetMagicJarData();
                }
            }

            //set{ m_magicJarData = value; }
        }


        float m_fUpdateTime = 0.0f;

        void _InitMagicJarUI()
        {
            m_comMagicItemList.Initialize();

            m_comMagicItemList.onBindItem = var =>
            {
                return CreateComItem(Utility.FindGameObject(var, "Item"));
            };

            m_comMagicItemList.onItemVisiable = var =>
            {
                if (m_magicJarData != null)
                {
                    List<ItemSimpleData> items = m_magicJarData.arrBonusItems;

                    if (var.m_index >= 0 && var.m_index < items.Count)
                    {
                        ItemData item = ItemDataManager.CreateItemDataFromTable(items[var.m_index].ItemID);

                        if (item != null)
                        {
                            item.Count = items[var.m_index].Count;
                            ComItem comItem = var.gameObjectBindScript as ComItem;

                            comItem.Setup(item, (var1, var2) =>
                            {
                                ItemTipManager.GetInstance().ShowTip(var2);
                                Utility.DoStartFrameOperation("PocketJarFrame", string.Format("MagicPot_ItemId_{0}", var2.TableID));
                            });

                            Utility.GetComponetInChild<Text>(var.gameObject, "Name").text = item.GetColorName();
                        }
                    }
                }
            };

            m_labMagicScore.text = PlayerBaseData.GetInstance().MagicJarScore.ToString();

            if (mCurPocketJarType == EPocketJarType.Magic_Lv55)
            {
                m_labMagicScoreDesc.text = TR.Value("MagicScoreDesc_Lv55");
            }
            else
            {
                m_labMagicScoreDesc.text = TR.Value("MagicScoreDesc");
            }

            //m_magicJarData = JarDataManager.GetInstance().GetMagicJarData();
            _InitMagicJarGoods(m_magicJarData);
        }

        void _ClearMagicJarUI()
        {
            //m_magicJarData = null;
        }

        void _InitMagicJarGoods(JarData a_data)
        {
            if (a_data == null)
            {
                return;
            }

            m_comMagicItemList.SetElementAmount(a_data.arrBonusItems.Count);

            // m_imgMagicJarIcon.sprite = AssetLoader.GetInstance().LoadRes(a_data.strJarImagePath, typeof(Sprite)).obj as Sprite;
            ETCImageLoader.LoadSprite(ref m_imgMagicJarIcon, a_data.strJarImagePath);
            m_labMagicJarName.text = a_data.strName;
            ComItem comItem = CreateComItem(m_objMagicBuyItemRoot);
            Assert.IsTrue(a_data.arrBuyItems != null && a_data.arrBuyItems.Count > 0);
            ItemData buyItem = a_data.arrBuyItems[0];
            comItem.Setup(buyItem, (var1, var2) =>
            {
                ItemTipManager.GetInstance().ShowTip(var2);
            });

            m_labMagicBuyItemTitle.text = TR.Value("magicjar_buy_item_title", buyItem.GetColorName());

            if (mCurPocketJarType == EPocketJarType.Magic_Lv55)
            {
                m_labMagicBuyItemDesc.text = TR.Value("magicjar_Lv55_buy_desc");
            }
            else
            {
                m_labMagicBuyItemDesc.text = TR.Value("magicjar_buy_desc");
            }

            for (int i = 0; i < m_objMagicBuyRoot.transform.childCount; ++i)
            {
                m_objMagicBuyRoot.transform.GetChild(i).gameObject.SetActive(false);
            }

            for (int i = 0; i < a_data.arrBuyInfos.Count; ++i)
            {
                GameObject objBuy = m_objMagicBuyRoot.transform.GetChild(i).gameObject;
                objBuy.SetActive(true);
                JarBuyInfo buyInfo = a_data.arrBuyInfos[i];

                Button btnBuy = objBuy.GetComponent<Button>();
                btnBuy.onClick.RemoveAllListeners();
                btnBuy.onClick.AddListener(() =>
                {
                    ShowItemsFrame.bSkipExplode = false;

                    if (buyInfo.nFreeCount > 0)
                    {
                        JarDataManager.GetInstance().RequestBuyJar(a_data, buyInfo);
                        return;
                    }

                    Assert.IsTrue(buyInfo.arrCosts.Count >= 1);
                    CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                    for (int j = 0; j < buyInfo.arrCosts.Count; ++j)
                    {
                        JarBuyCost cost = buyInfo.arrCosts[j];
                        int nCount = cost.GetRealCostCount(buyInfo.nBuyCount);
                        if (nCount <= ItemDataManager.GetInstance().GetOwnedItemCount((int)cost.item.TableID))
                        {
                            costInfo.nMoneyID = cost.item.TableID;
                            costInfo.nCount = nCount;
                            break;
                        }
                        else
                        {
                            if (j == buyInfo.arrCosts.Count - 1)
                            {
                                costInfo.nMoneyID = cost.item.TableID;
                                costInfo.nCount = nCount;
                            }
                        }
                    }

                    CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                    {
                        JarDataManager.GetInstance().RequestBuyJar(a_data, buyInfo);
                    });

                    Utility.DoStartFrameOperation("PocketJarFrame", string.Format("MagicPot_Buy{0}", buyInfo.nBuyCount));
                });

                Utility.GetComponetInChild<Text>(objBuy, "Time").text = TR.Value("magicjar_buy_times", buyInfo.nBuyCount);
                Text labCount = Utility.GetComponetInChild<Text>(objBuy, "Price/Count");
                Image imgIcon = Utility.GetComponetInChild<Image>(objBuy, "Price/Icon");
                for (int j = 0; j < buyInfo.arrCosts.Count; ++j)
                {
                    JarBuyCost cost = buyInfo.arrCosts[j];
                    int nCount = cost.GetRealCostCount(buyInfo.nBuyCount);
                    if (nCount <= ItemDataManager.GetInstance().GetOwnedItemCount((int)cost.item.TableID))
                    {
                        labCount.text = nCount.ToString();
                        // imgIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);
                        break;
                    }
                    else
                    {
                        if (j == buyInfo.arrCosts.Count - 1)
                        {
                            labCount.text = TR.Value("color_red", nCount);
                            // imgIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);
                        }
                    }
                }

                // 免费次数，时间
                _SetupFreeCD(objBuy, buyInfo);

                if (buyInfo.arrCosts.Count > 0)
                {
                    m_comMagicTicket.SetData(ComCommonConsume.eType.Item, ComCommonConsume.eCountType.Fatigue, (int)buyInfo.arrCosts[0].item.TableID);
                }
            }
        }

        public void OnHistoryClick(int id)
        {
            ClientSystemManager.GetInstance().OpenFrame<JarHistoryFrame>(FrameLayer.Middle, id);
        }

        public void OnPreTotalViewClick(List<ItemSimpleData> items)
        {
            if (items == null)
            {
                return;
            }

            List<ItemData> itemDatas = new List<ItemData>();

            foreach (var v in items)
            {
                if (v == null)
                {
                    continue;
                }

                ItemData item = ItemDataManager.CreateItemDataFromTable(v.ItemID);
                item.Count = v.Count;
                itemDatas.Add(item);
            }

            ClientSystemManager.GetInstance().OpenFrame<JarAwardsDetailFrame>(FrameLayer.Middle, itemDatas);
        }

        public void OnPreviewClicked(List<ItemSimpleData> items)
        {
            if (items == null)
            {
                return;
            }
            List<PreViewItemData> preViewDataList = new List<PreViewItemData>();

            for (int i = 0; i < items.Count; i++)
            {
                CalcPreviewModelItemIDList(items[i].ItemID, ref preViewDataList);
            }

            // 先排序 具体规则见 GetRank
            SortedDictionary<int, List<PreViewItemData>> preViewDataDic = new SortedDictionary<int, List<PreViewItemData>>();
            List<PreViewItemData> preViewDataListTemp = new List<PreViewItemData>();
            if (preViewDataDic != null && preViewDataListTemp != null)
            {
                for (int i = 0; i < preViewDataList.Count; i++)
                {
                    PreViewItemData preViewItemData = preViewDataList[i];
                    if (preViewItemData == null)
                    {
                        continue;
                    }

                    int iRank = GetRank(preViewItemData.itemId);
                    if (!preViewDataDic.ContainsKey(iRank))
                    {
                        preViewDataDic[iRank] = new List<PreViewItemData>();
                    }

                    preViewDataDic[iRank].Add(preViewItemData);
                }

                var iter = preViewDataDic.GetEnumerator();
                while (iter.MoveNext())
                {
                    List<PreViewItemData> adt = iter.Current.Value as List<PreViewItemData>;
                    if (adt == null)
                    {
                        continue;
                    }

                    preViewDataListTemp.AddRange(adt);
                }

                preViewDataList = preViewDataListTemp;
            }

            // 相同道具，选择时限最长的
            int id = 0;
            Dictionary<long, List<PreViewItemData>> resID2ItemData = new Dictionary<long, List<PreViewItemData>>();
            if (resID2ItemData != null)
            {
                for (int i = 0; i < preViewDataList.Count; i++)
                {
                    PreViewItemData preViewItemData = preViewDataList[i];
                    if (preViewItemData == null)
                    {
                        continue;
                    }

                    ItemData itemData = ItemDataManager.CreateItemDataFromTable(preViewItemData.itemId);
                    if (itemData == null)
                    {
                        continue;
                    }

                    if (itemData.TableData == null)
                    {
                        continue;
                    }

                    long uid = HashToLong(itemData.TableData.ResID, itemData.TableData.EquipPropID);

                    if (uid == 0) // 既没有资源id，也没有属性id 呵呵
                    {
                        uid = id++;
                    }

                    if (!resID2ItemData.ContainsKey(uid))
                    {
                        resID2ItemData[uid] = new List<PreViewItemData>();
                    }

                    resID2ItemData[uid].Add(preViewItemData);
                }

                preViewDataList.Clear();

                var iter = resID2ItemData.GetEnumerator();
                while (iter.MoveNext())
                {
                    List<PreViewItemData> adt = iter.Current.Value as List<PreViewItemData>;
                    if (adt == null)
                    {
                        continue;
                    }

                    int iIndex = -1;
                    int iTimeLeft = 0;
                    PreViewItemData preViewItemDataTemp = null;
                    for (int i = 0; i < adt.Count; i++)
                    {
                        PreViewItemData preViewItemData = adt[i];
                        if (preViewItemData == null)
                        {
                            continue;
                        }

                        ItemData itemData = ItemDataManager.CreateItemDataFromTable(preViewItemData.itemId);
                        if (itemData == null)
                        {
                            continue;
                        }

                        if (itemData.TableData == null)
                        {
                            continue;
                        }

                        if (itemData.TableData.TimeLeft == 0)
                        {
                            iIndex = i;
                            break;
                        }

                        if (itemData.TableData.TimeLeft > iTimeLeft)
                        {
                            iIndex = i;
                        }
                    }

                    if (iIndex >= 0 && iIndex < adt.Count)
                    {
                        preViewItemDataTemp = adt[iIndex];
                        preViewDataList.Add(preViewItemDataTemp);
                    }
                }
            }

            PreViewDataModel preViewDataModel = new PreViewDataModel();
            if (preViewDataModel == null)
            {
                return;
            }

            preViewDataModel.isCreatItem = true;
            preViewDataModel.preViewItemList = preViewDataList;

            ClientSystemManager.GetInstance().OpenFrame<PreviewModelFrame>(FrameLayer.Middle, preViewDataModel);
        }

        void _UpdateMagicJarGoods(JarData a_data)
        {
            for (int i = 0; i < a_data.arrBuyInfos.Count; ++i)
            {
                GameObject objBuy = m_objMagicBuyRoot.transform.GetChild(i).gameObject;
                objBuy.SetActive(true);
                JarBuyInfo buyInfo = a_data.arrBuyInfos[i];

                Utility.GetComponetInChild<Text>(objBuy, "Time").text = TR.Value("magicjar_buy_times", buyInfo.nBuyCount);
                Text labCount = Utility.GetComponetInChild<Text>(objBuy, "Price/Count");
                Image imgIcon = Utility.GetComponetInChild<Image>(objBuy, "Price/Icon");
                for (int j = 0; j < buyInfo.arrCosts.Count; ++j)
                {
                    JarBuyCost cost = buyInfo.arrCosts[j];
                    int nCount = cost.GetRealCostCount(buyInfo.nBuyCount);
                    if (nCount <= ItemDataManager.GetInstance().GetOwnedItemCount((int)cost.item.TableID))
                    {
                        labCount.text = nCount.ToString();
                        // imgIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);
                        break;
                    }
                    else
                    {
                        if (j == buyInfo.arrCosts.Count - 1)
                        {
                            labCount.text = TR.Value("color_red", nCount);
                            // imgIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);
                        }
                    }
                }

                // 免费次数，时间
                _SetupFreeCD(objBuy, buyInfo);

                if (buyInfo.arrCosts.Count > 0)
                {
                    m_comMagicTicket.SetData(ComCommonConsume.eType.Item, ComCommonConsume.eCountType.Fatigue, (int)buyInfo.arrCosts[0].item.TableID);
                }
            }
        }

        void _UpdateFreeCD(JarData a_data, float timeElapsed)
        {
            if (m_fUpdateTime <= 0)
            {
                return;
            }

            m_fUpdateTime -= timeElapsed;

            if (m_fUpdateTime <= 0)
            {
                for (int i = 0; i < a_data.arrBuyInfos.Count; ++i)
                {
                    JarBuyInfo buyInfo = a_data.arrBuyInfos[i];
                    if (buyInfo != null && buyInfo.nMaxFreeCount > 0)
                    {
                        _SetupFreeCD(m_objMagicBuyRoot.transform.GetChild(i).gameObject, buyInfo);
                    }
                }
            }
        }

        void _SetupFreeCD(GameObject a_objBuy, JarBuyInfo a_buyInfo)
        {
            GameObject objCost = Utility.FindGameObject(a_objBuy, "Price");
            GameObject objMagicFree = Utility.FindGameObject(a_objBuy, "Free", false);
            GameObject objFreeCD = Utility.FindGameObject(a_objBuy, "FreeCD", false);
            if (a_buyInfo.nMaxFreeCount > 0)
            {
                if (objMagicFree != null)
                {
                    objMagicFree.SetActive(a_buyInfo.nFreeCount > 0);

                    Text labFree = objMagicFree.GetComponent<Text>();
                    labFree.text = TR.Value("jar_free", a_buyInfo.nFreeCount, a_buyInfo.nMaxFreeCount);
                }
                objCost.gameObject.SetActive(a_buyInfo.nFreeCount <= 0);

                if (objFreeCD != null)
                {
                    if (a_buyInfo.nFreeCount < a_buyInfo.nMaxFreeCount)
                    {
                        objFreeCD.SetActive(true);
                        objFreeCD.GetComponent<Text>().text = _GetFreeTimeCDDesc(a_buyInfo.nFreeTimestamp);
                        m_fUpdateTime = 1.0f;
                    }
                    else
                    {
                        objFreeCD.SetActive(false);
                    }
                }
            }
            else
            {
                if (objMagicFree != null)
                {
                    objMagicFree.SetActive(false);
                }
                if (objFreeCD != null)
                {
                    objFreeCD.SetActive(false);
                }
                objCost.gameObject.SetActive(true);
            }
        }

        string _GetFreeTimeCDDesc(int a_timeStamp)
        {
            int nTimeLeft = a_timeStamp - (int)TimeManager.GetInstance().GetServerTime();
            if (nTimeLeft < 0)
            {
                nTimeLeft = 0;
            }

            int second = 0;
            int minute = 0;
            int hour = 0;
            second = nTimeLeft % 60;
            int temp = nTimeLeft / 60;
            if (temp > 0)
            {
                minute = temp % 60;
                hour = temp / 60;
            }

            return TR.Value("jar_free_cd", hour, minute, second);
        }

        //[UIEventHandle("Content/Groups/MagicGroup/Page/Right/Title/Shop")]
        void _OnShopClicked()
        {
            //ShopMainFrame.OpenLinkFrame("7|-1|7|2");

            //打开积分商店
            ShopNewDataManager.GetInstance().OpenShopNewFrame(7);
        }
        #endregion
    }
}
