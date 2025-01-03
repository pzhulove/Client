using UnityEngine;
using System.Collections;

namespace GameClient
{
    class BudoNewMessageBinder : MonoBehaviour
    {
        void OnLevelChanged(int iPreLv, int iCurLv)
        {
            _UpdateStatus();
        }

        void Start()
        {
            BudoManager.GetInstance().onBudoInfoChanged += OnBudoInfoChanged;
            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityUpdate, _OnActivityUpdated);
            GlobalEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SceneChangedFinish,_OnSceneChangedFinish);
            GlobalEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SystemChanged, _OnSystemChangedFinish);

            _UpdateStatus();
        }

        void _OnSceneChangedFinish(UIEvent uiEvent)
        {
            _UpdateStatus();
        }

        void _OnSystemChangedFinish(UIEvent uiEvent)
        {
            _UpdateStatus();
        }

        void _OnActivityUpdated(UIEvent a_event)
        {
            uint nID = (uint)a_event.Param1;
            if(nID == BudoManager.ActiveID)
            {
                _UpdateStatus();
            }
        }

        void _UpdateStatus()
        {
            NewMessageNoticeManager.GetInstance().RemoveNewMessageNoticeByTag("BudoActiceAddParty");
            NewMessageNoticeManager.GetInstance().RemoveNewMessageNoticeByTag("BudoActiceAcquireAward");

            if (BudoManager.GetInstance().NeedHintAddParty)
            {
                NewMessageNoticeManager.GetInstance().AddNewMessageNoticeWhenNoExist("BudoActiceAddParty",
                    null, 
                    data => {
                        BudoManager.GetInstance().TryBeginActive();
                        NewMessageNoticeManager.GetInstance().RemoveNewMessageNotice(data);
                    });
            }
            else if(BudoManager.GetInstance().CanAcqured)
            {
                ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                if (systemTown == null)
                {
                    return;
                }

                ProtoTable.CitySceneTable TownTableData = TableManager.instance.GetTableItem<ProtoTable.CitySceneTable>(systemTown.CurrentSceneID);
                if (TownTableData == null)
                {
                    return;
                }

                if(TownTableData.SceneSubType == ProtoTable.CitySceneTable.eSceneSubType.BUDO)
                {
                    return;
                }

                NewMessageNoticeManager.GetInstance().AddNewMessageNoticeWhenNoExist("BudoActiceAcquireAward", 
                    null,
                    data =>{
                    BudoManager.GetInstance().TryBeginActive();
                    NewMessageNoticeManager.GetInstance().RemoveNewMessageNotice(data);
                    });
            }
        }

        void OnBudoInfoChanged()
        {
            _UpdateStatus();
        }

        void OnDestroy()
        {
            BudoManager.GetInstance().onBudoInfoChanged -= OnBudoInfoChanged;
            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityUpdate, _OnActivityUpdated);
            GlobalEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SceneChangedFinish,_OnSceneChangedFinish);
            GlobalEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SystemChanged, _OnSystemChangedFinish);
        }
    }
}