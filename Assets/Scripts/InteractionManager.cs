using UnityEngine;
using System.Collections;

public class InteractionManager : MonoBehaviour {
    public static bool interactionsAreAllowed = true;

    public const int RECORD_MANAGER_INTERACTION_ID = 0;
    public const int RECORD_PLAYER_INTERACTION_ID = 1;

    public Interactible[] interactibles;
    public float interactionCooldownPeriod;

    private float timeAtLastChange = 0;
    private int currentlyInteractingWith = 0;

    void Update() {
        if (!Input.GetButton("Action (Tertiary)")) {
            timeAtLastChange -= interactionCooldownPeriod;
        }
        interactionsAreAllowed = Time.time - timeAtLastChange > interactionCooldownPeriod;
        if (interactionsAreAllowed) {
            // Interaction state changes
            if (Input.GetButton("Action (Tertiary)") && interactionsAreAllowed) {
                timeAtLastChange = Time.time;
                GetCurrentInteractible().StopInteracting();
                currentlyInteractingWith = (currentlyInteractingWith + 1) % interactibles.Length;
                GetCurrentInteractible().StartInteracting();
            }

            // Updates
            GetCurrentInteractible().Interact();
        }
    }

    private Interactible GetCurrentInteractible() {
        return interactibles[currentlyInteractingWith];
    }
}
