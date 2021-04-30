//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Samples.SnapGridFlow
{
    public class SGFDemoPlayer : MonoBehaviour
    {
        private CharacterController character;

        private void Awake()
        {
            character = GetComponent<CharacterController>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
            
            }
        }

    }
}
