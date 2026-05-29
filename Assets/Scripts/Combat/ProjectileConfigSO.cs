using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Projectile Config", fileName = "ProjectileConfig_")]
    public class ProjectileConfigSO : ScriptableObject {
        [Header("Visuals")]
        [SerializeField] Sprite sprite;
        [SerializeField] Color color = Color.white;
        [SerializeField, Tooltip("Uniform scale applied to the projectile root transform.")]
        float scale = 0.2f;

        [Header("Behavior")]
        [SerializeField] float speed = 12f;
        [SerializeField] float damage = 1f;
        [SerializeField] float lifetime = 1.5f;
        [SerializeField, Min(0f)] float knockbackForce = 0.1f;

        [Header("Pierce")]
        [SerializeField] bool pierce = false;
        [SerializeField, Min(0)] int maxPierceCount = 0;

        [Header("Poison")]
        [SerializeField] bool applyPoison = false;
        [SerializeField, Min(0f)] float poisonDamagePerSecond = 0f;
        [SerializeField, Min(0f)] float poisonDuration = 0f;

        public Sprite Sprite => sprite;
        public Color Color => color;
        public float Scale => scale;
        public float Speed => speed;
        public float Damage => damage;
        public float Lifetime => lifetime;
        public float KnockbackForce => knockbackForce;
        public bool Pierce => pierce;
        public int MaxPierceCount => maxPierceCount;
        public bool ApplyPoison => applyPoison;
        public float PoisonDamagePerSecond => poisonDamagePerSecond;
        public float PoisonDuration => poisonDuration;

        public ProjectileConfigSnapshot ToSnapshot() => new ProjectileConfigSnapshot {
            Sprite = sprite,
            Color = color,
            Scale = scale,
            Speed = speed,
            Damage = damage,
            Lifetime = lifetime,
            KnockbackForce = knockbackForce,
            Pierce = pierce,
            MaxPierceCount = maxPierceCount,
            ApplyPoison = false,
            PoisonDps = 0f,
            PoisonDuration = 0f,
        };
    }
}