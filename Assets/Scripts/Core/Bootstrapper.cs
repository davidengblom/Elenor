using UnityEngine;
using UnityEngine.SceneManagement;

namespace Elenor {
    public class Bootstrapper : MonoBehaviour {
        [SerializeField] string gameSceneName = "Game";

        void Awake() {
            Time.timeScale = 1f;
        }

        void Start() {
            SceneManager.LoadScene(gameSceneName);
        }
    }
}