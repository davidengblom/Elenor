using UnityEngine;

namespace Elenor {
    public class EnemyShooter : MonoBehaviour {
        [SerializeField] EnemySO data;
        [SerializeField] float muzzleOffset = 0.45f;
        [SerializeField, Tooltip("Random seconds added to the first shot, so enemies spawned at the same time don't fire together.")]
        float startupJitter = 0.5f;

        Transform _playerCache;
        float _nextFireTime;

        Transform Player {
            get {
                if (_playerCache == null) {
                    var go = GameObject.FindGameObjectWithTag("Player");
                    if (go != null) _playerCache = go.transform;
                }
                return _playerCache;
            }
        }

        void Start() {
            _nextFireTime = Time.time + Random.Range(0f, startupJitter);
        }

        void Update() {
            if (data == null || data.Weapon == null) return;
            if (Player == null) return;
            if (Time.time < _nextFireTime) return;

            Vector2 dir = ((Vector2)Player.position - (Vector2)transform.position).normalized;
            if (dir.sqrMagnitude < 0.0001f) return;

            Spawn(dir);
            _nextFireTime = Time.time + 1f / Mathf.Max(0.01f, data.Weapon.FireRate);
        }

        void Spawn(Vector2 dir) {
            var weapon = data.Weapon;
            Vector3 spawnPos = transform.position + (Vector3)(dir * muzzleOffset);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Projectile proj = Instantiate(
                weapon.ProjectilePrefab,
                spawnPos,
                Quaternion.Euler(0f, 0f, angle)
            );

            proj.Launch(dir * weapon.ProjectileSpeed, weapon.Damage, weapon.ProjectileLifetime);
        }
    }
}