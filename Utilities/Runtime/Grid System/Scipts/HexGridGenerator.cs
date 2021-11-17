using UnityEngine;

namespace Utilities.GridSystem
{
    public class HexGridGenerator<T> : AGridGenerator<T>
    {
        [Space]
        [SerializeField] protected float tileSizeX = 1.6f;
        [SerializeField] protected float tileSizeZ = 1.88f;
        [SerializeField] protected Vector3 tileCenterDelta = new Vector3(1f, 0f, 0.866f);

        public float TileToTileDist { get => tileSizeX; }

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

            //Approximates the hex grid to a rectangle grid
            int rawX = Mathf.FloorToInt(pos.x / tileSizeX);
            int columnParity = rawX % 2;
            int rawY = Mathf.FloorToInt((pos.z - (columnParity * tileSizeZ / 2)) / tileSizeZ) * 2 + columnParity;

            //Calculates the relative position of the point within the tile rectangle
            float relX = pos.x - rawX * tileSizeX;
            float relY;
            if (columnParity == 0)
                relY = pos.z - (rawY * tileSizeZ / 2);
            else
                relY = pos.z - (rawY / 2 + 0.5f) * tileSizeZ;

            if (relY < 0)
                relY += tileSizeZ;

            int x = rawX;
            int y = rawY;
            //The point is in the upper half of the rectangle
            if (relY >= tileSizeZ / 2)
            {
                //The point is above the hexagon's diagonal side,
                //so it's actually in the tile up and to the left
                if (relY > (1.732f * relX) + (tileSizeZ / 2))
                {
                    x--;
                    y++;
                }
            }
            else
            {
                //The point is below the hexagon's diagonal side,
                //so it's actually in the tile down and to the left
                if (relY < (-1.732f * relX) + (tileSizeZ / 2))
                {
                    x--;
                    y--;
                }
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
            return tileParent + new Vector3(tileCenterDelta.x, hover, tileCenterDelta.z);
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