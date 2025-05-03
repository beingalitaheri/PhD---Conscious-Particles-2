Shader "LeapMotion/Examples/Skybox/Simple Color Skybox" {
	Properties {
		_SkyColor("Sky Color", Color) = (0.37, 0.52, 0.73, 0)
	}
	SubShader {
		Tags{ "RenderType" = "Skybox" "Queue" = "Background" }
	  Pass
		{
			ZWrite Off
			Cull Off
			Fog { Mode Off }

			CGPROGRAM

        #pragma target 2.0
			  #pragma fragmentoption ARB_precision_hint_fastest
			  #pragma vertex vert
			  #pragma fragment frag

        #include "UnityCG.cginc"

		    struct meshData {
		      float4 position : POSITION;
		      float3 texcoord : TEXCOORD0;
	      };

	      struct Interpolators {
		      float4 position : SV_POSITION;
	      };

	      half3 _SkyColor;

	      Interpolators vert(meshData v) {
		      Interpolators o;
		      o.position = UnityObjectToClipPos(v.position);
		      return o;
	      }

	      half4 frag(Interpolators i) : COLOR {
          return half4(_SkyColor.x, _SkyColor.y, _SkyColor.z, 1);
	      }

			ENDCG
		}
	}
}