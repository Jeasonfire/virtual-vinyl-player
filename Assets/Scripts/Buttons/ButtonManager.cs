using UnityEngine;
using System.Collections;

public class ButtonManager : MonoBehaviour {
    private bool ignore = false;

    void Update() {
        if (ignore) {
            if (!Input.GetButton("Action (Primary)")) {
                ignore = false;
            } else {
                return;
            }
        }

        if (Input.GetButton("Action (Primary)")) {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(mouseRay, out hit);
            Button button = null;
            if (hit.collider != null && (button = hit.collider.GetComponent<Button>()) != null && !button.locked) {
                button.Toggle();
                ignore = true;
            }
        }
    }
}
