using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using DG.Tweening;
using GameClient;

public enum eDungeonGraphicType
{
    Start,
    Boss,
    Hell,
    Normal,
    Chest,
    Egg,
}

public struct DungeonGraphicInfo
{
    public enum State
    {
        DS_NONE = 0,
        DS_IN,
        DS_CLOSE,
        DS_OPEN,
    }

    public Image image;
    public Image imageFg;
    public Image imageArrow;
    public Image imageBoard;
    public int x;
    public int y;
    public State state;
    public bool visited;

    public int dataX;
    public int dataY;

    /// <summary>
    /// 在DDungeonData中的DSceneConnet的id
    /// </summary>
    public int id;

    public eDungeonGraphicType type;

    public void CopyRoomtState(DungeonGraphicInfo info)
    {
        state = info.state;
        visited = info.visited;
        type = info.type;
        id = info.id;

        dataX = info.x;
        dataY = info.y;
    }

    public void Reset()
    {
        dataX = -1;
        dataY = -1;

        id = -1;
        type = eDungeonGraphicType.Normal;
        visited = false;
        state = State.DS_CLOSE;

        if (image != null)
            image.enabled = false;
        if (imageFg != null)
            imageFg.enabled = false;
        if (imageArrow != null)
            imageArrow.enabled = false;
        //if (imageBoard != null)
        //    imageBoard.enabled = false;
    }
}

[ExecuteAlways]
public class ComDungeonMap : MonoBehaviour {
    public int mSizeUnit = 100;
    public Vector2 mDeltaFix;
    public Vector2 mTextFix;

    public Sprite mBackRoundImage;
    public Color  mBackRoundDarkColor;
    //public Material mBackRoundMaterial;
    public Sprite mItemImage;
    public Sprite mItemBlankImage;
    //public Material mItemMaterial;

    public Sprite mBossSprite;
    //public Material mBossMaterial;
    public Sprite mStartSprite;
    //public Material mStartMaterial;

    public Sprite mChestSprite;

    public Sprite mArrowRightSprite;
    
    private const int kMaxHell = 2;
    public Sprite[] mHellSprite = new Sprite[kMaxHell];
    //public Material mHellSpriteMaterial;

    [SpaceAttribute()]
    public Sprite[] mImageList = new Sprite[9];
    //public Material mImageMaterial;

    public IDungeonData mDungeonData;

    private GameObject mRoot;
    private GameObject eggRoom;
    public Text mText;
    //public GameObject mButton;
    public GameObject mTextRoot;

    [HeaderAttribute("地图开启动画设置")]
    public Color            mDarkColor;
    public AnimationCurve   mBlinkCurve;
    public float            mBlinkTime;
    [SpaceAttribute()]
    
    /* glod & box */
    // TODO 移除这个组件
    //public Text mTextGold;
    //public Text mTextBox;

    //public GameObject mTextBoxRoot;
    //public GameObject mTextGoldRoot;

    private int mGlodCount;
    private int mBoxCount;

    private DungeonGraphicInfo[] mDungeonGraphicInfoList;
    private List<Image>          mDungoenBlinkList = new List<Image>();

    private DungeonGraphicInfo[] mSmallMapGraphicInfoList;

    private int mCurrentX = -1;
    private int mCurrentY = -1;

    public delegate int GetNextRoomIdDel();
    private GetNextRoomIdDel mGetNextRoomIdDel;
    private Image mArrowBlink = null;
    private Transform mArrowBlinkParent;
    
    DungeonGraphicInfo[] dungeonGraphicInfoList
    {
        get{ return mDungeonGraphicInfoList; }
    }

    public void SetViewPortData(int playerPosX, int playerPosY)
    {
        int centerOffsetX = ((int)mViewPortData.x - 1) / 2;//计算自身位置房间个数 故减1
        int centerOffsetY = ((int)mViewPortData.y - 1) / 2;

        int startPosX = playerPosX - centerOffsetX;
        int startPosY = playerPosY - centerOffsetY;
        int endPosX = playerPosX + centerOffsetX + 1;
        int endPosY = playerPosY + centerOffsetY;
        if (startPosX < minX)                                                                   
        {
            startPosX = minX;
            endPosX = minX + (int)mViewPortData.x - 1;
        }
        if (startPosY < minY)
        {
            startPosY = minY;
            endPosY = minY + (int)mViewPortData.y - 1;
        }
        if (endPosX > maxX)
        {
            endPosX = maxX;
            startPosX = endPosX - (int)mViewPortData.x + 1;
        }
        if (endPosY > maxY)//既然小于0，则应大于等于最大宽高 保证区间在[0，maxLength -1];
        {
            endPosY = maxY;
            startPosY = endPosY - (int)mViewPortData.y + 1;
        }
        
        for (int i = 0; i < mSmallMapGraphicInfoList.Length; i++)
        {
            mSmallMapGraphicInfoList[i].Reset();
            mSmallMapGraphicInfoList[i].imageBoard.sprite = mItemBlankImage;
        }
        
        for (int x = startPosX; x <= endPosX; x++)
        {
            for (int y = startPosY; y <= endPosY; y++)
            {
                SetRoomState(x, y, startPosX, startPosY); //这里设置房间的状态 和它四周的连线情况 
            }
        }
        UpdateDungoenGraphicInfo();
    }

    void SetRoomState(int x, int y, int startX, int startY)
    {
        var miniMapIndex = FindRoomGrahpicIndex(mSmallMapGraphicInfoList, x - startX + 1, y - startY + 1);
        var fullMapIndex = FindRoomGrahpicIndex(mDungeonGraphicInfoList, x, y);
        if (miniMapIndex >= 0)
        {
            if (fullMapIndex >= 0)
            {
                mSmallMapGraphicInfoList[miniMapIndex].CopyRoomtState(mDungeonGraphicInfoList[fullMapIndex]);
                SetImage(mSmallMapGraphicInfoList[miniMapIndex]);
            }
            else
            {
                mSmallMapGraphicInfoList[miniMapIndex].Reset();
                mSmallMapGraphicInfoList[miniMapIndex].imageBoard.sprite = mItemBlankImage;
            }
        }
    }

    void SetImage(DungeonGraphicInfo item)
    {
        switch (item.type)
        {
            case eDungeonGraphicType.Start:
                if(item.imageBoard != null)
                {
                    item.imageBoard.enabled = true;
                    item.imageBoard.sprite = mItemImage;
                }
                if(item.imageFg != null)
                {
                    if (IsInCurrentPostion(item))
                    {
                        item.imageFg.enabled = true;
                        item.imageFg.sprite = mStartSprite;
                    }
                    else
                    {
                        item.imageFg.enabled = false;
                    }
                }
                if(item.image != null)
                {
                    item.image.enabled = true;
                    item.image.sprite = mImageList[_getIndex(_getConnect(item.dataX, item.dataY))];
                }
                break;
            case eDungeonGraphicType.Boss:
                if(item.imageBoard != null)
                {
                    item.imageBoard.enabled = true;
                    item.imageBoard.sprite = mItemImage;
                }
                if(item.imageFg != null)
                {
                    item.imageFg.enabled = true;
                    item.imageFg.sprite = mBossSprite;
                }
                if (item.image != null)
                {
                    item.image.enabled = true;
                    item.image.sprite = mImageList[_getIndex(_getConnect(item.dataX, item.dataY))];
                }
                break;
            case eDungeonGraphicType.Hell:
                if(item.imageFg != null)
                {
                    if(mHellMode == Protocol.DungeonHellMode.Hard)
                    {
                        item.imageFg.sprite = mHellSprite[1];
                        item.imageFg.enabled = true;
                        if (item.imageBoard != null)
                        {
                            item.imageBoard.enabled = true;
                            item.imageBoard.sprite = mItemImage;
                        }
                    }
                    else if(mHellMode == Protocol.DungeonHellMode.Normal)
                    {
                        item.imageFg.sprite = mHellSprite[0];
                        item.imageFg.enabled = true;
                        if (item.imageBoard != null)
                        {
                            item.imageBoard.enabled = true;
                            item.imageBoard.sprite = mItemImage;
                        }
                    }
                }
                
                break;
            case eDungeonGraphicType.Normal:
                //if (item.visited) 
                {
                    if(item.image != null)
                    {
                        item.image.sprite = mImageList[_getIndex(_getConnect(item.dataX, item.dataY))];
                        item.image.enabled = true;
                    }

                    if (item.imageBoard != null)
                    {
                        item.imageBoard.enabled = true;
                        item.imageBoard.sprite = mItemImage;
                    }
                }
                /*else
                {
                    if(item.image != null)
                    {
                        item.image.sprite = mImageList[0];
                    }
                }*/

                if (item.imageFg != null)
                {
                    if (IsInCurrentPostion(item)) 
                    {
                        item.imageFg.enabled = true;
                        item.imageFg.sprite = mStartSprite;
                    }
                    else
                    {
                        item.imageFg.enabled = false;
                    }
                }
                break;
            case eDungeonGraphicType.Chest:
                if(item.imageFg != null)
                {
                    item.imageFg.enabled = true;
                    item.imageFg.sprite = mChestSprite;
                }
                break;
            case eDungeonGraphicType.Egg:
                break;
            default:
                break;
        }

        
        if (item.visited)
        {
            if (item.image != null)
            {
                item.image.color = Color.white;
            }
            if (item.imageBoard != null)
            {
                item.imageBoard.color = Color.white;
            }
        }
        else
        {
            if (item.image != null)
            {
                item.image.color = mBackRoundDarkColor;
            }
            if (item.imageBoard != null)
            {
                item.imageBoard.color = mBackRoundDarkColor;
            }
        }
        

        if (item.state == DungeonGraphicInfo.State.DS_OPEN) 
        {
            if (item.image != null)
                item.image.enabled = true;
            if (item.imageBoard != null)
            {
                item.imageBoard.enabled = true;
                item.imageBoard.sprite = mItemImage;
            }
        }
        
        if (item.imageArrow != null && mArrowBlink != item.imageArrow)
        {
            if (IsInCurrentPostion(item)) 
            {
                item.imageArrow.enabled = false;
                int index = GetNextRoomId();
                if (index >= 0)
                {
                    var nextConnectData = mDungeonData.GetAreaConnectList(index);
                    var tran = item.imageArrow.GetComponent<RectTransform>();
                    //右
                    if (nextConnectData.GetPositionX() > mCurrentX)
                    {
                        tran.localEulerAngles = new Vector3(0,0,0);
                        tran.anchoredPosition = new Vector2(28,0);
                    }
                    //左
                    else if (nextConnectData.GetPositionX() < mCurrentX)
                    {
                        tran.localEulerAngles = new Vector3(0,0,180);
                        tran.anchoredPosition = new Vector2(-28,0);
                    }
                    //下
                    else if (nextConnectData.GetPositionY() > mCurrentY)
                    {
                        tran.localEulerAngles = new Vector3(0,0,-90);
                        tran.anchoredPosition = new Vector2(0,-28);
                    }
                    //上
                    else if (nextConnectData.GetPositionY() < mCurrentY)
                    {
                        tran.localEulerAngles = new Vector3(0,0,90);
                        tran.anchoredPosition = new Vector2(0,28);
                    }
                }
            }
            else
            {
                item.imageArrow.enabled = false;
            }
        }
    }

    bool IsInCurrentPostion(DungeonGraphicInfo item)
    {
        return item.dataX == mCurrentX && item.dataY == mCurrentY;
    }

    int FindRoomGrahpicIndex(DungeonGraphicInfo[] list, int x, int y)
    {
        for (int i = 0; i < list.Length; ++i)
        {
            if (list[i].x == x && list[i].y == y)
            {
                return i;
            }
        }
        return -1;
    }

    private void _initBackground()
    {
        int mWidth = Mathf.Max(4, (int)mViewPortData.x);
        int mHeight = (int)mViewPortData.y;

        var rect = gameObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(mWidth * mSizeUnit + mDeltaFix.x * 2, mHeight * mSizeUnit + mDeltaFix.y * 2);

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

        rootRect.offsetMin = new Vector2(0, -mSizeUnit+18);
        rootRect.offsetMax = new Vector2(0, 0);

        //rootRect.anchoredPosition = new Vector2(0, -mSizeUnit);

        var rootImage = mRoot.GetComponent<Image>();
        rootImage.type = Image.Type.Sliced;
        rootImage.sprite = mBackRoundImage;
        //rootImage.material = mBackRoundMaterial;
        rootImage.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        rootImage.raycastTarget = false;
        //if (null != mButton)
        //{
        //    var buttonRect = mButton.GetComponent<RectTransform>();

        //    buttonRect.anchorMin = new Vector2(1, 1);
        //    buttonRect.anchorMax = new Vector2(1, 1);

        //    buttonRect.offsetMin = new Vector2(-mSizeUnit/2-mDeltaFix.x, -mSizeUnit/2-mDeltaFix.y);
        //    buttonRect.offsetMax = new Vector2(0, 0);

        //    var buttonImage = mButton.GetComponent<Image>();
        //    buttonImage.type = Image.Type.Sliced;
        //    buttonImage.sprite = mBackRoundImage;
        //    //buttonImage.material = mBackRoundMaterial;

        //    var buttonCom = mButton.GetComponent<Button>();
        //    buttonCom.onClick.RemoveAllListeners();
        //    buttonCom.onClick.AddListener(()=> {
        //        mRoot.SetActive(!mRoot.activeSelf);
        //    });
        //}

        //var textRect = mTextRoot.GetComponent<RectTransform>();

        //textRect.anchorMin = new Vector2(0, 1);
        //textRect.anchorMax = new Vector2(1, 1);

        //textRect.offsetMin = new Vector2(mTextFix.x, -mSizeUnit-mTextFix.y);
        //textRect.offsetMax = new Vector2(-mTextFix.x, -mTextFix.y);

        //var textImage = mTextRoot.GetComponent<Image>();
        //textImage.type = Image.Type.Sliced;
        //textImage.sprite = mBackRoundImage;
    }

    private string _getMapItemName(int x, int y)
    {
        return string.Format("{0},{1}", x, y);
    }

    private int _getIndex(IDungeonConnectData item)
    {
        int xe = 1;
        int ans = 0;
        if (item == null)
            return ans;
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

    private Image _createTag(string name, Sprite sprite, GameObject root, float scale)
    {
        var obj = new GameObject(name, typeof(Image));
        Utility.AttachTo(obj, root);

        var startRect = obj.GetComponent<RectTransform>();
        startRect.offsetMin = new Vector2(-15, -15);
        startRect.offsetMax = new Vector2(15, 15);

        startRect.anchorMax = new Vector2(0.5f, 0.5f);
        startRect.anchorMin = new Vector2(0.5f, 0.5f);

        var startImage = obj.GetComponent<Image>();
        startImage.sprite = sprite;
        startImage.SetNativeSize();
        startImage.GetComponent<RectTransform>().localScale = Vector3.one * scale;
        startImage.raycastTarget = false;
        
        return startImage;
    }
    
    private Image _createArrow(string name, Sprite sprite, GameObject root, float scale)
    {
        var obj = new GameObject(name, typeof(Image));
        Utility.AttachTo(obj, root);

        var startRect = obj.GetComponent<RectTransform>();
        //startRect.offsetMin = new Vector2(-15, -15);
        //startRect.offsetMax = new Vector2(15, 15);

        startRect.anchorMax = new Vector2(0.5f, 0.5f);
        startRect.anchorMin = new Vector2(0.5f, 0.5f);

        var startImage = obj.GetComponent<Image>();
        startImage.sprite = sprite;
        startImage.SetNativeSize();
        startImage.GetComponent<RectTransform>().localScale = Vector3.one * scale;
        startImage.raycastTarget = false;
        
        return startImage;
    }

    private void _loadDungeon()
    {
        if (null != mText)
        {
            DungeonID id = new DungeonID(GameClient.BattleDataManager.GetInstance().BattleInfo.dungeonId);
           
            string diff = id.prestoryID > 0 ? "" : GameClient.ChapterUtility.GetHardString(id.diffID);

            //特殊处理，堕落深渊只有王者，现在进去难度显示错误，因为配的地下城ID是普通难度当成王者难度来使用
            if (ActivityDungeonDataManager.GetInstance()._judgeDungeonIDIsRotteneterHell(id.dungeonID))
            {
                diff = "王者";
            }

            var table = TableManager.instance.GetTableItem<ProtoTable.DungeonTable>(GameClient.BattleDataManager.GetInstance().BattleInfo.dungeonId);
            if (null != table)
            {
                if (table.SubType == ProtoTable.DungeonTable.eSubType.S_CITYMONSTER || table.SubType == ProtoTable.DungeonTable.eSubType.S_GUILD_DUNGEON)
                {
                    diff = "";
                }
                else if(table.SubType == ProtoTable.DungeonTable.eSubType.S_WEEK_HELL || table.SubType == ProtoTable.DungeonTable.eSubType.S_WEEK_HELL_PER ||
                    table.SubType == ProtoTable.DungeonTable.eSubType.S_ANNIVERSARY_HARD || table.SubType == ProtoTable.DungeonTable.eSubType.S_ANNIVERSARY_HARD)
                {
                    diff = "王者";
                }
                else if (table.SubType == ProtoTable.DungeonTable.eSubType.S_RAID_DUNGEON)
                {
                    int leftValue = GameClient.BattleDataManager.GetInstance().BattleInfo.dungeonId / 1000;
                    int realValue = leftValue % 10;
                    if (realValue == 1)
                    {
                        diff = "噩梦";
                    }
                }
            }
            mText.text = string.Format("<color=#14c5ff>{0}</color>{1}", diff, mDungeonData.GetName());
        }

        mDungeonGraphicInfoList = new DungeonGraphicInfo[mDungeonData.GetAreaConnectListLength()];
        for (int i = 0; i < mDungeonData.GetAreaConnectListLength(); ++i)
        {
            var item = mDungeonData.GetAreaConnectList(i);
            var dgi = mDungeonGraphicInfoList[i];

            dgi.id = item.GetId();
            dgi.x = item.GetPositionX(); dgi.y = item.GetPositionY();
            dgi.state = DungeonGraphicInfo.State.DS_CLOSE;
            dgi.visited = false;
            dgi.type = eDungeonGraphicType.Normal;
            if (item != null)
            {
                if (item.IsStart())
                {
                    dgi.state = DungeonGraphicInfo.State.DS_IN;
                    dgi.visited = true;
                    dgi.type = eDungeonGraphicType.Start;
                }

                if (item.IsBoss())
                {
                    dgi.type = eDungeonGraphicType.Boss;
                }
                if (item.IsEgg())
                {
                    dgi.type = eDungeonGraphicType.Egg;
                }
            }

            mDungeonGraphicInfoList[i] = dgi;
            //如果只显示小地图的话甚至于说大地图都不需要构建UI
            //只构建大地图数据,小地图UI随大地图数据显示
            //CreateMapUnit(i, j, mRoot, ref dgi, itemId);

        }
    }

    private void _initMapRoot()
    {
        int smallMapLenght = (int)mViewPortData.x * (int)mViewPortData.y;
        mSmallMapGraphicInfoList = new DungeonGraphicInfo[smallMapLenght];
        for (int i = 0; i < smallMapLenght; ++i)
        {
            int x = i % (int)mViewPortData.x;
            int y = i / (int)mViewPortData.x;
            var dgi = mSmallMapGraphicInfoList[i];
            CreateMapUnit(x + 1, y + 1, mRoot, ref dgi, i);
            mSmallMapGraphicInfoList[i] = dgi;
        }
    }

    /// <summary>
    /// 创建小地图单元格
    /// </summary>
    private void CreateMapUnit(int posX, int posY, GameObject root, ref DungeonGraphicInfo dgi, int id = 0)
    {
        var obj = new GameObject(_getMapItemName(posX, posY), typeof(Image));
        var fg = new GameObject(_getMapItemName(posX, posY), typeof(Image));

        var x = posX;
        var y = mViewPortData.y - (posY + 1);

        Utility.AttachTo(fg, obj);
        Utility.AttachTo(obj, root);

        var rect = obj.GetComponent<RectTransform>();
        rect.pivot = Vector2.zero;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;

        Vector2 tempFixedOffset = new Vector2(1, -1);
        rect.offsetMin = new Vector2(x * mSizeUnit + mDeltaFix.x + 2, y * mSizeUnit + mDeltaFix.y + 2) - tempFixedOffset * mSizeUnit;
        rect.offsetMax = new Vector2(x * mSizeUnit + mSizeUnit + mDeltaFix.x - 2, y * mSizeUnit + mSizeUnit + mDeltaFix.y - 2) - tempFixedOffset * mSizeUnit;

        var fgrect = fg.GetComponent<RectTransform>();
        fgrect.pivot = Vector2.one * 0.5f;
        fgrect.anchorMin = Vector2.zero;
        fgrect.anchorMax = Vector2.one;
        fgrect.offsetMin = Vector2.zero;
        fgrect.offsetMax = Vector2.zero;

        var imageItemBoard = obj.GetComponent<Image>();
        imageItemBoard.sprite = mItemBlankImage;
        imageItemBoard.type = Image.Type.Simple;

        var imageCom = fg.GetComponent<Image>();
        imageCom.type = Image.Type.Simple;

        dgi.image = imageCom;
        dgi.imageFg = _createTag("imageFg", null, obj, 1.0f);
        dgi.imageArrow = _createArrow("imageArrow", mArrowRightSprite, obj, 0.5f);
        dgi.imageBoard = imageItemBoard;
        dgi.x = posX;
        dgi.y = posY;
        dgi.state = DungeonGraphicInfo.State.DS_CLOSE;
        dgi.visited = false;
        dgi.id = id;
        dgi.type = eDungeonGraphicType.Normal;

        //dgi.imageBoard.enabled = false;
        dgi.imageFg.enabled = false;
        dgi.imageArrow.enabled = false;
        dgi.image.enabled = false;
        dgi.image.sprite = mImageList[0];
        //dgi.image.material = mImageMaterial;
    }

    public void SetEggRoomState(bool flag)
    {
        if (eggRoom != null)
        {
            eggRoom.CustomActive(flag);
        }
    }


    public void SetDropProgress(uint index)
    {
        if (mDungeonGraphicInfoList == null)
            return;

        for (int i = 0; i < mDungeonGraphicInfoList.Length - 1; i++)
        {
            var flag = (index & 1) == 0;
            index >>= 1;

            var dgi = mDungeonGraphicInfoList[i];
            if (flag)
            {
                dgi.type = eDungeonGraphicType.Chest;
            }
            else
            {
                dgi.type = eDungeonGraphicType.Normal;
            }
            mDungeonGraphicInfoList[i] = dgi;
        }
    }

    private Protocol.DungeonHellMode mHellMode;

    public void SetHell(Protocol.DungeonHellMode mode, int id)
    {
        if(mode == Protocol.DungeonHellMode.Null || id == -1)
        {
            return;
        }
        mHellMode = mode;

        for (int i = 0; i < mDungeonGraphicInfoList.Length; ++i)
        {
            var dgi = mDungeonGraphicInfoList[i];

            if (dgi.id == id)
            {
                dgi.type               	   = eDungeonGraphicType.Hell;
				mDungeonGraphicInfoList[i] = dgi;
                break;
            }
        }
    }

    public void InitTreasureMapDungeonUI()
    {
        int mWidth = 5;
        int mHeight = 5;
        ResizeMap(mWidth, mHeight);
    }
    public void ResizeMap(int width, int height)
    {
        if (gameObject.IsNull()) return;
        var rect = gameObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width * mSizeUnit + mDeltaFix.x * 2, height * mSizeUnit + mDeltaFix.y * 2);
        if (mTextRoot.IsNull()) return;
        mTextRoot.SetActive(false);
    }

    private class Range
    {
        int min;
        int max;
    }

    private Vector2 mFixedOffset = Vector2.zero;
    private Vector2 mFixedSize = Vector2.zero;

    public Vector2 mViewPortData = new Vector2(4, 3);//小地图宽高

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

        int realWeidth = maxX - minX + 1;
        int realHeight = maxY - minY + 1;

        mFixedSize   = new Vector2(realWeidth, realHeight);
        mFixedOffset = new Vector2(minX, height - maxY - 1);
    }

    public void SetDungeonData(IDungeonData scene, GetNextRoomIdDel getNextRoomIdDel = null)
    {
        mDungeonData = scene;
        mGetNextRoomIdDel = getNextRoomIdDel;
        
        _fixDungeonData(scene);

        _initBackground();
        _loadDungeon();
        _initMapRoot();

        _initData();

        //_updateText();
    }

    void OnDestroy()
    {
        if (null != mDungeonData)
        {
            for (int i = 0; i < mDungeonData.GetAreaConnectListLength(); ++i)
            {
                //mDungeonData.GetAreaConnectList(i).SetSceneData(null);
            }
            mDungeonData = null;
        }

        if (mGetNextRoomIdDel != null)
            mGetNextRoomIdDel = null;
    }

    //private void _updateText()
    //{
    //    if (null != mTextGold)
    //    {
    //        mTextGold.text = "0";
    //    }

    //    if (null != mTextBox)
    //    {
    //        mTextBox.text = "0";
    //    }
    //}


    public void UpdateDungoenGraphicInfo()
    {
        mDungoenBlinkList.Clear();
        
        for(int i = 0; i < mSmallMapGraphicInfoList.Length; ++i)
        {
            var current = mSmallMapGraphicInfoList[i];
        
            switch (current.state)
            {
                // case DungeonGraphicInfo.State.DS_IN:
                //     {
                //         if (current.image)
                //         {
                //             current.image.color = Color.white;
                //         }
                //         if (current.imageBoard)
                //         {
                //             current.imageBoard.color = Color.white;
                //         }
                //     }
                //     break;
                //
                // case DungeonGraphicInfo.State.DS_CLOSE:
                //     {
                //         if (current.image)
                //         {
                //             current.image.color = mDarkColor;
                //         }
                //         if (current.imageBoard)
                //         {
                //             current.imageBoard.color = mDarkColor;
                //         }
                //     }
                //     break;
        
                case DungeonGraphicInfo.State.DS_OPEN:
                    {
                        if (current.image)
                        {
                            mDungoenBlinkList.Add(current.image);
                        }
                        if (current.imageBoard)
                        {
                            mDungoenBlinkList.Add(current.imageBoard);
                        }
                    }
                    break;
            }
        }

        if (mArrowBlink != null && mArrowBlinkParent != null)
        {
            mArrowBlink.transform.SetParent(mArrowBlinkParent);
        }
        mArrowBlink = null;
        mArrowBlinkParent = null;
        if (mDungoenBlinkList.Count > 0)
        {
            for (int i = 0; i < mSmallMapGraphicInfoList.Length; ++i)
            {
                var current = mSmallMapGraphicInfoList[i];
                if (current.state == DungeonGraphicInfo.State.DS_IN)
                {
                    if (current.imageArrow != null)
                    {
                        current.imageArrow.enabled = true;
                        mArrowBlink = current.imageArrow;
                        mArrowBlinkParent = current.imageArrow.transform.parent;
                        mArrowBlink.transform.SetParent(mRoot.transform);
                        break;
                    }
                }
            }
        }

        //for (int i = 0; i < mDungeonGraphicInfoList.Length; ++i)
        //{
        //    var current = mDungeonGraphicInfoList[i];

        //    switch (current.state)
        //    {
        //        case DungeonGraphicInfo.State.DS_IN:
        //            {
        //                if (current.image)
        //                {
        //                    current.image.color = Color.white;
        //                }
        //                if (current.imageBoard)
        //                {
        //                    current.imageBoard.color = Color.white;
        //                }
        //            }
        //            break;

        //        case DungeonGraphicInfo.State.DS_CLOSE:
        //            {
        //                if (current.image)
        //                {
        //                    current.image.color = mDarkColor;
        //                }
        //                if (current.imageBoard)
        //                {
        //                    current.imageBoard.color = mDarkColor;
        //                }
        //            }
        //            break;

        //        case DungeonGraphicInfo.State.DS_OPEN:
        //            {
        //                if (current.image)
        //                {
        //                    mDungoenBlinkList.Add(current.image);
        //                }
        //                if (current.imageBoard)
        //                {
        //                    mDungoenBlinkList.Add(current.imageBoard);
        //                }
        //            }
        //            break;
        //    }

        //}
    }

    private void _initData()
    {
        mGlodCount = 0;
        mBoxCount = 0;
    }

    //public void AddGlod(int glod)
    //{
    //    mGlodCount += glod;
    //    if (mTextGold != null)
    //    {
    //        mTextGold.text = mGlodCount.ToString();
    //    }
    //    if(mTextGoldRoot != null)
    //    {
    //        var effct = AssetLoader.instance.LoadResAsGameObject("Effects/Scene_effects/EffectUI/EffUI_huoqu_guo");
    //        Utility.AttachTo(effct, mTextGoldRoot);
    //    }
    //}

    //public void AddBox(int box)
    //{
    //    mBoxCount += box;
    //    if(mTextBox != null)
    //     mTextBox.text = mBoxCount.ToString();
    //    if(mTextBoxRoot != null)
    //    {
    //        var effct = AssetLoader.instance.LoadResAsGameObject("Effects/Scene_effects/EffectUI/EffUI_huoqu_guo");
    //        Utility.AttachTo(effct, mTextBoxRoot);
    //    }
    //}

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

    private DungeonGraphicInfo _getDungeonGraphicInfo(int x, int y)
    {
        for (int i = 0; i < mDungeonGraphicInfoList.Length; ++i)
        {
            if (mDungeonGraphicInfoList[i].x == x && mDungeonGraphicInfoList[i].y == y)
            {
                return mDungeonGraphicInfoList[i];
            }
        }

        return new DungeonGraphicInfo();
    }

    public void SetStartPosition(int x, int y)
    {
        for (int i = 0; i < mDungeonGraphicInfoList.Length; ++i)
        {
            var cur = mDungeonGraphicInfoList[i];
            cur.state = DungeonGraphicInfo.State.DS_CLOSE;

            if (cur.x == x && cur.y == y)
            {
                cur.state = DungeonGraphicInfo.State.DS_IN;
                cur.visited = true;
            }
            mDungeonGraphicInfoList[i] = cur;
        }

        mCurrentX = x;
        mCurrentY = y;
    }

    public void SetOpenPosition(int x, int y)
    {
        for(int i = 0; i < mDungeonGraphicInfoList.Length; ++i)
        {      
             var cur = mDungeonGraphicInfoList[i];
             var state = DungeonGraphicInfo.State.DS_CLOSE; 
             if(cur.x == x && cur.y == y)
             {
                 state = DungeonGraphicInfo.State.DS_IN;   
             }
             cur.state = state;
             mDungeonGraphicInfoList[i] = cur;
        }
        ChangeLinkMapState(x, y, DungeonGraphicInfo.State.DS_OPEN);
    }

    public void ChangeLinkMapState(int x,int y,DungeonGraphicInfo.State state)
    {
        var index = Array.FindIndex(mDungeonGraphicInfoList,v=>{return v.x == x && v.y == y;});

        if(index >= 0)
        {
            var item = mDungeonData.GetAreaConnectList(index);

            for(int i = 0; i < 4; ++i)
            {
                if(item.GetIsConnect(i))
                {
                    int tempIndex;
                    mDungeonData.GetSideByType(x,y,(TransportDoorType)i,out tempIndex);

                    if(tempIndex >= 0)
                    {
                        var cur = mDungeonGraphicInfoList[tempIndex];
                        cur.state = state;
                        mDungeonGraphicInfoList[tempIndex] = cur;
                    }
                }
            } 
        }
    }

    public bool mIsPlay = true;
    void Update()
    {
#if UNITY_EDITOR
        if (mIsPlay && !Application.isPlaying)
        {
            SetDungeonData(mDungeonData);
            //_initBackground();
            //_loadDungeon();
        }
#endif

        /*if(mDungoenBlinkList.Count > 0)
        {
            float  t = Mathf.Repeat(Time.realtimeSinceStartup,mBlinkTime) / mBlinkTime;
            float  v = mBlinkCurve.Evaluate(t);
            Color  c = Color.Lerp(Color.white,mDarkColor,v);

            for(int i = 0; i < mDungoenBlinkList.Count; ++i)
            {
                var current = mDungoenBlinkList[i];
                current.color = c;
            }
        }*/
        
        if(mArrowBlink != null)
        {
            float  t = Mathf.Repeat(Time.realtimeSinceStartup,mBlinkTime) / mBlinkTime;
            float  v = mBlinkCurve.Evaluate(t);
            Color  c = Color.Lerp(Color.white,mDarkColor,v);
            mArrowBlink.color = c;
        }
    }

    private int GetNextRoomId()
    {
        if (mGetNextRoomIdDel != null)
            return mGetNextRoomIdDel();
        return -1;
    }
}
