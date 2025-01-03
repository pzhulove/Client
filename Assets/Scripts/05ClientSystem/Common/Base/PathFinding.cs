using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using PathFinder;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    static public class PathFinding
    {
        public const int OBLIQUE = 14;
        public const int STEP = 10;

        public class GridInfo
        {
            public Vector2 GridSize;                // 网格大小 
            public float GridDiagonalLength;        // 格子对角线长度
            public int GridMinX;                    // 在网格坐标系下的网格X轴的最小值
            public int GridMaxX;                    // 在网格坐标系下的网格X轴的最大值
            public int GridMinY;                    // 在网格坐标系下的网格y轴的最小值
            public int GridMaxY;                    // 在网格坐标系下的网格y轴的最大值
            public byte[] GridBlockLayer;           // 网格阻挡信息
        }

        static List<Grid> m_closeList = new List<Grid>();
        static PriorityQueue<Grid> m_openList = new PriorityQueue<Grid>(new GridComparer());
        static List<Grid> m_tempSurrroundGrids = new List<Grid>();
        static GridInfo m_gridInfo;

        public static List<Vector3> FindPath(Vector3 current, Vector3 target, GridInfo gridInfo)
        {
            current.y = 0.0f;
            target.y = 0.0f;
            List<Vector3> path = new List<Vector3>();

            Logger.LogProcessFormat("寻路 >>> 当前位置:({0},{1},{2}) 目标位置：({3},{4},{5})",
                    current.x, current.y, current.z, target.x, target.y, target.z);

            if ((current-target).sqrMagnitude <= 0.000002f)
            {
                path.Add(current);
                path.Add(target);

                Logger.LogProcessFormat("寻路 >>> 位置相同，无须寻路");
                return path;
            }

            // 新的寻路
            var v = TableManager.instance.GetTableItem<SwitchClientFunctionTable>(22);
            if (v == null || v.Open)
            {
                byte[] data = gridInfo.GridBlockLayer;
                int col = gridInfo.GridMaxX - gridInfo.GridMinX;
                int row = gridInfo.GridMaxY - gridInfo.GridMinY;

                int startPosX = (int)(Mathf.Floor(current.x / gridInfo.GridSize.x)) - gridInfo.GridMinX;
                int startPosY = (int)(Mathf.Floor(current.z / gridInfo.GridSize.y)) - gridInfo.GridMinY;
                int endPosx = (int)(Mathf.Floor(target.x / gridInfo.GridSize.x)) - gridInfo.GridMinX;
                int endPosY = (int)(Mathf.Floor(target.z / gridInfo.GridSize.y)) - gridInfo.GridMinY;

                PathHelper._start.Set(startPosX, startPosY);
                PathHelper._end.Set(endPosx, endPosY);

                if (PathHelper._start.x < 0 ||  PathHelper._start.y < 0 || PathHelper._end.x < 0 ||  PathHelper._end.y < 0){
                    Logger.LogError(string.Format("坐标 小于0了  {0}{1}{2}{3}", PathHelper._start.x ,  PathHelper._start.y , PathHelper._end.x ,  PathHelper._end.y));
                    return path;
                }


                if (PathHelper._astar.Do(data, col, row, PathHelper._start, PathHelper._end, PathHelper._list))
                {
                    path.Add(current);
                    float x = current.x;
                    float y = current.y;
                    float z = current.z;
                    for (int index = 0; index < PathHelper._list.Count - 1; ++index)
                    {
                        var point = PathHelper._list[index];
                        x += PathHelper.DIR_VALUE2[point, 0] * gridInfo.GridSize.x;
                        z += PathHelper.DIR_VALUE2[point, 1] * gridInfo.GridSize.y;
                        path.Add(new Vector3(x, y, z));
                    }
                    path.Add(target);
                }
                return path;
            }

            m_gridInfo = gridInfo;

            Grid start = new Grid(m_gridInfo, current);
            Grid end = new Grid(m_gridInfo, target);

            int endCloseIndex = -1;

            m_openList.Add(start);
            while (m_openList.Count != 0)
            {
                Grid tempStart = m_openList.TakeTop();
                m_closeList.Add(tempStart);
                int tempCloseIndex = m_closeList.Count - 1;
                //找出它相邻的点
                var surroundPoints = _SurrroundPoints(tempStart);
                for (int i = 0; i < surroundPoints.Count; ++i)
                {
                    Grid grid = surroundPoints[i];
                    int tempGridIndex = m_openList.FindIndex(value => { return value.X == grid.X && value.Y == grid.Y; });
                    if (tempGridIndex >= 0 && tempGridIndex < m_openList.Count)
                    {
                        Grid tempGrid = m_openList[tempGridIndex];
                        float G = _CalcG(tempStart, tempGrid);
                        if (G < tempGrid.G)
                        {
                            tempGrid.ParentGrid = tempCloseIndex;
                            tempGrid.G = G;
                            m_openList[tempGridIndex] = tempGrid;
                        }
                    }
                    else
                    {
                        grid.ParentGrid = tempCloseIndex;
                        grid.G = _CalcG(tempStart, grid);
                        grid.H = _CalcH(end, grid);
                        m_openList.Add(grid);
                    }
                }

                int endOpenIndex = m_openList.FindIndex(value => { return value.X == end.X && value.Y == end.Y; });
                if (endOpenIndex >= 0 && endOpenIndex < m_openList.Count)
                {
                    m_closeList.Add(m_openList[endOpenIndex]);
                    endCloseIndex = m_closeList.Count - 1;
                    // TODO 优先队列暂时没实现随机删除元素，这里对逻辑也没有影响，就先放着
                    //m_openList.RemoveAt(endOpenIndex);
                    break;
                }
            }


            if (endCloseIndex >= 0 && endCloseIndex < m_closeList.Count)
            {
                // 确保最后一个点，一定是目标点
                path.Insert(0, target);
                if (m_closeList[endCloseIndex].RealPos == target)
                {
                    endCloseIndex = m_closeList[endCloseIndex].ParentGrid;
                }
                while (endCloseIndex >= 0 && endCloseIndex < m_closeList.Count)
                {
                    path.Insert(0, m_closeList[endCloseIndex].RealPos);
                    endCloseIndex = m_closeList[endCloseIndex].ParentGrid;
                }
            }

            m_openList.Clear();
            m_closeList.Clear();
            m_tempSurrroundGrids.Clear();

            for (int i = 0; i < path.Count; ++i)
            {
                Logger.LogProcessFormat("寻路 >>> 路径点:({0},{1},{2})", path[i].x, path[i].y, path[i].z);
            }

            return path;
        }

        static private float _CalcG(Grid start, Grid point)
        {
            float G = 0.0f;
            if (start.X == point.X)
            {
                G = m_gridInfo.GridSize.x;
            }
            else if (start.Y == point.Y)
            {
                G = m_gridInfo.GridSize.y;
            }
            else
            {
                G = m_gridInfo.GridDiagonalLength;
            }
            return G + start.G;
        }

        static float _CalcH(Grid end, Grid point)
        {
            Vector3 offset = end.RealPos - point.RealPos;
            return (float)Math.Sqrt(offset.x* offset.x + offset.z * offset.z);
        }

        //获取某个点周围可以到达的点
        static List<Grid> _SurrroundPoints(Grid point)
        {
            m_tempSurrroundGrids.Clear();
            //var surroundPoints = new List<Grid>();

            for (int x = point.X - 1; x <= point.X + 1; x++)
            {
                for (int y = point.Y - 1; y <= point.Y + 1; y++)
                {
                    if (x != point.X || y != point.Y)
                    {
                        Grid grid = new Grid(m_gridInfo, x, y);
                        if (_GridCanReach(grid))
                        {
                            m_tempSurrroundGrids.Add(grid);
                        }
                    }
                }
            }

            return m_tempSurrroundGrids;
        }

        //在二维数组对应的位置不为障碍物
        static bool _GridCanReach(Grid grid)
        {
            if (grid.X < m_gridInfo.GridMinX || grid.X >= m_gridInfo.GridMaxX)
            {
                return false;
            }
            if (grid.Y < m_gridInfo.GridMinY || grid.Y >= m_gridInfo.GridMaxY)
            {
                return false;
            }
            int closeListIndex = m_closeList.FindIndex(value => { return value.X == grid.X && value.Y == grid.Y; });
            if (closeListIndex >= 0 && closeListIndex < m_closeList.Count)
            {
                return false;
            }

            int x = grid.X - m_gridInfo.GridMinX;
            int y = grid.Y - m_gridInfo.GridMinY;

            int index = (m_gridInfo.GridMaxX - m_gridInfo.GridMinX) * y + x;
            if (index >=0 && index < m_gridInfo.GridBlockLayer.Length)
            {
                return m_gridInfo.GridBlockLayer[index] == 0;
            }
            else
            {
                Logger.LogErrorFormat("GridCanReach index error!!! grid.X:{0} grid.Y:{1} GridMinX:{2} GridMinY:{3} GridXSize:{4} GridYSize:{5}",
                    grid.X, grid.Y, m_gridInfo.GridMinX, m_gridInfo.GridMinY, m_gridInfo.GridMaxX - m_gridInfo.GridMinX, m_gridInfo.GridMaxY - m_gridInfo.GridMinY);
                return false;
            }
        }

        public struct Grid
        {
			public int ParentGrid;// { get; set; }

			public float F;// { get; private set; }
            float m_g;
            public float G { get { return m_g; } set { m_g = value; F = m_g + m_h; } }
            float m_h;
            public float H { get { return m_h; } set { m_h = value; F = m_g + m_h; } }

			public int X;// { get; private set; }
			public int Y;// { get; private set; }
			public Vector3 RealPos;// { get; private set; }

            public Grid(GridInfo gridInfo, Vector3 realPos)
            {
                RealPos = realPos;
                X = (int)Math.Floor(RealPos.x / gridInfo.GridSize.x);
                Y = (int)Math.Floor(RealPos.z / gridInfo.GridSize.y);

                m_h = 0.0f;
                m_g = 0.0f;
                F = 0.0f;
                ParentGrid = -1;
            }

            public Grid(GridInfo gridInfo, int x, int y)
            {
                X = x;
                Y = y;
                RealPos = new Vector3(
                    ((float)X + 0.5f) * gridInfo.GridSize.x,
                    0.0f,
                    ((float)Y + 0.5f) * gridInfo.GridSize.y
                    );

                m_h = 0.0f;
                m_g = 0.0f;
                F = 0.0f;
                ParentGrid = -1;
            }
        }


        public class GridComparer : IComparer<Grid>
        {
            public int Compare(Grid x, Grid y)
            {
                if (x.F > y.F)
                {
                    return -1;
                }
                else if (x.F < y.F)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

}
