//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Samples
{
    public class GFNavDemoController : MonoBehaviour
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
