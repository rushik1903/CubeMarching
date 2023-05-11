using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Grid
{
    public Vector3 gridCenterPosition;
    public float CubeScaleSize = 1;
    public float scaleFactorForPerlin = 0.1f;
    public float surfaceLevel = 0;
    public int maxSurfaceLevel = 16;
    public int minSurfaceLevel = -16;
    public bool drawGizmos = false;
    public float pointToPointDist = 10;
    public bool runEveryFrame = false;
    public bool closeEdgeFaces = true;
    public bool spawnCubes = false;
    public bool drawEdges = false;

    public GameObject debugObject;
    public Vector3 gridWorldSize;

    public Vector3 gridSize;

    [NonSerialized]
    public GridPoint[,,] gridPoints;

    public Grid(Vector3 gridCenterPosition, Vector3 gridWorldSize, GameObject debugObject)
    {
        this.gridCenterPosition = gridCenterPosition;
        this.gridWorldSize = gridWorldSize;
        this.debugObject = debugObject;

        CubeScaleSize = 1;
        scaleFactorForPerlin = 10f;
        surfaceLevel = 0;
        maxSurfaceLevel = 16;
        minSurfaceLevel = -16;
        drawGizmos = true;
        pointToPointDist = 1;
        runEveryFrame = false;
        closeEdgeFaces = true;
        spawnCubes = false;
        drawEdges = false;
        //CreateGrid();
        //if (spawnCubes)
        //{
        //    DrawCubes();
        //}
    }

    public Vector3 GetBottomLeftPoint(){
        Vector3 bottomLeftStartPoint = new Vector3((int)((gridCenterPosition.x - ((gridSize.x -1) * pointToPointDist)/2)/pointToPointDist),
                                                    (int)((gridCenterPosition.y - ((gridSize.y -1)* pointToPointDist)/2)/pointToPointDist),
                                                    (int)((gridCenterPosition.z - ((gridSize.z -1)* pointToPointDist)/2)/pointToPointDist));
        bottomLeftStartPoint *= pointToPointDist;
        return bottomLeftStartPoint;
    }

    public void CreateGrid(){
        gridSize = gridWorldSize / pointToPointDist + new Vector3(1, 1, 1);
        gridSize.x = (int)gridSize.x;
        gridSize.y = (int)gridSize.y;
        gridSize.z = (int)gridSize.z;
        if (closeEdgeFaces)
        {
            gridPoints = new GridPoint[(int)gridSize.x + 2, (int)gridSize.y + 2, (int)gridSize.z + 2];
        }
        else
        {
            gridPoints = new GridPoint[(int)gridSize.x, (int)gridSize.y, (int)gridSize.z];
        }
        

        Vector3 bottomLeftStartPoint = GetBottomLeftPoint();
        for(int i = 0; i < gridSize.x; i++)
        {
            for(int j = 0;j < gridSize.y; j++)
            {
                for(int k = 0; k < gridSize.z; k++)
                {
                    int indexX = i;
                    int indexY = j;
                    int indexZ = k;
                    if (closeEdgeFaces && SeeIfPointIsInEdgeFace(i, j, k))
                    {
                        Vector3 worldPosition = bottomLeftStartPoint + new Vector3(i * pointToPointDist, j * pointToPointDist, k * pointToPointDist);

                        //to create edge faces
                        float normWt = 0;

                        int wt = (int)(normWt * (maxSurfaceLevel - minSurfaceLevel) + minSurfaceLevel);
                        gridPoints[i, j, k] = new GridPoint(wt, normWt, worldPosition, indexX, indexY, indexZ);
                    }
                    else
                    {
                        Vector3 worldPosition = bottomLeftStartPoint + new Vector3(i * pointToPointDist, j * pointToPointDist, k * pointToPointDist);

                        //float normWt = TerrainNoise(worldPosition, bottomLeftStartPoint.y, bottomLeftStartPoint.y+gridSize.y*pointToPointDist);
                        float normWt = Perlin3D(worldPosition);

                        int wt = (int)(normWt * (maxSurfaceLevel - minSurfaceLevel) + minSurfaceLevel);
                        gridPoints[i, j, k] = new GridPoint(wt, normWt, worldPosition, indexX, indexY, indexZ);
                    }
                    
                }
            }
        }
        Debug.Log("grid points size");
        Debug.Log(gridSize);
    }

    float TerrainNoise(Vector3 worldPosition, float bottom, float peak)
    {


        return 1-(worldPosition.y-bottom)/(peak-bottom);
    }
    bool SeeIfPointIsInEdgeFace(int i, int j, int k)
    {
        if (i == 0 || j == 0 || k == 0 || i == gridSize.x - 1 || j == gridSize.y - 1 || k == gridSize.z - 1)
        {
            return true;
        }
        return false;
    }

    public float Perlin3D(Vector3 worldPosition){
        worldPosition*= scaleFactorForPerlin;

        float ab = Mathf.PerlinNoise(worldPosition.x, worldPosition.y);
        float bc = Mathf.PerlinNoise(worldPosition.y, worldPosition.z);
        float ca = Mathf.PerlinNoise(worldPosition.x, worldPosition.z); 

        float ba = Mathf.PerlinNoise(worldPosition.y, worldPosition.x);
        float cb = Mathf.PerlinNoise(worldPosition.z, worldPosition.y);
        float ac = Mathf.PerlinNoise(worldPosition.x, worldPosition.z);

        float abc = ab+bc+ca+ba+cb+ac;
        worldPosition /= scaleFactorForPerlin;
        return abc / 6f;
    }

    

    public void DrawCubes(){
        if (!spawnCubes)
        {
            return;
        }
        if(gridPoints ==null){
            return;
        }
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                for (int k = 0; k < gridSize.z; k++)
                {
                    if(gridPoints[i,j,k].wt > surfaceLevel)
                    {
                        GameObject temp = GameObject.Instantiate(debugObject, gridPoints[i, j, k].worldPosition,  Quaternion.identity);
                        temp.transform.localScale *= (pointToPointDist*CubeScaleSize);
                        var tempGridPoint = temp.GetComponent<var>();
                        tempGridPoint.normWt = gridPoints[i,j,k].normWt;
                    }
                }
            }
        }
    }
}
