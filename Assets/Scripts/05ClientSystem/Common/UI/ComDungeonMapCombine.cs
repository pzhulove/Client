using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteAlways]
public class ComDungeonMapCombine : MonoBehaviour {

    public bool mIsPlay;
    public int mOffset = 10;
    public int mSizeUnit = 100;
    public DDungeonMapData mMapdata;

    public Sprite mBackground;
    public Sprite mItemImage;

    public Sprite mBossSprite;
    public Sprite mStartSprite;

    public Sprite[] mImageList = new Sprite[9];

    private ComDungeonUnitMap[] mUnitMapList = new ComDungeonUnitMap[0];

    private GameObject mRoot;

    public void SetDungeonMap(DDungeonMapData data)
    {
        mMapdata = data;
        _loadMap();
    }

    public void OnDestroy()
    {
        if (null != mMapdata)
        {
            mMapdata = null;
        }
    }

    public int mDungeonID;
    public void SetDungeonID(int id)
    {
        mDungeonID = id / 10 * 10;
        _updateMapTips();
    }

    private void _updateMapTips()
    {
        for (int i = 0; i < mUnitMapList.Length; i++)
        {
            var item = mMapdata.dungeonList[i];
            var com = mUnitMapList[i];

            if (item.dungeonid == mDungeonID)
            {
                com.transform.SetAsLastSibling();
                com.SetActive(true);
            }
            else
            {
                com.transform.SetAsFirstSibling();
                com.SetActive(false);
            }
        }
    }

    private void _loadMap()
    {
        var list = mMapdata.dungeonList;

        if (mRoot != null)
        {
            GameObject.DestroyImmediate(mRoot);
        }

        mRoot = new GameObject("Background", typeof(Image));
        Utility.AttachTo(mRoot, gameObject);
        mRoot.transform.SetAsFirstSibling();

        var rootRect = mRoot.GetComponent<RectTransform>();

        rootRect.anchorMin = new Vector2(0.5f, 0.5f);
        rootRect.anchorMax = new Vector2(0.5f, 0.5f);

        rootRect.offsetMin = new Vector2(-mSizeUnit * mMapdata.weith/2 - mOffset, -mSizeUnit * mMapdata.heigth/2 - mOffset);
        rootRect.offsetMax = new Vector2(mSizeUnit * mMapdata.weith/2 + mOffset, mSizeUnit * mMapdata.heigth/2 + mOffset);

        var rootImage = mRoot.GetComponent<Image>();
        rootImage.type = Image.Type.Sliced;
        rootImage.sprite = mBackground;

        mUnitMapList = new ComDungeonUnitMap[list.Length];

        for (int i = 0; i < list.Length; i++)
        {
            var item = list[i];

            var obj = new GameObject(i.ToString(), typeof(ComDungeonUnitMap), typeof(RectTransform));
            Utility.AttachTo(obj, mRoot);
            var com = obj.GetComponent<ComDungeonUnitMap>();
            com.mImageList = mImageList;
            com.mSizeUnit = mSizeUnit;
            com.mStartSprite = mStartSprite;
            com.mBossSprite = mBossSprite;
            com.mIsPlay = mIsPlay;
            com.SetDungeonData(item.dungeon);

            mUnitMapList[i] = com;

            var rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(item.posx * mSizeUnit / 2, item.posy * mSizeUnit / 2);
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (mIsPlay)
        {
            _loadMap();
            SetDungeonID(mDungeonID);
        }
#endif
    }

}
