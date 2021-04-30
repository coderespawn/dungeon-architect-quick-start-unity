//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using DungeonArchitect.Builders.Snap;

public class DoorTracker : MonoBehaviour
{
    public SnapQuery snapQuery;

    void OnDrawGizmosSelected()
    {
        var position = transform.position;
        SnapQueryModuleInfo moduleInfo;
        if (snapQuery.GetModuleInfo(position, out moduleInfo))
        {
            var bounds = moduleInfo.instanceInfo.WorldBounds;
            DrawGizmoBounds(bounds, Color.red);

            var incomingDoors = snapQuery.GetModuleIncomingDoors(position);
            foreach (var incomingDoor in incomingDoors)
            {
                var doorRenderer = incomingDoor.GetComponent<Renderer>();
                if (doorRenderer != null)
                {
                    DrawGizmoBounds(doorRenderer.bounds, Color.green);
                }
            }

            var outgoingDoors = snapQuery.GetModuleOutgoingDoors(position);
            foreach (var outgoingDoor in outgoingDoors)
            {
                var doorRenderer = outgoingDoor.GetComponent<Renderer>();
                if (doorRenderer != null)
                {
                    DrawGizmoBounds(doorRenderer.bounds, Color.cyan);
                }
            }

        }
    }

    void DrawGizmoBounds(Bounds bounds, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(bounds.center, bounds.size + new Vector3(0.1f, 0.1f, 0.1f));
    }
}
