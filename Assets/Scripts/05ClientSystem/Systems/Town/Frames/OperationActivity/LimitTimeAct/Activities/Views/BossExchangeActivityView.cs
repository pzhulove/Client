using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public sealed class BossExchangeActivityView : MonoBehaviour, IDisposable
    {
        [SerializeField] private RectTransform mItemRoot = null;
        [SerializeField] private ActivityNote mNote;
        [SerializeField] private Button mButtonGoShop;

        private readonly Dictionary<int,BossExchangeItem> mItems = new Dictionary<int, BossExchangeItem>();
        private ActivityItemBase.OnActivityItemClick<int> mOnItemClick;
        private UnityAction mOnGoShopClick;

        [SerializeField]
        private int mChallengeScoreActivityId = 22870;
        [SerializeField]
        private Text mCurScoreTxt;

        private BossExchangeModel mData;
        public void Init(BossExchangeModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick, UnityAction onGoShopClick)
        {
            if (model.Id == 0)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }

            mOnItemClick = onItemClick;
            mOnGoShopClick = onGoShopClick;
            InitItems(model);
            mNote.Init(model, false);
            mButtonGoShop.SafeAddOnClickListener(mOnGoShopClick);
            mData = model;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdateIntegrationChallengeScore, _OnUpdateChallengeNum);
            if (model.Id== mChallengeScoreActivityId)
            {
                ActivityDataManager.GetInstance().RequestChallengeScore();
                mCurScoreTxt.CustomActive(true);
            }
            else
            {
                mCurScoreTxt.CustomActive(false);
            }
           

        }

        private void _OnUpdateChallengeNum(UIEvent uiEvent)
        {
            if (mData!=null&& mData.Id == mChallengeScoreActivityId)
            {
                mCurScoreTxt.SafeSetText(string.Format(TR.Value("IntegrationChallenge_CurScore"), PlayerBaseData.GetInstance().ChanllengeScore));
            }
        }

        public void UpdateData(BossExchangeModel data)
        {
            GameObject go = null;

            if (data.ExchangeTasks == null)
            {
                return;
            }

            int i = 0;
            foreach (var task in data.ExchangeTasks.Values)
            {
                if (mItems.ContainsKey(task.Id))
                {
                    mItems[task.Id].UpdateData(task);
                }
                else
                {
                    if (go == null)
                    {
                        go = AssetLoader.GetInstance().LoadResAsGameObject(data.ItemPath);
                    }

                    _AddItem(go, i, task);
                }

                i++;
            }


            //遍历删除多余的数据
            List<int> dataIdList = new List<int>(mItems.Keys);
            for (i = 0; i < dataIdList.Count; ++i)
            {
                bool isHave = false;
                foreach (var task in data.ExchangeTasks.Values)
                {
                    if (dataIdList[i] == task.Id)
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

        void InitItems(BossExchangeModel model)
	    {
	        if (model.ExchangeTasks == null || model.ExchangeTasks.Count <= 0)
	        {
	            return;
	        }

	        GameObject go = AssetLoader.GetInstance().LoadResAsGameObject(model.ItemPath);
	        if (go == null)
	        {
	            Logger.LogError("加载预制体失败，路径:" + model.ItemPath);
	            return;
	        }

	        if (go.GetComponent<BossExchangeItem>() == null)
	        {
	            GameObject.Destroy(go);
	            Logger.LogError("预制体上找不到BossExchangeItem的脚本，预制体路径是:" + model.ItemPath);
	            return;
	        }

            this.mItems.Clear();
	        int i = 0;
	        foreach (var task in model.ExchangeTasks.Values)
	        {
	            _AddItem(go, task.Id, task,model.Id==mChallengeScoreActivityId);
	            i++;
	        }

	        GameObject.Destroy(go);
        }

        void _AddItem(GameObject go, int id, BossExchangeTaskModel data,bool isChallengScoreActivity=false)
        {
            if (go == null)
            {
                return;
            }

            GameObject item = GameObject.Instantiate(go);
            item.transform.SetParent(mItemRoot, false);
            var script = item.GetComponent<BossExchangeItem>();
            if (script != null)
            {
                script.Init(id, data, mOnItemClick, isChallengScoreActivity);

                if (data.Id <= 0)
                {
                    Logger.LogErrorFormat("BossExchangeActivityView中 _AddItem函数 活动任务Id  = {0}，服务器下发数据错误 @杨文浩", data.Id);
                    return;
                }

                if (mItems.ContainsKey(data.Id) == false)
                {
                    mItems.Add(data.Id, script);
                }
                else
                {
                    Logger.LogErrorFormat("BossExchangeActivityView中 _AddItem函数 mItems.add时，活动任务 Key重复  Key = {0}", data.Id);
                }
            }
        }

        public void Dispose()
        {
            if (mNote != null)
                mNote.Dispose();
            mButtonGoShop.SafeRemoveOnClickListener(mOnGoShopClick);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdateIntegrationChallengeScore, _OnUpdateChallengeNum);
        }

        public void Close()
        {
            Dispose();
            Destroy(gameObject);
        }
    }
}
