using UnityEngine;

namespace Elenor {
    public class Door : DoorBase {
        static readonly Color WeaponRoomColor = new Color(0.047f, 0.902f, 0.949f, 1f); // #0ce6f2
        static readonly Color ModifierRoomColor = Color.white;
        static readonly Color BossArenaColor = new Color(0.125f, 0.082f, 0.2f, 1f); // #201533

        protected override void OnPlayerEntered() {
            if (RoomManager.Instance != null) {
                RoomManager.Instance.GoToNeighborInDirection(direction);
            }
        }

        public void SetNeighborRoomType(RoomType type, bool neighborIsBossArena) {
            if (!TryGetComponent<SpriteRenderer>(out var sr)) return;

            if (neighborIsBossArena) {
                sr.color = BossArenaColor;
                return;
            }
            
            sr.color = type switch {
                RoomType.WeaponRoom => WeaponRoomColor,
                RoomType.ModifierRoom => ModifierRoomColor,
                _ => sr.color,
            };
        }
    }
}