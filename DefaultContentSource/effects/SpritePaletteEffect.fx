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
	return max(input.b, max(input.g, input.r));
}

float2 PaletteCoords (float3 base, float3 input)
{
	//build a ordered list of the currently used channels
	//apply input color such that zero values are bypassed
	//offset green and blue channels so they overwrite each other
	float3 _activeChannels = float3
	( 
		saturate(base.r * input.r * 65025), 
		saturate(base.g * input.g * 65025) * 2, 
		saturate(base.b * input.b * 65025) * 3
	);

	//the max value is now the index of the channel we need
	//remove 1 to account for 1-based indexing above
	float _index = Max3(_activeChannels) - 1;

	float _x = base[_index];
	float _y = input[_index];

	return float2(_x,_y);
}

float4 MainPS( VertexShaderOutput input ) : COLOR
{
	float4 _baseColor = tex2D( s0, input.TextureCoordinates );
	
    return tex2D(_paletteTextureSampler, PaletteCoords(_baseColor.rgb, input.Color.rgb)) * (input.Color.a * _baseColor.a);
}


technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile ps_2_0 MainPS();
	}
};