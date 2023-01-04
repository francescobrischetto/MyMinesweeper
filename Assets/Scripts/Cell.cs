using UnityEngine;
using UnityEngine.UIElements;

public struct Cell
{
    //The type of cell
    public enum Type
    {
        Invalid,
        Empty,
        Mine,
        Number
    }

    public Type type;
    //Int position in grid
    public Vector3Int position;
    //Number of mines
    public int number;
    //Status of the cell
    public bool revealed;
    public bool flagged;
    public bool exploded;
}
