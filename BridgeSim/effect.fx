float4x4 World;
float4x4 View;
float4x4 Projection;
float3 Diffuse;


void VS(inout float4 pos : POSITION0)
{
	pos = mul(mul(mul(pos, World), View), Projection);
}

float4 PS(float4 pos : POSITION0) : COLOR0
{
	return float4(Diffuse.xyz, 1);
}

technique tech1
{
	pass p1
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}


