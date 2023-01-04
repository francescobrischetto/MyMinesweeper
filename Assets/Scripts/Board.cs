using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    //Only this class will be able to change the data of the tilemap, that are visible publicly
    public Tilemap tilemap { get; private set; }
    public Tile[] Tiles;
    public enum TileType
    {
        Unknown = 0,
        Empty = 1,
        Mine = 2,
        Exploded = 3,
        Num1 = 4,
        Num2 = 5,
        Num3 = 6,
        Num4 = 7,
        Num5 = 8,
        Num6 = 9,
        Num7 = 10,
        Num8 = 11,
        Flag = 12,
    }
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    //Method to draw the board
    public void Draw(Cell[,] state)
    {
        int width = state.GetLength(0);
        int height = state.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                //Set the cell to the currect tile (based on minesweeper rules)
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }

    }

    //Returns the Tile based on minesweeper logic -> flagged or unknown to the player are the "easy" ones
    private Tile GetTile(Cell cell)
    {
        if (cell.revealed)
        {
            return GetRevealedTile(cell);
        }
        else if (cell.flagged)
        {
            return Tiles[(int)TileType.Flag];
        }
        else
        {
            return Tiles[(int)TileType.Unknown];
        }
    }

    //Returns the Tile based on minesweeper logic when the player reveales a tile
    private Tile GetRevealedTile(Cell cell)
    {
        switch (cell.type)
        {
            case Cell.Type.Empty: return Tiles[(int)TileType.Empty];
            case Cell.Type.Mine: return Tiles[(int)TileType.Mine];
            case Cell.Type.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    //Returns the Tile based on minesweeper logic when the player reveales a tile (only to number tiles) - separated for clear visibility
    private Tile GetNumberTile(Cell cell)
    {
        switch (cell.number)
        {
            case 1: return Tiles[(int)TileType.Num1];
            case 2: return Tiles[(int)TileType.Num2];
            case 3: return Tiles[(int)TileType.Num3];
            case 4: return Tiles[(int)TileType.Num4];
            case 5: return Tiles[(int)TileType.Num5];
            case 6: return Tiles[(int)TileType.Num6];
            case 7: return Tiles[(int)TileType.Num7];
            case 8: return Tiles[(int)TileType.Num8];
            default: return null;
        }
    }

}
