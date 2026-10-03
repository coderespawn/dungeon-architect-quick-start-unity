Shader "DungeonArchitect.Samples.ShooterGame/MiniMap" {
 
	Properties {
	    _Color ("Main Color (A=Opacity)", Color) = (1,1,1,1)
	    _MainTex ("Base (A=Opacity)", 2D) = ""
	}
	 
	SubShader {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
       
        ZWrite Off

        Pass {
        	Blend SrcAlpha OneMinusSrcAlpha
            SetTexture[_MainTex] {
                Combine texture * constant ConstantColor[_Color]
            }
        }
    }
	 
}
