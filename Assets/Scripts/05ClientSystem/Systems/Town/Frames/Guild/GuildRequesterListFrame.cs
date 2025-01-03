using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    class GuildRequesterListFrame : ClientFrame
    {
        [UIControl("Content/ScrollView")]
        ComUIListScript m_comRequesterList;

        enum EColType
        {
            Job = 0,
            Name,
            Level,

            Count,
        }
        class SortInfo
        {
            public bool bAscending = false;
            public Image imgAscending = null;
        }
        List<SortInfo> m_arrSortInfos = new List<SortInfo>();
        EColType m_eSortType = EColType.Level;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildRequesterList";
        }

        protected override void _OnOpenFrame()
        {
            if (GuildDataManager.GetInstance().HasSelfGuild() == false)
            {
                return;
            }

            _InitRequesterList();
            _InitSort();
            _RegisterUIEvent();

            GuildDataManager.GetInstance().RequestGuildRequesters();
        }

        protected override void _OnCloseFrame()
        {
            m_eSortType = EColType.Level;
            m_arrSortInfos.Clear();
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildRequestersUpdated, _OnGuildRequestersUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildProcessRequesterSuccess, _OnProcessRequesterSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildRequestersUpdated, _OnGuildRequestersUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildProcessRequesterSuccess, _OnProcessRequesterSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildHasDismissed, _OnGuildDismissed);
        }

        void _InitRequesterList()
        {
            m_comRequesterList.Initialize();

            m_comRequesterList.onItemVisiable = (item) =>
            {
                List<GuildRequesterData> arrRequesters = GuildDataManager.GetInstance().myGuild.arrRequesters;
                if (arrRequesters == null)
                {
                    Logger.LogErrorFormat("【公会申请列表】【_InitRequesterList】列表数据为空");
                    return;
                }

                if (item.m_index >= 0 && item.m_index < arrRequesters.Count)
                {
                    GameObject obj = item.gameObject;
                    GuildRequesterData data = arrRequesters[item.m_index];

                    ComCommonBind bind = obj.GetComponent<ComCommonBind>();
                    if(bind == null)
                    {
                        Logger.LogErrorFormat("公会玩家申请列表item丢失绑定组件ComCommonBind！！！");
                        return;
                    }
                    ProtoTable.JobTable jobTable = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(data.nJobID);
                    Utility.GetComponetInChild<Text>(obj, "Job/Name").text = jobTable.Name;
                    string path = "";
                    ProtoTable.ResTable resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(jobTable.Mode);
                    if (resData != null)
                    {
                        path = resData.IconPath;
                    }
                    // Utility.GetComponetInChild<Image>(obj, "Job/Face").sprite = AssetLoader.instance.LoadRes(path, typeof(Sprite)).obj as Sprite;
                    {
                        Image img = Utility.GetComponetInChild<Image>(obj, "Job/Face");
                        ETCImageLoader.LoadSprite(ref img, path);
                    }
                    Text name = Utility.GetComponetInChild<Text>(obj, "Name/Text");
                    if (data.remark != null && data.remark != "")
                    {
                        name.text = data.remark;
                    }
                    else
                    {
                        name.text = data.strName;
                    }
                    Button btnJob = bind.GetCom<Button>("Job");
                    btnJob.SafeRemoveAllListener();
                    btnJob.SafeAddOnClickListener(() => 
                    {
                        OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(data.uGUID);
                    });
                    
                    Utility.GetComponetInChild<Text>(obj, "Level/Text").text = string.Format("Lv{0}", data.nLevel);

                    //Image imgBg = Utility.GetComponetInChild<Image>(obj, "BG");
                    //imgBg.gameObject.SetActive(i % 2 != 0);

                    ulong uGUID = data.uGUID;
                    Button btnAgree = Utility.GetComponetInChild<Button>(obj, "Oper/Agree");
                    btnAgree.onClick.RemoveAllListeners();
                    btnAgree.onClick.AddListener(() =>
                    {
                        GuildDataManager.GetInstance().ProcessRequester(uGUID, true);
                    });

                    Button btnRefuse = Utility.GetComponetInChild<Button>(obj, "Oper/Refuse");
                    btnRefuse.onClick.RemoveAllListeners();
                    btnRefuse.onClick.AddListener(() =>
                    {
                        GuildDataManager.GetInstance().ProcessRequester(uGUID, false);
                    });
                }
                else
                {
                    Logger.LogErrorFormat("【公会申请列表】【_InitRequesterList】刷新索引{0}无效，当前数据范围是{1}，{2}", item.m_index, 0, arrRequesters.Count-1);
                    return;
                }
            };
        }

        void _UpdateRequesterList(bool a_bResetScrollPos = false)
        {
            if (GuildDataManager.GetInstance().myGuild.arrRequesters != null)
            {
                m_comRequesterList.SetElementAmount(GuildDataManager.GetInstance().myGuild.arrRequesters.Count);

                if (a_bResetScrollPos)
                {
                    m_comRequesterList.m_scrollRect.verticalNormalizedPosition = 1.0f;
                }
            }
            else
            {
                Logger.LogErrorFormat("【公会申请列表】【_UpdateRequesterList】列表数据为空");
            }
        }

        void _InitSort()
        {
            for (int i = 0; i < (int)EColType.Count; ++i)
            {
                m_arrSortInfos.Add(new SortInfo());
            }
            m_arrSortInfos[(int)EColType.Job].imgAscending = Utility.GetComponetInChild<Image>(frame, "Content/ScrollView/Title/Job/Sort");
            m_arrSortInfos[(int)EColType.Name].imgAscending = Utility.GetComponetInChild<Image>(frame, "Content/ScrollView/Title/Name/Sort");
            m_arrSortInfos[(int)EColType.Level].imgAscending = Utility.GetComponetInChild<Image>(frame, "Content/ScrollView/Title/Level/Sort");
            for (int i = 0; i < m_arrSortInfos.Count; ++i)
            {
                m_arrSortInfos[i].imgAscending.gameObject.SetActive(false);
            }

            SortInfo sortInfo = m_arrSortInfos[(int)m_eSortType];
            sortInfo.imgAscending.gameObject.SetActive(true);
            sortInfo.imgAscending.transform.localRotation = sortInfo.bAscending ? Quaternion.Euler(0, 0, 180) : Quaternion.Euler(0, 0, 0);
        }

        void _UpdateSort()
        {
            int nIndex = (int)m_eSortType;
            if (nIndex < 0 && nIndex >= m_arrSortInfos.Count)
            {
                return;
            }

            for (int i = 0; i < m_arrSortInfos.Count; ++i)
            {
                m_arrSortInfos[i].imgAscending.gameObject.SetActive(false);
            }
            SortInfo sortInfo = m_arrSortInfos[(int)m_eSortType];
            sortInfo.imgAscending.gameObject.SetActive(true);
            sortInfo.imgAscending.transform.localRotation = sortInfo.bAscending ? Quaternion.Euler(0, 0, 180) : Quaternion.Euler(0, 0, 0);
        }

        void _ChangeSortRule(EColType a_colType)
        {
            int nIndex = (int)a_colType;
            if (nIndex >= 0 && nIndex < m_arrSortInfos.Count)
            {
                m_eSortType = a_colType;
                m_arrSortInfos[nIndex].bAscending = !m_arrSortInfos[nIndex].bAscending;
                
                _UpdateSort();
                _SortRequesterData();
                _UpdateRequesterList();
            }
        }

        void _SortRequesterData()
        {
            int nIndex = (int)m_eSortType;
            if (nIndex >= 0 && nIndex < m_arrSortInfos.Count)
            {
                List<GuildRequesterData> arrRequesters = GuildDataManager.GetInstance().myGuild.arrRequesters;
                if (arrRequesters == null)
                {
                    Logger.LogErrorFormat("【公会申请列表】【_SortRequesterData】列表数据为空");
                    return;
                }

                int nSign = m_arrSortInfos[nIndex].bAscending ? 1 : -1;
                switch (m_eSortType)
                {
                    case EColType.Job:
                        {
                            arrRequesters.Sort((a_left, a_right) =>
                            {
                                return (a_left.nJobID - a_right.nJobID) * nSign;
                            });
                            break;
                        }
                    case EColType.Name:
                        {
                            arrRequesters.Sort((a_left, a_right) =>
                            {
                                return string.Compare(a_left.strName, a_right.strName) * nSign;
                            });
                            break;
                        }
                    case EColType.Level:
                        {
                            arrRequesters.Sort((a_left, a_right) =>
                            {
                                return (a_left.nLevel - a_right.nLevel) * nSign;
                            });
                            break;
                        }
                }
            }
        }

        void _OnGuildDismissed(UIEvent a_event)
        {
            frameMgr.CloseFrame(this);
        }

        void _OnGuildRequestersUpdate(UIEvent a_event)
        {
            _SortRequesterData();
            _UpdateRequesterList(true);
        }

        void _OnProcessRequesterSuccess(UIEvent a_event)
        {
            _UpdateRequesterList();
        }

        [UIEventHandle("Content/Refresh")]
        void _OnRefreshClicked()
        {
            GuildDataManager.GetInstance().RequestGuildRequesters();
        }

        [UIEventHandle("Content/Clear")]
        void _OnClearClicked()
        {
            GuildDataManager.GetInstance().ProcessRequester(0, false);
        }

        [UIEventHandle("Content/Close")]
        void _OnCloseClicked()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Content/ScrollView/Title/Job")]
        void _OnTitleJobClicked()
        {
            _ChangeSortRule(EColType.Job);
        }

        [UIEventHandle("Content/ScrollView/Title/Name")]
        void _OnTitleNameClicked()
        {
            _ChangeSortRule(EColType.Name);
        }

        [UIEventHandle("Content/ScrollView/Title/Level")]
        void _OnTitleLevelClicked()
        {
            _ChangeSortRule(EColType.Level);
        }
    }
}
