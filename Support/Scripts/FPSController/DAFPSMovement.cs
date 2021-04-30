//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Samples.Common
{
    public class DAFPSMovement : MonoBehaviour
    {
        public float speed = 12;
        public float gravity = -9.81f;
        public bool enableJumping = false;
        public float jumpHeight = 1;

        public CharacterController controller;
        float speedY = 0;

        void Update()
        {
            if (controller == null) return;

            var x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            var z = Input.GetAxis("Vertical") * speed * Time.deltaTime;

            Vector3 move = transform.right * x + transform.forward * z;

            if (controller.isGrounded)
            {
                speedY = 0;
                if (enableJumping && Input.GetButton("Jump"))
                {
                    speedY += jumpHeight;
                }
            }
            speedY += gravity * Time.deltaTime;

            move.y = speedY * Time.deltaTime;
            controller.Move(move);
        }
    }
}
