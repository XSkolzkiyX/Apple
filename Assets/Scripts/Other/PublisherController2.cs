using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class MyCustomEvent : UnityEvent<GameObject, Transform>
{
    public void DemoFunction()
    {
        Debug.Log("Custom");
    }
}

public class PublisherController2 : MonoBehaviour
{
    public UnityEvent<string, bool> myEvent;
    public MyCustomEvent myCustomEvent;
    public string text;
    public bool value;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            myEvent.Invoke(text, value);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            myCustomEvent.DemoFunction();
            myCustomEvent.Invoke(gameObject, transform);
        }
    }
}
