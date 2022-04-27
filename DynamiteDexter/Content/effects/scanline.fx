static const float MAX_BRIGHTNESS 	= 1.0;
static const float BOOST_RATIO 		= 0.25;

float surfaceHeight;
float brightness;

sampler s0;

float4 Scanline(float4 position : SV_Position, float4 color : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	int verticalPosition = (int)(coords.y * surfaceHeight); 
	
	if (brightness > MAX_BRIGHTNESS)
		brightness = MAX_BRIGHTNESS;
		
	float boost = brightness * BOOST_RATIO;

	color = tex2D(s0, coords);
	
	if (verticalPosition % 2 != 0)
		color.rgb *= 0.5 + boost;

	return color;
}

technique PostProcess
{
    pass P0
    {
        PixelShader = compile ps_4_0_level_9_3 Scanline();
    }
}