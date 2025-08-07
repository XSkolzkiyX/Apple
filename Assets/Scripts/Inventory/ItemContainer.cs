using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField] private string[] triggerNames;
    [SerializeField] private KeyCode targetKey;
    [SerializeField] private Animator animator;

    private void Update()
    {
        if(Input.GetKeyDown(targetKey))
        {
            UseItem();
        }
    }

    public virtual void UseItem()
    {
        if (animator && triggerNames.Length > 0) animator.SetTrigger(triggerNames[Random.Range(0, triggerNames.Length)]);
        Debug.Log($"Item {gameObject.name} has been used...");
    }
}
