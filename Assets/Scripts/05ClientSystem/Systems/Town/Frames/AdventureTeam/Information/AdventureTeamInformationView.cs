using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public enum AdventureTeamMainTabType
    {
        None = -1,
        BaseInformation = 0,       //基础信息
        CharacterCollection = 1,   //角色收藏
        CharacterExpedition = 2,   //佣兵远征
        PassingBless = 3,          //传承赐福 （成长药剂）
        WeeklyTask = 4,            //周常任务
    }

    [Serializable]
    public class AdventureTeamMainTabDataModel
    {
        public AdventureTeamMainTabType mainTabType;
        public string mainTabName;
        public GameObject contentRoot;
        public UIPrefabWrapper uiPrefabInfo;
    }

    public class AdventureTeamInformationView : MonoBehaviour
    {
        [Space(10)] [HeaderAttribute("AdventureTeamInformationData")] [SerializeField]
        private List<AdventureTeamMainTabDataModel> mainTabDataModelList = new List<AdventureTeamMainTabDataModel>();

        [Space(10)] [HeaderAttribute("Control")] [SerializeField]
        private CommonTabToggleGroup tabGroup;

        [Space(10)][HeaderAttribute("State")][SerializeField]
        private StateController stateController;

        [SerializeField] private Button closeButton;
        [SerializeField] private ComAdventureTeamTabRedPointBinder comRedPointBinder;

        private AdventureTeamMainTabType _defaultSelectedType = 0;
        private Dictionary<AdventureTeamMainTabType, AdventureTeamContentBaseView> mainViewDic = null;

        private void Awake()
        {
            BindEvents();
            mainViewDic = new Dictionary<AdventureTeamMainTabType, AdventureTeamContentBaseView>();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            if (mainViewDic != null)
            {
                mainViewDic.Clear();
                mainViewDic = null;
            }
        }

        private void BindEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseFrame);
            }
        }

        private void UnBindEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(OnCloseFrame);
            }
        }

        private void OnCloseFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<AdventureTeamInformationFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<AdventureTeamInformationFrame>();
            }
        }

        private void _TabClickHandler(CommonTabData tabData)
        {
            if (null == tabData)
            {
                return;
            }
            _SetContentState((AdventureTeamMainTabType)tabData.id);
        }

        private void _SetContentState(AdventureTeamMainTabType tabType)
        {
            string tabName = string.Empty;
            if (null != mainTabDataModelList)
            {
                var mainTabData = mainTabDataModelList.Find(x => { return x.mainTabType == tabType; });
                if(null != mainTabData)
                {
                    tabName = mainTabData.mainTabName;
                    var uiPrefabWrapper = mainTabData.uiPrefabInfo;
                    var uiRoot = mainTabData.contentRoot;
                    _LoadContentView(uiPrefabWrapper, uiRoot, tabType);
                }
            }
            if (null == stateController || string.IsNullOrEmpty(tabName))
            {
                return;
            }
            stateController.Key = tabName;
        }

        private void _LoadContentView(UIPrefabWrapper uiPrefabWrapper, GameObject uiRoot, AdventureTeamMainTabType tabType)
        {
            AdventureTeamContentBaseView teamBaseView = null;
            if (null == mainViewDic)
            {
                mainViewDic = new Dictionary<AdventureTeamMainTabType, AdventureTeamContentBaseView>();
            }
            if (mainViewDic.ContainsKey(tabType))
            {
                teamBaseView = mainViewDic[tabType];
                if (teamBaseView != null)
                {
                    teamBaseView.OnEnableView();
                }
            }
            else
            {
                if (uiPrefabWrapper != null)
                {
                    var uiPrefab = uiPrefabWrapper.LoadUIPrefab();
                    if (uiPrefab != null)
                    {
                        Utility.AttachTo(uiPrefab, uiRoot);
                        teamBaseView = uiPrefab.GetComponent<AdventureTeamContentBaseView>();
                        if (teamBaseView != null)
                        {
                            teamBaseView.InitData();
                            mainViewDic.SafeAdd(tabType, teamBaseView);
                        }
                    }
                }
            }
        }

        #region PUBLIC METHODS

        public void InitView(AdventureTeamMainTabType type)
        {
            if (mainTabDataModelList == null)
                return;

            var mainTabDataModelNumber = mainTabDataModelList.Count;
            if (mainTabDataModelNumber <= 0)
                return;

            if ((int)type < 0 || (int)type >= mainTabDataModelNumber)
            {
                _defaultSelectedType = 0;
            }
            else
            {
                _defaultSelectedType = (AdventureTeamMainTabType)type;
            }

            if (tabGroup != null)
            {
                tabGroup.InitComTab(_TabClickHandler, (int)_defaultSelectedType);
            }

            //绑定页签按钮红点
            if (comRedPointBinder != null && tabGroup != null)
            {
                comRedPointBinder.InitBinder(tabGroup);
                comRedPointBinder.CheckRedPointsShowOnUIEventCome();
            }
        }

        public void SelectViewByTab(AdventureTeamMainTabType type)
        {
            if (mainTabDataModelList == null)
                return;

            var mainTabDataModelNumber = mainTabDataModelList.Count;
            if (mainTabDataModelNumber <= 0)
                return;

            int typeIndex = (int)type;

            if (typeIndex < 0 || typeIndex >= mainTabDataModelNumber)
            {
                _defaultSelectedType = 0;
            }
            else
            {
                _defaultSelectedType = (AdventureTeamMainTabType)type;
            }

            if (tabGroup != null)
            {
                var toggle = tabGroup.GetToggle(typeIndex);
                if (toggle)
                {
                    toggle.isOn = true;
                }
            }
        }        

        #endregion
    }
}