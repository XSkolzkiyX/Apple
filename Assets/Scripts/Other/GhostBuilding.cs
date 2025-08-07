using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBuilding : MonoBehaviour
{
    public List<string> allowCollisionTags;
    public Vector3 buildingSize;
    public Vector3 offset;

    public bool CheckCollision()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position + offset, buildingSize / 2, transform.rotation);
        foreach (Collider collider in colliders)
        {
            if (!collider.isTrigger && !allowCollisionTags.Contains(collider.tag)) return false;
        }
        return true;
    }
}

