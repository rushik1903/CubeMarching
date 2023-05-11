using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPoint  
{
    public int wt = 0;
    public float normWt = 0;
    public Vector3 worldPosition;
    public int indexX = 0;
    public int indexY = 0;
    public int indexZ = 0;

    public GridPoint(int _wt, float _normWt, Vector3 _worldPosition, int _indexX, int _indexY, int _indexZ)
    {
        wt = _wt;
        normWt = _normWt;
        worldPosition = _worldPosition;
        indexX = _indexX;
        indexY = _indexY;
        indexZ = _indexZ;
    }
}
