namespace Elenor {
    public class Door : DoorBase {
        protected override void OnPlayerEntered() {
            if (RoomManager.Instance != null) {
                RoomManager.Instance.GoToNeighborInDirection(direction);
            }
        }
    }
}