using UnityEngine;

public class QuitGameOnKeypress : MonoBehaviour {
    public KeyCode key = KeyCode.Escape;

    private void Update() {
        if (Input.GetKeyDown(key)) Application.Quit();
    }
}