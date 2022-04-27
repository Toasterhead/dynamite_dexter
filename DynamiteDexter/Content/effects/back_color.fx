float addR;
float addG;
float addB;

sampler s0;

float4 Scanline(float4 position : SV_Position, float4 color : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	color = tex2D(s0, coords);
	
	color.r += addR;
	color.g += addG;
	color.b += addB;

	return color;
}

technique PostProcess
{
    pass P0
    {
        PixelShader = compile ps_4_0_level_9_3 Scanline();
    }
}