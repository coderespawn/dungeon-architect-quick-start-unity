//$ Copyright 2015-26, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
// Simple vegetation shading used by SG_DA_Vegetation (Custom Function node)
// Which vertex color channel holds what is set on the material (0=R, 1=G, 2=B, 3=A, -1=none).
// Defaults: R = root (0) to tip (1), A = emission mask

#ifndef DA_VEGETATION_INCLUDED
#define DA_VEGETATION_INCLUDED

float DAVegetationHash(float3 p)
{
    p = frac(p * 0.3183099 + 0.1);
    p *= 17.0;
    return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
}

float DAVegetationChannel(float4 color, float channel, float fallback)
{
    if (channel < -0.5) return fallback;
    if (channel < 0.5) return color.r;
    if (channel < 1.5) return color.g;
    if (channel < 2.5) return color.b;
    return color.a;
}

void DAVegetation_float(float3 PositionWS, float3 NormalWS, float4 VertexColor,
    float4 RootColor, float4 TipColor, float4 EmissionColor,
    float ColorVariation, float Smoothness, float RootDarkness,
    float GradientChannel, float VariationChannel, float EmissionChannel,
    out float3 BaseColor, out float3 Emission, out float SmoothnessOut, out float Occlusion)
{
    float gradient = DAVegetationChannel(VertexColor, GradientChannel, 1.0);

    // per instance variation from the object position, works with GPU instancing
    float3 origin = UNITY_MATRIX_M._m03_m13_m23;
    float h = DAVegetationHash(floor(origin * 3.0) + 0.5);
    // per blade variation from the vertex color when the mesh has it
    h = frac(h + DAVegetationChannel(VertexColor, VariationChannel, 0.0));
    float h2 = DAVegetationHash(floor(origin * 3.0) + 17.3);

    float3 tip = TipColor.rgb * lerp(1.0 - ColorVariation, 1.0 + ColorVariation, h);
    tip = lerp(tip, tip.gbr * 0.6 + tip * 0.4, ColorVariation * h2 * 0.5);
    float3 color = lerp(RootColor.rgb, tip, gradient);

    float rootShade = lerp(1.0 - RootDarkness, 1.0, gradient);
    BaseColor = saturate(color * rootShade);
    Emission = EmissionColor.rgb * DAVegetationChannel(VertexColor, EmissionChannel, 1.0);
    SmoothnessOut = Smoothness;
    Occlusion = rootShade;
}

void DAVegetation_half(half3 PositionWS, half3 NormalWS, half4 VertexColor,
    half4 RootColor, half4 TipColor, half4 EmissionColor,
    half ColorVariation, half Smoothness, half RootDarkness,
    half GradientChannel, half VariationChannel, half EmissionChannel,
    out half3 BaseColor, out half3 Emission, out half SmoothnessOut, out half Occlusion)
{
    float3 baseColor, emission;
    float smoothness, occlusion;
    DAVegetation_float(PositionWS, NormalWS, VertexColor, RootColor, TipColor, EmissionColor,
        ColorVariation, Smoothness, RootDarkness, GradientChannel, VariationChannel, EmissionChannel, baseColor, emission, smoothness, occlusion);
    BaseColor = baseColor;
    Emission = emission;
    SmoothnessOut = smoothness;
    Occlusion = occlusion;
}

#endif
