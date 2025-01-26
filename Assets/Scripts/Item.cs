using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;
    
    public virtual void UseItem()
    {
        Debug.Log($"Item {itemData} used");
    }

    public virtual void DropItem()
    {
        Debug.Log($"Item {itemData} droped");
        Destroy(gameObject);
    }
}
