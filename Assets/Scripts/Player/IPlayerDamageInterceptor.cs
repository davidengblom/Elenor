namespace Elenor {
    /// <summary>
    /// Optional player-side damage gate checked before HP loss and normal i-frame rejection.
    /// </summary>
    public interface IPlayerDamageInterceptor {
        /// <summary>
        /// Attempt to absorb incoming damage.
        /// </summary>
        /// <param name="grantMiniInvuln">True when the interceptor fully depleted.</param>
        /// <returns>True if HP should not be reduced.</returns>
        bool TryInterceptDamage(float amount, DamageSource source, out bool grantMiniInvuln);
    }
}