using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameClient
{
    public class ComAchievementEffectPlayItem : MonoBehaviour
    {
        public Text Name;
        public Text Desc;
        public Image Icon;
        public Image pointIcon = null;
        public string PointFmtString = string.Empty;
        public UnityEvent onCreate;
        public UnityEvent onRecycle;
        int _iId = -1;
        public void OnClickLink()
        {
            AchievementGroupDataManager.OnLink2FixedAchievementItemById(_iId);
        }

        public void OnCreate()
        {
            if(null != onCreate)
            {
                onCreate.Invoke();
            }
        }

        public void OnRecycle()
        {
            if (null != onRecycle)
            {
                onRecycle.Invoke();
            }
        }

        public void SetValue(int iId)
        {
            _iId = iId;
            var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(iId);
            if(null != missionItem)
            {
                if(null != Name)
                {
                    Name.text = missionItem.TaskName;
                }             
            }
            var achievementItem = TableManager.GetInstance().GetTableItem<ProtoTable.AchievementGroupSubItemTable>(iId);
            if(null != achievementItem)
            {
                if(null != Desc)
                {
                    Desc.text = achievementItem.Desc;
                }
         
                Icon.SafeSetImage(achievementItem.Icon, true);
                pointIcon.SafeSetImage(achievementItem.PointIcon, true);
            }
        }
    }
}