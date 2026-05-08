using UnityEngine;

namespace Elenor {
    /// <summary>
    /// Anything that can take damage. Implementers are responsible for their own
    /// invulnerability windows, death handling and event firing.
    /// </summary>
    public interface IDamageable {
        /// <summary>
        /// Apply damage to this object. Implementers may early-out if already
        /// dead, currently invulnerable or the amount is zero or negative.
        /// </summary>
        /// <param name="amount">Damage points to apply.</param>
        /// <param name="hitImpulse">
        void TakeDamage(float amount, Vector2 hitImpulse = default);
    }
}
