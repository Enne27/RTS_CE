Shader "Custom/RTSFogOfWar"
{
    Properties
    {
        _FogTex ("Fog Texture (Auto-Asignada)", 2D) = "black" {}
        _FogColor ("Color de la Niebla", Color) = (0,0,0,1)
        _ExploredAlpha ("Opacidad de Zona Explorada", Range(0,1)) = 0.5
    }
    SubShader
    {
        // Configuramos el shader para que sea transparente y se dibuje sobre el mundo
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _FogTex;
            float4 _FogColor;
            float _ExploredAlpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Leer la textura generada por nuestro Manager
                fixed4 fogData = tex2D(_FogTex, i.uv);
                
                float isVisible = fogData.r;   // Canal rojo = Visión actual
                float isExplored = fogData.g;  // Canal verde = Historial

                // Matemática de mezcla sin 'if' para máximo rendimiento:
                // Si isVisible es 1 -> finalAlpha es 0 (Transparente).
                // Si isVisible es 0 y isExplored es 1 -> finalAlpha es _ExploredAlpha (Gris).
                // Si ambos son 0 -> finalAlpha es 1 (Negro total).
                float finalAlpha = 1.0 - max(isVisible, isExplored * (1.0 - _ExploredAlpha));
                
                return fixed4(_FogColor.rgb, finalAlpha * _FogColor.a);
            }
            ENDCG
        }
    }
}