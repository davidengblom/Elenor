using UnityEngine;

namespace Elenor {
    public class Door : DoorBase {
        static readonly Color WeaponRoomColor = new Color(0.047f, 0.902f, 0.949f, 1f); // #0ce6f2
        static readonly Color ModifierRoomColor = Color.white;
        protected override void OnPlayerEntered() {
            if (RoomManager.Instance != null) {
                RoomManager.Instance.GoToNeighborInDirection(direction);
            }
        }

        public void SetNeighborRoomType(RoomType type) {
            if (!TryGetComponent<SpriteRenderer>(out var sr)) return;
            sr.color = type switch {
                RoomType.WeaponRoom => WeaponRoomColor,
                RoomType.ModifierRoom => ModifierRoomColor,
                _ => sr.color,
            };
        }
    }
}