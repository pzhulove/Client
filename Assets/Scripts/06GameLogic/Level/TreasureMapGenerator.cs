//CreateTime 2019-11-15
//Created by shensi
//Description:
//生成宝藏关卡算法
//构建每个不同位置点可以连通的方向矢量 （连通图） 
//先随机出 出生点 通过构建的连通图进行深度遍历(DFS) 如果还有待分配的房间，进行一次无随机连通信息生成房间来补正
//构建好线路图以后，再依据游戏规则，按顺序随机生成魔王点，钥匙点，地图道具点，宝藏点，宝箱点，试炼房间点(剩余的必然是杂兵点)

using System;
using System.Collections.Generic;
static public class TreasureMapGenerator
{
#if UNITY_EDITOR
    public static sbyte DropItemCount
    {
        get { return dropItemCount; }
    }
    public static sbyte TrialRoomCount
    {
        get { return trialRoomCount; }
    }
    public static sbyte RoomCount
    {
        get { return roomCount; }
    }
#endif
    public enum POS_TYPE
    {
        LEFT_TOP_CORNER = 0,
        LEFT_CORNER,
        LEFT_BOTTOM_CORNER,
        RIGHT_TOP_CORNER,
        RIGHT_CORNER,
        RIGHT_BOTTOM_CORNER,
        TOP_CORNER,
        BOTTOM_CORNER,
        NORMAL,
        MAX_COUNT
    };
    public enum ROOM_TYPE
    {
        NONE = 0,
        NORMAL_ROOM = 1, //普通房间
        BOSS_ROOM, //大魔王房间
        END_ROOM, //大宝箱房间
        KEY_ROOM, //钥匙房间
        MAP_ROOM, //地图道具房间
        DROPITEM_ROOM, //小宝箱房间
        TRIAL_ROOM,  //试炼房间
        BORN_ROOM,//出生点房间
        MAX_COUNT,
    }
    static readonly byte[] doorCountTemplate = new byte[(int)(POS_TYPE.MAX_COUNT)] //不同的点位门数量
    {
        2,3,2,2,3,2,3,3,4
    };
    public class Direction2
    {
        public sbyte x = 0;
        public sbyte y = 0;
    }
    class RouterInfo
    {
        public Direction2 pos = new Direction2();
        public byte randDoorCount = 0;
        public byte startIndex = 0;
        public byte conflictIndex = 0;
        public byte curDoorCount = 0;
    };

    public class TMStack<T> where T : class
    {
        private T[] _array = null;
        private int _count = 0;

        public TMStack(int capacity)
        {
            _array = new T[capacity];
        }

        public TMStack(T[] array)
        {
            _array = new T[array.Length];
            foreach (T item in array)
            {
                Push(item);
            }
        }

        public void Push(T item)
        {
            if (_array.Length <= _count)
            {
                Array.Resize(ref _array, _count * 2);
            }

            _array[_count] = item;
            _count++;
        }

        public void Clear()
        {
            _count = 0;
        }

        public bool Contains(T item)
        {
            if (_array != null)
            {
                for (int index = 0; index < _count; ++index)
                {
                    if (item == _array[index])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public T Peek()
        {
            if (_count == 0)
            {
                return default(T);
            }
            return _array[_count - 1];
        }
        public T Pop()
        {
            if (_count == 0)
            {
                return default(T);
            }

            if (_count == 1)
            {
                _count = 0;
                return _array[0];
            }

            T pop = _array[_count - 1];
            _count--;
            return pop;
        }

        public int Count
        {
            get { return _count; }
        }
    }

    //四方向邻接表
    public class LinkInfo
    {
        private Direction2[] linkPos = new Direction2[4] { new Direction2(), new Direction2(), new Direction2(), new Direction2() };
        private int linkCount = 0;
        public bool AddLink(int posX, int posY)
        {
            if (linkCount >= 4)
            {
                //   Console.WriteLine("link out of range");
                return false;
            }
            if (HasLink(posX, posY))
            {
                // Console.WriteLine("link the same pos {0} {1}", posX, posY);
                return false;
            }
            linkPos[linkCount].x = (sbyte)posX;
            linkPos[linkCount].y = (sbyte)posY;
            linkCount++;
            return true;
        }
        public bool HasLink(int posX, int posY)
        {
            for (int i = 0; i < linkCount; i++)
            {
                if (linkPos[i].x == posX && linkPos[i].y == posY)
                {
                    return true;
                }
            }
            return false;
        }
        public void Reset()
        {
            linkCount = 0;
        }
        public int GetCount()
        {
            return linkCount;
        }
        public Direction2 GetLink(int index)
        {
            if (index < 0 || index >= linkCount) return null;
            return linkPos[index];
        }

    }

    //连通图 四方向
    static public readonly Dictionary<int, List<Direction2>> doorDir = new Dictionary<int, List<Direction2>>
    {
        { (int)POS_TYPE.LEFT_TOP_CORNER, new List<Direction2>{ new Direction2 { x = 1, y = 0 }, new Direction2 { x = 0, y = 1} } },//左上
        { (int)POS_TYPE.LEFT_CORNER, new List<Direction2>{ new Direction2 { x =1, y = 0 }, new Direction2 { x =-1, y = 0}, new Direction2 { x =0, y = 1} } },//左
        { (int)POS_TYPE.LEFT_BOTTOM_CORNER, new List<Direction2>{ new Direction2 { x =-1, y = 0}, new Direction2 { x = 0, y = 1 } } },//左下
        { (int)POS_TYPE.RIGHT_TOP_CORNER, new List<Direction2>{new Direction2 { x =1, y = 0}, new Direction2 { x = 0, y = -1 } } },//右上
        { (int)POS_TYPE.RIGHT_CORNER,new List<Direction2>{new Direction2 { x = -1, y = 0} ,new Direction2 { x = 0, y = -1 }, new Direction2 { x = 1, y = 0 } } },//右
        { (int)POS_TYPE.RIGHT_BOTTOM_CORNER,new List<Direction2>{new Direction2 { x = -1, y = 0},new Direction2 { x = 0, y = -1 } } },//右下
        { (int)POS_TYPE.TOP_CORNER,new List<Direction2>{new Direction2 { x = 1, y = 0 },new Direction2 { x = 0, y = 1 },new Direction2 { x = 0, y = -1 } } },//上
        { (int)POS_TYPE.BOTTOM_CORNER,new List<Direction2>{new Direction2 { x = -1, y = 0 },new Direction2 { x = 0, y = -1 },new Direction2 { x = 0, y = 1 } } },//下
        { (int)POS_TYPE.NORMAL, new List<Direction2>{new Direction2 { x = -1, y = 0 },new Direction2 { x = 0, y = -1 },new Direction2 { x = 1, y = 0 },new Direction2 { x = 0, y = 1 } } },//居中
    };
    //连通图 八方向
    static public readonly Dictionary<int, List<Direction2>> doorCrossDir = new Dictionary<int, List<Direction2>> //八方向
    {
        { (int)POS_TYPE.LEFT_TOP_CORNER, new List<Direction2>{ new Direction2 { x = 1, y = 0 }, new Direction2 { x = 0, y = 1} ,new Direction2 { x = 1,y = 1} } },//左上
        { (int)POS_TYPE.LEFT_CORNER, new List<Direction2>{ new Direction2 { x =1, y = 0 }, new Direction2 { x =-1, y = 0}, new Direction2 { x =0, y = 1},new Direction2 { x = -1,y = 1},new Direction2 { x = 1,y = 1} } },//左
        { (int)POS_TYPE.LEFT_BOTTOM_CORNER, new List<Direction2>{ new Direction2 { x =-1, y = 0}, new Direction2 { x = 0, y = 1 },new Direction2 { x = -1,y = 1} } },//左下
        { (int)POS_TYPE.RIGHT_TOP_CORNER, new List<Direction2>{new Direction2 { x =1, y = 0}, new Direction2 { x = 0, y = -1 },new Direction2 { x = 1,y = -1} } },//右上
        { (int)POS_TYPE.RIGHT_CORNER,new List<Direction2>{new Direction2 { x = -1, y = 0} ,new Direction2 { x = 0, y = -1 }, new Direction2 { x = 1, y = 0 },new Direction2 { x = -1,y = -1},new Direction2 { x = 1,y = -1} } },//右
        { (int)POS_TYPE.RIGHT_BOTTOM_CORNER,new List<Direction2>{new Direction2 { x = -1, y = 0},new Direction2 { x = 0, y = -1 },new Direction2 { x = -1,y = -1} } },//右下
        { (int)POS_TYPE.TOP_CORNER,new List<Direction2>{new Direction2 { x = 1, y = 0 },new Direction2 { x = 0, y = 1 },new Direction2 { x = 0, y = -1 } ,new Direction2 { x = 1,y = -1},new Direction2 { x = 1,y = 1 } } },//上
        { (int)POS_TYPE.BOTTOM_CORNER,new List<Direction2>{new Direction2 { x = -1, y = 0 },new Direction2 { x = 0, y = -1 },new Direction2 { x = 0, y = 1 },new Direction2 { x = -1,y = -1},new Direction2 { x = -1,y = 1} } },//下
        { (int)POS_TYPE.NORMAL, new List<Direction2>{new Direction2 { x = -1, y = 0 },new Direction2 { x = 0, y = -1 },new Direction2 { x = 1, y = 0 },new Direction2 { x = 0, y = 1 },new Direction2 { x = -1,y = -1 },new Direction2 { x = -1,y = 1},new Direction2 { x = 1,y = -1},new Direction2 { x = 1,y = 1} } },//居中
    };

    static TMStack<RouterInfo> travelRouter = new TMStack<RouterInfo>(20); //非递归方式实现深度优先，这里保存的栈信息 （优化为定长方式 减少gc）
    static RouterInfo[] routerBuffer = new RouterInfo[50];         //栈缓存
    static int routerUsedCount = 0;                                 //已用的缓存数量
    static bool outOfRouterBuffer = false;                          //是否栈缓存已经用完

    static public readonly byte MAX_ROW = 6;
    static public readonly byte MAX_COL = 8;

    static byte[,] roomTypes = new byte[MAX_ROW, MAX_COL];        //房间类型数据（元素内容为房间类型）
    static LinkInfo[,] linkInfo = new LinkInfo[MAX_ROW, MAX_COL]; //房间邻接信息
    static bool[] visitedRoom = new bool[MAX_ROW * MAX_COL];     //连通性检测时用到的已访问房间标记
    static sbyte visitedRoomCount = 0;                            //连通性检测时已经遍历的房间数量

    static sbyte roomCount = 30;                             //待随机生成的房间数量
    static sbyte leftRoomCount = 30;                         //剩余还未生成的房间数量
    static sbyte bornPosX = 0;                                //出生点房间位置
    static sbyte bornPosY = 0;

    static sbyte dropItemCount = 4;                           //待随机生成的宝箱房间数量
    static sbyte canPutDropItemCount = 0;                     //还能设置为宝箱房间随机库数量
    static sbyte realGenDropItemCount = 0;                    //已生成的宝箱房间数量

    static sbyte trialRoomCount = 7;                          //待生成的试炼房间数量

    static byte[] randRoomIndexSet = null;                  //已生成的房间还未定义类型随机库集合（元素内容为位置点对应的二维索引）
    static sbyte randRoomCount = 0;                           //已生成房间还未定义类型的房间数量
    static string errorInfo = string.Empty;                 //生成出错信息

    static FrameRandomImp randomInst = null;
    static uint initRandomSeed = 0;
    //从缓存池里面获得新的栈实例
    static RouterInfo  GenRouterInfo()
    {
        if (outOfRouterBuffer)
        {
            Logger.LogErrorFormat("router buffer is out of range seed {0} {1}", randomInst.GetSeed(), initRandomSeed);
            return new RouterInfo();
        }
        if (routerUsedCount >= routerBuffer.Length)
        {
            outOfRouterBuffer = true;
            Logger.LogErrorFormat("router buffer is out of range seed {0} {1}",randomInst.GetSeed(), initRandomSeed);
            var newRouter = new RouterInfo();
            return newRouter; //最差的情况
        }
        int curIndex = routerUsedCount;
        if (routerBuffer[curIndex] == null)
        {
            routerBuffer[curIndex] = new RouterInfo();
        }
        routerUsedCount++;
        return routerBuffer[curIndex];
    }
    //放入缓存池
    static void PushRouterInfo(RouterInfo info)
    {
        if (outOfRouterBuffer) return;
        if (routerUsedCount <= 0) return;
        routerUsedCount--;
    }
    //恢复缓存池处于未占用状态
    static void ClearRouterInfo()
    {
        routerUsedCount = 0;
        outOfRouterBuffer = false;
    }
    //获得游戏坐标的方位类型
    static public POS_TYPE GetPosTypeInGameCoord(int x,int y)
    {
        return GetPosType(y,x);
    }
    static public POS_TYPE GetPosType(int x, int y)
    {
        if (x == 0 && y == 0)
        {
            return POS_TYPE.LEFT_TOP_CORNER;
        }
        else if (x == MAX_ROW - 1 && y == 0)
        {
            return POS_TYPE.LEFT_BOTTOM_CORNER;
        }
        else if (x == 0 && y == MAX_COL - 1)
        {
            return POS_TYPE.RIGHT_TOP_CORNER;
        }
        else if (x == MAX_ROW - 1 && y == MAX_COL - 1)
        {
            return POS_TYPE.RIGHT_BOTTOM_CORNER;
        }
        else if (x == 0)
        {
            return POS_TYPE.TOP_CORNER;
        }
        else if (x == MAX_ROW - 1)
        {
            return POS_TYPE.BOTTOM_CORNER;
        }
        else if (y == 0)
        {
            return POS_TYPE.LEFT_CORNER;
        }
        else if (y == MAX_COL - 1)
        {
            return POS_TYPE.RIGHT_CORNER;
        }
        return POS_TYPE.NORMAL;
    }
    #region 以下段为不建议在游戏中使用 这里给的只是算法范例
    //获得游戏坐标可以行走点 (该接口不能在游戏中使用 ps 用的是游戏静态变量表示，会和验证不同步)
    static public bool GetNextLinkPosInGameCoord(int x,int y,ref int nextX, ref int nextY,int dirIndex)
    {
        var posType = GetPosTypeInGameCoord(x, y);
        int linkIndex = 0;
        bool ret = GetLinkPos(y,x,ref nextY,ref nextX,posType,false,dirIndex,ref linkIndex);
        if(ret)
        {
            return roomTypes[y, x] != 0;
        }
        return ret;
    }
    //游戏坐标是否在boss房间范围内
    static public bool isInBossRangeInGameCoord(int x,int y,int bossX,int bossY)
    {
        var posType = GetPosTypeInGameCoord(x, y);
        int linkIndex = 0;
        for (int dir = 0; dir < 8;dir++)
        {
            int nextX = 0;
            int nextY = 0;
            if(GetCrossLinkPos(y,x,ref nextY,ref nextX, posType,false,dir,ref linkIndex))
            {
                if(nextX == bossX && nextY == bossY)
                {
                    return true;
                }
            }
            else
            {
                break;
            }
        }
        return false;
        
    }
    #endregion
    //获得x，y位置点相邻房间位置 四方向
    static public bool GetLinkPos(int x, int y, ref int nextX, ref int nextY, POS_TYPE posType, bool needRand, int tryCount, ref int linkIndex)
    {
        if (doorDir.ContainsKey((int)posType))
        {
            var linkList = doorDir[(int)posType];
            if (needRand)
            {
                linkIndex = randomInst.InRange(0, linkList.Count);
            }
            else
            {
                linkIndex = tryCount;
            }
            if (linkIndex >= linkList.Count) return false;
            var dir = linkList[linkIndex];
            nextX = x + dir.x;
            nextY = y + dir.y;
            return true;
        }
        return false;
    }
    //获得x，y位置点相邻房间位置 八方向
    static public bool GetCrossLinkPos(int x, int y, ref int nextX, ref int nextY, POS_TYPE posType, bool needRand, int tryCount, ref int linkIndex)
    {
        if (doorCrossDir.ContainsKey((int)posType))
        {
            var linkList = doorCrossDir[(int)posType];
            if (needRand)
            {
                linkIndex = randomInst.InRange(0, linkList.Count);
            }
            else
            {
                linkIndex = tryCount;
            }
            if (linkIndex >= linkList.Count) return false;
            var linkdir = linkList[linkIndex];
            nextX = x + linkdir.x;
            nextY = y + linkdir.y;
            return true;
        }
        return false;
    }
    //在指定的位置处生成一个房间
    static void AddRoom(int x,int y)
    {
        //该位置点出还没有生成房间
        leftRoomCount--;
        roomTypes[x, y] = 1;
        randRoomIndexSet[randRoomCount] = (byte)(x * MAX_COL + y); //已生成的房间放入随机库
        randRoomCount++;
    }
    static void AddLink(int x,int y,int nextX,int nextY)
    {
        linkInfo[nextX, nextY].AddLink(x, y);
        linkInfo[x, y].AddLink(nextX, nextY);
    }
    //生成基本房间线路图
    static bool GenerateLinkRoom(int x, int y)
    {
        if (leftRoomCount == 0) return false;
        var posType = GetPosType(x, y);
        int maxDoorCount = doorCountTemplate[(int)(posType)];
        int curDoorCount = 0;
        int randDoorCount = randomInst.InRange(1, maxDoorCount + 1);
        //最大遍历门的次数
        for (int i = 0; i < maxDoorCount && leftRoomCount > 0; i++)
        {
            int nextX = x;
            int nextY = y;
            int tryCount = 0;
            int linkIndex = 0;
            GetLinkPos(x, y, ref nextX, ref nextY, posType, true, tryCount, ref linkIndex);
            if (roomTypes[nextX, nextY] == 0) 
            {
                AddRoom(nextX,nextY);
                AddLink(x,y, nextX, nextY);
                curDoorCount++;
                GenerateLinkRoom(nextX, nextY);
                if (curDoorCount >= randDoorCount)
                {
                    return true;//当有效的门数量达到随机出来的最大门数量时则生成线路图结束
                }
            }
            else
            {
                bool find = false;
                //当前已生成房间了，那么在该点四方向搜索可行的通路点，并在通路点上生成房间
                for (int j = 0; j < 4; j++)
                {
                    linkIndex = j;
                    tryCount = j;
                    if (!GetLinkPos(x, y, ref nextX, ref nextY, posType, false, tryCount, ref linkIndex))
                    {
                        // Console.WriteLine(string.Format("x {0} y {1} posType {2} tryCount {3} linkIndex {4} out of Range",x,y,posType,tryCount,linkIndex));
                        break;
                    }
                    if (roomTypes[nextX, nextY] == 0)
                    {
                        AddRoom(nextX,nextY);
                        AddLink(x, y, nextX, nextY);
                        curDoorCount++;
                        GenerateLinkRoom(nextX, nextY);
                        find = true;
                        if (curDoorCount >= randDoorCount) return true;//当有效的门数量达到随机出来的最大门数量时则生成线路图结束
                        break;
                    }
                    else
                    {
                        //额外的连接信息，做一个随机 (使该房间的领接房间数量有一定的变化)
                        bool canAddLink = false;
                        if (randDoorCount < 3 && linkInfo[nextX, nextY].GetCount() < 3)
                        {
                            canAddLink = true;
                        }
                        else if (randomInst.InRange(0, 101) >= 35)
                        {
                            canAddLink = true;
                        }
                        if (canAddLink)
                        {
                            AddLink(x, y, nextX, nextY);
                        }
                    }

                }
                if (!find)
                {
                    // Console.WriteLine(string.Format("x {0} y {1} posType {2} tryCount {3} linkIndex {4} Failed", x, y, posType, tryCount, linkIndex));
                }
            }
        }
        return true;
    }

    //生成基本房间线路图 非递归(基本没有GC)
    static bool GenerateLinkRoom_2(int x, int y)
    {
        travelRouter.Clear();
        ClearRouterInfo();
        if (leftRoomCount == 0) return false;
        var curPosType = GetPosType(x, y);
        int maxDoorCount = doorCountTemplate[(int)(curPosType)];
        int curDoorCount = 0;
        int randDoorCount = randomInst.InRange(1, maxDoorCount + 1);
        var router = GenRouterInfo();
        router.randDoorCount = (byte)randDoorCount;
        router.pos.x = (sbyte)x;
        router.pos.y = (sbyte)y;
        router.startIndex = 0;
        router.curDoorCount = 0;
        router.conflictIndex = 0;
        travelRouter.Push(router);
        while (travelRouter.Count > 0)
        {
            var curStack = travelRouter.Peek();
            x = curStack.pos.x;
            y = curStack.pos.y;
            curPosType = GetPosType(x,y);
            randDoorCount = curStack.randDoorCount;
            maxDoorCount = doorCountTemplate[(int)(curPosType)];
            curDoorCount = curStack.curDoorCount;
            //Logger.LogErrorFormat("Peek Stack cur {0} {1} curstack startIndex {2} randDoorCount {3} conflictIndex {4} curdoorCount {5}",
            //                                       x, y, curStack.startIndex, curStack.randDoorCount, curStack.confictIndex, curStack.curDoorCount);
            bool needRecurse = false;
            //最大遍历门的次数
            for (int i = curStack.startIndex; i < maxDoorCount && leftRoomCount > 0 && curDoorCount < randDoorCount; i++)
            {
                int nextX = x;
                int nextY = y;
                int tryCount = 0;
                int linkIndex = 0;
                GetLinkPos(x, y, ref nextX, ref nextY, curPosType, true, tryCount, ref linkIndex);
                if (nextX < 0 || nextX >= MAX_ROW || nextY < 0 || nextY >= MAX_COL)
                {
                    Logger.LogErrorFormat("wrong stack info : x {0} y {1} nextX {2} nextY {3} curPosType {4} index {5} seed {6} {7}", x, y, nextX, nextY, curPosType, i,randomInst.GetSeed(), initRandomSeed);
                    return false;
                }
                int roomIndex = nextX * MAX_COL + nextY;
                if (roomTypes[nextX, nextY] == 0)
                {
                    AddRoom(nextX, nextY);
                    AddLink(x, y, nextX, nextY);
                    curDoorCount++;
                    if (leftRoomCount == 0) return true;
                    var nextStack = GenRouterInfo();
                    var nextPosType = GetPosType(nextX,nextY);
                    int nextMaxDoorCount = doorCountTemplate[(int)(nextPosType)];
                    nextStack.randDoorCount = (byte)randomInst.InRange(1, nextMaxDoorCount + 1);
                    nextStack.pos.x = (sbyte)nextX;
                    nextStack.pos.y = (sbyte)nextY;
                    nextStack.startIndex = 0;
                    nextStack.conflictIndex = 0;
                    nextStack.curDoorCount = 0;
                    curStack.startIndex = (byte)(i+1);
                    curStack.conflictIndex = 0;
                    curStack.curDoorCount = (byte)curDoorCount;
                    travelRouter.Push(nextStack);
                    //Logger.LogErrorFormat("Push Stack cur {0} {1} next {2} {3} curstack startIndex {4} randDoorCount {5} conflictIndex {6} curdoorCount {7}",
                    //                               x, y, nextX, nextY, curStack.startIndex, curStack.randDoorCount, curStack.confictIndex, curStack.curDoorCount);
                    needRecurse = true;
                    break;
                }
                else
                {
                    //当前已生成房间了，那么在该点四方向搜索可行的通路点，并在通路点上生成房间
                    bool bFind = false;
                    for (int j = curStack.conflictIndex; j < 4; j++)
                    {
                        linkIndex = j;
                        tryCount = j;
                        if (!GetLinkPos(x, y, ref nextX, ref nextY, curPosType, false, tryCount, ref linkIndex))
                        {
                            // Console.WriteLine(string.Format("x {0} y {1} posType {2} tryCount {3} linkIndex {4} out of Range",x,y,posType,tryCount,linkIndex));
                            curStack.startIndex = (byte)(i+1);
                            curStack.conflictIndex = 0;
                            break;
                        }
                        if (roomTypes[nextX, nextY] == 0)
                        {
                            AddRoom(nextX, nextY);
                            AddLink(x, y, nextX, nextY);
                            curDoorCount++;
                            if (leftRoomCount == 0) return true;
                            var nextStack = GenRouterInfo();
                            var nextPosType = GetPosType(nextX, nextY);
                            int nextMaxDoorCount = doorCountTemplate[(int)(nextPosType)];
                            nextStack.randDoorCount = (byte)randomInst.InRange(1, nextMaxDoorCount + 1);
                            nextStack.pos.x = (sbyte)nextX;
                            nextStack.pos.y = (sbyte)nextY;
                            nextStack.conflictIndex = 0;
                            nextStack.curDoorCount = 0;
                            nextStack.startIndex = 0;
                            travelRouter.Push(nextStack);
                            curStack.startIndex = (byte)(i+1);
                            curStack.conflictIndex = 0;
                            curStack.curDoorCount = (byte)curDoorCount;
                            bFind = true;
                            needRecurse = true;
                            //Logger.LogErrorFormat("Confict Push Stack cur {0} {1} next {2} {3} curstack startIndex {4} randDoorCount {5} conflictIndex {6} curdoorCount {7}", 
                            //                        x, y, nextX, nextY, curStack.startIndex, curStack.randDoorCount, curStack.confictIndex, curStack.curDoorCount);
                            break;
                        }
                        else
                        {
                            //额外的连接信息，做一个随机
                            bool canAddLink = false;
                            if (randDoorCount < 3 && linkInfo[nextX,nextY].GetCount() < 3)
                            {
                                canAddLink = true;
                            }
                            else if (randomInst.InRange(0, 101) >= 35)
                            {
                                canAddLink = true;
                            }
                            if (canAddLink)
                            {
                                AddLink(x, y, nextX, nextY);
                            }
                        }
                    }
                    if(bFind)
                    {
                        break;
                    }
                }
            }
            if(!needRecurse)
            {
              //  Logger.LogErrorFormat("Pop Stack cur {0} {1} startIndex {2} randDoorCount {3} conflictIndex {4} curdoorCount {5}", x, y,curStack.startIndex,curStack.randDoorCount,curStack.confictIndex, curStack.curDoorCount);
                travelRouter.Pop();
                PushRouterInfo(curStack);
            }
        }
        return true;
    }

    //无随机生成房间和线路 深度优先
    static bool TraverseAndGenerateLinkRoom(int x, int y)
    {
        if (leftRoomCount == 0) return true;
        if (roomTypes[x, y] == 0) return false;
        int nextX = 0;
        int nextY = 0;
        var link = linkInfo[x, y];

        var posType = GetPosType(x, y);
        int maxDoorCount = doorCountTemplate[(int)posType];
        for (int dir = 0; dir < 4; dir++)
        {
            int linkIndex = dir;
            if (GetLinkPos(x, y, ref nextX, ref nextY, posType, false, dir, ref linkIndex))
            {
                int roomIndex = nextX * MAX_COL + nextY;
                if (roomTypes[nextX, nextY] == 0)
                {
                    AddRoom(nextX, nextY);
                    AddLink(x, y, nextX, nextY);
                    if (leftRoomCount == 0) return true;
                    visitedRoom[roomIndex] = true;
                    visitedRoomCount++;
                    TraverseAndGenerateLinkRoom(nextX, nextY);
                }
                else
                {
                    if (visitedRoom[roomIndex]) continue;
                    visitedRoom[roomIndex] = true;
                    visitedRoomCount++;
                    TraverseAndGenerateLinkRoom(nextX, nextY);
                }
            }
            else
            {
                break;
            }
        }
        if (leftRoomCount == 0) return true;
        return false;
    }
    //无随机生成房间和线路 深度优先 非递归 (基本不产生GC)
    static bool TraverseAndGenerateLinkRoom_2(int x, int y)
    {
        travelRouter.Clear();
        ClearRouterInfo();
        if (leftRoomCount == 0) return true;
        if (roomTypes[x, y] == 0) return false;
        int nextX = 0;
        int nextY = 0;
        var link = linkInfo[x, y];
        var curPosType = GetPosType(x, y);
        int maxDoorCount = doorCountTemplate[(int)curPosType];
        var router = GenRouterInfo();
        router.pos.x = (sbyte)x;
        router.pos.y = (sbyte)y;
        travelRouter.Push(router);
        while (travelRouter.Count > 0)
        {
            var curStackInfo = travelRouter.Pop(); //弹出栈顶的内容，并构建运行时需要的数据
            x = curStackInfo.pos.x;
            y = curStackInfo.pos.y;
            curPosType = GetPosType(x, y);
            PushRouterInfo(curStackInfo);
            for (int dir = 0; dir < 4; dir++)
            {
                int linkIndex = dir;
                if (GetLinkPos(x, y, ref nextX, ref nextY, curPosType, false, dir, ref linkIndex))
                {
                    if (nextX < 0 || nextX >= MAX_ROW || nextY < 0 || nextY >= MAX_COL)
                    {
                        Logger.LogErrorFormat("wrong stack info : x {0} y {1} nextX {2} nextY {3} curPosType {4} dir {5} seed {6} {7}", x, y, nextX, nextY, curPosType, dir,randomInst.GetSeed(), initRandomSeed);
                        return false;
                    }
                    int roomIndex = nextX * MAX_COL + nextY;
                    if (roomTypes[nextX, nextY] == 0)
                    {
                        AddRoom(nextX, nextY);
                        AddLink(x, y, nextX, nextY);
                        if (leftRoomCount == 0) return true;
                        visitedRoom[roomIndex] = true;
                        visitedRoomCount++;
                        var nextRouter = GenRouterInfo();
                        nextRouter.pos.x = (sbyte)nextX;
                        nextRouter.pos.y = (sbyte)nextY;
                        travelRouter.Push(nextRouter); //将领接点信息压入堆栈，进行深度遍历
                    }
                    else
                    {
                        if (visitedRoom[roomIndex]) continue;
                        visitedRoom[roomIndex] = true;
                        visitedRoomCount++;
                        var nextStack = GenRouterInfo();
                        nextStack.pos.x = (sbyte)nextX;
                        nextStack.pos.y = (sbyte)nextY;
                        travelRouter.Push(nextStack);
                    }
                }
                else
                {
                    break;
                }
            }
            if (leftRoomCount == 0) return true;
        }
        return false;
    }
    //继续生成待生成的房间
    //根据生成的基本线路图算法，还可能会存在残留没有分配的房间，那么逐点查找是否存在已经生成房间的邻近未分配房间点
    static bool FixLeftRoom()
    {
        if (leftRoomCount == 0) return true;
        Array.Clear(visitedRoom, 0, visitedRoom.Length);
        visitedRoomCount = 1;
        visitedRoom[bornPosX * MAX_COL + bornPosY] = true;
        TraverseAndGenerateLinkRoom_2(bornPosX, bornPosY);
        if (leftRoomCount == 0) return true;
        errorInfo = string.Format("leftRoom is not zero {0} seed {1} {2}", leftRoomCount, randomInst.GetSeed(), initRandomSeed);
        return false;
    }
    //增加单个房间领接数量
    static void FixRoomLink()
    {
        for (int i = 0; i < MAX_ROW; i++)
        {
            for (int j = 0; j < MAX_COL; j++)
            {
                var val = roomTypes[i, j];
                if (val == 0) continue;
                var posType = GetPosType(i, j);
                int realCount = linkInfo[i, j].GetCount();
                int maxCount = doorCountTemplate[(int)posType];
                if (maxCount >= 3 && realCount < 3)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        int nextX = 0;
                        int nextY = 0;
                        int linkIndex = 0;
                        if (!GetLinkPos(i, j, ref nextX, ref nextY, posType, false, k, ref linkIndex)) break;
                        if (roomTypes[nextX, nextY] == 0) continue;
                        if (randomInst.InRange(0, 101) < 90) continue;
                        AddLink(i, j, nextX, nextY);
                    }
                }
            }
        }
    }

    //指定位置房间对其他所有已产生的房间进行连通性测试    
    //从指定位置房间遍历其他房间的数量为总房间数则连通测试成功
    static public bool CanTraverseAllRoom(int x,int y)
    {
        if (visitedRoomCount == roomCount) return true;
        var link = linkInfo[x, y];
        for(int i = 0; i < link.GetCount();i++)
        {
            var curLink = link.GetLink(i);
            int index = curLink.x * MAX_COL + curLink.y;
            if (visitedRoom[index]) continue;
            if(roomTypes[curLink.x,curLink.y] != 0)
            {
                var nextLink = linkInfo[curLink.x, curLink.y];
                if (!nextLink.HasLink(x, y)) return false;
                visitedRoom[index] = true;
                visitedRoomCount++;
                bool bRet = CanTraverseAllRoom(curLink.x,curLink.y);
                if (!bRet) return bRet;
            }
            else
            {
               //  Console.WriteLine("Already Linked undefine room {0} {1} {2} {3}", x,y,curLink.x, curLink.y);
               errorInfo = string.Format("Already Linked undefine room {0} {1} {2} {3} {4} {5}", x,y,curLink.x, curLink.y,randomInst.GetSeed(), initRandomSeed);
            }
            if (visitedRoomCount == roomCount) return true;
        }
        return true;
    }
    //指定位置房间对其他所有已产生的房间进行连通性测试     非递归
    //从指定位置房间遍历其他房间的数量为总房间数则连通测试成功
    static public bool CanTraverseAllRoom_2(int x, int y)
    {
        if (visitedRoomCount == roomCount) return true;
        travelRouter.Clear();
        ClearRouterInfo();
        var router = GenRouterInfo();
        router.pos.x = (sbyte)x;
        router.pos.y = (sbyte)y;
        travelRouter.Push(router);
        while (travelRouter.Count > 0)
        {
            var curRouter = travelRouter.Pop();
            x = curRouter.pos.x;
            y = curRouter.pos.y;
            PushRouterInfo(curRouter);
            var link = linkInfo[x, y];
            for (int i = 0; i < link.GetCount(); i++)
            {
                var curLink = link.GetLink(i);
                int index = curLink.x * MAX_COL + curLink.y;
                if (visitedRoom[index]) continue;
                if (roomTypes[curLink.x, curLink.y] != 0)
                {
                    var nextLink = linkInfo[curLink.x, curLink.y];
                    if (!nextLink.HasLink(x, y)) return false;
                    visitedRoomCount++;
                    visitedRoom[index] = true;
                    var nextRouter = GenRouterInfo();
                    nextRouter.pos.x = curLink.x;
                    nextRouter.pos.y = curLink.y;
                    travelRouter.Push(nextRouter);
                }
                else
                {
                    //  Console.WriteLine("Already Linked undefine room {0} {1} {2} {3}", x,y,curLink.x, curLink.y);
                    errorInfo = string.Format("Already Linked undefine room {0} {1} {2} {3} {4} {5}", x, y, curLink.x, curLink.y,randomInst.GetSeed(), initRandomSeed);
                }
                if (visitedRoomCount == roomCount) return true;
            }
        }
        return true;
    }

    //检查是否所有的房间都有路可以走
    static public bool CheckAllRoomLinked()
    {
        Array.Clear(visitedRoom, 0, visitedRoom.Length);
        visitedRoomCount = 1;
        visitedRoom[bornPosX * MAX_COL + bornPosY] = true;
        CanTraverseAllRoom_2(bornPosX, bornPosY); //双向链表，只需要检测相邻两个点都有对方的领接信息即可
        if (visitedRoomCount == roomCount) return true;
        return false;

    }
    //在随机库中随机出一个房间并给这个房间定义房间类型
    static int GenerateOneRoomType(ROOM_TYPE type)
    {
        int index = randomInst.InRange(0, randRoomCount);
        int arrayIndex = randRoomIndexSet[index];
        int x = arrayIndex / MAX_COL;
        int y = arrayIndex % MAX_COL;
        roomTypes[x, y] = (byte)type;
        //有效的长度减少，直接把末尾的放入到已经选过的数组元素中去 已选过的直接废弃
        randRoomIndexSet[index] = randRoomIndexSet[randRoomCount - 1];
        randRoomCount--;
        return arrayIndex;
    }
    //随机库中randIndex位置处对应的坐标点能不能设置宝箱房间(游戏规则:宝箱房间周围4方向上不能存在其他宝箱关卡)
    static bool CanSetDropItem(int randIndex, ref int x, ref int y)
    {
        int arrayIndex = randRoomIndexSet[randIndex];
        x = arrayIndex / MAX_COL;
        y = arrayIndex % MAX_COL;
        var posType = GetPosType(x, y);
        bool hasDropRoom = false; //周边有宝箱点
        int doorCount = 0;        //可以设置宝箱点的数量
        for (int dir = 0; dir < 4; dir++)
        {
            int linkIndex = dir;
            int nextX = 0;
            int nextY = 0;
            if (GetLinkPos(x, y, ref nextX, ref nextY, posType, false, dir, ref linkIndex))
            {
                if (roomTypes[nextX, nextY] == (byte)ROOM_TYPE.DROPITEM_ROOM && linkInfo[nextX, nextY].HasLink(x, y))
                {
                    hasDropRoom = true;
                    break;
                }
                if (roomTypes[nextX, nextY] == 1)
                {
                    doorCount++;
                }
            }
            else
            {
                break;
            }
        }
        return !hasDropRoom && doorCount > 0;
    }
    //设置指定x，y位置处为宝箱房间 并将该位置从随机库中删除
    static void SetDropItemRoom(int x, int y, int index)
    {
        roomTypes[x, y] = (byte)ROOM_TYPE.DROPITEM_ROOM;
        randRoomIndexSet[index] = randRoomIndexSet[canPutDropItemCount - 1]; //把有效随机宝箱的随机库数据移入到待移除的位置处
        randRoomIndexSet[canPutDropItemCount - 1] = randRoomIndexSet[randRoomCount - 1];//把随机库末尾的数据移动到已移除的位置处去（相当于彻底移除出随机库）
        canPutDropItemCount--;
        randRoomCount--;
        realGenDropItemCount++;
    }
    //随机出宝箱房间后，但按照游戏规则不能设置宝箱后，继续随机宝箱房间 （不能设置的宝箱房间位置移出随机库）
    static bool RandConflictDropItemRoom()
    {
        int index = randomInst.InRange(0, canPutDropItemCount);
        int x = 0;
        int y = 0;
        int randSetIndex = -1;
        bool canSetRoom = CanSetDropItem(index, ref x, ref y);
        if (canSetRoom)
        {
            SetDropItemRoom(x, y, index);
        }
        else
        {
            int arrayIndex = x * MAX_COL + y;
            randSetIndex = Array.IndexOf<byte>(randRoomIndexSet, (byte)arrayIndex, 0, canPutDropItemCount);
            if (randSetIndex < 0)
            {
                //  Console.WriteLine(string.Format("生成宝箱关卡失败 {0} {1} {2} {3}", x, y, randRoomIndexSet, canPutDropItemCount));
                return false;
            }
            else
            {
                //将可选的宝箱关卡数量减少    继续随机   
                randRoomIndexSet[randSetIndex] = randRoomIndexSet[canPutDropItemCount - 1];
                randRoomIndexSet[canPutDropItemCount - 1] = (byte)arrayIndex;
                canPutDropItemCount--;
            }
        }
        return canSetRoom;
    }
    //生成宝箱房间
    static void GenerateDropItemRoom()
    {
        canPutDropItemCount = randRoomCount;
        realGenDropItemCount = 0;
        for (int i = 0; i < dropItemCount; i++)
        {
            int index = randomInst.InRange(0, canPutDropItemCount);
            int x = 0;
            int y = 0;
            bool canSetRoom = CanSetDropItem(index, ref x, ref y);
            if (canSetRoom)
            {
                SetDropItemRoom(x, y, index);
            }
            else
            {
                //继续随个4次
                bool bFind = false;
                for (int j = 0; j < 4; j++)
                {
                    if (RandConflictDropItemRoom())
                    {
                        bFind = true;
                        break;
                    }
                }
                if (!bFind)
                {
                    //仍旧找不到，直接从头到尾遍历随机库，一个一个的查找
                    for (int j = 0; j < canPutDropItemCount; j++)
                    {
                        if (CanSetDropItem(j, ref x, ref y))
                        {
                            SetDropItemRoom(x, y, j);
                            break;
                        }
                    }
                }
            }
        }
    }
    static bool IsRandRoomIndexSetSameValue()
    {
        for (int i = 0; i < randRoomCount; i++)
        {
            for (int j = i + 1; j < randRoomCount; j++)
            {
                if (randRoomIndexSet[i] == randRoomIndexSet[j])
                {
                    Logger.LogErrorFormat("the same value {0} {1} {2} {3}", i, j, randRoomIndexSet[j], randRoomCount);
                    return true;
                }
            }
        }
        return false;
    }
    //房间类型生成
    static void GenerateRoomType()
    {
        #region 大魔王房间
        //大魔王房间不能在出生点周围 八方向 把出生点周围八方向上的点全部排除出随机库
        var bornPosType = GetPosType(bornPosX, bornPosY);
        int bossRandRoomCount = randRoomCount;
        for (int dir = 0; dir < 8; dir++)
        {
            int nextPosX = bornPosX;
            int nextPosY = bornPosY;
            int linkIndex = dir;
            if (GetCrossLinkPos(bornPosX, bornPosY, ref nextPosX, ref nextPosY, bornPosType, false, dir, ref linkIndex))
            {
                if (roomTypes[nextPosX, nextPosY] != 0)
                {

                    int roomIndex = nextPosX * MAX_COL + nextPosY;
                    int findIndex = Array.IndexOf<byte>(randRoomIndexSet, (byte)roomIndex, 0, bossRandRoomCount);
                    if (findIndex == -1)
                    {
                        //Console.Write("bornX {0} bornY {1} nextPosX {2} nextPosY {3} can not find in rand index libary", bornPosX, bornPosY, nextPosX, nextPosY);
                    }
                    else
                    {
                        randRoomIndexSet[findIndex] = randRoomIndexSet[bossRandRoomCount - 1];
                        randRoomIndexSet[bossRandRoomCount - 1] = (byte)roomIndex;
                        bossRandRoomCount--;
                    }
                }
            }
            else
            {
                break;
            }
        }
        int bossRoomindex = randomInst.InRange(0, bossRandRoomCount);
        int bossIndex = randRoomIndexSet[bossRoomindex];
        int x = bossIndex / MAX_COL;
        int y = bossIndex % MAX_COL;
        roomTypes[x, y] = (byte)ROOM_TYPE.BOSS_ROOM;
        //有效的长度减少，直接把末尾的放入到已经选过的数组元素中去 已选过的直接废弃
        randRoomIndexSet[bossRoomindex] = randRoomIndexSet[randRoomCount - 1];
        randRoomCount--;
        #endregion
        GenerateOneRoomType(ROOM_TYPE.KEY_ROOM); //钥匙房间
        GenerateOneRoomType(ROOM_TYPE.MAP_ROOM); //地图房间;
        #region 宝藏房间
        //宝藏房间      
        int bossPosX = bossIndex / MAX_COL;
        int bossPosY = bossIndex % MAX_COL;
        var bossPosType = GetPosType(bossPosX, bossPosY);
        int doorCount = 0;
        int nextX = bossPosX;
        int nextY = bossPosY;
        int doorX = 0;
        int doorY = 0;
        int unknownDoorCount = 0;
        for (int dir = 0; dir < 4; dir++)
        {
            nextX = bossPosX;
            nextY = bossPosY;
            int linkIndex = dir;
            if (GetLinkPos(bossPosX, bossPosY, ref nextX, ref nextY, bossPosType, false, dir, ref linkIndex))
            {
                if (roomTypes[nextX, nextY] != 0 && linkInfo[bossPosX, bossPosY].HasLink(nextX, nextY))
                {
                    doorCount++;
                    if (roomTypes[nextX, nextY] == 1) 
                    {
                        unknownDoorCount++;
                        doorX = nextX;
                        doorY = nextY;
                    }
                }
            }
            else
            {
                break;
            }
        }
        if (doorCount == 1 && unknownDoorCount == 1)
        {
            //大魔王点如果只有一条通路，需要把临近大魔王的点从随机库中移除（这里的移除只是移除到有效数组的末尾）
            int nextRoomIndex = (doorX * MAX_COL + doorY);
            int index = Array.IndexOf<byte>(randRoomIndexSet, (byte)nextRoomIndex,0,randRoomCount);
            if (index < 0)
            {
                //  Console.WriteLine(string.Format("生成宝藏关卡失败 {0} {1} {2} {3}", nextX, nextY, nextRoomIndex, randRoomIndexSet));
            }
            else
            {
                byte temp = randRoomIndexSet[index];
                randRoomIndexSet[index] = randRoomIndexSet[randRoomCount - 1];
                randRoomIndexSet[randRoomCount - 1] = temp;

                int randIndex = randomInst.InRange(0, randRoomCount - 1);
                int arrayIndex = randRoomIndexSet[randIndex];
                int endY = arrayIndex % MAX_COL;
                int endX = arrayIndex / MAX_COL;
                roomTypes[endX, endY] = (byte)ROOM_TYPE.END_ROOM;
                //有效的长度减少，直接把末尾的放入到已经选过的数组元素中去 已选过的直接废弃
                randRoomIndexSet[randIndex] = randRoomIndexSet[randRoomCount - 1];
                randRoomCount--;
            }
        }
        else
        {
            //多条通路情况，可以随意生成宝藏关卡
            GenerateOneRoomType(ROOM_TYPE.END_ROOM);
        }
        #endregion

        //生成宝箱房间
        GenerateDropItemRoom();

        //生成试炼房间
        for (int i = 0; i < trialRoomCount; i++)
        {
            GenerateOneRoomType(ROOM_TYPE.TRIAL_ROOM);
        }
    }
    //检查是否所有的房间类型的数量已经满足
    static bool CheckAllRoomType()
    {
        int realCount = 0;
        int NORMAL_ROOM_COUNT = 0; //普通房间
        int BOSS_ROOM_COUNT = 0; //大魔王房间
        int END_ROOM_COUNT = 0;//大宝箱房间
        int KEY_ROOM_COUNT = 0; //钥匙房间
        int MAP_ROOM_COUNT = 0; //地图道具房间
        int DROPITEM_ROOM_COUNT = 0; //小宝箱房间
        int TRIAL_ROOM_COUNT = 0;  //试炼房间
        int BORN_ROOM_COUNT = 0;//出生点房间
        for (int i = 0; i < MAX_ROW; i++)
        {
            for (int j = 0; j < MAX_COL; j++)
            {
                var val = roomTypes[i, j];
                if (val == 0) continue;
                realCount++;
                if (val == (byte)ROOM_TYPE.NORMAL_ROOM)
                {
                    NORMAL_ROOM_COUNT++;
                }
                else if (val == (byte)ROOM_TYPE.BOSS_ROOM)
                {
                    BOSS_ROOM_COUNT++;
                }
                else if (val == (byte)ROOM_TYPE.END_ROOM)
                {
                    END_ROOM_COUNT++;
                }
                else if (val == (byte)ROOM_TYPE.KEY_ROOM)
                {
                    KEY_ROOM_COUNT++;
                }
                else if (val == (byte)ROOM_TYPE.MAP_ROOM)
                {
                    MAP_ROOM_COUNT++;
                }
                else if (val == (byte)ROOM_TYPE.DROPITEM_ROOM)
                {
                    DROPITEM_ROOM_COUNT++;
                }
                else if (val == (byte)ROOM_TYPE.TRIAL_ROOM)
                {
                    TRIAL_ROOM_COUNT++;
                }
                else if (val == (byte)ROOM_TYPE.BORN_ROOM)
                {
                    BORN_ROOM_COUNT++;
                }
            }
        }
        if (realCount != roomCount)
        {
            errorInfo = string.Format("seed {0} {1} room count is not same {2} {3}!", randomInst.GetSeed() , initRandomSeed, realCount, roomCount);
            return false;
        }
        if(BORN_ROOM_COUNT != 1)
        {
            errorInfo = string.Format("seed {0} {1} born room count is not single !", randomInst.GetSeed(), initRandomSeed);
            return false;
        }
        if(BOSS_ROOM_COUNT != 1)
        {
            errorInfo = string.Format("seed {0} {1} boss room count is not single !", randomInst.GetSeed(), initRandomSeed);
            return false;
        }
        if(END_ROOM_COUNT != 1)
        {
            errorInfo = string.Format("seed {0} {1} end room count is not single !", randomInst.GetSeed(), initRandomSeed);
            return false;
        }
        if(KEY_ROOM_COUNT != 1)
        {
            errorInfo = string.Format("seed {0} {1} key room count is not single !", randomInst.GetSeed(), initRandomSeed);
            return false;
        }
        if(MAP_ROOM_COUNT != 1)
        {
            errorInfo = string.Format("seed {0} {1} map room count is not single !", randomInst.GetSeed(), initRandomSeed);
            return false;
        }
        if(DROPITEM_ROOM_COUNT != dropItemCount)
        {
            errorInfo = string.Format("seed {0} {1} drop room count is not same {2} {3}!", randomInst.GetSeed(), initRandomSeed, DROPITEM_ROOM_COUNT, dropItemCount);
            return false;
        }
        if(TRIAL_ROOM_COUNT != trialRoomCount)
        {
            errorInfo = string.Format("seed {0} {1} Trial room count is not same {2} {3}!", randomInst.GetSeed(), initRandomSeed,TRIAL_ROOM_COUNT, trialRoomCount);
            return false;
        }
        //Logger.LogErrorFormat("RoomCount {9} RealCount {0} NORMAL_ROOM_COUNT {1} BOSS_ROOM_COUNT  {2} END_ROOM_COUNT {3} KEY_ROOM_COUNT {4} MAP_ROOM_COUNT {5} DROPITEM_ROOM_COUNT {6} TRIAL_ROOM_COUNT {7} BORN_ROOM_COUNT {8}",
        //realCount,
        //NORMAL_ROOM_COUNT,
        //BOSS_ROOM_COUNT,
        //END_ROOM_COUNT,
        //KEY_ROOM_COUNT,
        //MAP_ROOM_COUNT,
        //DROPITEM_ROOM_COUNT,
        //TRIAL_ROOM_COUNT,
        //BORN_ROOM_COUNT,roomCount);
        //Logger.LogError("====================");
        return true;
    }
    static bool BuildRoom(FrameRandomImp rand)
    {
        randomInst = rand;
        initRandomSeed = 0;
        if (randomInst == null)
        {
            Logger.LogErrorFormat("random Instance is not valid");
            return false;
        }
        initRandomSeed = randomInst.GetSeed();
        for (int i = 0; i < MAX_ROW; i++)
        {
            for (int j = 0; j < MAX_COL; j++)
            {
                roomTypes[i, j] = 0;
                if (linkInfo[i, j] == null)
                {
                    linkInfo[i, j] = new LinkInfo();
                }
                else
                {
                    linkInfo[i, j].Reset();
                }
            }
        }
        //最大随机库，保证随机库无需频繁申请空间
        if(randRoomIndexSet == null)
        {
            randRoomIndexSet = new byte[MAX_ROW * MAX_COL];
        }
        roomCount = (sbyte)randomInst.InRange(36, 49);
        randRoomCount = 0;
        Array.Clear(randRoomIndexSet, 0, randRoomIndexSet.Length);
        //生成出生点房间
        bornPosX = (sbyte)randomInst.InRange(0, (int)MAX_ROW);
        bornPosY = (sbyte)randomInst.InRange(0, (int)MAX_COL);
        roomTypes[bornPosX, bornPosY] = (int)ROOM_TYPE.BORN_ROOM;
        sbyte one = 1;
        leftRoomCount = (sbyte)((int)(roomCount) - (int)(one));
        bool allResult = true;
        //构建线路图
        bool ret = GenerateLinkRoom_2(bornPosX, bornPosY);
        if (leftRoomCount > 0)
        {
            Logger.LogProcessFormat("还需要生成 {0} 房间", leftRoomCount);
        }

        if (!ret)
        {
            Logger.LogError(errorInfo);
        }
        ret = FixLeftRoom();
        allResult = ret;
        if (!ret)
        {
            Logger.LogError(errorInfo);
            Logger.LogProcessFormat("待生成 {0} 房间 还剩 {1} 房间 没有分配", leftRoomCount, randRoomCount);
            randomInst = null;
            return allResult;
        }

        ret = CheckAllRoomLinked();
        FixRoomLink();
        allResult &= ret;
        if (!ret)
        {
            Logger.LogError(errorInfo);
        }
        //生成房间
        dropItemCount = (sbyte)randomInst.InRange(3, 5);////3~4个宝箱房间（3~4个宝箱纯随机即可）
        trialRoomCount = (sbyte)randomInst.InRange(5,8);//（5~7个试炼房间，纯随机即可）
        GenerateRoomType();
        ret = CheckAllRoomType();
        allResult &= ret;
        if (!ret)
        {
            Logger.LogError(errorInfo);
        }
        randomInst = null;
        return allResult;
    }
    //生成DungeonData 返回值为出生点的索引
    public static int BuildTreasureDungeonData(FrameRandomImp rand,out IDungeonData dungeonData)
    {
        bool ret = true;
        dungeonData = null;
        ret = BuildRoom(rand);
        if (!ret) return -1;

        int height = MAX_ROW;
        int width = MAX_COL;
        DSceneDataConnect[] areaconnectlist = new DSceneDataConnect[roomCount];

        int index = 0;
        int startIndex = -1;

        for (int i = 0; i < MAX_ROW; i++)
        {
            for (int j = 0; j < MAX_COL; j++)
            {
                var mRoomType = roomTypes[i, j];

                if (roomTypes[i, j] == 0)
                { continue; }
                //TODO  isconnect,linkAreaIndex,//这两个要跟策划配置绑定id,sceneareapath,
                DSceneDataConnect sceneData = new DSceneDataConnect();
                for (int k = 0; k < 4; k++)
                {
                    sceneData.isconnect[k] = false;
                }
                //sceneData.treasureType = mRoomType;
                if (mRoomType == (byte)ROOM_TYPE.BORN_ROOM)
                {
                    startIndex = index;
                    sceneData.isstart = true;
                }
                if (mRoomType == (byte)ROOM_TYPE.END_ROOM)
                {
                    sceneData.isboss = true;
                }
                sceneData.treasureType = mRoomType;
                sceneData.positionx = j;
                sceneData.positiony = i;
                sceneData.id = index;
                sceneData.areaindex = index;
                areaconnectlist[index++] = sceneData;
                var library = TableManager.GetInstance().GetTreasureDungeonRandomLibrary(mRoomType);
                if (library == null || library.Count <= 0)
                {
                    Logger.LogErrorFormat("TreasureMap At {0} {1} Type {2} seed {3} not set room path!", i, j, mRoomType, initRandomSeed);
                    continue;
                }
                int roomRandIndex = rand.InRange(0, library.Count);
                sceneData.sceneareapath = library[roomRandIndex];

            }
        }
        //第二遍遍历来添加连接信息
        for (int i = 0; i < MAX_ROW; i++)
        {
            for (int j = 0; j < MAX_COL; j++)
            {
                LinkInfo mLinkInfo = linkInfo[i, j];
                if (mLinkInfo.GetCount() <= 0)
                {
                    continue;
                }
                //以下计算与游戏内默认一直，只会连通←↑→↓
                for (int k = 0; k < areaconnectlist.Length; ++k)
                {
                    var kSceneData = areaconnectlist[k];
                    if (!mLinkInfo.HasLink(kSceneData.GetPositionY(), kSceneData.GetPositionX()))
                    {
                        continue;
                    }
                    if (kSceneData.GetPositionY() - i == 0)
                    {
                        if (kSceneData.GetPositionX() - j == 1) //←←←←←
                        {
                            kSceneData.isconnect[(int)TransportDoorType.Left] = true;
                        }
                        if (kSceneData.GetPositionX() - j == -1) //→→→→→
                        {
                            kSceneData.isconnect[(int)TransportDoorType.Right] = true;
                        }
                    }
                    if (kSceneData.GetPositionX() - j == 0)
                    {
                        if (kSceneData.GetPositionY() - i == 1) //↑↑↑↑↑
                        {
                            kSceneData.isconnect[(int)TransportDoorType.Top] = true;
                        }
                        if (kSceneData.GetPositionY() - i == -1) //↓↓↓↓↓
                        {
                            kSceneData.isconnect[(int)TransportDoorType.Buttom] = true;
                        }
                    }
                }
            }
        }
#if !LOGIC_SERVER
        var mDungeonData = UnityEngine.ScriptableObject.CreateInstance<DDungeonData>();
        dungeonData = mDungeonData;

        if (mDungeonData == null)
        {
            return -1;
        }

        mDungeonData.height = height;
        mDungeonData.weidth = width;
        mDungeonData.areaconnectlist = areaconnectlist;
        mDungeonData.startindex = startIndex;

#if UNITY_EDITOR
        FloodVisitAllSceneConnect(mDungeonData, startIndex);
#endif
#else
        
        FlatBuffers.FlatBufferBuilder builder = new FlatBuffers.FlatBufferBuilder(1);
        FlatBuffers.VectorOffset areaconnectlistVector = default(FlatBuffers.VectorOffset);
        if (areaconnectlist != null)
        {
            List<FlatBuffers.Offset<FBDungeonData.DSceneDataConnect>> list = new List<FlatBuffers.Offset<FBDungeonData.DSceneDataConnect>>();

            for (int i = 0; i < areaconnectlist.Length; ++i)
            {
                var editorCon = areaconnectlist[i];
                FlatBuffers.VectorOffset isConnectVector = default(FlatBuffers.VectorOffset);
                if (editorCon != null)
                {
                    isConnectVector = FBDungeonData.DSceneDataConnect.CreateIsconnectVector(
                                builder, editorCon.isconnect);
                }
                FlatBuffers.VectorOffset linkAreaIndexVector = default(FlatBuffers.VectorOffset);
                if (editorCon != null)
                {
                    linkAreaIndexVector = FBDungeonData.DSceneDataConnect.CreateLinkAreaIndexVector(
                                builder, editorCon.linkAreaIndex);
                }
                var data = FBDungeonData.DSceneDataConnect.CreateDSceneDataConnect(builder,
                    //default(VectorOffset),
                    isConnectVector,
                    linkAreaIndexVector,
                    editorCon.areaindex,
                    editorCon.id,
                    builder.CreateString(editorCon.sceneareapath),
                    editorCon.positionx,
                    editorCon.positiony,
                    editorCon.isboss,
                    editorCon.isstart,
                    editorCon.isegg,
                    editorCon.isnothell,
                    editorCon.treasureType
                );
                list.Add(data);
            }

            areaconnectlistVector = FBDungeonData.DDungeonData.CreateAreaconnectlistVector(builder, list.ToArray());
        }
        FlatBuffers.Offset<FBDungeonData.DDungeonData> sdata = FBDungeonData.DDungeonData.CreateDDungeonData(builder,
            builder.CreateString(string.Empty),
            height,
            width,
            startIndex,
            areaconnectlistVector
        );
        FBDungeonData.DDungeonData.FinishDDungeonDataBuffer(builder, sdata);
        var ms = new System.IO.MemoryStream(builder.DataBuffer.Data, builder.DataBuffer.Position, builder.Offset);

        FlatBuffers.ByteBuffer buffer = new FlatBuffers.ByteBuffer(ms.ToArray());
        FBDungeonData.DDungeonData fbdata = FBDungeonData.DDungeonData.GetRootAsDDungeonData(buffer);

        dungeonData = new BattleDungeonData(fbdata);
#if UNITY_EDITOR
        //FloodVisitAllSceneConnect(mDungeonData, startIndex);
#endif
#endif
        return startIndex;
    }

#if UNITY_EDITOR
    static void FloodVisitAllSceneConnect(DDungeonData dungeonData, int startIndex)
    {
        bool[] tagMap = new bool[dungeonData.areaconnectlist.Length];
        for(int i = 0; i < tagMap.Length; ++i)
        {
            tagMap[i] = false;
        }
        Flooding(dungeonData, startIndex, tagMap);

        for(int i = 0; i < tagMap.Length; ++i)
        {
            if(tagMap[i] == false)
            {
                Logger.LogError("ERROR Dungeondata is inValid!!!");
                return;
            }
        }
    }

    static void Flooding(DDungeonData dungeonData, int startIndex, bool[] tagMap)
    {
        try
        {
            if (tagMap[startIndex] == true)
            {
                return;
            }
            else
            {
                tagMap[startIndex] = true;
                int nextIndex;
                for (int i = 0; i < (int)TransportDoorType.None; ++i)
                {
                    TransportDoorType type = (TransportDoorType)i;
                    if (dungeonData.areaconnectlist[startIndex].isconnect[i])
                    {
                        dungeonData.GetSideByType(dungeonData.areaconnectlist[startIndex], type, out nextIndex);
                        if (nextIndex != -1)
                        {
                            Flooding(dungeonData, nextIndex, tagMap);
                        }
                        else
                        {
                            Logger.LogError("Find connect room fail!!!");
                        }
                    }
                    
                }

            }
        }
        catch(Exception e)
        {
            Logger.LogError(string.Format("index:{0},message:{1}", startIndex, e.Message));
        }
    }
#endif
}

