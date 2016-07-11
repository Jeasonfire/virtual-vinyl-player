using UnityEngine;

public class ButtonActAutomate : Activatable {
    public RecordPlayer player;
    public Button armUpButton;

    public override void Activate() {
        armUpButton.locked = true;
        armUpButton.activated = false;
        //player.animator.SetAutomated(true);
    }

    public override void Deactivate() {
        armUpButton.locked = false;
        //player.animator.SetAutomated(false);
    }
}
