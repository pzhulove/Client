using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
using UnityEngine.UI;
using System.ComponentModel;
using System;
using Network;
///////删除linq
using ProtoTable;
using Protocol;

namespace GameClient
{
    public class VIVOPrivilegeFrame : ClientFrame
    {

        public enum VIVOTABType
        {
            [System.ComponentModel.DescriptionAttribute("启动特权")]
            OTT_PRIVILRGR = 0,
            OTT_COUNT,
        }
        VIVOTABType m_eFunctionType = VIVOTABType.OTT_PRIVILRGR;

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/VIVOPrivilegeFrame";
        }
        protected sealed override void _OnOpenFrame()
        {
            _InitOPPOTABTabs();
            ActiveManager.GetInstance().onActivityUpdate += _OnActivityUpdate;//让活动刷新的回调
            GameStatisticManager.GetInstance().DoStartVIVO(StartVIVOType.VIVOOPEN);
        }
        protected sealed override void _OnCloseFrame()
        {
            for (int i = 0; i < m_kFunctionObject.Length; i++)
            {
                m_kFunctionObject[i].Clear();
            }
            Array.Clear(m_acPrefabInits, 0, m_acPrefabInits.Length);
            m_akOPPOTABTabs.DestroyAllObjects();
            
            myprivilegeActivityList.Clear();
            ActiveManager.GetInstance().onActivityUpdate -= _OnActivityUpdate;//让活动刷新的回调
        }
        protected sealed override void _OnUpdate(float timeElapsed)
        {
            fTime += timeElapsed;

            if (fTime > 1)
            {
                IsUpdate = false;
                RefreshPriviPickUpBtn();
            }
        }
        public sealed override bool IsNeedUpdate()
        {
            return IsUpdate;
        }

        void _InitOPPOTABTabs()
        {
            m_eFunctionType = VIVOTABType.OTT_PRIVILRGR;
            GameObject goParent = Utility.FindChild(frame, "VerticalFilter");
            GameObject goPrefab = Utility.FindChild(goParent, "Filter");
            goPrefab.CustomActive(false);

            var OnFunctionLoad = Delegate.CreateDelegate(typeof(VIVOTABTab.OnFunctionLoad), this, "_OnFunctionLoad");
            for (int i = 0; i < (int)VIVOTABType.OTT_COUNT; ++i)
            {
                m_akOPPOTABTabs.Create((VIVOTABType)i, new object[] { goParent, goPrefab, i, this, OnFunctionLoad });
                m_kFunctionObject[i].Clear();

                m_aInits[i] = false;
            }

            m_akOPPOTABTabs.Filter(null);

            m_akOPPOTABTabs.GetObject(m_eFunctionType).toggle.isOn = true;

        }

        const int m_iConfigPrefabCount = 1;
        string[] m_Prefabs = new string[m_iConfigPrefabCount]
        {
            "UIFlatten/Prefabs/Activity/VIVOPrivilege", //0
        };

        char[] m_acPrefabInits = new char[m_iConfigPrefabCount];

        List<GameObject>[] m_kFunctionObject = new List<GameObject>[(int)VIVOTABType.OTT_COUNT]
        {
            new List<GameObject>(),
        };

        bool[] m_aInits = new bool[(int)VIVOTABType.OTT_COUNT];

        [UIObject("ChildFrame")]
        GameObject goChildFrame;

        void _OnFunctionLoad(VIVOTABType eVIVOTABType)
        {
            switch (eVIVOTABType)
            {

                case VIVOTABType.OTT_PRIVILRGR:
                    {
                        if (0 == m_acPrefabInits[0])
                        {
                            GameObject goPrivilege = AssetLoader.instance.LoadRes(m_Prefabs[0], typeof(GameObject)).obj as GameObject;
                            goPrivilege.name = "Privilege";
                            Utility.AttachTo(goPrivilege, goChildFrame);

                            m_kFunctionObject[(int)VIVOTABType.OTT_PRIVILRGR].Add(goPrivilege);
                            m_acPrefabInits[0] = (char)1;
                        }
                    }

                    break;
            }
        }

        class VIVOTABTab : CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            VIVOTABType eVIVOTABType;
            VIVOPrivilegeFrame frame;

            public VIVOTABType VIVOTABType
            {
                get { return eVIVOTABType; }
            }

            public Toggle toggle;
            Text labelMark;
            Text labelCheckMark;
            public delegate void OnFunctionLoad(VIVOTABType eOPPOTABType);
            public OnFunctionLoad onFunctionLoad;
            OPPOFunctionRedBinder functionRedBinder=null;

            public sealed override void OnDestroy()
            {
                goLocal = null;
                goPrefab = null;
                goParent = null;
                frame = null;
                labelMark = null;
                labelCheckMark = null;
                toggle.onValueChanged.RemoveAllListeners();
                toggle = null;
                onFunctionLoad = null;
                functionRedBinder = null;
            }

            public sealed override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                eVIVOTABType = (VIVOTABType)param[2];
                frame = (VIVOPrivilegeFrame)param[3];
                this.onFunctionLoad = param[4] as OnFunctionLoad;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    toggle = goLocal.GetComponent<Toggle>();
                    toggle.onValueChanged.AddListener((bool bValue) =>
                    {
                        if (bValue)
                        {
                            _OnSelected();
                        }
                    });

                    var path = Utility.GetEnumDescription(eVIVOTABType);
                    labelMark = Utility.FindComponent<Text>(goLocal, "Text");
                    labelMark.text = path;
                    labelCheckMark = Utility.FindComponent<Text>(goLocal, "CheckMark/Text");
                    labelCheckMark.text = path;
                    functionRedBinder = goLocal.GetComponent<OPPOFunctionRedBinder>();
                }
                Enable();
                _Update();
            }

            public sealed override void OnRecycle()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }

            public sealed override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }

            public sealed override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }
            public sealed override void OnRefresh(object[] param)
            {
                _Update();
            }

            public sealed override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }

            void _Update()
            {
                if (eVIVOTABType== VIVOTABType.OTT_PRIVILRGR)
                {
                    functionRedBinder.AddCheckFunction(OPPOFunctionRedBinder.OPPOFunctionType.OFT_PRIVILRGR);
                }
                else
                {
                    functionRedBinder.ClearCheckFunctions();
                }
            }

            void _OnSelected()
            {
                if (null != onFunctionLoad)
                {
                    onFunctionLoad(eVIVOTABType);
                    onFunctionLoad = null;
                }
                frame.OnFunctionChanged(eVIVOTABType);
            }
        }

        CachedObjectDicManager<VIVOTABType, VIVOTABTab> m_akOPPOTABTabs = new CachedObjectDicManager<VIVOTABType, VIVOTABTab>();
        
        void OnFunctionChanged(VIVOTABType eVIVOTABType)
        {
            m_eFunctionType = eVIVOTABType;
            for (int i = 0; i < m_kFunctionObject.Length; ++i)
            {
                if (m_eFunctionType != (VIVOTABType)i)
                {
                    for (int j = 0; j < m_kFunctionObject[i].Count; j++)
                    {
                        m_kFunctionObject[i][j].CustomActive(false);
                    }
                }
            }

            for (int i = 0; i < m_kFunctionObject[(int)eVIVOTABType].Count; i++)
            {
                m_kFunctionObject[(int)eVIVOTABType][i].CustomActive(true);
            }
            if (!m_aInits[(int)m_eFunctionType])
            {
               
                if (m_eFunctionType == VIVOTABType.OTT_PRIVILRGR)
                {
                    m_comRewardItemList = Utility.FindComponent<ComUIListScript>(frame, "ChildFrame/Privilege/Items");
                    gray = Utility.FindComponent<UIGray>(frame, "ChildFrame/Privilege/OK");
                    OKText = Utility.FindComponent<Text>(frame, "ChildFrame/Privilege/OK/Text");
                    _InitRewardItemList();
                    GetPriviItemData();
                    _AddButton("ChildFrame/Privilege/OK", OnOKButtonClick);
                }
            }
        }
        
        #region  Privilege
        public const int privilegeRwerdID = 23001;
        public const int privilegeID =23000;
        bool IsUpdate = false;
        float fTime = 0.0f;
        ComUIListScript m_comRewardItemList;
        UIGray gray;
        Text OKText;
        List<ActiveManager.ActivityData> myprivilegeActivityList = new List<ActiveManager.ActivityData>();

        void RefreshPriviPickUpBtn()
        {
            if (!_CheckPrivilrge())
            {
                return;
            }
            IsStartGameFromCenter();
        }
        public bool _CheckPrivilrge()
        {
            int index = 0;
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(privilegeID);

            if (activeData == null)
            {
                return false;
            }
            for (int i = 0; i < activeData.akChildItems.Count; i++)
            {
                if (activeData.akChildItems[i].status == (int)TaskStatus.TASK_FINISHED)
                {
                    index++;
                }
            }
            if (index > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        void IsStartGameFromCenter()
        {
            if (SDKInterface.Instance.IsStartFromGameCenter())
            {
                gray.enabled = false;
                OKText.text = "领取";
            }
            else
            {
                gray.enabled = false;
                OKText.text = "前往游戏中心";
            }
        }
       
        void OnOKButtonClick()
        {
            if (SDKInterface.Instance.IsStartFromGameCenter())
            {
                ActiveManager.GetInstance().SendSubmitActivity(privilegeRwerdID);

                GameStatisticManager.GetInstance().DoStartVIVO(StartVIVOType.VIVOPRIVILEGE);
            }
            else
            {
                IsUpdate = true;
                SDKInterface.Instance.OpenGameCenter();
                GameStatisticManager.GetInstance().DoStartVIVO(StartVIVOType.VIVOJUMPGAMECENTER);
            }
               
        }
        void GetPriviItemData()
        {
            myprivilegeActivityList.Clear();
            ActiveManager.ActiveData activeData = ActiveManager.GetInstance().GetActiveData(privilegeID);

            if (activeData == null)
            {
                Logger.LogErrorFormat("activeData is null");
                return;
            }

            for (int i = 0; i < activeData.akChildItems.Count; i++)
            {
                myprivilegeActivityList.Add(activeData.akChildItems[i]);
            }
            UpdataPrivileState();
        }
        void UpdataPrivileState()
        {
            for (int i = 0; i < myprivilegeActivityList.Count; i++)
            {
                if (myprivilegeActivityList[i].status == (int)TaskStatus.TASK_OVER)
                {
                    gray.enabled = true;
                    OKText.text = "已领取";
                }
                else if (myprivilegeActivityList[i].status == (int)TaskStatus.TASK_FINISHED)
                {
                    IsStartGameFromCenter();

                }
                else if (myprivilegeActivityList[i].status == (int)TaskStatus.TASK_UNFINISH)
                {
                    gray.enabled = false;
                    OKText.text = "领取";
                }
            }
        }

        void _InitRewardItemList()
        {
            List<AwardItemData> items = ActiveManager.GetInstance().GetActiveAwards(privilegeRwerdID);

            if (items == null)
            {
                Logger.LogErrorFormat("PrivilegeItems is null ...");
                return;
            }

            m_comRewardItemList.Initialize();

            m_comRewardItemList.onBindItem = var =>
            {
                return CreateComItem(Utility.FindGameObject(var, "itemPos"));
            };

            m_comRewardItemList.onItemVisiable = var =>
            {
                if (items != null)
                {
                    if (var.m_index >= 0 && var.m_index < items.Count)
                    {
                        ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(items[var.m_index].ID);
                        itemData.Count = items[var.m_index].Num;

                        ComItem comItem = var.gameObjectBindScript as ComItem;
                        comItem.Setup(itemData, (var1, var2) =>
                        {
                            ItemTipManager.GetInstance().ShowTip(var2);
                        });

                        Utility.GetComponetInChild<Text>(var.gameObject, "Name").text = itemData.GetColorName();
                    }
                }
            };

            m_comRewardItemList.SetElementAmount(items.Count);

        }
        #endregion
        
        private void _OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            GetPriviItemData();
        }
        
        [UIEventHandle("close")]
        void OnCloseClick()
        {
            frameMgr.CloseFrame(this);
        }
    }
}

