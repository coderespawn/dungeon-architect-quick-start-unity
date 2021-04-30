//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;
using DungeonArchitect.Graphs;


namespace DungeonArchitect.Samples.ShooterGame
{
    public class MiniMapGenerator : MonoBehaviour
    {
        public List<Graph> miniMapThemes;
        GameObject miniMapDungeonObject;
        Dungeon minimapDungeon;

        // Use this for initialization
        public void BuildMiniMap(Dungeon baseDungeon)
        {
            if (miniMapDungeonObject == null)
            {
                miniMapDungeonObject = Instantiate(baseDungeon.gameObject);
            }

            // Move the mini-map dungeon down
            minimapDungeon = miniMapDungeonObject.GetComponent<Dungeon>();
            minimapDungeon.transform.position = gameObject.transform.position;

            // Disable unwanted components from the cloned minimap dungeon
            DisableComponent<WaypointGenerator>(miniMapDungeonObject);
            DisableComponent<LevelNpcSpawner>(miniMapDungeonObject);
            DisableComponent<SpecialRoomFinder>(miniMapDungeonObject);
            DisableComponent<MiniMapRebuilder>(miniMapDungeonObject);

            // Apply the mini-map themes and rebuild
            minimapDungeon.dungeonThemes = miniMapThemes;
            minimapDungeon.Config.Seed = baseDungeon.Config.Seed;
            minimapDungeon.Build();
        }

        public void DestroyDungeon()
        {
            if (minimapDungeon != null)
            {
                minimapDungeon.DestroyDungeon();
            }

        }

        void DisableComponent<T>(GameObject obj) where T : MonoBehaviour
        {
            var component = obj.GetComponent<T>();
            if (component != null)
            {
                component.enabled = false;
            }
        }
    }
}
