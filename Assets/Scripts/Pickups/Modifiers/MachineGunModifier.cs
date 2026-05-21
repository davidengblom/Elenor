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
        public Color BulletColor { get; private set; } = Color.white;
        public Sprite BulletSprite { get; private set; }

        public void Configure(
            float fireRateMultiplier,
            float damagePerShotMultiplier,
            float bulletScaleMultiplier,
            Color bulletColor,
            Sprite bulletSprite
        ) {
            FireRateMultiplier = fireRateMultiplier;
            DamagePerShotMultiplier = damagePerShotMultiplier;
            BulletScaleMultiplier = bulletScaleMultiplier;
            BulletColor = bulletColor;
            BulletSprite = bulletSprite;
        }
    }
}