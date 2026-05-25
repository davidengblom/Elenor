using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Weapons/Weapon", fileName = "Weapon_")]
    public class WeaponSO : PickupSO {
        [Header("Identity")]
        [SerializeField] Sprite icon;

        [Header("Firing")]
        [SerializeField] ProjectileConfigSO projectileConfig;
        [SerializeField, Tooltip("Shots per second")]
        float fireRate = 4f;
        [SerializeField, Tooltip("Multiplier applied to projectile damage.")]
        float damageMultiplier = 1f;

        public override PickupCategory Category => PickupCategory.Weapon;

        public Sprite Icon => icon;
        public ProjectileConfigSO ProjectileConfig => projectileConfig;
        public float FireRate => fireRate;
        public float DamageMultiplier => damageMultiplier;
    }
}