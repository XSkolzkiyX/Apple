using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Outline))]
public class ItemController : MonoBehaviour
{
    public ItemData itemData;
    [HideInInspector] public Outline outline;
    [HideInInspector] public Rigidbody rigidbody;

    private void Start()
    {
        outline = GetComponent<Outline>();
        rigidbody = GetComponent<Rigidbody>();
    }
}
