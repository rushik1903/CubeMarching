using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class SpawnTerrainOnMove : MonoBehaviour
{
    public Material material;
    public float maxRadius = 100;
    private Vector3 gridWorldSize;
    public float _gridWorldSideLength = 50f;
    public float gridWorldSideLength
    {
        get
        {
            return _gridWorldSideLength;
        }
        set
        {
            _gridWorldSideLength = value;
            gridWorldSize = new Vector3(_gridWorldSideLength, _gridWorldSideLength, _gridWorldSideLength);
        }
    }
    public GameObject debugObject;
    public float pointToPointDist = 1;



    public float CubeScaleSize = 1;

    [Header("Noise Attributes")]
    public float scaleFactorForPerlin = 0.1f;
    [Range(-16, 16)]
    public float surfaceLevel = 0;
    public int maxSurfaceLevel = 16;
    public int minSurfaceLevel = -16;

    [Header("Runtime Variables")]
    public bool drawGizmos = false;
    public bool runEveryFrame = false;
    public bool closeEdgeFaces = true;
    public bool spawnCubes = false;
    public bool drawEdges = false;

    Vector3 gridCenterPosition;
    Vector3 playerCenterPosition;
    List<Vector3> subTerrainPositions_Rendered;
    List<Vector3> subTerrainPositions_renderNextFrame;
    List<Vector3> subTerrainPositions_unrenderNextFrame;
    List<CubeMarchingEssentials> cubeMarchingEssentialsList;

    struct CubeMarchingEssentials
    {
        public CubeMarching cubeMarching;
        public GameObject gameObject;
        public MeshFilter meshFilter;
    }

    private void Start()
    {
        gridWorldSideLength = _gridWorldSideLength;
        gridCenterPosition = transform.position;
        playerCenterPosition = transform.position;
        FindPositionsForSubTerrains_AddingAllCurrent_FinalShapeSphere();
    }

    private void FindPositionsForSubTerrains_AddingAllCurrent_FinalShapeSphere()
    {
        subTerrainPositions_Rendered = new List<Vector3>();
        float widthOfSubTerrain = gridWorldSideLength;
        float spaceMaxRadius = Mathf.Floor(maxRadius / widthOfSubTerrain) * widthOfSubTerrain;
        Vector3 nonMovingCenterOfWorld = FindNearestSpacePoint(gridCenterPosition);
        Vector3 bottomLeftOfMoving = nonMovingCenterOfWorld - new Vector3(spaceMaxRadius, spaceMaxRadius, spaceMaxRadius) + new Vector3(widthOfSubTerrain/2, widthOfSubTerrain / 2, widthOfSubTerrain / 2);

        int no_of_iterations = (int)((2 * spaceMaxRadius) / widthOfSubTerrain);

        for (int i = 0; i < no_of_iterations; i++)
        {
            for (int j = 0; j < no_of_iterations; j++)
            {
                for (int k = 0; k < no_of_iterations; k++)
                {
                    Vector3 pos = bottomLeftOfMoving + new Vector3(i * widthOfSubTerrain, j * widthOfSubTerrain, k * widthOfSubTerrain);

                    //below if statement is the only thing that changes to change the shape of final big form
                    //currently its circle, removing if and adding all will make it a cube.
                    //if(Vector3.Distance(pos,nonMovingCenterOfWorld) <= spaceMaxRadius)
                    //{
                        subTerrainPositions_Rendered.Add(pos);
                    //}
                }
            }
        }
        Debug.Log("no_of_cubes : " + subTerrainPositions_Rendered.Count.ToString());
        //foreach(Vector3 x in subTerrainPositions_Rendered)
        //{
        //    Debug.Log(x);
        //}
        RenderAtEachPointInCurrentlyRendered();
    }

    private void RenderAtEachPointInCurrentlyRendered()
    {
        cubeMarchingEssentialsList = new List<CubeMarchingEssentials>();
        foreach (Vector3 center in subTerrainPositions_Rendered)
        {
            GameObject gameObject = new GameObject();
            if (gameObject.GetComponent<MeshRenderer>() == null)
            {
                MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
                renderer.material= material;
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material = material;
            }
            //no need to instantiate, declaring it instantiates automatically
            //GameObject.Instantiate(gameObject);
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            CubeMarching cubeMarching = new CubeMarching(meshFilter, center, gridWorldSize, debugObject);
            SetVariablesForCubeMarching(cubeMarching);
            CubeMarchingEssentials item;
            item.cubeMarching = cubeMarching;
            item.meshFilter = meshFilter;
            item.gameObject = gameObject;
            cubeMarchingEssentialsList.Add(item);
        }
        Debug.Log("no_of_cubeMarchingEssentialsList : " + cubeMarchingEssentialsList.Count.ToString());
    }

    private void SetVariablesForCubeMarching(CubeMarching cubeMarching)
    {
        cubeMarching.CubeScaleSize = CubeScaleSize;
        cubeMarching.scaleFactorForPerlin = scaleFactorForPerlin;
        cubeMarching.surfaceLevel = surfaceLevel;
        cubeMarching.maxSurfaceLevel = maxSurfaceLevel;
        cubeMarching.minSurfaceLevel = minSurfaceLevel;
        cubeMarching.drawGizmos = drawGizmos;
        cubeMarching.pointToPointDist = pointToPointDist;
        cubeMarching.runEveryFrame = runEveryFrame;
        cubeMarching.closeEdgeFaces = closeEdgeFaces;
        cubeMarching.spawnCubes = spawnCubes;
        cubeMarching.drawEdges = drawEdges;
    }

    private Vector3 FindNearestSpacePoint(Vector3 point)
    {
        float widthOfSubTerrain = gridWorldSideLength;
        Vector3 spacePoint = point / widthOfSubTerrain;
        spacePoint.x = Mathf.Floor(spacePoint.x);
        spacePoint.y = Mathf.Floor(spacePoint.y);
        spacePoint.z = Mathf.Floor(spacePoint.z);
        spacePoint *= widthOfSubTerrain;
        return spacePoint;
    }

    void Update()
    {
        gridCenterPosition = transform.position;
        if (Input.GetKeyDown(KeyCode.G) || runEveryFrame)
        {
            Debug.Log("reseting");
            GameObject[] respawns;
            respawns = GameObject.FindGameObjectsWithTag("cube");

            foreach (GameObject respawn in respawns)
            {
                GameObject.Destroy(respawn);
            }
            if(cubeMarchingEssentialsList!=null && cubeMarchingEssentialsList.Count > 0)
            {
                foreach(CubeMarchingEssentials item in cubeMarchingEssentialsList)
                {
                    Debug.Log("here");
                    item.cubeMarching.CreateGrid();
                    if (spawnCubes)
                    {
                        item.cubeMarching.DrawCubes();
                    }
                    else
                    {
                        item.cubeMarching.CreateCubes();
                        item.cubeMarching.MarchMesh();
                    }
                }
            }
        }
    }

    //void ReNewList()
    //{
    //    item.cubeMarching = new List<CubeMarching>();

    //    CubeMarching cubeMarching = new CubeMarching(gameObject.GetComponent<MeshFilter>(), transform.position, gridWorldSize, debugObject);
    //    item.cubeMarching.Add(cubeMarching);
    //    SetVariables();
    //}

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(maxRadius * 2, maxRadius * 2, maxRadius * 2));
    }

    void OnDrawGizmos()
    {
        if (cubeMarchingEssentialsList != null && cubeMarchingEssentialsList.Count >= 1)
        {

            foreach(CubeMarchingEssentials item in cubeMarchingEssentialsList)
            {
                if (item.cubeMarching.gridPoints != null)
                {
                    for (int i = 0; i < item.cubeMarching.gridSize.x; i++)
                    {   
                        for (int j = 0; j < item.cubeMarching.gridSize.y; j++)
                        {
                            for (int k = 0; k < item.cubeMarching.gridSize.z; k++)
                            {
                                if (item.cubeMarching.drawGizmos)
                                {
                                    Gizmos.color = Color.Lerp(Color.black, Color.white, item.cubeMarching.gridPoints[i, j, k].normWt);
                                    if (item.cubeMarching.gridPoints[i, j, k].wt >= item.cubeMarching.surfaceLevel)
                                    {
                                        Gizmos.color = Color.white;
                                    }
                                    else
                                    {
                                        Gizmos.color = Color.black;
                                    }
                                    Gizmos.DrawCube(item.cubeMarching.gridPoints[i, j, k].worldPosition, new Vector3(1f, 1f, 1f));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
