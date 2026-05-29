using UnityEngine;
using Elenor;

namespace Elenor.UI.HUD {
    /// <summary>
    /// Base for HUD widgets that react to PlayerPickupInventory events.
    /// </summary>
    public abstract class PickupInventoryListener : MonoBehaviour {
        protected PlayerPickupInventory Inventory { get; private set; }

        protected virtual void Start() {
            Transform player = PlayerLocator.Player;
            if (player == null) return;

            Inventory = player.GetComponent<PlayerPickupInventory>();
            if (Inventory == null) return;

            Inventory.PickupAcquired += OnPickupAcquired;
            Inventory.PickupLeveledUp += OnPickupLeveledUp;
            OnInventoryReady();
        }

        protected virtual void OnDestroy() {
            if (Inventory == null) return;
            Inventory.PickupAcquired -= OnPickupAcquired;
            Inventory.PickupLeveledUp -= OnPickupLeveledUp;
        }

        /// <summary> Called once the inventory reference is resolved. </summary>
        protected virtual void OnInventoryReady() {}

        protected abstract void OnPickupAcquired(PickupSO pickup, int level);
        protected abstract void OnPickupLeveledUp(PickupSO pickup, int level);
    }
}