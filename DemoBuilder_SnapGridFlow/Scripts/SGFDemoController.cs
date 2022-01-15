//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using DungeonArchitect;
using UnityEngine;

namespace DungeonArchitect.Samples.SnapGridFlow
{
    public class SGFDemoController : MonoBehaviour
    {
        public Dungeon dungeon;

        protected virtual void Start()
        {
            if (dungeon != null)
            {
                dungeon.Build();
                
                // Setup the visibility graph to track the player
                var playerObject = GameObject.FindWithTag("Player");
                if (playerObject != null)
                {
                    // Detach the spawned player object from the room prefab
                    playerObject.transform.SetParent(null, true);
                }
            }
        }

    }
}
