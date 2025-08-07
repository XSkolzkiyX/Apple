using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorWayController : MonoBehaviour
{
    public List<string> activationTags = new List<string> { "Door" };

    public void ActivateDoor()
    {
        gameObject.SetActive(false);
    }

    public void DeactivateDoor()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider col)
    {
        if(activationTags.Contains(col.tag))
        {
            ActivateDoor();
        }
    }
}
