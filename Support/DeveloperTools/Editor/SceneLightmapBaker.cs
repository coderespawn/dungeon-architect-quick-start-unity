using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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


        [MenuItem("Dungeon Architect/Internal Dev Tools/Lightmap/Clear Current", priority = 2001)]
        public static void ClearCurrentScene()
        {
            Lightmapping.Clear();
            Lightmapping.ClearLightingDataAsset();
        }

        [MenuItem("Dungeon Architect/Internal Dev Tools/Lightmap/Bake Current", priority = 2002)]
        public static void BakeCurrentScene()
        {
            SetupLightSettings();
            ClearCurrentScene();
            Lightmapping.Bake();
        }

        [MenuItem("Dungeon Architect/Internal Dev Tools/Lightmap/Clear All", priority = 2021)]
        public static void ClearAllScenes()
        {
            OpenAllScenes(() => ClearCurrentScene(), false);
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
                if (assetPath.EndsWith(".unity") && assetPath.StartsWith("Assets/DungeonArchitect"))
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
