using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public sealed class LimitTimeMallGiftPackActivityView : MonoBehaviour, IDisposable
    {
        public delegate void OnActivityMallItemSelectEvent<T>(int id);
        [SerializeField] private Text mNoGoodsTip = null;
        [SerializeField] private MallTipView mMallTipView;
        [SerializeField] private ToggleGroup mToggleGroup;
        [SerializeField] protected RectTransform mItemRoot = null;

        private readonly Dictionary<uint, MallGiftPackItem> mItems = new Dictionary<uint, MallGiftPackItem>();
        private ActivityItemBase.OnActivityItemClick<int> mOnItemClick;
        private LimitTimeGiftPackModel mModel;

        public void Close()
        {
            Dispose();
            Destroy(gameObject);
        }
        
        public void Init(LimitTimeGiftPackModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model.Id == 0)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }
            mModel = model;
            mOnItemClick = onItemClick;
            InitItems(model);
            UpdateTip();
        }

        public void UpdateData(LimitTimeGiftPackModel model)
        {
            GameObject go = null;

            mModel = model;
            if (model.Id == 0 || mModel.DetailDatas == null)
                return;

            for (int i = 0; i < mModel.DetailDatas.Count; ++i)
            {
                if (mItems.ContainsKey(mModel.DetailDatas[i].Id))
                {
                    mItems[mModel.DetailDatas[i].Id].UpdateData(mModel.DetailDatas[i]);
                }
                else
                {
                    if (go == null)
                    {
                        go = AssetLoader.GetInstance().LoadResAsGameObject(mModel.ItemPath);
                    }
                    _AddItem(go, i, mModel);
                }
            }

            //遍历删除多余的数据
            List<uint> dataIdList = new List<uint>(mItems.Keys);
            for (int i = 0; i < dataIdList.Count; ++i)
            {
                bool isHave = false;
                for (int j = 0; j < mModel.DetailDatas.Count; ++j)
                {
                    if (dataIdList[i] == mModel.DetailDatas[j].Id)
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

            UpdateTip();
        }

        void UpdateTip()
        {
            if (mItems.Count <= 0)
            {
                mNoGoodsTip.CustomActive(true);
                mMallTipView.CustomActive(false);
            }
            else
            {
                mNoGoodsTip.CustomActive(false);
                mMallTipView.CustomActive(true);
            }
        }

        void InitItems(LimitTimeGiftPackModel model)
	    {
	        if (model.DetailDatas == null || model.DetailDatas.Count <= 0)
	        {
	            return;
	        }

	        GameObject go = AssetLoader.GetInstance().LoadResAsGameObject(model.ItemPath);
	        if (go == null)
	        {
	            Logger.LogError("加载预制体失败，路径:" + model.ItemPath);
	            return;
	        }

	        if (go.GetComponent<MallGiftPackItem>() == null)
	        {
	            GameObject.Destroy(go);
	            Logger.LogError("预制体上找不到ICommonActivityItem的脚本，预制体路径是:" + model.ItemPath);
	            return;
	        }

            this.mItems.Clear();

            for (int index = 0; index < model.DetailDatas.Count; ++index)
            {
                _AddItem(go, index, model);
            }

	        GameObject.Destroy(go);
	    }

        void _AddItem(GameObject go, int index, LimitTimeGiftPackModel data)
        {
            if (go == null)
            {
                return;
            }

            GameObject item = GameObject.Instantiate(go);
            item.transform.SetParent(mItemRoot, false);
            var script = item.GetComponent<MallGiftPackItem>();
            if (script != null)
            {
                script.Init(index, data.DetailDatas[index], OnActivityMallItemSelect,(int)mModel.Id, mToggleGroup);
                script.SetSelect(0 == index);
                mItems.Add(data.DetailDatas[index].Id, script);
            }
        }

        //选中道具
        public void OnActivityMallItemSelect(int index)
        {
            if (mModel.DetailDatas != null && index >= 0 && index < mModel.DetailDatas.Count)
            {
                var data = mModel.DetailDatas[index];
                mMallTipView.OnInit(ActivityDataManager.GetInstance().GetGiftPackData(mModel.MallType, data.Id));
            }
        }

        public void Dispose()
        {
        }
    }
}
