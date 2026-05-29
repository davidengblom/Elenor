using UnityEngine;
using System.Collections.Generic;

namespace Elenor {
    [RequireComponent(typeof(PlayerDash))]
    [RequireComponent(typeof(PlayerShooter))]
    public class DashModModifier : MonoBehaviour, IBehaviorModifier {
        public int Level { get; set; }

        PlayerDash _dash;
        PlayerShooter _shooter;
        PlayerKillTracker _tracker;

        float _dashDamageMultiplier;
        float _durationMultiplier = 1f;
        bool _refundOnKill;

        float _currentDashDamage;
        readonly HashSet<EnemyHealth> _hitThisDash = new();
        CircleCollider2D _playerCollider;
        int _playerLayer;
        int _enemyLayer;
        readonly Collider2D[] _overlapResults = new Collider2D[16];
        ContactFilter2D _enemyFilter;

        void Awake() {
            _dash = GetComponent<PlayerDash>();
            _shooter = GetComponent<PlayerShooter>();
            _tracker = GetComponent<PlayerKillTracker>();
            if (_tracker == null) _tracker = gameObject.AddComponent<PlayerKillTracker>();
            _playerCollider = GetComponent<CircleCollider2D>();
            _playerLayer = gameObject.layer;
            _enemyLayer = LayerMask.NameToLayer("Enemy");
            _enemyFilter = new ContactFilter2D();
            _enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
            _enemyFilter.useTriggers = true;
        }

        void FixedUpdate() {
            if (!_dash.IsDashing || _currentDashDamage <= 0f) return;

            float scale = Mathf.Max(transform.localScale.x, transform.localScale.y);
            float radius = _playerCollider != null ? _playerCollider.radius * scale : 0.5f * scale;

            int count = Physics2D.OverlapCircle(
                transform.position,
                radius,
                _enemyFilter,
                _overlapResults
            );

            Vector2 impulse = _dash.Body.linearVelocity.sqrMagnitude > 0.0001f ? _dash.Body.linearVelocity.normalized * 2f : Vector2.zero;

            for (int i = 0; i < count; i++) {
                if (!_overlapResults[i].TryGetComponent<EnemyHealth>(out var enemy)) continue;
                if (_hitThisDash.Contains(enemy)) continue;

                _hitThisDash.Add(enemy);
                enemy.TakeDamage(_currentDashDamage, impulse, DamageSource.PlayerDash);
            }
        }

        void OnEnable() {
            _dash.DashStarted += OnDashStarted;
            _dash.DashEnded += OnDashEnded;
            _tracker.DirectKill += OnDirectKill;
        }

        void OnDisable() {
            if (_dash != null) {
                _dash.DashStarted -= OnDashStarted;
                _dash.DashEnded -= OnDashEnded;
            }
            if (_tracker != null) _tracker.DirectKill -= OnDirectKill;
            RestoreEnemyCollision();
        }

        void RestoreEnemyCollision() {
            if (_enemyLayer < 0) return;
            Physics2D.IgnoreLayerCollision(_playerLayer, _enemyLayer, false);
        }

        public void Configure(float dashDamageMultiplier, float durationMultiplier, bool refundOnKill) {
            _dashDamageMultiplier = Mathf.Max(0f, dashDamageMultiplier);
            _durationMultiplier = Mathf.Max(1f, durationMultiplier);
            _refundOnKill = refundOnKill;
            _dash.SetDurationMultiplier(_durationMultiplier);
        }

        void OnDashStarted() {
            _hitThisDash.Clear();
            _currentDashDamage = _shooter.GetEffectiveShotDamage() * _dashDamageMultiplier;

            if (_currentDashDamage > 0f && _enemyLayer >= 0) {
                Physics2D.IgnoreLayerCollision(_playerLayer, _enemyLayer, true);
            }
        }

        void OnDashEnded() {
            RestoreEnemyCollision();
            _hitThisDash.Clear();
        }

        void OnDirectKill(DamageSource source) {
            if (!_refundOnKill || !_dash.IsDashing) return;
            _dash.RefundCooldown();
        }
    }
}