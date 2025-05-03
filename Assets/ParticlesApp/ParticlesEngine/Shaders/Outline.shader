Shader "Unlit/Outline" {
  Properties {
    _Color   ("Color",         Color) = (1,1,1,1)
    _Outline ("Outline Color", Color) = (1,1,1,1)
    _Width   ("Outline Width", Float) = 0.01
  }

  CGINCLUDE
  #include "UnityCG.cginc"

  float4 _Color;
  float4 _Outline;
  float _Width;

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

  Interpolators vert_extrude(meshData v) {
    Interpolators o;
    o.vertex = UnityObjectToClipPos(v.vertex + float4(_Width * v.normal, 0));
    return o;
  }

  fixed4 frag(Interpolators i) : SV_Target {
    return _Color;
  }

  fixed4 frag_outline(Interpolators i) : SV_Target{
    return _Outline;
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

    Pass{
      Cull Front

      CGPROGRAM
      #pragma vertex vert_extrude
      #pragma fragment frag_outline
      ENDCG
    }
	}
}
