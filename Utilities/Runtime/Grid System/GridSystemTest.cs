using System.Collections.Generic;
using UnityEngine;
using Utilities.GridSystem;

public class GridSystemTest : MonoBehaviour
{
    public Camera cam;
    public AGridGenerator<int> gridGen;

    public enum TestType { Unconditioned, Ranged, Unoccupied, PathFind }
    public TestType testType;
    public int range;
    public Color selectedColor;
    public Color otherColor;
    
    List<MeshRenderer> tileMRs = new List<MeshRenderer>();
    (int, int) firstSelectedPos = (-1, -1);
    (int, int) secondSelectedPos = (-1, -1);

    private void Start()
    {
        for (int x = 0; x < gridGen.Pathfind.grid.width; x++)
        {
            for (int y = 0; y < gridGen.Pathfind.grid.height; y++)
            {
                if (gridGen.Pathfind.grid.ValidXY(x, y))
                {
                    if (!gridGen.Pathfind.grid.GetObject(x, y).isWalkable)
                    {
                        gridGen.GetTileMR(x, y).material.color = Color.black;
                    }
                }
            }
        }
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                Test(hit);
            }
        }
    }

    void Test(RaycastHit hit)
    {
        if (firstSelectedPos == (-1, -1))
        {
            firstSelectedPos = SelectTile(hit);

            if (testType == TestType.Ranged)
                HighlightTiles(gridGen.Grid.GetPositionsInRange(firstSelectedPos, range));
        }
        else if (secondSelectedPos == (-1, -1))
        {
            if (testType == TestType.Unconditioned)
            {
                secondSelectedPos = SelectTile(hit);

                (int, int)[] linepos = gridGen.Grid.GetPositionsInLine(firstSelectedPos, secondSelectedPos);
                HighlightTiles(linepos);
            }
            else if (testType == TestType.Ranged)
            {
                if (gridGen.Grid.GetDistance(firstSelectedPos, gridGen.WorldSpaceToXY(hit.transform.position)) <= range)
                {
                    secondSelectedPos = SelectTile(hit);
                }
                else Debug.Log("Tile out of range.");
            }
            else if (testType == TestType.Unoccupied)
            {
                if (gridGen.Movement.CheckEmpty(gridGen.WorldSpaceToXY(hit.transform.position)))
                {
                    secondSelectedPos = SelectTile(hit);

                    (int, int)[] linepos = gridGen.Grid.GetPositionsInLine(firstSelectedPos, secondSelectedPos);

                    foreach ((int, int) pos in linepos)
                    {
                        if (!gridGen.Movement.CheckEmpty(pos))
                        {
                            Debug.Log("Tile in path occupied");
                            ClearTiles();
                            return;
                        }
                    }
                    HighlightTiles(linepos);
                }
                else Debug.Log("Tile occupied");
            }
            else if (testType == TestType.PathFind)
            {
                secondSelectedPos = SelectTile(hit);

                (int, int)[] pathpos = gridGen.Pathfind.GetPath(firstSelectedPos, secondSelectedPos);
                if (pathpos == null)
                {
                    Debug.Log("No path was found...");
                    ClearTiles();
                    return;
                }
                HighlightTiles(pathpos);
            }
        }
        else
        {
            if (testType != TestType.PathFind)
                gridGen.Movement.MoveTransformInLine(firstSelectedPos, secondSelectedPos, 1f, .1f, () => { ClearTiles(); });
            else 
                gridGen.Movement.MoveTransformInPath(firstSelectedPos, secondSelectedPos, 1f, .1f, () => { ClearTiles(); });
        }
    }
    void TestMovementUnconditioned(RaycastHit hit)
    {
        if (firstSelectedPos == (-1, -1))
        {
            firstSelectedPos = SelectTile(hit);
        }
        else if (secondSelectedPos == (-1, -1))
        {
            secondSelectedPos = SelectTile(hit);

            (int, int)[] linepos = gridGen.Grid.GetPositionsInLine(firstSelectedPos, secondSelectedPos);
            HighlightTiles(linepos);
        }
        else
        {
            gridGen.Movement.MoveTransformInLine(firstSelectedPos, secondSelectedPos, 2f, .1f, () => { ClearTiles(); });
        }
    }

    void TestMoveRanged(RaycastHit hit)
    {
        if (firstSelectedPos == (-1, -1))
        {
            firstSelectedPos = SelectTile(hit);

            HighlightTiles(gridGen.Grid.GetPositionsInRange(firstSelectedPos, range));
        }
        else if (secondSelectedPos == (-1, -1))
        {
            if (gridGen.Grid.GetDistance(firstSelectedPos, gridGen.WorldSpaceToXY(hit.transform.position)) <= range)
            {
                secondSelectedPos = SelectTile(hit);
            }
            else Debug.Log("Tile out of range.");
        }
        else
        {
            gridGen.Movement.MoveTransformInLine(firstSelectedPos, secondSelectedPos, 2f, .1f, () => { ClearTiles(); });
        }
    }

    void TestMoveUnoccupied(RaycastHit hit)
    {
        if (firstSelectedPos == (-1, -1))
        {
            firstSelectedPos = SelectTile(hit);
        }
        else if (secondSelectedPos == (-1, -1))
        {
            if (gridGen.Movement.CheckEmpty(gridGen.WorldSpaceToXY(hit.transform.position)))
            {
                secondSelectedPos = SelectTile(hit);

                (int, int)[] linepos = gridGen.Grid.GetPositionsInLine(firstSelectedPos, secondSelectedPos);

                foreach ((int, int) pos in linepos)
                {
                    if (!gridGen.Movement.CheckEmpty(pos))
                    {
                        Debug.Log("Tile in path occupied");
                        ClearTiles();
                        return;
                    }
                }
                HighlightTiles(linepos);
            }
            else Debug.Log("Tile occupied");
        }
        else
        {
            gridGen.Movement.MoveTransformInLine(firstSelectedPos, secondSelectedPos, 2f, .1f, () => { ClearTiles(); });
        }
    }

    private void HighlightTiles((int, int)[] pos)
    {
        for (int i = 0; i < pos.Length; i++)
        {
            MeshRenderer rTileMR = gridGen.GetTileMR(pos[i]);
            rTileMR.material.color = otherColor;
            tileMRs.Add(rTileMR);
        }
    }

    private void ClearTiles()
    {
        foreach (MeshRenderer mr in tileMRs)
        {
            mr.material.color = Color.white;
        }
        tileMRs.Clear();

        firstSelectedPos = (-1, -1);
        secondSelectedPos = (-1, -1);
    }

    private (int, int) SelectTile(RaycastHit hit)
    {
        (int, int) hitPos = gridGen.WorldSpaceToXY(hit.transform.position);

        MeshRenderer tileMR = gridGen.GetTileMR(hitPos);
        tileMR.material.color = selectedColor;
        tileMRs.Add(tileMR);

        return hitPos;
    }
}
