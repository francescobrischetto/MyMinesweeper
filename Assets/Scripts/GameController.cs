using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int width = 16;
    public int height = 16;
    public int mineCount = 32;
    public Board board;
    private Cell[,] state;

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        state = new Cell[width, height];
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        GenerateCells();
        GenerateMines();
        GenerateNumbers();
        board.Draw(state);
    }

    private void GenerateCells()
    {
        for(int x=0; x < width; x++)
        {
            for(int y=0; y<height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                cell.type = Cell.Type.Empty;
                state[x, y] = cell;
            }
        }
    }

    private void GenerateMines()
    {
        for(int i=0; i < mineCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            if (state[x,y].type == Cell.Type.Mine)      i--;
            state[x, y].type = Cell.Type.Mine;
        }
    }

    private void GenerateNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                if (cell.type == Cell.Type.Mine) continue;
                cell.number = CountAdjacentsMines(x, y);
                if (cell.number > 0) cell.type = Cell.Type.Number;
                state[x, y] = cell;
            }
        }
    }

    private int CountAdjacentsMines(int cellX, int cellY)
    {
        int count = 0;
        for(int offX=-1; offX<=1; offX++)
        {
            for (int offY = -1; offY <= 1; offY++)
            {
                if (offX == 0 && offY == 0) continue;
                int adjacentX = offX + cellX;
                int adjacentY = offY + cellY;
                if(!IsValidPosition(adjacentX, adjacentY)) continue;
                if (state[adjacentX, adjacentY].type == Cell.Type.Mine) count++;
            }
        }
        return count;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            FlagTile();
        }
    }

    private void FlagTile()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCellFromPosition(cellPosition.x, cellPosition.y);
        if (cell.type == Cell.Type.Invalid || cell.revealed) return;
        cell.flagged = !cell.flagged;
        state[cellPosition.x, cellPosition.y] = cell;
        board.Draw(state);
    }

    private Cell GetCellFromPosition(int x, int y)
    {
        if (IsValidPosition(x, y)) return state[x, y];
        else
        {
            Cell invalidCell = new Cell();
            invalidCell.type = Cell.Type.Invalid;
            return invalidCell;
        }
    }

    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    
}
