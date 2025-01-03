using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class ComAchievementLevelUpListener : MonoBehaviour
    {
        void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAchievementScoreChanged, _OnAchievementScoreChanged);
        }

        void _OnAchievementScoreChanged(UIEvent uiEvent)
        {
            int _pre = (int)uiEvent.Param1;
            int _current = (int)uiEvent.Param2;
            if(_current > _pre && _pre > 0)
            {
                var preItem = AchievementGroupDataManager.GetInstance().GetAchievementLevelByPoint(_pre);
                var curItem = AchievementGroupDataManager.GetInstance().GetAchievementLevelByPoint(_current);
                if(null != preItem && null != curItem && preItem.ID != curItem.ID && curItem.Level > preItem.Level)
                {
                    AchievementLevelUpPlayFrame.CommandOpen(new AchievementLevelUpPlayFrameData { iId = curItem.ID });
                }
            }
        }

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAchievementScoreChanged, _OnAchievementScoreChanged);
        }
    }
}