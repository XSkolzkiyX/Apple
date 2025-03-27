using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscriberController2 : MonoBehaviour
{
    private PublisherController2 publisher;
    private void Start()
    {
        publisher = FindFirstObjectByType<PublisherController2>();
        publisher.myEvent.AddListener(HiddenFunction);
    }

    public void ShowTextMessage(string text)
    {
        Debug.Log(text);
    }

    public void ShowTextAndBoolMessage(string text, bool a)
    {
        Debug.Log(text);
        Debug.Log(a);
    }

    private void HiddenFunction(string a, bool b)
    {
        Debug.Log($"HiddenFunction: {a}, {b}");
    }

    public void Function(GameObject a, Transform b)
    {
        Debug.Log(a.name);
        Debug.Log(b.rotation);
    }

    private void OnDestroy()
    {
        publisher.myEvent.RemoveListener(HiddenFunction);
    }
}
