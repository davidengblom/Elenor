using System.Collections;
using UnityEngine;
using System;

namespace Elenor {
    public class EnemyHealth : MonoBehaviour, IDamageable {
        [SerializeField] EnemySO data;
        [SerializeField, Tooltip("Fallback if no EnemySO is assigned.")]
        float fallbackMaxHealth = 3f;

        [Header("Hit Feedback")]
        [SerializeField, Min(0.01f)] float flashDuration = 0.08f;
        [SerializeField] Color flashColor = Color.white;
        [SerializeField, Min(0f), Tooltip("Seconds the mover is paused after a knockback hit.")]
        float knockbackStunDuration = 0.15f;

        float _current;
        RoomController _room;
        SpriteRenderer _sprite;
        Color _baseColor;
        Coroutine _flashRoutine;
        float _poisonDps;
        float _poisonExpiresAt;
        Coroutine _poisonRoutine;
        EnemyMover _mover;
        Rigidbody2D _rb;
        DamageSource _lastDamageSource;

        public float Current => _current;
        public bool IsAlive => _current > 0f;

        public event Action Died;

        void Awake() {
            Init(data);
            _sprite = GetComponent<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
            _mover = GetComponent<EnemyMover>();
        }

        void Start() {
            if (_sprite != null) _baseColor = _sprite.color;
        }

        public void Init(EnemySO so) {
            data = so;
            _current = so != null ? so.MaxHealth : fallbackMaxHealth;
        }

        void OnEnable() {
            _room = GetComponentInParent<RoomController>();
            if (_room != null) _room.RegisterEnemy(gameObject);
        }

        void OnDisable() {
            if (_room != null) _room.UnregisterEnemy(gameObject);
        }

        public void TakeDamage(float amount, Vector2 hitImpulse = default, DamageSource source = DamageSource.Unspecified) {
            if (!IsAlive || amount <= 0f) return;
            _lastDamageSource = source;
            _current = Mathf.Max(0f, _current - amount);

            if (_sprite != null) {
                if (_flashRoutine != null) StopCoroutine(_flashRoutine);
                _flashRoutine = StartCoroutine(FlashRoutine());
            }

            if (hitImpulse.sqrMagnitude > 0f && _rb != null) {
                _rb.AddForce(hitImpulse, ForceMode2D.Impulse);
                if (_mover != null && knockbackStunDuration > 0f) {
                    _mover.Stun(knockbackStunDuration);
                } 
            }

            if (_current <= 0f) Die();
        }

        public void ApplyPoison(float dps, float duration) {
            if (dps <= 0f || duration <= 0f || !IsAlive) return;
            _poisonDps = dps;
            _poisonExpiresAt = Time.time + duration;

            if (_poisonRoutine == null) {
                _poisonRoutine = StartCoroutine(PoisonTickRoutine());
            }

            if (_sprite != null) _sprite.color = new Color(0.6f, 1f, 0.5f, _baseColor.a);
        }

        IEnumerator PoisonTickRoutine() {
            float nextTick = Time.time + 1f;

            while (IsAlive && Time.time < _poisonExpiresAt) {
                if (Time.time >= nextTick) {
                    TakeDamage(_poisonDps, default, DamageSource.Poison);
                    nextTick = Time.time + 1f;
                }
                yield return null;
            }

            if (_sprite != null && IsAlive) _sprite.color = _baseColor;
            _poisonRoutine = null;
        }

        void Die() {
            ReportDirectKillIfApplicable();
            Died?.Invoke();
            if (Hitstop.Instance != null) Hitstop.Instance.Pulse();
            Destroy(gameObject);
        }

        void ReportDirectKillIfApplicable() {
            if (_lastDamageSource != DamageSource.PlayerProjectile && _lastDamageSource != DamageSource.PlayerDash) return;

            Transform player = PlayerLocator.Player;
            if (player == null) return;
            if (player.TryGetComponent<PlayerKillTracker>(out var tracker)) {
                tracker.ReportDirectKill(_lastDamageSource);
            }
        }

        IEnumerator FlashRoutine() {
            _sprite.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            if (_sprite != null) _sprite.color = _baseColor;
            _flashRoutine = null;
        }
    }
}