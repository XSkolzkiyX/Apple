using UnityEngine;

public enum CollectableItemType
{
    None,
    HealPotion,
    RegenerationPotion,
    SpeedPotion,
    StrengthPotion
}

public class CollectableItem : MonoBehaviour
{
    public CollectableItemType type;
}
