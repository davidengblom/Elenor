using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(PlayerHealth))]
    public class ShieldModifier : MonoBehaviour, IBehaviorModifier, IPlayerDamageInterceptor {
        public int Level { get; set; }

        [SerializeField, Tooltip("Optional. Automatically found from child named ShieldVisual if unset.")]
        GameObject shieldVisual;

        PlayerHealth _health;

        int _maxHits = 1;
        int _currentHits;
        float _regenSeconds = 12f;
        float _regenTimer;
        bool _isRegenerating;
        bool _initialized;

        public bool HasCharges => _currentHits > 0;

        void Awake() {
            _health = GetComponent<PlayerHealth>();
            if (shieldVisual == null) {
                Transform t = transform.Find("ShieldVisual"); //TODO: There has to be a better way to do this no?
                if (t != null) shieldVisual = t.gameObject;
            }
        }

        public void Configure(int maxHits, float regenSeconds) {
            int oldMax = _maxHits;
            _maxHits = Mathf.Max(1, maxHits);
            _regenSeconds = Mathf.Max(0.01f, regenSeconds);

            if (_isRegenerating) {
                _regenTimer = Mathf.Min(_regenTimer, _regenSeconds);
            }

            if (!_initialized) {
                _currentHits = _maxHits;
                _isRegenerating = false;
                _regenTimer = 0f;
                _initialized = true;
            } else if (_maxHits > oldMax && _currentHits > 0) {
                // L3 upgrade, extra charge
                _currentHits = Mathf.Min(_currentHits + (_maxHits - oldMax), _maxHits);
            }

            UpdateVisual();
        }

        void Update() {
            if (!_isRegenerating) return;
            if (Time.timeScale <= 0f) return;
            if (RoomManager.Instance != null && RoomManager.Instance.IsRoomTransitioning) return;

            _regenTimer -= Time.deltaTime;
            if (_regenTimer <= 0f) RestoreShield();
        }

        public bool TryInterceptDamage(float amount, DamageSource source, out bool grantMiniInvuln) {
            grantMiniInvuln = false;
            if (_currentHits <= 0) return false;

            _currentHits--;
            if (_currentHits <= 0) {
                grantMiniInvuln = true;
                BeginRegen();
            }

            UpdateVisual();
            return true;
        }

        void BeginRegen() {
            _isRegenerating = true;
            _regenTimer = _regenSeconds;
            UpdateVisual();
        }

        void RestoreShield() {
            _isRegenerating = false;
            _regenTimer = 0f;
            _currentHits = _maxHits;
            UpdateVisual();
        }

        void UpdateVisual() {
            if (shieldVisual != null) {
                shieldVisual.SetActive(HasCharges);
            }
        }
    }
}