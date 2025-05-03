Shader "Unlit/DebugVelocities" {
	Properties {
    _Scale   ("Scale", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

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

			sampler2D _ParticleVelocities;
      float _Scale;
			
			Interpolators vert (meshData v) {
				Interpolators o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (Interpolators i) : SV_Target {
				fixed4 col = abs(tex2D(_ParticleVelocities, i.uv));
        return col * _Scale;
			}
			ENDCG
		}
	}
}
