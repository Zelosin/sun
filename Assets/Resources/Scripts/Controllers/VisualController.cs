using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VisualController : MonoBehaviour {
    private IEnumerator fade(Image imageToFade, bool isFadeIn = true, float speed = 1f, bool isCycle = false) {
        
        var targetAlpha = isFadeIn ? 1.0f : 0f;
        Color curColor = imageToFade.color;
        curColor.a = isFadeIn ? 0 : 1;
        
        while(Mathf.Abs(curColor.a - targetAlpha) > 0.01f) {
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, speed * Time.deltaTime);
            imageToFade.color = curColor;
            yield return null;
        }

        curColor.a = targetAlpha;
        imageToFade.color = curColor;

        if (isCycle) StartCoroutine(fade(imageToFade, !isFadeIn, speed));
    }

    public void fadeInEffect(Image imageToFade, float speed = 1f, bool isCycle = false) {
        StartCoroutine(fade(imageToFade, true, speed, isCycle));
    }

    public void fadeOutEffect(Image imageToFade, float speed = 1f, bool isCycle = false) {
        StartCoroutine(fade(imageToFade, false, speed, isCycle));
    }
}