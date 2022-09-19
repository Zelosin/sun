using System.Collections;
using UnityEngine;

public class Fader : MonoBehaviour {
    
    public IEnumerator fadeIn(Color gameObjectColor, float duration = 0.7f) {
        float normalizedTime = 0;
        while(normalizedTime <= 1f) {
            normalizedTime += Time.deltaTime / duration;
            gameObjectColor = new Color(0, 0, 0, normalizedTime);
            yield return null;
        }
    }
    
    public IEnumerator fadeOut(Color gameObjectColor, float duration = 0.7f) {
        float normalizedTime = 0;
        while(normalizedTime <= 1f) {
            normalizedTime += Time.deltaTime / duration;
            gameObjectColor = new Color(0, 0, 0,  1 - normalizedTime);
            yield return null;
        }
    }
}