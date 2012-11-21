const float3 luminanceFilter = { 0.2125, 0.7154, 0.0721 };

shared float2 texelSize;

texture texTexture : TEXTURE0;
sampler sampTexture = sampler_state {
	Texture = (texTexture);
};

struct VS_INPUT {
	float4 position : POSITION;
	float2 texCoord : TEXCOORD0;
};
struct VS_OUTPUT {
	float2 texCoord : TEXCOORD0;
};
#define PS_INPUT VS_OUTPUT

VS_OUTPUT VS(VS_INPUT IN, out float4 outPosition : POSITION) {
	outPosition = IN.position;

	VS_OUTPUT OUT;
	OUT.texCoord = IN.texCoord + texelSize / 2;

	return OUT;
}

float4 PS(PS_INPUT IN) : COLOR {
	float3 sample = tex2D(sampTexture, IN.texCoord).rgb;
	float greyLevel = mul(sample, luminanceFilter);
			  
	return float4(greyLevel.rrr, 1);
}

technique ShaderModel3 {
	pass Main {
		VertexShader = compile vs_3_0 VS();
		PixelShader  = compile ps_3_0 PS();    
	}
}
technique ShaderModel2a {
	pass Main {
		VertexShader = compile vs_2_a VS();
		PixelShader  = compile ps_2_a PS();    
	}
}
technique ShaderModel2b {
	pass Main {
		VertexShader = compile vs_2_0 VS();
		PixelShader  = compile ps_2_b PS();    
	}
}
technique ShaderModel2 {
	pass Main {
		VertexShader = compile vs_2_0 VS();
		PixelShader  = compile ps_2_0 PS();    
	}
}