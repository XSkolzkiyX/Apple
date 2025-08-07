using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    [SerializeField] private GameObject applePrefab;
    private Transform apples;
    [SerializeField] private float range;
    [SerializeField] private float height;
    [SerializeField] private float spawnDelay;
    [SerializeField] private float appleLifetime;
    [SerializeField] private int appleLimit;

    private void Start()
    {
        apples = GameObject.Find("Apples").transform;
        StartCoroutine(GenerateApple());
    }

    private IEnumerator GenerateApple()
    {
        yield return new WaitForSeconds(spawnDelay);
        if (apples.childCount < appleLimit)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-range, range), height, Random.Range(-range, range)) + transform.position;
            GameObject curApple = Instantiate(applePrefab, spawnPosition, Random.rotation, apples);
            Destroy(curApple, appleLifetime);
        }
        StartCoroutine(GenerateApple());
    }
}
