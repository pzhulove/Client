using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    public class ComAchievementEventListener : MonoBehaviour
    {
        void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAchievementComplete, _OnAchievementComplete);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAchievementOver, _OnAchievementComplete);
        }

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAchievementComplete, _OnAchievementComplete);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAchievementOver, _OnAchievementComplete);
        }

        // Use this for initialization
        void Start()
        {
            _TriggerEvent();
        }

        void _OnAchievementOver(UIEvent uiEvent)
        {
            int iId = (int)uiEvent.Param1;
            GameClient.AchievementAwardPlayFrame.CommandOpen(new AchievementAwardPlayFrameData {  iId = iId });
        }

        void _OnAchievementComplete(UIEvent uiEvent)
        {
            _TriggerEvent();
        }

        void _TriggerEvent()
        {
//             FunctionUnLock functionUnLockData = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.AchievementG);
//             if (functionUnLockData == null)
//             {
//                 return;
//             }
// 
//             if (functionUnLockData.StartLevel > PlayerBaseData.GetInstance().Level)
//             {
//                 return;
//             }

            if (!ClientSystemManager.GetInstance().IsFrameOpen<AchievementEffectPlayFrame>())
            {
                if(MissionManager.GetInstance().HasAchievementItem())
                {
                    ClientSystemManager.GetInstance().OpenFrame<AchievementEffectPlayFrame>();
                }
            }
        }
    }
}