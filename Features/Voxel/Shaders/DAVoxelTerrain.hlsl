//$ Copyright 2015-26, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
// Voxel terrain shading used by SG_DA_VoxelTerrain (Custom Function node)

#ifndef DA_VOXEL_TERRAIN_INCLUDED
#define DA_VOXEL_TERRAIN_INCLUDED

float DAVoxelHash(float3 p)
{
    p = frac(p * 0.3183099 + 0.1);
    p *= 17.0;
    return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
}

// value noise, 0..1
float DAVoxelValueNoise(float3 x)
{
    float3 i = floor(x);
    float3 f = frac(x);
    f = f * f * (3.0 - 2.0 * f);
    return lerp(lerp(lerp(DAVoxelHash(i + float3(0, 0, 0)), DAVoxelHash(i + float3(1, 0, 0)), f.x),
                     lerp(DAVoxelHash(i + float3(0, 1, 0)), DAVoxelHash(i + float3(1, 1, 0)), f.x), f.y),
                lerp(lerp(DAVoxelHash(i + float3(0, 0, 1)), DAVoxelHash(i + float3(1, 0, 1)), f.x),
                     lerp(DAVoxelHash(i + float3(0, 1, 1)), DAVoxelHash(i + float3(1, 1, 1)), f.x), f.y), f.z);
}

void DAVoxelTerrain_float(float3 PositionWS, float3 NormalWS, float4 VertexColor,
    float4 TopColor, float4 SideColor, float4 CeilingColor,
    float TopThreshold, float TopBlend, float VariationScale, float VariationStrength, float AOStrength,
    out float3 BaseColor, out float Occlusion)
{
    float3 n = normalize(NormalWS);

    // large and small scale variation so flat colors don't look tiled
    float macro = DAVoxelValueNoise(PositionWS * VariationScale);
    float detail = DAVoxelValueNoise(PositionWS * VariationScale * 6.0);
    float variation = macro * 0.7 + detail * 0.3;

    // noise breaks up the line between floor and wall
    float slope = n.y + (variation - 0.5) * 0.3;
    float topWeight = smoothstep(TopThreshold - TopBlend, TopThreshold + TopBlend, slope);
    float ceilingWeight = smoothstep(TopThreshold - TopBlend, TopThreshold + TopBlend, -slope);

    float3 color = SideColor.rgb;
    color = lerp(color, CeilingColor.rgb, ceilingWeight);
    color = lerp(color, TopColor.rgb, topWeight);
    color *= lerp(1.0 - VariationStrength, 1.0 + VariationStrength, variation);

    BaseColor = saturate(color);
    Occlusion = lerp(1.0, VertexColor.r, AOStrength);
}

void DAVoxelTerrain_half(half3 PositionWS, half3 NormalWS, half4 VertexColor,
    half4 TopColor, half4 SideColor, half4 CeilingColor,
    half TopThreshold, half TopBlend, half VariationScale, half VariationStrength, half AOStrength,
    out half3 BaseColor, out half Occlusion)
{
    float3 baseColor;
    float occlusion;
    DAVoxelTerrain_float(PositionWS, NormalWS, VertexColor, TopColor, SideColor, CeilingColor,
        TopThreshold, TopBlend, VariationScale, VariationStrength, AOStrength, baseColor, occlusion);
    BaseColor = baseColor;
    Occlusion = occlusion;
}

#endif
