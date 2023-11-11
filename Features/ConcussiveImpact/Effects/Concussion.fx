sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float uOpacity;
float3 uSecondaryColor;
float uTime;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uImageOffset;
float uIntensity;
float uProgress;
float2 uDirection;
float2 uZoom;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;

bool active;
float4 concussiveShade;
float concussiveProgress;
float dazeProgress;
float2 dazeOffset;

float4 Tint(float4 oldColor, float4 refColor, float tintProgress)
{
    float4 colorDifference = (refColor - oldColor) * tintProgress;
    return oldColor + colorDifference;
}

float4 Concussion(float4 position : SV_POSITION, float2 coords : TEXCOORD0) : COLOR0
{
    if (!active)
    {
        return tex2D(uImage0, coords);
    }
        
    float4 output = Tint(tex2D(uImage0, coords), tex2D(uImage0, coords + dazeOffset), dazeProgress);
    output = Tint(output, tex2D(uImage0, coords + float2(-dazeOffset.x, dazeOffset.y)), dazeProgress);
    output = Tint(output, tex2D(uImage0, coords + float2(dazeOffset.y, dazeOffset.x)), dazeProgress);
    output = Tint(output, tex2D(uImage0, coords + float2(-dazeOffset.y, dazeOffset.x)), dazeProgress);
    output = Tint(output, concussiveShade, concussiveProgress);
    return output;
}

technique Technique1
{
    pass Concussion
    {
        PixelShader = compile ps_2_0 Concussion();
    }
}