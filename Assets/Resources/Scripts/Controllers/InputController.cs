using UnityEngine;

public class InputController : MonoBehaviour {
    [SerializeField] private GameObject historyView;
    [SerializeField] private GameObject settingsView;
    [SerializeField] private ReadController readController;

    [SerializeField] private RectTransform historyButton;
    [SerializeField] private RectTransform settingsButton;
    [SerializeField] private RectTransform historyExitButton;
    [SerializeField] private RectTransform settingsExitButton;

    private void Update() {
        if (Input.GetButtonDown(ControlStore.INTERACT_BUTTON)) {
            if (RectTransformUtility.RectangleContainsScreenPoint(historyExitButton, Input.mousePosition) || 
                RectTransformUtility.RectangleContainsScreenPoint(settingsExitButton, Input.mousePosition)) {
                GlobalClientStatus.gameStatus = GlobalGameStatus.IN_GAME;
                historyView.SetActive(false);
                settingsView.SetActive(false);
                return;
            }

            if (GlobalClientStatus.gameStatus == GlobalGameStatus.IN_GAME) {
                if (RectTransformUtility.RectangleContainsScreenPoint(historyButton, Input.mousePosition)) {
                    GlobalClientStatus.gameStatus = GlobalGameStatus.IN_HISTORY;
                    historyView.SetActive(true);
                    return;
                }

                if (RectTransformUtility.RectangleContainsScreenPoint(settingsButton, Input.mousePosition)) {
                    GlobalClientStatus.gameStatus = GlobalGameStatus.IN_SETTINGS;
                    settingsView.SetActive(true);
                    return;
                }

                if (GlobalClientStatus.readStatus == GlobalReadStatus.IN_SPEECH)
                    readController.showCurrentTextImmediately();
                else readController.loadChunkAndUpdateText();
            }
        }


        if (Input.GetButtonDown(ControlStore.HISTORY_BUTTON)) {
            if (GlobalClientStatus.gameStatus == GlobalGameStatus.IN_HISTORY) {
                GlobalClientStatus.gameStatus = GlobalGameStatus.IN_GAME;
                historyView.SetActive(false);
            }
            else if (GlobalClientStatus.gameStatus == GlobalGameStatus.IN_GAME) {
                GlobalClientStatus.gameStatus = GlobalGameStatus.IN_HISTORY;
                historyView.SetActive(true);
            }
        }

        if (Input.GetKeyDown(ControlStore.SETTINGS_KEY)) {
            if (GlobalClientStatus.gameStatus == GlobalGameStatus.IN_SETTINGS) {
                GlobalClientStatus.gameStatus = GlobalGameStatus.IN_GAME;
                settingsView.SetActive(false);
            }
            else if (GlobalClientStatus.gameStatus == GlobalGameStatus.IN_GAME) {
                GlobalClientStatus.gameStatus = GlobalGameStatus.IN_SETTINGS;
                settingsView.SetActive(true);
            }
        }
    }
}