using DungeonArchitect.Builders.GridFlow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Samples.GridFlow
{
    public class PickableItem : MonoBehaviour
    {
        public InventoryItemType itemType = InventoryItemType.None;
        public Sprite icon = null;

        private void OnTriggerEnter(Collider other)
        {
            var go = other.gameObject;
            var inventory = go.GetComponentInChildren<Inventory>();
            if (inventory != null)
            {
                var item = new InventoryItem();
                item.itemType = itemType;
                item.itemId = GetItemId();
                item.icon = icon;

                if (inventory.Add(item))
                {
                    // We successfully added this item to the inventory. destroy this game object
                    Destroy(gameObject);
                    return;
                }
            }
        }

        string GetItemId()
        {
            var itemMetadata = GetComponent<GridFlowItemMetadataComponent>();
            return (itemMetadata != null) ? itemMetadata.itemId : "";
        }
    }
}
