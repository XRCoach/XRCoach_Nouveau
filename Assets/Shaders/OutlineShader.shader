Shader "Custom/OutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,1,0,1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.03
        _OutlineIntensity ("Outline Intensity", Range(0, 2)) = 1.0
        
        // Paramètres pour le feedback de précision
        _AccuracyColor ("Accuracy Color", Color) = (0,1,0,1)
        _AccuracyBlend ("Accuracy Blend", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        
        // Pass 1: Outline
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Front
            ZWrite On
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };
            
            float _OutlineWidth;
            float4 _OutlineColor;
            float _OutlineIntensity;
            
            v2f vert(appdata v)
            {
                v2f o;
                
                // Extrude les vertices le long de la normale
                float3 norm = normalize(v.normal);
                float3 outlinePos = v.vertex.xyz + norm * _OutlineWidth;
                
                o.pos = UnityObjectToClipPos(float4(outlinePos, 1.0));
                o.color = _OutlineColor * _OutlineIntensity;
                
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
        
        // Pass 2: Base avec couleur de précision
        Pass
        {
            Name "BASE"
            Tags { "LightMode" = "ForwardBase" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _AccuracyColor;
            float _AccuracyBlend;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                // Texture de base
                fixed4 texColor = tex2D(_MainTex, i.uv);
                
                // Lighting simple
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(0, dot(i.worldNormal, lightDir));
                float3 lighting = NdotL * _LightColor0.rgb + UNITY_LIGHTMODEL_AMBIENT.rgb;
                
                // Blend avec la couleur de précision
                fixed4 finalColor = texColor * _Color;
                finalColor.rgb = lerp(finalColor.rgb, _AccuracyColor.rgb, _AccuracyBlend);
                finalColor.rgb *= lighting;
                
                return finalColor;
            }
            ENDCG
        }
    }
    
    FallBack "Diffuse"
}