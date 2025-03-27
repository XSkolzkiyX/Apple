using System.Collections.Generic;
using UnityEngine;
using System;

public enum PropType
{
    BigBox,
    SmallBox,
    Barrel,
    Urn
}

public enum ItemType
{
    BigHealthRegenPotion,
    HealthRegenPotion,
    SpeedPotion,
    ShieldPotion,
}

[Serializable]
public class LootTable
{
    public List<ItemType> itemTypes;
    public List<float> percentage;
}

[CreateAssetMenu(fileName = "New Prop", menuName = "Data/Prop Data")]
public class PropData : ScriptableObject
{
    [Range(0, 1000)] public float health;
    public bool isSwordBreakable;

    public PropType propType;
    public GameObject breakEffect;

    public LootTable lootTable;
}