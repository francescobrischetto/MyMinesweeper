using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private int width = 9;
    private int height = 9;
    private int minesInBoard = 10;
    public MinesweeperBoard board;
    private MinesweeperCell[,] gameState;
    private bool gameOver;


    public void Start()
    {
        NewGame();
    }

    public void Update()
    {
        //Restarting the game
        if (Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }
        //Handle mouse clicking for Flagging or Revealing a Tile
        else if (!gameOver)
        {
            if (Input.GetMouseButtonDown(1))
            {
                FlagTile();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                RevealTile();
            }
        }
    }

    //Creating a new game with all the previously set parameters
    private void NewGame()
    {
        gameState = new MinesweeperCell[width, height];
        //Centering the camera
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        gameOver = false;
        //Generating all the empty Cells
        GenerateCells();
        //Randomly generating the mines
        GenerateMines();
        //Changing the empty cells to the right number cell (if any mines is in neighbourhood)
        CalculateNumbersBasedOnNeighborhood();
        //Drawing the board
        board.Draw(gameState);
    }

    //Function that generates all the empty Cells
    private void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                MinesweeperCell cell = new MinesweeperCell();
                cell.type = MinesweeperCell.Type.Empty;
                cell.position = new Vector3Int(x, y, 0);
                gameState[x, y] = cell;
            }
        }
    }

    //Function that randomly generates the mines
    private void GenerateMines()
    {
        for (int i = 0; i < minesInBoard; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            //if we choosed a random position that already has a Mine, we loop back, decrementing i
            if (gameState[x, y].type == MinesweeperCell.Type.Mine)
                i--;
            else gameState[x, y].type = MinesweeperCell.Type.Mine;
        }
    }

    //Function that set the number of mines according to the adjacent cell mines
    private void CalculateNumbersBasedOnNeighborhood()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                MinesweeperCell cell = gameState[x, y];
                //If it is a mine, we don't need to count the adjacent mines
                if (cell.type == MinesweeperCell.Type.Mine) continue;
                cell.number = CountAdjacentsMines(x, y);
                //Only if the number > 0 we change the cell type to a number
                if (cell.number > 0) cell.type = MinesweeperCell.Type.Number;
                gameState[x, y] = cell;
            }
        }
    }

    //Function that counts the number of mines given a position in the grid
    private int CountAdjacentsMines(int cellX, int cellY)
    {
        int total = 0;
        for (int offX = -1; offX <= 1; offX++)
        {
            for (int offY = -1; offY <= 1; offY++)
            {
                //we don't have to consider our cell
                if (offX == 0 && offY == 0) continue;
                int adjacentX = offX + cellX;
                int adjacentY = offY + cellY;
                //we don't have to consider invalid position (out of boundaries)
                if (!IsValidPosition(adjacentX, adjacentY)) continue;
                if (gameState[adjacentX, adjacentY].type == MinesweeperCell.Type.Mine) total++;
            }
        }
        return total;
    }

    //Helping function that returns a cell in a given position OR return an invalid cell -> this is necessary when we click OUTSIDE the board!
    private MinesweeperCell GetCellFromPosition(int x, int y)
    {
        if (IsValidPosition(x, y)) return gameState[x, y];
        else
        {
            MinesweeperCell invalidCell = new MinesweeperCell();
            invalidCell.type = MinesweeperCell.Type.Invalid;
            return invalidCell;
        }
    }

    //Helping function that checks if x,y are inside boundaries
    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    //Helping function that returns the cell in mouse position (eventually an invalid cell is returned)
    private MinesweeperCell GetClickedCell()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        return GetCellFromPosition(cellPosition.x, cellPosition.y);
    }
    //Function that flags a Tile
    private void FlagTile()
    {
        MinesweeperCell clickedCell = GetClickedCell();
        //We can't flag an Invalid cell (outside boundaries) or a revealed cell
        if (clickedCell.type == MinesweeperCell.Type.Invalid || clickedCell.revealed) return;
        //We toggle the flagged state
        clickedCell.flagged = !clickedCell.flagged;
        gameState[clickedCell.position.x, clickedCell.position.y] = clickedCell;
        //We draw the board
        board.Draw(gameState);
    }

    //Function that reveals a Tile
    private void RevealTile()
    {
        MinesweeperCell clickedCell = GetClickedCell();
        //We can't reveal an Invalid cell (outside boundaries), an already revealed cell or a flagged cell (that prevents the revealing)
        if (clickedCell.type == MinesweeperCell.Type.Invalid || clickedCell.revealed || clickedCell.flagged) return;
        //If we reveal a Mine -> we lose
        if (clickedCell.type == MinesweeperCell.Type.Mine)
        {
            clickedCell.revealed = true;
            clickedCell.exploded = true;
            gameState[clickedCell.position.x, clickedCell.position.y] = clickedCell;
            GameLost();
        }
        //If we reveal an Empty cell -> We need a flooding algorithm that reveals all the empty/number cells in the neighborhood 
        else if (clickedCell.type == MinesweeperCell.Type.Empty) FloodRevealing(clickedCell);
        //If we click a Number cell -> We need to reveal only that and check if we won
        else
        {
            clickedCell.revealed = true;
            gameState[clickedCell.position.x, clickedCell.position.y] = clickedCell;
            //Everytime we reveal something, we need to check the win state
            if (CheckWinState()) GameWon();
        }
        //We draw the board
        board.Draw(gameState);
    }

    //Recursive function for flooding empty cells to reveal as much as possible
    private void FloodRevealing(MinesweeperCell cell)
    {
        //We can't reveal an already revealed cell, a Mine or an invalid cell (outside boundaries)
        if (cell.revealed || cell.type == MinesweeperCell.Type.Mine || cell.type == MinesweeperCell.Type.Invalid) return;
        //We still want to reveal other cell types (like numbers and empty ones)
        cell.revealed = true;
        gameState[cell.position.x, cell.position.y] = cell;
        //Everytime we reveal something, we need to check the win state
        if (CheckWinState()) GameWon();
        //Only if the current cell is empty -> we need to recursively reveal all the neighborhood cells (numbers or empty ones)
        if (cell.type == MinesweeperCell.Type.Empty)
        {
            FloodRevealing(GetCellFromPosition(cell.position.x - 1, cell.position.y));
            FloodRevealing(GetCellFromPosition(cell.position.x + 1, cell.position.y));
            FloodRevealing(GetCellFromPosition(cell.position.x, cell.position.y - 1));
            FloodRevealing(GetCellFromPosition(cell.position.x, cell.position.y + 1));
        }
    }

    //Function that checks the winning state -> if the player has revealed all the cells without the mine
    private bool CheckWinState()
    {
        // The user has revealed every tile that is not a mine!
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                MinesweeperCell cell = gameState[x, y];
                if (cell.type != MinesweeperCell.Type.Mine && !cell.revealed)
                    return false;
            }
        }
        return true;
    }

    //Function that ends the game in Lost state -> revealing all the hidden mines
    private void GameLost()
    {
        Debug.Log("Lost!");
        //we set the gameover bool
        gameOver = true;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //We reveals all the mines in the grid
                MinesweeperCell cell = gameState[x, y];
                if (cell.type == MinesweeperCell.Type.Mine)
                {
                    cell.revealed = true;
                    gameState[x, y] = cell;
                }
            }
        }
    }

    //Function that ends the game in Won state -> flagging all the mines
    private void GameWon()
    {
        Debug.Log("Won!");
        //we set the gameover bool
        gameOver = true;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //We flags all the mines in the grid
                MinesweeperCell cell = gameState[x, y];
                if (cell.type == MinesweeperCell.Type.Mine)
                {
                    cell.flagged = true;
                    gameState[x, y] = cell;
                }
            }
        }
    }
}
