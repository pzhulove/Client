using Protocol;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class VanityCustomClearanceActivityView : LimitTimeActivityViewCommon
    {
        [SerializeField]
        private string VanityAddUpCustomClearanceRewardItemPath = "UIFlatten/Prefabs/OperateActivity/YiJie/Items/VanityAddUpCustomClearanceRewardItem";
        [SerializeField]
        private GameObject mAddUpItemRoot;

     
        [SerializeField]
        private Vector3 mNormalItem2Pos2;
    
        [SerializeField]
        private Vector2 mNormalItem2Size2;
        [SerializeField]
        private GameObject mmNormalItem1Go;
        [SerializeField]
        private RectTransform mNormalItem2Rect;


        public sealed  override void UpdateData(ILimitTimeActivityModel data)
        {
            if (data.Id == 0 || data.TaskDatas == null || mItems == null)
            {
                Logger.LogError("ActivityLimitTimeData data is null");
                return;
            }
            GameObject go = null;
            GameObject addUpItemGo = null;

            for (int i = 0; i < data.TaskDatas.Count; ++i)
            {
                if (mItems.ContainsKey(data.TaskDatas[i].DataId))
                {
                    mItems[data.TaskDatas[i].DataId].UpdateData(data.TaskDatas[i]);
                }
                else
                {
                    if (go == null)
                    {
                        go = AssetLoader.GetInstance().LoadResAsGameObject(data.ItemPath);
                    }

                    if (addUpItemGo == null)
                    {
                        addUpItemGo = AssetLoader.GetInstance().LoadResAsGameObject(VanityAddUpCustomClearanceRewardItemPath);
                    }

                    if (data.TaskDatas[i].ParamNums.Count > 0 && data.TaskDatas[i].ParamNums[0] >= 1)
                    {
                        _AddItem(go, i, data);
                    }
                    else
                    {
                        _AddAddUpItem(addUpItemGo,i, data);
                    }
                }
            }

            //遍历删除多余的数据
            List<uint> dataIdList = new List<uint>(mItems.Keys);
            for (int i = 0; i < dataIdList.Count; ++i)
            {
                bool isHave = false;
                for (int j = 0; j < data.TaskDatas.Count; ++j)
                {
                    if (dataIdList[i] == data.TaskDatas[j].DataId)
                    {
                        isHave = true;
                        break;
                    }
                }

                if (!isHave)
                {
                    var item = mItems[dataIdList[i]];
                    mItems.Remove(dataIdList[i]);
                    item.Destroy();
                }
            }

            if (go != null)
            {
                Destroy(go);
            }

            if (addUpItemGo != null)
            {
                Destroy(addUpItemGo);
            }
        }
        protected sealed override void _InitItems(ILimitTimeActivityModel data)
        {
            GameObject go = AssetLoader.GetInstance().LoadResAsGameObject(data.ItemPath);
            if (go == null)
            {
                Logger.LogError("加载预制体失败，路径:" + data.ItemPath);
                return;
            }

            if (go.GetComponent<IActivityCommonItem>() == null)
            {
                Destroy(go);
                Logger.LogError("预制体上找不到ICommonActivityItem的脚本，预制体路径是:" + data.ItemPath);
                return;
            }

            GameObject mAddUpItem = AssetLoader.GetInstance().LoadResAsGameObject(VanityAddUpCustomClearanceRewardItemPath);
            if (mAddUpItem == null)
            {
                Logger.LogError("加载预制体失败，路径:" + VanityAddUpCustomClearanceRewardItemPath);
                return;
            }

            if (mAddUpItem.GetComponent<IActivityCommonItem>() == null)
            {
                Destroy(go);
                Logger.LogError("预制体上找不到ICommonActivityItem的脚本，预制体路径是:" + VanityAddUpCustomClearanceRewardItemPath);
                return;
            }

            mItems.Clear();
            bool isHaveExtraItem = false;
            for (int i = 0; i < data.TaskDatas.Count; i++)
            {
                if (data.TaskDatas[i].ParamNums.Count > 0 && data.TaskDatas[i].ParamNums[0] >= 1)
                {
                    isHaveExtraItem = true;
                    _AddItem(go, i, data);
                }
                else
                {
                    _AddAddUpItem(mAddUpItem,i, data);
                }
            }
            if(mmNormalItem1Go!=null)
            {
                mmNormalItem1Go.gameObject.SetActive(isHaveExtraItem);
                if (!isHaveExtraItem)
                {
                    mNormalItem2Rect.anchoredPosition = mNormalItem2Pos2;
                    mNormalItem2Rect.sizeDelta = mNormalItem2Size2;
                   
                }
               
            }
           
            Destroy(go);
            Destroy(mAddUpItem);
        }
        
        void _AddAddUpItem(GameObject go ,int id, ILimitTimeActivityModel data)
        {
            GameObject item = GameObject.Instantiate(go);
            item.transform.SetParent(mAddUpItemRoot.transform, false);
            item.GetComponent<IActivityCommonItem>().Init(data.TaskDatas[id].DataId, data.Id, data.TaskDatas[id], mOnItemClick);
            mItems.Add(data.TaskDatas[id].DataId, item.GetComponent<IActivityCommonItem>());
        }
    }
}
