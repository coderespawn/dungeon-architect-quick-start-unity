//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Builders.GridFlow;
using UnityEngine;


namespace DungeonArchitect.Samples.GridFlow
{
    public class GridFlowGameController : MonoBehaviour
    {
        public Dungeon dungeon;
        // Start is called before the first frame update
        void Start()
        {
            // Randomize and build the dungeon
            if (dungeon != null)
            {
                var dungeonConfig = dungeon.GetComponent<GridFlowDungeonConfig>();
                var random = new System.Random();
                dungeonConfig.Seed = (uint)random.Next();
                dungeon.Build();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
