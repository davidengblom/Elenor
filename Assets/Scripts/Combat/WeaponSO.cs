using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Weapon", fileName = "Weapon_New")]
    public class WeaponSO : ScriptableObject {
        [SerializeField] Projectile projectilePrefab;
        [SerializeField] float damage = 1f;
        [SerializeField, Tooltip("Shots per second")] float fireRate = 3f;
        [SerializeField] float projectileSpeed = 12f;
        [SerializeField] float projectileLifetime = 1.5f;

        public Projectile ProjectilePrefab => projectilePrefab;
        public float Damage => damage;
        public float FireRate => fireRate;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileLifetime => projectileLifetime;
    }
}