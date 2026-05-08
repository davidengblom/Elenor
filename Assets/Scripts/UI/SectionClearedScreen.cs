using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Elenor {
    public class SectionClearedScreen : MonoBehaviour {
        [Header("UI References")]
        [SerializeField] GameObject panel;
        [SerializeField] Button restartRunButton;
        [SerializeField] Button quitToMenuButton;
        [SerializeField] TMP_Text timeText;
        [SerializeField] TMP_Text damageText;
        [SerializeField] TMP_Text upgradesText;

        [Header("Scene Flow")]
        [SerializeField] string gameSceneName = "Game";
        [SerializeField] string mainMenuSceneName = "MainMenu";

        void Awake() {
            if (panel != null) panel.SetActive(false);
        }

        void Start() {
            if (RunManager.Instance != null) {
                RunManager.Instance.SectionCompleted += OnSectionCompleted;
            } else {
                Debug.LogWarning("SectionClearedScreen: no RunManager found.", this);
            }

            if (restartRunButton != null) {
                restartRunButton.onClick.AddListener(OnRestartClicked);
            } else {
                Debug.LogWarning("SectionClearedScreen: no restartButton assigned.", this);
            }
            if (quitToMenuButton != null) {
                quitToMenuButton.onClick.AddListener(OnQuitToMenuClicked);
            } else {
                Debug.LogWarning("SectionClearedScreen: no quitToMenuButton assigned.", this);
            }
        }

        void OnDestroy() {
            if (RunManager.Instance != null) {
                RunManager.Instance.SectionCompleted -= OnSectionCompleted;
            }
            if (restartRunButton != null) {
                restartRunButton.onClick.RemoveListener(OnRestartClicked);
            }
        }

        void OnSectionCompleted() {
            PopulateStats();
            if (panel != null) panel.SetActive(true);
            Time.timeScale = 0f; // Stop time
        }

        void PopulateStats() {
            if (timeText != null) {
                float t = RunStats.Instance != null ? RunStats.Instance.ElapsedTime : 0f;
                int minutes = Mathf.FloorToInt(t / 60f);
                int seconds = Mathf.FloorToInt(t % 60f);
                timeText.text = $"Time: {minutes:00}:{seconds:00}";
            }

            if (damageText != null) {
                int hits = RunStats.Instance != null ? RunStats.Instance.HitsTaken : 0;
                float damage = RunStats.Instance != null ? RunStats.Instance.DamageTaken : 0f;
                damageText.text = $"Hits taken: {hits} ({damage:0} damage)";
            }

            if (upgradesText != null) {
                Transform player = PlayerLocator.Player;
                if (player != null) {
                    var stats = player.GetComponent<PlayerStats>();
                    var health = player.GetComponent<PlayerHealth>();
                    float dmg = stats != null ? stats.DamageMultiplier : 1f;
                    float spd = stats != null ? stats.MoveSpeed : 0f;
                    float maxHP = health != null ? health.MaxHealth : 0f;
                    upgradesText.text = $"DMG: {dmg:0.0}x\nSPD: {spd:0.0}\nMax HP: {maxHP:0}";
                } else {
                    upgradesText.text = "Upgrades: --";
                }
            }
        }

        void OnRestartClicked() {
            Time.timeScale = 1f; // Resume time
            SceneManager.LoadScene(gameSceneName);
        }

        void OnQuitToMenuClicked() {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}