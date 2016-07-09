using UnityEngine;

public class ButtonActArmController : Activatable {
    public RecordPlayer player;

    public override void Activate() {
        player.animator.SetArmUp();
    }

    public override void Deactivate() {
        player.animator.SetArmDown();
    }
}
