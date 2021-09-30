using System.Collections.Generic;
using System;
using UnityEngine;

namespace Utilities.GridSystem
{
    public class HexGrid<T> : AGrid<T>
    {
        public HexGrid(int width, int height)
        {
            this.width = width;
            // Uses doubled-height hex grid to make calculations easier
            this.height = 2 * height;

            gridArray = new T[width, 2 * height];
        }

        internal override bool ValidXY(int x, int y)
        {
            //In a doubled-coordinate hex grid the coordinates in each tile are either both even or
            //both odd so their sum is always even.
            return base.ValidXY(x, y) && (x + y) % 2 == 0;
        }

        public override int GetDistance(int x1, int y1, int x2, int y2)
        {
            int dx = Mathf.Abs(x1 - x2);
            int dy = Mathf.Abs(y1 - y2);

            // Double-height so dy is always two times what it should be
            return dx + Mathf.Max(0, (dy - dx) / 2);
        }

        public override (int, int)[] GetPositionsInRange(int x, int y, int range, bool includeStart = false)
        {
            if (ValidXY(x, y) && range > 0)
            {
                List<(int, int)> positions = new List<(int, int)>();

                // Loops through the position delta
                for (int dx = -range; dx <= range; dx++)
                {
                    for (int dy = -2 * range; dy <= 2 * range; dy++)
                    {
                        // Adds the delta to  the starting position to get the new position
                        int nX = x + dx;
                        int nY = y + dy;

                        // Checks if it's valid and, if so, adds it to the array
                        if (ValidXY(nX, nY) && GetDistance(x, y, nX, nY) <= range) positions.Add((nX, nY));
                    }
                }

                if (!includeStart) positions.Remove((x, y));

                return positions.ToArray();
            }
            else
            {
                Debug.LogWarning($"Inputted Invalid XY ({x}, {y}) or Range ({range}) in GetPositionsInRange.");
                return default;
            }
        }

        public override (int, int)[] GetPositionsInLine(int x1, int y1, int x2, int y2, bool includeStart = false, bool includeEnd = false)
        {
            if (ValidXY(x1, y1) && ValidXY(x2, y2))
            {
                List<(int, int)> line = new List<(int, int)>();
                bool useFV = Math.Abs((y1 - y2) / 2) >= 4;
                bool forceVertical = false;

                if (includeStart) line.Add((x1, y1));
                while (x1 != x2 || y1 != y2)
                {
                    // Gets the total difference between current position and end
                    int dx = x2 - x1;
                    int dy = y2 - y1;

                    // Gets the new step from the sign of the difference
                    int mx = Math.Sign(dx);
                    int my = Math.Sign(dy);

                    // There must always be some vertical movement
                    if (my == 0) my++;

                    if (useFV)
                    {
                        // Alternates between vertical and horizontal movement to create a smoother line
                        if (forceVertical && GetDistance(x1, y1, x2, y2) > 2)
                        {
                            mx = 0;
                            forceVertical = false;
                        }
                        else forceVertical = true;
                    }

                    // Movement within the same column needs double vertical movement
                    if (mx == 0) my *= 2;

                    x1 += mx;
                    y1 += my;
                    line.Add((x1, y1));
                }
                if (!includeEnd) line.Remove((x2, y2));

                return line.ToArray();
            }
            else
            {
                Debug.LogWarning($"Inputted Invalid XY ({x1}, {y1}; {x2}, {y2}) in GetPositionsInLine.");
                return default;
            }
        }
    }
}