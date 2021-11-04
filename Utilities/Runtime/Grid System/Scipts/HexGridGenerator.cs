using UnityEngine;

namespace Utilities.GridSystem
{
    public class HexGridGenerator<T> : AGridGenerator<T>
    {
        [Space]
        [SerializeField] float tileSizeX = 1.6f;
        [SerializeField] float tileSizeZ = 1.88f;

        public event System.Action OnGridGenerated;
        public event System.Action OnTilesInstantiated;

        private void Awake()
        {
            movementManager.SetGridGen(this);
            InstantiateGrid();
            InstantiateTileGOs();
        }

        protected override void InstantiateGrid()
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
                        SetGridObject(x, y);
                    }
                }
            }

            OnGridGenerated?.Invoke();
        }

        protected override void SetGridObject(int x, int y)
        {
            int nY = x % 2 == 0 ? y / 2 : (y - 1) / 2;
            grid.SetObject(x, y, startingState[x][nY].value);

            movementManager.transforms[x, nY] = startingState[x][nY].transform;
            gridPathfinding.grid.SetObject(x, y, new PathNode(x, y, !startingState[x][nY].unwalkable));
        }

        protected override void InstantiateTileGOs()
        {
            tileGOs = new GameObject[gridWidth, 2 * gridHeight];
            tileMRs = new MeshRenderer[gridWidth, 2 * gridHeight];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < 2 * gridHeight; y++)
                {
                    if ((x + y) % 2 == 0)
                    {
                        InstantiateTile(x, y);
                    }
                }
            }

            OnTilesInstantiated?.Invoke();
        }

        protected internal override bool ValidXY(int x, int y)
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
                y = Mathf.RoundToInt((pos.z * 2 / tileSizeZ) - 1);
            }

            if (!ValidXY(x, y))
            {
                Debug.LogWarning($"Received out of range coordinates ({worldPos}->{x}, {y}).");

                x = 0;
                y = 0;
            }
            return (x, y);
        }

        public override Vector3 XYToTileCenter(int x, int y, float hover)
        {
            Vector3 tileParent = XYToWorldSpace(x, y);
            return tileParent + new Vector3(1, hover, 0.86f);
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