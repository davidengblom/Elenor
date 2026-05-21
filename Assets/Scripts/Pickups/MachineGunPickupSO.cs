using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Pickups/Machine Gun", fileName = "Pickup_MachineGun")]
    public class MachineGunPickupSO : PickupSO {
        [System.Serializable]
        public struct LevelData {
            [Tooltip("Multiplier applied to weapon fire rate.")]
            public float fireRateMultiplier;
            [Tooltip("Multiplier applied to weapon damage per shot.")]
            public float damagePerShotMultiplier;
            [Tooltip("Multiplier applied to bullet scale.")]
            public float bulletScaleMultiplier;
            [Tooltip("Color applied to bullet.")]
            public Color bulletColor;
            [Tooltip("Sprite applied to bullet.")]
            public Sprite bulletSprite;
        }

        [SerializeField] LevelData[] levels = new LevelData[MaxLevel];

        public override System.Type ModifierType => typeof(MachineGunModifier);

        public override void ConfigureModifier(IBehaviorModifier modifier, int level) {
            if (modifier is not MachineGunModifier mg) return;
            int index = Mathf.Clamp(level - 1, 0, levels.Length - 1);
            LevelData data = levels[index];
            mg.Configure(
                data.fireRateMultiplier,
                data.damagePerShotMultiplier,
                data.bulletScaleMultiplier,
                data.bulletColor,
                data.bulletSprite
            );
        }

        void OnValidate() {
            if (levels == null || levels.Length != MaxLevel) {
                System.Array.Resize(ref levels, MaxLevel);
            }
        }
    }
}