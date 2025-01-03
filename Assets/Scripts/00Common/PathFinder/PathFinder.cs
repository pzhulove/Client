using System;
using System.Collections.Generic;
//using System.Security.Policy;


namespace PathFinder
{
    public interface IPathFinder
    {
        bool Do(int[] obstacle, int width, int height, Point pstart, Point pend, List<int> steps);
        bool Do(int[] obstacle, int width, int height, Point pstart, Point pend, List<Point> steps);

        bool Do(byte[] obstacle, int width, int height, Point pstart, Point pend, List<int> steps);
        bool Do(byte[] obstacle, int width, int height, Point pstart, Point pend, List<Point> steps);

    }

    public class PathHelper{

        public static int[,] DIR_VALUE2 =
        {
            {1, 0}, {-1, 0}, {0, 1}, {0, -1},
            {1, 1}, {-1, 1}, {1, -1}, {-1, -1}
        };

        public enum MoveDir
        {
            RIGHT = 0, //0
            LEFT = 1, //180
            TOP = 2, //90
            DOWN = 3, //
            RIGHT_TOP = 4,
            LEFT_TOP = 5,
            RIGHT_DOWN = 6,
            LEFT_DOWN = 7,

            COUNT
        }

        public static List<int> _list = new List<int>();
        public static Point _start = new Point();
        public static Point _end = new Point();
        public static AStar _astar = new AStar();

    }

    public class Point
    {
        public Point(int px = 0, int py = 0)
        {
            x = px;
            y = py;
        }

        public void Set(int px = 0, int py = 0)
        {
            x = px;
            y = py;
        }

        public bool IsIn(int w, int h)
        {
            if (x < 0 || x >= w)
            {
                return false;
            }

            if (y < 0 || y >= h)
            {
                return false;
            }

            return true;
        }

        public int x;
        public int y;

    }



}