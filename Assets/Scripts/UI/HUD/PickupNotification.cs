using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Elenor;

namespace Elenor.UI.HUD {
    public class PickupNotification : PickupInventoryListener {
        [SerializeField] CanvasGroup group;
        [SerializeField] Image icon;
        [SerializeField] TMP_Text titleText;
        [SerializeField] TMP_Text flavorText;
        [SerializeField] float fadeIn = 0.2f;
        [SerializeField] float hold = 4.5f;
        [SerializeField] float fadeOut = 0.5f;

        Coroutine _routine;

        void Awake() {
            if (group != null) group.alpha = 0f;
        }

        protected override void OnPickupAcquired(PickupSO pickup, int level) {
            Show(pickup, $"{pickup.DisplayName} - Lv {level}", pickup.FlavorText);
        }

        protected override void OnPickupLeveledUp(PickupSO pickup, int level) {
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

        // TODO: This module should not be responsible for fading etc.
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