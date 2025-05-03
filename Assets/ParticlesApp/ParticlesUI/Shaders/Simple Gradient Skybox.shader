Shader "LeapMotion/Examples/Skybox/Simple Gradient Skybox" {
	Properties {
		_SkyColor("Sky Color", Color) = (0.37, 0.52, 0.73, 0)
		_HorizonColor("Horizon Color", Color) = (0.89, 0.96, 1, 0)
    _HorizonOffset("Horizon Offset", Float) = 0.0
    _Sharpness("Sharpness Multiplier", Range(0.2, 4)) = 1.0
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
		      float3 texcoord : TEXCOORD0;
	      };

	      half3 _SkyColor;
	      half3 _HorizonColor;
        half _HorizonOffset;
        half _Sharpness;

	      Interpolators vert(meshData v) {
		      Interpolators o;
		      o.position = UnityObjectToClipPos(v.position);
		      o.texcoord = v.texcoord;
		      return o;
	      }

	      half4 frag(Interpolators i) : COLOR {
          float3 v = normalize(i.texcoord);
          half3 color = lerp(_HorizonColor, _SkyColor, smoothstep(0, 1, (v.y + _HorizonOffset) * _Sharpness));
          return half4(color.x, color.y, color.z, 1);
	      }

			ENDCG
		}
	}
}