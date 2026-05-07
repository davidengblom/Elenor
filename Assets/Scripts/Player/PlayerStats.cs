using System;
using UnityEngine;

namespace Elenor {
    /// <summary>
    /// Mutable player stats that pickups modify.
    /// </summary>
    public class PlayerStats : MonoBehaviour {
        [SerializeField] float damageMultiplier = 1f;
        [SerializeField] float moveSpeed = 6f;

        public event Action StatsChanged;

        public float DamageMultiplier {
            get => damageMultiplier;
            set {
                damageMultiplier = Mathf.Max(0f, value);
                StatsChanged?.Invoke();
            }
        }

        public float MoveSpeed {
            get => moveSpeed;
            set {
                moveSpeed = Mathf.Max(0f, value);
                StatsChanged?.Invoke();
            }
        }

        void Start() {
            StatsChanged?.Invoke();
        }
    }
}