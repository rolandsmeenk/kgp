Texture2D WorldTexture : register(t0);
Texture2D<uint> BodyIndexTexture : register(t1);

RWStructuredBuffer<float3> RWPositionBuffer : register(u0); //Must have counter flag
RWStructuredBuffer<float4> RWBodyIndexBuffer : register(u1);

[numthreads(8, 8, 1)]
void CS_Filter(uint3 i : SV_DispatchThreadID)
{
	if (i.x >= 512 || i.y >= 424) { return; }

	uint pid = BodyIndexTexture[i.xy];
	float3 w = WorldTexture[i.xy].xyz;
	if (pid < 8 && length(w) < 500)
	{
		uint idx = RWPositionBuffer.IncrementCounter();
		RWPositionBuffer[idx] = w;

		float2 map = RGBDepthMapTexture.Load(int3(i.xy,0)).xy;
		RWBodyIndexBuffer[idx] = pid;
	}
}