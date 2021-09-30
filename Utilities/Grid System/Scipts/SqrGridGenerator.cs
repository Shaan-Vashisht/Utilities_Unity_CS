using UnityEngine;

namespace Utilities.GridSystem
{
    public class SqrGridGenerator<T> : AGridGenerator<T>
    {
        [Space]
        [Tooltip("Wether tiles that are diagonal to each other are adiacent or not.")]
        [SerializeField] bool adiacentDiagonals;
        [SerializeField] float tileSize;

        private void Awake()
        {
            movementManager.SetGridGen(this);
            InstantiateGrid();
            InstantiateTileGOs();
        }

        internal override void InstantiateGrid()
        {
            grid = new SqrGrid<T>(gridWidth, gridHeight, adiacentDiagonals);

            movementManager.transforms = new Transform[gridWidth, gridHeight];
            gridPathfinding = new GridPathfinding(new SqrGrid<PathNode>(gridWidth, gridHeight, adiacentDiagonals));

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    grid.SetObject(x, y, startingState[x][y].value);

                    movementManager.transforms[x, y] = startingState[x][y].transform;
                    gridPathfinding.grid.SetObject(x, y, new PathNode(x, y, !startingState[x][y].unwalkable));
                }
            }
        }

        internal override void InstantiateTileGOs()
        {
            tileGOs = new GameObject[gridWidth, gridHeight];
            tileMRs = new MeshRenderer[gridWidth, gridHeight];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    tileGOs[x, y] = Instantiate(tilePrefab, XYToWorldSpace(x, y), Quaternion.identity, transform);
                    tileMRs[x, y] = tileGOs[x, y].GetComponentInChildren<MeshRenderer>();

                    Transform tr = movementManager.transforms[x, y];
                    if (tr != null)
                        tr.position = movementManager.XYToObjectTransformPosition(x, y);
                }
            }
        }

        internal override bool ValidXY(int x, int y)
        {
            return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
        }

        public override Vector3 XYToWorldSpace(int x, int y)
        {
            Vector3 pos = new Vector3(x * tileSize, 0, y * tileSize);
            return transform.TransformPoint(pos);
        }

        public override (int, int) WorldSpaceToXY(Vector3 worldPos)
        {
            Vector3 pos = transform.InverseTransformPoint(worldPos);

            int x = Mathf.FloorToInt(pos.x / tileSize);
            int y = Mathf.FloorToInt(pos.z / tileSize);

            if (!ValidXY(x, y))
            {
                x = 0;
                y = 0;

                Debug.LogWarning($"Received out of range coordinates ({worldPos}).");
            }
            return (x, y);
        }
    }
}