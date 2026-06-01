using UnityEngine;

namespace Elenor {
    /// <summary>
    /// Generic base for levelable player modifiers.
    /// </summary>
    public abstract class LevelablePlayerModifierSO<TModifier, TLevelData> : PlayerModifierSO where TModifier : Component, IBehaviorModifier where TLevelData : struct {
        [SerializeField] protected TLevelData[] levels = new TLevelData[MaxLevel];

        public override System.Type ModifierComponentType => typeof(TModifier);

        public override void ConfigureModifier(IBehaviorModifier modifier, int level) {
            if (modifier is not TModifier typed) return;
            int index = Mathf.Clamp(level - 1, 0, levels.Length - 1);
            ApplyLevel(typed, levels[index]);
        }

        protected abstract void ApplyLevel(TModifier modifier, TLevelData data);

        protected virtual void OnValidate() {
            if (levels == null || levels.Length != MaxLevel) {
                System.Array.Resize(ref levels, MaxLevel);
            }
        }
    }
}