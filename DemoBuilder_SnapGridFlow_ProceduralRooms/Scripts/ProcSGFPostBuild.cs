using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Samples.SGF.ProceduralRooms
{
    public class ProcSGFPostBuild : DungeonEventListener
    {
        public override void OnPostDungeonBuild(Dungeon dungeon, DungeonModel model)
        {
            base.OnPostDungeonBuild(dungeon, model);
            
            uint masterSeed = 0;
            if (dungeon != null && dungeon.Config != null)
            {
                masterSeed = dungeon.Config.Seed;
            }

            // Grab all the SGF room chunks and build their nested dungeons
            var moduleScripts = GameObject.FindObjectsByType<ProcSgfRoomModule>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var moduleScript in moduleScripts)
            {
                moduleScript.masterSeed = (int)masterSeed;
                moduleScript.BuildRoom((int)dungeon.Config.Seed, null);
            }
        }
    }
}
