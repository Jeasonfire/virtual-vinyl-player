using UnityEngine;
using System.Collections;

public class ButtonActRewind : Activatable {
    public RecordPlayer player;
    public Button parentButton;

    public override void Activate() {
        player.animator.Rewind();
        parentButton.activated = false;
    }

    public override void Deactivate() {
    }
}
