using UnityEngine;

namespace Elenor {
    /// <summary>
    /// Behavior modifier added to the player when Pickup_MachineGun is acquired.
    /// Multiplies fire rate and divides shot damage.
    /// </summary>
    public class MachineGunModifier : MonoBehaviour, IBehaviorModifier {
        public int Level { get; set; }
        public float FireRateMultiplier { get; private set; } = 1f;
        public float DamagePerShotMultiplier { get; private set; } = 1f;
        public float BulletScaleMultiplier { get; private set; } = 1f;
        public Projectile ProjectilePrefab { get; private set; }

        public void Configure(
            float fireRateMultiplier,
            float damagePerShotMultiplier,
            float bulletScaleMultiplier,
            Projectile projectilePrefab
        ) {
            FireRateMultiplier = fireRateMultiplier;
            DamagePerShotMultiplier = damagePerShotMultiplier;
            BulletScaleMultiplier = bulletScaleMultiplier;
            ProjectilePrefab = projectilePrefab;
        }
    }
}