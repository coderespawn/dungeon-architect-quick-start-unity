using System.Collections;
using System.Collections.Generic;
using DungeonArchitect.Builders.SnapGridFlow;
using DungeonArchitect.Flow.Domains.Layout.Tooling.Graph3D;
using DungeonArchitect.Samples.ShooterGame;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Samples.SnapGridFlow
{
    public enum SGFHideFloorMode
    {
        HideAboveAndBelow,
        HideAbove,
        HideBelow,
        None,
    }
    
    public class SGFHideOtherFloors : DungeonEventListener
    {
        public SGFHideFloorMode hideMode = SGFHideFloorMode.HideAboveAndBelow;
        
        [Tooltip("offset to apply on the distance check from the player's Y position, when purging rooms above the player")]
        public float distanceThresholdBelow = 0;
        
        [Tooltip("offset to apply on the distance check from the player's Y position, when purging rooms above the player")]
        public float distanceThresholdAbove = 0;

        [Tooltip("How often should the test be run (in seconds)")]
        public float updateFrequency = 0.2f;

        
        [Tooltip("Set this to true if you want to automatically find the player object after the dungeon is built. Make sure you player prefab is tagged as 'Player'")]
        public bool autoFindPlayerObject = true;
        
        [Tooltip("The game object to track.  Leave this blank if you have set the above flag to true")]
        public Transform playerObject;
        
        void Start()
        {
            InvokeRepeating("HideFloors", 0, updateFrequency);
        }

        void HideFloors()
        {
            if (playerObject == null) return;
            float playerY = playerObject.position.y;

            var model = GetComponent<SnapGridFlowModel>();
            foreach (var moduleNode in model.snapModules)
            {
                // Grab the world bounds of the module
                var localBounds = moduleNode.GetModuleBounds();
                var worldBounds = MathUtils.TransformBounds(moduleNode.WorldTransform, localBounds);
                
                // Test if we need to hide it
                float moduleLowest = worldBounds.center.y - worldBounds.extents.y;
                float moduleHighest = worldBounds.center.y + worldBounds.extents.y;

                bool belowPlayer = playerY > moduleHighest;
                bool abovePlayer = playerY < moduleLowest;

                bool shouldHide = false;
                if (hideMode == SGFHideFloorMode.HideAboveAndBelow)
                {
                    shouldHide = belowPlayer || abovePlayer;
                } 
                else if (hideMode == SGFHideFloorMode.HideAbove)
                {
                    shouldHide = abovePlayer;
                } 
                else if (hideMode == SGFHideFloorMode.HideAbove)
                {
                    shouldHide = belowPlayer;
                }
                else if (hideMode == SGFHideFloorMode.None)
                {
                    shouldHide = false;
                }

                moduleNode.SpawnedModule.gameObject.SetActive(!shouldHide);
            }
        }

        
        /// <summary>
        /// Find the player object, if we need automatic detection
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="model"></param>
        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            if (autoFindPlayerObject)
            {
                playerObject = null;
                var player = GameObject.FindWithTag(GameTags.Player);
                if (player != null)
                {
                    playerObject = player.transform;
                }
                else
                {
                    Debug.LogError("Hide Floor Script: No Player object found. Make sure your player prefab is tagged as 'Player'");
                }
            }
        }

        /// <summary>
        /// Clear the player object if we have automatic detection
        /// </summary>
        /// <param name="dungeon"></param>
        public override void OnDungeonDestroyed(Dungeon dungeon)
        {
            if (autoFindPlayerObject)
            {
                playerObject = null;
            }
        }
    }
}

