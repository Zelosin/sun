using System.Collections;
using Ink.Runtime;
using TMPro;
using UnityEngine;

public class ReadController : MonoBehaviour {
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private TMP_Text historyText;
    [SerializeField] private NetworkController networkController;
    private Story dialogStory;

    private Fader fader;
    private bool isDialogStarted = false;
    private bool isFadeInInterrapter = false;

    private Coroutine speechCoroutine;

    private void Awake() {
        historyText.text = string.Empty;

        fader = GetComponent<Fader>();
        networkController.parEvent += (sender, arg) => {
            switch (arg) {
                case TagStore.CONNECTED:
                    networkController.sendMessage(TagStore.HAND_SHAKE);
                    GlobalClientStatus.IS_PAR_CONNECTED = true;
                    Debug.Log(arg);
                    break;
                case TagStore.HAND_SHAKE:
                    GlobalClientStatus.IS_PAR_CONNECTED = true;
                    Debug.Log(arg);
                    break;
                case TagStore.DUO:
                    JobExecutor.wkr.addJob(() => loadChunkAndUpdateText(true));
                    break;
                case TagStore.BLOCK_PAR:
                    GlobalClientStatus.IS_BLOCKED = true;
                    break;
                case TagStore.RELEASE_PAR:
                    GlobalClientStatus.IS_BLOCKED = false;
                    break;
                case TagStore.RELEASE_PAR_LIM:
                    JobExecutor.wkr.addJob(() => loadChunkAndUpdateText(true));
                    GlobalClientStatus.IS_BLOCKED = true;
                    break;
            }
        };
    }

    private void Start() {
        dialogStory = new Story(textAsset.text);
        loadChunkAndUpdateText();
        StartCoroutine(fader.fadeIn(dialogText.color));
    }

    private void checkTags() {
        if (dialogStory.currentTags.Contains(TagStore.BLOCK_SELF)) GlobalClientStatus.IS_BLOCKED = true;
        if (dialogStory.currentTags.Contains(TagStore.BLOCK_PAR)) networkController.sendMessage(TagStore.BLOCK_PAR);
        if (dialogStory.currentTags.Contains(TagStore.RELEASE_PAR)) networkController.sendMessage(TagStore.RELEASE_PAR);
        if (dialogStory.currentTags.Contains(TagStore.RELEASE_PAR_LIM))
            networkController.sendMessage(TagStore.RELEASE_PAR_LIM);

        if (dialogStory.currentTags.Contains(TagStore.WAIT_PAR)) GlobalClientStatus.IS_PAR_CONNECTED = false;
        if (dialogStory.currentTags.Contains(TagStore.DUO)) networkController.sendMessage(TagStore.DUO);
    }

    public void showCurrentTextImmediately() {
        StopCoroutine(speechCoroutine);
        GlobalClientStatus.readStatus = GlobalReadStatus.RED;
        dialogText.text = dialogStory.currentText;
    }

    public void loadChunkAndUpdateText(bool isForceCall = false) {
        if (dialogStory.canContinue &&
            ((!GlobalClientStatus.IS_BLOCKED && GlobalClientStatus.IS_PAR_CONNECTED &&
              GlobalClientStatus.gameStatus == GlobalGameStatus.IN_GAME) || isForceCall)) {
            var nextText = dialogStory.Continue();
            if (!isForceCall) checkTags();
            dialogText.text = string.Empty;
            historyText.text += nextText + "\n";
            GlobalClientStatus.readStatus = GlobalReadStatus.IN_SPEECH;
            speechCoroutine = StartCoroutine(buildText(nextText, dialogText));
        }
        else {
            endDialog();
        }
    }

    private IEnumerator buildText(string text, TMP_Text textComponent) {
        for (var i = 0; i < text.Length; i++) {
            textComponent.text = string.Concat(textComponent.text, text[i]);
            yield return new WaitForSeconds(1 - SettingsStore.SPEECH_SPEED / 100);
        }

        GlobalClientStatus.readStatus = GlobalReadStatus.RED;
    }

    private void endDialog() {
        StartCoroutine(fader.fadeOut(dialogText.color));
    }
}