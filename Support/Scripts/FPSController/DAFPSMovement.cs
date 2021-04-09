using UnityEngine;

namespace DungeonArchitect.Samples.Common
{
    public class DAFPSMovement : MonoBehaviour
    {
        public float speed = 12;
        public float gravity = -9.81f;
        public float jumpHeight = 1;

        public CharacterController controller;
        Vector3 velocity = Vector3.zero;

        void Update()
        {
            if (controller == null) return;

            var x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            var z = Input.GetAxis("Vertical") * speed * Time.deltaTime;

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move);

            if (Input.GetButton("Jump") && controller.isGrounded)
            {
                //velocity.y += Mathf.Sqrt(jumpHeight * -1 * gravity);
            }

            velocity.y += gravity * Time.deltaTime;
            var movementPerFrame = velocity * Time.deltaTime;
            controller.Move(movementPerFrame);
            if (controller.isGrounded)
            {
                velocity.y = 0;
            }
        }
    }
}
