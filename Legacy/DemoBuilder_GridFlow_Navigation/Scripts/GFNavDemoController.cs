//$ Copyright 2015-25, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
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