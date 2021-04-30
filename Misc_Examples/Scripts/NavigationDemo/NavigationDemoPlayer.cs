//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Samples.Navigation
{
    public class NavigationDemoPlayer : MonoBehaviour
    {
        CharacterController character;
        void Awake()
        {
            character = GetComponent<CharacterController>();
        }

        public float speed = 3.0F;
        public float rotateSpeed = 3.0F;

        void FixedUpdate()
        {
            transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            float curSpeed = speed * Input.GetAxis("Vertical");
            character.SimpleMove(forward * curSpeed);
        }
    }
}
