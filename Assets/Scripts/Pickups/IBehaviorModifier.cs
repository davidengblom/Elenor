namespace Elenor {
    /// <summary>
    /// Marker interface for any pickup-derived modifier component on the player.
    /// </summary>
    public interface IBehaviorModifier {
        int Level { get; set; }
    }
}