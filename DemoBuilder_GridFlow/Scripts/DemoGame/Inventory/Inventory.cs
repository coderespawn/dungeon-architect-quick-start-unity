//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.Samples.GridFlow
{
    public enum InventoryItemType
    {
        None,
        Key,
    }

    [System.Serializable]
    public class InventoryItem
    {
        public InventoryItemType itemType = InventoryItemType.None;
        public string itemId;
        public Sprite icon = null;
    }

    public class Inventory : MonoBehaviour
    {
        public InventorySlot[] slots;
        InventoryUI inventoryUI;

        private void Awake()
        {
            inventoryUI = GameObject.FindObjectOfType<InventoryUI>();
        }

        /// <summary>
        /// Adds an item to a free slot
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool Add(InventoryItem item)
        {
            foreach (var slot in slots)
            {
                if (slot.item.itemType == InventoryItemType.None)
                {
                    // Found a free slot
                    slot.item = item;

                    // Update the UI
                    if (inventoryUI != null)
                    {
                        inventoryUI.UpdateUI(this);
                    }
                    return true;
                }
            }
            return false;
        }

        public bool ContainsItem(string itemId)
        {
            foreach (var slot in slots)
            {
                if (slot.item.itemType != InventoryItemType.None)
                {
                    if (slot.item.itemId == itemId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
