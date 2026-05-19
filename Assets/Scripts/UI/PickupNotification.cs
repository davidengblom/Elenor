using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Elenor {
    public class PickupNotification : MonoBehaviour {
        [SerializeField] CanvasGroup group;
        [SerializeField] Image icon;
        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text flavorText;
        [SerializeField] float fadeIn = 0.2f;
        [SerializeField] float hold = 2.5f;
        [SerializeField] float fadeOut = 0.5f;

        PlayerPickupInventory _inventory;
        Coroutine _routine;

        void Awake() {
            if (group != null) group.alpha = 0f;
        }

        // TODO: Isn't this literally the same code as the Start function in PickupIconRow?
        void Start() {
            Transform player = PlayerLocator.Player;
            if (player == null) return;
            _inventory = player.GetComponent<PlayerPickupInventory>();
            if (_inventory == null) return;

            _inventory.PickupAcquired += OnAcquired;
            _inventory.PickupLeveledUp += OnLeveledUp;
        }

        void OnDestroy() {
            if (_inventory == null) return;
            _inventory.PickupAcquired -= OnAcquired;
            _inventory.PickupLeveledUp -= OnLeveledUp;
        }

        void OnAcquired(PickupSO pickup, int level) {
            Show(pickup, $"{pickup.DisplayName}", pickup.FlavorText);
        }

        void OnLeveledUp(PickupSO pickup, int level) {
            Show(pickup, $"{pickup.DisplayName} - Lv {level}", pickup.FlavorText);
        }

        void Show(PickupSO pickup, string title, string flavor) {
            if (icon != null) {
                icon.sprite = pickup.Sprite;
                icon.color = pickup.DisplayColor;
            }
            if (titleText != null) titleText.text = title;
            if (flavorText != null) flavorText.text = flavor;

            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(Run());
        }

        // TODO: Temp solution. Use something in engine instead probablyy
        IEnumerator Run() {
            yield return Fade(0f, 1f, fadeIn);
            yield return new WaitForSeconds(hold);
            yield return Fade(1f, 0f, fadeOut);
            _routine = null;
        }

        IEnumerator Fade(float from, float to, float duration) {
            if (group == null || duration <= 0f) {
                if (group != null) group.alpha = to;
                yield break;
            }
            float t = 0f;
            while (t < duration) {
                t += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }
            group.alpha = to;
        }
    }
}