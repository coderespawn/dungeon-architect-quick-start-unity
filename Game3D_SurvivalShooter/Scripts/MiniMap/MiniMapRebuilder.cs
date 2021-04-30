//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
namespace DungeonArchitect.Samples.ShooterGame
{
    public class MiniMapRebuilder : DungeonEventListener
    {
        public MiniMapGenerator miniMap;

        /// <summary>
        /// Called after the dungeon is completely built
        /// </summary>
        /// <param name="model">The dungeon model</param>
        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            miniMap.BuildMiniMap(dungeon);
        }

        /// <summary>
        /// Called after the dungeon is destroyed
        /// </summary>
        /// <param name="model">The dungeon model</param>
        public override void OnDungeonDestroyed(Dungeon dungeon)
        {
            miniMap.DestroyDungeon();
        }
    }
}
