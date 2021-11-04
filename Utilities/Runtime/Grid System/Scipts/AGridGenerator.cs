using UnityEngine;
using Utilities.Objects;

namespace Utilities.GridSystem
{
    public abstract class AGridGenerator<T> : MonoBehaviour
    {
        [SerializeField] protected int gridWidth;
        [SerializeField] protected int gridHeight;

        protected AGrid<T> grid;
        public AGrid<T> Grid { get => grid; }

        [Tooltip("The elements in startingState represent the columns starting from the left, " +
            "the elements in the arrays represent the tiles starting from the bottom.")]
        [SerializeField] protected SerializableArray<TileValues<T>>[] startingState = { new SerializableArray<TileValues<T>>(0) };
        [SerializeField] protected GameObject tilePrefab;
        protected GameObject[,] tileGOs;
        protected MeshRenderer[,] tileMRs;
        [Space]
        [Space]
        [SerializeField] protected GridMovementManager<T> movementManager;
        public GridMovementManager<T> Movement { get => movementManager; }

        protected GridPathfinding gridPathfinding;
        public GridPathfinding Pathfind { get => gridPathfinding; }

        protected abstract void InstantiateGrid();
        protected abstract void SetGridObject(int x, int y);
        protected abstract void InstantiateTileGOs();
        protected virtual void InstantiateTile(int x, int y)
        {
            tileGOs[x, y] = Instantiate(tilePrefab, XYToWorldSpace(x, y), Quaternion.identity, transform);
            tileMRs[x, y] = tileGOs[x, y].GetComponentInChildren<MeshRenderer>();

            Transform tr = movementManager.transforms[x, y];
            if (tr != null)
                tr.position = movementManager.XYToObjectTransformPosition(x, y);
        }

        protected internal abstract bool ValidXY(int x, int y);
        protected internal bool ValidXY((int, int) pos)
        {
            return ValidXY(pos.Item1, pos.Item2);
        }

        public abstract Vector3 XYToWorldSpace(int x, int y);
        public virtual Vector3 XYToWorldSpace((int, int) pos)
        {
            return XYToWorldSpace(pos.Item1, pos.Item2);
        }

        public abstract Vector3 XYToTileCenter(int x, int y, float hover);
        public virtual Vector3 XYToTileCenter((int, int) pos, float hover)
        {
            return XYToTileCenter(pos.Item1, pos.Item2, hover);
        }

        public abstract (int, int) WorldSpaceToXY(Vector3 worldPos);

        public virtual GameObject GetTileGO(int x, int y)
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
        public virtual GameObject GetTileGO((int, int) pos)
        {
            return GetTileGO(pos.Item1, pos.Item2);
        }

        public virtual MeshRenderer GetTileMR(int x, int y)
        {
            if (ValidXY(x, y))
            {
                return tileMRs[x, y];
            }
            else
            {
                Debug.LogWarning($"Inputted invalid XY ({x}, {y}) in GetTileGO.");
                return null;
            }
        }
        public virtual MeshRenderer GetTileMR((int, int) pos)
        {
            return GetTileMR(pos.Item1, pos.Item2);
        }

        private void OnValidate()
        {
            //Validate2DArray(ref startingState, gridHeight, gridWidth);
            ObjectUtilities.Lock2DArrayLength(ref startingState, gridWidth, gridHeight);
        }
    }

    [System.Serializable]
    public class TileValues<T>
    {
        public T value;
        public Transform transform;
        public bool unwalkable;
    }
}