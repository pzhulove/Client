using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public sealed class BossKillActivityView : MonoBehaviour, IDisposable
    {
        [SerializeField] private RectTransform mItemRoot = null;
        [SerializeField] private ActivityNote mNote;
        [SerializeField] private Button mButtonGoShop;

        private readonly Dictionary<uint,BossKillItem> mItems = new Dictionary<uint, BossKillItem>();
        private ActivityItemBase.OnActivityItemClick<int> mOnItemClick;
        private UnityAction mOnGoShopClick;

        public void Init(BossKillModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick, UnityAction onGoShopClick)
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

        }

        public void Close()
        {
            Dispose();
            Destroy(gameObject);
        }

        public void UpdateData(BossKillModel data)
        {
            GameObject go = null;

            if (data.MonsterDatas == null)
            {
                return;
            }

            for (int i = 0; i < data.MonsterDatas.Count; ++i)
            {
                if (mItems.ContainsKey(data.MonsterDatas[i].Id))
                {
                    mItems[data.MonsterDatas[i].Id].UpdateData(data.MonsterDatas[i]);
                }
                else
                {
                    if (go == null)
                    {
                        go = AssetLoader.GetInstance().LoadResAsGameObject(data.ItemPath);
                    }

                    _AddItem(go, i, data.MonsterDatas[i]);
                }
            }

            //遍历删除多余的数据
            List<uint> dataIdList = new List<uint>(mItems.Keys);
            for (int i = 0; i < dataIdList.Count; ++i)
            {
                bool isHave = false;
                for (int j = 0; j < data.MonsterDatas.Count; ++j)
                {
                    if (dataIdList[i] == data.MonsterDatas[j].Id)
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

        void InitItems(BossKillModel model)
	    {
	        if (model.MonsterDatas == null || model.MonsterDatas.Count <= 0)
	        {
	            return;
	        }

	        GameObject go = AssetLoader.GetInstance().LoadResAsGameObject(model.ItemPath);
	        if (go == null)
	        {
	            Logger.LogError("加载预制体失败，路径:" + model.ItemPath);
	            return;
	        }

	        if (go.GetComponent<BossKillItem>() == null)
	        {
	            GameObject.Destroy(go);
	            Logger.LogError("预制体上找不到BossKillItem的脚本，预制体路径是:" + model.ItemPath);
	            return;
	        }

            this.mItems.Clear();

            for (int i = 0; i < model.MonsterDatas.Count; ++i)
            {
                _AddItem(go, i, model.MonsterDatas[i]);
            }

	        GameObject.Destroy(go);
        }

        void _AddItem(GameObject go, int id, BossKillMonsterModel data)
        {
            if (go == null)
            {
                return;
            }

            GameObject item = GameObject.Instantiate(go);
            item.transform.SetParent(mItemRoot, false);
            var script = item.GetComponent<BossKillItem>();
            if (script != null)
            {
                script.Init(id, data, mOnItemClick);
                mItems.Add(data.Id, script);
            }
        }

        public void Dispose()
        {
            if (mNote != null)
                mNote.Dispose();
            mButtonGoShop.SafeRemoveOnClickListener(mOnGoShopClick);
        }
    }
}
