sampler s0; // from SpriteBatch

texture _paletteTexture;

sampler2D _paletteTextureSampler = sampler_state
{
	Texture = <_paletteTexture>;
    AddressU = Clamp;
    AddressV = Clamp;
    MagFilter = Point;
    MinFilter = Point;
};


struct VertexShaderOutput
{
	float4 Position : POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float Max3 (float3 input)
{
	return max(input.r, max(input.g, input.b));
}

float2 PaletteCoords (float3 base, float3 input)
{
	//get the x coord from the base pixel
	float _x = Max3(base);
	
	//isolate the correct channel by forcing the channels to 1 if above 0
	//shouldnt ever be more than 1 channel in use per pixel
	float _y = Max3(clamp(base * float3(256, 256, 256), 0, 1) * input);

	return float2(_x,_y);
}

float4 MainPS( VertexShaderOutput input ) : COLOR
{
	float4 _baseColor = tex2D( s0, input.TextureCoordinates );
	
    return tex2D(_paletteTextureSampler, PaletteCoords(_baseColor.rgb, input.Color.rgb)) * _baseColor.a;
}


technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile ps_2_0 MainPS();
	}
};