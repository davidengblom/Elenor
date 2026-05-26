using UnityEngine;
using System.Collections.Generic;

namespace Elenor {
    /// <summary>
    /// Selects items for item rooms with pool exclusion rules.
    /// </summary>
    public static class ItemRoomDropPipeline {
        public static WeaponSO SelectWeapon(
            IReadOnlyList<PickupSO> allPickups,
            WeaponSO equippedWeapon
        ) {
            var candidates = new List<WeaponSO>();
            for (int i = 0; i < allPickups.Count; i++) {
                if (allPickups[i] is WeaponSO w && w != equippedWeapon) {
                    candidates.Add(w);
                }
            }
            if (candidates.Count == 0) return null;
            return candidates[Random.Range(0, candidates.Count)]; // TODO: Maybe have tweakable spawn rates for balancing purposes?
        }

        // TODO: This method and the one above are essentially identical. Can we combine them somehow?
        public static ModifierSO SelectModifier(
            IReadOnlyList<PickupSO> allPickups,
            PlayerPickupInventory inventory
        ) {
            var candidates = new List<ModifierSO>();
            for (int i = 0; i < allPickups.Count; i++) {
                if (allPickups[i] is ModifierSO m) {
                    int level = inventory != null ? inventory.GetLevel(m) : 0;
                    if (level < PickupSO.MaxLevel) {
                        candidates.Add(m);
                    }
                }
            }
            if (candidates.Count == 0) return null;
            return candidates[Random.Range(0, candidates.Count)]; // TODO: Maybe have tweakable spawn rates for balancing purposes?
        }
    }
}