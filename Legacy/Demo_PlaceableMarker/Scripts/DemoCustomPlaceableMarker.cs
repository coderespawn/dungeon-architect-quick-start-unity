//$ Copyright 2015-25, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

namespace DungeonArchitect.Samples
{
    public class DemoCustomPlaceableMarker : MonoBehaviour
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