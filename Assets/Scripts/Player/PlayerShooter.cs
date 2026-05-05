using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(PlayerInputReader))]
    public class PlayerShooter : MonoBehaviour {
        [SerializeField] WeaponSO weapon;
        [SerializeField, Tooltip("How far in front of the player the bullet spawns.")]
        float muzzleOffset = 0.45f;

        [SerializeField, Tooltip("Multiplier applied on top of weapon's base damage.")]
        float damageMultiplier = 1f;

        public float DamageMultiplier {
            get => damageMultiplier;
            set => damageMultiplier = Mathf.Max(0f, value);
        }
        PlayerInputReader _input;
        Camera _cam;
        float _nextFireTime;

        void Awake() {
            _input = GetComponent<PlayerInputReader>();
            _cam = Camera.main;
        }

        void Update() {
            if (weapon == null || !_input.FireHeld) return;
            if (Time.time < _nextFireTime) return;

            Vector2 dir = ComputeAimDirection();
            if (dir.sqrMagnitude < 0.0001f) return;

            Spawn(dir);
            _nextFireTime = Time.time + 1f / Mathf.Max(0.01f, weapon.FireRate);
        }

        Vector2 ComputeAimDirection() {
            Vector3 mouse = _input.AimScreenPosition;
            mouse.z = -_cam.transform.position.z;
            Vector3 world = _cam.ScreenToWorldPoint(mouse);
            return ((Vector2)world - (Vector2)transform.position).normalized;
        }

        void Spawn(Vector2 dir) {
            Vector3 spawnPos = transform.position + (Vector3)(dir * muzzleOffset);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Projectile proj = Instantiate(
                weapon.ProjectilePrefab,
                spawnPos,
                Quaternion.Euler(0f, 0f, angle)
            );

            proj.Launch(dir * weapon.ProjectileSpeed, weapon.Damage * damageMultiplier, weapon.ProjectileLifetime);
        }

        [ContextMenu("Debug: Log Damage")]
        void DebugLogDamage() {
            float baseDmg = weapon != null ? weapon.Damage : 0f;
            Debug.Log($"PlayerShooter: base={baseDmg} × mult={damageMultiplier} = {baseDmg * damageMultiplier}", this);
        }
    }
}