using TMPro;
using UnityEngine;

namespace Elenor {
    public class HUD : MonoBehaviour {
        [SerializeField] TMP_Text healthText;
        [SerializeField] TMP_Text damageText;
        [SerializeField] TMP_Text speedText;
        [SerializeField] TMP_Text roomsText;

        PlayerHealth _health;
        PlayerStats _stats;

        void Start() {
            Transform player = PlayerLocator.Player;
            if (player != null) {
                _health = player.GetComponent<PlayerHealth>();
                _stats = player.GetComponent<PlayerStats>();
            }

            if (_health != null) {
                _health.HealthChanged += OnHealthChanged;
                OnHealthChanged(_health.Current, _health.Max);
            }
            if (_stats != null) {
                _stats.StatsChanged += OnStatsChanged;
                OnStatsChanged();
            }
            if (RoomManager.Instance != null) {
                RoomManager.Instance.RoomsClearedChanged += OnRoomsChanged;
                OnRoomsChanged(RoomManager.Instance.RoomsCleared);
            }
        }

        void OnDestroy() {
            if (_health != null) _health.HealthChanged -= OnHealthChanged;
            if (_stats != null) _stats.StatsChanged -= OnStatsChanged;
            if (RoomManager.Instance != null) RoomManager.Instance.RoomsClearedChanged -= OnRoomsChanged;
        }

        void OnHealthChanged(float current, float max) {
            if (healthText != null) {
                healthText.text = $"HP: {current:0}/{max:0}";
            }
        }

        void OnStatsChanged() {
            if (_stats == null) return;
            if (damageText != null) {
                damageText.text = $"DMG: {_stats.DamageMultiplier:0.0}x";
            }
            if (speedText != null) {
                speedText.text = $"SPD: {_stats.MoveSpeed:0.0}";
            }
        }

        void OnRoomsChanged(int count) {
            if (roomsText != null) {
                roomsText.text = $"Rooms Cleared: {count}";
            }
        }
    }
}