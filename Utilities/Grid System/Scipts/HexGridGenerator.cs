using UnityEngine;

namespace Utilities.GridSystem
{
    public class HexGridGenerator<T> : AGridGenerator<T>
    {
        [Space]
        [SerializeField] float tileSizeX = 1.6f;
        [SerializeField] float tileSizeZ = 1.88f;

        private void Awake()
        {
            movementManager.SetGridGen(this);
            InstantiateGrid();
            InstantiateTileGOs();
        }

        internal override void InstantiateGrid()
        {
            grid = new HexGrid<T>(gridWidth, gridHeight);

            movementManager.transforms = new Transform[gridWidth, 2 * gridHeight];
            gridPathfinding = new GridPathfinding(new HexGrid<PathNode>(gridWidth, gridHeight));

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < 2 * gridHeight; y++)
                {
                    if ((x + y) % 2 == 0)
                    {
                        int nY = x % 2 == 0 ? y / 2 : (y - 1) / 2;
                        grid.SetObject(x, y, startingState[x][nY].value);

                        movementManager.transforms[x, nY] = startingState[x][nY].transform;
                        gridPathfinding.grid.SetObject(x, y, new PathNode(x, y, !startingState[x][nY].unwalkable));
                    }
                }
            }
        }

        internal override void InstantiateTileGOs()
        {
            tileGOs = new GameObject[gridWidth, 2 * gridHeight];
            tileMRs = new MeshRenderer[gridWidth, 2 * gridHeight];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < 2 * gridHeight; y++)
                {
                    if ((x + y) % 2 == 0)
                    {
                        tileGOs[x, y] = Instantiate(tilePrefab, XYToWorldSpace(x, y), Quaternion.identity, transform);
                        tileMRs[x, y] = tileGOs[x, y].GetComponentInChildren<MeshRenderer>();

                        Transform tr = movementManager.transforms[x, y];
                        if (tr != null)
                            tr.position = movementManager.XYToObjectTransformPosition(x, y);
                    }
                }
            }
        }

        internal override bool ValidXY(int x, int y)
        {
            return x >= 0 && x < gridWidth && y >= 0 && y < 2 * gridHeight && (x + y) % 2 == 0;
        }

        public override (int, int) WorldSpaceToXY(Vector3 worldPos)
        {
            Vector3 pos = transform.InverseTransformPoint(worldPos);

            int x = Mathf.FloorToInt(pos.x / tileSizeX);
            int y;

            if (x % 2 == 0)
            {
                y = Mathf.FloorToInt(pos.z * 2 / tileSizeZ);
            }
            else
            {
                y = Mathf.FloorToInt(pos.z / tileSizeZ * 2);
            }

            if (!ValidXY(x, y))
            {
                x = 0;
                y = 0;

                Debug.LogWarning($"Received out of range coordinates ({worldPos}).");
            }
            return (x, y);
        }

        public override Vector3 XYToWorldSpace(int x, int y)
        {
            Vector3 pos;

            if (x % 2 == 0)
            {
                // Regular Column
                pos = new Vector3(x * tileSizeX, 0, y * tileSizeZ / 2);
            }
            else
            {
                // Offset Column
                pos = new Vector3(x * tileSizeX, 0, (y / 2 + 0.5f) * tileSizeZ);
            }
            return transform.TransformPoint(pos);
        }

        public override GameObject GetTileGO(int x, int y)
        {
            if (ValidXY(x, y))
            {
                return tileGOs[x, y];
            }
            else
            {
                Debug.LogWarning($"Inputted invalid XY ({x}, {y}) in GetTileGO.");
                return null;
            }
        }
    }
}