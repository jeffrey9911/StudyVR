﻿//This is a modified version of a shader published by smb02dunnal on the Unity forums:
//https://forum.unity3d.com/threads/billboard-geometry-shader.169415/

//Authors:
//Christopher Remde @Charite University Hospital Berlin & Alexander Radacki @NMY Mixed-Reality Communication GmbH

Shader "Pointcloud/Pointcloud"
{
    Properties
    {
        _PointSize("PointSize", Range(0, 0.1)) = 0.01
        _DistanceScale("DistanceScale", Range(0, 2.0)) = 1.3
        _MinPointSize("MinPointSize", Range(0, 0.1)) = 0.002
        _MaxX("MaxX", Range(-1000, 1000)) = 1000
        _MinX("MinX", Range(-1000, 1000)) = -1000
        _MaxZ("MaxZ", Range(-1000, 1000)) = 1000
        _MinZ("MinZ", Range(-1000, 1000)) = -1000
        _MaxY("MaxY", Range(-1000, 1000)) = 1000
        _MinY("MinY", Range(-1000, 1000)) = -1000
    }
        SubShader
    {
        Pass
        {
            Tags{ "RenderType" = "Opaque" }
            LOD 200
            Cull Off
            CGPROGRAM

            #pragma target 5.0
            #pragma vertex VS_Main
            #pragma fragment FS_Main
            #pragma geometry GS_Main
            #include "UnityCG.cginc" 
            #include "AutoLight.cginc"

        // **************************************************************
        // Data structures                                              *
        // **************************************************************
        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            float4 color : COLOR;

            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
        struct GS_INPUT
        {
            float4  pos     : POSITION;
            float4  col     : COLOR;
            LIGHTING_COORDS(0, 1)

            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };
        struct FS_INPUT
        {
            float4  pos     : POSITION;
            float4  col     : COLOR;

            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };

        // **************************************************************
        // Vars                                                         *
        // **************************************************************
        float _PointSize;
        float _DistanceScale;
        float _MinPointSize;
        float _MaxX;
        float _MinX;
        float _MaxZ;
        float _MinZ;
        float _MaxY;
        float _MinY;

        // **************************************************************
        // Shader Programs                                              *
        // **************************************************************
        // Vertex Shader ------------------------------------------------
        GS_INPUT VS_Main(appdata v)
        {
            GS_INPUT output;
            // init
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_OUTPUT(GS_INPUT, output);
            // transfer id to gs
            UNITY_TRANSFER_INSTANCE_ID(v, output);

            output.pos = v.vertex;
            output.col = v.color;
            return output;
        }

        // Geometry Shader -----------------------------------------------------
        [maxvertexcount(4)]
        void GS_Main(point GS_INPUT p[1], inout TriangleStream<FS_INPUT> triStream)
        {
            FS_INPUT pIn;

            // init
            UNITY_SETUP_INSTANCE_ID(p[0]);
            UNITY_INITIALIZE_OUTPUT(FS_INPUT, pIn);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(pIn);

            float3 up = UNITY_MATRIX_IT_MV[1].xyz;
            float3 right = -UNITY_MATRIX_IT_MV[0].xyz;
            float dist = length(ObjSpaceViewDir(p[0].pos));
            float halfS = dist * _DistanceScale * _PointSize + _MinPointSize;
            float4 v[4];
            v[0] = float4(p[0].pos + halfS * right - halfS * up, 1.0f);
            v[1] = float4(p[0].pos + halfS * right + halfS * up, 1.0f);
            v[2] = float4(p[0].pos - halfS * right - halfS * up, 1.0f);
            v[3] = float4(p[0].pos - halfS * right + halfS * up, 1.0f);

            // WoldSpace clip
            //float tempX= ( mul(unity_ObjectToWorld, p[0].pos)).x;
            //float tempY= ( mul(unity_ObjectToWorld, p[0].pos)).y;
            //float tempZ= ( mul(unity_ObjectToWorld, p[0].pos)).z;

            // ObjectSpace clip
            float tempX = p[0].pos.x;
            float tempY = p[0].pos.y;
            float tempZ = p[0].pos.z;

            if (_MaxX < tempX || _MinX > tempX || _MaxZ < tempZ || _MinZ > tempZ || _MaxY < tempY || _MinY > tempY)
            {
                p[0].col.r = 0;
                p[0].col.g = 0;
                p[0].col.b = 0;
            }
            else
            {
                pIn.pos = UnityObjectToClipPos(v[0]);
                pIn.col = p[0].col;
                triStream.Append(pIn);
                pIn.pos = UnityObjectToClipPos(v[1]);
                pIn.col = p[0].col;
                triStream.Append(pIn);
                pIn.pos = UnityObjectToClipPos(v[2]);
                pIn.col = p[0].col;
                triStream.Append(pIn);
                pIn.pos = UnityObjectToClipPos(v[3]);
                pIn.col = p[0].col;
                triStream.Append(pIn);
            }
        }
        // Fragment Shader -----------------------------------------------
        float4 FS_Main(FS_INPUT input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
            float atten = LIGHT_ATTENUATION(input);
            return input.col;
        }
        ENDCG
    }
    }
}




