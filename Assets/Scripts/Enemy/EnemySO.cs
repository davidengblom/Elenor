using UnityEngine;

namespace Elenor {
    [CreateAssetMenu(menuName = "Elenor/Enemy", fileName = "Enemy_New")]
    public class EnemySO : ScriptableObject {
        [SerializeField] float maxHealth = 3f;
        [SerializeField] WeaponSO weapon;

        public float MaxHealth => maxHealth;
        public WeaponSO Weapon => weapon;
    }
}