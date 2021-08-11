using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Samples.Snap
{
    public class SnapCustomMarker : MonoBehaviour
    {
        public string markerName = "MyMarker";
        private GUIStyle textStyle;
        public bool hideGizmoVisuals = false;

        private void OnDrawGizmos()
        {
            DrawGizmo(false);
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmo(true);
        }

        void DrawGizmo(bool selected)
        {
            if (hideGizmoVisuals) return;
            
            var t = transform;
            var position = t.position;

            Gizmos.color = selected ? Color.red : Color.blue;
            Gizmos.DrawSphere(position, 0.2f);

            if (textStyle == null)
            {
                textStyle = new GUIStyle(GUI.skin.label);
                textStyle.normal.textColor = Color.red;
            }
            Handles.Label(position + new Vector3(0, 1.0f, 0), markerName, textStyle);
            Handles.color = Color.red;
            Handles.ArrowHandleCap(0, position, t.rotation, 1.1f, EventType.Repaint);
        }
    }
}
