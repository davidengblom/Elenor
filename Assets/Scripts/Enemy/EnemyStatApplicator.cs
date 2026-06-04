using UnityEngine;

namespace Elenor {
    /// <summary>
    /// Spawn time enemy state modification.
    /// </summary>
    public static class EnemyStatApplicator {
        /// <summary> Global multiplier applied at spawn (Future: set per section/floor) </summary>
        public static float SectionMultiplier { get; set; } = 1f;

        public static float GetMaxHealth(EnemySO so, float fallback = 3f) {
            if (so == null) return fallback * SectionMultiplier;
            return so.MaxHealth * SectionMultiplier;
        }

        public static float GetContactDamage(EnemySO so) {
            if (so == null) return 0f;
            return so.ContactDamage * SectionMultiplier;
        }

        /// <summary> Scales any damage value computed from SO/weapon config </summary>
        public static float ScaleDamage(float baseDamage) {
            return baseDamage * SectionMultiplier;
        }
    }
}