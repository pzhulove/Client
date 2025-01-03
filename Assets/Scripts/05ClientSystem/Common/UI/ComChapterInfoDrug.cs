using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;

namespace GameClient
{
    public class ComChapterInfoDrug : MonoBehaviour, IChapterInfoDrugs
    {
        public bool mWithSplit = false;

        public int[] mDrugs = new int[0];

        private const string kPrefabUnit      = "UIFlatten/Prefabs/Chapter/Normal/ChapterInfoDrug";
        private const string kPrefabUnitSplit = "UIFlatten/Prefabs/Chapter/Normal/ChapterInfoDrugSplit";

        private List<ComChapterInfoDrugUnit> mCache = new List<ComChapterInfoDrugUnit>();
        private List<GameObject> mCacheSplit = new List<GameObject>();


        private void _loadUnit(int id)
        {
            ItemTable item = TableManager.instance.GetTableItem<ItemTable>(id);
            if (null != item)
            {
                GameObject go = CGameObjectPool.instance.GetGameObject(kPrefabUnit, enResourceType.UIPrefab, (uint)GameObjectPoolFlag.None);

                go.name = string.Format("{0}", id);

                Utility.AttachTo(go, this.gameObject);

                ComChapterInfoDrugUnit unit = go.GetComponent<ComChapterInfoDrugUnit>();
                unit.LoadUnit(id);
                mCache.Add(unit);
            }
        }

        private void _loadUnitSplit()
        {
            GameObject go = AssetLoader.instance.LoadResAsGameObject(kPrefabUnitSplit);
            Utility.AttachTo(go, this.gameObject);
            mCacheSplit.Add(go);
        }


        private void _unloadUnit()
        {
            for (int i = 0; i < mCache.Count; ++i)
            {
                if (null != mCache[i])
                {
                    mCache[i].UnloadUnit();
                    CGameObjectPool.instance.RecycleGameObject(mCache[i].gameObject);
                }
            }

            mCache.Clear();

            for (int i = 0; i < mCacheSplit.Count; ++i)
            {
                if (null != mCacheSplit[i])
                {
                    GameObject.Destroy(mCacheSplit[i]);
                    mCacheSplit[i] = null;
                }
            }
            mCacheSplit.Clear();
        }

#region events
        private void _bindEvents()
        {
            /// show the count changed
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemUseSuccess,     _onUpdateItems);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemTakeSuccess,    _onUpdateItems);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemUseSuccess,     _onUpdateItems);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemSellSuccess,    _onUpdateItems);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemNotifyRemoved,        _onUpdateItems);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemNotifyGet,      _onUpdateItems);

            // show the free changed
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.LevelChanged,       _onLevelChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BuffListChanged,    _onLevelChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BuffAdded,          _onLevelChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BuffRemoved,        _onLevelChanged);
        }

        private void _unbindEvents()
        { 
            /// show the count changed
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemUseSuccess,   _onUpdateItems);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemTakeSuccess,  _onUpdateItems);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemUseSuccess,   _onUpdateItems);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemSellSuccess,  _onUpdateItems);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemNotifyRemoved,      _onUpdateItems);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemNotifyGet,    _onUpdateItems);

            // show the free changed
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.LevelChanged,     _onLevelChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BuffListChanged,  _onLevelChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BuffAdded,        _onLevelChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BuffRemoved,      _onLevelChanged);
        }

        private void _onLevelChanged(UIEvent ui)
        {
            if (null != mCache)
            {
                for (int i = 0; i < mCache.Count; ++i)
                {
                    if (null != mCache[i])
                    {
                        mCache[i].UpdateCost();
                    }
                }
            }
        }

        private void _onUpdateItems(UIEvent ui)
        {
            if (null != mCache)
            {
                for (int i = 0; i < mCache.Count; ++i)
                {
                    if (null != mCache[i])
                    {
                        mCache[i].UpdateCount();
                    }
                }
            }
        }
#endregion

        private void _updateDrugs()
        {
            StopAllCoroutines();
            StartCoroutine(_updateDrugsIters());

            //_unloadUnit();

            //if (null != mDrugs)
            //{
            //    for (int i = 0; i < mDrugs.Length; ++i)
            //    {
            //        _loadUnit(mDrugs[i]);

            //        if (mWithSplit && i < mDrugs.Length - 1)
            //        {
            //            _loadUnitSplit();
            //        }
            //    }
            //}
        }

        private IEnumerator _updateDrugsIters()
        {
            _unloadUnit();

            yield return Yielders.EndOfFrame;

            if (null != mDrugs)
            {
                for (int i = 0; i < mDrugs.Length; ++i)
                {
                    _loadUnit(mDrugs[i]);

                    yield return Yielders.EndOfFrame;

                    if (mWithSplit && i < mDrugs.Length - 1)
                    {
                        _loadUnitSplit();
                    }
                }
            }
        }

        void Awake()
        {
            _bindEvents();
            _updateDrugs();
        }

        void OnDestroy()
        {
            _unloadUnit();
            _unbindEvents();
        }

        #region IChapterInfoDrugs implementation
        public void SetBuffDrugs(IList<int> drugs)
        {
            if (null != drugs)
            {
                mDrugs = new int[drugs.Count];
                for (int i = 0; i < drugs.Count; ++i)
                {
                    mDrugs[i] = drugs[i];
                }
                _updateDrugs();
            }
        }
        #endregion
    }
}
