using Unity.Cinemachine;
using UnityEngine;

namespace Elenor {
    [RequireComponent(typeof(CinemachineImpulseSource), typeof(PlayerHealth))]
    public class PlayerHitImpulse : MonoBehaviour {
        [SerializeField, Min(0.3f), Tooltip("Impulse strength multiplied by damage taken.")]
        float impulsePerDamage = 0.3f;

        CinemachineImpulseSource _impulse;
        PlayerHealth _health;

        void Awake() {
            _impulse = GetComponent<CinemachineImpulseSource>();
            _health = GetComponent<PlayerHealth>();
        }

        void OnEnable() {
            _health.Damaged += OnDamaged;
        }

        void OnDisable() {
            _health.Damaged -= OnDamaged;
        }

        void OnDamaged(float amount) {
            _impulse.GenerateImpulse(amount * impulsePerDamage);
        }
    }
}