using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketController : Item
{
    public GameObject waterObject;
    public static bool isActive = false;

    private void Start()
    {
        if(isActive) waterObject.SetActive(true);
    }

    public override void DropItem()
    {
        isActive = false;
        waterObject.SetActive(false);
        base.DropItem();
    }

    public override void UseItem()
    {
        isActive = true;
        waterObject.SetActive(true);
        base.UseItem();
    }
}
