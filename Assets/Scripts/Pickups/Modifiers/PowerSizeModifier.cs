using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(PlayerStats))]
    public class PowerSizeModifier : MonoBehaviour, IBehaviorModifier {
        public int Level { get; set; }

        PlayerStats _stats;
        float _appliedDamageMultiplier = 1f;
        float _appliedSpeedMultiplier = 1f;
        float _appliedScaleMultiplier = 1f;

        void Awake() {
            _stats = GetComponent<PlayerStats>();
        }

        /// <summary>
        /// Apply the given multiplicative bonuses.
        /// </summary>
        public void Configure(float damageMultiplier, float speedMultiplier, float scaleMultiplier) {
            Revert();
            _appliedDamageMultiplier = Mathf.Max(0f, damageMultiplier);
            _appliedSpeedMultiplier = Mathf.Max(0f, speedMultiplier);
            _appliedScaleMultiplier = Mathf.Max(0f, scaleMultiplier);
            Apply();
        }

        void Revert() {
            if (_stats != null) {
                _stats.DamageMultiplier /= _appliedDamageMultiplier;
                _stats.MoveSpeed /= _appliedSpeedMultiplier;
            }
            transform.localScale /= _appliedScaleMultiplier;
        }

        void Apply() {
            if (_stats != null) {
                _stats.DamageMultiplier *= _appliedDamageMultiplier;
                _stats.MoveSpeed *= _appliedSpeedMultiplier;
            }
            transform.localScale *= _appliedScaleMultiplier;
        }
    }
}