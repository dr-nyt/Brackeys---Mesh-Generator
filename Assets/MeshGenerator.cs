using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour {
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    [Header("Mesh Settings")]
    public int xSize = 20;
    public int zSize = 20;
    public int textureWidth = 1024;
    public int textureHeight = 1024;

    [Header("Wave 1")]
    public float frequency1;
    public float amplitude1;

    [Header("Wave 2")]
    public float frequency2;
    public float amplitude2;

    [Header("Wave 3")]
    public float frequency3;
    public float amplitude3;

    public float noiseStrength;

    public Gradient gradient;
    float minTerrainHeight;
    float maxTerrainHeight;

    // Start is called before the first frame update
    void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //StartCoroutine(CreateShapeDelayed());
        CreateShape();
    }

    void Update() {
        UpdateMesh();
    }

    IEnumerator CreateShapeDelayed() {
        initArrays();

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++) {
            for (int x = 0; x < xSize; x++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;

                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;

                yield return new WaitForSeconds(.01f);
            }
            vert++;
        }

        createUVs();
    }

    void CreateShape() {
        initArrays();

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++) {
            for (int x = 0; x < xSize; x++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;

                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        createUVs();
    }

    void UpdateMesh() {
        mesh.Clear();
        
        updateVertices();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    void initArrays() {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        updateVertices();
        triangles = new int[xSize * zSize * 6];
    }

    void updateVertices() {
        for (int i = 0, z = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {
                float y = amplitude1 * Mathf.PerlinNoise(x * frequency1, z * frequency1)
                        + amplitude2 * Mathf.PerlinNoise(x * frequency2, z * frequency2)
                        + amplitude3 * Mathf.PerlinNoise(x * frequency3, z * frequency3)
                        * noiseStrength;
                vertices[i] = new Vector3(x - (xSize / 2), y, z - (zSize / 2));

                if (y > maxTerrainHeight) maxTerrainHeight = y;
                else if (y < minTerrainHeight) minTerrainHeight = y;

                i++;
            }
        }
    }

    void createUVs() {
        colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= zSize; z++) {
            for (int x = 0; x <= xSize; x++) {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    /*private void OnDrawGizmos() {
        if (vertices == null) return;

        for (int i = 0; i < vertices.Length; i++) {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }*/
}
