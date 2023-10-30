using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private GameObject prefabBaseProjectile;

    [SerializeField] private List<GameObject> projectiles = new List<GameObject>();
    [SerializeField] private Material material;
    private int idProjectile = 0;

    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            // GameObject gm = Instantiate(prefabBaseProjectile);
            GenerateMesh();
        }
    }
    public GameObject GetProjectile()
    {
        GameObject projectile = projectiles[idProjectile];
        idProjectile++;
        if (idProjectile >= projectiles.Count)
        {
            idProjectile = 0;
        }

        return projectile;
    }

    private void GenerateMesh()
    {
        GameObject cube = Instantiate(prefabBaseProjectile);
        MeshFilter meshFilter = cube.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-1, -1, -1),
            new Vector3(1, -1, -1),
            new Vector3(1, 1, -1),
            new Vector3(-1, 1, -1),
            new Vector3(-1, -1, 1),
            new Vector3(1, -1, 1),
            new Vector3(1, 1, 1),
            new Vector3(-1, 1, 1)
        };

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
        }

        int[] triangles = GenerateCubeTriangles();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        cube.AddComponent<MeshRenderer>().material = material;
        cube.transform.localScale /= 2;
        projectiles.Add(cube);
        cube.SetActive(false);
    }

    private int[] GenerateCubeTriangles()
    {
        int[] triangles = new int[]
        {
            0, 2, 1, 0, 3, 2,
            4, 5, 6, 4, 6, 7,
            0, 5, 4, 0, 1, 5,
            1, 2, 6, 1, 6, 5,
            2, 3, 6, 6, 3, 7,
            0, 7, 3, 0, 4, 7
        };

        return triangles;
    }
}