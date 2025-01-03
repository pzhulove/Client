using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class MouseYearRedPackageView : MonoBehaviour, IActivityView
    {
        [SerializeField]
        private string mDefaultLogoPath = "UI/Image/Background/UI_Xianshihuodong_SloganBg_01";
        [SerializeField]
        private Text mTimeTxt;
        [SerializeField]
        private Image mImageLogo;
        [SerializeField]
        private Text mTextLogo;
        [SerializeField]
        private Text mTotalCashTxt;



        private readonly Dictionary<uint, IActivityCommonItem> mItems = new Dictionary<uint, IActivityCommonItem>();
        private ActivityItemBase.OnActivityItemClick<int> mOnItemClick;
        [SerializeField]
        private Transform mItemRoot;
        public void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model == null)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }
            _InitNote(model);
            
            mOnItemClick = onItemClick;
            _InitItems(model);
            _ShowTotalRechargeNum();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueUpdate);
        }

      

        public void UpdateData(ILimitTimeActivityModel data)
        {
            if (data.Id == 0 || data.TaskDatas == null || mItems == null)
            {
                Logger.LogError("ActivityLimitTimeData data is null");
                return;
            }
            GameObject go = null;

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

                    _AddItem(go, i, data);
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
        }


        private  void _InitItems(ILimitTimeActivityModel data)
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

            mItems.Clear();

            for (int i = 0; i < data.TaskDatas.Count; ++i)
            {
                var taskData = data.TaskDatas[i];
                if (taskData == null)
                {
                    continue;
                }

                _AddItem(go, i, data);
            }
            Destroy(go);
        }

        private void _AddItem(GameObject go, int id, ILimitTimeActivityModel data)
        {
            GameObject item = GameObject.Instantiate(go);
            item.transform.SetParent(mItemRoot, false);
            item.GetComponent<IActivityCommonItem>().Init(data.TaskDatas[id].DataId, data.Id, data.TaskDatas[id], mOnItemClick);
            mItems.Add(data.TaskDatas[id].DataId, item.GetComponent<IActivityCommonItem>());
        }


        private void _InitNote(ILimitTimeActivityModel model)
        {
            mTimeTxt.SafeSetText(string.Format("{0}~{1}", _TransTimeStampToStr(model.StartTime), _TransTimeStampToStr(model.EndTime)));
            if (!string.IsNullOrEmpty(model.LogoDesc))
            {
                mTextLogo.SafeSetText(model.LogoDesc);
            }
            else
            {
                mTextLogo.SafeSetText(TR.Value("activity_login_introduce"));
            }

            if (mImageLogo != null)
            {
                if (!string.IsNullOrEmpty(model.LogoPath))
                {
                    ETCImageLoader.LoadSprite(ref mImageLogo, model.LogoPath);
                }
                else
                {
                    ETCImageLoader.LoadSprite(ref mImageLogo, mDefaultLogoPath);
                }
                mImageLogo.SetNativeSize();
            }
        }


        private void _OnCountValueUpdate(UIEvent uiEvent)
        {
            string key = uiEvent.Param1 as string;
            if (key == CounterKeys.TOTAL_RECHARGENUM)
            {
                _ShowTotalRechargeNum();
            }
        }

        private void _ShowTotalRechargeNum()
        {
            int tatolCount = CountDataManager.GetInstance().GetCount(CounterKeys.TOTAL_RECHARGENUM);
            mTotalCashTxt.SafeSetText(string.Format(TR.Value("TotalRechargeNum"), tatolCount));
        }

        private string _TransTimeStampToStr(UInt32 timeStamp)
        {
            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
            return string.Format("{0}年{1}月{2}日{3:HH:mm}", dt.Year, dt.Month, dt.Day, dt);
        }


        public void Show()
        {

        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Close()
        {
            Dispose();
            Destroy(gameObject);
        }

        public void Dispose()
        {
            foreach (var item in mItems.Values)
            {
                item.Dispose();
            }

            mItems.Clear();
            mOnItemClick = null;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueUpdate);
        }
    }
}
