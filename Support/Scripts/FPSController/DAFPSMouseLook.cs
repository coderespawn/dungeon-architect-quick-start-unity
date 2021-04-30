//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Samples.Common
{
    public class DAFPSMouseLook : MonoBehaviour
    {
        public float lookSpeed = 1.25f;
        public Transform playerBody;

        float angleUpDown = 0;

        public void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

            angleUpDown -= mouseY;
            angleUpDown = Mathf.Clamp(angleUpDown, -90, 90);
            transform.localRotation = Quaternion.Euler(angleUpDown, 0, 0);

            if (playerBody != null)
            {
                playerBody.Rotate(0, mouseX, 0);
            }
        }
    }
}