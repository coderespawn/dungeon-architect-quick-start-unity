//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace DungeonArchitect.Editors.DevTools
{
    public class SceneLightmapBaker
    {
        static void SetupLightSettings()
        {
            Lightmapping.realtimeGI = false;
            Lightmapping.bakedGI = false;
            Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
        }


        //[MenuItem("Dungeon Architect/Internal Dev Tools/Lightmap/Clear Current", priority = 2001)]
        public static void ClearCurrentScene()
        {
            Lightmapping.Clear();
            Lightmapping.ClearLightingDataAsset();
        }

        //[MenuItem("Dungeon Architect/Internal Dev Tools/Lightmap/Bake Current", priority = 2002)]
        public static void BakeCurrentScene()
        {
            SetupLightSettings();
            ClearCurrentScene();
            Lightmapping.Bake();
        }

        [MenuItem("Window/Rendering/Dungeon Architect/Internal Dev Tools/Clear Lighting on All Samples", priority = 2021)]
        public static void ClearAllScenes()
        {
            bool proceed = EditorUtility.DisplayDialog("Build Lighting", "Are you sure you want to clear lighting data on all the scenes?", "Yes", "No");
            if (proceed)
            {
                OpenAllScenes(() => ClearCurrentScene(), false);
            }
        }


        [MenuItem("Window/Rendering/Dungeon Architect/Internal Dev Tools/Bake Lighting on All Samples", priority = 2021)]
        public static void BakeAllScenes()
        {
            bool build = EditorUtility.DisplayDialog("Build Lighting", "Are you sure you want to build lighting on all the scenes?", "Yes", "No");
            if (build)
            {
                OpenAllScenes(() => BakeCurrentScene(), true);
            }
        }

        static void OpenAllScenes(System.Action action, bool saveAfterAction)
        {
            if (action == null)
            {
                return;
            }

            var assetPaths = AssetDatabase.GetAllAssetPaths();
            var scenePaths = new List<string>();
            foreach (var assetPath in assetPaths)
            {
                if (assetPath.EndsWith(".unity") && assetPath.StartsWith("Assets/CodeRespawn"))
                {
                    if (!assetPath.Contains("DungeonArchitect_LaunchPad"))
                    {
                        scenePaths.Add(assetPath);
                    }
                }
            }
            foreach (var scenePath in scenePaths)
            {
                EditorSceneManager.OpenScene(scenePath);
                action.Invoke();

                if (saveAfterAction)
                {
                    EditorSceneManager.MarkAllScenesDirty();
                    EditorSceneManager.SaveOpenScenes();
                }
            }
        }
    }
}
