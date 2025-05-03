Shader "Hidden/Leap Motion/Graphic Renderer/InternalPack" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
  }
  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    Blend One Zero

    Cull Off
    ZTest Off
    ZWrite Off

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"

      struct meshData {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
      };

      struct Interpolators {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
      };

      sampler2D _MainTex;
      float4 _MainTex_ST;
      
      Interpolators vert (meshData v) {
        Interpolators o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        return o;
      }
      
      fixed4 frag (Interpolators i) : SV_Target {
        fixed4 col = tex2D(_MainTex, i.uv);
        return col;
      }
      ENDCG
    }
  }
}
