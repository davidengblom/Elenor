using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Elenor;

namespace Elenor.UI.HUD {
    public class StatPanel : MonoBehaviour {
        [SerializeField] TMP_Text healthText;
        [SerializeField] TMP_Text damageText;
        [SerializeField] TMP_Text speedText;
        [SerializeField] TMP_Text roomsText;
        [SerializeField] TMP_Text roomIndexText;

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
                RoomManager.Instance.RoomsClearedChanged += OnRoomsClearedChanged;
                RoomManager.Instance.RoomChanged += OnRoomChanged;
                OnRoomsClearedChanged(RoomManager.Instance.RoomsCleared);
                OnRoomChanged(RoomManager.Instance.CurrentGridPos);
            }
        }

        void OnDestroy() {
            if (_health != null) _health.HealthChanged -= OnHealthChanged;
            if (_stats != null) _stats.StatsChanged -= OnStatsChanged;
            if (RoomManager.Instance != null) {
                RoomManager.Instance.RoomsClearedChanged -= OnRoomsClearedChanged;
                RoomManager.Instance.RoomChanged -= OnRoomChanged;
            } 
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

        void OnRoomsClearedChanged(int count) {
            if (roomsText != null) {
                roomsText.text = $"Rooms Cleared: {count}";
            }
        }

        void OnRoomChanged(Vector2Int gridPos) {
            if (roomIndexText != null) {
                roomIndexText.text = $"Pos {gridPos.x},{gridPos.y}";
            }
        }
    }
}