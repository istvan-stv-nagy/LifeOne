Shader "Custom/ParticleRender"
{
    Properties
    {
        _Size ("Size", Float) = 0.2
        _TrailLength ("Trail Length", Float) = 0.05
        _MaxTrail ("Max Trail", Float) = 0.25
        _VisuType ("Visu Type", Int) = 0
    }

    SubShader
    {
        Pass
        {
            Blend One One
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            struct ParticleStruct
            {
                float2 position;
                float2 velocity;
                int type;
                int clusterId;
            };

            StructuredBuffer<ParticleStruct> particles;

            float _Size;
            float _TrailLength;
            float _MaxTrail;
            int _VisuType;

            struct appdata
            {
                float3 vertex : POSITION;
                float2 uv : TEXCOORD0;
                uint id : SV_InstanceID;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                int type: TEXCOORD1;
                int clusterId : TEXCOORD2;
            };

            float2 ComputeTrailOffset(ParticleStruct p, float2 local_xy)
            {
                float2 vel = p.velocity;

                float speed = length(vel);
                float2 dir = float2(0, 0);
                if (speed > 0.0001)
                {
                    dir = normalize(vel);
                }
                float2 perp = float2(-dir.y, dir.x);

                float trail = min(sqrt(speed) * _TrailLength, _MaxTrail);

                float2 offset = dir * local_xy.y * trail + perp * local_xy.x * _Size;
                return offset;
            }

            v2f vert (appdata v)
            {
                v2f o;

                ParticleStruct p = particles[v.id];

                float2 offset = ComputeTrailOffset(p, v.vertex.xy);

                float3 worldPos = float3(p.position + offset, 0);

                o.pos = UnityObjectToClipPos(float4(worldPos, 1));
                o.type = p.type;
                o.clusterId = p.clusterId;
                o.uv = v.uv;

                return o;
            }

            float3 hsv2rgb(float3 c)
            {
                float4 K = float4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            float3 GetClusterColor(int id)
            {
                if (id == -1)
                {
                    return float3(0, 0, 0);
                }
                float h = frac(sin(id * 12.9898) * 43758.5453);
                return hsv2rgb(float3(h, 0.8, 1.0));
            }

            float3 GetParticleColor(int type)
            {
                if (type == 0)
                    return float3(1,1,0);
                else if (type == 1)
                    return float3(0,0,1);
                else if (type == 2)
                    return float3(1,1,1);
                else if (type == 3)
                    return float3(1,0,0);
                else if (type == 4)
                    return float3(0,1,1);
                else if (type == 5)
                    return float3(1,20.0/255.0,147.0/255.0);
                else if (type == 6)
                    return float3(107.0/255.0,142.0/255.0,35.0/255.0);
                else if (type == 7)
                    return float3(0,128.0/255.0,128.0/255.0);
                else if (type == 8)
                    return float3(245.0/255.0,245.0/255.0,220.0/255.0);
                else if (type == 9)
                    return float3(210.0/255.0,105.0/255.0,30.0/255.0);
                else
                    return float3(128.0/255.0,0,0);
            }

            float3 GetColor(int type, int clusterId)
            {
                switch (_VisuType)
                {
                    case 0: return GetParticleColor(type);
                    case 1: return GetClusterColor(clusterId);
                }
                return float3(0.5,0.5,0.5);
            }

            float ComputeGlow(v2f i)
            {
                float2 uv = i.uv - 0.5;
                float dist = length(uv);
                float glow = smoothstep(0.5, 0.0, dist);
                return glow;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 color = GetColor(i.type, i.clusterId);
                float glow = ComputeGlow(i);
                return float4(color * glow, glow);
            }

            ENDCG
        }
    }
}
