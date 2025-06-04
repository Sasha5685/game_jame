﻿Shader "Hidden/UModelerX/DrawFace"
{
    Properties
    {
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)] _CullMode("CullMode", Float) = 0
	}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off
        ZWrite Off
        ZTest[_ZTest]

        Cull[_CullMode]
        Offset -1, -1

        Pass
        {
            Name "Face"

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "UModelerX.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
                float4 color : TEXCOORD2;
                // y는 softSelection. 0이면 color를 넣어줌.
                float2 edgeData : TEXCOORD0;
			};

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
                float softSelection : TEXCOORD0;
            };

			v2f vert (appdata v)
			{
				v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.softSelection = v.edgeData.y;
                return o;
			}
			
            float4 frag(v2f i) : SV_Target
            {
                return Temperature(i.color, i.softSelection);
			}
			ENDCG
        }
	}
}
