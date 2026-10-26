//$ Copyright 2015-26, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
// Simple vegetation shading used by SG_DA_Vegetation (Custom Function node)
// Vertex color: R = root (0) to tip (1), A = emission mask

#ifndef DA_VEGETATION_INCLUDED
#define DA_VEGETATION_INCLUDED

float DAVegetationHash(float3 p)
{
    p = frac(p * 0.3183099 + 0.1);
    p *= 17.0;
    return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
}

void DAVegetation_float(float3 PositionWS, float3 NormalWS, float4 VertexColor,
    float4 RootColor, float4 TipColor, float4 EmissionColor,
    float ColorVariation, float Smoothness, float RootDarkness,
    out float3 BaseColor, out float3 Emission, out float SmoothnessOut, out float Occlusion)
{
    float gradient = VertexColor.r;

    // per instance variation from the object position, works with GPU instancing
    float3 origin = UNITY_MATRIX_M._m03_m13_m23;
    float h = DAVegetationHash(floor(origin * 3.0) + 0.5);
    float h2 = DAVegetationHash(floor(origin * 3.0) + 17.3);

    float3 tip = TipColor.rgb * lerp(1.0 - ColorVariation, 1.0 + ColorVariation, h);
    tip = lerp(tip, tip.gbr * 0.6 + tip * 0.4, ColorVariation * h2 * 0.5);
    float3 color = lerp(RootColor.rgb, tip, gradient);

    float rootShade = lerp(1.0 - RootDarkness, 1.0, gradient);
    BaseColor = saturate(color * rootShade);
    Emission = EmissionColor.rgb * VertexColor.a;
    SmoothnessOut = Smoothness;
    Occlusion = rootShade;
}

void DAVegetation_half(half3 PositionWS, half3 NormalWS, half4 VertexColor,
    half4 RootColor, half4 TipColor, half4 EmissionColor,
    half ColorVariation, half Smoothness, half RootDarkness,
    out half3 BaseColor, out half3 Emission, out half SmoothnessOut, out half Occlusion)
{
    float3 baseColor, emission;
    float smoothness, occlusion;
    DAVegetation_float(PositionWS, NormalWS, VertexColor, RootColor, TipColor, EmissionColor,
        ColorVariation, Smoothness, RootDarkness, baseColor, emission, smoothness, occlusion);
    BaseColor = baseColor;
    Emission = emission;
    SmoothnessOut = smoothness;
    Occlusion = occlusion;
}

#endif
