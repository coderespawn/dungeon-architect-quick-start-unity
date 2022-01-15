using System.Collections;
using System.Collections.Generic;
using DungeonArchitect;
using DungeonArchitect.Builders.SnapGridFlow;
using UnityEngine;

public class VG_GameController : MonoBehaviour
{
    public Dungeon dungeon;
    public Material redMaterial;

    private Transform playerTransform;
    private SnapGridFlowQuery query;

    void Awake()
    {
        if (dungeon != null)
        {
            query = dungeon.GetComponent<SnapGridFlowQuery>();
        }
    }
    
    void Start()
    {
        if (dungeon != null)
        {
            dungeon.Config.Seed = (uint)(Random.value * int.MaxValue);
            dungeon.Build();

            var visibilityGraph = dungeon.GetComponent<SnapGridFlowVisibilityGraph>();
            if (visibilityGraph != null)
            {
                // Use your own logic to find your player game object
                var playerObject = GameObject.FindGameObjectWithTag("Player");
                
                if (playerObject != null)
                {
                    playerTransform = playerObject.transform;
                    
                    // Detach the spawned player object from the room prefab (we don't want to hide the player's camera when the spawn room hides)
                    playerTransform.SetParent(null, true);
                
                    visibilityGraph.trackedObjects = new Transform[] { playerObject.transform };
                }
            }
        }
    }

    void Update()
    {
        // Paint the rooms red when we pass through them
        if (playerTransform != null && query != null)
        {
            var module = query.GetRoomNodeAtLocation(playerTransform.position);
            
            // Grab the bounds like this
            var roomBounds = module.GetModuleBounds();
            
            // Grab the spawned room game object like this
            var roomGameObject = module.SpawnedModule;
            
            // Set the color to red
            if (redMaterial != null)
            {
                var renderers = roomGameObject.GetComponentsInChildren<Renderer>();
                foreach (var roomRenderer in renderers)
                {
                    roomRenderer.material = redMaterial;
                }
            }
        }
    }
}
