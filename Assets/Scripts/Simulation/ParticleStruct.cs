using UnityEngine;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct ParticleStruct
{
    public Vector2 position;
    public Vector2 velocity;
    public int type;
    public int clusterId;
}
