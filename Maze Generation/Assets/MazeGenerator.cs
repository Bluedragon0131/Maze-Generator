﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    // dimensions of maze must be even or will be decremented to even
    public int mazeRows = 10;
    public int mazeColumns = 10;

    // cell prefab that contains location with walls
    [SerializeField] private GameObject cellPrefab;

    // disable main sprite so cell has no background
    public bool disableCellSprite = false;

    // list of cells to access all, the unvisited, and those placed on the stack to backtrack in case of locked location
    private Dictionary<Vector2, Cell> allCells = new Dictionary<Vector2, Cell>();
    private List<Cell> unvisited = new List<Cell>();
    private List<Cell> stack = new List<Cell>();

    // create the center to start
    private int centerSize = 2;
    private Cell[] centerCells = new Cell[4];

    // cell variables to hold currenet and checking cells
    private Cell currentCell;
    private Cell checkCell;

    // neighbor positions to randomly select the next cell
    private Vector2[] neighbourPositions = new Vector2[] { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, -1) };

    // size of cells
    private float cellSize;

    private GameObject mazeParent;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMaze(mazeRows, mazeColumns);
    }

    private void GenerateMaze(int rows, int columns)
    {
        if (mazeParent != null) DeleteMaze();

        mazeRows = rows;
        mazeColumns = columns;
        CreateLayout();
    }

    public void CreateLayout()
    {
        InitValues();

        Vector2 startPos = new Vector2(-(cellSize * (mazeColumns / 2)) + (cellSize / 2), -(cellSize * (mazeRows / 2)) + (cellSize / 2));
        Vector2 spawnPos = startPos;

        for (int x = 1; x <= mazeColumns; x++)
        {
            for (int y = 1; y <= mazeRows; y++)
            {
                GenerateCell(spawnPos, new Vector2(x, y));

                spawnPos.y += cellSize;
            }

            spawnPos.y = startPos.y;
            spawnPos.x += cellSize;
        }

        CreateCenter();
        RunAlgorithm();
        MakeExit();
    }

    public void RunAlgorithm()
    {
        unvisited.Remove(currentCell);

        while(unvisited.Count > 0)
        {
            List<Cell> unvisitedNeighbours = GetUnvisitedNeighbours(currentCell);
            if (unvisitedNeighbours.Count > 0)
            {
                checkCell = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];
                stack.Add(currentCell);
                CompareWalls(currentCell, checkCell);
                currentCell = checkCell;
                unvisited.Remove(currentCell);
            }
            else if (stack.Count > 0)
            {
                currentCell = stack[stack.Count - 1];
                stack.Remove(currentCell);
            }
        }
    }

    public void MakeExit()
    {
        List<Cell> edgeCells = new List<Cell>();

        foreach(KeyValuePair<Vector2, Cell> cell in allCells)
        {
            if (cell.Key.x == 0 || cell.Key.x == mazeColumns || cell.Key.y == 0 || cell.Key.y == mazeRows)
            {
                edgeCells.Add(cell.Value);
            }
        }

        Cell newCell = edgeCells[Random.Range(0, edgeCells.Count)];

        if (newCell.gridPos.x == 0) RemoveWall(newCell.cScript, 1);
        else if (newCell.gridPos.x == mazeColumns) RemoveWall(newCell.cScript, 2);
        else if (newCell.gridPos.y == mazeRows) RemoveWall(newCell.cScript, 3);
        else RemoveWall(newCell.cScript, 4);

        Debug.Log("Maze gereation finished");
    }

    public List<Cell> GetUnvisitedNeighbours(Cell curCell)
    {
        List<Cell> neighbours = new List<Cell>();
        Cell nCell = curCell;
        Vector2 cPos = curCell.gridPos;

        foreach (Vector2 p in neighbourPositions)
        {
            Vector2 nPos = cPos + p;
            if (allCells.ContainsKey(nPos)) nCell = allCells[nPos];
            if (unvisited.Contains(nCell)) neighbours.Add(nCell);
        }

        return neighbours;
    }

    public void CompareWalls(Cell cCell, Cell nCell)
    {
        if (nCell.gridPos.x < cCell.gridPos.x)
        {
            RemoveWall(nCell.cScript, 2);
            RemoveWall(cCell.cScript, 1);
        }
        else if (nCell.gridPos.x > cCell.gridPos.x)
        {
            RemoveWall(nCell.cScript, 1);
            RemoveWall(cCell.cScript, 2);
        }
        else if (nCell.gridPos.y > cCell.gridPos.y)
        {
            RemoveWall(nCell.cScript, 4);
            RemoveWall(cCell.cScript, 3);
        }
        else if (nCell.gridPos.y < cCell.gridPos.y)
        {
            RemoveWall(nCell.cScript, 3);
            RemoveWall(cCell.cScript, 4);
        }
    }

    public void RemoveWall(CellScript cScript, int wallID)
    {
        if (wallID == 1) cScript.wallL.SetActive(false);
        else if (wallID == 2) cScript.wallR.SetActive(false);
        else if (wallID == 3) cScript.wallU.SetActive(false);
        else if (wallID == 4) cScript.wallD.SetActive(false);
    }

    public void CreateCenter()
    {
        centerCells[0] = allCells[new Vector2((mazeColumns / 2), (mazeRows / 2) + 1)];
        RemoveWall(centerCells[0].cScript, 4);
        RemoveWall(centerCells[0].cScript, 2);
        centerCells[1] = allCells[new Vector2((mazeColumns / 2) + 1, (mazeRows / 2) + 1)];
        RemoveWall(centerCells[1].cScript, 4);
        RemoveWall(centerCells[1].cScript, 1);
        centerCells[2] = allCells[new Vector2((mazeColumns / 2), (mazeRows / 2))];
        RemoveWall(centerCells[2].cScript, 3);
        RemoveWall(centerCells[2].cScript, 2);
        centerCells[3] = allCells[new Vector2((mazeColumns / 2) + 1, (mazeRows / 2))];
        RemoveWall(centerCells[3].cScript, 3);
        RemoveWall(centerCells[3].cScript, 1);

        List<int> rndList = new List<int> { 0, 1, 2, 3 };
        int startCell = rndList[Random.Range(0, rndList.Count)];
        rndList.Remove(startCell);
        currentCell = centerCells[startCell];
        foreach(int c in rndList)
        {
            unvisited.Remove(centerCells[c]);
        }
    }

    public void GenerateCell(Vector2 pos, Vector2 keyPos)
    {
        Cell newCell = new Cell();

        newCell.gridPos = keyPos;
        newCell.cellObject = Instantiate(cellPrefab, pos, cellPrefab.transform.rotation);
        if (mazeParent != null) newCell.cellObject.transform.parent = mazeParent.transform;
        newCell.cellObject.name = "Cell - X: " + keyPos.x + " Y: " + keyPos.y;
        newCell.cScript = newCell.cellObject.GetComponent<CellScript>();
        if (disableCellSprite) newCell.cellObject.GetComponent<SpriteRenderer>().enabled = false;

        allCells[keyPos] = newCell;
        unvisited.Add(newCell);
    }

    public void DeleteMaze()
    {
        if (mazeParent != null) Destroy(mazeParent);
    }

    public void InitValues()
    {
        if (IsOdd(mazeRows)) mazeRows--;
        if (IsOdd(mazeColumns)) mazeColumns--;

        if (mazeRows < 4) mazeRows = 4;
        if (mazeColumns < 4) mazeColumns--;

        cellSize = cellPrefab.transform.localScale.x;

        mazeParent = new GameObject();
        mazeParent.transform.position = Vector2.zero;
        mazeParent.name = "Maze";
    }

    public bool IsOdd(int value)
    {
        return value % 2 != 0;
    }

    public class Cell
    {
        public Vector2 gridPos;
        public GameObject cellObject;
        public CellScript cScript;
    }
}
