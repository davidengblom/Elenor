using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerShooter : MonoBehaviour {
        [SerializeField] WeaponSO weapon;
        [SerializeField, Tooltip("How far in front of the player the bullet spawns.")]
        float muzzleOffset = 0.45f;

        PlayerInputReader _input;
        PlayerStats _stats;
        float _nextFireTime;

        void Awake() {
            _input = GetComponent<PlayerInputReader>();
            _stats = GetComponent<PlayerStats>();
        }

        void Update() {
            if (weapon == null || !_input.ShootHeld) return;
            if (Time.time < _nextFireTime) return;

            Vector2 dir = _input.ShootInput.normalized;
            if (dir.sqrMagnitude < 0.0001f) return;

            Spawn(dir);

            float fireRate = weapon.FireRate * GetFireRateMultiplier();
            _nextFireTime = Time.time + 1f / Mathf.Max(0.01f, fireRate);
        }

        void Spawn(Vector2 dir) {
            Vector3 spawnPos = transform.position + (Vector3)(dir * muzzleOffset);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            var mg = GetComponent<MachineGunModifier>();
            Projectile prefab = mg != null && mg.ProjectilePrefab != null ? mg.ProjectilePrefab : weapon.ProjectilePrefab;

            Projectile proj = Instantiate(
                prefab,
                spawnPos,
                Quaternion.Euler(0f, 0f, angle)
            );

            float dmg = weapon.Damage * (_stats != null ? _stats.DamageMultiplier : 1f) * GetDamageMultiplier();
            proj.Launch(dir * weapon.ProjectileSpeed, dmg, weapon.ProjectileLifetime, weapon.KnockbackForce);
        }

        float GetFireRateMultiplier() {
            var mg = GetComponent<MachineGunModifier>();
            return mg != null ? mg.FireRateMultiplier : 1f;
        }

        float GetDamageMultiplier() {
            var mg = GetComponent<MachineGunModifier>();
            return mg != null ? mg.DamagePerShotMultiplier : 1f;
        }

        [ContextMenu("Debug: Log Damage")]
        void DebugLogDamage() {
            float baseDmg = weapon != null ? weapon.Damage : 0f;
            float mult = _stats != null ? _stats.DamageMultiplier : 1f;
            Debug.Log($"PlayerShooter: base={baseDmg} × mult={mult} = {baseDmg * mult}", this);
        }
    }
}