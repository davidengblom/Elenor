using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Weapons/Behaviors/ChargeShot", fileName = "ChargeShotBehavior_")]
    public class ChargeShotBehaviorSO : WeaponBehaviorSO {
        [Header("Charged Shot")]
        [SerializeField] ProjectileConfigSO chargedProjectileConfig;
        [SerializeField, Min(0.01f)] float maxChargeTime = 0.8f;
        [SerializeField, Min(1f)] float fullChargeDamageMultiplier = 4f;
        [Tooltip("Hold shorter than this counts as a tap shot.")]
        [SerializeField, Min(0f)] float minChargedHoldSeconds = 0.18f;

        public ProjectileConfigSO ChargedProjectileConfig => chargedProjectileConfig;
        public float MaxChargeTime => maxChargeTime;
        public float FullChargeDamageMultiplier => fullChargeDamageMultiplier;
        public float MinChargedHoldSeconds => minChargedHoldSeconds;
    }
}