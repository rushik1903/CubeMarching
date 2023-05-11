using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMarcher
{
    public int indexX;
    public int indexY;
    public int indexZ;

    public GridPoint[] gridPointsOfCube;

    public CubeMarcher(int indexX, int indexY, int indexZ, GridPoint[] gridPointsOfCube)
    {
        this.indexX = indexX;
        this.indexY = indexY;
        this.indexZ = indexZ;
        this.gridPointsOfCube = gridPointsOfCube;
        if(this.gridPointsOfCube == null)
        {
            this.gridPointsOfCube = new GridPoint[8];
        }
    }
}
