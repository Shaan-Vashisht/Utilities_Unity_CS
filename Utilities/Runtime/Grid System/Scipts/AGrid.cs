using System.Collections.Generic;
using UnityEngine;

namespace Utilities.GridSystem
{
    public abstract class AGrid<T>
    {
        internal int width;
        internal int height;
        protected T[,] gridArray;

        internal virtual bool ValidXY(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }
        internal virtual bool ValidXY((int, int) pos)
        {
            return ValidXY(pos.Item1, pos.Item2);
        }

        public T GetObject(int x, int y)
        {
            if (ValidXY(x, y))
            {
                return gridArray[x, y];
            }
            else
            {
                Debug.LogWarning($"Inputted Invalid XY ({x}, {y}) in GetObject.");
                return default;
            }
        }
        public T GetObject((int, int) pos)
        {
            return GetObject(pos.Item1, pos.Item2);
        }

        public T[] GetObjects((int, int)[] positions)
        {
            List<T> objs = new List<T>();
            foreach ((int, int) pos in positions)
            {
                objs.Add(GetObject(pos));
            }

            return objs.ToArray();
        }

        public void SetObject(int x, int y, T value)
        {
            if (ValidXY(x, y))
            {
                gridArray[x, y] = value;
            }
            else
            {
                Debug.LogWarning($"Inputted Invalid XY ({x}, {y}) in SetObject.");
            }
        }
        public void SetObject((int, int) pos, T value)
        {
            SetObject(pos.Item1, pos.Item2, value);
        }

        /// <summary>
        /// Returns the distance between two tiles in the grid.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public abstract int GetDistance(int x1, int y1, int x2, int y2);
        public int GetDistance((int, int) pos1, (int, int) pos2)
        {
            return GetDistance(pos1.Item1, pos1.Item2, pos2.Item1, pos2.Item2);
        }

        /// <summary>
        /// Returns an array with all of the positions within a range of the given position. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="range"></param>
        /// <param name="includeStart"></param>
        /// <returns></returns>
        public abstract (int, int)[] GetPositionsInRange(int x, int y, int range, bool includeStart = false);
        public (int, int)[] GetPositionsInRange((int, int) pos, int range, bool includeStart = false)
        {
            return GetPositionsInRange(pos.Item1, pos.Item2, range, includeStart);
        }

        /// <summary>
        /// Returns an array with the positions that form a line from the starting position to the ending position.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="includeStart"></param>
        /// <param name="includeEnd"></param>
        /// <returns></returns>
        public abstract (int, int)[] GetPositionsInLine(int x1, int y1, int x2, int y2, bool includeStart = false, bool includeEnd = false);
        public (int, int)[] GetPositionsInLine((int, int) p1, (int, int) p2, bool includeStart = false, bool includeEnd = false)
        {
            return GetPositionsInLine(p1.Item1, p1.Item2, p2.Item1, p2.Item2, includeStart, includeEnd);
        }

        /// <summary>
        /// Inverts the objects at the given positions.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void SwapObjects(int x1, int y1, int x2, int y2)
        {
            if (ValidXY(x1, y1) && ValidXY(x2, y2))
            {
                T temp = GetObject(x1, y1);
                SetObject(x1, y1, GetObject(x2, y2));
                SetObject(x2, y2, temp);
            }
            else
            {
                Debug.LogWarning($"Inputted Invalid XY ({x1}, {y1}; {x2}, {y2}) in SwapObjects");
            }
        }
        public void SwapObjects((int, int) p1, (int, int) p2)
        {
            SwapObjects(p1.Item1, p1.Item2, p2.Item1, p2.Item2);
        }
    }
}