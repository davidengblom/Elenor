using UnityEngine;

namespace Elenor {
    public class EnemyShooter : MonoBehaviour, IEnemyComponent {
        [SerializeField] EnemySO data;
        [SerializeField] Projectile projectileShellPrefab;
        [SerializeField] float muzzleOffset = 0.45f;
        [SerializeField, Tooltip("Random seconds added to the first shot, so enemies spawned at the same time don't fire together.")]
        float startupJitter = 0.5f;

        float _nextFireTime;

        void Awake() {
            if (projectileShellPrefab == null) {
                Debug.LogError("EnemyShooter: no projectileShellPrefab assigned.", this);
            }
            Init(data);
        }

        public void Init(EnemySO so) {
            data = so;
            if (so == null || !so.IsRanged) {
                enabled = false;
                return;
            }
            if (so.Weapon == null || so.Weapon.ProjectileConfig == null) {
                Debug.LogError($"EnemyShooter: {so.DisplayName} has no weapon or projectileConfig", this);
                enabled = false;
                return;
            }
            enabled = true;
            _nextFireTime = Time.time + Random.Range(0f, startupJitter);
        }

        void Update() {
            if (data == null || !data.IsRanged) return;

            if (TryGetComponent<EnemyLineOfSightAggro>(out var los) && !los.CanAttack) return;

            Transform player = PlayerLocator.Player;
            if (player == null) return;
            if (Time.time < _nextFireTime) return;

            Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
            if (dir.sqrMagnitude < 0.0001f) return;

            Spawn(dir);
            _nextFireTime = Time.time + 1f / Mathf.Max(0.01f, data.Weapon.FireRate);
        }

        void Spawn(Vector2 dir) {
            var weapon = data.Weapon;
            ProjectileConfigSO config = weapon.ProjectileConfig;

            Vector3 spawnPos = transform.position + (Vector3)(dir * muzzleOffset);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Projectile proj = Instantiate(
                projectileShellPrefab,
                spawnPos,
                Quaternion.Euler(0f, 0f, angle)
            );

            proj.Configure(config);
            float dmg = EnemyStatApplicator.ScaleDamage(config.Damage * weapon.DamageMultiplier);
            proj.Launch(
                dir * config.Speed,
                dmg,
                config.Lifetime,
                config.KnockbackForce
            );
        }
    }
}