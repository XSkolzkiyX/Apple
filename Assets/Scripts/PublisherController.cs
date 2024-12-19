using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MyEvent : UnityEvent<string, bool, float> { }

public class PublisherController : MonoBehaviour
{
    public MyEvent myEvent;
    public string text;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            myEvent.Invoke(text, true, Random.Range(0, 100));
        }
    }
}
