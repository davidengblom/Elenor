using UnityEngine;
using System;

namespace Elenor {
    /// <summary>
    /// Receives reports when the player gets a direct kill.
    /// </summary>
    public class PlayerKillTracker : MonoBehaviour {
        public event Action<DamageSource> DirectKill;

        public void ReportDirectKill(DamageSource source) {
            if (source != DamageSource.PlayerProjectile && source != DamageSource.PlayerDash) return;
            DirectKill?.Invoke(source);
        }
    }
}