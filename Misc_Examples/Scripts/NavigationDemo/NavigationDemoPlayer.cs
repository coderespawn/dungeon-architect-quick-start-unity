using System.Collections;
using System.Collections.Generic;
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

        void Update()
        {
            transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            float curSpeed = speed * Input.GetAxis("Vertical");
            character.SimpleMove(forward * curSpeed);
        }
    }
}
