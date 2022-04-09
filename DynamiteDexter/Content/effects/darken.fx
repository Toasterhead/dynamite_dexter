float rectLeft;
float rectTop;
float rectRight;
float rectBottom;
float fieldWidth;
float fieldHeight;
sampler s0;

float4 MyPixelShader(float4 position : SV_Position, float4 color : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float left 		= rectLeft 		/ fieldWidth;
	float top 		= rectTop 		/ fieldHeight;
	float right 	= rectRight 	/ fieldWidth;
	float bottom	= rectBottom	/ fieldHeight;
	
	color = tex2D(s0, coords);

	if (coords.x < left || coords.x >= right || coords.y < top || coords.y >= bottom)
		color.rgb = 0.0f;
	
	return color;
}

technique PostProcess
{
    pass P0 { PixelShader = compile ps_4_0_level_9_1 MyPixelShader(); }
}