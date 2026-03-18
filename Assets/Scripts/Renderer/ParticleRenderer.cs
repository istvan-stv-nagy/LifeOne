using UnityEngine;

public class ParticleRenderer : MonoBehaviour
{
    [SerializeField]
    private Material material;

    private ComputeBuffer particleBuffer;
    private int particleCount;

    public void SetParticleCount(int count)
    {
        particleCount = count;
    }

    public void SetParticleComputeBuffer(ComputeBuffer buffer)
    {
        particleBuffer = buffer;
    }

    public void SetVisuType(int visuType)
    {
        material.SetInt("_VisuType", visuType);
    }

    void Update()
    {
        material.SetBuffer("particles", particleBuffer);

        Graphics.DrawMeshInstancedProcedural(
            MeshGenerator.Quad,
            0,
            material,
            new Bounds(Vector3.zero, Vector3.one * 100),
            particleCount
        );
    }
}