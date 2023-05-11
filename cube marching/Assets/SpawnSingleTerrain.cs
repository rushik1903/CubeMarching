using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSingleTerrain : MonoBehaviour
{
    public float maxRadius = 50;
    public Vector3 gridWorldSize;
    List<CubeMarching> cubeMarchingList;
    public GameObject debugObject;



    public float CubeScaleSize = 1;
    public float scaleFactorForPerlin = 0.1f;
    public float surfaceLevel = 0;
    public int maxSurfaceLevel = 16;
    public int minSurfaceLevel = -16;
    public bool drawGizmos = false;
    public float pointToPointDist = 1;
    public bool runEveryFrame = false;
    public bool closeEdgeFaces = true;
    public bool spawnCubes = false;
    public bool drawEdges = false;

    Vector3 gridCenterPosition;

    private void Start()
    {
        gridCenterPosition = transform.position;
        ReNewList();
    }

    private void SetVariables()
    {
        if (cubeMarchingList != null && cubeMarchingList.Count > 0)
        {
            cubeMarchingList[0].gridCenterPosition = transform.position;
            cubeMarchingList[0].CubeScaleSize= CubeScaleSize;
            cubeMarchingList[0].scaleFactorForPerlin= scaleFactorForPerlin;
            cubeMarchingList[0].surfaceLevel= surfaceLevel;
            cubeMarchingList[0].maxSurfaceLevel= maxSurfaceLevel;
            cubeMarchingList[0].minSurfaceLevel= minSurfaceLevel;
            cubeMarchingList[0].drawGizmos = drawGizmos;
            cubeMarchingList[0].pointToPointDist = pointToPointDist;
            cubeMarchingList[0].runEveryFrame= runEveryFrame;
            cubeMarchingList[0].closeEdgeFaces= closeEdgeFaces;
            cubeMarchingList[0].spawnCubes= spawnCubes;
            cubeMarchingList[0].drawEdges= drawEdges;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) || runEveryFrame)
        {
            SetVariables();
            Debug.Log("reseting");
            GameObject[] respawns;
            respawns = GameObject.FindGameObjectsWithTag("cube");

            foreach (GameObject respawn in respawns)
            {
                GameObject.Destroy(respawn);
            }
            if(cubeMarchingList!= null && cubeMarchingList.Count>0)
            {
                cubeMarchingList[0].CreateGrid();
                if (spawnCubes)
                {
                    cubeMarchingList[0].DrawCubes();
                }
                else
                {
                    cubeMarchingList[0].CreateCubes();
                    cubeMarchingList[0].MarchMesh();
                }
            }
            
        }
    }

    void ReNewList()
    {
        cubeMarchingList = new List<CubeMarching>();

        CubeMarching cubeMarching = new CubeMarching(gameObject.GetComponent<MeshFilter>(), transform.position, gridWorldSize, debugObject);
        cubeMarchingList.Add(cubeMarching);
        SetVariables();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, gridWorldSize.z));
    }

    void OnDrawGizmos()
    {
        if (cubeMarchingList!=null && cubeMarchingList.Count >= 1)
        {
            if (cubeMarchingList[0].gridPoints != null)
            {
                for (int i = 0; i < cubeMarchingList[0].gridSize.x; i++)
                {
                    for (int j = 0; j < cubeMarchingList[0].gridSize.y; j++)
                    {
                        for (int k = 0; k < cubeMarchingList[0].gridSize.z; k++)
                        {
                            if (cubeMarchingList[0].drawGizmos)
                            {
                                Gizmos.color = Color.Lerp(Color.black, Color.white, cubeMarchingList[0].gridPoints[i, j, k].normWt);
                                if (cubeMarchingList[0].gridPoints[i, j, k].wt >= cubeMarchingList[0].surfaceLevel)
                                {
                                    Gizmos.color = Color.white;
                                }
                                else
                                {
                                    Gizmos.color = Color.black;
                                }
                                Gizmos.DrawCube(cubeMarchingList[0].gridPoints[i, j, k].worldPosition, new Vector3(1f, 1f, 1f));
                            }
                        }
                    }
                }
            }
        }
    }
}
