//$ Copyright 2015-21, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using UnityEngine.UI;

namespace DungeonArchitect.Samples.GridFlow
{
    public class InventoryUI : MonoBehaviour
    {
        public Image[] slotImages;

        public void UpdateUI(Inventory inventory)
        {
            for (int i = 0; i < inventory.slots.Length && i < slotImages.Length; i++)
            {
                var slot = inventory.slots[i];
                if (slot.item.itemType != InventoryItemType.None)
                {
                    var image = slotImages[i];
                    image.sprite = slot.item.icon;
                    image.color = Color.white;
                }
            }
        }
    }
}
