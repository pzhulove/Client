using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GameClient
{
    [ExecuteAlways]
    public class ComChapterDungeonMap : MonoBehaviour 
    {
        public ComCommonBind mBind;

        public IDungeonData mData;

        private int _getIndex(IDungeonConnectData item)
        {
            int xe = 1;
            int ans = 0;
            for (int j = 0; j < 4; ++j)
            {
                if (item.GetIsConnect(j))
                {
                    ans += xe;
                }
                xe *= 2;
            }

            return ans;
        }

        private List<GameObject> mCache = new List<GameObject>();

        private void _unloadCache()
        {
            for (int i = 0; i < mCache.Count; ++i)
            {
				GameObject.Destroy(mCache[i]);
            }

            mCache.Clear();
        }

        Color mapBgColor = new Color32(255,255,255,150);
        //private GameObject _createTag(string name, Sprite sprite, GameObject root, float scale)
        //{
        //    var obj = new GameObject(name, typeof(Image));
        //    Utility.AttachTo(obj, root);

        //    var startRect = obj.GetComponent<RectTransform>();
        //    startRect.offsetMin = new Vector2(-15, -15);
        //    startRect.offsetMax = new Vector2(15, 15);

        //    startRect.anchorMax = new Vector2(0.5f, 0.5f);
        //    startRect.anchorMin = new Vector2(0.5f, 0.5f);

        //    var startImage = obj.GetComponent<Image>();
        //    startImage.sprite = sprite;
        //    startImage.SetNativeSize();
        //    startImage.GetComponent<RectTransform>().localScale = Vector3.one * scale;
        //    startImage.color = mapBgColor;
        //    return obj;
        //}

        private GameObject _createTag(string name, string spriteName, GameObject root, float scale)
        {
            var obj = new GameObject(name, typeof(Image));
            Utility.AttachTo(obj, root);

            var startRect = obj.GetComponent<RectTransform>();
            startRect.offsetMin = new Vector2(-15, -15);
            startRect.offsetMax = new Vector2(15, 15);

            startRect.anchorMax = new Vector2(0.5f, 0.5f);
            startRect.anchorMin = new Vector2(0.5f, 0.5f);

            var startImage = obj.GetComponent<Image>();
            // startImage.sprite = sprite;
            mBind.GetSprite(spriteName, ref startImage);
            startImage.SetNativeSize();
            startImage.GetComponent<RectTransform>().localScale = Vector3.one * scale;
            startImage.color = mapBgColor;
            return obj;
        }

        private IEnumerator _sedDungeonDataIter(IDungeonData data)
        {
            _unloadCache();

            yield return Yielders.EndOfFrame;

            if (null != mBind && null != data)
            {
                GridLayoutGroup gridlayout = mBind.GetCom<GridLayoutGroup>("gridlayout");
                GameObject      root       = mBind.GetGameObject("root");

                gridlayout.constraintCount = data.GetWeidth();

                int all = data.GetWeidth() * data.GetHeight();
                int len = data.GetAreaConnectListLength();

                for (int i = 0; i < all - len; ++i)
                {
                    GameObject ob = new GameObject("emp");
                    Utility.AttachTo(ob, root);
                    Image image = ob.AddComponent<Image>();
                    image.color = Color.clear;
                    mCache.Add(ob);

                    yield return Yielders.EndOfFrame;
                }


                List<IDungeonConnectData> list = new List<IDungeonConnectData>();
                for (int i = 0; i < len; ++i)
                {
                    list.Add(data.GetAreaConnectList(i));
                }

                list.Sort((x, y)=>
                {
                    return (x.GetPositionY() * data.GetWeidth() + x.GetPositionX()) - (y.GetPositionY() * data.GetWeidth() + y.GetPositionX());
                });

                yield return Yielders.EndOfFrame;

                for (int i = 0; i < list.Count; ++i)
                {
                    yield return Yielders.EndOfFrame;

                    GameObject ob = new GameObject();
                    Utility.AttachTo(ob, root);

                    mCache.Add(ob);

                    Image image = ob.AddComponent<Image>();
                    image.color = mapBgColor;

					var idx = _getIndex(list[i]);

                    // image.sprite = mBind.GetSprite(string.Format("{0}", idx));
                    mBind.GetSprite(string.Format("{0}", idx), ref image);

                    ob.name = string.Format("{0},{1}", list[i].GetPositionX(), list[i].GetPositionY());

                    ob.transform.SetSiblingIndex((list[i].GetPositionY()) * data.GetWeidth() + list[i].GetPositionX() + 1);

                    //if (list[i].ishell)
                    //{
                    //    _createTag("hell", mBind.GetSprite("hell"), ob, 0.5f);
                    //}
                    if (list[i].IsBoss())
                    {
                        // _createTag("boss", mBind.GetSprite("boss"), ob, 0.5f);
                        _createTag("boss", "boss", ob, 0.5f);
                    }
                    else if (list[i].IsStart())
                    {
                        // _createTag("start", mBind.GetSprite("start"), ob, 1.0f);
                        _createTag("start", "start", ob, 1.0f);
                    }
                }
            }



        }

        public void SetDungeonData(IDungeonData data)
        {
            StopAllCoroutines();
            StartCoroutine(_sedDungeonDataIter(data));
        }
    }
}
