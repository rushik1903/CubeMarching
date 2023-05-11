using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTerain : MonoBehaviour
{
    public float maxRadius = 50;
    public Vector3 gridWorldSize;
    List<CubeMarching> cubeMarchingList;
    public GameObject debugObject;

    private void Start()
    {
        ReNewList();
    }

    void ReNewList()
    {
        cubeMarchingList = new List<CubeMarching>();

        CubeMarching cube = new CubeMarching(gameObject.GetComponent<MeshFilter>(), gameObject.transform.position, gridWorldSize, debugObject);
        cubeMarchingList.Add(cube);
    }
}
