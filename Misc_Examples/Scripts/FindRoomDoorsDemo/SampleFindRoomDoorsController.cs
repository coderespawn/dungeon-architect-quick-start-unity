//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Builders.Grid;

namespace DungeonArchitect.Samples
{
    public class SampleFindRoomDoorsController : MonoBehaviour
    {
        public Dungeon dungeon;
        public Transform player;

        GridDungeonConfig config;
        GridDungeonQuery dungeonQuery;
        
        private void Awake()
        {
            if (dungeon != null)
            {
                dungeonQuery = dungeon.GetComponent<GridDungeonQuery>();
                config = dungeon.GetComponent<GridDungeonConfig>();
            }
        }

        void Start()
        {
            if (dungeon != null)
            {
                dungeon.Build();
            }

            // Place the player on a valid dungeon platform
            {
                Cell cell = dungeonQuery.GetRandomCell();
                var cellBounds = cell.GetWorldBounds(config.GridCellSize);
                player.transform.position = cellBounds.center;
            }
        }

        ///////////////////// Find the doors for the room the player is on and handle them ///////////////////// 
        void Update()
        {
            // Find the cell the player is on
            if (dungeonQuery != null)
            {
                Cell cell;
                if (dungeonQuery.GetCellAtPosition(player.position, out cell))
                {
                    GameObject[] doorObjects = new GameObject[0];
                    if (cell.CellType == CellType.Room)
                    {
                        // The player is inside a room. 
                        // Grab the spawned door game objects
                        dungeonQuery.GetDoorsForCell(cell.Id, out doorObjects);
                    }

                    // Do something with the door objects
                    ProcessDoorObjects(doorObjects);
                }
            }
        }

        ///////////////////// Custom game code to handle the door logic ///////////////////// 
        // In this example, the active doors are colored red
        void ProcessDoorObjects(GameObject[] doorObjects)
        {
            // This code finds the new objects that were tracked and paints them red
            
            // First restore the previously tracked door colors
            foreach (var trackedDoor in trackedDoorObjects)
            {
                var renderer = trackedDoor.gameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.white;
                }
            }

            // Set the color of the newly tracked doors to red
            trackedDoorObjects = doorObjects;
            foreach (GameObject trackedDoor in trackedDoorObjects)
            {
                var renderer = trackedDoor.gameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.red;
                }
            }
            
        }

        GameObject[] trackedDoorObjects = new GameObject[0];
    }
}
