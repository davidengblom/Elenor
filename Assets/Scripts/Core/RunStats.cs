using UnityEngine;

namespace Elenor {
    public class RunStats : MonoBehaviour {
        public static RunStats Instance { get; private set; }

        public float ElapsedTime => Time.time - _runStartTime;
        public int HitsTaken { get; private set; }
        public float DamageTaken { get; private set; }

        PlayerHealth _health;
        float _lastHealth;
        float _runStartTime;

        void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void OnDestroy() {
            if (_health != null) _health.HealthChanged -= OnHealthChanged;
            if (Instance == this) Instance = null;
        }

        void Start() {
            _runStartTime = Time.time;
            Transform player = PlayerLocator.Player;
            if (player != null) _health = player.GetComponent<PlayerHealth>();

            if (_health != null) {
                _lastHealth = _health.Current;
                _health.HealthChanged += OnHealthChanged;
            } else {
                Debug.LogWarning("RunStats: no PlayerHealth found.", this);
            }
        }

        void OnHealthChanged(float current, float max) {
            float delta = _lastHealth - current;
            if (delta > 0f) {
                DamageTaken += delta;
                HitsTaken++;
            }
            _lastHealth = current;
        }
    }
}