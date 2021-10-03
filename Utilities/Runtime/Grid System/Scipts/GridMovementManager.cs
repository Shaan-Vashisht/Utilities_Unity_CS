using UnityEngine;
using System;

namespace Utilities.GridSystem
{
    [Serializable]
    public class GridMovementManager<T>
    {
        [SerializeField] float transformHover = 1f;
        [SerializeField] float tweeningTolerance = 0.2f;

        internal Transform[,] transforms;

        AGridGenerator<T> gridGen;

        public void SetGridGen(AGridGenerator<T> gridGen)
        {
            if (this.gridGen == null) this.gridGen = gridGen;
        }

        public bool CheckEmpty((int, int) pos)
        {
            if (gridGen.ValidXY(pos))
            {
                return transforms[pos.Item1, pos.Item2] == null;
            }
            return false;
        }

        /// <summary>
        /// Teleports the transform at startPos to endPos.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="callbacks"></param>
        public void MoveTransform((int, int) startPos, (int, int) endPos, params Action[] callbacks)
        {
            if (gridGen.ValidXY(startPos) && gridGen.ValidXY(endPos))
            {
                Transform tr = transforms[startPos.Item1, startPos.Item2];

                if (tr == null) return;

                gridGen.Grid.SwapObjects(startPos, endPos);

                SwapTransforms(startPos, endPos);

                foreach (Action callback in callbacks)
                {
                    callback?.Invoke();
                }
            }
        }

        /// <summary>
        /// Tweens the transform at startPos to endPos.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="time"></param>
        /// <param name="callbacks"></param>
        public void MoveTransformSmooth((int, int) startPos, (int, int) endPos, float time, params Action[] callbacks)
        {
            if (gridGen.ValidXY(startPos) && gridGen.ValidXY(endPos))
            {
                Transform tr = transforms[startPos.Item1, startPos.Item2];

                if (tr == null) return;

                gridGen.Grid.SwapObjects(startPos, endPos);

                TweenManager.i.TweenVector3(tr.position, XYToObjectTransformPosition(endPos), time, tweeningTolerance,
                    (Vector3 newpos) => { tr.position = newpos; },
                    () => { SwapTransforms(startPos, endPos); });

                foreach (Action callback in callbacks)
                {
                    callback?.Invoke();
                }
            }
        }

        /// <summary>
        /// Tweens the transform at startPos to endPos in a line within the grid.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="time"></param>
        /// <param name="wait"></param>
        /// <param name="callbacks"></param>
        public void MoveTransformInLine((int, int) startPos, (int, int) endPos, float time, float wait, params Action[] callbacks)
        {
            if (gridGen.ValidXY(startPos) && gridGen.ValidXY(endPos))
            {
                Transform tr = transforms[startPos.Item1, startPos.Item2];

                if (tr == null) return;

                (int, int)[] line = gridGen.Grid.GetPositionsInLine(startPos, endPos, false, true);
                Vector3[] sequence = new Vector3[line.Length];
                for (int i = 0; i < line.Length; i++)
                {
                    sequence[i] = XYToObjectTransformPosition(line[i]);
                }

                gridGen.Grid.SwapObjects(startPos, endPos);

                TweenManager.i.TweenVector3Sequence(tr.position, sequence, time, wait, tweeningTolerance,
                    (Vector3 newpos) => { tr.position = newpos; },
                    () => { SwapTransforms(startPos, endPos); });

                foreach (Action callback in callbacks)
                {
                    callback?.Invoke();
                }
            }
        }

        /// <summary>
        /// Tweens the transform at startPos to endPos along an A* path.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="time"></param>
        /// <param name="wait"></param>
        /// <param name="callbacks"></param>
        public void MoveTransformInPath((int, int) startPos, (int, int) endPos, float time, float wait, params Action[] callbacks)
        {
            if (gridGen.ValidXY(startPos) && gridGen.ValidXY(endPos))
            {
                Transform tr = transforms[startPos.Item1, startPos.Item2];

                if (tr == null) return;

                (int, int)[] path = gridGen.Pathfind.GetPath(startPos, endPos);
                Vector3[] sequence = new Vector3[path.Length];
                for (int i = 0; i < path.Length; i++)
                {
                    sequence[i] = XYToObjectTransformPosition(path[i]);
                }

                gridGen.Grid.SwapObjects(startPos, endPos);

                TweenManager.i.TweenVector3Sequence(tr.position, sequence, time, wait, tweeningTolerance,
                    (Vector3 newpos) => { tr.position = newpos; },
                    () => { SwapTransforms(startPos, endPos); });

                foreach (Action callback in callbacks)
                {
                    callback?.Invoke();
                }
            }
        }

        private void SwapTransforms((int, int) pos1, (int, int) pos2)
        {
            Transform temp = transforms[pos1.Item1, pos1.Item2];
            transforms[pos1.Item1, pos1.Item2] = transforms[pos2.Item1, pos2.Item2];
            transforms[pos2.Item1, pos2.Item2] = temp;

            Transform t1 = transforms[pos1.Item1, pos1.Item2];
            Transform t2 = transforms[pos2.Item1, pos2.Item2];

            if (t1 != null)
                t1.position = XYToObjectTransformPosition(pos1.Item1, pos1.Item2);
            if (t2 != null)
                t2.position = XYToObjectTransformPosition(pos2.Item1, pos2.Item2);
        }

        internal Vector3 XYToObjectTransformPosition(int x, int y)
        {
            return gridGen.GetTileGO(x, y).transform.GetChild(0).position + new Vector3(0, transformHover, 0);
        }
        internal Vector3 XYToObjectTransformPosition((int, int) pos)
        {
            return XYToObjectTransformPosition(pos.Item1, pos.Item2);
        }
    }
}
