float4x4 World;
float4x4 View;
float4x4 Projection;
float3 Diffuse;
float3 DiffuseDir;
sampler tex0 : register(s0);

void VS(inout float4 pos : POSITION0, inout float4 norm : NORMAL0, out float4 diffuse : COLOR0, inout float2 tc : TEXCOORD0)
{
	pos = mul(mul(mul(pos, World), View), Projection);
	norm.w = 0;
	norm = normalize(mul(norm, World));
	float a = saturate(dot(DiffuseDir, norm.xyz)) * 0.7 + 0.3;
	diffuse = float4(Diffuse.xyz * a, 1);
}

void PS(float4 pos : POSITION0, inout float4 diffuse : COLOR0, float2 tc : TEXCOORD0)
{
	diffuse *= tex2D(tex0, tc);
}

technique tech1
{
	pass p1
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}


