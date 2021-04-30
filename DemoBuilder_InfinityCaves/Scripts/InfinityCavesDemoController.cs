//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
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
