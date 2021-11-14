//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect;
using DungeonArchitect.Builders.SnapGridFlow;
using UnityEngine;

namespace DungeonArchitect.Samples.SnapGridFlow
{
    public class SGFDemoController_VisiblityGraph : MonoBehaviour
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
                    var visibilityGraph = dungeon.GetComponent<SnapGridFlowVisibilityGraph>();
                    if (visibilityGraph != null)
                    {
                        visibilityGraph.trackedObjects = new Transform[] { playerObject.transform };
                    }
                }
            }
        }

    }
}
