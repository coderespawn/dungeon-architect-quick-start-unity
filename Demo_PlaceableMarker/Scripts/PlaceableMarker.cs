﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Samples
{
    public class PlaceableMarker : MonoBehaviour
    {
        public string markerName = "MyMarker";

        void OnDrawGizmosSelected()
        {
            DrawGizmo(true);
        }

        void OnDrawGizmos()
        {
            DrawGizmo(false);
        }

        void DrawGizmo(bool selected)
        {
            // Draw the wireframe
            Gizmos.color = selected ? Color.red : Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }
}
