using System.Collections.Generic;
using System;
using UnityEngine;

namespace Utilities.GridSystem
{
    public class SqrGrid<T> : AGrid<T>
    {
        public bool adiacentDiagonals { get; }

        public SqrGrid(int width, int height, bool adiacentDiagonals)
        {
            this.width = width;
            this.height = height;

            gridArray = new T[width, height];

            this.adiacentDiagonals = adiacentDiagonals;
        }

        public override int GetDistance(int x1, int y1, int x2, int y2)
        {
            if (adiacentDiagonals)
                // Box distance
                return Math.Max(Mathf.Abs(x1 - x2), Mathf.Abs(y1 - y2));
            else
                // Taxicab distance
                return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
        }

        public override (int, int)[] GetPositionsInRange(int x, int y, int range, bool includeStart = false)
        {
            if (ValidXY(x, y) && range > 0)
            {
                List<(int, int)> positions = new List<(int, int)>();

                // Loops through the position delta
                for (int dx = -range; dx <= range; dx++)
                {
                    for (int dy = -range; dy <= range; dy++)
                    {
                        // Adds the delta to the starting position to get the new position
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
                bool goVertical = Math.Abs(y1 - y2) > Math.Abs(x1 - x2);

                if (includeStart) line.Add((x1, y1));
                while (x1 != x2 || y1 != y2)
                {
                    // Gets the total difference between current position and end
                    int dx = x2 - x1;
                    int dy = y2 - y1;

                    // Gets the new step from the sign of the difference
                    int mx = Math.Sign(dx);
                    int my = Math.Sign(dy);

                    if (adiacentDiagonals)
                    {
                        // Can move in both directions at the same time
                        x1 += mx;
                        y1 += my;
                    }
                    else
                    {
                        // Can move in one direction at a time, alternates between 
                        // vertical and horizontal movement to create a smoother line
                        if (goVertical && my != 0)
                        {
                            y1 += my;
                            goVertical = false;
                        }
                        else
                        {
                            x1 += mx;
                            goVertical = true;
                        }
                    }

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