using UnityEngine;

namespace Elenor {
    /// <summary>
    /// Base for all pickups. A pickup is identified by a stable key (SO)
    /// Picking up a duplicate of the same SO levels the existing pickup (max level 3)
    /// </summary>
    public abstract class PickupSO : ScriptableObject {
        [Header("Identity")]
        [SerializeField] string displayName = "Pickup";
        [SerializeField, TextArea] string flavorText = "";

        [Header("Visuals")]
        [SerializeField] Sprite sprite;
        [SerializeField] Color displayColor = Color.white;

        [Header("Drop pool")]
        [SerializeField] PickupRarity rarity = PickupRarity.Common;

        public string DisplayName => displayName;
        public string FlavorText => flavorText;
        public Sprite Sprite => sprite;
        public Color DisplayColor => displayColor;
        public PickupRarity Rarity => rarity;

        public const int MaxLevel = 3;

        /// <summary>
        /// Returns the BehaviorModifier component type this pickup needs to the player.
        /// </summary>
        public abstract System.Type ModifierType { get; }

        /// <summary>
        /// Apply per level config for a freshly attached or existing modifier component.
        /// </summary>
        public abstract void ConfigureModifier(IBehaviorModifier modifier, int level);
    }

    public enum PickupRarity {
        Common,
        Rare,
        Legendary,
    }
}