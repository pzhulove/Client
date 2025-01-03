using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    class ComAchievementAwardItem : MonoBehaviour
    {
        public Image icon;
        public Text desc;
        public Text achievementPoint;
        public GameObject[] goParents = new GameObject[0];
        public StateController mState;
        public string[] keys = new string[0];
        AchievementLevelInfoTable data = null;
        ComItem[] comItems = null;
        public string mAchievementPointString = string.Empty;

        public void OnClickAcquired()
        {
            if (ItemDataManager.GetInstance().IsPackageFull())
            {
                SystemNotifyManager.SystemNotify(9058);
                return;
            }

            if (null != data && 0 != data.ID)
            {
                AchievementGroupDataManager.GetInstance().SendGetAward(data.ID);
                GameClient.AchievementAwardPlayFrame.CommandOpen(new AchievementAwardPlayFrameData { iId = data.ID });
            }
        }

        public int getStatus()
        {
            if(null != data && keys.Length == 3)
            {
                bool bAcquired = AchievementGroupDataManager.GetInstance().IsAchievementFinished(data.ID);
                if(bAcquired)
                {
                    return 2;
                }
                else
                {
                    int iPoint = PlayerBaseData.GetInstance().AchievementScore;
                    if (iPoint < data.Max)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            return (int)0;
        }

        public void OnItemVisible(AchievementLevelInfoTable achievementItem)
        {
            data = achievementItem;

            if(null == data)
            {
                return;
            }

            ETCImageLoader.LoadSprite(ref icon, data.Icon);

            if(null != desc)
            {
                desc.text = string.Format(data.Name,data.Max);
            }

            if(null == comItems)
            {
                comItems = new ComItem[goParents.Length];
                for(int i = 0; i < goParents.Length; ++i)
                {
                    comItems[i] = ComItemManager.Create(goParents[i]);
                }
            }

            if(data.AwardCount.Count == data.AwardID.Count)
            {
                for(int i = 0; i < comItems.Length; ++i)
                {
                    goParents[i].CustomActive(i < data.AwardID.Count);
                    if(i < data.AwardID.Count)
                    {
                        ItemData itemData = ItemDataManager.CreateItemDataFromTable(data.AwardID[i]);
                        if(null != itemData)
                        {
                            itemData.Count = data.AwardCount[i];
                        }
                        comItems[i].Setup(itemData, _OnItemClicked);
                    }
                }
            }

            int status = getStatus();
            if(status >= 0 && status < keys.Length)
            {
                if(null != mState)
                {
                    mState.Key = keys[status];
                }
            }

            if(null != achievementPoint)
            {
                achievementPoint.text = string.Format(mAchievementPointString, data.Max);
            }
        }

        void _OnItemClicked(GameObject obj, ItemData item)
        {
            if(null != item)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }
        }

        void OnDestroy()
        {
            if(null != comItems)
            {
                for(int i = 0; i < comItems.Length; ++i)
                {
                    ComItemManager.Destroy(comItems[i]);
                    comItems[i] = null;
                }
                comItems = null;
            }
        }
    }
}