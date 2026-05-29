using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/Pickup Registry", fileName = "PickupRegistry")]
    public class PickupRegistrySO : ScriptableObject {
        [SerializeField, Tooltip("Every PickupSO in the game. New pickups are added as authored.")]
        List<PickupSO> allPickups = new();

        public IReadOnlyList<PickupSO> AllPickups => allPickups;

        public IEnumerable<PickupSO> GetByRarities(IReadOnlyList<PickupRarity> allowed) {
            if (allowed == null || allowed.Count == 0) yield break;
            for (int i = 0; i < allPickups.Count; i++) {
                PickupSO pickup = allPickups[i];
                if (pickup == null) continue;
                if (allowed.Contains(pickup.Rarity)) yield return pickup;
            }
        }
    }
}