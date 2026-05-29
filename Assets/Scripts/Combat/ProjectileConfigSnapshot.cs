using UnityEngine;

namespace Elenor {
    public struct ProjectileConfigSnapshot {
        public Sprite Sprite;
        public Color Color;
        public float Scale;
        public float Speed;
        public float Damage;
        public float Lifetime;
        public float KnockbackForce;
        public bool Pierce;
        public int MaxPierceCount;
    
        public bool ApplyPoison;
        public float PoisonDps;
        public float PoisonDuration;
    }
}
