using UnityEngine;

namespace Elenor {
    public class PoisonModifier : MonoBehaviour, IBehaviorModifier, IProjectileModifier {
        public int Level { get; set; }

        float _dps;
        float _duration;

        public void Configure(float dps, float duration) {
            _dps = dps;
            _duration = duration;
        }

        public void Modify(ref ProjectileConfigSnapshot snapshot) {
            if (_dps <= 0f || _duration <= 0f) return;
            snapshot.ApplyPoison = true;
            snapshot.PoisonDps = _dps;
            snapshot.PoisonDuration = _duration;
        }
    }
}