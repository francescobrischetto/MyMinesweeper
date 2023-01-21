using UnityEngine;
using UnityEngine.UIElements;

public struct MinesweeperCell
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
    //Number of mines in the surroundings
    public int number;
    //Status of the cell
    //Revealed to the player
    public bool revealed;
    //Flagged by the player
    public bool flagged;
    //Exploded (the lose condition for the player)
    public bool exploded;
}
