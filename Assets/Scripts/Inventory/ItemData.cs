using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item Data")]
public class ItemData : ScriptableObject
{
    public GameObject model;
    public GameObject prefab;
    public Sprite icon;
    public int price;
    public float durability = -1;
}
