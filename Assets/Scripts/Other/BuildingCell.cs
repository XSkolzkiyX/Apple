using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Front,
    Right,
    Back,
    Left
}

public class BuildingCell : MonoBehaviour
{
    public Vector2Int footprint = new Vector2Int(1, 1);
    public List<GameObject> doors;

    public void OpenDoor(Direction direction)
    {
        if((int)direction < doors.Count)
        {
            if (!doors[(int)direction]) return;
            doors[(int)direction].SetActive(false);
        }
    }
}
