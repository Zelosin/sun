
using System;
using UnityEngine;

public class MouseInputController : MonoBehaviour
{

    [SerializeField] private ReadController readController;

    private void Update() {
        if (Input.GetButtonDown(ControlStore.INTERACT_BUTTON)) {
            readController.loadChunkAndUpdateText();
        }
    }

}
