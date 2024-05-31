Shader "Alphi/MyLit"
{

    Properties
    {
        [Header(Surface options)] // Creates a text header
        // [MainColor] allows Material.color to use the correct property
        [MainColor]_ColorTint("Color", Color) = (1, 1, 1, 1)
        [MainTexture]_MainTexture("Texture", 2D) = "white" {} 
        [NoiseTexture]_NoiseTexture("Texture", 2D) = "white" {} 
        [ScaleVertex]_Scale("Scale", Float) = 1
        [uvOffset]_uvOffset("Offset", Vector) = (1, 1, 1, 1)
        [Specular]_Specular("Specular", Range(0,1)) = 1
        [Smoothness]_Smoothness("Smoothness", Range(0,1)) = 1
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipline" }

        Pass
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}
        
            HLSLPROGRAM
        
            #define _SPECULAR_COLOR

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "MyLitForwardLitPass.hlsl"

            ENDHLSL
        }        
    }
}
