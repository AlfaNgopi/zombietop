using UnityEngine;
using System.Collections.Generic;

public class VisionCone : MonoBehaviour
{
    public float viewRadius = 5f;
    [Range(0, 360)] public float viewAngle = 90f;
    public int rayCount = 50; // More rays = smoother edges
    public LayerMask obstacleMask;

    private Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void LateUpdate() // Use LateUpdate so player movement is finished
    {
        DrawFOV();
    }

    void DrawFOV()
    {
        float angleStep = viewAngle / rayCount;
        float currentAngle = -viewAngle / 2;
        
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // The first vertex is the center (player position)
        vertices.Add(Vector3.zero); 

        for (int i = 0; i <= rayCount; i++)
        {
            // Calculate the direction of the ray
            Vector3 dir = Quaternion.Euler(0, 0, currentAngle) * transform.up;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);

            Vector3 vertex;
            if (hit.collider == null)
            {
                vertex = dir * viewRadius; // No hit, extend to max range
            }
            else
            {
                // Convert world hit point to local space relative to player
                vertex = transform.InverseTransformPoint(hit.point);
            }

            vertices.Add(vertex);

            // Build triangles (0 is center, i is current, i+1 is next)
            if (i > 0)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }

            currentAngle += angleStep;
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}