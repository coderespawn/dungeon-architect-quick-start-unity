//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEngine.AI;
using DungeonArchitect.Builders.Grid;

namespace DungeonArchitect.Samples.Navigation
{
    public class NavigationDemoController : MonoBehaviour
    {
        public Dungeon dungeon;
        public GameObject player;
        public GameObject[] npcs;
        public Vector3 spawnOffset;

        // Use this for initialization
        void Awake()
        {
            dungeon.Config.Seed = (uint)Random.Range(0, 100000);
            dungeon.Build();

            TeleportToValidPlatform(player);
            foreach (var npc in npcs)
            {
                TeleportToValidPlatform(npc);
            }
        }

        void TeleportToValidPlatform(GameObject obj)
        {
            if (obj == null) return;

            var gridModel = dungeon.ActiveModel as GridDungeonModel;
            var gridConfig = dungeon.Config as GridDungeonConfig;
            int numCells = gridModel.Cells.Count;
            if (numCells > 0)
            {
                int randomCellIndex = Random.Range(0, numCells);
                var cell = gridModel.Cells[randomCellIndex];
                var cellCenter = Vector3.Scale(cell.Bounds.CenterF(), gridConfig.GridCellSize);
                var tilePosition = cellCenter + spawnOffset;
                var hit = new NavMeshHit();
                if (NavMesh.SamplePosition(tilePosition, out hit, 1.0f, NavMesh.AllAreas))
                {
                    obj.transform.position = hit.position;
                }
                else
                {
                    Debug.LogError("Failed to place " + obj.name + " in nav mesh");
                }

            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
