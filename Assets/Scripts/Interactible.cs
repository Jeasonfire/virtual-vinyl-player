using UnityEngine;

public abstract class Interactible : MonoBehaviour {
    public abstract void Interact();
    public abstract void StopInteracting();
    public abstract void StartInteracting();
}
