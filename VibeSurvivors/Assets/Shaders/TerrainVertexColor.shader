Shader "Custom/TerrainVertexColor"
{
    Properties
    {
      _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    
    SubShader
    {
        Tags 
        { 
      "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
    "Queue" = "Geometry"
        }
        LOD 300

        Pass
        {
  Name "ForwardLit"
    Tags { "LightMode" = "UniversalForward" }

       HLSLPROGRAM
       #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
          #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
     #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
       #pragma multi_compile _ _SHADOWS_SOFT
    
  #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

   struct Attributes
            {
     float4 positionOS : POSITION;
         float3 normalOS : NORMAL;
        float2 uv : TEXCOORD0;
      float4 color : COLOR;
            };

            struct Varyings
            {
      float4 positionCS : SV_POSITION;
        float2 uv : TEXCOORD0;
  float4 color : COLOR;
           float3 positionWS : TEXCOORD1;
         float3 normalWS : TEXCOORD2;
          };

   TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

    CBUFFER_START(UnityPerMaterial)
    float4 _BaseMap_ST;
     float4 _BaseColor;
   float _Smoothness;
        float _Metallic;
      CBUFFER_END

            Varyings vert(Attributes input)
         {
     Varyings output;
    
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
          VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
           
          output.positionCS = vertexInput.positionCS;
      output.positionWS = vertexInput.positionWS;
    output.normalWS = normalInput.normalWS;
   output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
      output.color = input.color;
       
                return output;
            }

     half4 frag(Varyings input) : SV_Target
    {
              // Texture ve vertex color
     half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
           half4 albedo = baseMap * input.color * _BaseColor;
       
      // Lighting
    InputData lightingInput = (InputData)0;
         lightingInput.positionWS = input.positionWS;
 lightingInput.normalWS = normalize(input.normalWS);
        lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                lightingInput.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
     
        // Surface data
     SurfaceData surfaceData = (SurfaceData)0;
        surfaceData.albedo = albedo.rgb;
   surfaceData.alpha = albedo.a;
    surfaceData.metallic = _Metallic;
           surfaceData.smoothness = _Smoothness;
         surfaceData.normalTS = half3(0, 0, 1);
surfaceData.occlusion = 1.0;
            surfaceData.emission = 0;
         
         // Calculate lighting
         half4 color = UniversalFragmentPBR(lightingInput, surfaceData);
           
         return color;
   }
            ENDHLSL
        }
        
        // Shadow caster pass
        Pass
        {
   Name "ShadowCaster"
       Tags { "LightMode" = "ShadowCaster" }

   ZWrite On
   ZTest LEqual
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
  #pragma fragment ShadowPassFragment
    
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
struct Attributes
    {
                float4 positionOS : POSITION;
      float3 normalOS : NORMAL;
    };

     struct Varyings
  {
      float4 positionCS : SV_POSITION;
            };

  float3 _LightDirection;

            Varyings ShadowPassVertex(Attributes input)
    {
      Varyings output;
   float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
        float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
      return output;
 }

    half4 ShadowPassFragment(Varyings input) : SV_TARGET
     {
                return 0;
  }
  ENDHLSL
        }
        
        // Depth only pass
        Pass
        {
       Name "DepthOnly"
   Tags { "LightMode" = "DepthOnly" }

            ZWrite On
    ColorMask 0

 HLSLPROGRAM
    #pragma vertex DepthOnlyVertex
 #pragma fragment DepthOnlyFragment      
          #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    
            struct Attributes
            {
    float4 positionOS : POSITION;
  };

            struct Varyings
        {
                float4 positionCS : SV_POSITION;
     };

  Varyings DepthOnlyVertex(Attributes input)
            {
           Varyings output;
        output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
       return output;
            }

    half4 DepthOnlyFragment(Varyings input) : SV_TARGET
            {
                return 0;
      }
        ENDHLSL
   }
    }
    
    FallBack "Universal Render Pipeline/Lit"
}
