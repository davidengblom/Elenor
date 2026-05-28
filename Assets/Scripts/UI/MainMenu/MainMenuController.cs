using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Elenor;

namespace Elenor.UI.MainMenu {
    public class MainMenuController : MonoBehaviour {
        [SerializeField] Button startRunButton;
        [SerializeField] Button quitButton;
        [SerializeField] string gameSceneName = "Game";

        void Awake() {
            Time.timeScale = 1f;
        }

        void Start() {
            if (startRunButton != null) {
                startRunButton.onClick.AddListener(OnStartRunClicked);
            } else {
                Debug.LogWarning("MainMenuController: no startRunButton assigned.", this);
            }

            if (quitButton != null) {
                quitButton.onClick.AddListener(OnQuitClicked);
            } else {
                Debug.LogWarning("MainMenuController: no quitButton assigned.", this);
            }
        }

        void OnDestroy() {
            if (startRunButton != null) {
                startRunButton.onClick.RemoveListener(OnStartRunClicked);
            }
            if (quitButton != null) {
                quitButton.onClick.RemoveListener(OnQuitClicked);
            }
        }

        void OnStartRunClicked() {
            SceneManager.LoadScene(gameSceneName);
        }

        void OnQuitClicked() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}