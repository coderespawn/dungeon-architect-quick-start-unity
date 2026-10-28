//$ Copyright 2015-26, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace DungeonArchitect.Samples.Voxel.Editors
{
/// <summary>
/// Generates the procedural scatter art of the cave sample: grass clumps, boulders, stalagmites, mushrooms and hanging moss.
/// Overwrites the meshes, materials and prefabs under Features/Voxel/ScatterArt
/// </summary>
public static class DAScatterArtGenerator
{
    const string Root = "Assets/CodeRespawn/DungeonArchitect_Samples/Features/Voxel/ScatterArt";
    const string Shaders = "Assets/CodeRespawn/DungeonArchitect_Samples/Features/Voxel/Shaders/";
    const string CaveMaterial = "Assets/CodeRespawn/DungeonArchitect_Samples/Features/Voxel/Materials/M_DA_Voxel_Cave.mat";

    class MeshBuilder
    {
        public List<Vector3> V = new List<Vector3>();
        public List<Vector3> N = new List<Vector3>();
        public List<Color> C = new List<Color>();
        public List<int> T = new List<int>();
        public int Add(Vector3 v, Vector3 n, Color c) { V.Add(v); N.Add(n); C.Add(c); return V.Count - 1; }
        public void Tri(int a, int b, int c) { T.Add(a); T.Add(b); T.Add(c); }
        public Mesh Build(string name, bool recalcNormals)
        {
            var m = new Mesh { name = name };
            if (V.Count > 65000) m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            m.SetVertices(V); m.SetColors(C); m.SetTriangles(T, 0);
            if (recalcNormals) m.RecalculateNormals(); else m.SetNormals(N);
            m.RecalculateBounds();
            m.RecalculateTangents();
            return m;
        }
    }

    static System.Random rng;
    static float R(float a, float b) => a + (float)rng.NextDouble() * (b - a);

    // smooth 3D value noise for displacement
    static float Hash(int x, int y, int z, int s)
    {
        unchecked
        {
            uint h = (uint)(x * 374761393 + y * 668265263 + z * 2147483647 + s * 1274126177);
            h = (h ^ (h >> 13)) * 1274126177u;
            return ((h ^ (h >> 16)) & 0xFFFFFF) / 16777215f;
        }
    }
    static float Noise(Vector3 p, int s)
    {
        int ix = Mathf.FloorToInt(p.x), iy = Mathf.FloorToInt(p.y), iz = Mathf.FloorToInt(p.z);
        float fx = p.x - ix, fy = p.y - iy, fz = p.z - iz;
        fx = fx * fx * (3 - 2 * fx); fy = fy * fy * (3 - 2 * fy); fz = fz * fz * (3 - 2 * fz);
        float L(int a, int b, int c) => Hash(ix + a, iy + b, iz + c, s);
        return Mathf.Lerp(Mathf.Lerp(Mathf.Lerp(L(0,0,0), L(1,0,0), fx), Mathf.Lerp(L(0,1,0), L(1,1,0), fx), fy),
                          Mathf.Lerp(Mathf.Lerp(L(0,0,1), L(1,0,1), fx), Mathf.Lerp(L(0,1,1), L(1,1,1), fx), fy), fz);
    }
    static float Fbm(Vector3 p, int s) => Noise(p, s) * 0.55f + Noise(p * 2.1f, s + 1) * 0.3f + Noise(p * 4.3f, s + 2) * 0.15f;

    // ---- grass: a clump of curved tapered blades
    static Mesh GrassClump(int seed, int blades)
    {
        rng = new System.Random(seed);
        var mb = new MeshBuilder();
        const int segments = 4;
        for (var b = 0; b < blades; b++)
        {
            var angle = R(0, Mathf.PI * 2);
            var dist = R(0, 0.14f);
            var basePos = new Vector3(Mathf.Cos(angle) * dist, 0, Mathf.Sin(angle) * dist);
            var yaw = R(0, 360);
            var height = R(0.28f, 0.62f);
            var width = R(0.03f, 0.05f);
            var lean = R(0.1f, 0.45f);
            var leanDir = Quaternion.Euler(0, R(0, 360), 0) * Vector3.forward;
            var side = Quaternion.Euler(0, yaw, 0) * Vector3.right;
            var faceNormal = Vector3.Cross(side, Vector3.up).normalized;
            var normal = (faceNormal * 0.25f + Vector3.up).normalized;
            var start = mb.V.Count;
            for (var s = 0; s <= segments; s++)
            {
                var t = s / (float)segments;
                var center = basePos + Vector3.up * (height * t) + leanDir * (lean * height * t * t);
                var w = width * (1 - t * 0.9f);
                var col = new Color(t, 0, 0, 0);
                mb.Add(center - side * w * 0.5f, normal, col);
                mb.Add(center + side * w * 0.5f, normal, col);
            }
            for (var s = 0; s < segments; s++)
            {
                var i = start + s * 2;
                mb.Tri(i, i + 2, i + 1);
                mb.Tri(i + 1, i + 2, i + 3);
            }
        }
        return mb.Build("SM_DA_Grass_" + seed, false);
    }

    // ---- icosphere helper
    static void Icosphere(int subdivisions, out List<Vector3> verts, out List<int> tris)
    {
        var t = (1 + Mathf.Sqrt(5)) / 2;
        verts = new List<Vector3> {
            new Vector3(-1, t, 0), new Vector3(1, t, 0), new Vector3(-1, -t, 0), new Vector3(1, -t, 0),
            new Vector3(0, -1, t), new Vector3(0, 1, t), new Vector3(0, -1, -t), new Vector3(0, 1, -t),
            new Vector3(t, 0, -1), new Vector3(t, 0, 1), new Vector3(-t, 0, -1), new Vector3(-t, 0, 1) };
        for (var i = 0; i < verts.Count; i++) verts[i] = verts[i].normalized;
        tris = new List<int> { 0,11,5, 0,5,1, 0,1,7, 0,7,10, 0,10,11, 1,5,9, 5,11,4, 11,10,2, 10,7,6, 7,1,8,
                               3,9,4, 3,4,2, 3,2,6, 3,6,8, 3,8,9, 4,9,5, 2,4,11, 6,2,10, 8,6,7, 9,8,1 };
        for (var s = 0; s < subdivisions; s++)
        {
            var cache = new Dictionary<long, int>();
            var vs = verts;
            int Mid(int a, int b)
            {
                long key = a < b ? ((long)a << 32) | (uint)b : ((long)b << 32) | (uint)a;
                if (cache.TryGetValue(key, out var idx)) return idx;
                vs.Add(((vs[a] + vs[b]) * 0.5f).normalized);
                cache[key] = vs.Count - 1;
                return vs.Count - 1;
            }
            var next = new List<int>();
            for (var i = 0; i < tris.Count; i += 3)
            {
                int a = tris[i], b = tris[i + 1], c = tris[i + 2];
                int ab = Mid(a, b), bc = Mid(b, c), ca = Mid(c, a);
                next.AddRange(new[] { a, ab, ca, b, bc, ab, c, ca, bc, ab, bc, ca });
            }
            tris = next;
        }
    }

    // ---- boulder: displaced, flattened icosphere, about 1 unit across, pivot at the ground
    static Mesh Boulder(int seed)
    {
        rng = new System.Random(seed);
        Icosphere(3, out var verts, out var tris);
        var stretch = new Vector3(R(0.9f, 1.2f), R(0.55f, 0.8f), R(0.8f, 1.1f));
        var offset = new Vector3(R(0, 100), R(0, 100), R(0, 100));
        var mb = new MeshBuilder();
        foreach (var d in verts)
        {
            var r = 0.5f * (0.8f + 0.45f * Fbm(d * 1.6f + offset, seed) + 0.12f * Noise(d * 7 + offset, seed + 9));
            var p = Vector3.Scale(d * r, stretch);
            if (p.y < -0.05f) p.y = -0.05f + (p.y + 0.05f) * 0.35f;   // flat-ish bottom
            mb.Add(p, Vector3.zero, Color.white);
        }
        // pivot on the ground, sink a little
        var minY = float.MaxValue;
        foreach (var v in mb.V) minY = Mathf.Min(minY, v.y);
        for (var i = 0; i < mb.V.Count; i++)
        {
            var v = mb.V[i]; v.y -= minY + 0.08f; mb.V[i] = v;
            // darker towards the ground, the terrain shader reads it as AO
            mb.C[i] = new Color(Mathf.Clamp01(0.45f + v.y * 1.2f), 1, 1, 1);
        }
        mb.T.AddRange(tris);
        return mb.Build("SM_DA_Boulder_" + seed, true);
    }

    // ---- stalagmite: a bumpy cone with rings, base at the pivot, about 1 unit tall
    static Mesh Spike(int seed)
    {
        rng = new System.Random(seed);
        const int rings = 16, sides = 12;
        var height = 1.0f;
        var baseRadius = R(0.26f, 0.36f);
        var bend = new Vector3(R(-0.12f, 0.12f), 0, R(-0.12f, 0.12f));
        var offset = new Vector3(R(0, 100), R(0, 100), R(0, 100));
        var mb = new MeshBuilder();
        for (var r = 0; r <= rings; r++)
        {
            var t = r / (float)rings;
            var y = -0.15f + t * (height + 0.15f);
            var radius = baseRadius * Mathf.Pow(1 - t, 1.25f) * (1 + 0.12f * Mathf.Sin(t * 38 + seed));
            var center = bend * (t * t);
            for (var s = 0; s < sides; s++)
            {
                var a = s / (float)sides * Mathf.PI * 2;
                var dir = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a));
                var bump = 1 + 0.25f * (Fbm(dir * 2 + new Vector3(0, y * 3, 0) + offset, seed) - 0.5f);
                mb.Add(center + dir * radius * bump + Vector3.up * y, Vector3.zero, new Color(Mathf.Lerp(0.55f, 1, t), 1, 1, 1));
            }
        }
        for (var r = 0; r < rings; r++)
        {
            for (var s = 0; s < sides; s++)
            {
                int a = r * sides + s, b = r * sides + (s + 1) % sides, c = a + sides, d = b + sides;
                mb.Tri(a, c, b); mb.Tri(b, c, d);
            }
        }
        return mb.Build("SM_DA_Stalagmite_" + seed, true);
    }

    // ---- mushrooms: a small cluster, R = 0 stem / 1 cap, A = 1 on the cap (emission mask)
    static Mesh Mushrooms(int seed)
    {
        rng = new System.Random(seed);
        var mb = new MeshBuilder();
        var count = rng.Next(3, 6);
        for (var m = 0; m < count; m++)
        {
            var angle = R(0, Mathf.PI * 2);
            var dist = m == 0 ? 0 : R(0.05f, 0.14f);
            var basePos = new Vector3(Mathf.Cos(angle) * dist, 0, Mathf.Sin(angle) * dist);
            var size = m == 0 ? 1 : R(0.45f, 0.85f);
            var stemHeight = R(0.12f, 0.24f) * size;
            var stemRadius = 0.018f * size;
            var capRadius = R(0.05f, 0.08f) * size;
            var tilt = new Vector3(R(-0.25f, 0.25f), 1, R(-0.25f, 0.25f)).normalized;
            const int sides = 10;

            // stem
            var stemStart = mb.V.Count;
            for (var r = 0; r <= 3; r++)
            {
                var t = r / 3f;
                var c = basePos + tilt * (stemHeight * t);
                for (var s = 0; s < sides; s++)
                {
                    var a = s / (float)sides * Mathf.PI * 2;
                    var dir = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a));
                    mb.Add(c + dir * stemRadius * (1.2f - 0.3f * t), dir, new Color(0, 0, 0, 0));
                }
            }
            for (var r = 0; r < 3; r++)
                for (var s = 0; s < sides; s++)
                {
                    int a = stemStart + r * sides + s, b = stemStart + r * sides + (s + 1) % sides, c = a + sides, d = b + sides;
                    mb.Tri(a, c, b); mb.Tri(b, c, d);
                }

            // cap: squashed dome plus a flat underside
            var top = basePos + tilt * stemHeight;
            const int capRings = 5;
            var capStart = mb.V.Count;
            for (var r = 0; r <= capRings; r++)
            {
                var phi = r / (float)capRings * Mathf.PI * 0.5f;
                for (var s = 0; s < sides; s++)
                {
                    var a = s / (float)sides * Mathf.PI * 2;
                    var dir = new Vector3(Mathf.Cos(a) * Mathf.Cos(phi), Mathf.Sin(phi), Mathf.Sin(a) * Mathf.Cos(phi));
                    var p = top + new Vector3(dir.x * capRadius, dir.y * capRadius * 0.65f, dir.z * capRadius);
                    mb.Add(p, dir, new Color(1, 0, 0, 1));
                }
            }
            for (var r = 0; r < capRings; r++)
                for (var s = 0; s < sides; s++)
                {
                    int a = capStart + r * sides + s, b = capStart + r * sides + (s + 1) % sides, c = a + sides, d = b + sides;
                    mb.Tri(a, b, c); mb.Tri(b, d, c);
                }
            var under = mb.Add(top - Vector3.up * 0.005f, Vector3.down, new Color(0.3f, 0, 0, 0.3f));
            for (var s = 0; s < sides; s++)
            {
                mb.Tri(under, capStart + s, capStart + (s + 1) % sides);
            }
        }
        return mb.Build("SM_DA_Mushrooms_" + seed, false);
    }

    // ---- hanging moss: thin strands hanging down from the pivot, R = 0 at the top, 1 at the tip
    static Mesh Moss(int seed)
    {
        rng = new System.Random(seed);
        var mb = new MeshBuilder();
        var strands = 10;
        for (var k = 0; k < strands; k++)
        {
            var basePos = new Vector3(R(-0.25f, 0.25f), 0, R(-0.25f, 0.25f));
            var length = R(0.5f, 1.4f);
            var width = R(0.03f, 0.06f);
            var side = Quaternion.Euler(0, R(0, 360), 0) * Vector3.right;
            var normal = Vector3.Cross(side, Vector3.up).normalized;
            const int segments = 6;
            var start = mb.V.Count;
            for (var s = 0; s <= segments; s++)
            {
                var t = s / (float)segments;
                var sway = new Vector3(Mathf.Sin(t * 5 + k) * 0.04f, 0, Mathf.Cos(t * 4 + k) * 0.04f);
                var c = basePos + Vector3.down * (length * t) + sway;
                var w = width * (1 - t * 0.7f);
                mb.Add(c - side * w * 0.5f, normal, new Color(t, 0, 0, 0));
                mb.Add(c + side * w * 0.5f, normal, new Color(t, 0, 0, 0));
            }
            for (var s = 0; s < segments; s++)
            {
                var i = start + s * 2;
                mb.Tri(i, i + 1, i + 2);
                mb.Tri(i + 1, i + 3, i + 2);
            }
        }
        return mb.Build("SM_DA_HangingMoss_" + seed, false);
    }

    static Material VegetationMaterial(string name, Color root, Color tip, Color emission, float variation, float rootDarkness)
    {
        var path = $"{Root}/Materials/{name}.mat";
        var shader = AssetDatabase.LoadAssetAtPath<Shader>(Shaders + "SG_DA_Vegetation.shadergraph");
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null) { mat = new Material(shader); AssetDatabase.CreateAsset(mat, path); }
        mat.shader = shader;
        mat.SetColor("_RootColor", root);
        mat.SetColor("_TipColor", tip);
        mat.SetColor("_EmissionColor", emission);
        mat.SetFloat("_ColorVariation", variation);
        mat.SetFloat("_RootDarkness", rootDarkness);
        mat.SetFloat("_Smoothness", 0.15f);
        mat.enableInstancing = true;
        mat.doubleSidedGI = true;
        if (emission.maxColorComponent > 0) mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        EditorUtility.SetDirty(mat);
        return mat;
    }

    static Material RockOnlyMaterial()
    {
        var path = $"{Root}/Materials/M_DA_Scatter_Rock.mat";
        var cave = AssetDatabase.LoadAssetAtPath<Material>(CaveMaterial);
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null) { mat = new Material(cave); AssetDatabase.CreateAsset(mat, path); }
        mat.CopyPropertiesFromMaterial(cave);
        mat.SetTexture("_TopAlbedo", cave.GetTexture("_SideAlbedo"));
        mat.SetTexture("_TopNormal", cave.GetTexture("_SideNormal"));
        mat.SetTexture("_TopMask", cave.GetTexture("_SideMask"));
        mat.SetColor("_TopColor", Color.white);
        mat.SetFloat("_TopTileSize", cave.GetFloat("_SideTileSize"));
        mat.SetFloat("_TopSmoothness", 1);
        mat.enableInstancing = true;
        EditorUtility.SetDirty(mat);
        return mat;
    }

    static GameObject Prefab(string name, Mesh mesh, Material material, bool collider)
    {
        var meshPath = $"{Root}/Meshes/{mesh.name}.asset";
        var existing = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
        if (existing != null) { EditorUtility.CopySerialized(mesh, existing); mesh = existing; }
        else AssetDatabase.CreateAsset(mesh, meshPath);

        var go = new GameObject(name);
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>().sharedMaterial = material;
        if (collider) go.AddComponent<MeshCollider>().sharedMesh = mesh;
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, $"{Root}/Prefabs/{name}.prefab");
        Object.DestroyImmediate(go);
        return prefab;
    }

    static void Folder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + name)) AssetDatabase.CreateFolder(parent, name);
    }

    public static string Run()
    {
        var sb = new StringBuilder();
        Folder("Assets/CodeRespawn/DungeonArchitect_Samples/Features/Voxel", "ScatterArt");
        Folder(Root, "Meshes"); Folder(Root, "Materials"); Folder(Root, "Prefabs");
        AssetDatabase.ImportAsset(Shaders + "DAVegetation.hlsl", ImportAssetOptions.ForceUpdate);
        AssetDatabase.ImportAsset(Shaders + "SG_DA_Vegetation.shadergraph", ImportAssetOptions.ForceUpdate);
        var vegShader = AssetDatabase.LoadAssetAtPath<Shader>(Shaders + "SG_DA_Vegetation.shadergraph");
        sb.AppendLine($"vegetation shader errors: {ShaderUtil.ShaderHasError(vegShader)}");

        var grassMat = VegetationMaterial("M_DA_Scatter_Grass", new Color(0.05f, 0.09f, 0.03f), new Color(0.27f, 0.42f, 0.1f), Color.black, 0.25f, 0.5f);
        var mossMat = VegetationMaterial("M_DA_Scatter_Moss", new Color(0.05f, 0.08f, 0.04f), new Color(0.2f, 0.3f, 0.12f), Color.black, 0.2f, 0.3f);
        var glowMat = VegetationMaterial("M_DA_Scatter_Mushroom_Glow", new Color(0.7f, 0.68f, 0.6f), new Color(0.2f, 0.55f, 0.75f), new Color(0.15f, 0.7f, 1.4f), 0.15f, 0.2f);
        var redMat = VegetationMaterial("M_DA_Scatter_Mushroom_Red", new Color(0.75f, 0.7f, 0.62f), new Color(0.6f, 0.12f, 0.07f), Color.black, 0.2f, 0.2f);
        var caveMat = AssetDatabase.LoadAssetAtPath<Material>(CaveMaterial);
        var rockMat = RockOnlyMaterial();

        var names = new List<string>();
        for (var i = 0; i < 3; i++) names.Add(Prefab($"P_DA_Grass_{i}", GrassClump(100 + i, 16), grassMat, false).name);
        for (var i = 0; i < 4; i++) names.Add(Prefab($"P_DA_Boulder_{i}", Boulder(200 + i), caveMat, true).name);
        for (var i = 0; i < 3; i++) names.Add(Prefab($"P_DA_Stalagmite_{i}", Spike(300 + i), rockMat, true).name);
        for (var i = 0; i < 3; i++) names.Add(Prefab($"P_DA_Mushrooms_Glow_{i}", Mushrooms(400 + i), glowMat, false).name);
        for (var i = 0; i < 2; i++) names.Add(Prefab($"P_DA_Mushrooms_Red_{i}", Mushrooms(450 + i), redMat, false).name);
        for (var i = 0; i < 2; i++) names.Add(Prefab($"P_DA_HangingMoss_{i}", Moss(500 + i), mossMat, false).name);
        AssetDatabase.SaveAssets();
        sb.AppendLine("prefabs: " + string.Join(", ", names));
        return sb.ToString();
    }
}
}
