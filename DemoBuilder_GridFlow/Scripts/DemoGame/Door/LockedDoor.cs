//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Items;
using UnityEngine;

namespace DungeonArchitect.Samples.GridFlow
{
    public class LockedDoor : MonoBehaviour
    {
        public Transform doorLeft;
        public Transform doorRight;

        private Animator animator;

        private string lockId;
        private string[] validKeys = new string[0];
 

        private void Start()
        {
            // find the door id (grab it from the item metadata component that DA creates)
            var lockItemMetadata = FindItemMetadata();
            if (lockItemMetadata != null)
            {
                lockId = lockItemMetadata.itemId;
                validKeys = lockItemMetadata.referencedItemIds;
            }

            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Search the current game object and works its way up the hierarchy to find the item metadata object 
        /// </summary>
        /// <returns></returns>
        FlowItemMetadataComponent FindItemMetadata()
        {
            var obj = gameObject;
            while (obj != null)
            {
                var itemMetadata = obj.GetComponent<FlowItemMetadataComponent>();
                if (itemMetadata != null)
                {
                    return itemMetadata;
                }

                var parentTransform = obj.transform.parent; 
                obj = (parentTransform != null) ? parentTransform.gameObject : null;
            }

            return null;
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
