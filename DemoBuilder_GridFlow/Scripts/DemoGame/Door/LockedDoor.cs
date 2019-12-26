using DungeonArchitect.Builders.GridFlow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Samples.GridFlow
{
    public class LockedDoor : MonoBehaviour
    {
        public Transform doorLeft;
        public Transform doorRight;

        private Animator animator;

        private string[] validKeys = new string[0];
 

        private void Start()
        {
            // find the door id (grab it from the item metadata component that DA creates)
            var lockMetadata = GetComponent<GridFlowDoorLockComponent>();
            if (lockMetadata != null)
            {
                validKeys = lockMetadata.validKeyIds;
            }

            animator = GetComponent<Animator>();
        }


        void OnTriggerEnter(Collider other)
        {
            if (CanOpenDoor(other))
            {
                OpenDoor();
            }
        }

        void OnTriggerExit(Collider other)
        {
            CloseDoor();
        }

        bool CanOpenDoor(Collider other)
        {
            var inventory = other.gameObject.GetComponentInChildren<Inventory>();
            if (inventory != null)
            {
                // Check if any of the valid keys are present in the inventory of the collided object
                foreach (var validKey in validKeys)
                {
                    if (inventory.ContainsItem(validKey))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        void OpenDoor()
        {
            animator.SetBool("doorOpen", true);
        }

        void CloseDoor()
        {
            animator.SetBool("doorOpen", false);
        }

    }
}
