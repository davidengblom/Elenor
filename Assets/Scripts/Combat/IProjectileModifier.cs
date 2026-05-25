namespace Elenor {
    public interface IProjectileModifier {
        void Modify(ref ProjectileConfigSnapshot snapshot);
    }
}