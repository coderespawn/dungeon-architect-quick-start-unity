//$ Copyright 2015-26, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
// Voxel terrain shading used by SG_DA_VoxelTerrain (Custom Function node)
// Side layer (walls, ceilings) is triplanar. Top layer (floors) is projected from above only, floors are mostly flat
// Mask textures follow the Unity metallic workflow: metallic in R, smoothness in A

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

// Whiteout blend of tangent space normals into a world space normal
float3 DAVoxelTriplanarNormal(float3 tnX, float3 tnY, float3 tnZ, float3 n, float3 weights)
{
    tnX = float3(tnX.xy + n.zy, abs(tnX.z) * n.x);
    tnY = float3(tnY.xy + n.xz, abs(tnY.z) * n.y);
    tnZ = float3(tnZ.xy + n.xy, abs(tnZ.z) * n.z);
    return normalize(tnX.zyx * weights.x + tnY.xzy * weights.y + tnZ.xyz * weights.z);
}

void DAVoxelTerrain_float(float3 PositionWS, float3 NormalWS, float4 VertexColor,
    UnityTexture2D TopAlbedo, UnityTexture2D TopNormal, UnityTexture2D TopMask,
    UnityTexture2D SideAlbedo, UnityTexture2D SideNormal, UnityTexture2D SideMask,
    float4 TopColor, float4 SideColor, float4 CeilingColor,
    float TopTileSize, float SideTileSize, float NormalStrength, float SmoothnessScale,
    float TopThreshold, float TopBlend, float VariationScale, float VariationStrength, float AOStrength,
    out float3 BaseColor, out float3 NormalOut, out float Metallic, out float Smoothness, out float Occlusion)
{
    float3 n = normalize(NormalWS);

    // large and small scale variation so the tiling is less obvious
    float macro = DAVoxelValueNoise(PositionWS * VariationScale);
    float detail = DAVoxelValueNoise(PositionWS * VariationScale * 6.0);
    float variation = macro * 0.7 + detail * 0.3;

    // noise breaks up the line between floor and wall
    float slope = n.y + (variation - 0.5) * 0.3;
    float topWeight = smoothstep(TopThreshold - TopBlend, TopThreshold + TopBlend, slope);
    float ceilingWeight = smoothstep(TopThreshold - TopBlend, TopThreshold + TopBlend, -slope);

    // side layer, triplanar
    float3 weights = pow(abs(n), 4.0);
    weights /= dot(weights, 1.0);
    float2 uvX = PositionWS.zy / SideTileSize;
    float2 uvY = PositionWS.xz / SideTileSize;
    float2 uvZ = PositionWS.xy / SideTileSize;

    float3 sideAlbedo = SAMPLE_TEXTURE2D(SideAlbedo.tex, SideAlbedo.samplerstate, uvX).rgb * weights.x
                      + SAMPLE_TEXTURE2D(SideAlbedo.tex, SideAlbedo.samplerstate, uvY).rgb * weights.y
                      + SAMPLE_TEXTURE2D(SideAlbedo.tex, SideAlbedo.samplerstate, uvZ).rgb * weights.z;
    float4 sideMask = SAMPLE_TEXTURE2D(SideMask.tex, SideMask.samplerstate, uvX) * weights.x
                    + SAMPLE_TEXTURE2D(SideMask.tex, SideMask.samplerstate, uvY) * weights.y
                    + SAMPLE_TEXTURE2D(SideMask.tex, SideMask.samplerstate, uvZ) * weights.z;
    float3 tnX = UnpackNormalScale(SAMPLE_TEXTURE2D(SideNormal.tex, SideNormal.samplerstate, uvX), NormalStrength);
    float3 tnY = UnpackNormalScale(SAMPLE_TEXTURE2D(SideNormal.tex, SideNormal.samplerstate, uvY), NormalStrength);
    float3 tnZ = UnpackNormalScale(SAMPLE_TEXTURE2D(SideNormal.tex, SideNormal.samplerstate, uvZ), NormalStrength);
    float3 sideNormal = DAVoxelTriplanarNormal(tnX, tnY, tnZ, n, weights);
    float3 sideColor = sideAlbedo * lerp(SideColor.rgb, CeilingColor.rgb, ceilingWeight);

    // top layer, projected from above
    float2 uvTop = PositionWS.xz / TopTileSize;
    float3 topColor = SAMPLE_TEXTURE2D(TopAlbedo.tex, TopAlbedo.samplerstate, uvTop).rgb * TopColor.rgb;
    float4 topMask = SAMPLE_TEXTURE2D(TopMask.tex, TopMask.samplerstate, uvTop);
    float3 tnTop = UnpackNormalScale(SAMPLE_TEXTURE2D(TopNormal.tex, TopNormal.samplerstate, uvTop), NormalStrength);
    tnTop = float3(tnTop.xy + n.xz, abs(tnTop.z) * n.y);
    float3 topNormal = normalize(tnTop.xzy);

    float3 color = lerp(sideColor, topColor, topWeight);
    color *= lerp(1.0 - VariationStrength, 1.0 + VariationStrength, variation);
    float4 mask = lerp(sideMask, topMask, topWeight);

    BaseColor = saturate(color);
    NormalOut = normalize(lerp(sideNormal, topNormal, topWeight));
    Metallic = mask.r;
    Smoothness = saturate(mask.a * SmoothnessScale);
    Occlusion = lerp(1.0, VertexColor.r, AOStrength);
}

void DAVoxelTerrain_half(half3 PositionWS, half3 NormalWS, half4 VertexColor,
    UnityTexture2D TopAlbedo, UnityTexture2D TopNormal, UnityTexture2D TopMask,
    UnityTexture2D SideAlbedo, UnityTexture2D SideNormal, UnityTexture2D SideMask,
    half4 TopColor, half4 SideColor, half4 CeilingColor,
    half TopTileSize, half SideTileSize, half NormalStrength, half SmoothnessScale,
    half TopThreshold, half TopBlend, half VariationScale, half VariationStrength, half AOStrength,
    out half3 BaseColor, out half3 NormalOut, out half Metallic, out half Smoothness, out half Occlusion)
{
    float3 baseColor, normalOut;
    float metallic, smoothness, occlusion;
    DAVoxelTerrain_float(PositionWS, NormalWS, VertexColor, TopAlbedo, TopNormal, TopMask, SideAlbedo, SideNormal, SideMask,
        TopColor, SideColor, CeilingColor, TopTileSize, SideTileSize, NormalStrength, SmoothnessScale,
        TopThreshold, TopBlend, VariationScale, VariationStrength, AOStrength,
        baseColor, normalOut, metallic, smoothness, occlusion);
    BaseColor = baseColor;
    NormalOut = normalOut;
    Metallic = metallic;
    Smoothness = smoothness;
    Occlusion = occlusion;
}

#endif
