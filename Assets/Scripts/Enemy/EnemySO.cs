using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Enemy", fileName = "Enemy_New")]
    public class EnemySO : ScriptableObject {
        [Header("Health")]
        [SerializeField] float maxHealth = 3f;

        [Header("Ranged")]
        [SerializeField] WeaponSO weapon;

        [Header("Movement")]
        [SerializeField, Tooltip("0 = stationary, >0 = walks toward player at this speed.")]
        float moveSpeed = 0f;

        [Header("Melee")]
        [SerializeField, Tooltip("Damage dealt to the player on contact. 0 disables.")]
        float contactDamage = 0f;

        public float MaxHealth => maxHealth;
        public WeaponSO Weapon => weapon;
        public float MoveSpeed => moveSpeed;
        public float ContactDamage => contactDamage;
    }
}