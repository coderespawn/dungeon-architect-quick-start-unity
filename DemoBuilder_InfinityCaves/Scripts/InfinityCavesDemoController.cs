//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

namespace DungeonArchitect.Samples.InfinityCaves
{
    public class InfinityCavesDemoController : MonoBehaviour
    {
        public InfinityDungeon dungeon;

        void Start()
        {
            if (dungeon != null)
            {
                dungeon.BuildDungeon();
            }
        }

    }
}
