namespace Elenor {
    public class ExitDoor : DoorBase {
        protected override void OnPlayerEntered() {
            if (RunManager.Instance != null) {
                RunManager.Instance.AdvanceFloor();
            }
        }
    }
}