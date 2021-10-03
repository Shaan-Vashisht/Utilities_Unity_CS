using System;
using System.Collections.Generic;


namespace Utilities.GridSystem
{
    public class GridPathfinding
    {
        internal AGrid<PathNode> grid;

        private List<PathNode> openList;
        private List<PathNode> closedList;

        private bool isSqrWithDiagonals = false;

        public GridPathfinding(AGrid<PathNode> grid)
        {
            this.grid = grid;

            if (grid.GetType().IsAssignableFrom(typeof(SqrGrid<PathNode>)))
                isSqrWithDiagonals = ((SqrGrid<PathNode>)grid).adiacentDiagonals;
        }

        public (int, int)[] GetPath(int x1, int y1, int x2, int y2)
        {
            PathNode startNode = grid.GetObject(x1, y1);
            PathNode endNode = grid.GetObject(x2, y2);

            openList = new List<PathNode> { startNode };
            closedList = new List<PathNode>();

            for (int x = 0; x < grid.width; x++)
            {
                for (int y = 0; y < grid.height; y++)
                {
                    if (grid.ValidXY(x, y))
                    {
                        PathNode pathNode = grid.GetObject(x, y);
                        pathNode.gCost = int.MaxValue;
                        pathNode.CalculateFCost();
                        pathNode.cameFromNode = null;
                    }
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateHCost((x1, y1), (x2, y2));
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(openList);

                if (currentNode == endNode) return CalculatePath(endNode);

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode node in grid.GetObjects(grid.GetPositionsInRange(currentNode.pos, 1)))
                {
                    if (closedList.Contains(node)) continue;
                    if (!node.isWalkable)
                    {
                        closedList.Add(node);
                        continue;
                    }

                    int tentativeGCost = currentNode.gCost + grid.GetDistance(currentNode.pos, node.pos);
                    if (tentativeGCost < node.gCost)
                    {
                        node.cameFromNode = currentNode;
                        node.gCost = tentativeGCost;
                        node.hCost = CalculateHCost(node.pos, endNode.pos);
                        node.CalculateFCost();

                        if (!openList.Contains(node)) openList.Add(node);
                    }
                }
            }

            return null;
        }
        public (int, int)[] GetPath((int, int) pos1, (int, int) pos2)
        {
            return GetPath(pos1.Item1, pos1.Item2, pos2.Item1, pos2.Item2);
        }

        private PathNode GetLowestFCostNode(List<PathNode> list)
        {
            PathNode lowestCostNode = list[0];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].fCost < lowestCostNode.fCost) lowestCostNode = list[i];
            }
            return lowestCostNode;
        }

        private (int, int)[] CalculatePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            path.Add(endNode);
            PathNode currentNode = endNode;

            while (currentNode.cameFromNode != null)
            {
                path.Add(currentNode.cameFromNode);
                currentNode = currentNode.cameFromNode;
            }
            path.Reverse();

            (int, int)[] pathPos = new (int, int)[path.Count];
            for (int i = 0; i < path.Count; i++)
            {
                pathPos[i] = path[i].pos;
            }
            return pathPos;
        }

        private int CalculateHCost((int, int) pos1, (int, int) pos2)
        {
            if (isSqrWithDiagonals)
            {
                int dx = Math.Abs(pos1.Item1 - pos2.Item1);
                int dy = Math.Abs(pos1.Item2 - pos2.Item2);
                int remaining = Math.Abs(dx - dy);
                return Math.Min(dx, dy) * 14 + remaining * 10;
            }
            else
                return grid.GetDistance(pos1, pos2);
        }
    }

    public class PathNode
    {
        public int x;
        public int y;
        public (int, int) pos;

        public bool isWalkable;
        public int gCost;
        public int hCost;
        public int fCost;

        public PathNode cameFromNode;

        public PathNode(int x, int y, bool v)
        {
            this.x = x;
            this.y = y;
            pos = (x, y);
            this.isWalkable = v;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}