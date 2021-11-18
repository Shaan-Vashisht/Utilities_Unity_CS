using UnityEngine;

namespace Utilities.GridSystem
{
    public class SqrGridGenerator<T> : AGridGenerator<T>
    {
        [Space]
        [Tooltip("Wether tiles that are diagonal to each other are adiacent or not.")]
        [SerializeField] bool adiacentDiagonals;
        [SerializeField] float tileSize;

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
            grid = new SqrGrid<T>(gridWidth, gridHeight, adiacentDiagonals);

            movementManager.transforms = new Transform[gridWidth, gridHeight];
            gridPathfinding = new GridPathfinding(new SqrGrid<PathNode>(gridWidth, gridHeight, adiacentDiagonals));

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    SetGridObject(x, y);
                }
            }

            OnGridGenerated?.Invoke();
        }

        protected override void SetGridObject(int x, int y)
        {
            grid.SetObject(x, y, startingState[x][y].value);

            movementManager.transforms[x, y] = startingState[x][y].transform;
            gridPathfinding.grid.SetObject(x, y, new PathNode(x, y, !startingState[x][y].unwalkable));
        }

        protected override void InstantiateTileGOs()
        {
            tileGOs = new GameObject[gridWidth, gridHeight];
            tileMRs = new MeshRenderer[gridWidth, gridHeight];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    InstantiateTile(x, y);
                }
            }

            OnTilesInstantiated?.Invoke();
        }

        public override bool ValidXY(int x, int y)
        {
            return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
        }

        public override Vector3 XYToWorldSpace(int x, int y)
        {
            Vector3 pos = new Vector3(x * tileSize, 0, y * tileSize);
            return transform.TransformPoint(pos);
        }

        public override Vector3 XYToTileCenter(int x, int y, float hover)
        {
            return new Vector3((x + 0.5f) * tileSize, hover, (y + 0.5f) * tileSize);
        }

        public override (int, int) WorldSpaceToXY(Vector3 worldPos)
        {
            Vector3 pos = transform.InverseTransformPoint(worldPos);

            int x = Mathf.FloorToInt(pos.x / tileSize);
            int y = Mathf.FloorToInt(pos.z / tileSize);

            if (!ValidXY(x, y))
            {
                Debug.LogWarning($"Received out of range coordinates ({worldPos}->{x}, {y}).");

                x = 0;
                y = 0;
            }
            return (x, y);
        }
    }
}