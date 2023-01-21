using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MinesweeperBoard : MonoBehaviour
{
    //Only this class will be able to change the data of the Unity tilemap used to draw the grid
    public Tilemap tilemap { get; private set; }
    //An array with all the possible tiles of Minesweeper (also with numbers)
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

    //Method to draw the entire board
    public void Draw(MinesweeperCell[,] gameState)
    {
        int width = gameState.GetLength(0);
        int height = gameState.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                MinesweeperCell cell = gameState[x, y];
                //Set the cell to the currect tile (REQUIRES Vector3Int Position even if we are working on 2D)
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }

    }

    //Returns the correct Tile based on the tile status -> flagged or unknown to the player are the "easy" ones
    private Tile GetTile(MinesweeperCell cell)
    {
        //Revealed
        if (cell.revealed)
        {
            switch (cell.type)
            {
                case MinesweeperCell.Type.Empty:  return Tiles[(int)TileType.Empty];
                //If the cell is a Mine, can be exploded by the player that clicked on that
                case MinesweeperCell.Type.Mine:   return cell.exploded ? Tiles[(int)TileType.Exploded] : Tiles[(int)TileType.Mine];
                case MinesweeperCell.Type.Number: return GetNumber(cell);
                default:                          return null;
            }
        }
        //Flagged by the player
        else if (cell.flagged)
        {
            return Tiles[(int)TileType.Flag];
        }
        //Not already discovered by the player
        else
        {
            return Tiles[(int)TileType.Unknown];
        }
    }

    //Returns the Tile when the player reveales a tile that contains a number
    private Tile GetNumber(MinesweeperCell cell)
    {
        switch (cell.number)
        {
            case 1:   return Tiles[(int)TileType.Num1];
            case 2:   return Tiles[(int)TileType.Num2];
            case 3:   return Tiles[(int)TileType.Num3];
            case 4:   return Tiles[(int)TileType.Num4];
            case 5:   return Tiles[(int)TileType.Num5];
            case 6:   return Tiles[(int)TileType.Num6];
            case 7:   return Tiles[(int)TileType.Num7];
            case 8:   return Tiles[(int)TileType.Num8];
            default:  return null;
        }
    }

}
