using UnityEngine;

public static class MeshGenerator
{
    static Mesh quad;

    public static Mesh Quad
    {
        get
        {
            if (quad != null) return quad;

            quad = new Mesh();

            quad.vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, 0),
                new Vector3(0.5f, -0.5f, 0),
                new Vector3(-0.5f, 0.5f, 0),
                new Vector3(0.5f, 0.5f, 0)
            };

            quad.uv = new Vector2[]
            {
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(0,1),
                new Vector2(1,1)
            };

            quad.triangles = new int[]
            {
                0,2,1,
                2,3,1
            };

            quad.RecalculateNormals();

            return quad;
        }
    }
}