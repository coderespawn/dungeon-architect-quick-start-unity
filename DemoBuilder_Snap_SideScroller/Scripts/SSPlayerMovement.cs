//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DASideScroller
{
    public class SSPlayerMovement : MonoBehaviour
    {
        public float speed = 6f;            // The speed that the player will move at.
        public float gravity = 40;
        public float jumpSpeed = 15;
        public int maxJumps = 2;
        public float interJumpDelay = 0.4f;

        bool facingLeft = false;
        CharacterController character;
        Vector3 movement;                   // The vector to store the direction of the player's movement.
        Animator anim;                      // Reference to the animator component.
        int numJumpsPerformed = 0;
        float lastJumpTimestamp = 0;

        void Awake()
        {
            anim = GetComponent<Animator>();
            character = GetComponent<CharacterController>();
            //character.enabled = false;
        }

        public void OnTeleportered()
        {
            character.enabled = true;
        }


        void Update()
        {
            // Store the input axes.
            float h = Input.GetAxisRaw("Horizontal");
            //float v = Input.GetAxisRaw("Vertical");

            // Move the player around the scene.
            Move(h);

            // Turn the player to face the mouse cursor.
            Turning(h);

            // Animate the player.
            Animating(h);
        }

        void PerformJump()
        {
            if (numJumpsPerformed >= maxJumps)
            {
                return;
            }

            // Check if enough time has passed since
            if (Time.time - lastJumpTimestamp < interJumpDelay)
            {
                return;
            }

            movement.y = jumpSpeed;
            numJumpsPerformed++;
            lastJumpTimestamp = Time.time;
        }

        void Move(float h)
        {

            if (character.isGrounded)
            {
                // Set the movement vector based on the axis input.
                movement.Set(h, 0, 0);
                movement *= speed;
                numJumpsPerformed = 0;
            }
            else
            {
                movement.x = h * speed;

                if (character.velocity.y == 0)
                {
                    movement.y = 0;
                }
            }

            if (Input.GetButton("Jump"))
            {
                PerformJump();
            }

            // Apply gravity
            movement.y -= gravity * Time.deltaTime;
            
            if (character.enabled)
            {
                character.Move(movement * Time.deltaTime);
            }
        }


        void Turning(float h)
        {
            if (h != 0)
            {
                facingLeft = (h < 0);
            }

            var angle = facingLeft ? -90 : 90;
            transform.rotation = Quaternion.Euler(0, angle, 0);
            /*
            // Create a ray from the mouse cursor on screen in the direction of the camera.
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Create a RaycastHit variable to store information about what was hit by the ray.
            RaycastHit floorHit;

            // Perform the raycast and if it hits something on the floor layer...
            if (Physics.Raycast(camRay, out floorHit, camRayLength))
            {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                Vector3 playerToMouse = floorHit.point - transform.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

                // Set the player's rotation to this new rotation.
                transform.rotation = newRotatation;
                //playerRigidbody.MoveRotation (newRotatation);
            }
            */
        }


        void Animating(float h)
        {
            // Create a boolean that is true if either of the input axes is non-zero.
            bool walking = h != 0f;

            // Tell the animator whether or not the player is walking.
            anim.SetBool("IsWalking", walking);
        }

    }

}
