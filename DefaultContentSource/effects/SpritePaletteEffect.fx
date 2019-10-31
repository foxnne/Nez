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
	//offset the green and blue channels from red such that 
	//green overwrites red, and blue overwrites green, regardless of value
	//multiply by non-zero values of input to allow passthrough
	float3 _baseOffsets = float3
	( 
		base.r * ceil(input.r), 
		base.g * ceil(input.g) * 2, 
		base.b * ceil(input.b) * 3
	);

	//select the channel of the greatest value
	float _maxOffset = Max3(_baseOffsets);
	float3 _baseSelect = float3
	(
		step(_maxOffset, _baseOffsets.r),
		step(_maxOffset, _baseOffsets.g),
		step(_maxOffset, _baseOffsets.b)
	);

	//at this point there is only one non-zero value
	//multiply input by selection and get the max
	//use 123 to avoid zeroing out the red channel
	float _index = Max3(_baseSelect * float3(1,2,3)) - 1;

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