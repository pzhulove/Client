using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComAdventureTeamTabRedPointBinder : MonoBehaviour
    {
        #region MODEL PARAMS

        CommonTabToggleGroup tabGroup;

        // index = 0 => key 不重复定义
        private int[,] waitUIEventWithMainTabTypeArray = new int[,]
        {
            {(int)EUIEventID.OnAdventureTeamBaseInfoFrameOpen, (int)AdventureTeamMainTabType.BaseInformation},
            {(int)EUIEventID.OnAdventureTeamLevelUp, (int)AdventureTeamMainTabType.BaseInformation},
            {(int)EUIEventID.OnAdventureTeamBlessCrystalCountChanged, (int)AdventureTeamMainTabType.BaseInformation},
            {(int)EUIEventID.OnAdventureTeamCollectionInfoChanged, (int)AdventureTeamMainTabType.CharacterCollection},
            {(int)EUIEventID.OnAdventureTeamExpeditionAwardChanged, (int)AdventureTeamMainTabType.CharacterExpedition},
            {(int)EUIEventID.OnAdventureTeamInheritBlessCountChanged, (int)AdventureTeamMainTabType.PassingBless},
            {(int)EUIEventID.OnAdventureTeamWeeklyTaskStatusChanged, (int)AdventureTeamMainTabType.WeeklyTask}
        };
        
        #endregion
        
        #region VIEW PARAMS

        
        #endregion
        
        #region PRIVATE METHODS
        
        //Unity life cycle
        void Awake()
        {
            if (waitUIEventWithMainTabTypeArray != null)
            {
                for (int i = 0; i < waitUIEventWithMainTabTypeArray.GetLength(0); i++)
                {
                    UIEventSystem.GetInstance().RegisterEventHandler((EUIEventID)waitUIEventWithMainTabTypeArray[i,0], _OnUIEventCome);
                }
            }
        }       
        
        //Unity life cycle
        void OnDestroy () 
        {
            if (waitUIEventWithMainTabTypeArray != null)
            {
                for (int i = 0; i < waitUIEventWithMainTabTypeArray.GetLength(0); i++)
                {
                    UIEventSystem.GetInstance().UnRegisterEventHandler((EUIEventID)waitUIEventWithMainTabTypeArray[i, 0], _OnUIEventCome);
                }
                waitUIEventWithMainTabTypeArray = null;
            }
        }

        private void _OnUIEventCome(UIEvent uiEvent)
        {
            if (waitUIEventWithMainTabTypeArray == null)
            {
                return;
            }

            int tabTypeKey = (int)AdventureTeamMainTabType.None;
            bool isTabRedPointShow = false;

            for (int i = 0; i < waitUIEventWithMainTabTypeArray.GetLength(0); i++)
            {
                EUIEventID eventId = (EUIEventID)waitUIEventWithMainTabTypeArray[i, 0];

                if (uiEvent == null)
                {
                    if (tabTypeKey == waitUIEventWithMainTabTypeArray[i, 1])
                    {
                        continue;
                    }
                    else
                    {
                        tabTypeKey = waitUIEventWithMainTabTypeArray[i, 1];
                    }
                }
                else if (eventId == uiEvent.EventID)
                {
                    tabTypeKey = waitUIEventWithMainTabTypeArray[i, 1];
                }
                else
                {
                    continue;
                }

                if (tabGroup != null)
                {
                    isTabRedPointShow = _CheckTabsRedPointShow((AdventureTeamMainTabType)tabTypeKey);
                    tabGroup.OnSetRedPoint(tabTypeKey, isTabRedPointShow);
                }
            }
        }

        private bool _CheckTabsRedPointShow(AdventureTeamMainTabType tabType)
        {
            switch (tabType)
            {
                case AdventureTeamMainTabType.BaseInformation:
                    return AdventureTeamDataManager.GetInstance().IsBaseInfoTabRedPointShow();
                case AdventureTeamMainTabType.CharacterCollection:
                    return AdventureTeamDataManager.GetInstance().IsCharacterCollectionTabRedPointShow();
                case AdventureTeamMainTabType.CharacterExpedition:
                    return AdventureTeamDataManager.GetInstance().IsCharacterExpeditionTabRedPointShow();
                case AdventureTeamMainTabType.PassingBless:
                    return AdventureTeamDataManager.GetInstance().IsPassingBlessTabRedPointShow();
                case AdventureTeamMainTabType.WeeklyTask:
                    return AdventureTeamDataManager.GetInstance().IsWeeklyTaskTabRedPointShow();
                default:
                    return false;
            }
        }
        
        #endregion

        #region PUBLIC　METHOD

        public void InitBinder(CommonTabToggleGroup tabGroup)
        {
            this.tabGroup = tabGroup;
        }

        public void CheckRedPointsShowOnUIEventCome()
        {
            _OnUIEventCome(null);
        }

        #endregion
    }
}