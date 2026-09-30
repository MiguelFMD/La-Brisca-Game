Shader "UI/ProceduralHealthBar"
{
    Properties
    {
        _Health ("Vida (0 a 1)", Range(0, 1)) = 1.0
        _AspectRatio ("Relación de Aspecto (Ancho/Alto)", Float) = 4.0
        
        [Header(Colores)]
        _FillColor ("Color de Llenado", Color) = (0.2, 0.8, 0.2, 1.0)
        _LowHealthColor ("Color Vida Baja", Color) = (0.8, 0.1, 0.1, 1.0)
        _Threshold ("Umbral Vida Baja", Range(0, 1)) = 0.25
        _BgColor ("Color de Fondo", Color) = (0.1, 0.1, 0.1, 0.8)
        
        [Header(Bordes y Forma)]
        _BorderColor ("Color del Borde", Color) = (0.9, 0.9, 0.9, 1.0)
        _BorderWidth ("Grosor del Borde", Range(0, 0.5)) = 0.05
        _Rounding ("Redondeo de Esquinas", Range(0, 0.5)) = 0.15
        
        [Header(Segmentos)]
        _Segments ("Cantidad de Segmentos", Range(1, 20)) = 1.0
        _SegmentWidth ("Grosor del Segmento", Range(0, 0.1)) = 0.01

        // Propiedades requeridas para que funcione con Máscaras de UI en Unity
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            float _Health;
            float _AspectRatio;
            float4 _FillColor;
            float4 _LowHealthColor;
            float _Threshold;
            float4 _BgColor;
            float4 _BorderColor;
            float _BorderWidth;
            float _Rounding;
            float _Segments;
            float _SegmentWidth;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color;
                return o;
            }

            // Función para calcular bordes perfectos (Signed Distance Field)
            float RoundedBoxSDF(float2 p, float2 b, float r)
            {
                float2 q = abs(p) - b + r;
                return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - r;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;

                // Ajustar la relación de aspecto para que el redondeo no se deforme
                float2 sdfUV = uv - 0.5;
                sdfUV.x *= _AspectRatio;

                float2 boxSize = float2(0.5 * _AspectRatio, 0.5);
                float r = _Rounding;

                // Distancia del borde exterior
                float d = RoundedBoxSDF(sdfUV, boxSize, r);
                float smoothing = fwidth(d); 
                smoothing = max(smoothing, 0.001); // Suavizado anti-aliasing
                float alpha = 1.0 - smoothstep(0.0, smoothing, d);

                // Distancia del borde interior (Fondo y Relleno)
                float innerR = max(0.0, r - _BorderWidth);
                float innerD = RoundedBoxSDF(sdfUV, boxSize - _BorderWidth, innerR);
                float innerMask = 1.0 - smoothstep(0.0, smoothing, innerD);

                // Lógica de llenado de vida (Mapeada solo al área interior)
                float xMin = _BorderWidth / _AspectRatio;
                float xMax = 1.0 - xMin;
                float fillUvX = (uv.x - xMin) / (xMax - xMin);
                
                // Cortar todo lo que esté por encima del valor de vida actual
                float fillMask = step(fillUvX, _Health);

                // Lógica de Separadores/Segmentos
                float segmentMask = 1.0;
                if (_Segments > 1.0)
                {
                    float segUv = fillUvX * _Segments;
                    float distToEdge = min(frac(segUv), 1.0 - frac(segUv));
                    segmentMask = smoothstep(_SegmentWidth * _Segments, (_SegmentWidth * _Segments) + 0.01, distToEdge);
                }

                // Efecto de Parpadeo cuando la vida es baja
                float4 currentFillColor = _FillColor;
                if (_Health <= _Threshold)
                {
                    // Pulso suave basado en el tiempo
                    float pulse = (sin(_Time.y * 10.0) * 0.5 + 0.5) * 0.4;
                    currentFillColor = lerp(_LowHealthColor, _LowHealthColor + float4(pulse, pulse, pulse, 0), pulse);
                }

                // --- MEZCLA DE COLORES ---
                float4 finalColor = _BorderColor;
                float4 bgColor = _BgColor;

                // 1. Mezclar fondo con barra de vida y segmentos
                float4 innerColor = lerp(bgColor, currentFillColor, fillMask * segmentMask);

                // 2. Mezclar el interior con el borde exterior
                finalColor = lerp(finalColor, innerColor, innerMask);

                // 3. Aplicar transparencia global y color del componente UI
                finalColor.a *= alpha * IN.color.a;

                return finalColor;
            }
            ENDCG
        }
    }
}