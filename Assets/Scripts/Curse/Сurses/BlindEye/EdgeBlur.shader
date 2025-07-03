Shader "Custom/EdgeBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurIntensity ("Blur Intensity", Range(0, 1)) = 0.2
        _EdgeWidth ("Edge Width", Range(0, 0.5)) = 0.3
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
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
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BlurIntensity;
            float _EdgeWidth;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float2 uv = i.uv;
                
                
                float2 dist = abs(uv - center);
                float maxDist = max(dist.x, dist.y);
                
                
                float edgeFactor = smoothstep(0.5 - _EdgeWidth, 0.5, maxDist);
                
                
                fixed4 col = tex2D(_MainTex, uv);
                
               
                if (edgeFactor > 0)
                {
                    fixed4 blurred = fixed4(0, 0, 0, 0);
                    float blurSize = _BlurIntensity * edgeFactor * 0.01;
                    
                    
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            float2 offset = float2(x, y) * blurSize;
                            blurred += tex2D(_MainTex, uv + offset);
                        }
                    }
                    blurred /= 9.0;
                    
                    
                    col = lerp(col, blurred, edgeFactor);
                }
                
                return col;
            }
            ENDCG
        }
    }
}