using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Data/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public float health;
    
    public float walkingSpeed;
    public float runningSpeed;
    public float rotationSpeed;

    public float attackDistance;
    public float meleeDistance;
}
