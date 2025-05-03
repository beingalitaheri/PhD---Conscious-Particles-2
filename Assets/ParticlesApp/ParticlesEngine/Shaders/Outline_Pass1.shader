Shader "Unlit/Outline Pass1" {
  Properties{
    _Color("Color", Color) = (1,1,1,1)
  }

  CGINCLUDE
  #include "UnityCG.cginc"

  float4 _Color;

  struct meshData {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
  };

  struct Interpolators {
    float4 vertex : SV_POSITION;
  };

  Interpolators vert(meshData v) {
    Interpolators o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    return o;
  }

  fixed4 frag(Interpolators i) : SV_Target {
    return _Color;
  }

  ENDCG

	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

		Pass {
      Cull Back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
