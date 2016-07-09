using UnityEngine;

public class ButtonActPlay : Activatable {
    public RecordPlayer player;
    
    public override void Activate() {
        player.animator.Play();
    }

    public override void Deactivate() {
        player.animator.Stop();
    }
}
