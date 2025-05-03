Shader "LeapMotion/DetectionExamples/PinchLine" {
  Properties{
    _Color("Tint", Color) = (1,1,1,1)
    _Glossiness("Smoothness", Range(0,1)) = 0.5
    _Metallic("Metallic", Range(0,1)) = 0.0
  }
  SubShader{
    Tags { "RenderType" = "Opaque" }
    LOD 200

    HLSLPROGRAM
    #pragma surface surf Standard fullforwardshadows
    #pragma target 3.0
    #include <UnityPBSLighting.cginc>

    struct Input {
      float4 color : COLOR;
    };

    half _Glossiness;
    half _Metallic;
    fixed4 _Color;

    void surf(Input IN, inout SurfaceOutputStandard o) {
      o.Albedo = (_Color * IN.color).rgb;
      o.Metallic = _Metallic;
      o.Smoothness = _Glossiness;
    }
    ENDHLSL
  }
  FallBack "Diffuse"
}
