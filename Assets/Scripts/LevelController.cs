using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private Transform trees;
    [SerializeField] private Terrain terrain;
    [SerializeField] private float range;
    [SerializeField] private float spawnDelay;
    [SerializeField] private float treeLifetime;
    [SerializeField] private int treeLimit;
    [SerializeField] private Vector3 offset;

    private void Start()
    {
        InvokeRepeating(nameof(GenerateTree), 0, spawnDelay);
    }

    private void GenerateTree()
    {
        if (trees.childCount >= treeLimit) return;
        Vector3 spawnPosition = new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range)) + transform.position;
        spawnPosition.y = terrain.SampleHeight(spawnPosition);
        GameObject curTree = Instantiate(treePrefab, spawnPosition + offset, Quaternion.identity, trees);
        curTree.transform.Rotate(0, Random.Range(0, 360), 0);
        Destroy(curTree, treeLifetime);
    }
}
