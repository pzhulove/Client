using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameClient
{
    public class AnniversaryBuffPrayActivityView : MonoBehaviour, IActivityView
    {
        [SerializeField]
        private Text mTimeTxt;
        [SerializeField]
        private Text mRuleDesTxt;

        [SerializeField]
        private Transform mNormalRoot;

        [SerializeField]
        private Transform mSpecialRoot1;

        [SerializeField]
        private Transform mSpecialRoot2;

        private Dictionary<uint, ILimitTimeActivityTaskDataModel> mTaskDataDic = new Dictionary<uint, ILimitTimeActivityTaskDataModel>();
        private List<ILimitTimeActivityTaskDataModel> mTaskDataList = new List<ILimitTimeActivityTaskDataModel>();

        private List<string> taskTimeStr = new List<string>();

        protected readonly Dictionary<uint, IActivityCommonItem> mItems = new Dictionary<uint, IActivityCommonItem>();
        public void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            mTimeTxt.SafeSetText(string.Format("{0}~{1}", _TransTimeStampToStr(model.StartTime), _TransTimeStampToStr(model.EndTime)));
            //mRuleDesTxt.SafeSetText(model.RuleDesc.Replace('|', '\n'));
            mRuleDesTxt.SafeSetText(model.RuleDesc);
            _InitItems(model);
        }

      

        public void Show()
        {
            
        }

        public void UpdateData(ILimitTimeActivityModel data)
        {
            if (data.Id == 0 || data.TaskDatas == null || mItems == null)
            {
                Logger.LogError("ActivityLimitTimeData data is null");
                return;
            }
            GameObject go = null;
            for (int i = 0; i < mTaskDataList.Count; i++)
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

                        _AddItem(go, i, data, data.TaskDatas[i]);
                    }
                
               
            }
            //遍历删除多余的数据
            List<uint> dataIdList = new List<uint>(mItems.Keys);
            for (int i = 0; i < dataIdList.Count; ++i)
            {
                bool isHave = false;
                for (int j = 0; j < mTaskDataList.Count; ++j)
                {
                    if (dataIdList[i] == mTaskDataList[j].DataId)
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
        }

     
       
        public void Close()
        {
            Dispose();
            Destroy(gameObject);
        }

        public void Dispose()
        {
            mItems.Clear();
        }

        public void Hide()
        {
            gameObject.CustomActive(false);
        }

        private void _InitItems(ILimitTimeActivityModel data)
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
                Logger.LogError("预制体上找不到IActivityCommonItem的脚本，预制体路径是:" + data.ItemPath);
                return;
            }

            mItems.Clear();
            mTaskDataDic.Clear();
            mTaskDataList.Clear();
            taskTimeStr.Clear();

            for (int i = 0; i < data.TaskDatas.Count; ++i)
            {
                if(!mTaskDataDic.ContainsKey(data.TaskDatas[i].TotalNum))
                {
                    mTaskDataDic.Add(data.TaskDatas[i].TotalNum, data.TaskDatas[i]);
                    mTaskDataList.Add(data.TaskDatas[i]);
                    if (data.TaskDatas[i].ParamNums!=null&&data.TaskDatas[i].ParamNums.Count >= 2)
                    {
                        taskTimeStr.Add(_TransTimeStamToStr(data.TaskDatas[i].ParamNums[0], data.TaskDatas[i].ParamNums[1]));
                    }
                }
                else
                {
                    //这个是新春祈福活动TotalNum为8是在某个地下城加buff的。都要展示在界面上
                    if (data.TaskDatas[i].TotalNum == 8)
                    {
                        mTaskDataList.Add(data.TaskDatas[i]);
                        if (data.TaskDatas[i].ParamNums != null && data.TaskDatas[i].ParamNums.Count >= 2)
                        {
                            taskTimeStr.Add(_TransTimeStamToStr(data.TaskDatas[i].ParamNums[0], data.TaskDatas[i].ParamNums[1]));
                        }
                    }
                    else
                    {
                        //这个是周年祈福活动TotalNum重复的话把重复的时间显示在一块
                        for (int j = 0; j < mTaskDataList.Count; j++)
                        {
                            if (mTaskDataList[j].TotalNum == data.TaskDatas[i].TotalNum)//得到重复的
                            {
                                if (data.TaskDatas[i].ParamNums != null && data.TaskDatas[i].ParamNums.Count >= 2)
                                {
                                    string oldDesStr = taskTimeStr[j];
                                    string curDesStr = _TransTimeStamToStr(data.TaskDatas[i].ParamNums[0], data.TaskDatas[i].ParamNums[1]);
                                    taskTimeStr.Remove(oldDesStr);
                                    string newDes = string.Format("{0}\n{1}", oldDesStr, curDesStr);
                                    taskTimeStr.Add(newDes);
                                }
                            }
                        }
                    }
                }
                
            }
            for (int i = 0; i < mTaskDataList.Count; i++)
            {
                _AddItem(go, i, data, mTaskDataList[i]);
            }
          
            Destroy(go);
        }
        private void _AddItem(GameObject go, int id, ILimitTimeActivityModel data, ILimitTimeActivityTaskDataModel taskData)
        {
            GameObject item = GameObject.Instantiate(go);
            //这边要判断显示的位置
            Transform parent = null;
            //两周年祈福活动
            if (data.Id == 1482)
            {
                if (taskData.TotalNum == 6)//特殊Buff的活动任务
                {
                    parent = mSpecialRoot1;
                    item.transform.SetParent(parent, false);
                    item.transform.localPosition = Vector3.zero;
                }
                else if (taskData.TotalNum == 7)
                {
                    parent = mSpecialRoot2;
                    item.transform.SetParent(parent, false);
                    item.transform.localPosition = Vector3.zero;
                }
                else
                {
                    parent = mNormalRoot;
                    item.transform.SetParent(parent, false);
                }
            }//新年祈福活动
           else if (data.Id == 1584)
            {
                if (taskData.TotalNum == 5)//特殊Buff的活动任务
                {
                    parent = mSpecialRoot1;
                    item.transform.SetParent(parent, false);
                    item.transform.localPosition = Vector3.zero;
                }
                else if (taskData.TotalNum == 4)
                {
                    parent = mSpecialRoot2;
                    item.transform.SetParent(parent, false);
                    item.transform.localPosition = Vector3.zero;
                }
                else
                {
                    parent = mNormalRoot;
                    item.transform.SetParent(parent, false);
                }
            }
            
            item.GetComponent<IActivityCommonItem>().Init(mTaskDataList[id].DataId, data.Id,mTaskDataList[id], null);
            item.GetComponent<AnniversaryBuffPrayItem>().ShowTime(taskTimeStr[id]);
            if(!mItems.ContainsKey(mTaskDataList[id].DataId))
            {
                mItems.Add(mTaskDataList[id].DataId, item.GetComponent<IActivityCommonItem>());
            }
           
        }
        private string _TransTimeStampToStr(UInt32 timeStamp)
        {
            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
            return string.Format("{0}月{1}日{2:HH:mm}", dt.Month, dt.Day, dt);
        }
        private string _TransTimeStampToStrEx(UInt32 timeStamp)
        {
            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
            return string.Format("{0}.{1}", dt.Month, dt.Day);
        }
        private string _formartTimeToStr(UInt32 start, UInt32 end)
        {
            return string.Format("{0}-{1}", _TransTimeStampToStrEx(start), _TransTimeStampToStrEx(end));
        }

        private string _TransTimeStamToStr(UInt32 startTime,UInt32 endTime)
        {
            return string.Format("{0}~{1}", _TransTimeStampToStr(startTime), _TransTimeStampToStr(endTime));
        }
    }
}
