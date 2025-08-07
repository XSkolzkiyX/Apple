using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Fish Data", menuName = "Data/Fish Data")]
public class FishData : ScriptableObject
{
    [Header("General")]
    public float health = 10;
    public float maxSpeed = 3;
    public float acceleration = 20;
    public float rotationSpeed = 2;
    public float attackDistance = 2;
    [Space(10)]

    [Header("WayPoints")]
    public float wayPointsRange = 25;
    public float waypointSelectionDelay = 4;

    public List<GameObject> drop;
}
