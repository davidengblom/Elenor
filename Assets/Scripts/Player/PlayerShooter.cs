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
            _nextFireTime = Time.time + 1f / Mathf.Max(0.01f, weapon.FireRate);
        }

        void Spawn(Vector2 dir) {
            Vector3 spawnPos = transform.position + (Vector3)(dir * muzzleOffset);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Projectile proj = Instantiate(
                weapon.ProjectilePrefab,
                spawnPos,
                Quaternion.Euler(0f, 0f, angle)
            );

            float dmg = weapon.Damage * (_stats != null ? _stats.DamageMultiplier : 1f);
            proj.Launch(dir * weapon.ProjectileSpeed, dmg, weapon.ProjectileLifetime);
        }

        [ContextMenu("Debug: Log Damage")]
        void DebugLogDamage() {
            float baseDmg = weapon != null ? weapon.Damage : 0f;
            float mult = _stats != null ? _stats.DamageMultiplier : 1f;
            Debug.Log($"PlayerShooter: base={baseDmg} × mult={mult} = {baseDmg * mult}", this);
        }
    }
}