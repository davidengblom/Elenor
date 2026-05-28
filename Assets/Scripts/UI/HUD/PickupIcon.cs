using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Elenor;

namespace Elenor.UI.HUD {
    public class PickupIcon : MonoBehaviour {
        [SerializeField] Image icon;
        [SerializeField] TMP_Text levelText;

        public void Bind(PickupSO pickup, int level) {
            if (pickup == null) return;
            if (icon != null) {
                icon.sprite = pickup.Sprite;
                icon.color = pickup.DisplayColor;
                icon.enabled = true;
            }
            UpdateLevel(level);
        }

        public void UpdateLevel(int level) {
            if (levelText != null) {
                levelText.text = $"Lv {level}";
            }
        }
    }
}