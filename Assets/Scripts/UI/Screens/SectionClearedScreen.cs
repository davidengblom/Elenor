using UnityEngine;
using TMPro;
using Elenor;

namespace Elenor.UI.Screens {
    public class SectionClearedScreen : GameScreen {
        [Header("Stats")]
        [SerializeField] TMP_Text timeText;
        [SerializeField] TMP_Text damageText;
        [SerializeField] TMP_Text upgradesText;

        protected override void Start() {
            base.Start();
            if (RunManager.Instance != null) {
                RunManager.Instance.SectionCompleted += OnSectionCompleted;
            } else {
                Debug.LogWarning("SectionClearedScreen: no RunManager found.", this);
            }
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if (RunManager.Instance != null) {
                RunManager.Instance.SectionCompleted -= OnSectionCompleted;
            }
        }

        void OnSectionCompleted() {
            PopulateStats();
            Show();
        }

        void PopulateStats() {
            if (timeText != null) {
                float t = RunStats.Instance?.ElapsedTime ?? 0f;
                int minutes = Mathf.FloorToInt(t / 60f);
                int seconds = Mathf.FloorToInt(t % 60f);
                timeText.text = $"Time: {minutes:00}:{seconds:00}";
            }

            if (damageText != null) {
                int hits = RunStats.Instance?.HitsTaken ?? 0;
                float damage = RunStats.Instance?.DamageTaken ?? 0f;
                damageText.text = $"Hits taken: {hits} ({damage:0.0} damage)";
            }

            if (upgradesText != null) {
                Transform player = PlayerLocator.Player;
                if (player != null) {
                    var health = player.GetComponent<PlayerHealth>();
                    float maxHP = health?.MaxHealth ?? 0f;
                    upgradesText.text = $"Max HP: {maxHP:0}";
                } else {
                    upgradesText.text = "Upgrades: --";
                }
            }
        }
    }
}