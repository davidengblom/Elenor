using UnityEngine;
using System;
using System.Collections.Generic;

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
        readonly List<Vector2> _spawnDirections = new();
        bool _isCharging;
        float _chargeStartTime;
        Vector2 _chargeAimDir;
        bool _wasShootHeld;

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
            if (_equippedWeapon == null) return;

            if (_equippedWeapon.Behavior is ChargeShotBehaviorSO chargeBehavior) {
                UpdateChargeFire(chargeBehavior);
                return;
            }

            UpdateStandardFire();
        }

        void UpdateStandardFire() {
            if (!_input.ShootHeld) return;
            if (Time.time < _nextFireTime) return;

            Vector2 dir = _input.ShootInput.normalized;
            if (dir.sqrMagnitude < 0.0001f) return;

            Spawn(dir);

            _nextFireTime = Time.time + 1f / Mathf.Max(0.01f, _equippedWeapon.FireRate);
        }

        void UpdateChargeFire(ChargeShotBehaviorSO behavior) {
            bool shootHeld = _input.ShootHeld;
            Vector2 aimDir = _input.ShootInput;

            if (shootHeld) {
                if (aimDir.sqrMagnitude > 0.0001f) {
                    if (!_isCharging) {
                        _chargeStartTime = Time.time;
                        _isCharging = true;
                    }
                    _chargeAimDir = aimDir.normalized;
                }
            } else if (_wasShootHeld && _isCharging) {
                ReleaseChargeShot(behavior);
                _isCharging = false;
            }

            _wasShootHeld = shootHeld;
        }

        void ReleaseChargeShot(ChargeShotBehaviorSO behavior) {
            if (_chargeAimDir.sqrMagnitude < 0.0001f) return;

            float chargeTime = Mathf.Min(Time.time - _chargeStartTime, behavior.MaxChargeTime);
            bool isTap = chargeTime < behavior.MinChargedHoldSeconds;

            if (isTap) {
                if (Time.time < _nextFireTime) return;
                Spawn(_chargeAimDir);
                _nextFireTime = Time.time + 1f / Mathf.Max(0.01f, _equippedWeapon.FireRate);
            } else {
                float charge01 = chargeTime / behavior.MaxChargeTime;
                float chargeMult = Mathf.Lerp(1f, behavior.FullChargeDamageMultiplier, charge01);
                Spawn(_chargeAimDir, behavior.ChargedProjectileConfig, chargeMult);
                // Charged releases bypass _nextFireTime check.
            }
        }

        void Spawn(Vector2 aimDir, ProjectileConfigSO configOverride = null, float chargeDamageMult = 1f) {
            _spawnDirections.Clear();

            foreach (var spawnMod in GetComponents<IProjectileSpawnModifier>()) {
                spawnMod.ContributeDirections(aimDir, _spawnDirections);
            }

            if (_spawnDirections.Count == 0) {
                _spawnDirections.Add(aimDir);
            }

            for (int i = 0; i < _spawnDirections.Count; i++) {
                SpawnProjectile(_spawnDirections[i], configOverride, chargeDamageMult);
            }
        }

        void SpawnProjectile(Vector2 dir, ProjectileConfigSO configOverride = null, float chargeDamageMult = 1f) {
            if (dir.sqrMagnitude < 0.0001f) return;
            dir = dir.normalized;

            Vector3 spawnPos = transform.position + (Vector3)(dir * muzzleOffset);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Projectile proj = Instantiate(
                projectileShellPrefab,
                spawnPos,
                Quaternion.Euler(0f, 0f, angle)
            );

            ProjectileConfigSO config = configOverride ?? _equippedWeapon.ProjectileConfig;
            ProjectileConfigSnapshot snapshot = config.ToSnapshot();

            // Apply all modifiers to snapshot
            foreach (var mod in GetComponents<IProjectileModifier>()) {
                mod.Modify(ref snapshot);
            }

            proj.Configure(snapshot);

            float statMult = _stats != null ? _stats.DamageMultiplier : 1f;
            float dmg = snapshot.Damage * _equippedWeapon.DamageMultiplier * statMult * chargeDamageMult;
            proj.Launch(
                dir * snapshot.Speed,
                dmg,
                snapshot.Lifetime,
                snapshot.KnockbackForce
            );
        }

        public void EquipWeapon(WeaponSO newWeapon) {
            _isCharging = false;
            _wasShootHeld = false;
            _nextFireTime = 0f;
            if (newWeapon == null) {
                Debug.LogError("PlayerShooter: EquipWeapon called with null.", this);
                return;
            }
            if (newWeapon.ProjectileConfig == null) {
                Debug.LogError($"PlayerShooter: weapon {newWeapon.DisplayName} has no projectileConfig", this);
                return;
            }
            if (newWeapon.Behavior is ChargeShotBehaviorSO charge && charge.ChargedProjectileConfig == null) {
                Debug.LogError($"PlayerShooter: ChargeShot weapon {newWeapon.DisplayName} missing chargedProjectileConfig.", this);
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

        public float GetEffectiveShotDamage() {
            if (_equippedWeapon == null || _equippedWeapon.ProjectileConfig == null) return 1f;
            float statMult = _stats != null ? _stats.DamageMultiplier : 1f;
            return _equippedWeapon.ProjectileConfig.Damage * _equippedWeapon.DamageMultiplier * statMult;
        }
    }
}