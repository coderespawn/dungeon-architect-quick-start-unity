//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect;
using UnityEngine;

namespace DungeonArchitect.Samples.SnapGridFlow
{
    public class SGFDemoController : MonoBehaviour
    {
        public Dungeon dungeon;

        void Start()
        {
            if (dungeon != null)
            {
                dungeon.Build();
            }
        }

    }
}
