using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using ProtoTable;

namespace GameClient
{
    class ComMoneyRewardsRecordEnterance : MonoBehaviour
    {
        public UnityEngine.Events.UnityEvent onSucceed;
        public UnityEngine.Events.UnityEvent onFailed;

        void Awake()
        {
            _UpdateStage();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMoneyRewardsStatusChanged, _OnMoneyRewardsStatusChanged);
        }

        void _OnMoneyRewardsStatusChanged(UIEvent uiEvent)
        {
            _UpdateStage();
        }

        void _UpdateStage()
        {
            bool bShow = MoneyRewardsDataManager.GetInstance().Status > PremiumLeagueStatus.PLS_PRELIMINAY && bIsShow();
          
            if (bShow)
            {
                if(null != onSucceed)
                {
                    onSucceed.Invoke();
                }
            }
            else
            {
                if (null != onFailed)
                {
                    onFailed.Invoke();
                }
            }
        }

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMoneyRewardsStatusChanged, _OnMoneyRewardsStatusChanged);
        }

        bool bIsShow()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (systemTown == null)
            {
                return true;
            }

            CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
            if (scenedata == null)
            {
                return false;
            }
            
            if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.CrossPk3v3 || scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3)
            {
                return false;
            }
            
            return true;
        }
    }
}