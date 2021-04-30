//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DASideScroller
{
    public class SSCamera : MonoBehaviour
    {
        public Transform target;            // The position that that camera will be following.
        public float positionSmoothing = 5f;        // The speed with which the camera will be following.
        public float lookAtSmoothing = 5f;        // The speed with which the camera will be following.
        public Vector3 offset;                     // The initial offset from the target.

        Vector3 currentLookAt;

        void Start()
        {
            // Calculate the initial offset.
            //offset = transform.position - target.position;
            currentLookAt = target.position;
        }


        void Update()
        {
            // Create a postion the camera is aiming for based on the offset from the target.
            Vector3 targetCamPos = target.position + offset;

            // Smoothly interpolate between the camera's current position and it's target position.
            transform.position = Vector3.Lerp(transform.position, targetCamPos, positionSmoothing * Time.deltaTime);
            currentLookAt = Vector3.Lerp(currentLookAt, target.position, lookAtSmoothing * Time.deltaTime);
            transform.LookAt(currentLookAt);
        }
    }
}
