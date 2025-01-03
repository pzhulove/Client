using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using EJarType = ProtoTable.JarBonus.eType;
using Protocol;

namespace GameClient
{

    class ActivityJarData
    {
        public int nActivityID;
    }

    public enum ActivityJarFrameType
    {
        Normal,  // 正常模式
        Artifact, // 神器罐活动模式（可以抽取折扣，此类型下会显示活动相关的UI，标题也要改变文字）
    }

    class ActivityJarFrame : ClientFrame
    {
        class ActJarInfo
        {
            public ActivityInfo activity;
            public JarData jarData;
        }

        #region OpenLinkFrame
        /// <summary>
        /// strParam Paramater
        /// 魅力时装罐: 9001
        /// 对应神奇罐: 9002，9003，9004，9005: 
        /// 表：ActivityJarTable，JarBouns 
        /// </summary>
        /// <param name="strParam"></param>
        public static void OpenLinkFrame(string strParam)
        {
            try
            {
                if(strParam == null || strParam.Length <= 0)
                {
                    ClientSystemManager.GetInstance().OpenFrame<ActivityJarFrame>(FrameLayer.Middle);
                    return;
                }

                string[] paramArray = strParam.Split('|');
                if(paramArray.Length > 0)
                {
                    int activityId = int.Parse(paramArray[0]);
                    ActivityJarData activityJarData = new ActivityJarData();
                    activityJarData.nActivityID = activityId;
                    ClientSystemManager.GetInstance().OpenFrame<ActivityJarFrame>(FrameLayer.Middle, activityJarData);
                }
                else
                {
                    ClientSystemManager.GetInstance().OpenFrame<ActivityJarFrame>(FrameLayer.Middle);
                }
            }
            catch(Exception e)
            {
                Logger.LogError("ActivityJarFrame.OpenLinkFrame : ==> " + e.ToString());
            }
        }
        #endregion

        #region Data
        [UIObject("Content/Tabs/Tab0")]
        GameObject m_objTab0;

        [UIControl("Content/Tabs/Tab0")]
        Toggle m_togTab0;

        [UIObject("Content/Tabs/Overlay0")]
        GameObject m_objOverlay0;

        [UIObject("Content/Tabs/Tab1")]
        GameObject m_objTab1;

        [UIControl("Content/Tabs/Tab1")]
        Toggle m_togTab1;

        [UIObject("Content/Tabs/Overlay1")]
        GameObject m_objOverlay1;

        [UIControl("Content/Groups/GoldGroup/Activity")]
        Image m_imgActivity;

        [UIControl("Content/Groups/GoldGroup/Activity/Time")]
        Text m_labActivityTime;

        [UIControl("Content/Groups/GoldGroup/Activity/Reset")]
        Text m_labResetTime;

        [UIControl("Content/Groups/GoldGroup/Activity/Rule")]
        Text m_labActivityRule;

        [UIControl("Content/Groups/GoldGroup/Left/Items")]
        ComUIListScript m_comItemList;

        [UIControl("Content/Groups/GoldGroup/Right/Discount/Rate")]
        Text m_labDiscountRate;

        [UIControl("Content/Groups/GoldGroup/Right/Discount/RemainCount")]
        Text m_labDiscountRemainCount;

        [UIControl("Content/BuyRecord/Content")]
        ComUIListScript m_comRecordList;

        [UIObject("Content/Groups/GoldGroup/Right/Buy")]
        GameObject m_objBuyFuncRoot;

        [UIControl("Content/Groups/GoldGroup/Left/Title/Text")]
        Text m_contentDescText;

        // 新增需求 神器罐子派对模式下不显示抽一次相关信息UI
        [UIObject("Content/Groups/GoldGroup/Right/Buy/Func")]
        GameObject m_objBuyOneFunc;

        ActJarInfo m_selectEquipActJar = null;
        List<ActJarInfo> m_arrEquipActData = new List<ActJarInfo>();

        ActJarInfo m_selectFashionActJar = null;
        List<ActJarInfo> m_arrFashionActData = new List<ActJarInfo>();

        ActJarInfo m_currActJar = null;

        WorldOpenJarRecordRes m_recordData = null;
        float m_fUpdateTime = 0.0f;

        public static ActivityJarFrameType frameType = ActivityJarFrameType.Normal;

        const string artifactJarActivityBannerPath = "UI/Image/Background/UI_Shenqiguan_Wodezhekou_Di.png:UI_Shenqiguan_Wodezhekou_Di";  

        #endregion

        #region ui_bind
        private Text title = null;
        private GameObject getDiscount = null;
        private Button btnGetDiscount = null;
        private Text Rate = null;
        private Text RemainCount = null;
        private GameObject mFunc1 = null;
        private Button mFunc10Btn = null;
        private UIGray mFunc10Gray = null;
        private Text mFunc10Text = null;
        private GameObject activityCloseTip = null;
        private GameObject GoldGroup = null;
        #endregion

        #region ClientFrame
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Jar/ActivityJar";
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnOpenFrame()
        {
            _InitUI();
            _RegisterUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            _ClearUI();
            _UnRegisterUIEvent();
        }       

        protected void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityUpdate, _OnActivityUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PlayerDataBaseUpdated, _OnUpdateJar);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.MagicJarUseSuccess, _OnUpdateJar);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JarOpenRecordUpdate, _OnUpdateOpenRecord);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ArtifactJarDataUpdate, _OnUpdateArtifactJarDiscountInfo);
        }

        protected void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityUpdate, _OnActivityUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PlayerDataBaseUpdated, _OnUpdateJar);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.MagicJarUseSuccess, _OnUpdateJar);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JarOpenRecordUpdate, _OnUpdateOpenRecord);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ArtifactJarDataUpdate, _OnUpdateArtifactJarDiscountInfo);
        }

        protected override void _bindExUI()
        {
            title = mBind.GetCom<Text>("title");
            getDiscount = mBind.GetGameObject("getDiscount");

            btnGetDiscount = mBind.GetCom<Button>("btnGetDiscount");
            btnGetDiscount.SafeRemoveAllListener();
            btnGetDiscount.SafeAddOnClickListener(() => 
            {
                ArtifactDataManager.GetInstance().SendGetDiscount();
            });

            Rate = mBind.GetCom<Text>("Rate");
            RemainCount = mBind.GetCom<Text>("RemainCount");
            mFunc1 = mBind.GetGameObject("Func1");
            mFunc10Btn = mBind.GetCom<Button>("Func10Btn");
            mFunc10Gray = mBind.GetCom<UIGray>("Func10Gray");
            mFunc10Text = mBind.GetCom<Text>("Func10Text");
            activityCloseTip = mBind.GetGameObject("activityCloseTip");
            GoldGroup = mBind.GetGameObject("GoldGroup");
        }

        protected override void _unbindExUI()
        {
            title = null;
            getDiscount = null;
            btnGetDiscount = null;
            Rate = null;
            RemainCount = null;
            mFunc1 = null;
            mFunc10Btn = null;
            mFunc10Gray = null;
            mFunc10Text = null;
            activityCloseTip = null;
            GoldGroup = null;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            _UpdateJarTime(timeElapsed);
        }
        #endregion

        #region InitUI
        void _InitUI()
        {
            _InitBuyJarRecord();
            _InitDisplayJarInfo();
            _InitMainTabs();
            _InitSelectedJarInfo();
            //_InitGetDiscountInfo();
            //_InitTitle();
            _InitArtifactJarDiscountInfo();
            _InitBg();
            _InitDiscountNum();
            _InitActivityTimeOpenInfo();
            _InitSelectJarTabsRect();
            _InitActivityTipInfo();
            _UpdateActivityJarUI();

            // 新增需求 神器罐子派对模式下不显示抽一次相关信息UI
            if(frameType == ActivityJarFrameType.Artifact)
            {
                m_objBuyOneFunc.CustomActive(false);
            }
        }

        /// <summary>
        /// 购买记录
        /// </summary>
        void _InitBuyJarRecord()
        {
            m_comRecordList.Initialize();

            m_comRecordList.onItemVisiable = var =>
            {
                if (m_recordData != null)
                {
                    if (var.m_index >= 0 && var.m_index < m_recordData.records.Length)
                    {
                        int nIdx = m_recordData.records.Length - var.m_index - 1;
                        OpenJarRecord record = m_recordData.records[nIdx];
                        ItemData item = ItemDataManager.GetInstance().GetCommonItemTableDataByID((int)record.itemId);
                        if (item != null)
                        {
                            string strItem = string.Format(" {{I 0 {0} 0}}", record.itemId);
                            var.GetComponent<LinkParse>().SetText(TR.Value("jar_record", record.name, strItem, record.num));
                            return;
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 右侧展示jar的信息
        /// </summary>
        void _InitDisplayJarInfo()
        {
            m_comItemList.Initialize();

            m_comItemList.onBindItem = var =>
            {
                return CreateComItem(Utility.FindGameObject(var, "Item"));
            };

            m_comItemList.onItemVisiable = var =>
            {
                if (m_currActJar != null)
                {
                    List<ItemSimpleData> items = m_currActJar.jarData.arrBonusItems;
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
                                
                                if (m_currActJar.jarData.eType == EJarType.EquipJar)
                                {
                                    Utility.DoStartFrameOperation("ActivityJarFrame", string.Format("{0}ArtifactTank_ItemId_{1}", m_currActJar.activity.level, var2.TableID));
                                }
                                else if (m_currActJar.jarData.eType == EJarType.FashionJar)
                                {
                                    Utility.DoStartFrameOperation("ActivityJarFrame", string.Format("GlamourFashionJar_ItemId_{0}", var2.TableID));
                                }
                            });

                            Utility.GetComponetInChild<Text>(var.gameObject, "Name").text = item.GetColorName();
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 左侧MainTabList
        /// </summary>
        void _InitMainTabs()
        {
            //初始化装备Tab，分为若干个等级(10,20,30,40,50神奇馆）
            {
                Button btnMain = Utility.GetComponetInChild<Button>(m_objTab0, "DropDownList/MainButton");
                btnMain.onClick.RemoveAllListeners();
                btnMain.onClick.AddListener(() =>
                {
                    m_objOverlay0.SetActive(!m_objOverlay0.activeSelf);
                });

                m_objOverlay0.SetActive(false);
                ComUIListScript comList = m_objOverlay0.GetComponent<ComUIListScript>();
                comList.Initialize();

                comList.onItemChageDisplay = (var,selected) => 
                {
                    ComCommonBind bind = var.GetComponent<ComCommonBind>();
                    StaticUtility.SafeSetVisible(bind, "selected", selected);
                };

                comList.onItemVisiable = (var) =>
                {
                    if (var.m_index >= 0 && var.m_index < m_arrEquipActData.Count)
                    {
                        ActJarInfo tempData = m_arrEquipActData[var.m_index];
                        Utility.GetComponetInChild<Text>(var.gameObject, "Label").text = tempData.jarData.strName; 
       
                        Button btnSelect = var.GetComponent<Button>();
                        btnSelect.onClick.RemoveAllListeners();
                        btnSelect.onClick.AddListener(() =>
                        {
                            comList.SelectElement(var.m_index);
                            _UpdateEquipmentJarInfo(m_arrEquipActData[var.m_index]);

                            Utility.DoStartFrameOperation("ActivityJarFrame", string.Format("{0}ArtifactTank", m_arrEquipActData[var.m_index].activity.level));
                        });
                    }
                };

                m_togTab0.onValueChanged.AddListener(var =>
                {
                    if (var)
                    {
                        _UpdateEquipmentJarInfo(m_selectEquipActJar, true);
                        if (m_contentDescText)
                        {
                            m_contentDescText.text = TR.Value("jar_content_desc_pink");
                        }
                    }

                    m_objOverlay0.SetActive(false);
                });
            }

            //初始化时装秀Tab
            {
                Button btnMain = Utility.GetComponetInChild<Button>(m_objTab1, "DropDownList/MainButton");
                btnMain.onClick.RemoveAllListeners();
                btnMain.onClick.AddListener(() =>
                {
                    m_objOverlay1.SetActive(!m_objOverlay1.activeSelf);
                });

                m_objOverlay1.SetActive(false);

                ComUIListScript comList = m_objOverlay1.GetComponent<ComUIListScript>();
                comList.Initialize();

                comList.onItemChageDisplay = (var, selected) =>
                {
                    ComCommonBind bind = var.GetComponent<ComCommonBind>();
                    StaticUtility.SafeSetVisible(bind, "selected", selected);
                };

                comList.onItemVisiable = (var) =>
                {
                    if (var.m_index >= 0 && var.m_index < m_arrFashionActData.Count)
                    {
                        ActJarInfo tempData = m_arrFashionActData[var.m_index];
                        Utility.GetComponetInChild<Text>(var.gameObject, "Label").text = tempData.jarData.strName;       

                        Button btnSelect = var.GetComponent<Button>();
                        btnSelect.onClick.RemoveAllListeners();
                        btnSelect.onClick.AddListener(() =>
                        {
                            comList.SelectElement(var.m_index);
                            _UpdateFashionJarInfo(m_arrFashionActData[var.m_index]);
                        });
                    }
                };

                m_togTab1.onValueChanged.AddListener(var =>
                {
                    if (var)
                    {
                        _UpdateFashionJarInfo(m_selectFashionActJar);
                        if (m_contentDescText)
                        {
                            m_contentDescText.text = TR.Value("jar_content_desc_pupple");
                        }

                        Utility.DoStartFrameOperation("ActivityJarFrame", "GlamourFashionJar");
                    }

                    m_objOverlay1.SetActive(false);
                });
            }

            _UpdateTabs(true);
        }

        /// <summary>
        /// 更新左侧MainTab的数据
        /// </summary>
        /// <param name="bIsInit"></param>
        void _UpdateTabs(bool bIsInit = false)
        {
            // 装备，时装Tab
            m_arrEquipActData.Clear();
            m_arrFashionActData.Clear();
            var iter = TableManager.GetInstance().GetTable<ProtoTable.ActivityJarTable>().GetEnumerator();
            while (iter.MoveNext())
            {
                ActivityInfo info = null;
                ActiveManager.GetInstance().allActivities.TryGetValue(iter.Current.Key, out info);
                if (info != null && info.state != 0)
                {
                    ProtoTable.ActivityJarTable table = iter.Current.Value as ProtoTable.ActivityJarTable;
                    JarData jarData = JarDataManager.GetInstance().GetJarData(table.JarID);
                    if (jarData.eType == EJarType.EquipJar)
                    {
                        ActJarInfo data = new ActJarInfo();
                        data.activity = info;
                        data.jarData = jarData;

                        // 有个坑爹的需求
                        // 60级罐子不参加 神器罐派对！！！
                        // add by qxy 2019-07-23
                        bool bAdd = true;
//                         if(frameType == ActivityJarFrameType.Artifact && info.id == 9007)
//                         {
//                             bAdd = false;
//                         }

                        if(bAdd)
                        {
                            m_arrEquipActData.Add(data);
                        }                        
                    }
                    else if (jarData.eType == EJarType.FashionJar)
                    {
                        ActJarInfo data = new ActJarInfo();
                        data.activity = info;
                        data.jarData = jarData;
                        m_arrFashionActData.Add(data);
                    }
                }
            }

            // 设置装备罐子
            if (m_arrEquipActData.Count > 0)
            {
                m_objTab0.SetActive(true);

                if (m_selectEquipActJar != null)
                {
                    bool bFound = false;
                    for (int i = 0; i < m_arrEquipActData.Count; ++i)
                    {
                        if (m_arrEquipActData[i].activity.id == m_selectEquipActJar.activity.id)
                        {
                            bFound = true;
                            break;
                        }
                    }
                    if (bFound == false)
                    {
                        m_selectEquipActJar = _GetLevelMatchAct(m_arrEquipActData);
                    }
                }
                else
                {
                    m_selectEquipActJar = _GetLevelMatchAct(m_arrEquipActData);
                }

                _UpdateEquipmentJarInfo(m_selectEquipActJar, bIsInit);

                m_objOverlay0.SetActive(false);
                m_objOverlay0.GetComponent<ComUIListScript>().SetElementAmount(m_arrEquipActData.Count);

                if(bIsInit)
                {
                    ComUIListScript comUIList = m_objOverlay0.GetComponent<ComUIListScript>();
                    if(comUIList != null && m_arrEquipActData != null && m_arrEquipActData.Count > 0)
                    {
                        comUIList.SelectElement(m_arrEquipActData.Count - 1);
                    }
                }
            }
            else
            {
                m_objTab0.SetActive(false);
            }

            // 设置时装活动罐子
            if (m_arrFashionActData.Count > 0)
            {
                m_objTab1.SetActive(true);

                if (m_selectFashionActJar != null)
                {
                    bool bFound = false;
                    for (int i = 0; i < m_arrFashionActData.Count; ++i)
                    {
                        if (m_arrFashionActData[i].activity.id == m_selectFashionActJar.activity.id)
                        {
                            bFound = true;
                            break;
                        }
                    }
                    if (bFound == false)
                    {
                        m_selectFashionActJar = _GetLevelMatchAct(m_arrFashionActData);
                    }
                }
                else
                {
                    m_selectFashionActJar = _GetLevelMatchAct(m_arrFashionActData);
                }
                _UpdateFashionJarInfo(m_selectFashionActJar);

                m_objOverlay1.SetActive(false);
                m_objOverlay1.GetComponent<ComUIListScript>().SetElementAmount(m_arrFashionActData.Count);

                if (bIsInit)
                {
                    ComUIListScript comUIList = m_objOverlay1.GetComponent<ComUIListScript>();
                    if (comUIList != null && m_arrFashionActData != null && m_arrFashionActData.Count > 0)
                    {
                        comUIList.SelectElement(m_arrFashionActData.Count - 1);
                    }
                }
            }
            else
            {
                m_objTab1.SetActive(false);
            }
        }

        void _InitArtifactJarDiscountInfo()
        {
            if(frameType != ActivityJarFrameType.Artifact)
            {
                return;
            }

            Rate.SafeSetText(TR.Value("jar_activity_rate",GetArtifactJarDiscountRate()));
            RemainCount.SafeSetText((GetArtifactJarDiscountTimes()).ToString());
            StaticUtility.SafeSetText(mBind, "leftDiscount", TR.Value("artifact_jar_discount_left_num", ActivityJarFrame.GetArtifactJarDiscountTimes()));

            return;
        }

        public static int GetArtifactJarDiscountRate()
        {
            if (ArtifactDataManager.GetInstance() == null)
            {
                return 0;
            }

            if (ArtifactDataManager.GetInstance().getArtifactDiscountData() == null)
            {
                return 0;
            }

            return ArtifactDataManager.GetInstance().getArtifactDiscountData().discountRate;
        }

        public static int GetArtifactJarDiscountTimes()
        {
            if (ArtifactDataManager.GetInstance() == null)
            {
                return 0;
            }

            if (ArtifactDataManager.GetInstance().getArtifactDiscountData() == null)
            {
                return 0;
            }

            return ArtifactDataManager.GetInstance().getArtifactDiscountData().discountEffectTimes;
        }

        public static bool IsHaveGotArtifactJarDiscount()
        {
            if (ArtifactDataManager.GetInstance() == null)
            {
                return false;
            }

            if(ArtifactDataManager.GetInstance().getArtifactDiscountData() == null)
            {
                return false;
            }

            return ArtifactDataManager.GetInstance().getArtifactDiscountData().extractDiscountStatus == ArtifactJarDiscountExtractStatus.AJDES_OVER;
        }

        void _InitGetDiscountInfo()
        {
            if(frameType == ActivityJarFrameType.Normal) // 正常模式没有折扣信息UI
            {
                getDiscount.CustomActive(false);
            }
            else if(frameType == ActivityJarFrameType.Artifact) // 神器罐活动模式
            {
                getDiscount.CustomActive(!IsHaveGotArtifactJarDiscount()); // 已经领取折扣了，隐藏UI，否则显示之
            }     
        }

        void _InitBg()
        {
            if (frameType == ActivityJarFrameType.Artifact)
            {
                StaticUtility.SafeSetVisible(mBind, "BG", false);
                StaticUtility.SafeSetVisible(mBind, "mask", false);
            }
        }

        void _InitDiscountNum()
        {
            if(frameType == ActivityJarFrameType.Artifact)
            {
                StaticUtility.SafeSetImage(mBind, "discount", string.Format(ShowArtifactJarDiscountFrame.numberStr, ActivityJarFrame.GetArtifactJarDiscountRate()));
                StaticUtility.SafeSetVisible<Image>(mBind, "discount", ActivityJarFrame.GetArtifactJarDiscountRate() > 0);
            }
        }

        void _InitActivityTimeOpenInfo()
        {
            OpActivityData data = ArtifactDataManager.GetInstance().getArtifactJarActData();
            if (data != null && frameType == ActivityJarFrameType.Artifact)
            {
                StaticUtility.SafeSetText(mBind, "activityTime", string.Format("活动时间 {0} - {1}", Function.GetOneData((int)data.startTime),Function.GetDateTime((int)data.endTime,false)));
                StaticUtility.SafeSetVisible<Text>(mBind, "activityTime", true);
            }
        }

        void _InitActivityTipInfo()
        {
            OpActivityData data = ArtifactDataManager.GetInstance().getArtifactJarActData();
            if(frameType == ActivityJarFrameType.Artifact)
            {
                StaticUtility.SafeSetVisible(mBind, "gongxiang", true);
                StaticUtility.SafeSetVisible(mBind, "activityTip", true);
                StaticUtility.SafeSetVisible<Text>(mBind, "leftDiscount", true);
                StaticUtility.SafeSetText(mBind, "leftDiscount", TR.Value("artifact_jar_discount_left_num", ActivityJarFrame.GetArtifactJarDiscountTimes()));              
            }
        }

        void _UpdateActivityJarUI()
        {
            if(frameType == ActivityJarFrameType.Artifact)
            {
                mFunc1.CustomActive(false);
            }
            else
            {
                mFunc1.CustomActive(true);
            }

            _UpdateFun10Gray();
            
        }
        void _UpdateFun10Gray()
        {
            if (frameType == ActivityJarFrameType.Artifact)
            {
                if (GetArtifactJarDiscountTimes() <= 0)
                {
                    mFunc10Btn.interactable = false;
                    mFunc10Gray.enabled = true;
                    mFunc10Text.text = TR.Value("artifact_jar_havebuy_text");
                }
                else
                {
                    mFunc10Btn.interactable = true;
                    mFunc10Gray.enabled = false;
                    mFunc10Text.text = TR.Value("artifact_jar_havenotbuy_text");
                }
            }
        }

        void MoveOffset(ref GameObject obj, int ix,int iy)
        {
            if(obj == null)
            {
                return;
            }

            RectTransform objrect = obj.GetComponent<RectTransform>();

            Vector3 endPos = new Vector3();
            endPos = objrect.localPosition;

            endPos.x += ix;
            endPos.y += iy;

            obj.transform.localPosition = endPos;
            return;
        }

        void _InitSelectJarTabsRect()
        {
            if(frameType == ActivityJarFrameType.Artifact)
            {
                m_objTab1.CustomActive(false);
                int iOffSetY = -80;
                MoveOffset(ref m_objTab0, 0, iOffSetY);
                MoveOffset(ref m_objOverlay0, 0, iOffSetY); 
            }
        }

        void _InitTitle()
        {
            string text = "";
            if(frameType == ActivityJarFrameType.Normal)
            {
                text = TR.Value("gemJar");
            }
            else if(frameType == ActivityJarFrameType.Artifact)
            {
                text = TR.Value("artifactJar");
            }

            title.SafeSetText(text);
        }

        void _InitSelectedJarInfo()
        {
            ActivityJarData actJarData = userData as ActivityJarData;
            if (actJarData != null)
            {
                ActJarInfo actJarInfo = _GetActJarInfo(m_arrEquipActData, actJarData.nActivityID);
                if (actJarInfo != null)
                {
                    m_selectEquipActJar = actJarInfo;
                    m_currActJar = m_selectEquipActJar;
                    m_togTab0.isOn = false;
                    m_togTab0.isOn = true;
                    return;
                }

                actJarInfo = _GetActJarInfo(m_arrFashionActData, actJarData.nActivityID);
                if (actJarInfo != null)
                {
                    m_selectFashionActJar = actJarInfo;
                    m_currActJar = m_selectFashionActJar;
                    m_togTab1.isOn = false;
                    m_togTab1.isOn = true;
                    return;
                }
            }

            if (m_arrEquipActData.Count > 0)
            {
                m_selectEquipActJar = _GetLevelMatchAct(m_arrEquipActData);
                m_currActJar = m_selectEquipActJar;
                m_togTab0.isOn = false;
                m_togTab0.isOn = true;
            }
            else if (m_arrFashionActData.Count > 0)
            {
                m_selectFashionActJar = _GetLevelMatchAct(m_arrFashionActData);
                m_currActJar = m_selectFashionActJar;
                m_togTab1.isOn = false;
                m_togTab1.isOn = true;
            }
        }
        #endregion

        #region ClearUI
        void _ClearUI()
        {
            m_selectEquipActJar = null;
            m_arrEquipActData.Clear();
            m_selectFashionActJar = null;
            m_arrFashionActData.Clear();

            m_currActJar = null;
            m_fUpdateTime = 0.0f;
            _ClearRecord();
        }
        #endregion

        #region UpdateJarInfo
        /// <summary>
        /// 更新装备Info
        /// </summary>
        /// <param name="a_selectActJar"></param>
        /// <param name="IsInit"></param>
        void _UpdateEquipmentJarInfo(ActJarInfo a_selectActJar, bool IsInit = false)
        {
            if(PlayerBaseData.GetInstance().Level < a_selectActJar.activity.level)
            {
                if(!IsInit)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("当前角色等级不足,无法查看详细信息,请继续提升等级~");
                }
               
                return;
            }

            m_selectEquipActJar = a_selectActJar;
            if (m_selectEquipActJar != null)
            {
                Utility.GetComponetInChild<Text>(m_objTab0, "Background/Label").text = m_selectEquipActJar.jarData.strName;
                Utility.GetComponetInChild<Text>(m_objTab0, "DropDownList/MainButton/Label").text = m_selectEquipActJar.jarData.strName;

                if (m_objTab0.GetComponent<Toggle>().isOn)
                {
                    _UpdateSelectedJarInfo(m_selectEquipActJar);
                }
            }

            m_objOverlay0.SetActive(false);
        }
       
        /// <summary>
        /// 更新时装Info
        /// </summary>
        /// <param name="a_selectActJar"></param>
        void _UpdateFashionJarInfo(ActJarInfo a_selectActJar)
        {
            m_selectFashionActJar = a_selectActJar;
            if (m_selectFashionActJar != null)
            {
                Utility.GetComponetInChild<Text>(m_objTab1, "Background/Label").text = m_selectFashionActJar.jarData.strName;
                Utility.GetComponetInChild<Text>(m_objTab1, "DropDownList/MainButton/Label").text = m_selectFashionActJar.jarData.strName;

                if (m_objTab1.GetComponent<Toggle>().isOn)
                {
                    _UpdateSelectedJarInfo(m_selectFashionActJar);
                }
            }
            
            m_objOverlay1.SetActive(false);
        }

        // 获取神器罐子活动ID(运营活动id)
        public static uint GetArtifactActivityID()
        {
            if(frameType != ActivityJarFrameType.Artifact)
            {
                return 0;
            } 
           
            if (ArtifactDataManager.GetInstance().getArtifactDiscountData() == null)
            {
                return 0;
            }

            return (uint)ArtifactDataManager.GetInstance().getArtifactDiscountData().opActId;
        }

        // 是否是活动展示阶段
        private static bool IsActivityExhibition()
        {
            return ArtifactFrame.IsArtifactJarShowActivityOpen();
        }
        public static void AdjustCostByActivityDiscount(ref int nTotalCost,int nPrice,int nBuyCount)
        {            
            int nDiscountCount = System.Math.Min(GetArtifactJarDiscountTimes(),nBuyCount); // 打折次数
            int nNoDiscountCount = nBuyCount - nDiscountCount; // 不打折次数
            if(nNoDiscountCount < 0)
            {
                nNoDiscountCount = 0;
            }

            // 额外优惠折扣
            // 10连抽有额外的9折
            float fTowDiscount = 1.0f;
            if (nBuyCount == 1)
            {
                fTowDiscount = 1.0f;
            }
            else if(nBuyCount == 10)
            {
                fTowDiscount = 0.9f;
            }

            // 总的花费为 (打折次数*单价*折扣率+不打折次数*单价) * 额外优惠折扣
            if (frameType == ActivityJarFrameType.Artifact)
            {
                nTotalCost = UnityEngine.Mathf.FloorToInt(((GetArtifactJarDiscountRate() / 10.0f) * (nDiscountCount * nPrice) + nNoDiscountCount * nPrice)* fTowDiscount);
            }            
        }

        /// <summary>
        /// 选择的装备或时装Jar更新
        /// </summary>
        /// <param name="a_actJar"></param>
        void _UpdateSelectedJarInfo(ActJarInfo a_actJar)
        {
            m_currActJar = a_actJar;

            JarDataManager.GetInstance().RequestJarBuyRecord(m_currActJar.jarData.nID);

            ProtoTable.ActivityJarTable activityJarTable = TableManager.GetInstance().GetTableItem<ProtoTable.ActivityJarTable>((int)m_currActJar.activity.id);
            ETCImageLoader.LoadSprite(ref m_imgActivity, activityJarTable.ActivityBG);           

            m_labActivityTime.text = _GetEndTimeDesc((int)m_currActJar.activity.dueTime);

            if (m_currActJar.activity.dueTime > (int)TimeManager.GetInstance().GetServerTime())
            {
                m_fUpdateTime = 1.0f;
            }
            m_labActivityRule.text = activityJarTable.ActivityRule;

            if (frameType == ActivityJarFrameType.Artifact)
            {
                StaticUtility.SafeSetVisible<Image>(mBind, "artifactJarActivity", true);
                StaticUtility.SafeSetImage(mBind, "artifactJarActivity", artifactJarActivityBannerPath);
                m_imgActivity.CustomActive(false);
                m_labActivityRule.text = "";
                m_labActivityTime.text = "";
            }

            m_comItemList.SetElementAmount(m_currActJar.jarData.arrBonusItems.Count);

            for (int i = 0; i < m_currActJar.jarData.arrBuyInfos.Count; ++i)
            {
                GameObject objBuy = m_objBuyFuncRoot.transform.GetChild(i).gameObject;
                objBuy.SetActive(true);
                JarBuyInfo buyInfo = m_currActJar.jarData.arrBuyInfos[i];

                Button btnBuy = objBuy.GetComponent<Button>();
                btnBuy.onClick.RemoveAllListeners();
                btnBuy.onClick.AddListener(() =>
                {
                    ShowItemsFrame.bSkipExplode = false;

                    if (buyInfo.nFreeCount > 0)
                    {
                        JarDataManager.GetInstance().RequestBuyJar(m_currActJar.jarData, buyInfo,GetArtifactActivityID());
                        return;
                    }

                    Assert.IsTrue(buyInfo.arrCosts.Count >= 1);
                    CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();
                    for (int j = 0; j < buyInfo.arrCosts.Count; ++j)
                    {
                        JarBuyCost cost = buyInfo.arrCosts[j];
                        int nCount = cost.GetRealCostCount(buyInfo.nBuyCount);

                        // 活动期间所有魔罐共享折扣率和次数，所以这里做下修正
                        ActivityJarFrame.AdjustCostByActivityDiscount(ref nCount, cost.item.Count, buyInfo.nBuyCount);

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

                    UnityEngine.Events.UnityAction tryBuyJar = () => 
                    {
                        CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                        {
                            JarDataManager.GetInstance().RequestBuyJar(m_currActJar.jarData, buyInfo, GetArtifactActivityID());
                        });
                    };

                    // 2019-01-05 策划说不提示次数已经用完 这里暴力处理下 万一后面又要了呢                     
                    if(false && GetArtifactJarDiscountTimes() <= 0 && !ArtifactDataManager.GetInstance().isNotNotify)
                    {
                        ClientSystemManager.GetInstance().OpenFrame<DiscountNumUseUpTipFrame>(FrameLayer.Middle, tryBuyJar);
                    }
                    else
                    {
                        tryBuyJar();
                    }

                    if (m_currActJar.jarData.eType == EJarType.EquipJar)
                    {
                        Utility.DoStartFrameOperation("ActivityJarFrame", string.Format("{0}ArtifactTank_Buy{1}", m_currActJar.activity.level, buyInfo.nBuyCount));
                    }
                    else if (m_currActJar.jarData.eType == EJarType.FashionJar)
                    {
                        Utility.DoStartFrameOperation("ActivityJarFrame", string.Format("GlamourFashionJar_Buy{0}", buyInfo.nBuyCount));
                    }
                });

                Utility.GetComponetInChild<Text>(objBuy, "Time").text = TR.Value("magicjar_buy_times", buyInfo.nBuyCount);

                Text labCount = Utility.GetComponetInChild<Text>(objBuy, "DiscountPrice/Count");
                Image imgIcon = Utility.GetComponetInChild<Image>(objBuy, "DiscountPrice/Icon");
                Text labPreCount = Utility.GetComponetInChild<Text>(objBuy, "Price/Count");
                Image imgPreIcon = Utility.GetComponetInChild<Image>(objBuy, "Price/Icon");
                for (int j = 0; j < buyInfo.arrCosts.Count; ++j)
                {
                    JarBuyCost cost = buyInfo.arrCosts[j];

                    ActivityJarBuyCost activityCost = cost as ActivityJarBuyCost;
                    if (activityCost != null)
                    {
                        m_labDiscountRate.text = TR.Value("jar_activity_rate", cost.fDiscount * 10);
                        m_labDiscountRemainCount.text = cost.nRemainDiscountTime.ToString();
                        _UpdateActivityJarUI();
                        if (frameType == ActivityJarFrameType.Artifact)
                        {
                            _InitArtifactJarDiscountInfo();
                        }

                        if (activityCost.bDisTimeReset)
                        {
                            m_labResetTime.text = TR.Value("jar_activity_discount_reset_time");
                        }
                        else
                        {
                            m_labResetTime.text = string.Empty;
                        }
                    }

                    int nCount = cost.GetRealCostCount(buyInfo.nBuyCount);

                    // 活动期间所有魔罐共享折扣率和次数，所以这里做下修正
                    ActivityJarFrame.AdjustCostByActivityDiscount(ref nCount, cost.item.Count, buyInfo.nBuyCount);


                    if (nCount <= ItemDataManager.GetInstance().GetOwnedItemCount((int)cost.item.TableID))
                    {
                        labCount.text = nCount.ToString();
                        // imgIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);

                        labPreCount.text = (cost.item.Count * buyInfo.nBuyCount).ToString();
                        // imgPreIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                        ETCImageLoader.LoadSprite(ref imgPreIcon, cost.item.Icon);
                        break;
                    }
                    else
                    {
                        if (j == buyInfo.arrCosts.Count - 1)
                        {
                            labCount.text = TR.Value("color_red", nCount);
                            // imgIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);

                            labPreCount.text = (cost.item.Count * buyInfo.nBuyCount).ToString();
                            // imgPreIcon.sprite = AssetLoader.GetInstance().LoadRes(cost.item.Icon, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref imgPreIcon, cost.item.Icon);
                        }
                    }
                }
            }
        }

        #endregion

        #region UIEventSystem
        void _OnActivityUpdate(UIEvent a_event)
        {
            uint nActivityID = (uint)(a_event.Param1);
            if (JarDataManager.GetInstance().IsActivityJar((int)nActivityID))
            {
                _UpdateTabs();
                _UpdateSelectActJar();
            }
        }

        void _OnUpdateJar(UIEvent a_event)
        {
            if (m_currActJar != null)
            {
                for (int i = 0; i < m_currActJar.jarData.arrBuyInfos.Count; ++i)
                {
                    GameObject objBuy = m_objBuyFuncRoot.transform.GetChild(i).gameObject;
                    objBuy.SetActive(true);
                    JarBuyInfo buyInfo = m_currActJar.jarData.arrBuyInfos[i];
                    Text labCount = Utility.GetComponetInChild<Text>(objBuy, "DiscountPrice/Count");
                    Image imgIcon = Utility.GetComponetInChild<Image>(objBuy, "DiscountPrice/Icon");
                    Text labPreCount = Utility.GetComponetInChild<Text>(objBuy, "Price/Count");
                    Image imgPreIcon = Utility.GetComponetInChild<Image>(objBuy, "Price/Icon");
                    for (int j = 0; j < buyInfo.arrCosts.Count; ++j)
                    {
                        JarBuyCost cost = buyInfo.arrCosts[j];

                        ActivityJarBuyCost activityCost = cost as ActivityJarBuyCost;
                        if (activityCost != null)
                        {
                            m_labDiscountRate.text = TR.Value("jar_activity_rate", cost.fDiscount * 10);
                            m_labDiscountRemainCount.text = cost.nRemainDiscountTime.ToString();
                            _UpdateActivityJarUI();
                            if (frameType == ActivityJarFrameType.Artifact)
                            {
                                _InitArtifactJarDiscountInfo();
                            }
                        }

                        int nCount = cost.GetRealCostCount(buyInfo.nBuyCount);

                        // 活动期间所有魔罐共享折扣率和次数，所以这里做下修正
                        ActivityJarFrame.AdjustCostByActivityDiscount(ref nCount, cost.item.Count, buyInfo.nBuyCount);

                        if (nCount <= ItemDataManager.GetInstance().GetOwnedItemCount((int)cost.item.TableID))
                        {
                            labCount.text = nCount.ToString();
                            ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);

                            labPreCount.text = (cost.item.Count * buyInfo.nBuyCount).ToString();
                            ETCImageLoader.LoadSprite(ref imgPreIcon, cost.item.Icon);
                            break;
                        }
                        else
                        {
                            if (j == buyInfo.arrCosts.Count - 1)
                            {
                                labCount.text = TR.Value("color_red", nCount);
                                ETCImageLoader.LoadSprite(ref imgIcon, cost.item.Icon);

                                labPreCount.text = (cost.item.Count * buyInfo.nBuyCount).ToString();
                                ETCImageLoader.LoadSprite(ref imgPreIcon, cost.item.Icon);
                            }
                        }
                    }
                }
            }
        }

        void _OnUpdateOpenRecord(UIEvent a_event)
        {
            _ClearRecord();
            m_recordData = a_event.Param1 as WorldOpenJarRecordRes;
            _UpdateRecord();
        }

        void _OnUpdateArtifactJarDiscountInfo(UIEvent a_event)
        {
            //_InitGetDiscountInfo();
            _InitArtifactJarDiscountInfo();
        }

        /// <summary>
        /// 接收到UI事件，更新当前的Jar信息
        /// </summary>
        void _UpdateSelectActJar()
        {
            if (m_currActJar != null)
            {
                ActJarInfo actJarInfo = _GetActJarInfo(m_arrEquipActData, (int)m_currActJar.activity.id);
                if (actJarInfo != null)
                {
                    m_selectEquipActJar = actJarInfo;
                    m_togTab0.isOn = false;
                    m_togTab0.isOn = true;
                    return;
                }

                actJarInfo = _GetActJarInfo(m_arrFashionActData, (int)m_currActJar.activity.id);
                if (actJarInfo != null)
                {
                    m_selectFashionActJar = actJarInfo;
                    m_togTab1.isOn = false;
                    m_togTab1.isOn = true;
                    return;
                }
            }

            // 当前活动罐子，已失效，重新选中一个活动罐子
            if (m_arrEquipActData.Count > 0)
            {
                m_selectEquipActJar = _GetLevelMatchAct(m_arrEquipActData);
                m_currActJar = m_selectEquipActJar;
                m_togTab0.isOn = false;
                m_togTab0.isOn = true;
            }
            else if (m_arrFashionActData.Count > 0)
            {
                m_selectFashionActJar = _GetLevelMatchAct(m_arrFashionActData);
                m_currActJar = m_selectFashionActJar;
                m_togTab1.isOn = false;
                m_togTab1.isOn = true;
            }
        }

        /// <summary>
        /// 接收到UI事件，更新当前的购买记录信息
        /// </summary>
        void _UpdateRecord()
        {
            if (m_recordData != null && m_currActJar != null && m_recordData.jarId == m_currActJar.jarData.nID)
            {
                m_comRecordList.SetElementAmount(m_recordData.records.Length);
                m_comRecordList.EnsureElementVisable(0);
            }
            else
            {
                m_comRecordList.SetElementAmount(0);
            }
        }

        /// <summary>
        /// 清除购买信息
        /// </summary>
        void _ClearRecord()
        {
            m_comRecordList.SetElementAmount(0);
            m_recordData = null;
        }
        #endregion

        #region ActivityTime
        //更新活动时间
        void _UpdateJarTime(float timeElapsed)
        {
            if (m_fUpdateTime <= 0)
            {
                return;
            }

            m_fUpdateTime -= timeElapsed;

            if (m_fUpdateTime <= 0)
            {
                if (m_currActJar != null)
                {
                    m_labActivityTime.text = _GetEndTimeDesc((int)m_currActJar.activity.dueTime);

                    if (m_currActJar.activity.dueTime > (int)TimeManager.GetInstance().GetServerTime())
                    {
                        m_fUpdateTime = 1.0f;
                    }
                }
                else
                {
                    m_labActivityTime.text = _GetEndTimeDesc(0);
                }
            }
        }

        string _GetEndTimeDesc(int a_timeStamp)
        {
            int nTimeLeft = a_timeStamp - (int)TimeManager.GetInstance().GetServerTime();
            if (nTimeLeft < 0)
            {
                nTimeLeft = 0;
            }

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
            return TR.Value("jar_activity_remain_time", day, hour, minute, second);
        }
        #endregion

        #region Helper
        ActJarInfo _GetLevelMatchAct(List<ActJarInfo> a_arrData)
        {
            a_arrData.Sort((var1, var2) =>
            {
                return var1.jarData.arrFilters[0] - var2.jarData.arrFilters[0];
            });

            int nIndex = 0;
            int nPlayerLevel = PlayerBaseData.GetInstance().Level;
            for (int i = 0; i < a_arrData.Count; ++i)
            {
                int nLevel = a_arrData[i].jarData.arrFilters[0];
                if (nLevel <= nPlayerLevel)
                {
                    nIndex = i;
                }
            }

            if (nIndex >= 0 && nIndex < a_arrData.Count)
            {
                return a_arrData[nIndex];
            }
            return null;
        }

        ActJarInfo _GetActJarInfo(List<ActJarInfo> a_arrData, int a_nActvityID)
        {
            for (int i = 0; i < a_arrData.Count; ++i)
            {
                if (a_arrData[i].activity.id == a_nActvityID)
                {
                    return a_arrData[i];
                }
            }
            return null;
        }
        #endregion

        [UIEventHandle("BG/Close")]
        void _OnCloseClicked()
        {
            if (frameType == ActivityJarFrameType.Artifact)
            {
                ClientSystemManager.GetInstance().CloseFrame<ArtifactFrame>();                
            }

            frameMgr.CloseFrame(this);
            return;
        }
    }
}
