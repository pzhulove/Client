using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using DG.Tweening;
using GameClient;

public class TreasureDungeonMap : MonoBehaviour
{
    public enum State
    {
        DS_NONE = 0,
        DS_IN,
        DS_CLOSE,
        DS_OPEN,
    }

    public struct RoomInfo
    {
        /// <summary>
        /// 通路
        /// </summary>
        public Image image;
        /// <summary>
        /// 特殊房间
        /// </summary>
        public Image imageFg;
        /// <summary>
        /// 背景
        /// </summary>
        public Image imageBoard;
        /// <summary>
        /// 大魔王
        /// </summary>
        public Image imageMonster;
        /// <summary>
        /// 玩家
        /// </summary>
        public Image imagePlayer;
        public int x;
        public int y;
        public State state;
        public bool visited;

        /// <summary>
        /// 在DDungeonData中的DSceneConnet的id
        /// </summary>
        public int id;

        public TreasureMapGenerator.ROOM_TYPE type;

        public void CopyRoomState(RoomInfo room)
        {
            image.sprite = room.image.sprite;
            image.material = room.image.material;
            image.enabled = room.image.enabled;
            image.transform.rotation = room.image.transform.rotation;

            imageFg.sprite = room.imageFg.sprite;
            imageFg.material = room.imageFg.material;
            imageFg.enabled = room.imageFg.enabled;
            imageFg.rectTransform.localScale = room.imageFg.rectTransform.localScale;

            imageBoard.sprite = room.imageBoard.sprite;
            imageBoard.material = room.imageBoard.material;
            imageBoard.enabled = room.imageBoard.enabled;

            imageMonster.sprite = room.imageMonster.sprite;
            imageMonster.material = room.imageMonster.material;
            imageMonster.enabled = room.imageMonster.enabled;

            imagePlayer.sprite = room.imagePlayer.sprite;
            imagePlayer.material = room.imagePlayer.material;
            imagePlayer.enabled = room.imagePlayer.enabled;

            state = room.state;
            visited = room.visited;
        }

        public void Hide()
        {
            image.enabled = false;
            imageFg.enabled = false;
            imageMonster.enabled = false;
            imagePlayer.enabled = false;
        }
    }

    public int mSizeUnit = 50;
    public Vector2 mDeltaFix = new Vector2(15, 15);
    public Vector2 mTextFix = new Vector2(3f, 2.1f);
    public Sprite mBackRoundImage;
    public Sprite mItemImage;

    public Sprite mItemImage2;

    public Sprite mTreasureSprite;
    public Sprite mStartSprite;
    public Sprite mMonsterSprite;

    public Sprite mBoxSprite;
    public Sprite mKeySprite;
    public Sprite mMapSprite;

    public float fgImageScale = 0.85f;

    [SpaceAttribute()]
    public Sprite[] mImageList = new Sprite[10];

    private IDungeonData mDungeonData;
    
    public Text mText;
    public GameObject mMapRoot;
    public Text mMiniMapText;
    public GameObject mMiniMapRoot;

    [HeaderAttribute("地图开启动画设置")]
    public Color            mDarkColor;
    public AnimationCurve   mBlinkCurve;
    public float            mBlinkTime;

    private RoomInfo[] mDungeonGraphicInfoList;
    private List<Image> mDungoenBlinkList = new List<Image>();

    private RoomInfo[] mSmallMapGraphicInfoList;
    private List<Image> mSmallMapList = new List<Image>();

    private int mCurrentX = -1;
    private int mCurrentY = -1;

    Vector2 viewPortData = new Vector2(5, 5);//小地图宽高

    private int[][] mapSpriteIndex = new int[16][]
    {
        new int[]{0,0 },
        new int[]{ 1,0},
        new int[]{ 1,90},
        new int[]{ 2,0},
        new int[]{ 1,180},
        new int[]{ 3,0},
        new int[]{ 2,90},
        new int[]{ 4,90},
        new int[]{ 1,270},
        new int[]{ 2,270},
        new int[]{ 3,270},
        new int[]{ 4,0},
        new int[]{ 2,180},
        new int[]{ 4,270},
        new int[]{ 4,180},
        new int[]{ 5,0},
    };
    public void SetViewPortData(int playerPosX, int playerPosY)
    {
        int centerOffsetX = ((int)viewPortData.x - 1) / 2;//计算自身位置房间个数 故减1
        int centerOffsetY = ((int)viewPortData.y - 1) / 2;

        int startPosX = playerPosX - centerOffsetX;
        int startPosY = playerPosY - centerOffsetY;
        int endPosX = playerPosX + centerOffsetX;
        int endPosY = playerPosY + centerOffsetY;
        if (startPosX < minX)
        {
            startPosX = minX;
            endPosX = minX + (int)viewPortData.x - 1;
        }
        if (startPosY < minY)
        {
            startPosY = minY;
            endPosY = minY + (int)viewPortData.y - 1;
        }
        if (endPosX > maxX)
        {
            endPosX = maxX;
            startPosX = endPosX - (int)viewPortData.x + 1;
        }
        if (endPosY > maxY)//既然小于0，则应大于等于最大宽高 保证区间在[0，maxLength -1];
        {
            endPosY = maxY;
            startPosY = endPosY - (int)viewPortData.y + 1;
        }
        for (int x = startPosX; x <= endPosX; x++) 
        {
            for (int y = startPosY; y <= endPosY; y++)  
            {
                SetRoomState(x, y, startPosX, startPosY); //这里设置房间的状态 和它四周的连线情况 (房间类型图标 ps 魔王 ，自己，宝箱等等)
            }
        }
        //UpdateDungoenGraphicInfo(mSmallMapGraphicInfoList, mSmallMapList);
    }

    void SetRoomState(int x,int y,int startX,int startY)
    {
        var miniMapIndex = FindRoomGrahpicIndex(mSmallMapGraphicInfoList, x - startX + 1, y - startY + 1);
        var fullMapIndex = FindRoomGrahpicIndex(mDungeonGraphicInfoList, x, y);
        if(miniMapIndex >= 0)
        {
            if (fullMapIndex >= 0)
            {
                mSmallMapGraphicInfoList[miniMapIndex].CopyRoomState(mDungeonGraphicInfoList[fullMapIndex]);
            }
            else
            {
                mSmallMapGraphicInfoList[miniMapIndex].Hide();

            }
        }
    }

    int FindRoomGrahpicIndex(RoomInfo[] list,int x,int y)
    {
        for(int i = 0; i < list.Length; ++i)
        {
            if(list[i].x == x && list[i].y == y)
            {
                return i;
            }
        }
        return -1;
    }

    private string _getMapItemName(int x, int y)
    {
        return string.Format("{0},{1}", x, y);
    }

    private int _getIndex(IDungeonConnectData item)
    {
        int ans = 0;
        for (int j = 0; j < 4; ++j)
        {
            ans *= 2;
            if (item.GetIsConnect(j))
            {
                ans += 1;
            }
        }

        return ans;
    }

    private Image _createTag(string name, Sprite sprite, GameObject root, float scale)
    {
        var obj = new GameObject(name, typeof(Image));
        Utility.AttachTo(obj, root);

        var startRect = obj.GetComponent<RectTransform>();
        startRect.offsetMin = new Vector2(-23, -23);
        startRect.offsetMax = new Vector2(23, 23);

        startRect.anchorMax = new Vector2(0.5f, 0.5f);
        startRect.anchorMin = new Vector2(0.5f, 0.5f);

        var startImage = obj.GetComponent<Image>();
        startImage.sprite = sprite;
        startImage.SetNativeSize();
        startImage.GetComponent<RectTransform>().localScale = Vector3.one * scale;

        return startImage;
    }

    int mCurrentBossX = -1;
    int mCurrentBossY = -1;
    bool isNearByBoss = false;

    public void RefreshBoss(int x,int y,bool isFade)
    {
        if (openAllRoomTag)
        {
            isFade = true;
        }
        if (!isFade)
        {
            var index = FindRoomGrahpicIndex(mDungeonGraphicInfoList, mCurrentBossX, mCurrentBossY);
            if (index >= 0)
            {
                var dgi = mDungeonGraphicInfoList[index];
                dgi.imageMonster.enabled = false;
                mDungeonGraphicInfoList[index] = dgi;
            }
        }
        else
        {
            bool needRefresh = false;
            if (mCurrentBossX != x || mCurrentBossY != y || isNearByBoss != isFade) 
            {
                needRefresh = true;
            }
            if (!needRefresh) return;
            for (int i = 0; i < mDungeonGraphicInfoList.Length; ++i)
            {
                var dgi = mDungeonGraphicInfoList[i];

                if (dgi.x == mCurrentBossX && dgi.y == mCurrentBossY)
                {
                    dgi.imageMonster.enabled = false;
                    if (dgi.visited == false)
                    {
                        dgi.imageBoard.enabled = false;
                    }
                }

                if (dgi.x == x && dgi.y == y)
                {
                    dgi.imageBoard.enabled = true;
                    dgi.imageMonster.enabled = true;
                }
                mDungeonGraphicInfoList[i] = dgi;
            }
        }
        mCurrentBossX = x;
        mCurrentBossY = y;
        isNearByBoss = isFade;
        //UpdateDungoenGraphicInfo(mDungeonGraphicInfoList, mDungoenBlinkList);
        SetViewPortData(mCurrentX, mCurrentY);
    }

    private Vector2 mFixedOffset = Vector2.zero;
    private Vector2 mFixedSize = Vector2.zero;

    int minX;
    int minY;
    int maxX;
    int maxY;

    private void _fixDungeonData(IDungeonData data)
    {
        int weidth = data.GetWeidth();
        int height = data.GetHeight();

        IDungeonConnectData clist = data.GetAreaConnectList(0);

        minX = clist.GetPositionX();
        minY = clist.GetPositionY();
        maxX = clist.GetPositionX();
        maxY = clist.GetPositionY();

        for (int i = 0; i < data.GetAreaConnectListLength(); ++i)
        {
            IDungeonConnectData cur = data.GetAreaConnectList(i);
            minX = Math.Min(minX, cur.GetPositionX());
            minY = Math.Min(minY, cur.GetPositionY());
            maxX = Math.Max(maxX, cur.GetPositionX());
            maxY = Math.Max(maxY, cur.GetPositionY());
        }

        int realWeidth = maxX - minX + 1;//地图宽
        int realHeight = maxY - minY + 1;//地图高         单位：1

        mFixedSize   = new Vector2(realWeidth, realHeight);//实际宽高
        mFixedOffset = new Vector2(minX, height - maxY - 1);
    }

    private GameObject mRoot;//整张地图

    private GameObject mMiniRoot;//小地图

    private void _initBackground(int width, int height,ref GameObject root,GameObject parent)
    {
        int mWidth = Mathf.Max(4, width);
        int mHeight = height;

        var rect = parent.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(mWidth * mSizeUnit + mDeltaFix.x * 2, mHeight * mSizeUnit + mDeltaFix.y * 2);

        if (root != null)
        {
            GameObject.DestroyImmediate(root);
        }

        root = new GameObject("Background", typeof(Image));
        Utility.AttachTo(root, parent);
        root.transform.SetAsFirstSibling();

        var rootRect = root.GetComponent<RectTransform>();

        rootRect.anchorMin = new Vector2(0, 0);
        rootRect.anchorMax = new Vector2(1, 1);

        rootRect.offsetMin = new Vector2(0, -mSizeUnit + 10);
        rootRect.offsetMax = new Vector2(0, 0);

        //rootRect.anchoredPosition = new Vector2(0, -mSizeUnit);

        var rootImage = root.GetComponent<Image>();
        rootImage.type = Image.Type.Sliced;
        rootImage.sprite = mBackRoundImage;
        rootImage.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }

    private void _loadDungeon()
    {
        DungeonID id = new DungeonID(GameClient.BattleDataManager.GetInstance().BattleInfo.dungeonId);

        string diff = ChapterUtility.GetHardString(3);

        if (null != mText)
        {
            mText.text = string.Format("<color=#14c5ff>{0}</color>{1}", diff, mDungeonData.GetName());
            if (null != mMiniMapText)
            {
                mMiniMapText.text = mText.text;
            }
        }

        mDungeonGraphicInfoList = new RoomInfo[(int)(mFixedSize.x* mFixedSize.y)];
        for (int i = minX; i <= maxX; ++i)
        {
            for (int j = minY; j <= maxY; ++j)
            {
                var item = _getConnect(i, j);
                var dgi = mDungeonGraphicInfoList[(i - minX) + (j - minY) * (int)mFixedSize.x];
                int itemId = -1;
                if(item != null)
                {
                    itemId = item.GetId();
                }
                CreateMapUnit(i, j, mRoot, ref dgi, itemId);
                dgi.imageBoard.enabled = true;
                dgi.imageBoard.sprite = mItemImage;
                if (item != null)
                {
                    var treasureMapType = (TreasureMapGenerator.ROOM_TYPE)item.GetTreasureType();
                    int index = _getIndex(item);
                    int spriteIndex = mapSpriteIndex[index][0];
                    dgi.image.sprite = mImageList[spriteIndex + 5];
                    dgi.image.transform.Rotate(new Vector3(0,0, mapSpriteIndex[index][1]));
                    dgi.type = treasureMapType;
                    switch (treasureMapType)
                    {
                        case TreasureMapGenerator.ROOM_TYPE.NONE:
                            break;
                        case TreasureMapGenerator.ROOM_TYPE.NORMAL_ROOM:
                            break;
                        case TreasureMapGenerator.ROOM_TYPE.BOSS_ROOM:
                            break;
                        case TreasureMapGenerator.ROOM_TYPE.END_ROOM:
                            dgi.imageBoard.enabled = true;
                            dgi.imageFg.enabled = true;
                            dgi.imageFg.sprite = mTreasureSprite;
                            dgi.imageFg.rectTransform.localScale = Vector3.one * fgImageScale;
                            break;
                        case TreasureMapGenerator.ROOM_TYPE.KEY_ROOM:
                            dgi.imageFg.sprite = mKeySprite;
                            break;
                        case TreasureMapGenerator.ROOM_TYPE.MAP_ROOM:
                            dgi.imageFg.sprite = mMapSprite;
                            break;
                        case TreasureMapGenerator.ROOM_TYPE.DROPITEM_ROOM:
                            dgi.imageFg.sprite = mBoxSprite;
                            dgi.imageFg.rectTransform.localScale = Vector3.one * fgImageScale;
                            break;
                        case TreasureMapGenerator.ROOM_TYPE.TRIAL_ROOM:
                            break;
                        case TreasureMapGenerator.ROOM_TYPE.BORN_ROOM:
                            dgi.imageBoard.enabled = true;
                            dgi.imageBoard.sprite = mItemImage2;
                            dgi.imagePlayer.enabled = true;
                            dgi.imagePlayer.sprite = mStartSprite;

                            //dgi.image.enabled = true;
                            //int temp = _getIndex(_getConnect(dgi.x, dgi.y));
                            //int tempIndex = mapSpriteIndex[temp][0];
                            //dgi.image.sprite = mImageList[tempIndex];
                            //dgi.image.sprite = mImageList[_getIndex(item)];

                            dgi.state = State.DS_IN;
                            dgi.visited = true;

                            mCurrentX = item.GetPositionX();
                            mCurrentY = item.GetPositionY();
                            break;
                        case TreasureMapGenerator.ROOM_TYPE.MAX_COUNT:
                            break;
                        default:
                            break;
                    }
                }
                mDungeonGraphicInfoList[(i - minX) + (j - minY) * (int)mFixedSize.x] = dgi;
            }
        }
        //UpdateDungoenGraphicInfo(mDungeonGraphicInfoList, mDungoenBlinkList);
    }

    private void _initSmallMapRoot()
    {
        int smallMapLenght = (int)viewPortData.x * (int)viewPortData.y;
        mSmallMapGraphicInfoList = new RoomInfo[smallMapLenght];
        for(int i = 0; i < smallMapLenght; ++i)
        {
            int x = i % 5;
            int y = i / 5;
            var dgi = mSmallMapGraphicInfoList[i];
            CreateMapUnit(x + 1, y + 1, mMiniRoot, ref dgi, i, true);
            mSmallMapGraphicInfoList[i] = dgi;
        }
    }

    /// <summary>
    /// 创建小地图单元格
    /// </summary>
    private void CreateMapUnit(int posX,int posY,GameObject root ,ref RoomInfo dgi,int id = 0, bool isSmallMap = false)
    {
        var obj = new GameObject(_getMapItemName(posX, posY), typeof(Image));
        var fg = new GameObject(_getMapItemName(posX, posY), typeof(Image));

        var x = posX;
        var y = mDungeonData.GetHeight() - (posY + 1);

        Utility.AttachTo(fg, obj);
        Utility.AttachTo(obj, root);

        var rect = obj.GetComponent<RectTransform>();
        rect.pivot = Vector2.zero;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;

        var newsize = mSizeUnit - 8;
        if (isSmallMap)
        {
            Vector2 tempFixedOffset = new Vector2(1, 0);
            rect.offsetMin = new Vector2(x * mSizeUnit + mDeltaFix.x + 2, y * mSizeUnit + mDeltaFix.y + 2) - tempFixedOffset * mSizeUnit;
            rect.offsetMax = new Vector2(x * mSizeUnit + mSizeUnit + mDeltaFix.x - 2, y * mSizeUnit + mSizeUnit + mDeltaFix.y - 2) - tempFixedOffset * mSizeUnit;
        }
        else
        {
            rect.offsetMin = new Vector2(x * mSizeUnit + mDeltaFix.x + 2, y * mSizeUnit + mDeltaFix.y + 2) - mFixedOffset * mSizeUnit;
            rect.offsetMax = new Vector2(x * mSizeUnit + mSizeUnit + mDeltaFix.x - 2, y * mSizeUnit + mSizeUnit + mDeltaFix.y - 2) - mFixedOffset * mSizeUnit;
        }
        

        var fgrect = fg.GetComponent<RectTransform>();
        fgrect.pivot = Vector2.one * 0.5f;
        fgrect.anchorMin = Vector2.zero;
        fgrect.anchorMax = Vector2.one;
        fgrect.offsetMin = Vector2.zero;
        fgrect.offsetMax = Vector2.zero;

        var imageItemBoard = obj.GetComponent<Image>();
        imageItemBoard.sprite = mItemImage;
        imageItemBoard.type = Image.Type.Simple;

        var imageCom = fg.GetComponent<Image>();
        imageCom.type = Image.Type.Simple;

        dgi.image = imageCom;
        dgi.imageFg = _createTag("imageFg", null, obj, 1.0f);
        dgi.imageMonster = _createTag("imageMonster", null, obj, 1.0f);
        dgi.imagePlayer = _createTag("imagePlayer", null, obj, 1.0f);
        dgi.imageBoard = imageItemBoard;
        dgi.x = posX;
        dgi.y = posY;
        dgi.state = State.DS_CLOSE;
        dgi.visited = false;
        dgi.id = id;

        dgi.imageBoard.enabled = false;
        dgi.imageFg.enabled = false;
        dgi.image.enabled = false;
        dgi.imageMonster.enabled = false;
        dgi.imagePlayer.enabled = false;
        dgi.imagePlayer.sprite = mStartSprite;
        dgi.imageMonster.sprite = mMonsterSprite;
    }

    private Button mBigMapBtn;
    private Button mMiniMapBtn;
    private void  _initButtonEvent()
    {
        mBigMapBtn = mMapRoot.GetComponent<Button>();
        mMiniMapBtn = mMiniMapRoot.GetComponent<Button>();
        if(mBigMapBtn)
        {
            mBigMapBtn.onClick.AddListener(_onBigMapButtonClick);
        }
        if (mMiniMapBtn)
        {
            mMiniMapBtn.onClick.AddListener(_onMiniMapButtonClick);
        }
    }

    #region UI_CALLBACK_EVENT
    private void _onBigMapButtonClick()
    {
        _setMapState(true);
    }

    private void _onMiniMapButtonClick()
    {
        _setMapState(false);
    }
    public class UITreasureEventParam
    {
        public int width = 0;
        public int height = 0;
    }
    UITreasureEventParam mMapSizeEventParam = new UITreasureEventParam();
    private void _setMapState(bool mMimiMapActive)
    {
        if (mMapRoot)
        {
            mMapRoot.SetActive(!mMimiMapActive);
        }
        if (mMiniMapRoot)
        {
            mMiniMapRoot.SetActive(mMimiMapActive);
        }
        if(mMimiMapActive)
        {
            mMapSizeEventParam.width = (int)viewPortData.x;
            mMapSizeEventParam.height = (int)viewPortData.y;
        }
        else
        {
            mMapSizeEventParam.width = (int)mFixedSize.x;
            mMapSizeEventParam.height = (int)mFixedSize.y;
        }
        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TreasureMapSizeChange, mMapSizeEventParam);
    }
    #endregion

    public void SetDungeonData(IDungeonData scene)
    {
        mDungeonData = scene;

        _fixDungeonData(scene);

        _initBackground((int)mFixedSize.x, (int)mFixedSize.y, ref mRoot, mMapRoot);
        _loadDungeon();

        _initBackground((int)viewPortData.x, (int)viewPortData.y, ref mMiniRoot, mMiniMapRoot);
        _initSmallMapRoot();

        _initButtonEvent();

        if (mMapRoot)
        {
            mMapRoot.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (null != mDungeonData)
        {
            //for (int i = 0; i < mDungeonData.GetAreaConnectListLength(); ++i)
            //{
            //    //mDungeonData.GetAreaConnectList(i).SetSceneData(null);
            //}
            mDungeonData = null;
        }
    }

    public void UpdateDungoenGraphicInfo(RoomInfo[] roomGraphicInfo,List<Image> imageList)
    {
        imageList.Clear();

        for (int i = 0; i < roomGraphicInfo.Length; ++i)
        {
            var current = roomGraphicInfo[i];

            switch (current.state)
            {
                case State.DS_IN:
                    {
                        if (current.image)
                        {
                            current.image.color = Color.white;
                        }
                        if (current.imageBoard)
                        {
                            current.imageBoard.color = Color.white;
                        }
                    }
                    break;

                case State.DS_CLOSE:
                    {
                        if (current.image)
                        {
                            current.image.color = mDarkColor;
                        }
                        if (current.imageBoard)
                        {
                            current.imageBoard.color = mDarkColor;
                        }
                    }
                    break;

                case State.DS_OPEN:
                    {
                        if (current.image)
                        {
                            imageList.Add(current.image);
                        }
                        if (current.imageBoard)
                        {
                            imageList.Add(current.imageBoard);
                        }
                    }
                    break;
            }
        }
    }

    private IDungeonConnectData _getConnect(int x, int y)
    {
        for (int i = 0; i < mDungeonData.GetAreaConnectListLength(); ++i)
        {
            var connect = mDungeonData.GetAreaConnectList(i);
            if (connect.GetPositionX() == x && connect.GetPositionY() == y)
            {
                return connect;
            }
        }

        return null;
    }

    public void SetStartPosition(int x, int y)
    {
        for (int i = 0; i < mDungeonGraphicInfoList.Length; ++i)
        {
            var cur = mDungeonGraphicInfoList[i];
            cur.state = State.DS_CLOSE;

            if (cur.x == mCurrentX && cur.y == mCurrentY)
            {
                cur.imagePlayer.enabled = false;
            }

            if (cur.x == x && cur.y == y)
            {
                cur.image.enabled = true;
                int index = _getIndex(_getConnect(cur.x, cur.y));
                int spriteIndex = mapSpriteIndex[index][0];
                cur.image.sprite = mImageList[spriteIndex];
                cur.imageBoard.enabled = true;
                cur.imageBoard.sprite = mItemImage2;
                cur.state = State.DS_IN;
                cur.imagePlayer.enabled = true;
                cur.visited = true;

                switch (cur.type)
                {
                    case TreasureMapGenerator.ROOM_TYPE.KEY_ROOM:
                    case TreasureMapGenerator.ROOM_TYPE.MAP_ROOM:
                    case TreasureMapGenerator.ROOM_TYPE.DROPITEM_ROOM:
                        if (!IsRoomFgImageClose(x, y))
                        {
                            cur.imageFg.enabled = true;
                        }
                        break;
                }
            }
            mDungeonGraphicInfoList[i] = cur;
        }

        //UpdateDungoenGraphicInfo(mDungeonGraphicInfoList, mDungoenBlinkList);

        mCurrentX = x;
        mCurrentY = y;
    }

    public void SetOpenPosition(int x, int y)
    {
        for(int i = 0; i < mDungeonGraphicInfoList.Length; ++i)
        {      
             var cur = mDungeonGraphicInfoList[i];
             var state = State.DS_CLOSE; 
             if(cur.x == x && cur.y == y)
             {
                 state = State.DS_IN;   
             }
             cur.state = state;
             mDungeonGraphicInfoList[i] = cur;
        }
        //ChangeLinkMapState(x, y, State.DS_OPEN);
        //UpdateDungoenGraphicInfo(mDungeonGraphicInfoList, mDungoenBlinkList);
    }

    //public void ChangeLinkMapState(int x, int y, State state)
    //{
    //    var index = FindRoomGrahpicIndex(mDungeonGraphicInfoList, x, y);//mDungeonGraphicInfoList,v=>{return v.x == x && v.y == y;});

    //    if(index >= 0)
    //    {
    //        var item = mDungeonData.GetAreaConnectList(index);

    //        for(int i = 0; i < 4; ++i)
    //        {
    //            if(item.GetIsConnect(i))
    //            {
    //                int tempIndex;
    //                mDungeonData.GetSideByType(x,y,(TransportDoorType)i,out tempIndex);

    //                if(tempIndex >= 0)
    //                {
    //                    var cur = mDungeonGraphicInfoList[tempIndex];
    //                    cur.state = state;
    //                    cur.image.enabled = true;
    //                    cur.imageBoard.enabled = true;
    //                    mDungeonGraphicInfoList[tempIndex] = cur;
    //                }
    //            }
    //        } 
    //    }
    //}

    bool openAllRoomTag = false;
    public void OpenAllRoom()
    {
        if (!openAllRoomTag)
        {
            for (int i = 0; i < mDungeonGraphicInfoList.Length; ++i)
            {
                var dgi = mDungeonGraphicInfoList[i];
                if (dgi.state != State.DS_IN && dgi.visited == false)
                {
                    dgi.visited = true;
                    dgi.imageBoard.enabled = true;
                    if(dgi.id != -1)
                    {
                        dgi.image.enabled = true;
                        int index = _getIndex(_getConnect(dgi.x, dgi.y));
                        int spriteIndex = mapSpriteIndex[index][0];
                        dgi.image.sprite = mImageList[spriteIndex + 5];
                    }
                    if (dgi.x == mCurrentBossX && dgi.y == mCurrentBossY)
                    {
                        dgi.imageMonster.enabled = true;
                    }
                    switch (dgi.type)
                    {
                        case TreasureMapGenerator.ROOM_TYPE.KEY_ROOM:
                            dgi.imageFg.enabled = true;
                            break;
                        case TreasureMapGenerator.ROOM_TYPE.MAP_ROOM:
                            dgi.imageFg.enabled = true;
                            break;
                        case TreasureMapGenerator.ROOM_TYPE.DROPITEM_ROOM:
                            dgi.imageFg.enabled = true;
                            break;
                        default:
                            break;
                    }
                    mDungeonGraphicInfoList[i] = dgi;
                }
            }
            SetViewPortData(mCurrentX, mCurrentX);
        }
        openAllRoomTag = true;
    }

    List<Vector2> ClearFgImagePosList = new List<Vector2>();
    private bool IsRoomFgImageClose(int x,int y)
    {
        for(int i = 0; i < ClearFgImagePosList.Count; ++i)
        {
            if(ClearFgImagePosList[i].x == x && ClearFgImagePosList[i].y == y)
            {
                return true;
            }
        }
        return false;
    }
    public void ClearCurrentRoomSpecialTag(TreasureMapGenerator.ROOM_TYPE type)
    {
        var index = FindRoomGrahpicIndex(mDungeonGraphicInfoList, mCurrentX, mCurrentY);
        if (index >= 0 && index < mDungeonGraphicInfoList.Length)
        {
            var dgi = mDungeonGraphicInfoList[index];
            if (dgi.type == type)
            {
                if(type == TreasureMapGenerator.ROOM_TYPE.MAP_ROOM)
                {
                    _setMapState(false);
                }
                dgi.imageFg.enabled = false;
                SetViewPortData(mCurrentX, mCurrentY);
                ClearFgImagePosList.Add(new Vector2(mCurrentX, mCurrentY));
            }
            //if (dgi.type == type)
            //{
            //    dgi.imageFg.enabled = false;
            //    SetViewPortData(mCurrentX, mCurrentY);
            //    ClearFgImagePosList.Add(new Vector2(mCurrentX, mCurrentY));
            //}
        }
    }
    
    void Update()
    {
        //if(mDungoenBlinkList.Count > 0)
        //{
        //    float  t = Mathf.Repeat(Time.realtimeSinceStartup,mBlinkTime) / mBlinkTime;
        //    float  v = mBlinkCurve.Evaluate(t);
        //    Color  c = Color.Lerp(Color.white,mDarkColor,v);

        //    for(int i = 0; i < mDungoenBlinkList.Count; ++i)
        //    {
        //        var current = mDungoenBlinkList[i];
        //        current.color = c;
        //    }
        //    for(int i = 0; i < mSmallMapList.Count; ++i)
        //    {
        //        var current = mSmallMapList[i];
        //        current.color = c;
        //    }
        //}
    }
    
}
