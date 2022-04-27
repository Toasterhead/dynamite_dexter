static const float 	PIXEL_WIDTH 	= 1.0 / 800.0;
static const float 	PIXEL_HEIGHT	= 1.0 / 480.0;

sampler s0;

float4 PhaseShift( float4 position : SV_Position, float4 color : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float2 rShift = float2(-PIXEL_WIDTH, 	 0.0			);
	float2 gShift = float2( PIXEL_WIDTH, 	 0.0			);
	float2 bShift = float2( 0.0, 			-PIXEL_HEIGHT	);
	
	color.r = tex2D(s0, coords + rShift).r;
	color.g = tex2D(s0, coords + gShift).g;
	color.b = tex2D(s0, coords + bShift).b;
	
    return color;
}

technique PostProcess
{
    pass P0
    {
        PixelShader = compile ps_4_0_level_9_3 PhaseShift();
    }
}