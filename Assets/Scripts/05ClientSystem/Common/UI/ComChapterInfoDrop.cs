using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using ProtoTable;

namespace GameClient
{
    public class ComChapterInfoDrop : MonoBehaviour, IChapterInfoDrops
    {
        public bool showName = true;
        public bool showLevel = true;
        public int  weith      = 0;
        public int  height     = 0;

        private const string kItemPrfabPath = "UIFlatten/Prefabs/Chapter/Normal/ChapterInfoDropItem";
        private List<ComChapterInfoDropUnit> mCache = new List<ComChapterInfoDropUnit>();

        private void _createDropUnit(int id, int mDungeonID)
        {
            GameObject go = AssetLoader.instance.LoadResAsGameObject(kItemPrfabPath);
            Utility.AttachTo(go, this.gameObject);

            //go.transform.SetAsFirstSibling();

            ComChapterInfoDropUnit unit = go.GetComponent<ComChapterInfoDropUnit>();
            if (null != unit)
            {
                unit.Load(id);
                unit.ShowName(showName);
                unit.ShowLevel(showLevel);
                unit.ShowFatigueCombustionBuffRoot(mDungeonID);
                unit.SetSize(new Vector2(weith, height));
                mCache.Add(unit);
            }
        }

        void OnDestroy()
        {
            _unload();
        }

        private void _unload()
        {
            for (int i = 0; i < mCache.Count; ++i)
            {
                mCache[i].Unload();
                GameObject.Destroy(mCache[i].gameObject);
            }

            mCache.Clear();
        }
    

        private IEnumerator _setDropListIter(IList<int> drops,int dungonID)
        {
            _unload();

            yield return Yielders.EndOfFrame;

            if (null != drops)
            {
                for (int i = 0; i < drops.Count; ++i)
                {
                    _createDropUnit(drops[i],dungonID);

                    yield return Yielders.EndOfFrame;
                }
            }

        }

        #region IChapterInfoDrops implementation
        public void SetDropList(IList<int> drops, int dungonID)
        {
            StopAllCoroutines();
            StartCoroutine(_setDropListIter(drops,dungonID));
        }

        public void UpdateDropCount(List<ComItemList.Items> drops)
        {
        }
        #endregion

#region 刷新精力燃烧
        public void RefreshFaFatigueCombustionBuff()
        {
            for (int i = 0; i < mCache.Count; i++)
            {
                mCache[i].CloseFatigueCombustionBuffRoot();
            }
        }
#endregion
    }
}
