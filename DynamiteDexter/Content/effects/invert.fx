sampler s0;

float4 MyPixelShader(float4 position : SV_Position, float4 color : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	color = tex2D(s0, coords);

	color.r = 1.0 - color.r;
	color.g = 1.0 - color.g;
	color.b = 1.0 - color.b;
	
	return color;
}

technique PostProcess
{
    pass P0 { PixelShader = compile ps_4_0_level_9_1 MyPixelShader(); }
}