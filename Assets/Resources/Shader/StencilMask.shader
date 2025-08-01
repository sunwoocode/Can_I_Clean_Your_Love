Shader "Custom/StencilMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        ColorMask 0
        ZWrite Off

        Stencil
        {
            Ref 1
            Comp always
            Pass replace
        }

        Pass
        {
            // 여기는 비워둠. Mask만 찍히고 렌더는 안 함
        }
    }
}
