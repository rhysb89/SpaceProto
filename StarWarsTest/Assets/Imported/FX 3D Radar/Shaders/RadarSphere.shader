// Shader created with Shader Forge v1.25 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.25;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32724,y:32693,varname:node_4795,prsc:2|emission-7438-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31847,y:32531,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:32118,y:32726,varname:node_2393,prsc:2|A-6074-RGB,B-2053-RGB,C-797-RGB,D-9248-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:31847,y:32702,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:31847,y:32861,ptovrint:True,ptlb:SphereColor,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.2833045,c2:0.456804,c3:0.9632353,c4:1;n:type:ShaderForge.SFN_Vector1,id:9248,x:31847,y:33012,varname:node_9248,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Add,id:7438,x:32550,y:32914,varname:node_7438,prsc:2|A-2393-OUT,B-6637-OUT;n:type:ShaderForge.SFN_NormalVector,id:769,x:31562,y:33473,prsc:2,pt:False;n:type:ShaderForge.SFN_ViewVector,id:3323,x:31562,y:33641,varname:node_3323,prsc:2;n:type:ShaderForge.SFN_Dot,id:1426,x:31840,y:33532,varname:node_1426,prsc:2,dt:0|A-769-OUT,B-3323-OUT;n:type:ShaderForge.SFN_Step,id:6482,x:32408,y:33494,varname:node_6482,prsc:2|A-1426-OUT,B-9614-OUT;n:type:ShaderForge.SFN_Slider,id:9614,x:32036,y:33633,ptovrint:False,ptlb:LineWidth,ptin:_LineWidth,varname:node_9614,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.48,max:1;n:type:ShaderForge.SFN_Color,id:7535,x:32287,y:33306,ptovrint:False,ptlb:LineColor,ptin:_LineColor,varname:node_7535,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:6637,x:32587,y:33302,varname:node_6637,prsc:2|A-7535-RGB,B-6482-OUT;proporder:6074-797-9614-7535;pass:END;sub:END;*/

Shader "ForceX/RadarSphere" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("SphereColor", Color) = (0.2833045,0.456804,0.9632353,1)
        _LineWidth ("LineWidth", Range(0, 1)) = 0.48
        _LineColor ("LineColor", Color) = (0.5,0.5,0.5,1)
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform float _LineWidth;
            uniform float4 _LineColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = ((_MainTex_var.rgb*i.vertexColor.rgb*_TintColor.rgb*0.5)+(_LineColor.rgb*step(dot(i.normalDir,viewDirection),_LineWidth)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    //CustomEditor "ShaderForgeMaterialInspector"
}
