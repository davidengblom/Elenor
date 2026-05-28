using UnityEngine;
using System;

namespace Elenor {
    [RequireComponent(typeof(PlayerInputReader))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerShooter : MonoBehaviour {
        [SerializeField] WeaponSO weapon;
        [SerializeField] Projectile projectileShellPrefab;
        [SerializeField] WeaponPickup weaponPickupPrefab;
        [SerializeField, Tooltip("How far in front of the player the bullet spawns.")]
        float muzzleOffset = 0.45f;

        PlayerInputReader _input;
        PlayerStats _stats;
        float _nextFireTime;
        WeaponSO _equippedWeapon;

        public WeaponSO EquippedWeapon => _equippedWeapon;
        public event Action<WeaponSO> WeaponEquipped;
        public event Action<WeaponSO> WeaponSwapped;

        void Awake() {
            _input = GetComponent<PlayerInputReader>();
            _stats = GetComponent<PlayerStats>();

            if (projectileShellPrefab == null) {
                Debug.LogError("PlayerShooter: no projectileShellPrefab assigned.", this);
            }
            if (weaponPickupPrefab == null) {
                Debug.LogError("PlayerShooter: no weaponPickupPrefab assigned.", this);
            }
            if (weapon == null) {
                Debug.LogError("PlayerShooter: no weapon assigned.", this);
                return;
            }
            EquipWeapon(weapon);
        }

        void Update() {
            if (_equippedWeapon == null || !_input.ShootHeld) return;
            if (Time.time < _nextFireTime) return;

            Vector2 dir = _input.ShootInput.normalized;
            if (dir.sqrMagnitude < 0.0001f) return;

            Spawn(dir);

            _nextFireTime = Time.time + 1f / Mathf.Max(0.01f, _equippedWeapon.FireRate);
        }

        void Spawn(Vector2 dir) {
            Vector3 spawnPos = transform.position + (Vector3)(dir * muzzleOffset);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Projectile proj = Instantiate(
                projectileShellPrefab,
                spawnPos,
                Quaternion.Euler(0f, 0f, angle)
            );

            ProjectileConfigSnapshot snapshot = _equippedWeapon.ProjectileConfig.ToSnapshot();

            // Apply all modifiers to snapshot
            foreach (var mod in GetComponents<IProjectileModifier>()) {
                mod.Modify(ref snapshot);
            }

            proj.Configure(snapshot);

            float statMult = _stats != null ? _stats.DamageMultiplier : 1f;
            float dmg = snapshot.Damage * _equippedWeapon.DamageMultiplier * statMult;
            proj.Launch(
                dir * snapshot.Speed,
                dmg,
                snapshot.Lifetime,
                snapshot.KnockbackForce
            );
        }

        public void EquipWeapon(WeaponSO newWeapon) {
            if (newWeapon == null) {
                Debug.LogError("PlayerShooter: EquipWeapon called with null.", this);
                return;
            }
            if (newWeapon.ProjectileConfig == null) {
                Debug.LogError($"PlayerShooter: weapon {newWeapon.DisplayName} has no projectileConfig", this);
                return;
            }

            _equippedWeapon = newWeapon;
            WeaponEquipped?.Invoke(_equippedWeapon);
        }

        /// <summary>
        /// Equips a new weapon and drops the current one on the floor.
        public bool SwapWeapon(WeaponSO newWeapon, Vector3 dropPosition, Transform dropParent = null) {
            if (newWeapon == null) return false;
            if (newWeapon == _equippedWeapon) return false;

            WeaponSO previous = _equippedWeapon;
            EquipWeapon(newWeapon);
            WeaponSwapped?.Invoke(newWeapon);

            if (previous != null && weaponPickupPrefab != null) {
                WeaponPickup drop = Instantiate(weaponPickupPrefab, dropPosition, Quaternion.identity, dropParent);
                drop.Configure(previous);
                if (RoomManager.Instance != null) {
                    RoomManager.Instance.RegisterDroppedWeapon(previous, dropPosition);
                }
            }

            return true;
        }
    }
}