using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

public class CubeMarching : Grid
{
    public MeshFilter meshFilter;
    CubeMarcher[,,] cubes;
    Vector3 cubeGridSize;

    HashSet<HashElement> alreadyAdded;

    List<Vector3> vertices;
    List<int> faces;
    Mesh mesh;

    public CubeMarching(MeshFilter meshFilter, Vector3 gridCenterPosition, Vector3 gridWorldSize, GameObject debugObject) 
        : base(gridCenterPosition, gridWorldSize, debugObject)
    {
        this.meshFilter = meshFilter;
        //CreateCubes();
        //MarchMesh();
    }

    public class HashElement
    {
        public Vector3 worldPosition;
        public int index;

        public HashElement(Vector3 p, int i)
        {
            worldPosition = p;
            index = i;
        }
    }
    public void CreateCubes()
    {
        cubeGridSize = new Vector3((int)(gridSize.x - 1), (int)(gridSize.y - 1), (int)(gridSize.z - 1));
        cubes = new CubeMarcher[(int)cubeGridSize.x, (int)cubeGridSize.y, (int)cubeGridSize.z];
        for (int i = 0; i < cubeGridSize.x; i++)
        {
            for (int j = 0; j < cubeGridSize.y; j++)
            {
                for (int k = 0; k < cubeGridSize.z; k++)
                {
                    cubes[i, j, k] = new CubeMarcher(i, j, k, null);
                    cubes[i, j, k].gridPointsOfCube[0] = gridPoints[i, j, k];
                    cubes[i, j, k].gridPointsOfCube[1] = gridPoints[i, j, k+1];
                    cubes[i, j, k].gridPointsOfCube[2] = gridPoints[i, j+1, k];
                    cubes[i, j, k].gridPointsOfCube[3] = gridPoints[i, j+1, k+1];
                    cubes[i, j, k].gridPointsOfCube[4] = gridPoints[i+1, j, k];
                    cubes[i, j, k].gridPointsOfCube[5] = gridPoints[i+1, j, k+1];
                    cubes[i, j, k].gridPointsOfCube[6] = gridPoints[i+1, j+1, k];
                    cubes[i, j, k].gridPointsOfCube[7] = gridPoints[i+1, j+1, k+1];
                }
            }
        }
        Debug.Log("cubes grid size :");
        Debug.Log(cubeGridSize);
    }

    public void MarchMesh(){
        alreadyAdded = new HashSet<HashElement>();
        vertices = new List<Vector3>();
        faces = new List<int>();

        for (int i = 0;i < cubeGridSize.x; i++)
        {
            for(int j = 0; j < cubeGridSize.y; j++)
            {
                for(int k = 0; k < cubeGridSize.z; k++)
                {
                    byte indexInTable = FindIndexForTable(i,j,k);
                    List<int> facesOfCube = Table.edgeIndexToFacesTable[(int)indexInTable];
                    for(int f = 0; f < facesOfCube.Count-1; f+= 3)
                    {
                        int e1 = facesOfCube[f];
                        int e2 = facesOfCube[f + 1];
                        int e3 = facesOfCube[f + 2];

                        int e10 = Table.edgeToCorners[e1][0];
                        int e11 = Table.edgeToCorners[e1][1];

                        Vector3 midPoint1 = (cubes[i, j, k].gridPointsOfCube[e10].worldPosition + cubes[i, j, k].gridPointsOfCube[e11].worldPosition) / 2;
                        midPoint1 -= gridCenterPosition;

                        int e20 = Table.edgeToCorners[e2][0];
                        int e21 = Table.edgeToCorners[e2][1];

                        Vector3 midPoint2 = (cubes[i, j, k].gridPointsOfCube[e20].worldPosition + cubes[i, j, k].gridPointsOfCube[e21].worldPosition) / 2;
                        midPoint2 -= gridCenterPosition;


                        int e30 = Table.edgeToCorners[e3][0];
                        int e31 = Table.edgeToCorners[e3][1];

                        Vector3 midPoint3 = (cubes[i, j, k].gridPointsOfCube[e30].worldPosition + cubes[i, j, k].gridPointsOfCube[e31].worldPosition) / 2;
                        midPoint3 -= gridCenterPosition;

                        int index1 = GiveIndexOfVertex(midPoint1);
                        int index2 = GiveIndexOfVertex(midPoint2);
                        int index3 = GiveIndexOfVertex(midPoint3);

                        if(index1 == -1)
                        {
                            vertices.Add(midPoint1);
                            alreadyAdded.Add(new HashElement(midPoint1, vertices.Count - 1));
                            index1 = vertices.Count - 1;
                        }
                        if (index2 == -1)
                        {
                            vertices.Add(midPoint2);
                            alreadyAdded.Add(new HashElement(midPoint2, vertices.Count - 1));
                            index2 = vertices.Count - 1;
                        }
                        if (index3 == -1)
                        {
                            vertices.Add(midPoint3);
                            alreadyAdded.Add(new HashElement(midPoint3, vertices.Count - 1));
                            index3 = vertices.Count - 1;
                        }

                        faces.Add(index3);
                        faces.Add(index2);
                        faces.Add(index1);
                    }
                }
            }
        }
        Debug.Log("vertices:");
        Debug.Log("Faces:");
        Debug.Log(vertices.Count);
        Debug.Log(faces.Count);
        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        //mesh.uv = newUV;
        mesh.triangles = faces.ToArray();
        meshFilter.mesh = mesh;
        mesh.RecalculateNormals();
    }

    int GiveIndexOfVertex(Vector3 worldPosition)
    {

        HashElement result = alreadyAdded.FirstOrDefault(f => f.worldPosition == worldPosition);

        if (result != null)
        {
            return result.index;
        }
        return -1;
    }

    byte FindIndexForTable(int i, int j, int k)
    {
        byte indexInTable = 0;
        if (cubes[i, j, k].gridPointsOfCube[0].wt >= surfaceLevel)
        {
            indexInTable |= (byte)(1 << 0);
        }
        if (cubes[i, j, k].gridPointsOfCube[1].wt >= surfaceLevel)
        {
            indexInTable |= (byte)(1 << 1);
        }
        if (cubes[i, j, k].gridPointsOfCube[2].wt >= surfaceLevel)
        {
            indexInTable |= (byte)(1 << 2);
        }
        if (cubes[i, j, k].gridPointsOfCube[3].wt >= surfaceLevel)
        {
            indexInTable |= (byte)(1 << 3);
        }
        if (cubes[i, j, k].gridPointsOfCube[4].wt >= surfaceLevel)
        {
            indexInTable |= (byte)(1 << 4);
        }
        if (cubes[i, j, k].gridPointsOfCube[5].wt >= surfaceLevel)
        {
            indexInTable |= (byte)(1 << 5);
        }
        if (cubes[i, j, k].gridPointsOfCube[6].wt >= surfaceLevel)
        {
            indexInTable |= (byte)(1 << 6);
        }
        if (cubes[i, j, k].gridPointsOfCube[7].wt >= surfaceLevel)
        {
            indexInTable |= (byte)(1 << 7);
        }
        return indexInTable;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        if(faces == null || spawnCubes)
        {
            return;
        }
        if (!drawEdges)
        {
            return;
        }
        for(int i=0;i<faces.Count;i+=3)
        {
            Vector3 i1 = vertices[faces[i]];
            Vector3 i2 = vertices[faces[i + 1]];
            Vector3 i3 = vertices[faces[i + 2]];

            Vector3 newi1 = i1 + gridCenterPosition;
            Vector3 newi2 = i2 + gridCenterPosition;
            Vector3 newi3 = i3 + gridCenterPosition;

            Gizmos.DrawLine(newi1, newi2);
            Gizmos.DrawLine(newi2, newi3);
            Gizmos.DrawLine(newi3, newi1);
        }
    }
}
