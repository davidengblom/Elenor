using UnityEngine;
using System.Collections;

namespace Elenor {
    public class EnemyPoisonStatus : MonoBehaviour {
        EnemyHealth _health;
        SpriteRenderer _sprite;
        Color _baseColor;

        float _dps;
        float _expiresAt;
        float _nextTick;
        bool _spawnCloudOnDeath;
        float _cloudRadius;
        float _cloudDuration;
        PoisonCloud _cloudPrefab;
        Coroutine _tickRoutine;

        void Awake() {
            _health = GetComponent<EnemyHealth>();
            _sprite = GetComponent<SpriteRenderer>();
        }

        void Start() {
            if (_sprite != null) _baseColor = _sprite.color;
            if (_health != null) _health.Died += OnDied;
        }

        void OnDestroy() {
            if (_health != null) _health.Died -= OnDied;
        }

        public void Apply(
            float dps,
            float duration,
            bool spawnCloudOnDeath,
            float cloudRadius,
            float cloudDuration,
            PoisonCloud cloudPrefab
        ) {
            if (dps <= 0f || duration <= 0f || _health == null || !_health.IsAlive) return;

            _dps = dps;
            _expiresAt = Time.time + duration;
            _spawnCloudOnDeath = spawnCloudOnDeath;
            _cloudRadius = cloudRadius;
            _cloudDuration = cloudDuration;
            _cloudPrefab = cloudPrefab;

            if (_tickRoutine == null) {
                _tickRoutine = StartCoroutine(TickRoutine());
            }

            if (_sprite != null) {
                _sprite.color = new Color(0.6f, 1f, 0.5f, _baseColor.a);
            }
        }

        IEnumerator TickRoutine() {
            _nextTick = Time.time + 1f;

            while (_health != null && _health.IsAlive && Time.time < _expiresAt) {
                if (Time.time >= _nextTick) {
                    _health.TakeDamage(_dps);
                    _nextTick = Time.time + 1f;
                }
                yield return null;
            }

            ClearVisuals();
            _tickRoutine = null;
        }

        void OnDied() {
            if (!_spawnCloudOnDeath || _cloudPrefab == null) return;
            if (Time.time > _expiresAt) return;

            PoisonCloud cloud = Instantiate(_cloudPrefab, transform.position, Quaternion.identity);
            cloud.Init(_dps, _cloudDuration, _cloudRadius);
        }

        void ClearVisuals() {
            if (_sprite != null) _sprite.color = _baseColor;
        }
    }
}