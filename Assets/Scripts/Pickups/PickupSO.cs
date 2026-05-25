using UnityEngine;

namespace Elenor {
    public enum PickupCategory {
        Weapon,
        Modifier,
    }
    public enum PickupRarity {
        Common,
        Rare,
        Legendary,
    }

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

        public abstract PickupCategory Category { get; }

        public const int MaxLevel = 3;
    }
}