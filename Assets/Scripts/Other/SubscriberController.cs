using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscriberController : MonoBehaviour
{
    private PublisherController publisher;

    private void Start()
    {
        publisher = FindFirstObjectByType<PublisherController>();
        publisher.myEvent.AddListener(ShowText);
    }

    private void ShowText(string text, bool a, float b)
    {
        Debug.Log($"{text}, {a}, {b}");
    }

    private void OnDestroy()
    {
        publisher.myEvent.RemoveListener(ShowText);
    }
}
