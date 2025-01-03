using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteAlways]
public class ComDungeonUnitMap : MonoBehaviour {
    public int mSizeUnit = 100;

    //public Sprite mBackRoundImage;
    public Sprite mItemImage;

    public Sprite mBossSprite;
    public Sprite mStartSprite;

    public Sprite[] mImageList = new Sprite[9];

    public bool mIsPlay = true;

    public IDungeonData mDungeonData;

    private GameObject mRoot;

    private void _initBackground()
    {
        int mWidth  = mDungeonData.GetWeidth();
        int mHeight = mDungeonData.GetHeight();

        var rect = gameObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(mWidth * mSizeUnit, mHeight * mSizeUnit);

        if (mRoot != null)
        {
            GameObject.DestroyImmediate(mRoot);
        }

        mRoot = new GameObject("Background", typeof(Image));
        Utility.AttachTo(mRoot, gameObject);
        mRoot.transform.SetAsFirstSibling();

        var rootRect = mRoot.GetComponent<RectTransform>();

        rootRect.anchorMin = new Vector2(0, 0);
        rootRect.anchorMax = new Vector2(1, 1);

        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        rootRect.anchoredPosition = new Vector2(0, -mSizeUnit);

        var rootImage = mRoot.GetComponent<Image>();
        rootImage.type = Image.Type.Sliced;
        rootImage.color = Color.clear;
        //rootImage.sprite = mBackRoundImage;
    }

    private string _getMapItemName(int x, int y)
    {
        return string.Format("{0},{1}", x, y);
    }

    private int mBossX = -1;
    private int mBossY = -1;

    private bool mInited = false;

    private GameObject mStartObject;
    private GameObject mBossObject;
    private Image[] mImageObjectList = new Image[0];
    private void _loadDungeon()
    {
        mImageObjectList = new Image[mDungeonData.GetAreaConnectListLength()];

        for (int i = 0; i < mDungeonData.GetAreaConnectListLength(); ++i)
        {
            var item = mDungeonData.GetAreaConnectList(i);
            var x = item.GetPositionX();
            var y = mDungeonData.GetHeight() - (item.GetPositionY() + 1);
            var obj = new GameObject(_getMapItemName(item.GetPositionX(), item.GetPositionY()), typeof(Image));

            Utility.AttachTo(obj, mRoot);

            var rect = obj.GetComponent<RectTransform>();
            rect.pivot = Vector2.zero;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;

            rect.offsetMin = new Vector2(x * mSizeUnit, y * mSizeUnit);
            rect.offsetMax = new Vector2(x * mSizeUnit + mSizeUnit, y * mSizeUnit + mSizeUnit);

            var imageCom = obj.GetComponent<Image>();
            imageCom.type = Image.Type.Simple;
            imageCom.sprite = mItemImage;

            if (item.IsStart())
            {
                mStartObject = new GameObject("start", typeof(Image));
                Utility.AttachTo(mStartObject, obj);

                var startRect = mStartObject.GetComponent<RectTransform>();
                startRect.offsetMin = new Vector2(-15, -15);
                startRect.offsetMax = new Vector2(15, 15);

                startRect.anchorMax = new Vector2(0.5f, 0.5f);
                startRect.anchorMin = new Vector2(0.5f, 0.5f);

                var startImage = mStartObject.GetComponent<Image>();
                startImage.sprite = mStartSprite;
            }

            if (item.IsBoss())
            {
                mBossObject = new GameObject("boss", typeof(Image));
                Utility.AttachTo(mBossObject, obj);

                var bossRect = mBossObject.GetComponent<RectTransform>();
                bossRect.offsetMin = new Vector2(-15, -15);
                bossRect.offsetMax = new Vector2(15, 15);

                bossRect.anchorMax = new Vector2(0.5f, 0.5f);
                bossRect.anchorMin = new Vector2(0.5f, 0.5f);

                var bossImage = mBossObject.GetComponent<Image>();
                bossImage.sprite = mBossSprite;

                mBossX = item.GetPositionX();
                mBossY = item.GetPositionY();
            }

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

            imageCom.sprite = mImageList[ans];
            mImageObjectList[i] = imageCom;
        }
    }


    private bool mActived = true;
    public void SetActive(bool active)
    {
        if (!mInited)
        {
            Logger.LogWarning("not inited");
            return;
        }

        if (mActived != active)
        {
            mActived = active;

            mStartObject.CustomActive(mActived);
			mBossObject.CustomActive(mActived);

            for (int i = 0; i < mImageObjectList.Length; i++)
            {
                var item = mImageObjectList[i];
                if (mActived)
                {
                    item.color = Color.white;
                }
                else
                {
                    item.color = Color.grey;
                }
            }
        }
    }


    public void SetDungeonData(IDungeonData scene)
    {
        if (scene == null)
        {
            Logger.LogWarning("scene data is nil");
            return;
        }

        mDungeonData = scene;

        _initBackground();
        _loadDungeon();

        _initData();

        mInited = true;
    }

    private void _initData()
    {
    }

    public void SetStartPosition(int x, int y)
    {
        if (!mInited)
        {
            Logger.LogWarning("not inited");
            return;
        }

        if (x == mBossX && y == mBossY
        && mBossX != -1 && mBossY != -1)
        {
            mStartObject.SetActive(false);
        }
        else
        {
            var target = Utility.FindGameObject(mRoot, _getMapItemName(x, y));
            if (target != null)
            {
                Utility.AttachTo(mStartObject, target);
            }
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (mIsPlay && !Application.isPlaying)
        {
            _initBackground();
            _loadDungeon();
        }
#endif
    }

}
