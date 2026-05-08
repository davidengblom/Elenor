using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Enemy", fileName = "Enemy_")]
    public class EnemySO : ScriptableObject {
        [Header("Identity")]
        [SerializeField] string displayName = "Enemy";

        [Header("Visuals")]
        [SerializeField] Sprite sprite;
        [SerializeField] Color tint = Color.white;
        [SerializeField, Min(0.1f)] float scale = 1f;

        [Header("Health")]
        [SerializeField, Min(1f)] float maxHealth = 3f;

        [Header("Movement")]
        [SerializeField, Min(0f), Tooltip("0 = stationary, >0 = walks toward player at this speed.")] 
        float moveSpeed = 0f;

        [Header("Ranged")]
        [SerializeField, Tooltip("Null = no ranged attack.")]
        WeaponSO weapon;

        [Header("Melee")]
        [SerializeField, Min(0f), Tooltip("Damage dealt to the player on body contact. 0 disables melee.")]
        float contactDamage = 0f;

        public string DisplayName => displayName;
        public Sprite Sprite => sprite;
        public Color Tint => tint;
        public float Scale => scale;
        public float MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public WeaponSO Weapon => weapon;
        public float ContactDamage => contactDamage;

        public bool IsStationary => moveSpeed <= 0f;
        public bool IsRanged => weapon != null;
        public bool IsMelee => contactDamage > 0f;
    }
}